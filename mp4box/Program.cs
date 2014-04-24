using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Configuration;
namespace mp4box
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]


        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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
