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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace mp4box
{
    static class Program
    {
        private class NativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool SetDllDirectory(string path);

            public static void SetUnmanagedDllDirectory()
            {
                string path = Path.Combine(Application.StartupPath, "tools");
                if (IntPtr.Size == 8)
                    path = Path.Combine(path, "x64");
                if (!SetDllDirectory(path))
                    throw new System.ComponentModel.Win32Exception();
            }
        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // 检查程序路径是否可写入
            if (!Util.IsDirWriteable(Path.GetDirectoryName(Application.ExecutablePath)))
            {
                bool bRunElevated = false;
                //检测程序是否以高权限运行
                foreach (string strParam in args)
                {
                    if (strParam.Equals("-elevate"))
                    {
                        bRunElevated = true;
                        break;
                    }
                }

                // 如果需要则提升权限
                if (!bRunElevated)
                {
                    try
                    {
                        Process p = new Process();
                        p.StartInfo.FileName = Application.ExecutablePath;
                        p.StartInfo.Arguments = "-elevate";
                        p.StartInfo.Verb = "runas";
                        p.Start();
                        return;
                    }
                    catch { }
                }
                MessageBox.Show("小丸工具箱无法启动因为当前目录无法写入\r\n\r\n请赋予小丸工具箱所需权限或者将应用程序移动到未受保护的目录下", "小丸工具箱 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            NativeMethods.SetUnmanagedDllDirectory();

            // var modulename = Process.GetCurrentProcess().MainModule.ModuleName;
            // var procesname = Path.GetFileNameWithoutExtension(modulename);
            // Process[] processes = Process.GetProcessesByName(procesname);
            // if (processes.Length > 1)
            // {
            // MessageBox.Show("你已经打开了一个小丸工具箱喔！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            // Application.Exit();
            // return;
            // }

            if (ConfigurationManager.AppSettings["SplashScreen"] == "True")
            {
                Application.Run(new mycontext());
            }
            else
            {
                Application.Run(new MainForm());
            }
        }
    }
}
