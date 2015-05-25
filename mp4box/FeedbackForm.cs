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

        private void FeedbackForm_Load(object sender, EventArgs e)
        {

        }

        private string ReadLogFile(string logPath)
        {
            if (File.Exists(logPath))
            {
                string logContent = File.ReadAllText(logPath);
                //logContent = logContent.Replace("\r\n", "<br /");
                return File.ReadAllText(logPath);
            }
            else
            {
                ShowErrorMessage("请输入正确的日志文件路径!");
                return string.Empty;
            }
        }

        private void PostButton_Click(object sender, EventArgs e)
        {
            string name = UserNameTextBox.Text;
            string qq = QQTextBox.Text;
            string email = EmailTextBox.Text;
            string title = TitleTextBox.Text;
            string msg = MessageTextBox.Text;
            string log = ReadLogFile(LogPathTextBox.Text);

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(msg))
            {
                ShowWarningMessage("请填写以上必填项后再提交!");
                return;
            }

            ServiceReference.WebServiceSoapClient service = new ServiceReference.WebServiceSoapClient();
            bool flag = service.PostFeedback(name, qq, email, title, msg,log);

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
