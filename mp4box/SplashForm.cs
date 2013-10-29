using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mp4box
{
    public partial class SplashForm : Form
    {
        int iCount = 0;
        public SplashForm()
        {
            InitializeComponent();
            //启动画面
            //去掉外框
            this.FormBorderStyle = FormBorderStyle.None;
            //背景图片
            this.BackgroundImage = Image.FromFile("startup.jpg");
            //屏幕中央
            this.StartPosition = FormStartPosition.CenterScreen;

        }

        private void FrmStart_Load(object sender, EventArgs e)
        {
            timer2.Start();
            timer2.Interval = 10;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (iCount > 800)
            {
                //启动动画完了之后..
                this.Close();
                return;
            }
            iCount += 10;
            //窗体透明度减小
            this.Opacity = 1 - Convert.ToDouble(iCount) / 1000;
        }


    }
}
