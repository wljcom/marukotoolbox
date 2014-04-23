using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
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
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            NativeMethods.SetUnmanagedDllDirectory();

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
