using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace mp4box
{
    //使用方法:定义一个启动类，应用程序从启动类启动，该类会使用继承自启动窗体虚基类的一个启动窗体类，在该类中定义启动窗体和主窗体。启动窗体和主窗体的代码略去，注意要删除机器生成的窗体代码的Main方法部分。
    public class StartUpClass
    {
        [STAThread]
        static void Main()
        {
            Application.Run(new mycontext());
        }
    }

    //启动窗体类(继承自启动窗体虚基类),启动画面会停留一段时间，该时间是设定的时间和主窗体构造所需时间两个的最大值 
    public class mycontext : SplashScreenApplicationContext
    {
        protected override void OnCreateSplashScreenForm()
        {
            this.SplashScreenForm = new SplashForm();//启动窗体 
        }
        protected override void OnCreateMainForm()
        {
            this.PrimaryForm = new MainForm();//主窗体 
        }
        protected override void SetSeconds()
        {
            this.SecondsShow = 2;//启动窗体显示的时间(秒) 
        }
    } 
}
