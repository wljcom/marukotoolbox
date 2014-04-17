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
using System.Drawing.Imaging;
using System.Net;
namespace mp4box
{
    public partial class MainForm : Form
    {
        StringBuilder avsBuilder = new StringBuilder(1000);
        string syspath = Environment.GetFolderPath(Environment.SpecialFolder.System).Remove(1);
        int indexofsource;
        int indexoftarget;
        byte mode = 1;
        string clip = "";
        string MIvideo = "";
        string namevideo = "";
        string namevideo2 = "";
        //string namevideo3 = "";
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
        //string nameout4;
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
        string auto;
        string startpath;
        string workpath = "!undefined";
        string avs = "";
        string tempavspath = "";
        string tempPic = "";
        DateTime ReleaseDate = DateTime.Parse("2014-4-14");

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
                string duration = MI.Get(StreamKind.General, 0, "Duration/String1");
                string fileSize = MI.Get(StreamKind.General, 0, "FileSize/String");
                //视频
                string vid = MI.Get(StreamKind.Video, 0, "ID");
                string video = MI.Get(StreamKind.Video, 0, "Format");
                string vBitRate = MI.Get(StreamKind.Video, 0, "BitRate/String");
                string vSize = MI.Get(StreamKind.Video, 0, "StreamSize/String");
                string width = MI.Get(StreamKind.Video, 0, "Width");
                string height = MI.Get(StreamKind.Video, 0, "Height");
                string risplayAspectRatio = MI.Get(StreamKind.Video, 0, "DisplayAspectRatio/String");
                string risplayAspectRatio2 = MI.Get(StreamKind.Video, 0, "DisplayAspectRatio");
                string frameRate = MI.Get(StreamKind.Video, 0, "FrameRate/String");
                string bitDepth = MI.Get(StreamKind.Video, 0, "BitDepth/String");
                string pixelAspectRatio = MI.Get(StreamKind.Video, 0, "PixelAspectRatio");
                string encodedLibrary = MI.Get(StreamKind.Video, 0, "Encoded_Library");
                string encodeTime = MI.Get(StreamKind.Video, 0, "Encoded_Date");
                string codecProfile = MI.Get(StreamKind.Video, 0, "Codec_Profile");
                string frameCount = MI.Get(StreamKind.Video, 0, "FrameCount");

                //音频
                string aid = MI.Get(StreamKind.Audio, 0, "ID");
                string audio = MI.Get(StreamKind.Audio, 0, "Format");
                string aBitRate = MI.Get(StreamKind.Audio, 0, "BitRate/String");
                string samplingRate = MI.Get(StreamKind.Audio, 0, "SamplingRate/String");
                string channel = MI.Get(StreamKind.Audio, 0, "Channel(s)");
                string aSize = MI.Get(StreamKind.Audio, 0, "StreamSize/String");
                info = Path.GetFileName(VideoName) + "\r\n" +
                    "容器：" + container + "\r\n" +
                    "总码率：" + bitrate + "\r\n" +
                    "大小：" + fileSize + "\r\n" +
                    "时长：" + duration + "\r\n" +
                    "\r\n" +
                    "视频(" + vid + ")：" + video + "\r\n" +
                    "码率：" + vBitRate + "\r\n" +
                    "大小：" + vSize + "\r\n" +
                    "分辨率：" + width + "x" + height + "\r\n" +
                    "宽高比：" + risplayAspectRatio + "(" + risplayAspectRatio2 + ")" + "\r\n" +
                    "帧率：" + frameRate + "\r\n" +
                    "位深度：" + bitDepth + "\r\n" +
                    "像素宽高比：" + pixelAspectRatio + "\r\n" +
                    "编码库：" + encodedLibrary + "\r\n" +
                    "Profile：" + codecProfile + "\r\n" +
                    "编码时间：" + encodeTime + "\r\n" +
                    "总帧数：" + frameCount + "\r\n" +

