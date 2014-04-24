using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace mp4box
{
    using Extentions;
    public partial class SplashForm : Form
    {
        //int iCount = 0;
        public SplashForm()
        {
            InitializeComponent();
            //去掉外框
            this.FormBorderStyle = FormBorderStyle.None;
            //屏幕中央
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Opacity = 0.1;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.InvokeIfRequired(() =>
                this.Opacity += 0.1);

            //if (iCount > 800)
            //{
            //    //启动动画完了之后..
            //    this.Close();
            //    return;
            //}
            //iCount += 10;
            ////窗体透明度减小
            //this.Opacity = 1 - Convert.ToDouble(iCount) / 1000;
        }
    }
}
