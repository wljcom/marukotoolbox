using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using MediaInfoLib;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System;
using ControlExs;
using System.Globalization;
using System.Threading;
using System.Configuration;


namespace mp4box
{

    public partial class MainForm : Form
    {

        StringBuilder avsBuilder = new StringBuilder(1000);
        string syspath = Environment.GetFolderPath(Environment.SpecialFolder.System).Remove(1);
        int indexofsource;
        int indexoftarget;
        byte mode = 1;
        byte aacmode = 1;
        string clip = "";
        string MIvideo = "";
        string namevideo = "";
        string namevideo2 = "";
        string namevideo3 = "";
        string namevideo4 = "";
        string namevideo5 = "";
        string namevideo6 = "";
        string nameaudio = "";
        string nameaudio2 = "";
        string nameaudio3 = "";
        string namevideo8 = "";
        string namevideo9 = "video";
        string nameout;
        string nameout2;
        string nameout3;
        string nameout4;
        string nameout5;
        string nameout6;
        string nameout9;
        string namesub;
        string namesub2 = "";
        string namesub9 = "subtitle";
        string MItext = "把视频文件拖到这里";


        string mkvextract;
        string mkvmerge;
        string mux;
        string x264;
        string ffmpeg;
        string neroaac;
        string aac;
        string aextract;
        string vextract;
        string batpath;
        string autovideo;
        string autoaudio;
        string auto;
        int num;
        string workpath;
        string avs = "";





        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();

        public MainForm()
        {
            InitializeComponent();

        }

        public string MediaInfo(string VideoName)
        {

            string info = "无视频信息";
            if (File.Exists(VideoName))
            {
                MediaInfo MI = new MediaInfo();
                MI.Open(VideoName);

                //全局
                string container = MI.Get(StreamKind.General, 0, "Format");
                string bitrate = MI.Get(StreamKind.General, 0, "BitRate/String");
                string duration = MI.Get(StreamKind.General, 0, "Duration/String");
                string fileSize = MI.Get(StreamKind.General, 0, "FileSize/String");

                //视频
                string vid = MI.Get(StreamKind.Video, 0, "ID");
                string video = MI.Get(StreamKind.Video, 0, "Format");
                string vBitRate = MI.Get(StreamKind.Video, 0, "BitRate/String");
                string width = MI.Get(StreamKind.Video, 0, "Width");
                string height = MI.Get(StreamKind.Video, 0, "Height");
                string risplayAspectRatio = MI.Get(StreamKind.Video, 0, "DisplayAspectRatio/String");
                string risplayAspectRatio2 = MI.Get(StreamKind.Video, 0, "DisplayAspectRatio");
                string frameRate = MI.Get(StreamKind.Video, 0, "FrameRate/String");
                string bitDepth = MI.Get(StreamKind.Video, 0, "BitDepth/String");
                string pixelAspectRatio = MI.Get(StreamKind.Video, 0, "PixelAspectRatio");
                string encodedLibrary = MI.Get(StreamKind.Video, 0, "Encoded_Library");

                //音频
                string aid = MI.Get(StreamKind.Audio, 0, "ID");
                string audio = MI.Get(StreamKind.Audio, 0, "Format");
                string aBitRate = MI.Get(StreamKind.Audio, 0, "BitRate/String");
                string samplingRate = MI.Get(StreamKind.Audio, 0, "SamplingRate/String");
                string channel = MI.Get(StreamKind.Audio, 0, "Channel(s)");


                info = Path.GetFileName(VideoName) + "\r\n" +
                    "容器：" + container + "\r\n" +
                    "总码率：" + bitrate + "\r\n" +
                    "时长：" + duration + "\r\n" +
                    "大小：" + fileSize + "\r\n" +
                    "\r\n" +

                    "视频(" + vid + ")：" + video + "\r\n" +
                    "码率：" + vBitRate + "\r\n" +
                    "分辨率：" + width + "x" + height + "\r\n" +
                    "宽高比：" + risplayAspectRatio + "(" + risplayAspectRatio2 + ")" + "\r\n" +
                    "帧率：" + frameRate + "\r\n" +
                    "位深度：" + bitDepth + "\r\n" +
                    "像素宽高比：" + pixelAspectRatio + "\r\n" +
                    "编码库：" + encodedLibrary + "\r\n" +
                    "\r\n" +

                    "音频(" + aid + ")：" + audio + "\r\n" +
                    "码率：" + aBitRate + "\r\n" +
                    "采样率：" + samplingRate + "\r\n" +
                    "声道数：" + channel + "\r\n";
                MI.Close();
            }
            return info;
        }


        public string AddExt(string name, string ext)
        {
            string finish = name.Remove(name.LastIndexOf("."));
            finish += ext;
            return finish;
        }


        public string muxbat(string input1, string input2, string fps, string output)
        {
            mux = "\"" + workpath + "\\mp4box.exe\" -fps " + fps + " -add " + input1 + " -add " + input2 + " -new \"" + output + "\"";
            return mux;
        }

        public string muxbat(string input1, string input2, string output)
        {
            mux = "\"" + workpath + "\\mp4box.exe\" -add " + input1 + " -add " + input2 + " -new \"" + output + "\"";
            return mux;
        }

        public string x264bat(string input, string output)
        {
            switch (mode)
            {
                case 1:
                    if (numheight.Value == 0 || numwidth.Value == 0)
                    {
                        x264 = "\"" + workpath + "\\" + cbx264file.SelectedItem.ToString() + "\"  --crf " + numcrf.Value + " --preset 8 --demuxer " + DemuxerComboBox.Text + " -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8  -o \"" + output + "\" \"" + input + "\"\r\n";
                    }
                    else
                    {
                        x264 = "\"" + workpath + "\\" + cbx264file.SelectedItem.ToString() + "\"  --crf " + numcrf.Value + " --preset 8 --demuxer " + DemuxerComboBox.Text + " -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 --vf resize:" + numwidth.Value + "," + numheight.Value + ",,,,lanczos -o \"" + output + "\" \"" + input + "\"\r\n";
                    }
                    break;

                case 2:
                    if (numheight.Value == 0 || numwidth.Value == 0)
                    {
                        x264 = "\"" + workpath + "\\" + cbx264file.SelectedItem.ToString() + "\"  -p1 --stats \"tmp.stat\" --preset 8 --demuxer " + DemuxerComboBox.Text + " -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 -o NUL \"" + input + "\" && \"" + workpath + "\\" + cbx264file.SelectedItem.ToString() + "\"  -p2 --stats \"tmp.stat\" -B " + numrate.Value + " --preset 8 --demuxer " + DemuxerComboBox.Text + " -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 -o \"" + output + "\" \"" + input + "\"\r\n";
                    }

                    else
                    {
                        x264 = "\"" + workpath + "\\" + cbx264file.SelectedItem.ToString() + "\"  -p1 --stats \"tmp.stat\" --preset 8 --demuxer " + DemuxerComboBox.Text + " -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8  --vf resize:" + numwidth.Value + "," + numheight.Value + ",,,,lanczos -o NUL \"" + input + "\" && \"" + workpath + "\\" + cbx264file.SelectedItem.ToString() + "\"  -p2 --stats \"tmp.stat\" -B " + numrate.Value + " --preset 8 --demuxer " + DemuxerComboBox.Text + " -r 3 -b 3 --me umh -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 --vf resize:" + numwidth.Value + "," + numheight.Value + ",,,,lanczos -o \"" + output + "\" \"" + input + "\"\r\n";
                    }
                    break;

                case 0:
                    x264 = "\"" + workpath + "\\" + cbx264file.SelectedItem.ToString() + "\"  " + txth264.Text + " --demuxer " + DemuxerComboBox.Text + " -o \"" + output + "\" \"" + input + "\"\r\n";
                    break;
            }
            return x264;
        }
        //public string x264bat(string input, string output)
        //{
        //    switch (mode)
        //    {
        //        case 1:
        //            if (numheight.Value == 0 || numwidth.Value == 0)
        //            {
        //                x264 = "\"" + workpath + "\\x264.exe\"  --crf " + numcrf.Value + " --preset 8 -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8  -o \"" + output + "\" \"" + input + "\"\r\n";
        //            }
        //            else
        //            {
        //                x264 = "\"" + workpath + "\\x264.exe\"  --crf " + numcrf.Value + " --preset 8 -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 --vf resize:" + numwidth.Value + "," + numheight.Value + ",,,,lanczos -o \"" + output + "\" \"" + input + "\"\r\n";
        //            }
        //            break;

        //        case 2:
        //            if (numheight.Value == 0 || numwidth.Value == 0)
        //            {
        //                x264 = "\"" + workpath + "\\x264.exe\"  -p1 --stats \"tmp.stat\" --preset 8 -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 -o NUL \"" + input + "\" && \"" + workpath + "\\x264.exe\"  -p2 --stats \"tmp.stat\" -B " + numrate.Value + " --preset 8 -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 -o \"" + output + "\" \"" + input + "\"\r\n";
        //            }

        //            else
        //            {
        //                x264 = "\"" + workpath + "\\x264.exe\"  -p1 --stats \"tmp.stat\" --preset 8 -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8  --vf resize:" + numwidth.Value + "," + numheight.Value + ",,,,lanczos -o NUL \"" + input + "\" && \"" + workpath + "\\x264.exe\"  -p2 --stats \"tmp.stat\" -B " + numrate.Value + " --preset 8 -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 --vf resize:" + numwidth.Value + "," + numheight.Value + ",,,,lanczos -o \"" + output + "\" \"" + input + "\"\r\n";
        //            }
        //            break;

        //        case 0:
        //            x264 = "\"" + workpath + "\\x264.exe\"  " + txth264.Text + " -o \"" + output + "\" \"" + input + "\"\r\n";
        //            break;
        //    }

        //    return x264;


        //}

        public string timeminus(int h1, int m1, int s1, int h2, int m2, int s2)
        {
            int h = 0;
            int m = 0;
            int s = 0;

            s = s2 - s1;
            if (s < 0)
            {
                m = -1;
                s = s + 60;
            }


            m = m + m2 - m1;
            if (m < 0)
            {
                h = -1;
                m = m + 60;
            }
            h = h + h2 - h1;

            return h.ToString() + ":" + m.ToString() + ":" + s.ToString();
        }

        public string timeplus(int h1, int m1, int s1, int h2, int m2, int s2)
        {
            int h = 0;
            int m = 0;
            int s = 0;

            s = s1 + s2;
            if (s >= 60)
            {
                m = 1;
                s = s - 60;
            }


            m = m + m1 + m2;
            if (m >= 60)
            {
                h = 1;
                m = m - 60;
            }
            h = h + h1 + h2;

            return h.ToString() + ":" + m.ToString() + ":" + s.ToString();
        }





