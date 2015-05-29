// ------------------------------------------------------------------
// Copyright (C) 2015 Maruko Toolbox Project
// 
//  Authors: komaruchan <sandy_0308@hotmail.com>
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

using ControlExs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mp4box
{
    public partial class FeedbackForm : FormBase
    {
        public FeedbackForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Detects the AviSynth version/date
        /// </summary>
        /// <returns></returns>
        private string CheckAviSynth()
        {
            string fileVersion = string.Empty;
            string fileDate = string.Empty;
            string fileProductName = string.Empty;
            bool bFoundInstalledAviSynth = false;

            string syswow64path = Environment.GetFolderPath(Environment.SpecialFolder.System).ToLowerInvariant().Replace("\\system32", "\\SysWOW64");
            if (!Directory.Exists(syswow64path)
                           && Util.GetFileInformation(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "avisynth.dll"), out fileVersion, out fileDate, out fileProductName))
                bFoundInstalledAviSynth = true;
            else if (Util.GetFileInformation(Path.Combine(syswow64path, "avisynth.dll"), out fileVersion, out fileDate, out fileProductName))
                bFoundInstalledAviSynth = true;

            if (bFoundInstalledAviSynth)
            {
                return "AviSynth" + (fileProductName.Contains("+") ? "+" : String.Empty) + "版本: " + fileVersion + " (" + fileDate + ")";
            }
            else return null;
        }

        private void FeedbackForm_Load(object sender, EventArgs e)
        {
            string version4 = OSInfo.GetDotNetVersion("4.0");
            string version = OSInfo.GetDotNetVersion();
            MessageTextBox.AppendText(String.Format("小丸工具箱 版本: {0}", new System.Version(ProductVersion)));
            MessageTextBox.AppendText(string.Format("\r\n操作系统: {0}{1} ({2}.{3}.{4}.{5})",
                OSInfo.GetOSName(), OSInfo.GetOSServicePack(), OSInfo.OSMajorVersion, OSInfo.OSMinorVersion, OSInfo.OSRevisionVersion, OSInfo.OSBuildVersion));
            if (String.IsNullOrEmpty(version4))
                MessageTextBox.AppendText("\r\n.NET Framework 4.0 未安装");
            else
                MessageTextBox.AppendText(string.Format("\r\nMicrosoft .NET Framework: {0}", version4));
            if (!String.IsNullOrEmpty(version) && !version4.Equals(version))
                MessageTextBox.AppendText(string.Format("\r\nMicrosoft .NET Framework: {0}", version));
            if (!String.IsNullOrEmpty(CheckAviSynth()))
                MessageTextBox.AppendText("\r\n" + CheckAviSynth());
            else
                MessageTextBox.AppendText("\r\nAvisynth 未安装");
            MessageTextBox.AppendText("\r\n------------------------------以上信息为自动检测请勿修改------------------------\r\n\r\n");
        }

        private string ReadLogFile(string logPath)
        {
            string logContent = File.ReadAllText(logPath);
            //logContent = logContent.Replace("\r\n", "<br /");
            return logContent;
        }

        private void PostButton_Click(object sender, EventArgs e)
        {
            string name = UserNameTextBox.Text;
            string qq = QQTextBox.Text;
            string email = EmailTextBox.Text;
            string title = TitleTextBox.Text;
            string msg = MessageTextBox.Text;
            string log = "";
            if (!string.IsNullOrEmpty(LogPathTextBox.Text))
            {
                if (File.Exists(LogPathTextBox.Text))
                {
                    log = ReadLogFile(LogPathTextBox.Text);
                }
                else
                {
                    ShowErrorMessage("请输入正确的日志文件路径!");
                    return;
                }
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(msg))
            {
                ShowWarningMessage("请填写以上必填项后再提交!");
                return;
            }

            ServiceReference.WebServiceSoapClient service = new ServiceReference.WebServiceSoapClient();
            bool flag = service.PostFeedback(name, qq, email, title, msg, log);

            if (flag)
            {
                ShowInfoMessage("提交成功，感谢反馈!");
            }
            else
            {
                ShowErrorMessage("提交失败!");
            }
            TitleTextBox.Clear();
            MessageTextBox.Clear();
        }

        private void LogFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "日志文件(*.log)|*.log|所有文件(*.*)|*.*";
            DialogResult result = ofd.ShowDialog();
            if (result == DialogResult.OK)
            {
                LogPathTextBox.Text = ofd.FileName;
            }
        }
    }
}
