using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace mp4box
{
    public partial class PreviewForm : Form
    {

        string workpath = System.Windows.Forms.Application.StartupPath;

        public PreviewForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            //axWindowsMediaPlayer1.Ctlcontrols.play();
        }

    }
}
