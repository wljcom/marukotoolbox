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
    using InvokeEx;
    public partial class WorkingForm : Form
    {
        /// <summary>
        /// Initialize a cmd output wrapper.
        /// </summary>
        /// <param name="commands">Proposing commands.
        ///     Expecting Environment.NewLine as line breaker.</param>
        public WorkingForm(string commands)
        {
            InitializeComponent();
            Commands = commands;
        }

        /// <summary>
        /// Gets or sets the proposed commands. Expecting Environment.NewLine as line breaker.
        /// <exception cref="System.ArgumentException">
        ///     An exception is thrown if it's empty or null</exception>
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

        #region Private Members Declaration

        /// <summary>
        /// Proposed commands.
        /// </summary>
        private string cmds;

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
        /// String decoder of current code page.
        /// </summary>
        private Encoding decoder;

        #endregion

        #region Regex Patterns
        /// <summary>
        /// Store const regex patterns.
        ///     The Regex objects are made to speed up matchup instead of passing string as arguments from time to time.
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
            /// -o "X:\workDIR\output.ext" "X:\workDIR\input.ext" --acodec faac --abitrate 128</para>
            /// </summary>
            /// <remarks>
            /// Without double quote: ^(?<workDIR>.+)>".+x264.+ -o "(?<fileOut>.+)" "(?<fileIn>.+)"
            /// </remarks>
            private const string fileRegStr = @"^(?<workDIR>.+)>"".+x264.+ -o ""(?<fileOut>.+)"" ""(?<fileIn>.+)""";

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
                @".+: Video: .+ fps, (?<tbr>\d+\.\d{2}) tbr, ";

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

            [DllImport("user32.dll")]
            public static extern int GetScrollPos(IntPtr hWnd, int nBar);
            [DllImport("user32.dll")]
            public static extern bool GetScrollRange(IntPtr hWnd, int nBar, out int lpMinPos, out int lpMaxPos);
        }
        #endregion

        #region UI Methods

        private void WorkingForm_Load(object sender, EventArgs e)
        {
            // save commands into a batch file
            batPath = System.IO.Path.Combine(
                Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine),
                "xiaowan" + DateTime.Now.ToFileTimeUtc().ToString() + ".bat");
            var sw = new System.IO.StreamWriter(batPath, false, new UTF8Encoding(false));
            sw.WriteLine("chcp " + Encoding.UTF8.CodePage);
            sw.WriteLine(Commands);
            sw.Close();
            // synchronize UI
            decoder = Encoding.GetEncoding(0);
            richTextBoxOutput.Select();
            // start working
            ProcStart();
        }

        private void WorkingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // check remaining works
            if (!proc.HasExited)
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
                {
                    // return value is useless when force aborted
                    proc.CancelOutputRead();
                    // exit process should be omitted too
                    proc.Exited -= new EventHandler(ProcExit);
                    // terminate threads
                    killProcTree(proc.Id);
                }
            }
            // clean up temp batch file
            System.IO.File.Delete(batPath);
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            killProcTree(proc.Id);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // prohibit saving before work completion
            if (!proc.HasExited)
                MessageBox.Show("Saving before completion is not recommonded." +
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
        /// Process Exit event handler.
        /// </summary>
        private void ProcExit(object sender, EventArgs e)
        {
            // synchronize UI
            buttonAbort.InvokeIfRequired(() =>
                buttonAbort.Enabled = false);
            this.InvokeIfRequired(() =>
                this.Text = "Xiaowan [Completed.]");
            progressBarX264.InvokeIfRequired(() =>
                progressBarX264.Value = progressBarX264.Maximum);
            // wait a little bit for the last asynchronous reading
            System.Threading.Thread.Sleep(75);
            // append finish tag
            richTextBoxOutput.InvokeIfRequired(() =>
            {
                richTextBoxOutput.AppendText(Environment.NewLine + "Work Finished." + Environment.NewLine);
                // fire a warning if something went wrong
                // this feature need %ERRORLEVEL% support in batch commands
                if (proc.ExitCode != 0)
                {
                    richTextBoxOutput.AppendText("Potential Error detected. Please double check the log."
                        + Environment.NewLine);
                    richTextBoxOutput.AppendText("Exit code is: " + proc.ExitCode.ToString()
                        + Environment.NewLine);
                }
            });
        }

        /// <summary>
        /// Asynchronous reading DataReceived event handler.
        /// </summary>
        private void OutputHandler(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                // convert and log first
                string utf8Data = localToUTF8(e.Data);
                richTextBoxOutput.InvokeIfRequired(() =>
                    richTextBoxOutput.AppendText(utf8Data + Environment.NewLine));
                // test if it is command
                Match result = Patterns.fileReg.Match(utf8Data);
                if (result.Success)
                    frameCount = EstimateFrame(result.Groups["workDIR"].Value, result.Groups["fileIn"].Value);
                // try ffms pattern
                result = Patterns.ffmsReg.Match(utf8Data);
                if (result.Success)
                    progressBarX264.InvokeIfRequired(() =>
                    {
                        progressBarX264.Value = Convert.ToInt32(
                            Double.Parse(result.Groups["percent"].Value)
                            * progressBarX264.Maximum / 100);
                    });
                // try lavf pattern
                result = Patterns.lavfReg.Match(utf8Data);
                if (result.Success)
                    progressBarX264.InvokeIfRequired(() =>
                    {
                        progressBarX264.Value = Convert.ToInt32(
                            Double.Parse(result.Groups["frame"].Value)
                            * progressBarX264.Maximum / frameCount);
                    });
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
        /// Convert a string from local code page to UTF8.
        /// </summary>
        /// <param name="localstr">String in local code page.</param>
        /// <returns></returns>
        private string localToUTF8(string localstr)
        {
            byte[] rawBytes = decoder.GetBytes(localstr);
            return Encoding.UTF8.GetString(rawBytes);
        }

        /// <summary>
        /// Get a rough estimation on frame counts via FFmpeg.
        /// <exception cref="System.ArgumentException">
        ///     An exception is thrown if the return value is unrecognizable.</exception>
        /// </summary>
        /// <param name="workPath">Path to ffmpeg binary.</param>
        /// <param name="filePath">Path to target file.</param>
        /// <returns>Estimated frame count with some tolerance.</returns>
        private int EstimateFrame(string workPath, string filePath)
        {
            string ffmpegPath = System.IO.Path.Combine(workPath, @"tools\ffmpeg.exe");
            var processInfo = new System.Diagnostics.ProcessStartInfo(ffmpegPath, "-i \"" + filePath + '"');
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            var ffproc = System.Diagnostics.Process.Start(processInfo);
            string mediaInfo = ffproc.StandardError.ReadToEnd();
            ffproc.WaitForExit();
            var result = Patterns.ffmpegReg.Match(mediaInfo);
            if (!result.Success)
                throw new ArgumentException("FFmpeg probing went wrong!");
            else
                // add a 1% tolorance to avoid unintentional overflow on progress bar
                return Convert.ToInt32(TimeSpan.Parse(result.Groups["duration"].Value).TotalSeconds
                    * Double.Parse(result.Groups["tbr"].Value) * 1.01);
        }
    }
}
