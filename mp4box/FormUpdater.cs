// ------------------------------------------------------------------
// Copyright (C) 2015 Maruko Toolbox Project
// 
//  Authors: LunarShaddow <aflyhorse@hotmail.com>
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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace mp4box
{
    public partial class FormUpdater : Form
    {
        /// <summary>
        /// Path of new (and temporary) assembly.
        /// </summary>
        private string newPath;

        /// <summary>
        /// Path of current assembly.
        /// </summary>
        private string exePath;

        /// <summary>
        /// Path of designated backup assembly.
        /// </summary>
        private string backupPath;

        /// <summary>
        /// Update Downloader.
        /// </summary>
        private System.Net.WebClient client;

        /// <summary>
        /// Construnctor, initiate an update at targeted directory.
        /// </summary>
        /// <param name="startpath">The directory containing xiaowan.exe.</param>
        /// <param name="date">The new release date.</param>
        public FormUpdater(string startpath, string date)
        {
            InitializeComponent();
            newPath = System.IO.Path.Combine(startpath, "xiaowan.exe.new");
            exePath = System.IO.Path.Combine(startpath, "xiaowan.exe");
            backupPath = System.IO.Path.Combine(startpath, "xiaowan.exe.bak");
            labelDate.Text = date;
        }

        #region UI Methods
        private void FormUpdater_Load(object sender, EventArgs e)
        {
            client = new System.Net.WebClient();
            client.DownloadFileCompleted += client_DownloadFileCompleted;
            client.DownloadProgressChanged += client_DownloadProgressChanged;
            client.DownloadFileAsync(new Uri("http://mtbftest.sinaapp.com/xiaowan.exe"), newPath);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            client.CancelAsync();
        }
        #endregion

        /// <summary>
        /// Update ProgressBar as requested.
        /// </summary>
        void client_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            progressBarDownload.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Clean up. Remove incomplete file or do actually file replacing.
        /// </summary>
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (System.IO.File.Exists(newPath))
                    System.IO.File.Delete(newPath);
                this.Close();
            }
            else
            {
                if (System.IO.File.Exists(backupPath))
                    System.IO.File.Exists(backupPath);
                System.IO.File.Move(exePath, backupPath);
                System.IO.File.Move(newPath, exePath);
                Application.Restart();
            }
        }
    }
}