        public string audiobat(string input, string output)
        {
            ffmpeg = "\"" + workpath + "\\ffmpeg.exe\" -y -i \"" + input + "\" -f wav temp.wav";
            if (aacmode == 1)
            {
                int AACbr = 1024 * Convert.ToInt32(numq.Value.ToString());
                string br = AACbr.ToString();

                if (AudioEncoderComboBox.SelectedIndex == 0)
                {
                    neroaac = "\"" + workpath + "\\neroAacEnc.exe\" -ignorelength -lc -br " + br + " -if \"temp.wav\"  -of \"" + output + "\"";
                }
                if (AudioEncoderComboBox.SelectedIndex == 1)
                {
                    neroaac = "\"" + workpath + "\\qaac.exe\" -q 2 -c  " + numq.Value.ToString() + " \"temp.wav\"  -o \"" + output + "\"";
                }
                aac = ffmpeg + "&&" + neroaac + "\r\n";
            }

            if (aacmode == 2)
            {
                if (AudioEncoderComboBox.SelectedIndex == 0)
                {
                    neroaac = "\"" + workpath + "\\neroAacEnc.exe\" " + txtNeroaac.Text.ToString() + " -if \"temp.wav\"  -of \"" + output + "\"";
                }
                if (AudioEncoderComboBox.SelectedIndex == 1)
                {
                    neroaac = "\"" + workpath + "\\qaac.exe\" " + txtNeroaac.Text.ToString() + " \"temp.wav\"  -o \"" + output + "\"";
                }
                aac = ffmpeg + "&&" + neroaac + "\r\n";
            }

            if (cbwavtemp.Checked == false)
            {
                aac += "del temp.wav\r\n";
            }
            return aac;
        }

        public void oneAuto()
        {
            x264 = x264bat(namevideo3, "temp.mp4");

            x264 = x264.Replace("\r\n", "");

            x264 += " --acodec none\r\n";

            //audio
            //如果是avs
            if (namevideo3.Substring(namevideo3.LastIndexOf(".") + 1) == "avs")
            {

                aextract = "\r\n";


                //string nameavs = namevideo3;
                //int a, b;
                //StreamReader sr = new StreamReader(namevideo3, System.Text.Encoding.Default);
                //string str = sr.ReadToEnd();
                //a = str.IndexOf("DirectShowSource(\"");
                //if (a == -1)
                //{
                //    a = str.IndexOf("FFVideoSource(\"");
                //    b = str.IndexOf("\"", a + 15);
                //    namevideo3 = str.Substring(a + 15, b - a - 15);
                //}
                //else
                //{
                //    b = str.IndexOf("\"", a + 18);
                //    namevideo3 = str.Substring(a + 18, b - a - 18);
                //}

                //if (namevideo3.IndexOf(":") != 1)
                //{
                //    namevideo3 = nameavs.Substring(0, nameavs.LastIndexOf("\\") + 1) + namevideo3;
                //}

            }

            else
            {
                aextract = audiobat(namevideo3, "temp.aac");
            }

            //mux
            mux = muxbat("temp.mp4", "temp.aac", cbFPS.Text, nameout4);

            if (cbDelTmp.Checked == false)
            {
                auto = aextract + x264 + mux + " \r\ndel temp.aac\r\ndel temp.mp4\r\ndel temp.wav\r\n";
            }
            else
            {
                auto = aextract + x264 + mux + " \r\n\r\n";
            }

            if (cbFLV.Checked == true)
            {
                string flvfile = AddExt(nameout4, "_FLV.flv");
                auto += "\r\n\"" + workpath + "\\ffmpeg.exe\"  -i  \"" + nameout4 + "\" -c copy -f flv  \"" + flvfile + "\" \r\n";
            }

            if (cbshutdown.Checked)
            {
                auto += "\r\n" + syspath + ":\\Windows\\System32\\shutdown -f -s -t 60\r\n";
            }

            auto += "cmd";


        }

        public void batchAuto()
        {
            int i;
            auto = "";
            for (i = 0; i < this.lbAuto.Items.Count; i++)
            {
                x264 = x264bat(lbAuto.Items[i].ToString(), "temp.mp4");
                x264 = x264.Replace("\r\n", "");
                x264 += " --acodec none\r\n";
                //audio
                //如果是avs
                if (lbAuto.Items[i].ToString().Substring(lbAuto.Items[i].ToString().LastIndexOf(".") + 1) == "avs")
                {
                    aextract = "\r\n";


                    //string tempvideoname;
                    //int a, b;
                    //StreamReader sr = new StreamReader(lbAuto.Items[i].ToString(), System.Text.Encoding.Default);
                    //string str = sr.ReadToEnd();
                    //a = str.IndexOf("DirectShowSource(\"");
                    //if (a == -1)
                    //{
                    //    a = str.IndexOf("FFVideoSource(\"");
                    //    b = str.IndexOf("\"", a + 15);
                    //    tempvideoname = str.Substring(a + 15, b - a - 15);
                    //}
                    //else
                    //{
                    //    b = str.IndexOf("\"", a + 18);
                    //    tempvideoname = str.Substring(a + 18, b - a - 18);
                    //}

                    //if (tempvideoname.IndexOf(":") != 1)
                    //{
                    //    tempvideoname = lbAuto.Items[i].ToString().Substring(0, lbAuto.Items[i].ToString().LastIndexOf("\\") + 1) + tempvideoname;
                    //}
                    //aextract = audiobat(tempvideoname, "temp.aac");



                    //ffmpeg = "\"" + workpath + "\\ffmpeg.exe\" -y -i \"" + tempvideoname + "\" -f wav temp.wav";
                }

                else //如果不是avs
                {
                    aextract = audiobat(lbAuto.Items[i].ToString(), "temp.aac");
                }

                //mux
                string finish = lbAuto.Items[i].ToString().Insert(lbAuto.Items[i].ToString().LastIndexOf("."), "_onekeybatch.mp4");
                finish = finish.Remove(finish.LastIndexOf("."));
                mux = muxbat("temp.mp4", "temp.aac", finish);
                //mux = "\"" + workpath + "\\mp4box.exe\" -add temp.mp4 -add temp.aac -new \"" + finish + "\"";
                auto += aextract + x264 + mux + " \r\ndel temp.aac\r\ndel temp.mp4\r\ndel temp.wav\r\n";
            }

            if (cbshutdown.Checked)
            {
                auto += "\r\n" + syspath + ":\\Windows\\System32\\shutdown -f -s -t 60";
            }
        }


        public void batchAuto2()
        {
            int i;
            auto = "";
            for (i = 0; i < this.lbAuto.Items.Count; i++)
            {
                string sub = "";
                string asssub = lbAuto.Items[i].ToString().Remove(lbAuto.Items[i].ToString().LastIndexOf(".")) + ".ass";
                string ssasub = lbAuto.Items[i].ToString().Remove(lbAuto.Items[i].ToString().LastIndexOf(".")) + ".ssa";
                string srtsub = lbAuto.Items[i].ToString().Remove(lbAuto.Items[i].ToString().LastIndexOf(".")) + ".srt";

                string finish;
                if (Directory.Exists(x264PathTextBox.Text))
                    finish = x264PathTextBox.Text + Path.GetFileNameWithoutExtension(lbAuto.Items[i].ToString()) + "_batch.mp4";
                else
                    finish = AddExt(lbAuto.Items[i].ToString(), "_Batch.mp4");

                x264 = x264bat(lbAuto.Items[i].ToString(), finish).Replace("\r\n", "");

                //判断是否内嵌字幕
                if (x264BatchSubCheckBox.Checked)
                {
                    if (File.Exists(asssub))
                    {
                        sub = asssub;
                    }
                    else if (File.Exists(ssasub))
                    {
                        sub = ssasub;
                    }
                    else if (File.Exists(srtsub))
                    {
                        sub = srtsub;
                    }

                    if (sub != "")
                    {
                        if (x264.IndexOf("--vf") == -1)
                        {
                            x264 += " --vf subtitles --sub \"" + sub + "\"";
                        }
                        else
                        {
                            Regex r = new Regex("--vf\\s\\S*");
                            Match m = r.Match(x264);
                            x264 = x264.Insert(m.Index + m.Value.Length, "/subtitles");
                            x264 += " --sub \"" + sub + "\"";
                        }
                    }
                }
                switch (x264AudioModeComboBox.Text)
                {
                    case "无音频流": x264 += " --acodec none"; break;
                    case "复制音频流": x264 += " --acodec copy"; break;
                    case "外置音频流": x264 += " --audiofile \"" + x264AudioParameterTextBox.Text + "\""; break;
                    case "QTAAC": x264 += " --acodec qtaac " + x264AudioParameterTextBox.Text; break;
                    case "libaacplus": x264 += " --acodec libaacplus " + x264AudioParameterTextBox.Text; break;
                    case "FAAC": x264 += " --acodec faac " + x264AudioParameterTextBox.Text; break;
                    default: ; break;
                }

                if (cbFPS2.Text != "不指定")
                {
                    switch (cbFPS2.Text)
                    {
                        case "23.976": x264 += " --fps 24000/1001"; break;
                        case "24": x264 += " --fps 24"; break;
                        case "25": x264 += " --fps 25"; break;
                        case "29.970": x264 += " --fps 30000/1001"; break;
                        case "30": x264 += " --fps 30"; break;
                        case "50": x264 += " --fps 50"; break;
                        case "59.940": x264 += " --fps 60000/1001"; break;
                        case "60": x264 += " --fps 60"; break;
                        default: x264 += " --fps " + cbFPS2.Text; break;
                    }
                }

                x264 += " \r\n";
                auto += x264;
            }
            if (x264BatchShutdownCheckBox.Checked)
            {
                auto += "\r\n" + syspath + ":\\Windows\\System32\\shutdown -f -s -t 60";
            }
            auto += "\r\ncmd";
        }

