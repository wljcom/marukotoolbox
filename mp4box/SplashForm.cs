using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

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

        private void SplashForm_Load(object sender, EventArgs e)
        {
            ArtTextLabel TitleLabel = new ArtTextLabel();
            TitleLabel.AutoSize = true;
            TitleLabel.Font = new Font("微软雅黑", 21.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            TitleLabel.ForeColor = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            TitleLabel.BackColor = Color.Transparent;
            TitleLabel.Location = new Point(300, 100);
            TitleLabel.Name = "TitleLabel";
            TitleLabel.Size = new Size(208, 29);
            TitleLabel.TabIndex = 2;
            TitleLabel.Text = "小丸工具箱";
            TitleLabel.Parent = pictureBox;
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

        private void SplashForm_Paint(object sender, PaintEventArgs e)
        {
            List<Point> list = new List<Point>();
            int width = this.Width;
            int height = this.Height;
            #region 四个圆角

            //左上  
            list.Add(new Point(0, 47));
            list.Add(new Point(1, 42));
            list.Add(new Point(2, 38));
            list.Add(new Point(3, 36));
            list.Add(new Point(4, 33));
            list.Add(new Point(5, 32));
            list.Add(new Point(6, 29));
            list.Add(new Point(7, 27));
            list.Add(new Point(8, 26));
            list.Add(new Point(9, 24));
            list.Add(new Point(10, 22));
            list.Add(new Point(11, 21));
            list.Add(new Point(12, 20));
            list.Add(new Point(13, 19));
            list.Add(new Point(14, 17));
            list.Add(new Point(15, 16));
            list.Add(new Point(16, 15));
            list.Add(new Point(17, 14));
            list.Add(new Point(19, 13));
            list.Add(new Point(20, 12));
            list.Add(new Point(21, 11));
            list.Add(new Point(22, 10));
            list.Add(new Point(24, 9));
            list.Add(new Point(26, 8));
            list.Add(new Point(27, 7));
            list.Add(new Point(29, 6));
            list.Add(new Point(32, 5));
            list.Add(new Point(33, 4));
            list.Add(new Point(36, 3));
            list.Add(new Point(38, 2));
            list.Add(new Point(42, 1));
            list.Add(new Point(47, 0));

            //右上  
            list.Add(new Point(width - 47, 0));
            list.Add(new Point(width - 42, 1));
            list.Add(new Point(width - 38, 2));
            list.Add(new Point(width - 36, 3));
            list.Add(new Point(width - 33, 4));
            list.Add(new Point(width - 32, 5));
            list.Add(new Point(width - 29, 6));
            list.Add(new Point(width - 27, 7));
            list.Add(new Point(width - 26, 8));
            list.Add(new Point(width - 24, 9));
            list.Add(new Point(width - 22, 10));
            list.Add(new Point(width - 21, 11));
            list.Add(new Point(width - 20, 12));
            list.Add(new Point(width - 19, 13));
            list.Add(new Point(width - 17, 14));
            list.Add(new Point(width - 16, 15));
            list.Add(new Point(width - 15, 16));
            list.Add(new Point(width - 14, 17));
            list.Add(new Point(width - 13, 19));
            list.Add(new Point(width - 12, 20));
            list.Add(new Point(width - 11, 21));
            list.Add(new Point(width - 10, 22));
            list.Add(new Point(width - 9, 24));
            list.Add(new Point(width - 8, 26));
            list.Add(new Point(width - 7, 27));
            list.Add(new Point(width - 6, 29));
            list.Add(new Point(width - 5, 32));
            list.Add(new Point(width - 4, 33));
            list.Add(new Point(width - 3, 36));
            list.Add(new Point(width - 2, 38));
            list.Add(new Point(width - 1, 42));
            list.Add(new Point(width - 0, 47));

            //右下  
            list.Add(new Point(width - 0, height - 47));
            list.Add(new Point(width - 1, height - 42));
            list.Add(new Point(width - 2, height - 38));
            list.Add(new Point(width - 3, height - 36));
            list.Add(new Point(width - 4, height - 33));
            list.Add(new Point(width - 5, height - 32));
            list.Add(new Point(width - 6, height - 29));
            list.Add(new Point(width - 7, height - 27));
            list.Add(new Point(width - 8, height - 26));
            list.Add(new Point(width - 9, height - 24));
            list.Add(new Point(width - 10, height - 22));
            list.Add(new Point(width - 11, height - 21));
            list.Add(new Point(width - 12, height - 20));
            list.Add(new Point(width - 13, height - 19));
            list.Add(new Point(width - 14, height - 17));
            list.Add(new Point(width - 15, height - 16));
            list.Add(new Point(width - 16, height - 15));
            list.Add(new Point(width - 17, height - 14));
            list.Add(new Point(width - 19, height - 13));
            list.Add(new Point(width - 20, height - 12));
            list.Add(new Point(width - 21, height - 11));
            list.Add(new Point(width - 22, height - 10));
            list.Add(new Point(width - 24, height - 9));
            list.Add(new Point(width - 26, height - 8));
            list.Add(new Point(width - 27, height - 7));
            list.Add(new Point(width - 29, height - 6));
            list.Add(new Point(width - 32, height - 5));
            list.Add(new Point(width - 33, height - 4));
            list.Add(new Point(width - 36, height - 3));
            list.Add(new Point(width - 38, height - 2));
            list.Add(new Point(width - 42, height - 1));
            list.Add(new Point(width - 47, height - 0));

            //左下  
            list.Add(new Point(47, height - 0));
            list.Add(new Point(42, height - 1));
            list.Add(new Point(38, height - 2));
            list.Add(new Point(36, height - 3));
            list.Add(new Point(33, height - 4));
            list.Add(new Point(32, height - 5));
            list.Add(new Point(29, height - 6));
            list.Add(new Point(27, height - 7));
            list.Add(new Point(26, height - 8));
            list.Add(new Point(24, height - 9));
            list.Add(new Point(22, height - 10));
            list.Add(new Point(21, height - 11));
            list.Add(new Point(20, height - 12));
            list.Add(new Point(19, height - 13));
            list.Add(new Point(17, height - 14));
            list.Add(new Point(16, height - 15));
            list.Add(new Point(15, height - 16));
            list.Add(new Point(14, height - 17));
            list.Add(new Point(13, height - 19));
            list.Add(new Point(12, height - 20));
            list.Add(new Point(11, height - 21));
            list.Add(new Point(10, height - 22));
            list.Add(new Point(9, height - 24));
            list.Add(new Point(8, height - 26));
            list.Add(new Point(7, height - 27));
            list.Add(new Point(6, height - 29));
            list.Add(new Point(5, height - 32));
            list.Add(new Point(4, height - 33));
            list.Add(new Point(3, height - 36));
            list.Add(new Point(2, height - 38));
            list.Add(new Point(1, height - 42));
            list.Add(new Point(0, height - 47));

            #endregion
            Point[] points = list.ToArray();
            GraphicsPath shape = new GraphicsPath();
            shape.AddPolygon(points);
            this.Region = new System.Drawing.Region(shape);
        }
    }

    class ArtTextLabel : Label
    {
        int _borderSize = 3;
        Color _borderColor = Color.Purple;
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            PointF pt = new PointF(0, 0);
            RenderRelievoText(pe.Graphics, pt);
        }


        //边框
        private void RenderBordText(Graphics g, PointF point)
        {
            using (Brush brush = new SolidBrush(_borderColor))
            {
                for (int i = 1; i <= _borderSize; i++)
                {
                    g.DrawString(
                        base.Text,
                        base.Font,
                        brush,
                        point.X - i,
                        point.Y);
                    g.DrawString(
                        base.Text,
                        base.Font,
                        brush,
                        point.X,
                        point.Y - i);
                    g.DrawString(
                        base.Text,
                        base.Font,
                        brush,
                        point.X + i,
                        point.Y);
                    g.DrawString(
                        base.Text,
                        base.Font,
                        brush,
                        point.X,
                        point.Y + i);
                }
            }
            using (Brush brush = new SolidBrush(base.ForeColor))
            {
                g.DrawString(
                    base.Text, base.Font, brush, point);
            }
        }

        //印版
        private void RenderFormeText(Graphics g, PointF point)
        {
            using (Brush brush = new SolidBrush(_borderColor))
            {
                for (int i = 1; i <= _borderSize; i++)
                {
                    g.DrawString(
                        base.Text,
                        base.Font,
                        brush,
                        point.X - i,
                        point.Y + i);
                }
            }
            using (Brush brush = new SolidBrush(base.ForeColor))
            {
                g.DrawString(
                    base.Text, base.Font, brush, point);
            }
        }


        //浮雕。
        private void RenderRelievoText(Graphics g, PointF point)
        {
            using (Brush brush = new SolidBrush(_borderColor))
            {
                for (int i = 1; i <= _borderSize; i++)
                {
                    g.DrawString(
                        base.Text,
                        base.Font,
                        brush,
                        point.X + i,
                        point.Y);
                    g.DrawString(
                        base.Text,
                        base.Font,
                        brush,
                        point.X,
                        point.Y + i);
                }
            }
            using (Brush brush = new SolidBrush(base.ForeColor))
            {
                g.DrawString(
                    base.Text, base.Font, brush, point);
            }
        }
    }
}
