using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mp4box
{
    public partial class WorkingForm : Form
    {
        // potential batch file path
        private string batPath;
        // potential working process
        private System.Diagnostics.Process proc;

        public WorkingForm(string cmds)
        {
            InitializeComponent();

            // save commands into a batch file
            batPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(),
                "xiaowan" + DateTime.Now.ToFileTimeUtc().ToString() + ".bat");
            var sw = new System.IO.StreamWriter(batPath);
            sw.WriteLine(cmds);
            sw.Close();
            // start working
            WorkStart();
        }

        private void WorkStart()
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
            proc.Exited += new EventHandler(WorkEnd);
            // setup asynchronous reading
            proc.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(OutputHandler);
            proc.BeginOutputReadLine();
        }

        private void WorkEnd(object sender, EventArgs e)
        {
            // toggle GUI
            buttonAbort.Invoke(new MethodInvoker(delegate
            {
                buttonAbort.Enabled = false;
            }));
            // wait a bit, finish off the last asynchronous reading
            System.Threading.Thread.Sleep(75);
            // add finish tag
            textBoxOutput.Invoke(new MethodInvoker(delegate
            {
                textBoxOutput.AppendText(Environment.NewLine + "Work Finished." + Environment.NewLine);
                // fire a warning if something went wrong
                // this feature need %ERRORLEVEL% support in batch commands
                if (proc.ExitCode != 0)
                {
                    textBoxOutput.AppendText("Potential Error detected. Please double check the log."
                        + Environment.NewLine);
                    textBoxOutput.AppendText("Exit code is: " + proc.ExitCode.ToString()
                        + Environment.NewLine);
                }
            }));
        }

        private void OutputHandler(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                textBoxOutput.Invoke(new MethodInvoker(delegate
                {
                    textBoxOutput.AppendText(e.Data + Environment.NewLine);
                }));
            }
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            killProcTree(proc.Id);
        }

        private void killProcTree(int pid)
        {
            // recursive first
            foreach (var m in new System.Management.ManagementObjectSearcher(
                "Select * From Win32_Process Where ParentProcessID=" + pid).Get())
            {
                killProcTree(Convert.ToInt32(m["ProcessID"]));
            }
            // then suicide
            try
            {
                System.Diagnostics.Process.GetProcessById(pid).Kill();
            }
            catch (ArgumentException)
            { }
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
                {
                    var sw = savDlg.OpenFile();
                    byte[] buffer = Encoding.UTF8.GetBytes(textBoxOutput.Text);
                    sw.Write(buffer, 0, buffer.Length);
                    sw.Close();
                }
            }
        }
    }
}