        private void btnaudio_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "音频(*.aac;*.mp3;*.mp4)|*.aac;*.mp3;*.mp4|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                nameaudio = openFileDialog1.FileName;
                txtaudio.Text = nameaudio;
            }






        }

        private void btnvideo_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                namevideo = openFileDialog1.FileName;
                txtvideo.Text = namevideo;
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {




        }

        private void btnout_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "视频(*.mp4)|*.mp4";
            DialogResult result = savefile.ShowDialog();

            if (result == DialogResult.OK)
            {
                nameout = savefile.FileName;
                txtout.Text = nameout;
            }
        }





        private void btnmux_Click(object sender, EventArgs e)
        {
            if (namevideo == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //else if (nameaudio == "")
            //{
            //    MessageBox.Show("请选择音频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            else if (nameout == "")
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {

                if (cbFPS.Text == "不指定")
                {
                    mux = "\"" + workpath + "\\mp4box.exe\" -add \"" + namevideo + "\" -add \"" + nameaudio + "\" -new \"" + nameout + "\" \r\n cmd";
                }
                else
                {
                    mux = "\"" + workpath + "\\mp4box.exe\" -fps " + cbFPS.Text + " -add \"" + namevideo + "\" -add \"" + nameaudio + "\" -new \"" + nameout + "\" \r\n cmd";
                }
                batpath = workpath + "\\mux.bat";

                File.WriteAllText(batpath, mux, UnicodeEncoding.Default);
                LogRecord(mux);
                Process.Start(batpath);
            }
        }





        private void btnaextract_Click(object sender, EventArgs e)
        {
            if (namevideo == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {
                aextract = "\"" + workpath + "\\mp4box.exe\" -raw 2 \"" + namevideo + "\"";
                batpath = workpath + "\\aextract.bat";
                File.WriteAllText(batpath, aextract, UnicodeEncoding.Default);
                LogRecord(aextract);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(String.Format(" \r\n有任何建议或疑问可以通过以下方式联系小丸。\nQQ：57655408\n微博：weibo.com/xiaowan3\n百度贴吧ID：小丸到达\n\n\t\t\t发布日期：2012年10月17日\n\t\t\t- ( ゜- ゜)つロ 乾杯~"), "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnvextract_Click(object sender, EventArgs e)
        {
            if (namevideo == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                vextract = "\"" + workpath + "\\mp4box.exe\" -raw 1 \"" + namevideo + "\"";
                batpath = workpath + "\\vextract.bat";
                File.WriteAllText(batpath, vextract, UnicodeEncoding.Default);
                LogRecord(vextract);
                System.Diagnostics.Process.Start(batpath);
            }
        }









        private void txtvideo_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtvideo.Text.ToString()))
            {
                namevideo = txtvideo.Text;
                txtout.Text = AddExt(txtvideo.Text, "_Mux.mp4");
            }
        }

        private void txtaudio_TextChanged(object sender, EventArgs e)
        {
            nameaudio = txtaudio.Text;
        }

        private void txtout_TextChanged(object sender, EventArgs e)
        {
            nameout = txtout.Text;

        }





        private void btnvideo3_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "视频(*.mp4;*.flv;*.mkv)|*.mp4;*.flv;*.mkv|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                namevideo3 = openFileDialog1.FileName;
                txtvideo3.Text = namevideo3;
                autovideo = namevideo3;
                num = autovideo.IndexOf(".");
                autovideo = autovideo.Remove(num);
                autoaudio = autovideo + "_track2.aac";
            }
        }

        private void btnout4_Click(object sender, EventArgs e)
        {
            if (namevideo3 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (txtout4.Text == "")
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                //x264
                oneAuto();
                batpath = workpath + "\\auto.bat";
                File.WriteAllText(batpath, auto, UnicodeEncoding.Default);
                LogRecord(auto);
                System.Diagnostics.Process.Start(batpath);
            }
        }


        private void txtvideo3_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtvideo3.Text.ToString()))
            {
                namevideo3 = txtvideo3.Text;
                txtout4.Text = AddExt(txtvideo3.Text, "_auto.mp4");
            }
        }

        private void txtout4_TextChanged(object sender, EventArgs e)
        {
            nameout4 = txtout4.Text;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }



        private void label5_Click(object sender, EventArgs e)
        {

        }





        private void txtvideo_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtvideo_DragDrop(object sender, DragEventArgs e)
        {
            txtvideo.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void txtaudio_DragDrop(object sender, DragEventArgs e)
        {
            txtaudio.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

        }

        private void txtaudio_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;

        }



        private void txtvideo2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;

        }

        private void txtvideo3_DragDrop(object sender, DragEventArgs e)
        {
            txtvideo3.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

            txtvideo3.Text = namevideo3;
            autovideo = namevideo3;
            num = autovideo.IndexOf(".");
            autovideo = autovideo.Remove(num);
            autoaudio = autovideo + "_track2.aac";

        }

        private void txtvideo3_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            //delete temp files
            string[] deletedfiles = { "temp.avs", "clip.bat", "aextract.bat", "vextract.bat", "x264.bat", "aac.bat", "auto.bat", "mux.bat", "flv.bat", "mkvmerge.bat", "mkvextract.bat", "tmp.stat.mbtree", "tmp.stat" };
            foreach (string deletedfile in deletedfiles)
            {
                File.Delete(deletedfile);
            }

            //save settings
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["CRF"].Value = numcrf.Value.ToString();
            cfa.AppSettings.Settings["VideoBitrate"].Value = numrate.Value.ToString();
            cfa.AppSettings.Settings["AudioBitrate"].Value = numq.Value.ToString();
            cfa.AppSettings.Settings["Width"].Value = numwidth.Value.ToString();
            cfa.AppSettings.Settings["Height"].Value = numheight.Value.ToString();
            cfa.AppSettings.Settings["x264Parameter"].Value = txth264.Text;
            cfa.AppSettings.Settings["AVSScript"].Value = txtAVS.Text;
            cfa.AppSettings.Settings["NeroaacParameter"].Value = txtNeroaac.Text;
            cfa.AppSettings.Settings["LanguageIndex"].Value = languageComboBox.SelectedIndex.ToString();
            //最后调用save
            cfa.Save();
            // 刷新命名节，在下次检索它时将从磁盘重新读取它。记住应用程序要刷新节点
            ConfigurationManager.RefreshSection("appSettings");

        }

        private void txtvideo4_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtvideo4.Text.ToString()))
            {
                namevideo4 = txtvideo4.Text;
                //string finish = namevideo4.Insert(namevideo4.LastIndexOf(".")-1,"");
                //string ext = namevideo4.Substring(namevideo4.LastIndexOf(".") + 1, 3);
                //finish += "_clip." + ext;
                string finish = namevideo4.Insert(namevideo4.LastIndexOf("."), "_clip");
                txtout5.Text = finish;
            }
        }

        private void txtout5_TextChanged(object sender, EventArgs e)
        {
            nameout5 = txtout5.Text;
        }

        private void btnvideo4_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "视频(*.mp4;*.flv;*.mkv)|*.mp4;*.flv;*.mkv|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                namevideo4 = openFileDialog1.FileName;
                txtvideo4.Text = namevideo4;
            }
        }

        private void btnout5_Click(object sender, EventArgs e)
        {

            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "视频(*.*)|*.*";
            DialogResult result = savefile.ShowDialog();

            if (result == DialogResult.OK)
            {
                nameout5 = savefile.FileName;
                txtout5.Text = nameout5;
            }

        }

        private void txtvideo4_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;

        }

        private void txtvideo4_DragDrop(object sender, DragEventArgs e)
        {
            txtvideo4.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

        }

        private void btnflv_Click(object sender, EventArgs e)
        {
            if (namevideo4 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {

                SaveFileDialog savefile = new SaveFileDialog();
                savefile.Filter = "视频(*.flv)|*.flv";
                DialogResult result = savefile.ShowDialog();

                if (result == DialogResult.OK)
                {
                    nameout5 = savefile.FileName;
                    txtout5.Text = nameout5;
                    ffmpeg = "\"" + workpath + "\\ffmpeg.exe\" -i  \"" + namevideo4 + "\" -c copy -f flv  \"" + nameout5 + "\"";
                    batpath = workpath + "\\flv.bat";
                    File.WriteAllText(batpath, ffmpeg, UnicodeEncoding.Default);
                    System.Diagnostics.Process.Start(batpath);
                }

            }
        }

        public static bool IsWindowsVistaOrNewer
        {
            get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 6); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            var modulename = Process.GetCurrentProcess().MainModule.ModuleName;
            var procesname = Path.GetFileNameWithoutExtension(modulename);
            Process[] processes = Process.GetProcessesByName(procesname);
            if (processes.Length > 1)
            {
                MessageBox.Show("你已经打开了一个小丸工具箱喔！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ja-JP");

            //use YAHEI in VistaOrNewer
            //if (IsWindowsVistaOrNewer)
            //{
            //    FontFamily myFontFamily = new FontFamily("微软雅黑"); //采用哪种字体
            //    Font myFont = new Font(myFontFamily, 9, FontStyle.Regular); //字是那种字体，显示的风格
            //    this.Font = myFont;
            //}

            //define workpath
            string temppath = System.Windows.Forms.Application.StartupPath;
            workpath = temppath;

            //select priority as normal

            x264PriorityComboBox.SelectedIndex = 2;
            x264AudioModeComboBox.SelectedIndex = 4;
            DemuxerComboBox.SelectedIndex = 2;
            AudioEncoderComboBox.SelectedIndex = 0;
            languageComboBox.SelectedIndex = 0;


            try
            {
                //load settings

                if (File.Exists("app.config"))
                {
                    numcrf.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["CRF"]);
                    numrate.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["VideoBitrate"]);
                    numq.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["AudioBitrate"]);
                    numwidth.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["Width"]);
                    numheight.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["Height"]);
                    txth264.Text = ConfigurationManager.AppSettings["x264Parameter"];
                    txtAVS.Text = ConfigurationManager.AppSettings["AVSScript"];
                    txtNeroaac.Text = ConfigurationManager.AppSettings["NeroaacParameter"];
                    languageComboBox.SelectedIndex = int.Parse(ConfigurationManager.AppSettings["LanguageIndex"]);
                }
            }
            catch (Exception)
            {
                throw;
            }




            //create directory
            string preset = workpath + "\\preset";
            if (!Directory.Exists(preset)) Directory.CreateDirectory(preset);

            DirectoryInfo TheFolder = new DirectoryInfo("preset");
            foreach (FileInfo FileName in TheFolder.GetFiles())
            {
                cbX264.Items.Add(FileName.Name.Replace(".txt", ""));
            }



            DirectoryInfo folder = new DirectoryInfo(workpath);
            foreach (FileInfo FileName in folder.GetFiles())
            {
                if (FileName.Name.IndexOf("x264") != -1 && FileName.Name.IndexOf(".exe") != -1)
                {
                    cbx264file.Items.Add(FileName.Name);
                }
            }
            cbx264file.SelectedIndex = cbx264file.Items.IndexOf("x264_32_tMod-8bit-420.exe");
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "视频(*.mkv)|*.mkv";
            DialogResult result = openFileDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                namevideo6 = openFileDialog1.FileName;
                txtvideo6.Text = namevideo6;
            }
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (namevideo6 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);


            }


            else
            {
                mkvextract = workpath + "\\ mkvextract.exe tracks \"" + namevideo6 + "\" 1:video.h264 2:audio.aac";
                batpath = workpath + "\\mkvextract.bat";
                File.WriteAllText(batpath, mkvextract, UnicodeEncoding.Default);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "视频(*.mp4)|*.mp4|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                namevideo5 = openFileDialog1.FileName;
                txtvideo5.Text = namevideo5;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "音频(*.mp3)|*.mp3|音频(*.aac)|*.aac|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                nameaudio3 = openFileDialog1.FileName;
                txtaudio3.Text = nameaudio3;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "视频(*.mkv)|*.mkv";
            DialogResult result = savefile.ShowDialog();

            if (result == DialogResult.OK)
            {
                nameout6 = savefile.FileName;
                txtout6.Text = nameout6;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (namevideo5 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            else if (nameaudio3 == "")
            {
                MessageBox.Show("请选择音频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else if (nameout6 == "")
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }



            else
            {

                mkvmerge = workpath + "\\mkvmerge.exe -o \"" + nameout6 + "\"   \"" + namevideo5 + "\"   \"" + nameaudio3 + "\"";
                batpath = workpath + "\\mkvmerge.bat";

                File.WriteAllText(batpath, mkvmerge, UnicodeEncoding.Default);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void txtout6_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                namevideo5 = openFileDialog1.FileName;
                txtvideo5.Text = namevideo5;
            }
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "视频(*.mkv)|*.mkv";
            DialogResult result = openFileDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                namevideo6 = openFileDialog1.FileName;
                txtvideo6.Text = namevideo6;
            }
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (namevideo5 == "" && nameaudio3 == "")
            {
                MessageBox.Show("请选择文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {
                if (txtaudio3.Text != "" && txtsub.Text != "")
                {
                    mkvmerge = "\"" + workpath + "\\mkvmerge.exe\" -o \"" + nameout6 + "\" \"" + namevideo5 + "\" \"" + nameaudio3 + "\" \"" + namesub + "\"";
                }

                if (txtaudio3.Text == "" && txtsub.Text == "")
                {
                    mkvmerge = "\"" + workpath + "\\mkvmerge.exe\" -o \"" + nameout6 + "\" \"" + namevideo5 + "\"";
                }


                if (txtaudio3.Text != "" && txtsub.Text == "")
                {
                    mkvmerge = "\"" + workpath + "\\mkvmerge.exe\" -o \"" + nameout6 + "\" \"" + namevideo5 + "\" \"" + nameaudio3 + "\"";
                }

                if (txtaudio3.Text == "" && txtsub.Text != "")
                {
                    mkvmerge = "\"" + workpath + "\\mkvmerge.exe\" -o \"" + nameout6 + "\" \"" + namevideo5 + "\" \"" + namesub + "\"";
                }

                mkvmerge += "\r\ncmd";

                batpath = workpath + "\\mkvmerge.bat";

                File.WriteAllText(batpath, mkvmerge, UnicodeEncoding.Default);
                LogRecord(mkvmerge);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "视频(*.mkv)|*.mkv";
            DialogResult result = savefile.ShowDialog();

            if (result == DialogResult.OK)
            {
                nameout6 = savefile.FileName;
                txtout6.Text = nameout6;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "音频(*.mp3;*.aac)|*.mp3;*.aac|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                nameaudio3 = openFileDialog1.FileName;
                txtaudio3.Text = nameaudio3;
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ASS字幕(*.ass;*.srt)|*.ass;*.srt|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                namesub = openFileDialog1.FileName;
                txtsub.Text = namesub;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (namevideo6 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                int i = namevideo6.IndexOf(".mkv");
                string mkvname = namevideo6.Remove(i);
                mkvextract = "\"" + workpath + "\\mkvextract.exe\" tracks \"" + namevideo6 + "\" 1:\"" + mkvname + "_video.h264\" 2:\"" + mkvname + "_audio.aac\"";
                batpath = workpath + "\\mkvextract.bat";
                File.WriteAllText(batpath, mkvextract, UnicodeEncoding.Default);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void txtvideo5_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtvideo5.Text.ToString()))
            {
                namevideo5 = txtvideo5.Text;
                string finish = namevideo5.Remove(namevideo5.LastIndexOf("."));
                finish += "_mkv封装.mkv";
                txtout6.Text = finish;
            }
        }

        private void txtaudio3_TextChanged(object sender, EventArgs e)
        {
            nameaudio3 = txtaudio3.Text;
        }

        private void txtsub_TextChanged(object sender, EventArgs e)
        {
            namesub = txtsub.Text;
        }

        private void txtout6_TextChanged_1(object sender, EventArgs e)
        {
            nameout6 = txtout6.Text;
        }

        private void txtvideo6_TextChanged(object sender, EventArgs e)
        {
            namevideo6 = txtvideo6.Text;
        }

        private void txtvideo6_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtvideo5_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;

        }

        private void txtaudio3_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;

        }

        private void txtsub_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;

        }

        private void txtvideo5_DragDrop(object sender, DragEventArgs e)
        {
            txtvideo5.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

        }

        private void txtaudio3_DragDrop(object sender, DragEventArgs e)
        {
            txtaudio3.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

        }

        private void txtsub_DragDrop(object sender, DragEventArgs e)
        {
            txtsub.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

        }

        private void txtvideo6_DragDrop(object sender, DragEventArgs e)
        {
            txtvideo6.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void btnMP4_Click(object sender, EventArgs e)
        {
            if (namevideo4 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {

                SaveFileDialog savefile = new SaveFileDialog();
                savefile.Filter = "视频(*.mp4)|*.mp4";
                DialogResult result = savefile.ShowDialog();

                if (result == DialogResult.OK)
                {
                    nameout5 = savefile.FileName;
                    txtout5.Text = nameout5;
                    ffmpeg = "\"" + workpath + "\\ffmpeg.exe\" -i  \"" + namevideo4 + "\" -c copy  \"" + nameout5 + "\"";
                    batpath = workpath + "\\flv.bat";
                    File.WriteAllText(batpath, ffmpeg, UnicodeEncoding.Default);
                    System.Diagnostics.Process.Start(batpath);
                }

            }
        }

        private void btnAutoAdd_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                lbAuto.Items.AddRange(openFileDialog1.FileNames);
            }
        }

        private void btnAutoDel_Click(object sender, EventArgs e)
        {
            if (lbAuto.Items.Count > 0)
            {
                if (lbAuto.SelectedItems.Count > 0)
                {
                    int index = lbAuto.SelectedIndex;
                    lbAuto.Items.RemoveAt(lbAuto.SelectedIndex);
                    if (index == lbAuto.Items.Count)
                    {
                        lbAuto.SelectedIndex = index - 1;
                    }
                    if (index >= 0 && index < lbAuto.Items.Count && lbAuto.Items.Count > 0)
                    {
                        lbAuto.SelectedIndex = index;
                    }
                }
            }
        }

        private void btnAutoClear_Click(object sender, EventArgs e)
        {
            lbAuto.Items.Clear();
        }



        private void lbAuto_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String s in files)
                {
                    (sender as ListBox).Items.Add(s);
                }
            }
            ListBox listbox = (ListBox)sender;
            indexoftarget = listbox.IndexFromPoint(listbox.PointToClient(new Point(e.X, e.Y)));
            if (indexoftarget != ListBox.NoMatches)
            {
                string temp = listbox.Items[indexoftarget].ToString();
                listbox.Items[indexoftarget] = listbox.Items[indexofsource];
                listbox.Items[indexofsource] = temp;
                listbox.SelectedIndex = indexoftarget;
            }
        }

        private void lbAuto_DragEnter(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //    e.Effect = DragDropEffects.All;
            //else e.Effect = DragDropEffects.None;
        }

        private void lbAuto_DragOver(object sender, DragEventArgs e)
        {
            //拖动源和放置的目的地一定是一个ListBox
            if (e.Data.GetDataPresent(typeof(System.String)) && ((ListBox)sender).Equals(lbAuto))
            {
                e.Effect = DragDropEffects.Move;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else e.Effect = DragDropEffects.None;
        }

        private void lbAuto_MouseDown(object sender, MouseEventArgs e)
        {
            indexofsource = ((ListBox)sender).IndexFromPoint(e.X, e.Y);
            if (indexofsource != ListBox.NoMatches)
            {
                ((ListBox)sender).DoDragDrop(((ListBox)sender).Items[indexofsource].ToString(), DragDropEffects.All);
            }
        }

        private void btnBatchAuto_Click(object sender, EventArgs e)
        {
            if (lbAuto.Items.Count != 0)
            {
                batchAuto2();
                batpath = workpath + "\\auto.bat";
                File.WriteAllText(batpath, auto, UnicodeEncoding.Default);
                LogRecord(auto);
                System.Diagnostics.Process.Start(batpath);
            }
            else MessageBox.Show("请输入视频！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void lbffmpeg_MouseDown(object sender, MouseEventArgs e)
        {
            indexofsource = ((ListBox)sender).IndexFromPoint(e.X, e.Y);
            if (indexofsource != ListBox.NoMatches)
            {
                ((ListBox)sender).DoDragDrop(((ListBox)sender).Items[indexofsource].ToString(), DragDropEffects.All);
            }
        }

        private void lbffmpeg_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String s in files)
                {
                    (sender as ListBox).Items.Add(s);
                }
            }
            ListBox listbox = (ListBox)sender;
            indexoftarget = listbox.IndexFromPoint(listbox.PointToClient(new Point(e.X, e.Y)));
            if (indexoftarget != ListBox.NoMatches)
            {
                string temp = listbox.Items[indexoftarget].ToString();
                listbox.Items[indexoftarget] = listbox.Items[indexofsource];
                listbox.Items[indexofsource] = temp;
                listbox.SelectedIndex = indexoftarget;
            }
        }

        private void lbffmpeg_DragOver(object sender, DragEventArgs e)
        {
            //拖动源和放置的目的地一定是一个ListBox
            if (e.Data.GetDataPresent(typeof(System.String)) && ((ListBox)sender).Equals(lbffmpeg))
            {
                e.Effect = DragDropEffects.Move;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else e.Effect = DragDropEffects.None;
        }

        private void btnffmpegAdd_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                lbffmpeg.Items.AddRange(openFileDialog1.FileNames);
            }
        }

        private void btnffmpegDel_Click(object sender, EventArgs e)
        {
            if (lbffmpeg.Items.Count > 0)
            {
                if (lbffmpeg.SelectedItems.Count > 0)
                {
                    int index = lbffmpeg.SelectedIndex;
                    lbffmpeg.Items.RemoveAt(lbffmpeg.SelectedIndex);
                    if (index == lbffmpeg.Items.Count)
                    {
                        lbffmpeg.SelectedIndex = index - 1;
                    }
                    if (index >= 0 && index < lbffmpeg.Items.Count && lbffmpeg.Items.Count > 0)
                    {
                        lbffmpeg.SelectedIndex = index;
                    }
                }
            }
        }

        private void btnffmpegClear_Click(object sender, EventArgs e)
        {
            lbffmpeg.Items.Clear();
        }

        private void btnBatchFLV_Click(object sender, EventArgs e)
        {
            if (lbffmpeg.Items.Count != 0)
            {
                string finish;
                int i;
                ffmpeg = "";
                for (i = 0; i < this.lbffmpeg.Items.Count; i++)
                {
                    finish = lbffmpeg.Items[i].ToString().Remove(lbffmpeg.Items[i].ToString().LastIndexOf(".")) + "_FLV封装.flv";

                    ffmpeg += "\"" + workpath + "\\ffmpeg.exe\" -i \"" + lbffmpeg.Items[i].ToString() + "\" -c copy -f flv \"" + finish + "\" \r\n";
                }
                ffmpeg += "\r\ncmd";
                batpath = workpath + "\\flv.bat";
                File.WriteAllText(batpath, ffmpeg, UnicodeEncoding.Default);
                LogRecord(ffmpeg);
                System.Diagnostics.Process.Start(batpath);
                //lbffmpeg.Items.Clear();
            }
            else MessageBox.Show("请输入视频！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);


        }

        private void btnBatchMP4_Click(object sender, EventArgs e)
        {
            if (lbffmpeg.Items.Count != 0)
            {
                string finish;
                int i;
                ffmpeg = "";
                for (i = 0; i < this.lbffmpeg.Items.Count; i++)
                {
                    finish = lbffmpeg.Items[i].ToString().Remove(lbffmpeg.Items[i].ToString().LastIndexOf(".")) + "_MP4封装.mp4";
                    ffmpeg += "\"" + workpath + "\\ffmpeg.exe\" -i \"" + lbffmpeg.Items[i].ToString() + "\" -c copy -f mp4 \"" + finish + "\" \r\n";
                }
                batpath = workpath + "\\flv.bat";
                File.WriteAllText(batpath, ffmpeg, UnicodeEncoding.Default);
                LogRecord(ffmpeg);
                System.Diagnostics.Process.Start(batpath);
                //lbffmpeg.Items.Clear();

            }
            else MessageBox.Show("请输入视频！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void txtvideo8_TextChanged(object sender, EventArgs e)
        {
            namevideo8 = txtvideo8.Text;
        }

        private void txtvideo8_DragDrop(object sender, DragEventArgs e)
        {
            txtvideo8.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void txtvideo8_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void btnvextract8_Click(object sender, EventArgs e)
        {
            if (namevideo8 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                vextract = "\"" + workpath + "\\FLVExtractCL.exe\" -v \"" + namevideo8 + "\"";
                batpath = workpath + "\\vextract.bat";
                File.WriteAllText(batpath, vextract, UnicodeEncoding.Default);
                LogRecord(vextract);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void btnaextract8_Click(object sender, EventArgs e)
        {
            if (namevideo8 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                aextract = "\"" + workpath + "\\FLVExtractCL.exe\" -a \"" + namevideo8 + "\"";
                batpath = workpath + "\\aextract.bat";
                File.WriteAllText(batpath, aextract, UnicodeEncoding.Default);
                LogRecord(aextract);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void btnvideo8_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "视频(*.flv;*.hlv)|*.flv;*.hlv";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                namevideo8 = openFileDialog1.FileName;
                txtvideo8.Text = namevideo;
            }
        }

        private void btnpreview9_Click(object sender, EventArgs e)
        {
            if (txtAVS.Text != "")
            {
                string filepath = workpath + "\\temp.avs";
                File.WriteAllText(filepath, txtAVS.Text.ToString(), UnicodeEncoding.Default);
                PreviewForm form2 = new PreviewForm();
                form2.Show();
                form2.axWindowsMediaPlayer1.URL = filepath;
            }
            else
            {
                MessageBox.Show("请输入正确的AVS脚本！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtout_DragDrop(object sender, DragEventArgs e)
        {
            txtout.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

        }

        private void txtout_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtout2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtout4_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtout6_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtout4_DragDrop(object sender, DragEventArgs e)
        {
            txtout4.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }


        private void txtout_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtout.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtout.Text.ToString());
            }
        }


        private void txtout3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtout3.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtout3.Text.ToString());
            }
        }

        private void txtout4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtout4.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtout4.Text.ToString());
            }
        }

        private void txtout6_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtout6.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtout6.Text.ToString());
            }
        }

        private void txtout9_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtout9.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtout9.Text.ToString());
            }
        }

        private void txtvideo9_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtsub9_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtout9_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtvideo9_DragDrop(object sender, DragEventArgs e)
        {
            txtvideo9.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void txtsub9_DragDrop(object sender, DragEventArgs e)
        {
            txtsub9.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void txtout9_DragDrop(object sender, DragEventArgs e)
        {
            txtout9.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void txtvideo9_TextChanged(object sender, EventArgs e)
        {

            if (File.Exists(txtvideo9.Text.ToString()))
            {
                namevideo9 = txtvideo9.Text;
                string finish = namevideo9.Remove(namevideo9.LastIndexOf("."));
                finish += "_AVS压制.mp4";
                txtout9.Text = finish;
                GenerateAVS();
            }

            //if (txtvideo9.Text != "")
            //{
            //    if (txtAVS.Text != "")
            //    {
            //        txtAVS.Text = txtAVS.Text.Replace(prevideo9, txtvideo9.Text);
            //    }
            //    else
            //    {
            //        DirectoryInfo TheFolder = new DirectoryInfo("avsfilter");
            //        foreach (FileInfo FileName in TheFolder.GetFiles())
            //        {
            //            avs += "LoadPlugin(\"" + workpath + "\\avsfilter\\" + FileName + "\")\r\n";
            //        }
            //        avs += "\r\nDirectShowSource(\"" + namevideo9 + "\",23.976,convertFPS=True)\r\nConvertToYV12()\r\n" + "TextSub(\"" + namesub9 + "\")\r\n";
            //        txtAVS.Text = avs;
            //        avs = "";
            //    }
            //    prevideo9 = txtvideo9.Text;
            //}

        }

        private void txtsub9_TextChanged(object sender, EventArgs e)
        {
            namesub9 = txtsub9.Text;
            GenerateAVS();

            //if (txtAVS.Text != "")
            //{
            //    txtAVS.Text=txtAVS.Text.Replace(namesub9, txtsub9.Text);
            //    namesub9 = txtsub9.Text;
            //}

            //else
            //{
            //    namesub9 = txtsub9.Text;
            //    DirectoryInfo TheFolder = new DirectoryInfo("avsfilter");
            //    foreach (FileInfo FileName in TheFolder.GetFiles())
            //    {
            //        avs += "LoadPlugin(\"" + workpath + "\\avsfilter\\" + FileName + "\")\r\n";
            //    }
            //    avs += "\r\nDirectShowSource(\"" + namevideo9 + "\",23.976,convertFPS=True)\r\nConvertToYV12()\r\n" + "TextSub(\"" + namesub9 + "\")\r\n";
            //    txtAVS.Text = avs;
            //    avs = "";
            //}
        }

        private void txtout9_TextChanged(object sender, EventArgs e)
        {
            nameout9 = txtout9.Text;
        }

        private void btnAVS9_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(nameout9))
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string filepath = workpath + "\\temp.avs";
                File.WriteAllText(filepath, txtAVS.Text, UnicodeEncoding.Default);
                x264 = x264bat(filepath, nameout9).Replace("\r\n", "");
                x264 += " --acodec none\r\n";
                batpath = workpath + "\\x264.bat";
                File.WriteAllText(batpath, x264, UnicodeEncoding.Default);
                LogRecord(x264);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void btnout9_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "视频(*.mp4)|*.mp4";
            DialogResult result = savefile.ShowDialog();

            if (result == DialogResult.OK)
            {
                txtout9.Text = nameout9 = savefile.FileName;
            }
        }

        private void btnAVSone_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(nameout9))
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                string filepath = workpath + "\\temp.avs";
                File.WriteAllText(filepath, txtAVS.Text, UnicodeEncoding.Default);

                x264 = x264bat(filepath, "temp.mp4").Replace("\r\n", "");
                x264 += " --acodec none\r\n";
                //audio
                aextract = audiobat(namevideo9, "temp.aac");
                //mux
                mux = muxbat("temp.mp4", "temp.aac", "23.976", nameout9);

                if (cbDelTmp.Checked == false)
                {
                    auto = aextract + x264 + "\r\n" + mux + " \r\ndel temp.aac\r\ndel temp.mp4\r\ndel temp.wav\r\ncmd ";
                }
                else
                {
                    auto = aextract + x264 + "\r\n" + mux + " \r\ncmd";
                }
                batpath = workpath + "\\auto.bat";
                File.WriteAllText(batpath, auto, UnicodeEncoding.Default);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void button6_Click_2(object sender, EventArgs e)
        {
            //if (Directory.Exists("avsfilter"))
            //{
            //    DirectoryInfo TheFolder = new DirectoryInfo("avsfilter");
            //    foreach (FileInfo FileName in TheFolder.GetFiles())
            //    {
            //        avs += "LoadPlugin(\"" + workpath + "\\avsfilter\\" + FileName + "\")\r\n";
            //    }
            //}

            avs += "LoadPlugin(\"avsfilter\\VSFilter.DLL\")\r\n";
            avs += string.Format("\r\nDirectShowSource(\"{0}\",23.976,convertFPS=True)\r\nConvertToYV12()\r\nCrop(0,0,0,0)\r\nAddBorders(0,0,0,0)\r\n" + "TextSub(\"{1}\")\r\n#LanczosResize(1280,960)\r\n", namevideo9, namesub9);
            //avs += "\r\nDirectShowSource(\"" + namevideo9 + "\",23.976,convertFPS=True)\r\nConvertToYV12()\r\nCrop(0,0,0,0)\r\nAddBorders(0,0,0,0)\r\n" + "TextSub(\"" + namesub9 + "\")\r\n#LanczosResize(1280,960)\r\n";
            txtAVS.Text = avs;
            avs = "";
        }


        private void txth264_TextChanged(object sender, EventArgs e)
        {

        }


        private void button8_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "视频(*.mp4)|*.mp4";
            DialogResult result = savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                nameout4 = savefile.FileName;
                txtout4.Text = nameout4;
            }
        }

        private void txtvideo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtvideo.Text.ToString());
            }
        }



        private void txtvideo3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo3.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtvideo3.Text.ToString());
            }
        }

        private void txtvideo4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo4.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtvideo4.Text.ToString());
            }
        }

        private void txtout5_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtout5.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtout5.Text.ToString());
            }
        }

        private void txtvideo8_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo8.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtvideo8.Text.ToString());
            }
        }

        private void txtvideo9_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo9.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtvideo9.Text.ToString());
            }
        }

        private void txtvideo6_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo6.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtvideo6.Text.ToString());
            }
        }

        private void txtvideo5_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo5.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtvideo5.Text.ToString());
            }
        }

        private void txtaudio3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtaudio3.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtaudio3.Text.ToString());
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcesses();
            for (int i = 0; i < processes.GetLength(0); i++)
            {
                //我是要找到我需要的YZT.exe的进程,可以根据ProcessName属性判断
                if (processes[i].ProcessName.Equals("x264"))
                {
                    switch (x264PriorityComboBox.SelectedIndex)
                    {
                        case 0: processes[i].PriorityClass = ProcessPriorityClass.Idle; break;
                        case 1: processes[i].PriorityClass = ProcessPriorityClass.BelowNormal; break;
                        case 2: processes[i].PriorityClass = ProcessPriorityClass.Normal; break;
                        case 3: processes[i].PriorityClass = ProcessPriorityClass.AboveNormal; break;
                        case 4: processes[i].PriorityClass = ProcessPriorityClass.High; break;
                        case 5: processes[i].PriorityClass = ProcessPriorityClass.RealTime; break;
                    }
                }
            }
        }


        private void btnsub9_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ASS字幕(*.ass;*.ssa)|*.ass;*.ssa|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                namesub9 = openFileDialog1.FileName;
                txtsub9.Text = namesub9;
            }
        }

        private void btnvideo9_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "视频(*.mp4;*.flv;*.mkv)|*.mp4;*.flv;*.mkv|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                namevideo9 = openFileDialog1.FileName;
                txtvideo9.Text = namevideo9;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            txtAVS.Clear();
        }

        private void btnClip_Click(object sender, EventArgs e)
        {
            if (namevideo4 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (nameout5 == "")
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {

                int h1 = int.Parse(maskb.Text.ToString().Substring(0, 2));
                int m1 = int.Parse(maskb.Text.ToString().Substring(3, 2));
                int s1 = int.Parse(maskb.Text.ToString().Substring(6, 2));
                int h2 = int.Parse(maske.Text.ToString().Substring(0, 2));
                int m2 = int.Parse(maske.Text.ToString().Substring(3, 2));
                int s2 = int.Parse(maske.Text.ToString().Substring(6, 2));

                clip = "\"" + workpath + "\\ffmpeg.exe\" -ss " + maskb.Text.ToString() + " -t " + timeminus(h1, m1, s1, h2, m2, s2) + " -i  \"" + namevideo4 + "\" -c copy \"" + nameout5 + "\" \r\ncmd";
                batpath = workpath + "\\clip.bat";
                File.WriteAllText(batpath, clip, UnicodeEncoding.Default);
                Process.Start(batpath);
            }
        }


        private void cbX264_SelectedIndexChanged(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(workpath + "\\preset\\" + cbX264.Text + ".txt", System.Text.Encoding.Default);
            txth264.Text = sr.ReadToEnd();
            sr.Close();
        }

        private void cbFPS_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnMIopen_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "视频(*.mp4;*.flv;*.mkv)|*.mp4;*.flv;*.mkv|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                MIvideo = openFileDialog1.FileName;
                txtMI.Text = MediaInfo(MIvideo);
            }
        }

        private void btnMIplay_Click(object sender, EventArgs e)
        {
            Process.Start(MIvideo);
        }

        private void btnMIcopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(MItext);
        }

        private void txtMI_DragDrop(object sender, DragEventArgs e)
        {
            MIvideo = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            txtMI.Text = MediaInfo(MIvideo);
        }

        private void txtMI_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void x264AudioParameterTextBox_DragDrop(object sender, DragEventArgs e)
        {
            x264AudioParameterTextBox.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
        }

        private void x264AudioParameterTextBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtsub2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }



        private void btnvideo7_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "视频(*.mkv)|*.mkv";
            DialogResult result = openFileDialog1.ShowDialog();


            if (result == DialogResult.OK)
            {
                namevideo6 = openFileDialog1.FileName;
                txtvideo6.Text = namevideo6;
            }
        }

        private void btnextract7_Click(object sender, EventArgs e)
        {
            if (namevideo6 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                int i = namevideo6.IndexOf(".mkv");
                if (i != -1)
                {
                    string mkvname = namevideo6.Remove(i);
                    mkvextract = "\"" + workpath + "\\mkvextract.exe\" tracks \"" + namevideo6 + "\" 1:\"" + mkvname + "_video.h264\" 2:\"" + mkvname + "_audio.aac\"";
                    batpath = workpath + "\\mkvextract.bat";
                    File.WriteAllText(batpath, mkvextract, UnicodeEncoding.Default);
                    LogRecord(mkvextract);
                    System.Diagnostics.Process.Start(batpath);
                }
                else MessageBox.Show("不是MKV文件！");

            }
        }

        private void txtvideo6_DragDrop_1(object sender, DragEventArgs e)
        {
            txtvideo6.Text = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

        }

        private void txtvideo6_DragEnter_1(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void txtMI_TextChanged(object sender, EventArgs e)
        {
            MItext = txtMI.Text;
        }

        private void txtAVScreate_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.nmm-hd.org/newbbs/viewtopic.php?f=8&t=219");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.sosg.net/read.php?tid=480646");

        }


        private void txtAVS_DragDrop(object sender, DragEventArgs e)
        {
            string avsfile = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            txtAVS.Text = File.ReadAllText(avsfile, Encoding.Default);
        }

        private void txtAVS_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://pan.baidu.com/share/link?shareid=4513&uk=4094576855");
        }

        private void btnaextract2_Click(object sender, EventArgs e)
        {
            if (namevideo == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {
                aextract = "\"" + workpath + "\\mp4box.exe\" -raw 3 \"" + namevideo + "\"";
                batpath = workpath + "\\aextract.bat";
                File.WriteAllText(batpath, aextract, UnicodeEncoding.Default);
                LogRecord(aextract);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void btnaextract3_Click(object sender, EventArgs e)
        {
            if (namevideo == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {
                aextract = "\"" + workpath + "\\mp4box.exe\" -raw 4 \"" + namevideo + "\"";
                batpath = workpath + "\\aextract.bat";
                File.WriteAllText(batpath, aextract, UnicodeEncoding.Default);
                LogRecord(aextract);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void txtvideo6_TextChanged_1(object sender, EventArgs e)
        {
            if (File.Exists(txtvideo6.Text.ToString()))
            {
                namevideo6 = txtvideo6.Text;
            }
        }

        #region 帮助页面

        private void AboutBtn_Click(object sender, EventArgs e)
        {
            QQMessageBox.Show(
                this,
                "小丸工具箱 独立开发最终版\r\n主页：http://maruko.appinn.me/ \r\n发布日期：2013年10月4日",
                "关于",
                QQMessageBoxIcon.Information,
                QQMessageBoxButtons.OK);
        }

        private void HomePageBtn_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://maruko.appinn.me/");
        }

        #endregion

        #region 视频页面

        private void x264VideoBtn_Click(object sender, EventArgs e)
        {
            ////OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "视频(*.mp4;*.flv;*.mkv;*.avi)|*.mp4;*.flv;*.mkv;*.avi|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                namevideo2 = openFileDialog1.FileName;
                x264VideoTextBox.Text = namevideo2;
            }
        }

        private void x264OutBtn_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "视频(*.mp4)|*.mp4";
            DialogResult result = savefile.ShowDialog();

            if (result == DialogResult.OK)
            {
                nameout2 = savefile.FileName;
                x264OutTextBox.Text = nameout2;
            }
        }

        private void x264SubBtn_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ASS字幕(*.ass;*.ssa)|*.ass;*.ssa|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                namesub2 = openFileDialog1.FileName;
                x264SubTextBox.Text = namesub2;
            }
        }

        private void x264StartBtn_Click(object sender, EventArgs e)
        {
            if (namevideo2 == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (nameout2 == "")
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {
                x264 = x264bat(namevideo2, nameout2).Replace("\r\n", "");

                if (x264SubTextBox.Text != "")
                {
                    if (x264.IndexOf("--vf") == -1)
                    {
                        x264 += " --vf subtitles --sub \"" + namesub2 + "\"";
                    }
                    else
                    {
                        Regex r = new Regex("--vf\\s\\S*");
                        Match m = r.Match(x264);
                        x264 = x264.Insert(m.Index + m.Value.Length, "/subtitles");
                        x264 += " --sub \"" + namesub2 + "\"";
                    }
                }

                switch (x264AudioModeComboBox.Text)
                {
                    case "无音频流": x264 += " --acodec none"; break;
                    case "复制音频流": x264 += " --acodec copy"; break;
                    case "外置音频流": x264 += " --audiofile \"" + x264AudioParameterTextBox.Text + "\""; break;
                    case "QTAAC": x264 += " --acodec qtaac " + x264AudioParameterTextBox.Text; break;
                    case "libaacplus": x264 += " --acodec libaacplus " + x264AudioParameterTextBox.Text; break;
                    case "FAAC": x264 += " --acodec faac " + x264AudioParameterTextBox.Text; break;
                    default: ; break;
                }


                if (cbFPS2.Text != "不指定")
                {
                    switch (cbFPS2.Text)
                    {
                        case "23.976": x264 += " --fps 24000/1001"; break;
                        case "24": x264 += " --fps 24"; break;
                        case "25": x264 += " --fps 25"; break;
                        case "29.970": x264 += " --fps 30000/1001"; break;
                        case "30": x264 += " --fps 30"; break;
                        case "50": x264 += " --fps 50"; break;
                        case "59.940": x264 += " --fps 60000/1001"; break;
                        case "60": x264 += " --fps 60"; break;
                        default: x264 += " --fps " + cbFPS2.Text; break;
                    }
                }

                x264 += "\r\n";

                if (x264FLVCheckBox.Checked == true)
                {
                    string flvfile = AddExt(nameout2, "_FLV.flv");
                    x264 += "\r\n\"" + workpath + "\\ffmpeg.exe\" -i  \"" + nameout2 + "\" -c copy -f flv  \"" + flvfile + "\" \r\n";
                }


                if (x264ShutdownCheckBox.Checked)
                {
                    x264 += "\r\n" + syspath + ":\\Windows\\System32\\shutdown -f -s -t 60";
                }

                x264 += "\r\ncmd";
                batpath = workpath + "\\x264.bat";
                File.WriteAllText(batpath, x264, UnicodeEncoding.Default);
                LogRecord(x264);
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void x264AddPresetBtn_Click(object sender, EventArgs e)
        {
            //create directory
            string preset = workpath + "\\preset";
            if (!Directory.Exists(preset)) Directory.CreateDirectory(preset);

            //add file
            aextract = "\"" + workpath + "\\FLVExtractCL.exe\" -a \"" + namevideo8 + "\"";
            batpath = workpath + "\\preset\\" + PresetNameTextBox.Text + ".txt";
            File.WriteAllText(batpath, txth264.Text, UnicodeEncoding.Default);

            //refresh combobox
            cbX264.Items.Clear();
            if (Directory.Exists(workpath + "\\preset"))
            {
                DirectoryInfo TheFolder = new DirectoryInfo("preset");
                foreach (FileInfo FileName in TheFolder.GetFiles())
                {
                    cbX264.Items.Add(FileName.Name.Replace(".txt", ""));
                }
            }
            if (cbX264.Items.Count > 0) cbX264.SelectedIndex = 0;
        }

        private void x264DeletePresetBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要删除这条预设参数？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                string name = batpath = workpath + "\\preset\\" + cbX264.Text + ".txt";
                File.Delete(name);

                cbX264.Items.Clear();
                if (Directory.Exists(workpath + "\\preset"))
                {
                    DirectoryInfo TheFolder = new DirectoryInfo("preset");
                    foreach (FileInfo FileName in TheFolder.GetFiles())
                    {
                        cbX264.Items.Add(FileName.Name.Replace(".txt", ""));
                    }
                }
                if (cbX264.Items.Count > 0) cbX264.SelectedIndex = 0;
            }
        }


        private void x264Mode2RadioButton_CheckedChanged(object sender, EventArgs e)
        {

            mode = 2;
            lbrate.Visible = true;
            numrate.Visible = true;
            label12.Visible = true;
            cbFPS2.Visible = true;
            lbFPS2.Visible = true;

            lbwidth.Visible = true;
            lbheight.Visible = true;
            numwidth.Visible = true;
            numheight.Visible = true;
            MaintainResolutionCheckBox.Visible = true;


            lbcrf.Visible = false;
            numcrf.Visible = false;

            label4.Visible = false;
            txth264.Visible = false;
            cbX264.Visible = false;
            x264AddPresetBtn.Visible = false;
            x264DeletePresetBtn.Visible = false;
            PresetNameTextBox.Visible = false;
        }

        private void x264Mode3RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            mode = 0;

            label4.Visible = true;
            txth264.Visible = true;
            cbX264.Visible = true;
            x264AddPresetBtn.Visible = true;
            x264DeletePresetBtn.Visible = true;
            PresetNameTextBox.Visible = true;

            lbwidth.Visible = false;
            lbheight.Visible = false;
            numwidth.Visible = false;
            numheight.Visible = false;
            MaintainResolutionCheckBox.Visible = false;


            lbrate.Visible = false;
            numrate.Visible = false;
            label12.Visible = false;

            lbcrf.Visible = false;
            numcrf.Visible = false;
            cbFPS2.Visible = false;
            lbFPS2.Visible = false;

        }

        private void x264Mode1RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            mode = 1;
            lbcrf.Visible = true;
            numcrf.Visible = true;
            cbFPS2.Visible = true;
            lbFPS2.Visible = true;

            lbwidth.Visible = true;
            lbheight.Visible = true;
            numwidth.Visible = true;
            numheight.Visible = true;
            MaintainResolutionCheckBox.Visible = true;

            lbrate.Visible = false;
            numrate.Visible = false;
            label12.Visible = false;


            label4.Visible = false;
            txth264.Visible = false;
            cbX264.Visible = false;
            x264AddPresetBtn.Visible = false;
            x264DeletePresetBtn.Visible = false;
            PresetNameTextBox.Visible = false;
        }

        private void x264AudioModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (x264AudioModeComboBox.SelectedIndex != -1)
            {
                switch (x264AudioModeComboBox.SelectedIndex)
                {
                    case 0:
                        x264AudioParameterTextBox.Text = "";
                        break;
                    case 1:
                        x264AudioParameterTextBox.Text = "可能压制失败，如出错请改用FAAC";
                        break;
                    case 2:
                        x264AudioParameterTextBox.Text = "把音频文件拖到这里";
                        break;
                    case 3:
                        x264AudioParameterTextBox.Text = "--abitrate 128";
                        break;
                    case 4:
                        x264AudioParameterTextBox.Text = "--abitrate 128";
                        break;
                    case 5:
                        x264AudioParameterTextBox.Text = "";
                        break;
                    default:
                        break;
                }
            }
        }

        private void x264PriorityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string processName = cbx264file.Text;
            processName = processName.Replace(".exe", "");

            Process[] processes = Process.GetProcesses();

            //if (x264PriorityComboBox.SelectedIndex == 4 || x264PriorityComboBox.SelectedIndex == 5)
            //{
            //    if (MessageBox.Show("优先级那么高的话会严重影响其他进程的运行速度，\r\n是否继续？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            //    {
            //        x264PriorityComboBox.SelectedIndex = 2;
            //    }
            //}


            //遍历电脑中的进程
            for (int i = 0; i < processes.GetLength(0); i++)
            {
                //我是要找到我需要的YZT.exe的进程,可以根据ProcessName属性判断
                if (processes[i].ProcessName.Equals(processName))
                {
                    switch (x264PriorityComboBox.SelectedIndex)
                    {
                        case 0: processes[i].PriorityClass = ProcessPriorityClass.Idle; break;
                        case 1: processes[i].PriorityClass = ProcessPriorityClass.BelowNormal; break;
                        case 2: processes[i].PriorityClass = ProcessPriorityClass.Normal; break;
                        case 3: processes[i].PriorityClass = ProcessPriorityClass.AboveNormal; break;
                        case 4: processes[i].PriorityClass = ProcessPriorityClass.High; break;
                        case 5: processes[i].PriorityClass = ProcessPriorityClass.RealTime; break;
                    }
                }
            }
        }
        private void x264VideoTextBox_TextChanged(object sender, EventArgs e)
        {
            string path = x264VideoTextBox.Text;
            if (File.Exists(path))
            {
                namevideo2 = path;
                x264OutTextBox.Text = AddExt(path, "_x264.mp4");
                //txtsub2.Text = AddExt(path, ".ass");
            }
        }

        private void x264OutTextBox_TextChanged(object sender, EventArgs e)
        {
            nameout2 = x264OutTextBox.Text;
        }

        private void x264SubTextBox_TextChanged(object sender, EventArgs e)
        {
            namesub2 = x264SubTextBox.Text;
        }




        private void x264BatchClearBtn_Click(object sender, EventArgs e)
        {
            lbAuto.Items.Clear();

        }

        private void x264BatchDeleteBtn_Click(object sender, EventArgs e)
        {
            if (lbAuto.Items.Count > 0)
            {
                if (lbAuto.SelectedItems.Count > 0)
                {
                    int index = lbAuto.SelectedIndex;
                    lbAuto.Items.RemoveAt(lbAuto.SelectedIndex);
                    if (index == lbAuto.Items.Count)
                    {
                        lbAuto.SelectedIndex = index - 1;
                    }
                    if (index >= 0 && index < lbAuto.Items.Count && lbAuto.Items.Count > 0)
                    {
                        lbAuto.SelectedIndex = index;
                    }
                }
            }
        }

        private void x264BatchAddBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                lbAuto.Items.AddRange(openFileDialog1.FileNames);
            }
        }

        #endregion


        private void ExtractMP4Button_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "视频(*.mp4)|*.mp4|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                namevideo = openFileDialog1.FileName;
                ExtractMP4TextBox.Text = namevideo;
            }
        }

        private void txtAVS_TextChanged(object sender, EventArgs e)
        {
            Match m = Regex.Match(txtAVS.Text, "ource\\(\"[a-zA-Z]:\\\\.+\\.\\w+\"");
            if (m.Success)
            {
                string str = m.ToString();
                str = str.Replace("ource(\"", "");
                str = str.Replace("\"", "");
                str = AddExt(str, "_AVS.mp4");
                txtout9.Text = str;
            }
        }


        #region 音频界面

        private void AudioListBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String s in files)
                {
                    (sender as ListBox).Items.Add(s);
                }
            }
            ListBox listbox = (ListBox)sender;
            indexoftarget = listbox.IndexFromPoint(listbox.PointToClient(new Point(e.X, e.Y)));
            if (indexoftarget != ListBox.NoMatches)
            {
                string temp = listbox.Items[indexoftarget].ToString();
                listbox.Items[indexoftarget] = listbox.Items[indexofsource];
                listbox.Items[indexofsource] = temp;
                listbox.SelectedIndex = indexoftarget;
            }
        }
        private void AudioListBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(System.String)) && ((ListBox)sender).Equals(lbAuto))
            {
                e.Effect = DragDropEffects.Move;
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else e.Effect = DragDropEffects.None;
        }

        private void AudioListBox_MouseDown(object sender, MouseEventArgs e)
        {
            indexofsource = ((ListBox)sender).IndexFromPoint(e.X, e.Y);
            if (indexofsource != ListBox.NoMatches)
            {
                ((ListBox)sender).DoDragDrop(((ListBox)sender).Items[indexofsource].ToString(), DragDropEffects.All);
            }
        }



        private void AudioBatchButton_Click(object sender, EventArgs e)
        {
            if (AudioListBox.Items.Count != 0)
            {
                string finish;
                aac = "";
                for (int i = 0; i < this.AudioListBox.Items.Count; i++)
                {
                    finish = AddExt(AudioListBox.Items[i].ToString(), "_AAC.aac");
                    aac += audiobat(AudioListBox.Items[i].ToString(), finish);
                    aac += "\r\n";
                }
                aac += "\r\ncmd";
                batpath = workpath + "\\aac.bat";
                File.WriteAllText(batpath, aac, UnicodeEncoding.Default);
                LogRecord(aac);
                System.Diagnostics.Process.Start(batpath);
            }
            else MessageBox.Show("请输入文件！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnaudio2_Click(object sender, EventArgs e)
        {
            //OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                nameaudio2 = openFileDialog1.FileName;
                txtaudio2.Text = nameaudio2;
            }
        }

        private void btnout3_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "音频(*.aac)|*.aac";
            DialogResult result = savefile.ShowDialog();

            if (result == DialogResult.OK)
            {
                nameout3 = savefile.FileName;
                txtout3.Text = nameout3;
            }
        }



        private void btnaac_Click(object sender, EventArgs e)
        {
            if (nameaudio2 == "")
            {
                MessageBox.Show("请选择音频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (nameout3 == "")
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            else
            {
                //ffmpeg = "\"" + workpath + "\\ffmpeg.exe\" -y -i \"" + nameaudio2 + "\" -f wav temp.wav";
                //int AACbr = 1024 * Convert.ToInt32(numq.Value.ToString());
                //string br = AACbr.ToString();
                //neroaac = "\"" + workpath + "\\neroAacEnc.exe\" -ignorelength -lc -br " + br + " -if \"temp.wav\"  -of \"" + nameout3 + "\"";
                //aac = ffmpeg + "&&" + neroaac + "\r\ncmd";
                //if (cbwavtemp.Checked == false)
                //{
                //    aac += "del temp.wav\r\ncmd";
                //}

                batpath = workpath + "\\aac.bat";
                File.WriteAllText(batpath, audiobat(nameaudio2, nameout3), UnicodeEncoding.Default);
                LogRecord(audiobat(nameaudio2, nameout3));
                System.Diagnostics.Process.Start(batpath);
            }
        }

        private void txtaudio2_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtaudio2.Text.ToString()))
            {
                nameaudio2 = txtaudio2.Text;
                txtout3.Text = AddExt(txtaudio2.Text, "_AAC.aac");
            }
        }

        private void txtout3_TextChanged(object sender, EventArgs e)
        {
            nameout3 = txtout3.Text;
        }

        private void txtaudio2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtaudio2.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtaudio2.Text.ToString());
            }
        }
        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            aacmode = 2;

            lbaacrate.Visible = false;
            lbaackbps.Visible = false;
            numq.Visible = false;
            txtNeroaac.Visible = true;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            aacmode = 1;

            lbaacrate.Visible = true;
            lbaackbps.Visible = true;
            numq.Visible = true;
            txtNeroaac.Visible = false;
        }

        private void AudioAddButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                AudioListBox.Items.AddRange(openFileDialog1.FileNames);
            }
        }

        private void AudioDeleteButton_Click(object sender, EventArgs e)
        {
            if (AudioListBox.Items.Count > 0)
            {
                if (AudioListBox.SelectedItems.Count > 0)
                {
                    int index = AudioListBox.SelectedIndex;
                    AudioListBox.Items.RemoveAt(AudioListBox.SelectedIndex);
                    if (index == AudioListBox.Items.Count)
                    {
                        AudioListBox.SelectedIndex = index - 1;
                    }
                    if (index >= 0 && index < AudioListBox.Items.Count && AudioListBox.Items.Count > 0)
                    {
                        AudioListBox.SelectedIndex = index;
                    }
                }
            }
        }

        private void AudioClearButton_Click(object sender, EventArgs e)
        {
            AudioListBox.Items.Clear();
        }

        #endregion


        #region AVS页面

        private void GenerateAVS()
        {
            //if (Directory.Exists("avsfilter"))
            //{
            //    DirectoryInfo TheFolder = new DirectoryInfo("avsfilter");
            //    foreach (FileInfo FileName in TheFolder.GetFiles())
            //    {
            //        avs += "LoadPlugin(\"" + workpath + "\\avsfilter\\" + FileName + "\")\r\n";
            //    }
            //}
            avsBuilder.Remove(0, avsBuilder.Length);

            avsBuilder.AppendLine("LoadPlugin(\"avsfilter\\VSFilter.DLL\")");
            if (UndotCheckBox.Checked) avsBuilder.AppendLine("LoadPlugin(\"avsfilter\\UnDot.DLL\")");
            avsBuilder.AppendLine("DirectShowSource(\"" + namevideo9 + "\")");
            avsBuilder.AppendLine("ConvertToYV12()");

            if (UndotCheckBox.Checked)
                avsBuilder.AppendLine("Undot()");

            if (TweakCheckBox.Checked)
                avsBuilder.AppendLine("Tweak(" + TweakChromaNumericUpDown.Value.ToString() + ", " + TweakSaturationNumericUpDown.Value.ToString() + ", " + TweakBrightnessNumericUpDown.Value.ToString() + ", " + TweakContrastNumericUpDown.Value.ToString() + ")");

            if (LevelsCheckBox.Checked)
                avsBuilder.AppendLine("Levels(0," + LevelsNumericUpDown.Value.ToString() + ",255,0,255)");

            if (LanczosResizeCheckBox.Checked)
                avsBuilder.AppendLine("LanczosResize(" + AVSWidthNumericUpDown.Value.ToString() + "," + AVSHeightNumericUpDown.Value.ToString() + ")");

            if (SharpenCheckBox.Checked)
                avsBuilder.AppendLine("Sharpen(" + SharpenNumericUpDown.Value.ToString() + ")");


            if (CropCheckBox.Checked)
                avsBuilder.AppendLine("Crop(" + AVSCropTextBox.Text + ")");


            if (AddBordersCheckBox.Checked)
                avsBuilder.AppendLine("AddBorders(" + AddBorders1NumericUpDown.Value.ToString() + "," + AddBorders2NumericUpDown.Value.ToString() + "," + AddBorders3NumericUpDown.Value.ToString() + "," + AddBorders4NumericUpDown.Value.ToString() + ")");

            if (AVSSubCheckBox.Checked)
            {
                if (Path.GetExtension(namesub9) == ".idx")
                    avsBuilder.AppendLine("vobsub(\"" + namesub9 + "\")");
                else
                    avsBuilder.AppendLine("TextSub(\"" + namesub9 + "\")");
            }

            if (TrimCheckBox.Checked)
                avsBuilder.AppendLine("Trim(" + TrimStartNumericUpDown.Value.ToString() + "," + TrimEndNumericUpDown.Value.ToString() + ")");

            txtAVS.Text = avsBuilder.ToString();
        }

        #region 更改AVS

        private void TweakCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void LanczosResizeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void AddBordersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void CropCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void TrimCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void LevelsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void SharpenCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void UndotCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void TweakChromaNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void TweakSaturationNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void TweakBrightnessNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void TweakContrastNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void AVSWidthNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void AVSHeightNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void AddBorders1NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void AddBorders2NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void AddBorders3NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void AddBorders4NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void AVSCropTextBox_TextChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void TrimStartNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void TrimEndNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void LevelsNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void SharpenNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }

        private void AVSSubCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GenerateAVS();
        }
        #endregion



        #endregion


        public void Log(string path)
        {
            ProcessStartInfo start = new ProcessStartInfo(path);//设置运行的命令行文件问ping.exe文件，这个文件系统会自己找到
            //如果是其它exe文件，则有可能需要指定详细路径，如运行winRar.exe
            start.CreateNoWindow = false;//不显示dos命令行窗口
            start.RedirectStandardOutput = true;//
            start.RedirectStandardInput = true;//
            start.UseShellExecute = false;//是否指定操作系统外壳进程启动程序
            Process p = Process.Start(start);
            StreamReader reader = p.StandardOutput;//截取输出流
            string line = reader.ReadLine();//每次读取一行
            StringBuilder log = new StringBuilder(2000);
            while (!reader.EndOfStream)
            {
                log.Append(line + "\r\n");
                line = reader.ReadLine();
            }
            p.WaitForExit();//等待程序执行完退出进程
            File.WriteAllText(workpath + "\\log.txt", log.ToString(), UnicodeEncoding.Default);
            p.Close();//关闭进程
            reader.Close();//关闭流
        }

        public void LogRecord(string log)
        {
            File.AppendAllText(workpath + "\\log.txt", "===========" + DateTime.Now.ToString() + "===========\r\n" + log + "\r\n\r\n", UnicodeEncoding.Default);
        }

        private void DeleteLogButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(workpath + "\\log.txt"))
            {
                File.Delete(workpath + "\\log.txt");
                MessageBox.Show("已经删除日志文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else MessageBox.Show("没有找到日志文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ViewLogButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(workpath + "\\log.txt"))
            {
                System.Diagnostics.Process.Start(workpath + "\\log.txt");
            }
            else MessageBox.Show("没有找到日志文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void x264PathButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();
            x264PathTextBox.Text = fbd.SelectedPath;
        }

        private void ExtractMP4TextBox_TextChanged(object sender, EventArgs e)
        {
            namevideo = ExtractMP4TextBox.Text;
        }

        private void MaintainResolutionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (MaintainResolutionCheckBox.Checked)
            {
                numwidth.Value = 0;
                numheight.Value = 0;
                numwidth.Enabled = false;
                numheight.Enabled = false;
            }

            else
            {
                numwidth.Enabled = true;
                numheight.Enabled = true;
            }




        }


        #region globalization

        public static void SetLang(string lang, Form form, Type formType)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            if (form != null)
            {
                System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(formType);
                resources.ApplyResources(form, "$this");
                AppLang(form, resources);
            }
        }

        private static void AppLang(Control control, System.ComponentModel.ComponentResourceManager resources)
        {
            foreach (Control c in control.Controls)
            {
                resources.ApplyResources(c, c.Name);
                AppLang(c, resources);
            }
        }

        private void languageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (languageComboBox.SelectedIndex)
            {
                case 0:
                    SetLang("zh-CN", this, typeof(MainForm));
                    break;
                case 1:
                    SetLang("zh-TW", this, typeof(MainForm));
                    break;
                case 2:
                    SetLang("ja-JP", this, typeof(MainForm));
                    break;
                case 3:
                    SetLang("en-US", this, typeof(MainForm));
                    break;
                default:
                    SetLang("zh-CN", this, typeof(MainForm));
                    break;
            }
        }

        #endregion



    }
}
