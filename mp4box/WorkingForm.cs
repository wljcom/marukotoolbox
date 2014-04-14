using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
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
        /// Definition of Regex formula used in filter.
        /// </summary>
        private static System.Text.RegularExpressions.Regex extractRex
            = new System.Text.RegularExpressions.Regex(@"^[[0-9]+.[0-9]+%]");

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
            var sw = new System.IO.StreamWriter(batPath);
            sw.WriteLine(Commands);
            sw.Close();
            // synchronize UI
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
                // log first
                richTextBoxOutput.InvokeIfRequired(() =>
                    richTextBoxOutput.AppendText(e.Data + Environment.NewLine));
                // extract x264 progress if possible
                System.Text.RegularExpressions.Match result = extractRex.Match(e.Data);
                if (result.Success)
                    progressBarX264.InvokeIfRequired(() =>
                    {
                        progressBarX264.Value = Convert.ToInt32(
                            Double.Parse(result.Value.Substring(1, result.Value.Length - 3))
                            * progressBarX264.Maximum / 100);
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
    }
}