                    "\r\n" +
                    "音频(" + aid + ")：" + audio + "\r\n" +
                    "大小：" + aSize + "\r\n" +
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
                    if (x264HeightNum.Value == 0 || x264WidthNum.Value == 0)
                    {
                        x264 = "\"" + workpath + "\\" + x264ExeComboBox.SelectedItem.ToString() + "\"  --crf " + x264CRFNum.Value + " --preset 8 --demuxer " + x264DemuxerComboBox.Text + " -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8  -o \"" + output + "\" \"" + input + "\"\r\n";
                    }
                    else
                    {
                        x264 = "\"" + workpath + "\\" + x264ExeComboBox.SelectedItem.ToString() + "\"  --crf " + x264CRFNum.Value + " --preset 8 --demuxer " + x264DemuxerComboBox.Text + " -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 --vf resize:" + x264WidthNum.Value + "," + x264HeightNum.Value + ",,,,lanczos -o \"" + output + "\" \"" + input + "\"\r\n";
                    }
                    break;
                case 2:
                    if (x264HeightNum.Value == 0 || x264WidthNum.Value == 0)
                    {
                        x264 = "\"" + workpath + "\\" + x264ExeComboBox.SelectedItem.ToString() + "\"  -p1 --stats \"tmp.stat\" --preset 8 --demuxer " + x264DemuxerComboBox.Text + " -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 -o NUL \"" + input + "\" && \"" + workpath + "\\" + x264ExeComboBox.SelectedItem.ToString() + "\"  -p2 --stats \"tmp.stat\" -B " + x264BitrateNum.Value + " --preset 8 --demuxer " + x264DemuxerComboBox.Text + " -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 -o \"" + output + "\" \"" + input + "\"\r\n";
                    }
                    else
                    {
                        x264 = "\"" + workpath + "\\" + x264ExeComboBox.SelectedItem.ToString() + "\"  -p1 --stats \"tmp.stat\" --preset 8 --demuxer " + x264DemuxerComboBox.Text + " -r 3 -b 3 --me umh  -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8  --vf resize:" + x264WidthNum.Value + "," + x264HeightNum.Value + ",,,,lanczos -o NUL \"" + input + "\" && \"" + workpath + "\\" + x264ExeComboBox.SelectedItem.ToString() + "\"  -p2 --stats \"tmp.stat\" -B " + x264BitrateNum.Value + " --preset 8 --demuxer " + x264DemuxerComboBox.Text + " -r 3 -b 3 --me umh -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8 --vf resize:" + x264WidthNum.Value + "," + x264HeightNum.Value + ",,,,lanczos -o \"" + output + "\" \"" + input + "\"\r\n";
                    }
                    break;
                case 0:
                    x264 = "\"" + workpath + "\\" + x264ExeComboBox.SelectedItem.ToString() + "\"  " + x264CustomParameterTextBox.Text + " --demuxer " + x264DemuxerComboBox.Text + " -o \"" + output + "\" \"" + input + "\"\r\n";
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
            int AACbr = 1024 * Convert.ToInt32(AudioBitrateComboBox.Text);
            string br = AACbr.ToString();
            ffmpeg = "\"" + workpath + "\\ffmpeg.exe\" -y -i \"" + input + "\" -f wav temp.wav";
            switch (AudioEncoderComboBox.SelectedIndex)
            {
                case 0:
                    if (AudioBitrateRadioButton.Checked)
                    {
                        neroaac = "\"" + workpath + "\\neroAacEnc.exe\" -ignorelength -lc -br " + br + " -if \"temp.wav\"  -of \"" + output + "\"";
                    }
                    if (AudioCustomizeRadioButton.Checked)
                    {
                        neroaac = "\"" + workpath + "\\neroAacEnc.exe\" " + AudioCustomParameterTextBox.Text.ToString() + " -if \"temp.wav\"  -of \"" + output + "\"";
                    }
                    break;
                case 1:
                    if (AudioBitrateRadioButton.Checked)
                    {
                        neroaac = "\"" + workpath + "\\qaac.exe\" -q 2 -c  " + AudioBitrateComboBox.Text + " \"temp.wav\"  -o \"" + output + "\"";
                    }
                    if (AudioCustomizeRadioButton.Checked)
                    {
                        neroaac = "\"" + workpath + "\\qaac.exe\" " + AudioCustomParameterTextBox.Text.ToString() + " \"temp.wav\"  -o \"" + output + "\"";
                    }
                    break;
                case 2:
                    if (Path.GetExtension(output) == ".aac")
                        output = AddExt(output, ".wav");
                    ffmpeg = "\"" + workpath + "\\ffmpeg.exe\" -y -i \"" + input + "\" -f wav \"" + output + "\"";
                    neroaac = "";
                    ;
                    break;
                default:
                    break;
            }
            aac = ffmpeg + "\r\n" + neroaac + "\r\n";
            if (cbwavtemp.Checked == false)
            {
                aac += "del temp.wav\r\n";
            }
            return aac;
        }
        public void oneAuto(string video, string output)
        {
            x264 = x264bat(video, "temp.mp4");
            x264 = x264.Replace("\r\n", "");
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
            x264 += " --acodec none\r\n";
            //audio
            //如果是avs
            if (video.Substring(video.LastIndexOf(".") + 1) == "avs")
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
                aextract = audiobat(video, "temp.aac");
            }
            //mux
            mux = muxbat("temp.mp4", "temp.aac", cbFPS.Text, output);
            //if (cbDelTmp.Checked == false)
            //{
            //    auto = aextract + x264 + mux + " \r\ndel temp.aac\r\ndel temp.mp4\r\ndel temp.wav\r\n";
            //}
            //else
            {
                auto = aextract + x264 + mux + " \r\n\r\n";
            }
            if (x264FLVCheckBox.Checked == true)
            {
                string flvfile = AddExt(output, "_FLV.flv");
                auto += "\r\n\"" + workpath + "\\ffmpeg.exe\"  -i  \"" + output + "\" -c copy -f flv  \"" + flvfile + "\" \r\n";
            }
            //if (x264ShutdownCheckBox.Checked)
            //{
            //    auto += "\r\n" + syspath + ":\\Windows\\System32\\shutdown -f -s -t 60\r\n";
            //}
            //auto += "cmd";
        }
        public void batchAuto()
        {
            int i;
            auto = "";
            for (i = 0; i < this.lbAuto.Items.Count; i++)
            {
                x264 = x264bat(lbAuto.Items[i].ToString(), "temp.mp4");
                x264 = x264.Replace("\r\n", "");
                //判断是否内嵌字幕
                string sub = "";
                string asssub = lbAuto.Items[i].ToString().Remove(lbAuto.Items[i].ToString().LastIndexOf(".")) + ".ass";
                string ssasub = lbAuto.Items[i].ToString().Remove(lbAuto.Items[i].ToString().LastIndexOf(".")) + ".ssa";
                string srtsub = lbAuto.Items[i].ToString().Remove(lbAuto.Items[i].ToString().LastIndexOf(".")) + ".srt";
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
                string finish;
                if (Directory.Exists(x264PathTextBox.Text))
                    finish = x264PathTextBox.Text + "\\" + Path.GetFileNameWithoutExtension(lbAuto.Items[i].ToString()) + "_onekeybatch.mp4";
                else
                    finish = AddExt(lbAuto.Items[i].ToString(), "_onekeybatch.mp4");
                mux = muxbat("temp.mp4", "temp.aac", finish);
                //mux = "\"" + workpath + "\\mp4box.exe\" -add temp.mp4 -add temp.aac -new \"" + finish + "\"";
                auto += aextract + x264 + mux + " \r\ndel temp.aac\r\ndel temp.mp4\r\ndel temp.wav\r\n";
                //auto += aextract + x264 + mux + " \r\ndel temp.aac\r\ndel temp.mp4\r\ndel temp.wav\r\n@echo off&&echo MsgBox \"Finish!\",64,\"Maruko Toolbox\" >> msg.vbs &&call msg.vbs &&del msg.vbs\r\ncmd";

            }
            //if (x264ShutdownCheckBox.Checked)
            //{
            //    auto += "\r\n" + syspath + ":\\Windows\\System32\\shutdown -f -s -t 60";
            //}
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
                    finish = x264PathTextBox.Text + "\\" + Path.GetFileNameWithoutExtension(lbAuto.Items[i].ToString()) + "_batch.mp4";
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
                switch (x264AudioModeComboBox.SelectedIndex)
                {
                    case 1: x264 += " --acodec none"; break;
                    case 2: x264 += " --acodec copy"; break;
                    case 3: x264 += " --audiofile \"" + x264AudioParameterTextBox.Text + "\""; break;
                    case 4: x264 += " --acodec qtaac " + x264AudioParameterTextBox.Text; break;
                    case 5: x264 += " --acodec faac " + x264AudioParameterTextBox.Text; break;
                    //case 6: x264 += " --acodec libaacplus " + x264AudioParameterTextBox.Text; break;
                    case 0: break;
                    default: ; break;
                }
                if (cbFPS2.Text != "auto")
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
                auto = auto + "\r\n" + x264;
            }

            //if (x264ShutdownCheckBox.Checked)
            //{
            //    auto += "\r\n" + syspath + ":\\Windows\\System32\\shutdown -f -s -t 60";
            //}
            //auto += "\r\n@echo off&&echo MsgBox \"Finish!\",64,\"Maruko Toolbox\" >> msg.vbs &&call msg.vbs &&del msg.vbs\r\ncmd";
            auto += "\r\n";
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
                if (cbFPS.Text == "auto")
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
        private void btnout4_Click(object sender, EventArgs e)
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
                //x264
                oneAuto(namevideo2, nameout2);
                batpath = workpath + "\\auto.bat";
                LogRecord(auto);
                WorkingForm wf = new WorkingForm(auto);
                wf.Owner = this;
                wf.Show();
                //File.WriteAllText(batpath, auto, UnicodeEncoding.Default);
                //System.Diagnostics.Process.Start(batpath);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            #region Delete Temp Files

            if (SetupDeleteTempFileCheckBox.Checked && !workpath.Equals("!undefined"))
            {
                List<string> deleteFileList = new List<string>();

                string systemDisk = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 3);
                string systemTempPath = systemDisk + @"windows\temp";

                //Delete all BAT files
                DirectoryInfo theFolder = new DirectoryInfo(workpath);
                foreach (FileInfo NextFile in theFolder.GetFiles())
                {
                    if (NextFile.Extension.Equals(".bat"))
                        deleteFileList.Add(NextFile.FullName);
                }

                //string[] deletedfiles = { tempPic, "msg.vbs", tempavspath, "temp.avs", "clip.bat", "aextract.bat", "vextract.bat",
                //                            "x264.bat", "aac.bat", "auto.bat", "mux.bat", "flv.bat", "mkvmerge.bat", "mkvextract.bat", "tmp.stat.mbtree", "tmp.stat" };
                string[] deletedfiles = { tempPic, tempavspath, workpath + "msg.vbs", workpath + "tmp.stat.mbtree", workpath + "tmp.stat" };
                deleteFileList.AddRange(deletedfiles);

                foreach (string file in deleteFileList)
                {
                    File.Delete(file);
                }
            }
            #endregion

            #region Save Settings
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["x264CRF"].Value = x264CRFNum.Value.ToString();
            cfa.AppSettings.Settings["x264Bitrate"].Value = x264BitrateNum.Value.ToString();
            cfa.AppSettings.Settings["x264AudioParameter"].Value = x264AudioParameterTextBox.Text;
            cfa.AppSettings.Settings["x264AudioMode"].Value = x264AudioModeComboBox.SelectedIndex.ToString();
            cfa.AppSettings.Settings["x264Exe"].Value = x264ExeComboBox.SelectedIndex.ToString();
            cfa.AppSettings.Settings["x264Demuxer"].Value = x264DemuxerComboBox.SelectedIndex.ToString();
            cfa.AppSettings.Settings["x264Width"].Value = x264WidthNum.Value.ToString();
            cfa.AppSettings.Settings["x264Height"].Value = x264HeightNum.Value.ToString();
            cfa.AppSettings.Settings["x264CustomParameter"].Value = x264CustomParameterTextBox.Text;
            cfa.AppSettings.Settings["x264Priority"].Value = x264PriorityComboBox.SelectedIndex.ToString();
            cfa.AppSettings.Settings["AVSScript"].Value = AVSScriptTextBox.Text;
            cfa.AppSettings.Settings["AudioEncoder"].Value = AudioEncoderComboBox.SelectedIndex.ToString();
            cfa.AppSettings.Settings["AudioCustomParameter"].Value = AudioCustomParameterTextBox.Text;
            cfa.AppSettings.Settings["AudioParameter"].Value = AudioBitrateComboBox.Text;
            cfa.AppSettings.Settings["OnePicAudioBitrate"].Value = OnePicAudioBitrateNum.Value.ToString();
            cfa.AppSettings.Settings["OnePicFPS"].Value = OnePicFPSNum.Value.ToString();
            cfa.AppSettings.Settings["OnePicCRF"].Value = OnePicCRFNum.Value.ToString();
            cfa.AppSettings.Settings["BlackFPS"].Value = BlackFPSNum.Value.ToString();
            cfa.AppSettings.Settings["BlackCRF"].Value = BlackCRFNum.Value.ToString();
            cfa.AppSettings.Settings["BlackBitrate"].Value = BlackBitrateNum.Value.ToString();
            cfa.AppSettings.Settings["SetupDeleteTempFile"].Value = SetupDeleteTempFileCheckBox.Checked.ToString();
            cfa.AppSettings.Settings["LanguageIndex"].Value = languageComboBox.SelectedIndex.ToString();

            cfa.Save();
            ConfigurationManager.RefreshSection("appSettings"); // 刷新命名节，在下次检索它时将从磁盘重新读取它。记住应用程序要刷新节点
            #endregion
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

        public static bool IsWindowsVistaOrNewer
        {
            get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major >= 6); }
        }


        /// <summary>
        /// 还原默认参数
        /// </summary>
        private void InitParameter()
        {
            x264CRFNum.Value = 24;
            x264BitrateNum.Value = 800;
            x264AudioParameterTextBox.Text = "--abitrate 128";
            x264AudioModeComboBox.SelectedIndex = 5;
            x264DemuxerComboBox.SelectedIndex = 2;
            x264WidthNum.Value = 0;
            x264HeightNum.Value = 0;
            x264CustomParameterTextBox.Text = "";
            x264PriorityComboBox.SelectedIndex = 2;
            AudioEncoderComboBox.SelectedIndex = 0;
            AudioCustomParameterTextBox.Text = "";
            AudioBitrateComboBox.Text = "128";
            OnePicAudioBitrateNum.Value = 128;
            OnePicFPSNum.Value = 1;
            OnePicCRFNum.Value = 24;
            AVSScriptTextBox.Text = "";
            BlackFPSNum.Value = 1;
            BlackCRFNum.Value = 51;
            BlackBitrateNum.Value = 900;
            SetupDeleteTempFileCheckBox.Checked = true;


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
            //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("zh-TW");
            //use YAHEI in VistaOrNewer
            //if (IsWindowsVistaOrNewer)
            //{
            //    FontFamily myFontFamily = new FontFamily("微软雅黑"); //采用哪种字体
            //    Font myFont = new Font(myFontFamily, 9, FontStyle.Regular); //字是那种字体，显示的风格
            //    this.Font = myFont;
            //}

            //define workpath
            startpath = System.Windows.Forms.Application.StartupPath;
            workpath = startpath + "\\tools";
            if (!Directory.Exists(workpath))
                Directory.CreateDirectory(workpath);
            //string diskSymbol = startpath.Substring(0, 1);

            string systemDisk = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 3);
            string systemTempPath = systemDisk + @"windows\temp";
            tempavspath = systemTempPath + "\\temp.avs";
            tempPic = systemTempPath + "\\marukotemp.jpg";
            InitParameter();

            DirectoryInfo folder = new DirectoryInfo(workpath);
            foreach (FileInfo FileName in folder.GetFiles())
            {
                if (FileName.Name.Contains("x264") && Path.GetExtension(FileName.Name) == ".exe")
                {
                    x264ExeComboBox.Items.Add(FileName.Name);
                }
            }

            ReleaseDatelabel.Text = ReleaseDate.ToString("yyyy-M-d");

            //load Help Text
            if (File.Exists(startpath + "\\help.txt"))
            {
                StreamReader sr = new StreamReader(startpath + "\\help.txt", System.Text.Encoding.UTF8);
                HelpTextBox.Text = sr.ReadToEnd();
                sr.Close();
            }
            try
            {
                //load settings
                x264CRFNum.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["x264CRF"]);
                x264BitrateNum.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["x264Bitrate"]);
                x264AudioParameterTextBox.Text = ConfigurationManager.AppSettings["x264AudioParameter"];
                x264AudioModeComboBox.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["x264AudioMode"]);
                x264ExeComboBox.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["x264Exe"]);
                x264DemuxerComboBox.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["x264Demuxer"]);
                x264WidthNum.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["x264Width"]);
                x264HeightNum.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["x264Height"]);
                x264CustomParameterTextBox.Text = ConfigurationManager.AppSettings["x264CustomParameter"];
                x264PriorityComboBox.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["x264Priority"]);
                AVSScriptTextBox.Text = ConfigurationManager.AppSettings["AVSScript"];
                AudioEncoderComboBox.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["AudioEncoder"]);
                AudioCustomParameterTextBox.Text = ConfigurationManager.AppSettings["AudioCustomParameter"];
                AudioBitrateComboBox.Text = ConfigurationManager.AppSettings["AudioBitrate"];
                OnePicAudioBitrateNum.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["OnePicAudioBitrate"]);
                OnePicFPSNum.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["OnePicFPS"]);
                OnePicCRFNum.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["OnePicCRF"]);
                BlackFPSNum.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["BlackFPS"]);
                BlackCRFNum.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["BlackCRF"]);
                BlackBitrateNum.Value = Convert.ToDecimal(ConfigurationManager.AppSettings["BlackBitrate"]);
                SetupDeleteTempFileCheckBox.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["SetupDeleteTempFile"]);

                if (x264ExeComboBox.SelectedIndex == -1)
                {
                    x264ExeComboBox.SelectedIndex = x264ExeComboBox.Items.IndexOf("x264_32_tMod-8bit-420.exe");
                }

                if (int.Parse(ConfigurationManager.AppSettings["LanguageIndex"]) == -1)  //First Startup
                {
                    string culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                    switch (culture)
                    {
                        case "zh-CN":
                            languageComboBox.SelectedIndex = 0;
                            break;
                        case "zh-SG":
                            languageComboBox.SelectedIndex = 0;
                            break;
                        case "zh-TW":
                            languageComboBox.SelectedIndex = 1;
                            break;
                        case "zh-HK ":
                            languageComboBox.SelectedIndex = 1;
                            break;
                        case "zh-MO":
                            languageComboBox.SelectedIndex = 1;
                            break;
                        case "en-US":
                            languageComboBox.SelectedIndex = 2;
                            break;
                        case "ja-JP":
                            languageComboBox.SelectedIndex = 3;
                            break;
                        default:
                            break;
                    }
                }
                else
                    languageComboBox.SelectedIndex = int.Parse(ConfigurationManager.AppSettings["LanguageIndex"]);
            }
            catch (Exception)
            {
                throw;
            }
            //create directory
            string preset = workpath + "\\preset";
            if (!Directory.Exists(preset))
                Directory.CreateDirectory(preset);
            DirectoryInfo TheFolder = new DirectoryInfo(preset);
            foreach (FileInfo FileName in TheFolder.GetFiles())
            {
                cbX264.Items.Add(FileName.Name.Replace(".txt", ""));
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
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

        private void button2_Click_1(object sender, EventArgs e)
        {
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

        private void btnAutoAdd_Click(object sender, EventArgs e)
        {
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
                LogRecord(auto);
                WorkingForm wf = new WorkingForm(auto, lbAuto.Items.Count);
                wf.Owner = this;
                wf.Show();

                //File.WriteAllText(batpath, auto, UnicodeEncoding.Default);
                //System.Diagnostics.Process.Start(batpath);
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
                ffmpeg += "\r\ncmd";
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
            if (AVSScriptTextBox.Text != "")
            {
                string filepath = workpath + "\\temp.avs";
                File.WriteAllText(filepath, AVSScriptTextBox.Text.ToString(), UnicodeEncoding.Default);
                PreviewForm form2 = new PreviewForm();
                form2.Show();
                form2.axWindowsMediaPlayer1.URL = filepath;
            }
            else
            {
                MessageBox.Show("请输入正确的AVS脚本！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                string filepath = tempavspath;
                //string filepath = workpath + "\\temp.avs";
                File.WriteAllText(filepath, AVSScriptTextBox.Text, UnicodeEncoding.Default);
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
                File.WriteAllText(filepath, AVSScriptTextBox.Text, UnicodeEncoding.Default);
                x264 = x264bat(filepath, "temp.mp4").Replace("\r\n", "");
                x264 += " --acodec none\r\n";
                //audio
                aextract = audiobat(namevideo9, "temp.aac");
                //mux
                mux = muxbat("temp.mp4", "temp.aac", "23.976", nameout9);
                //if (cbDelTmp.Checked == false)
                //{
                //    auto = aextract + x264 + "\r\n" + mux + " \r\ndel temp.aac\r\ndel temp.mp4\r\ndel temp.wav\r\ncmd ";
                //}
                //else
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
            AVSScriptTextBox.Text = avs;
            avs = "";
        }
        private void txth264_TextChanged(object sender, EventArgs e)
        {
        }
        private void txtvideo_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo.Text.ToString()))
            {
                System.Diagnostics.Process.Start(txtvideo.Text.ToString());
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
                if (processes[i].ProcessName.Equals(Path.GetFileNameWithoutExtension(x264ExeComboBox.Text)))
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
            AVSScriptTextBox.Clear();
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
            x264CustomParameterTextBox.Text = sr.ReadToEnd();
            sr.Close();
        }
        private void cbFPS_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void btnMIopen_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "视频(*.mp4;*.flv;*.mkv)|*.mp4;*.flv;*.mkv|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                MIvideo = openFileDialog1.FileName;
                MediaInfoTextBox.Text = MediaInfo(MIvideo);
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
                int i = namevideo6.IndexOf(".");
                string mkvname = namevideo6.Remove(i);
                QQButton btn = (QQButton)sender;
                switch (btn.Name)
                {
                    case "MkvExtract1Button":
                        mkvextract = "\"" + workpath + "\\mkvextract.exe\" tracks \"" + namevideo6 + "\" 1:\"" + mkvname + "_audio.aac\"";
                        break;
                    case "MkvExtract2Button":
                        mkvextract = "\"" + workpath + "\\mkvextract.exe\" tracks \"" + namevideo6 + "\" 2:\"" + mkvname + "_track2\"";
                        break;
                    case "MkvExtract3Button":
                        mkvextract = "\"" + workpath + "\\mkvextract.exe\" tracks \"" + namevideo6 + "\" 3:\"" + mkvname + "_track3\"";
                        break;
                    case "MkvExtract4Button":
                        mkvextract = "\"" + workpath + "\\mkvextract.exe\" tracks \"" + namevideo6 + "\" 4:\"" + mkvname + "_track4\"";
                        break;
                    case "btnextract7":
                        mkvextract = "\"" + workpath + "\\mkvextract.exe\" tracks \"" + namevideo6 + "\" 0:\"" + mkvname + "_video.h264\"";
                        break;
                }
                batpath = workpath + "\\mkvextract.bat";
                File.WriteAllText(batpath, mkvextract, UnicodeEncoding.Default);
                LogRecord(mkvextract);
                System.Diagnostics.Process.Start(batpath);
            }
        }
        private void txtMI_TextChanged(object sender, EventArgs e)
        {
            MItext = MediaInfoTextBox.Text;
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
            DateTime CompileDate = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location); //获得程序编译时间
            QQMessageBox.Show(
                this,
                "小丸工具箱 2014版\r\n主页：http://maruko.appinn.me/ \r\n编译日期：" + CompileDate.ToString(),
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
        private void x264OneBatchButton_Click(object sender, EventArgs e)
        {
            if (lbAuto.Items.Count != 0)
            {
                batchAuto();
                batpath = workpath + "\\auto.bat";
                LogRecord(auto);
                WorkingForm wf = new WorkingForm(auto);
                wf.Owner = this;
                wf.Show();

                //File.WriteAllText(batpath, auto, UnicodeEncoding.Default);
                //System.Diagnostics.Process.Start(batpath);
            }
            else MessageBox.Show("请输入视频！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        private void x264VideoBtn_Click(object sender, EventArgs e)
        {
            //
            openFileDialog1.Filter = "视频(*.mp4;*.flv;*.mkv;*.avi;*.wmv;*.mpg;*.avs)|*.mp4;*.flv;*.mkv;*.avi;*.wmv;*.mpg;*.avs|所有文件(*.*)|*.*";
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
                //如果是AVS复制到C盘根目录
                if (Path.GetExtension(x264VideoTextBox.Text) == ".avs")
                {
                    //if (File.Exists(tempavspath)) File.Delete(tempavspath);
                    File.Copy(x264VideoTextBox.Text, tempavspath, true);
                    namevideo2 = tempavspath;
                }
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
                switch (x264AudioModeComboBox.SelectedIndex)
                {
                    case 1: x264 += " --acodec none"; break;
                    case 2: x264 += " --acodec copy"; break;
                    case 3: x264 += " --audiofile \"" + x264AudioParameterTextBox.Text + "\""; break;
                    case 4: x264 += " --acodec qtaac " + x264AudioParameterTextBox.Text; break;
                    case 5: x264 += " --acodec faac " + x264AudioParameterTextBox.Text; break;
                    //case 6: x264 += " --acodec libaacplus " + x264AudioParameterTextBox.Text; break;
                    case 0: break;
                    default: ; break;
                }
                if (cbFPS2.Text != "auto")
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
                if (x264SeekNumericUpDown.Value != 0)
                {
                    x264 += " --seek " + x264SeekNumericUpDown.Value.ToString();
                }
                if (x264FramesNumericUpDown.Value != 0)
                {
                    x264 += " --frames " + x264FramesNumericUpDown.Value.ToString();
                }
                x264 += "\r\n";
                if (x264FLVCheckBox.Checked == true)
                {
                    string flvfile = AddExt(nameout2, "_FLV.flv");
                    x264 += "\r\n\"" + workpath + "\\ffmpeg.exe\" -i  \"" + nameout2 + "\" -c copy -f flv  \"" + flvfile + "\" \r\n";
                }

                //if (x264ShutdownCheckBox.Checked)
                //{
                //    x264 += "\r\n" + syspath + ":\\Windows\\System32\\shutdown -f -s -t 60";
                //}

                LogRecord(x264);
                WorkingForm wf = new WorkingForm(x264);
                wf.Owner = this;
                wf.Show();

                //x264 += "\r\ncmd";
                //batpath = workpath + "\\x264.bat";
                //File.WriteAllText(batpath, x264, UnicodeEncoding.Default);
                //System.Diagnostics.Process.Start(batpath);
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
            File.WriteAllText(batpath, x264CustomParameterTextBox.Text, UnicodeEncoding.Default);
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
            x264BitrateNum.Visible = true;
            label12.Visible = true;
            cbFPS2.Visible = true;
            lbFPS2.Visible = true;
            lbwidth.Visible = true;
            lbheight.Visible = true;
            x264WidthNum.Visible = true;
            x264HeightNum.Visible = true;
            MaintainResolutionCheckBox.Visible = true;
            lbcrf.Visible = false;
            x264CRFNum.Visible = false;
            label4.Visible = false;
            x264CustomParameterTextBox.Visible = false;
            cbX264.Visible = false;
            x264AddPresetBtn.Visible = false;
            x264DeletePresetBtn.Visible = false;
            PresetNameTextBox.Visible = false;
        }
        private void x264Mode3RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            mode = 0;
            label4.Visible = true;
            x264CustomParameterTextBox.Visible = true;
            cbX264.Visible = true;
            x264AddPresetBtn.Visible = true;
            x264DeletePresetBtn.Visible = true;
            PresetNameTextBox.Visible = true;
            lbwidth.Visible = false;
            lbheight.Visible = false;
            x264WidthNum.Visible = false;
            x264HeightNum.Visible = false;
            MaintainResolutionCheckBox.Visible = false;
            lbrate.Visible = false;
            x264BitrateNum.Visible = false;
            label12.Visible = false;
            lbcrf.Visible = false;
            x264CRFNum.Visible = false;
            cbFPS2.Visible = false;
            lbFPS2.Visible = false;
        }
        private void x264Mode1RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            mode = 1;
            lbcrf.Visible = true;
            x264CRFNum.Visible = true;
            cbFPS2.Visible = true;
            lbFPS2.Visible = true;
            lbwidth.Visible = true;
            lbheight.Visible = true;
            x264WidthNum.Visible = true;
            x264HeightNum.Visible = true;
            MaintainResolutionCheckBox.Visible = true;
            lbrate.Visible = false;
            x264BitrateNum.Visible = false;
            label12.Visible = false;
            label4.Visible = false;
            x264CustomParameterTextBox.Visible = false;
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
                    case 1:
                        x264AudioParameterTextBox.Text = "";
                        break;
                    case 2:
                        x264AudioParameterTextBox.Text = "";
                        x264AudioParameterTextBox.EmptyTextTip = "可能失败，如出错请用FAAC";
                        break;
                    case 3:
                        x264AudioParameterTextBox.Text = "";
                        x264AudioParameterTextBox.EmptyTextTip = "把音频文件拖到这里";
                        break;
                    case 4:
                        x264AudioParameterTextBox.Text = "--abitrate 128";
                        break;
                    case 5:
                        x264AudioParameterTextBox.Text = "--abitrate 128";
                        break;
                    case 6:
                        x264AudioParameterTextBox.Text = "";
                        break;
                    case 0:
                        x264AudioParameterTextBox.Text = "";
                        break;
                    default:
                        break;
                }
            }
        }
        private void x264PriorityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string processName = x264ExeComboBox.Text;
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
        #region 音频界面
        private void AudioEncoderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtaudio2.Text))
            {
                if (AudioEncoderComboBox.SelectedIndex == 2)
                    txtout3.Text = AddExt(txtaudio2.Text, "_WAV.wav");
                else
                    txtout3.Text = AddExt(txtaudio2.Text, "_AAC.aac");
            }
        }
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
                    if (AudioEncoderComboBox.SelectedIndex == 2)
                        finish = AddExt(AudioListBox.Items[i].ToString(), "_WAV.wav");
                    else
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
                if (AudioEncoderComboBox.SelectedIndex == 2)
                    txtout3.Text = AddExt(txtaudio2.Text, "_WAV.wav");
                else
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
            lbaacrate.Visible = false;
            lbaackbps.Visible = false;
            AudioBitrateComboBox.Visible = false;
            AudioCustomParameterTextBox.Visible = true;
        }
        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            lbaacrate.Visible = true;
            lbaackbps.Visible = true;
            AudioBitrateComboBox.Visible = true;
            AudioCustomParameterTextBox.Visible = false;
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
            if (!string.IsNullOrEmpty(txtsub9.Text))
            {
                if (Path.GetExtension(namesub9) == ".idx")
                    avsBuilder.AppendLine("vobsub(\"" + namesub9 + "\")");
                else
                    avsBuilder.AppendLine("TextSub(\"" + namesub9 + "\")");
            }
            if (TrimCheckBox.Checked)
                avsBuilder.AppendLine("Trim(" + TrimStartNumericUpDown.Value.ToString() + "," + TrimEndNumericUpDown.Value.ToString() + ")");
            AVSScriptTextBox.Text = avsBuilder.ToString();
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

        #endregion
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
            Match m = Regex.Match(AVSScriptTextBox.Text, "ource\\(\"[a-zA-Z]:\\\\.+\\.\\w+\"");
            if (m.Success)
            {
                string str = m.ToString();
                str = str.Replace("ource(\"", "");
                str = str.Replace("\"", "");
                str = AddExt(str, "_AVS.mp4");
                txtout9.Text = str;
            }
        }
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
                x264WidthNum.Value = 0;
                x264HeightNum.Value = 0;
                x264WidthNum.Enabled = false;
                x264HeightNum.Enabled = false;
            }
            else
            {
                x264WidthNum.Enabled = true;
                x264HeightNum.Enabled = true;
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
            StreamReader sr;
            x264Mode1RadioButton.Checked = true;
            AudioBitrateRadioButton.Checked = true;
            switch (languageComboBox.SelectedIndex)
            {
                case 0:
                    SetLang("zh-CN", this, typeof(MainForm));
                    x264PriorityComboBox.Items.Clear();
                    x264PriorityComboBox.Items.AddRange(new string[] { "低", "低于标准", "普通", "高于标准", "高", "实时" });
                    x264PriorityComboBox.SelectedIndex = 2;
                    x264VideoTextBox.EmptyTextTip = "可以把文件拖曳到这里";
                    //x264OutTextBox.EmptyTextTip = "宽度和高度全为0即不改变分辨率";
                    x264PathTextBox.EmptyTextTip = "字幕文件和视频文件在同一目录下且同名";
                    //txtvideo3.EmptyTextTip = "音频参数在音频选项卡设定";
                    ExtractMP4TextBox.EmptyTextTip = "抽取的视频或音频在原视频目录下";
                    txtvideo8.EmptyTextTip = "抽取的视频或音频在原视频目录下";
                    txtvideo6.EmptyTextTip = "抽取的视频或音频在原视频目录下";
                    //load Help Text
                    if (File.Exists(startpath + "\\help.txt"))
                    {
                        sr = new StreamReader(startpath + "\\help.txt", System.Text.Encoding.UTF8);
                        HelpTextBox.Text = sr.ReadToEnd();
                        sr.Close();
                    }
                    break;
                case 1:
                    SetLang("zh-TW", this, typeof(MainForm));
                    x264PriorityComboBox.Items.Clear();
                    x264PriorityComboBox.Items.AddRange(new string[] { "低", "在標準以下", "標準", "在標準以上", "高", "即時" });
                    x264PriorityComboBox.SelectedIndex = 2;
                    x264VideoTextBox.EmptyTextTip = "可以把文件拖曳到這裡";
                    //x264OutTextBox.EmptyTextTip = "寬度和高度全為0即不改變解析度";
                    x264PathTextBox.EmptyTextTip = "字幕和視頻在同一資料夾下且同名";
                    //txtvideo3.EmptyTextTip = "音頻參數需在音頻選項卡设定";
                    ExtractMP4TextBox.EmptyTextTip = "新檔案生成在原資料夾";
                    txtvideo8.EmptyTextTip = "新檔案生成在原資料夾";
                    txtvideo6.EmptyTextTip = "新檔案生成在原資料夾";
                    //load Help Text
                    if (File.Exists(startpath + "\\help_zh_tw.txt"))
                    {
                        sr = new StreamReader(startpath + "\\help_zh_tw.txt", System.Text.Encoding.UTF8);
                        HelpTextBox.Text = sr.ReadToEnd();
                        sr.Close();
                    }
                    break;
                case 2:
                    SetLang("en-US", this, typeof(MainForm));
                    x264PriorityComboBox.Items.Clear();
                    x264PriorityComboBox.Items.AddRange(new string[] { "Idle", "BelowNormal", "Normal", "AboveNormal", "High", "RealTime" });
                    x264PriorityComboBox.SelectedIndex = 2;
                    x264VideoTextBox.EmptyTextTip = "Drag file here";
                    //x264OutTextBox.EmptyTextTip = "Both the width and height equal zero means using original resolution";
                    x264PathTextBox.EmptyTextTip = "Subtitle and Video must be of the same name and in the same folder";
                    //txtvideo3.EmptyTextTip = "It is necessary to set audio parameter in the Audio tab";
                    ExtractMP4TextBox.EmptyTextTip = "New file will be created in the original folder";
                    txtvideo8.EmptyTextTip = "New file will be created in the original folder";
                    txtvideo6.EmptyTextTip = "New file will be created in the original folder";
                    //load Help Text
                    if (File.Exists(startpath + "\\help.txt"))
                    {
                        sr = new StreamReader(startpath + "\\help.txt", System.Text.Encoding.UTF8);
                        HelpTextBox.Text = sr.ReadToEnd();
                        sr.Close();
                    }
                    break;
                case 3:
                    SetLang("ja-JP", this, typeof(MainForm));
                    x264PriorityComboBox.Items.Clear();
                    x264PriorityComboBox.Items.AddRange(new string[] { "低", "通常以下", "通常", "通常以上", "高", "リアルタイム" });
                    x264PriorityComboBox.SelectedIndex = 2;
                    x264VideoTextBox.EmptyTextTip = "ビデオファイルをここに引きずってください";
                    //x264OutTextBox.EmptyTextTip = "Both the width and height equal zero means using original resolution";
                    x264PathTextBox.EmptyTextTip = "字幕とビデオは同じ名前と同じフォルダにある必要があります";
                    //txtvideo3.EmptyTextTip = "It is necessary to set audio parameter in the Audio tab";
                    ExtractMP4TextBox.EmptyTextTip = "新しいファイルはビデオファイルのあるディレクトリに生成する";
                    txtvideo8.EmptyTextTip = "新しいファイルはビデオファイルのあるディレクトリに生成する";
                    txtvideo6.EmptyTextTip = "新しいファイルはビデオファイルのあるディレクトリに生成する";
                    break;
                default:
                    SetLang("zh-CN", this, typeof(MainForm));
                    break;
            }
        }
        #endregion
        public static void RunProcess(string exe, string arg)
        {
            Thread thread = new Thread(() =>
            {
                ProcessStartInfo psi = new ProcessStartInfo(exe, arg);
                psi.CreateNoWindow = true;
                Process p = new Process();
                p.StartInfo = psi;
                p.Start();
                p.WaitForExit();
                MessageBox.Show("ts");
                p.Close();
            });
            thread.IsBackground = true;
            thread.Start();
        }
        private void DonateButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://me.alipay.com/marukochan");
        }
        private void AVSSaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "AVS(*.avs)|*.avs";
            DialogResult result = savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                File.WriteAllText(savefile.FileName, AVSScriptTextBox.Text, UnicodeEncoding.Default);
            }
        }
        private void MuxReplaceAudioButton_Click(object sender, EventArgs e)
        {
            if (namevideo == "")
            {
                MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (nameaudio == "")
            {
                MessageBox.Show("请选择音频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (nameout == "")
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            mux = "";
            mux = "\"" + workpath + "\\ffmpeg.exe\" -y -i \"" + namevideo + "\" -vcodec copy -an  \"" + workpath + "\\video_noaudio.mp4\" \r\n";
            mux += "\"" + workpath + "\\ffmpeg.exe\" -y -i \"" + workpath + "\\video_noaudio.mp4\" -i \"" + nameaudio + "\" -vcodec copy  -acodec copy \"" + nameout + "\" \r\n";
            mux += "del \"" + workpath + "\\video_noaudio.mp4\" \r\n";
            batpath = workpath + "\\mux.bat";
            File.WriteAllText(batpath, mux, UnicodeEncoding.Default);
            LogRecord(mux);
            Process.Start(batpath);
        }
        private void cbx264file_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (x264ExeComboBox.Text.Contains("all"))
            {
                x264AudioModeComboBox.Items.Clear();
                x264AudioModeComboBox.Items.Add("默认");
                x264AudioModeComboBox.Items.Add("无音频流");
                x264AudioModeComboBox.Items.Add("复制音频流");
                x264AudioModeComboBox.Items.Add("外置音频流");
                x264AudioModeComboBox.SelectedIndex = 1;
            }
            else
            {
                x264AudioModeComboBox.Items.Clear();
                x264AudioModeComboBox.Items.Add("默认");
                x264AudioModeComboBox.Items.Add("无音频流");
                x264AudioModeComboBox.Items.Add("复制音频流");
                x264AudioModeComboBox.Items.Add("外置音频流");
                x264AudioModeComboBox.Items.Add("QTAAC");
                x264AudioModeComboBox.Items.Add("FAAC");
                x264AudioModeComboBox.SelectedIndex = 5;
            }
        }

        #region 一图流

        private void AudioPicButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "图片(*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                AudioPicTextBox.Text = openFileDialog1.FileName;
            }
        }
        private void AudioPicAudioButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "音频(*.aac;*.mp3;*.mp4;*.wav)|*.aac;*.mp3;*.mp4;*.wav|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                AudioPicAudioTextBox.Text = openFileDialog1.FileName;
            }
        }
        private void AudioOnePicOutputButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "MP4视频(*.mp4)|*.mp4|FLV视频(*.flv)|*.flv";
            savefile.FileName = "Single";
            DialogResult result = savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                AudioOnePicOutputTextBox.Text = savefile.FileName;
            }
        }
        public int SecondsFromHHMMSS(string hhmmss)
        {
            int hh = int.Parse(hhmmss.Substring(0, 2));
            int mm = int.Parse(hhmmss.Substring(3, 2));
            int ss = int.Parse(hhmmss.Substring(6, 2));
            return hh * 3600 + mm * 60 + ss;
        }
        private void AudioOnePicButton_Click(object sender, EventArgs e)
        {
            if (!File.Exists(AudioPicTextBox.Text))
            {
                MessageBox.Show("请选择图片文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (!File.Exists(AudioPicAudioTextBox.Text))
            {
                MessageBox.Show("请选择音频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (AudioOnePicOutputTextBox.Text == "")
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(AudioPicTextBox.Text);
                int sourceWidth = img.Width;
                int sourceHeight = img.Height;
                if (img.Width % 2 != 0 || img.Height % 2 != 0)
                {
                    MessageBox.Show("图片的长和宽必须是偶数。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    img.Dispose();
                    return;
                }
                if (img.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    File.Copy(AudioPicTextBox.Text, tempPic, true);
                }
                else
                {
                    System.Drawing.Imaging.Encoder ImageEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameter ep = new EncoderParameter(ImageEncoder, 100L);
                    EncoderParameters eps = new EncoderParameters(1);
                    ImageCodecInfo ImageCoderType = getImageCoderInfo("image/jpeg");
                    eps.Param[0] = ep;
                    img.Save(tempPic, ImageCoderType, eps);
                    //img.Save(tempPic, ImageFormat.Jpeg);
                }
                //获得音频时长
                MediaInfo MI = new MediaInfo();
                MI.Open(AudioPicAudioTextBox.Text);
                int seconds = SecondsFromHHMMSS(MI.Get(StreamKind.General, 0, "Duration/String3"));
                if (AudioCopyCheckBox.Checked)
                {
                    mux = "ffmpeg -loop 1 -r " + OnePicFPSNum.Value.ToString() + " -t " + seconds.ToString() + " -f image2 -i \"" + tempPic + "\" -vcodec libx264 -crf " + OnePicCRFNum.Value.ToString() + " -y SinglePictureVideo.mp4\r\n";
                    mux += "ffmpeg -i SinglePictureVideo.mp4 -i \"" + AudioPicAudioTextBox.Text + "\" -c:v copy -c:a copy -y \"" + AudioOnePicOutputTextBox.Text + "\"\r\n";
                    mux += "del SinglePictureVideo.mp4\r\n";
                }
                else
                {
                    mux = "ffmpeg -i \"" + AudioPicAudioTextBox.Text + "\" -f wav - |neroaacenc -br " + OnePicAudioBitrateNum.Value.ToString() + "000 -ignorelength -if - -of audio.mp4 -lc\r\n";
                    mux += "ffmpeg -loop 1 -crf " + OnePicCRFNum.Value.ToString() + " -r " + OnePicFPSNum.Value.ToString() + " -t " + seconds.ToString() + " -f image2 -i \"" + tempPic + "\" -vcodec libx264 -crf " + OnePicCRFNum.Value.ToString() + " -y SinglePictureVideo.mp4\r\n";
                    mux += "ffmpeg -i SinglePictureVideo.mp4 -i audio.mp4 -c:v copy -c:a copy -y \"" + AudioOnePicOutputTextBox.Text + "\"\r\n";
                    mux += "del SinglePictureVideo.mp4\r\ndel audio.mp4\r\n";
                }
                /*
                string audioPath = AddExt(Path.GetFileName(AudioPicAudioTextBox.Text), "_atmp.mp4");
                string videoPath = AddExt(Path.GetFileName(AudioPicAudioTextBox.Text), "_vtmp.mp4");
                string picturePath = "c:\\" + Path.GetFileNameWithoutExtension(AudioPicTextBox.Text) + "_tmp.jpg";
                if (AudioCopyCheckBox.Checked)
                {
                    mux = "ffmpeg -loop 1 -r " + AudioOnePicFPSNum.Value.ToString() + " -t " + seconds.ToString() + " -f image2 -i \"" + picturePath + "\" -vcodec libx264 -crf 24 -y \"" + videoPath + "\"\r\n";
                    mux += "ffmpeg -i \"" + videoPath + "\" -i \"" + AudioPicAudioTextBox.Text + "\" -c:v copy -c:a copy -y \"" + AudioOnePicOutputTextBox.Text + "\"\r\n";
                    mux += "del \"" + videoPath + "\"\r\ndel \"" + picturePath + "\"\r\n";
                }
                else
                {
                    mux = "ffmpeg -i \"" + AudioPicAudioTextBox.Text + "\" -f wav - |neroaacenc -br " + AudioOnePicAudioBitrateNum.Value.ToString() + "000 -ignorelength -if - -of \"" + audioPath + "\" -lc\r\n";
                    mux += "ffmpeg -loop 1 -r " + AudioOnePicFPSNum.Value.ToString() + " -t " + seconds.ToString() + " -f image2 -i \"" + picturePath + "\" -vcodec libx264 -crf 24 -y \"" + videoPath + "\"\r\n";
                    mux += "ffmpeg -i \"" + videoPath + "\" -i \"" + audioPath + "\" -c:v copy -c:a copy -y \"" + AudioOnePicOutputTextBox.Text + "\"\r\n";
                    mux += "del \"" + videoPath + "\"\r\ndel \"" + audioPath + "\"\r\ndel \"" + picturePath + "\"\r\n";
                }
                */
                batpath = Path.Combine(workpath, Path.GetRandomFileName() + ".bat");
                File.WriteAllText(batpath, mux, UnicodeEncoding.Default);
                LogRecord(mux);
                Process.Start(batpath);
            }
        }
        private void txtMI_DragDrop(object sender, DragEventArgs e)
        {
            MIvideo = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            MediaInfoTextBox.Text = MediaInfo(MIvideo);
        }
        private void txtMI_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }
        private void AudioPicAudioTextBox_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(AudioPicAudioTextBox.Text.ToString()))
            {
                AudioOnePicOutputTextBox.Text = AddExt(AudioPicAudioTextBox.Text, "_SP.flv");
            }
        }
        /// <summary>
        /// 获取图片编码类型信息
        /// </summary>
        /// <param name="ImageCoderType">编码类型</param>
        /// <returns>ImageCodecInfo</returns>
        private ImageCodecInfo getImageCoderInfo(string ImageCoderType)
        {
            ImageCodecInfo[] coderTypeArray = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo coderType in coderTypeArray)
            {
                if (coderType.MimeType.Equals(ImageCoderType))
                    return coderType;
            }
            return null;
        }

        #endregion


        #region 后黑

        private void BlackVideoButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "FLV视频(*.flv)|*.flv";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                BlackVideoTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void BlackOutputButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "FLV视频(*.flv)|*.flv";
            DialogResult result = savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                BlackOutputTextBox.Text = savefile.FileName;
            }
        }

        private void BlackPicButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "图片(*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                BlackPicTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void BlackStartButton_Click(object sender, EventArgs e)
        {
            string videoname = BlackVideoTextBox.Text;
            MediaInfo MI = new MediaInfo();
            MI.Open(videoname);
            double videobitrate = double.Parse(MI.Get(StreamKind.General, 0, "BitRate"));
            double targetBitrate = (double)BlackBitrateNum.Value;

            //验证
            if (!File.Exists(BlackVideoTextBox.Text) || Path.GetExtension(BlackVideoTextBox.Text) != ".flv")
            {
                MessageBox.Show("请选择FLV视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!File.Exists(BlackPicTextBox.Text) && BlackNoPicCheckBox.Checked == false)
            {
                MessageBox.Show("请选择图片文件或勾选使用黑屏", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (BlackOutputTextBox.Text == "")
            {
                MessageBox.Show("请选择输出文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (videobitrate < 1000000)
            {
                MessageBox.Show("此视频不需要后黑。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (videobitrate > 2500000)
            {
                MessageBox.Show("此视频码率过大，请先压制再后黑。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            //处理图片
            int videoWidth = int.Parse(MI.Get(StreamKind.Video, 0, "Width"));
            int videoHeight = int.Parse(MI.Get(StreamKind.Video, 0, "Height"));
            if (BlackNoPicCheckBox.Checked)
            {
                Bitmap bm = new Bitmap(videoWidth, videoHeight);
                Graphics g = Graphics.FromImage(bm);
                //g.FillRectangle(Brushes.White, new Rectangle(0, 0, 800, 600));
                g.Clear(Color.Black);
                bm.Save(tempPic, ImageFormat.Jpeg);
                g.Dispose();
                bm.Dispose();
            }
            else
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(BlackPicTextBox.Text);
                int sourceWidth = img.Width;
                int sourceHeight = img.Height;
                if (img.Width % 2 != 0 || img.Height % 2 != 0)
                {
                    MessageBox.Show("图片的长和宽必须都是偶数。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    img.Dispose();
                    return;
                }
                if (img.Width != videoWidth || img.Height != videoHeight)
                {
                    MessageBox.Show("图片的长和宽和视频不一致。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    img.Dispose();
                    return;
                }
                if (img.RawFormat.Equals(ImageFormat.Jpeg))
                {
                    File.Copy(BlackPicTextBox.Text, tempPic, true);
                }
                else
                {
                    System.Drawing.Imaging.Encoder ImageEncoder = System.Drawing.Imaging.Encoder.Quality;
                    EncoderParameter ep = new EncoderParameter(ImageEncoder, 100L);
                    EncoderParameters eps = new EncoderParameters(1);
                    ImageCodecInfo ImageCoderType = getImageCoderInfo("image/jpeg");
                    eps.Param[0] = ep;
                    img.Save(tempPic, ImageCoderType, eps);
                    //img.Save(tempPic, ImageFormat.Jpeg);
                }
            }
            int blackSecond = 300;
            //计算后黑时长
            if (BlackSecondComboBox.Text == "auto")
            {
                int seconds = SecondsFromHHMMSS(MI.Get(StreamKind.General, 0, "Duration/String3"));
                double s = videobitrate / 1000.0 * (double)seconds / targetBitrate - (double)seconds;
                blackSecond = (int)s;
                BlackSecondComboBox.Text = blackSecond.ToString();
            }
            else
            {
                blackSecond = int.Parse(Regex.Replace(BlackSecondComboBox.Text.ToString(), @"\D", "")); //排除除数字外的所有字符
            }

            //批处理
            mux = "ffmpeg -loop 1 -r " + BlackFPSNum.Value.ToString() + " -t " + blackSecond.ToString() + " -f image2 -i \"" + tempPic + "\" -vcodec libx264 -crf " + BlackCRFNum.Value.ToString() + " -y black.flv\r\n";
            mux += string.Format("flvbind \"{0}\"  \"{1}\"  black.flv\r\n", BlackOutputTextBox.Text, BlackVideoTextBox.Text);
            mux += "del black.flv\r\n";

            batpath = Path.Combine(workpath, Path.GetRandomFileName() + ".bat");
            File.WriteAllText(batpath, mux, UnicodeEncoding.Default);
            LogRecord(mux);
            Process.Start(batpath);
        }

        private void BlackVideoTextBox_TextChanged(object sender, EventArgs e)
        {
            string path = BlackVideoTextBox.Text;
            if (File.Exists(path))
            {
                BlackOutputTextBox.Text = AddExt(path, "_black.flv");
            }
        }
        #endregion

        private void BlackNoPicCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            BlackPicTextBox.Enabled = !BlackNoPicCheckBox.Checked;
            BlackPicButton.Enabled = !BlackNoPicCheckBox.Checked;
        }

        private void BlackSecondComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BlackSecondComboBox.Text != "auto")
            {
                BlackBitrateNum.Enabled = false;
            }
            else
            {
                BlackBitrateNum.Enabled = true;
            }
        }

        private void SetDefaultButton_Click(object sender, EventArgs e)
        {
            InitParameter();
        }

        //Ctrl+A 可以全选文本
        private void MediaInfoTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }
        }

        private void AVSScriptTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }
        }

        private void x264CustomParameterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.A)
            {
                ((TextBox)sender).SelectAll();
            }
        }

        private void CheckUpdateButton_Click(object sender, EventArgs e)
        {
            WebRequest request = WebRequest.Create("http://hi.baidu.com/xiaowanmaruko/item/119203f757097c603c148502");
            request.Credentials = CredentialCache.DefaultCredentials;
            // Get the response.
            request.BeginGetResponse(new AsyncCallback(OnResponse), request);
        }

        protected void OnResponse(IAsyncResult ar)
        {
            WebRequest wrq = (WebRequest)ar.AsyncState;
            WebResponse wrs = wrq.EndGetResponse(ar);
            // read the response ...
            Stream dataStream = wrs.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            Regex reg = new Regex(@"Maruko20\S+Maruko");
            Match m = reg.Match(responseFromServer);
            if (m.Success)
            {
                string a = m.Value.Replace("Maruko", "");
                DateTime NewDate = DateTime.Parse(a);
                //DateTime CompileDate = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location); //获得程序编译时间
                int s = DateTime.Compare(NewDate, ReleaseDate);
                if (s == 1) //NewDate is later than ReleaseDate
                {
                    DialogResult dr = MessageBox.Show(string.Format("新鲜小丸已于{0}上架，主人不来尝一口咩？", NewDate.ToString("yyyy-M-d")), "Info", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (dr == DialogResult.Yes)
                    {
                        Process.Start("http://maruko.appinn.me");
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("喵~伦家已经是最新版啦！", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}