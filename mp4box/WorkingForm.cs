// ------------------------------------------------------------------
// Copyright (C) 2011-2015 Maruko Toolbox Project
// 
//  Authors: komaruchan <sandy_0308@hotmail.com>
//           LunarShaddow <aflyhorse@hotmail.com>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
// express or implied.
// See the License for the specific language governing permissions
// and limitations under the License.
// -------------------------------------------------------------------
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace mp4box
{
    using Extension;
    using Win32API;
    public partial class WorkingForm : Form
    {
        /// <summary>
        /// Initialize a cmd output wrapper.
        /// </summary>
        /// <param name="commands">Proposing commands.
        ///     Expecting <see cref="Environment.NewLine"/> as line breaker.</param>
        /// <param name="workcount">The amount of roposing commands.
        ///     Single-pass work don't need to specify this parameter.</param>
        public WorkingForm(string commands, int workcount = 1)
        {
            InitializeComponent();
            Commands = commands;
            WorkQueued = workcount;
            this.Parent = this.Owner;
            StartPosition = FormStartPosition.CenterScreen;
            bgworker.DoWork += bgworker_DoWork;
            bgworker.WorkerReportsProgress = true;
            bgworker.ProgressChanged += bgworker_ProgressChanged;
            bgworker.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// Gets or sets the proposed commands. Expecting <see cref="Environment.NewLine"/> as line breaker.
        /// <exception cref="System.ArgumentException">
        ///     An exception is thrown if set it empty or null.</exception>
        /// </summary>
        public string Commands
        {
            get
            {
                return cmds;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("Null or empty commands string.");
                else
                    cmds = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of proposed works.
        /// <exception cref="System.ArgumentException">
        ///     An exception is thrown if set it zero of negative.</exception>
        /// </summary>
        public int WorkQueued
        {
            get
            {
                return workQueued;
            }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Negative number? Really?");
                else
                    workQueued = value;
            }
        }

        /// <summary>
        /// Makesure a file really exists.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <param name="arg">Supplement info.</param>
        /// <returns>The file exist or not.</returns>
        public static bool CheckFileExist(string path, string info = "")
        {
            if (!System.IO.File.Exists(path))
                MessageBox.Show("找不到 " + path + " 了啦" + Environment.NewLine
                    + "其他信息：" + info);
            return System.IO.File.Exists(path);
        }

        #region Private Members Declaration

        /// <summary>
        /// Proposed commands.
        /// </summary>
        private string cmds;

        /// <summary>
        /// Queued works quantity.
        /// </summary>
        private int workQueued;

        /// <summary>
        /// Completed works quantity.
        /// </summary>
        private int workCompleted;

        /// <summary>
        /// Path to the batch file.
        /// </summary>
        private string batPath;

        /// <summary>
        /// Potential working process.
        /// </summary>
        private System.Diagnostics.Process proc;

        /// <summary>
        /// Frame count of current processing file via FFmpeg.
        /// </summary>
        private int frameCount;

        /// <summary>
        /// Path to the tools directory.
        /// </summary>
        private string workPath;

        /// <summary>
        /// true if the system is Win7 or later
        /// </summary>
        private bool win7supported;

        /// <summary>
        /// true if autoscroll is enabled
        /// </summary>
        private bool autoscroll;

        /// <summary>
        /// Internal log, without progress Indicators
        /// </summary>
        private StringBuilder internellog = new StringBuilder();

        /// <summary>
        /// The one who invokes batch file.
        /// </summary>
        private BackgroundWorker bgworker = new BackgroundWorker();

        #endregion

        #region Regex Patterns
        /// <summary>
        /// Store const regex patterns.
        ///     The Regex objects are made to speed up matchup instead of
        ///     passing the pattern string as arguments from time to time.
        /// </summary>
        public static class Patterns
        {
            /// <summary>
            /// ffms splitter sample output:
            /// <para>[8.1%] 273/3357 frames, 56.22 fps, 17.99 kb/s, 25.01 KB, eta 0:00:54, est.size 307.54 KB</para>
            /// </summary>
            private const string ffmsRegStr = @"^\[(?<percent>\d+\.\d+)%\]";

            /// <summary>
            /// ffms splitter Regex.
            ///     Available patterns: percent.
            /// </summary>
            public static readonly Regex ffmsReg = new Regex(ffmsRegStr);

            /// <summary>
            /// lavf splitter sample output:
            /// <para>387 frames: 68.65 fps, 14.86 kb/s, 29.27 KB</para>
            /// </summary>
            private const string lavfRegStr = @"^(?<frame>\d+) frames:";

            /// <summary>
            /// lavf splitter Regex.
            ///     Available patterns: frame.
            /// </summary>
            public static readonly Regex lavfReg = new Regex(lavfRegStr);

            /// <summary>
            /// NeroAAC sample output:
            /// <para>Processed 10 seconds...</para>
            /// </summary>
            private const string neroRegStr = @"Processed (?<duration>\d+) seconds...";

            /// <summary>
            /// NeroAAC Progress Regex.
            ///     Available patterns: duration.
            /// </summary>
            public static readonly Regex neroReg = new Regex(neroRegStr);

            /// <summary>
            /// x264 execution sample output:
            /// <para>X:\toolDIR>"X:\toolDIR\tools\x264_32_tMod-8bit-420.exe" --crf 24 --preset 8 --demuxer ffms
            /// -r 3 --b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8
            /// -o "X:\workDIR\output.ext" "X:\workDIR\input.ext" --vf subtitles --sub "X:\workDIR\sub.ext"
            /// --acodec faac --abitrate 128</para>
            /// </summary>
            /// <remarks>
            /// Without double quote: ^.+>"(?<workDIR>[^"]+)\\x264.+ -o "(?<fileOut>[^"]+)" "(?<fileIn>[^"]+)"
            /// </remarks>
            private const string fileRegStr = @">""(?<workDIR>[^""]+)\\x264.+-o ""(?<fileOut>[^""]+)"" ""(?<fileIn>[^""]+)""";

            /// <summary>
            /// Filename and working directory Regex.
            ///     Available patterns: workDIR, fileOut, fileIn.
            /// </summary>
            public static readonly Regex fileReg = new Regex(fileRegStr, RegexOptions.Singleline);

            /// <summary>
            /// ffmpeg -i sample output:
            /// <para>Duration: 00:02:20.22, start: 0.000000, bitrate: 827 kb/s</para>
            /// <para>Stream #0:0: Video: h264 (High), yuv420p, 1280x720, 545 kb/s, 24.42 fps, 23.98 tbr, 1k tbn, 47.95 tbc</para>
            /// <para>Stream #0:1: Audio: aac, 48000 Hz, stereo, fltp, 128 kb/s</para>
            /// </summary>
            private const string ffmpegRegStr = @"\bDuration: (?<duration>\d{2}:\d{2}:\d{2}\.\d{2}), start: " +
                @".+: Video: .+ (?<tbr>\d+\.?\d+) tbr, ";

            /// <summary>
            /// ffmpeg output Regex.
            ///     Available patterns: duration, tbr.
            /// </summary>
            public static readonly Regex ffmpegReg = new Regex(ffmpegRegStr, RegexOptions.Singleline);
        }

        #endregion

        #region UI Methods

        private void WorkingForm_Load(object sender, EventArgs e)
        {
            // save commands into a batch file
            batPath = System.IO.Path.Combine(
                Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.User),
                "xiaowan" + DateTime.Now.ToFileTimeUtc().ToString() + ".bat");
            var encoder = Encoding.GetEncoding(0);
            var sw = new System.IO.StreamWriter(batPath, false, encoder);
            sw.WriteLine(Commands);
            sw.Close();
            // synchronize UI
            workCompleted = -1;
            richTextBoxOutput.Select();
            // check win7 supported
            win7supported = (Environment.OSVersion.Version.Major > 6 ||
                (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 1));
            if (win7supported)
            {
                taskbarProgress = (ITaskbarList3)new ProgressTaskbar();
                taskbarProgress.SetProgressState(this.Handle, TBPFLAG.TBPF_NORMAL);
            }
            // validate the command string
            if (Commands.Equals(encoder.GetString(encoder.GetBytes(Commands))))
                bgworker.RunWorkerAsync();
            else
            {
                MessageBox.Show("Path or filename contains invalid characters, please rename and retry."
                    + Environment.NewLine +
                    "路径或文件名含有不可识别的字符，请重命名后重试。");
                this.Close();
            }
            notifyIcon.Visible = true;
            MainForm main = (MainForm)this.Owner;
            workPath = main.workPath;
            autoscroll = true;
        }

        private void WorkingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // check remaining works
            if (!(proc == null || proc.HasExited))
            {
                // ask if user hit close button on any accident
                var result = MessageBox.Show("Processing is still undergoing, confirm exit?",
                    "Xiaowan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == System.Windows.Forms.DialogResult.No)
                {
                    // cancel event and return
                    e.Cancel = true;
                    return;
                }
                else
                    ProcAbort();
            }
            // clean up temp batch file
            System.IO.File.Delete(batPath);
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            bgworker.CancelAsync();
            ProcAbort();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // prohibit saving before work completion
            if (!proc.HasExited)
                MessageBox.Show("Saving before completion is not recommended." +
                    Environment.NewLine + "Please manually abort the process or wait patiently.");
            else
            {
                var savDlg = new SaveFileDialog();
                savDlg.FileName = "log_" + DateTime.Now.ToShortDateString().Replace('/', '-');
                savDlg.Filter = "Log files|*.log|All files|*.*";
                savDlg.FilterIndex = 1;
                savDlg.RestoreDirectory = true;
                if (savDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    System.IO.File.WriteAllText(savDlg.FileName, internellog.ToString(), Encoding.Default);
            }
        }

        private void richTextBoxOutput_VScroll(object sender, EventArgs e)
        {
            var info = new NativeMethods.SCROLLINFO();
            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            info.fMask = NativeMethods.ScrollBarFlags.SIF_ALL;
            NativeMethods.GetScrollInfo(richTextBoxOutput.Handle, NativeMethods.ScrollBarConsts.SB_VERT, ref info);
            autoscroll = (info.nTrackPos + richTextBoxOutput.Font.Height
                > info.nMax - richTextBoxOutput.ClientSize.Height);
        }

        private void WorkingForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                BalloonTip("丸子跑到这里了喔~", 75);
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void richTextBoxOutput_Enter(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }

        void bgworker_DoWork(object sender, DoWorkEventArgs e)
        {
            // setup the process
            CheckFileExist(batPath);
            var processInfo = new System.Diagnostics.ProcessStartInfo(batPath, "2>&1");
            processInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardOutput = true;
            proc = System.Diagnostics.Process.Start(processInfo);
            // attach exit event handler
            proc.EnableRaisingEvents = true;
            proc.Exited += new EventHandler(ProcExit);
            // setup asynchronous reading
            proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(OutputHandler);
            proc.BeginOutputReadLine();
            proc.WaitForExit();
        }

        void bgworker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateProgress((double)e.UserState);
        }

        #endregion

        /// <summary>
        /// Process Exit event handler. Automatically called when process exit normally.
        /// </summary>
        private void ProcExit(object sender, EventArgs e)
        {
            // synchronize UI
            UpdateWorkCountUI();
            buttonAbort.InvokeIfRequired(() =>
                buttonAbort.Enabled = false);
            UpdateProgress(1);
            if (win7supported)
                taskbarProgress.SetProgressState(this.Handle, TBPFLAG.TBPF_NOPROGRESS);
            // wait a little bit for the last asynchronous reading
            System.Threading.Thread.Sleep(75);
            // append finish tag
            Print(Environment.NewLine + "Work Complete!");
            // fire a warning if something went wrong
            // this feature need %ERRORLEVEL% support in batch commands
            if (proc.ExitCode != 0)
            {
                Print(Environment.NewLine +
                    "Potential Error detected. Please double check the log.");
                Print(Environment.NewLine +
                    "Exit code is: " + proc.ExitCode.ToString());
            }
            // flash form and show balloon tips
            FlashForm();
            BalloonTip(@"完成了喵~ (= ω =)");

            // shutdown the system if required
            MainForm main = (MainForm)this.Owner;
            if (main.shutdownState)
            {
                System.Diagnostics.Process.Start("shutdown", "-s");
                // wait a bit to ensure synchronization
                System.Threading.Thread.Sleep(75);
                if (System.Windows.Forms.DialogResult.Cancel == MessageBox.Show(
                    "System will shutdown in 20 seconds. Click \"Cancel\" to stop countdown."
                    + Environment.NewLine +
                    "系统将在20秒后自动关机。点击“取消”停止倒计时。",
                    "Warning", MessageBoxButtons.OKCancel))
                {
                    System.Diagnostics.Process.Start("shutdown", "-a");
                }
            }
        }

        /// <summary>
        /// Manually call this method to abort all workings.
        /// </summary>
        private void ProcAbort()
        {
            // return value is useless when force aborted
            proc.CancelOutputRead();
            // exit process should be omitted too
            proc.Exited -= new EventHandler(ProcExit);
            // terminate threads
            killProcTree(proc.Id);
            if (win7supported)
            {
                // turn progressbar red
                NativeMethods.PostMessage(progressBarX264.Handle, NativeMethods.Win32CommCtrlMsgs.PBM_SETSTATE,
                    new UIntPtr((uint)NativeMethods.ProgressBarState.PBST_ERROR), new IntPtr(0));
                // reset taskbar progress
                taskbarProgress.SetProgressState(this.Handle, TBPFLAG.TBPF_NOPROGRESS);
            }
            // Print abort message to log
            Print(Environment.NewLine + "Work is aborted by user.");
            // Disable abort button
            buttonAbort.Enabled = false;
        }

        /// <summary>
        /// Asynchronous reading DataReceived event handler.
        /// </summary>
        private void OutputHandler(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                // convert and log first
                Print(e.Data + Environment.NewLine);
                // test if it is command
                Match result = Patterns.fileReg.Match(e.Data);
                if (result.Success)
                {
                    UpdateWorkCountUI();
                    progressBarX264.InvokeIfRequired(() =>
                        progressBarX264.Style = ProgressBarStyle.Blocks
                    );
                    bgworker.ReportProgress(0, 0.0);
                    frameCount = EstimateFrame(workPath, result.Groups["fileIn"].Value);
                }
                // try ffms pattern
                result = Patterns.ffmsReg.Match(e.Data);
                if (result.Success)
                    bgworker.ReportProgress(0,
                        Double.Parse(result.Groups["percent"].Value) / 100);
                else
                {
                    // try lavf pattern
                    result = Patterns.lavfReg.Match(e.Data);
                    if (result.Success)
                        bgworker.ReportProgress(0,
                            Double.Parse(result.Groups["frame"].Value) / frameCount);
                    else
                    {
                        // try nero pattern
                        result = Patterns.neroReg.Match(e.Data);
                        if (!result.Success)
                            internellog.AppendLine(e.Data);
                    }
                }
            }
        }

        /// <summary>
        /// Kill a process as well as all childs by PID.
        /// </summary>
        /// <param name="pid">The PID of the target process.</param>
        private void killProcTree(int pid)
        {
            // get child list first
            var childlist = new System.Management.ManagementObjectSearcher(
                "Select * From Win32_Process Where ParentProcessID=" + pid).Get();
            // suicide. Dump exceptions silently.
            try
            {
                System.Diagnostics.Process.GetProcessById(pid).Kill();
            }
            catch (ArgumentException) { }
            // recursive genocide
            foreach (var m in new System.Management.ManagementObjectSearcher(
                "Select * From Win32_Process Where ParentProcessID=" + pid).Get())
            {
                killProcTree(Convert.ToInt32(m["ProcessID"]));
            }
        }

        /// <summary>
        /// Update Work Count UI on call. Each call will bump up completed work count by 1.
        /// </summary>
        private void UpdateWorkCountUI()
        {
            ++workCompleted;
            this.InvokeIfRequired(() =>
                this.Text = "Xiaowan (" + workCompleted + '/' + WorkQueued + ')');
            this.labelworkCount.InvokeIfRequired(() =>
                labelworkCount.Text = workCompleted.ToString() + '/' + WorkQueued + " Completed");
        }

        /// <summary>
        /// Update Progress Bar as well as the number on it.
        /// </summary>
        /// <param name="value">Progress expressed in decimal (0.00-1.00).</param>
        private void UpdateProgress(double value)
        {
            // Some kind of safeguard
            if (value < 0)
                value = 0;
            if (value > 1)
                value = 1;
            // Update UI
            progressBarX264.InvokeIfRequired(() =>
                progressBarX264.Value = Convert.ToInt32(value * progressBarX264.Maximum));
            labelProgress.InvokeIfRequired(() =>
                labelProgress.Text = value.ToString("P"));
            if (win7supported)
                taskbarProgress.SetProgressValue(this.Handle,
                    Convert.ToUInt64(value * progressBarX264.Maximum), Convert.ToUInt64(progressBarX264.Maximum));

            notifyIcon.Text = "小丸工具箱" + Environment.NewLine +
                labelworkCount.Text + " - " + labelProgress.Text;
        }

        /// <summary>
        /// Get a rough estimation on frame counts via FFmpeg.
        ///     If failed, return <see cref="Int32.MaxValue"/> instead.
        /// </summary>
        /// <param name="workPath">Path to ffmpeg binary.</param>
        /// <param name="filePath">Path to target file.</param>
        /// <returns>Estimated frame count. 1% tolerance added.</returns>
        private int EstimateFrame(string workPath, string filePath)
        {
            string ffmpegPath = System.IO.Path.Combine(workPath, "ffmpeg.exe");
            CheckFileExist(ffmpegPath);
            var processInfo = new System.Diagnostics.ProcessStartInfo(ffmpegPath, "-i \"" + filePath + '"');
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            var ffproc = System.Diagnostics.Process.Start(processInfo);
            // log and append
            string mediaInfo = ffproc.StandardError.ReadToEnd();
            Print("Input file: " + filePath + Environment.NewLine);
            Print(mediaInfo + Environment.NewLine);
            ffproc.WaitForExit();
            var result = Patterns.ffmpegReg.Match(mediaInfo);
            if (!result.Success)
            {
                Print("Warning: Error detected on previous file. Estimatation may not work."
                        + Environment.NewLine);
                return Int32.MaxValue;
            }
            else
                // add a 1% tolerance to avoid unintentional overflow on progress bar
                return Convert.ToInt32(TimeSpan.Parse(result.Groups["duration"].Value).TotalSeconds
                    * Double.Parse(result.Groups["tbr"].Value) * 1.01);
        }

        /// <summary>
        /// Flash the current form.
        /// </summary>
        /// <returns>Whether the form need flash or not.</returns>
        private bool FlashForm()
        {
            var info = new NativeMethods.FLASHWINFO();
            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            this.InvokeIfRequired(() =>
                info.hwnd = this.Handle);
            info.dwFlags = NativeMethods.FlashWindowFlags.FLASHW_ALL |
                NativeMethods.FlashWindowFlags.FLASHW_TIMERNOFG;
            // Flash 3 times
            info.uCount = 3;
            info.dwTimeout = 0;
            return NativeMethods.FlashWindowEx(ref info);
        }

        /// <summary>
        /// Pop a balloon tip.
        /// </summary>
        /// <param name="notes">The content to show.</param>
        /// <param name="timeout">Tip timeout.</param>
        private void BalloonTip(string notes, int timeout = 500)
        {
            MainForm main = (MainForm)this.Owner;
            if (main.trayMode)
            {
                notifyIcon.Visible = true;
                notifyIcon.ShowBalloonTip(timeout, "小丸工具箱", notes, ToolTipIcon.Info);
            }
        }

        /// <summary>
        /// Append and scroll Textbox if needed.
        /// </summary>
        /// <param name="str">String to append.</param>
        private void Print(string str)
        {
            richTextBoxOutput.InvokeIfRequired(() =>
            {
                richTextBoxOutput.AppendText(str);
                if (autoscroll)
                {
                    var info = new NativeMethods.SCROLLINFO();
                    info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
                    info.fMask = NativeMethods.ScrollBarFlags.SIF_RANGE;
                    NativeMethods.GetScrollInfo(richTextBoxOutput.Handle, NativeMethods.ScrollBarConsts.SB_VERT, ref info);
                    NativeMethods.PostMessage(richTextBoxOutput.Handle, NativeMethods.Win32Msgs.WM_VSCROLL,
                        NativeMethods.MakeWParam((uint)NativeMethods.ScrollBarCmds.SB_THUMBPOSITION, (uint)info.nMax), IntPtr.Zero);
                }
            });
        }
    }
}
