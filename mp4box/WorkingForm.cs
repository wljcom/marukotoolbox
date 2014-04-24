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
    using Extentions;
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
            public static readonly Regex fileReg = new Regex(fileRegStr);

            /// <summary>
            /// ffmpeg -i sample output:
            /// <para>Duration: 00:02:20.22, start: 0.000000, bitrate: 827 kb/s</para>
            /// <para>Stream #0:0: Video: h264 (High), yuv420p, 1280x720, 545 kb/s, 24.42 fps, 23.98 tbr, 1k tbn, 47.95 tbc</para>
            /// <para>Stream #0:1: Audio: aac, 48000 Hz, stereo, fltp, 128 kb/s</para>
            /// </summary>
            private const string ffmpegRegStr = @"\bDuration: (?<duration>\d{2}:\d{2}:\d{2}\.\d{2}), start: " +
                @".+: Video: .+ fps, (?<tbr>\d+\.?\d+) tbr, ";

            /// <summary>
            /// ffmpeg output Regex.
            ///     Available patterns: duration, tbr.
            /// </summary>
            public static readonly Regex ffmpegReg = new Regex(ffmpegRegStr, RegexOptions.Singleline);
        }

        #endregion

        #region Native Functions
        /// <summary>
        /// Class hosting Native Win32 APIs
        /// </summary>
        private class NativeMethods
        {
            // Definitions extracted from <Winuser.h>
            public const int SB_VERT = 1;

            public enum FlashWindowFlags : uint
            {
                /// <summary>
                /// Stop flashing. The system restores the window to its original state.
                /// </summary>
                FLASHW_STOP = 0,

                /// <summary>
                /// Flash the window caption.
                /// </summary>
                FLASHW_CAPTION = 1,

                /// <summary>
                /// Flash the taskbar button.
                /// </summary>
                FLASHW_TRAY = 2,

                /// <summary>
                /// Flash both the window caption and taskbar button.
                /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
                /// </summary>
                FLASHW_ALL = 3,

                /// <summary>
                /// Flash continuously, until the FLASHW_STOP flag is set.
                /// </summary>
                FLASHW_TIMER = 4,

                /// <summary>
                /// Flash continuously until the window comes to the foreground.
                /// </summary>
                FLASHW_TIMERNOFG = 12
            }

            // http://msdn.microsoft.com/en-us/library/windows/desktop/ms679348(v=vs.85).aspx
            [StructLayout(LayoutKind.Sequential)]
            public struct FLASHWINFO
            {
                /// <summary>
                /// The size of the structure in bytes.
                /// </summary>
                public uint cbSize;
                /// <summary>
                /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
                /// </summary>
                public IntPtr hwnd;
                /// <summary>
                /// The Flash Status.
                /// </summary>
                public FlashWindowFlags dwFlags; //uint
                /// <summary>
                /// The number of times to Flash the window.
                /// </summary>
                public uint uCount;
                /// <summary>
                /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
                /// </summary>
                public uint dwTimeout;
            }

            // http://msdn.microsoft.com/en-us/library/windows/desktop/bb787585(v=vs.85).aspx
            [DllImport("user32.dll")]
            public static extern int GetScrollPos(IntPtr hWnd, int nBar);

            // http://msdn.microsoft.com/en-us/library/windows/desktop/bb787587(v=vs.85).aspx
            [DllImport("user32.dll")]
            public static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);

            // http://msdn.microsoft.com/en-us/library/windows/desktop/ms679347(v=vs.85).aspx
            [DllImport("user32.dll")]
            public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
        }
        #endregion

        #region UI Methods

        private void WorkingForm_Load(object sender, EventArgs e)
        {
            // save commands into a batch file
            batPath = System.IO.Path.Combine(
                Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine),
                "xiaowan" + DateTime.Now.ToFileTimeUtc().ToString() + ".bat");
            var encoder = Encoding.GetEncoding(0);
            var sw = new System.IO.StreamWriter(batPath, false, encoder);
            sw.WriteLine(Commands);
            sw.Close();
            // synchronize UI
            workCompleted = -1;
            richTextBoxOutput.Select();
            // validate the command string
            if (Commands.Equals(encoder.GetString(encoder.GetBytes(Commands))))
                ProcStart();
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
                savDlg.Filter = "Log files|*.log|All files|*.*";
                savDlg.FilterIndex = 1;
                savDlg.RestoreDirectory = true;
                if (savDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    richTextBoxOutput.SaveFile(savDlg.FileName, RichTextBoxStreamType.UnicodePlainText);
            }
        }

        private void richTextBoxOutput_VScroll(object sender, EventArgs e)
        {
            int vMin, vMax;
            NativeMethods.GetScrollRange(richTextBoxOutput.Handle, NativeMethods.SB_VERT, out vMin, out vMax);
            if (NativeMethods.GetScrollPos(richTextBoxOutput.Handle, NativeMethods.SB_VERT)
                + richTextBoxOutput.Font.Height > vMax - richTextBoxOutput.ClientSize.Height)
                richTextBoxOutput.Select();
            else
                this.ActiveControl = null;
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
   
        #endregion

        /// <summary>
        /// Start the working process.
        /// </summary>
        private void ProcStart()
        {
            // setup the process
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
        }

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
            // wait a little bit for the last asynchronous reading
            System.Threading.Thread.Sleep(75);
            // append finish tag
            richTextBoxOutput.InvokeIfRequired(() =>
            {
                richTextBoxOutput.AppendText(Environment.NewLine + "Work Complete!");
                // fire a warning if something went wrong
                // this feature need %ERRORLEVEL% support in batch commands
                if (proc.ExitCode != 0)
                {
                    richTextBoxOutput.AppendText(Environment.NewLine +
                        "Potential Error detected. Please double check the log.");
                    richTextBoxOutput.AppendText(Environment.NewLine +
                        "Exit code is: " + proc.ExitCode.ToString());
                }
            });
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
            // Print abort message to log
            richTextBoxOutput.InvokeIfRequired(() =>
                richTextBoxOutput.AppendText(Environment.NewLine + "Work is aborted by user."));
        }

        /// <summary>
        /// Asynchronous reading DataReceived event handler.
        /// </summary>
        private void OutputHandler(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                // convert and log first
                richTextBoxOutput.InvokeIfRequired(() =>
                    richTextBoxOutput.AppendText(e.Data + Environment.NewLine));
                // test if it is command
                Match result = Patterns.fileReg.Match(e.Data);
                if (result.Success)
                {
                    UpdateWorkCountUI();
                    UpdateProgress(0);
                    frameCount = EstimateFrame(workPath, result.Groups["fileIn"].Value);
                }
                // try ffms pattern
                result = Patterns.ffmsReg.Match(e.Data);
                if (result.Success)
                    UpdateProgress(Double.Parse(result.Groups["percent"].Value) / 100);
                // try lavf pattern
                result = Patterns.lavfReg.Match(e.Data);
                if (result.Success)
                    UpdateProgress(Double.Parse(result.Groups["frame"].Value) / frameCount);
            }
        }

        /// <summary>
        /// Kill a process as well as all childs by PID.
        /// </summary>
        /// <param name="pid">The PID of the target process.</param>
        private void killProcTree(int pid)
        {
            // recursive first
            foreach (var m in new System.Management.ManagementObjectSearcher(
                "Select * From Win32_Process Where ParentProcessID=" + pid).Get())
            {
                killProcTree(Convert.ToInt32(m["ProcessID"]));
            }
            // then suicide, usually threads throw exceptions when killed. Dump them silently.
            try
            {
                System.Diagnostics.Process.GetProcessById(pid).Kill();
            }
            catch (ArgumentException) { }
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
            progressBarX264.InvokeIfRequired(() =>
                progressBarX264.Value = Convert.ToInt32(value * progressBarX264.Maximum));
            labelProgress.InvokeIfRequired(() =>
                labelProgress.Text = value.ToString("P"));
            notifyIcon.Text = "小丸工具箱" + Environment.NewLine +
                labelworkCount.Text + " - " + labelProgress.Text;
        }

        /// <summary>
        /// Get a rough estimation on frame counts via FFmpeg.
        /// </summary>
        /// <param name="workPath">Path to ffmpeg binary.</param>
        /// <param name="filePath">Path to target file.</param>
        /// <returns>Estimated frame count. 1% tolerance added.</returns>
        private int EstimateFrame(string workPath, string filePath)
        {
            string ffmpegPath = System.IO.Path.Combine(workPath, "ffmpeg.exe");
            var processInfo = new System.Diagnostics.ProcessStartInfo(ffmpegPath, "-i \"" + filePath + '"');
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            var ffproc = System.Diagnostics.Process.Start(processInfo);
            // log and append
            string mediaInfo = ffproc.StandardError.ReadToEnd();
            richTextBoxOutput.InvokeIfRequired(() =>
            {
                richTextBoxOutput.AppendText("Input file: " + filePath + Environment.NewLine);
                richTextBoxOutput.AppendText(mediaInfo + Environment.NewLine);
            });
            ffproc.WaitForExit();
            var result = Patterns.ffmpegReg.Match(mediaInfo);
            if (!result.Success)
            {
                killProcTree(proc.Id);
                if (System.Windows.Forms.DialogResult.Yes == MessageBox.Show(
                    "Fatal Error Detected! Click \"Yes\" to save log. More details on help page."
                    + Environment.NewLine +
                    "出现严重错误！点“是”可以保存错误日志文件。提交错误报告清查看帮助页。",
                    "Fatal Error.", MessageBoxButtons.YesNo, MessageBoxIcon.Error))
                    ProcAbort();
                return -1;
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
    }
}
