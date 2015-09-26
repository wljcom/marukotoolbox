// ------------------------------------------------------------------
// Copyright (C) 2011-2015 Maruko Toolbox Project
//
//  Authors: komaruchan <sandy_0308@hotmail.com>
//           LunarShaddow <aflyhorse@hotmail.com>
//           LYF <lyfjxymf@sina.com>
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

using ControlExs;
using MediaInfoLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;

namespace mp4box
{
    public partial class MainForm : Form
    {
        public string workPath = "!undefined";
        public bool shutdownState = false;
        public bool trayMode = false;
        private XDocument xdoc;

        #region Private Members Declaration

        private StringBuilder avsBuilder = new StringBuilder(1000);
        private string syspath = Environment.GetFolderPath(Environment.SpecialFolder.System).Remove(1);
        private int indexofsource;
        private int indexoftarget;
        private byte x264mode = 1;
        private string clip = "";
        private string MIvideo = "";
        private string namevideo = "";
        private string namevideo2 = "";
        private string namevideo4 = "";
        private string namevideo5 = "";
        private string namevideo6 = "";
        private string nameaudio = "";
        private string nameaudio2 = "";
        private string nameaudio3 = "";
        private string namevideo8 = "";
        private string namevideo9 = "video";
        private string nameout;
        private string nameout2;
        private string nameout3;
        private string nameout5;
        private string nameout6;
        private string nameout9;
        private string namesub;
        private string namesub2 = "";
        private string namesub9 = "subtitle";
        private string MItext = "把视频文件拖到这里";
        private string mkvextract;
        private string mkvmerge;
        private string mux;
        private string x264;
        private string ffmpeg;
        private string aac;
        private string aextract;
        private string batpath;
        private string auto;
        private string startpath;
        private string avs = "";
        private string tempavspath = "";
        private string tempPic = "";
        private string logFileName, logPath;
        private DateTime ReleaseDate = DateTime.Parse("2015-9-10 8:0:0");

        #endregion Private Members Declaration

        #region CPU Porocessors Number

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public uint dwOemId;
            public uint dwPageSize;
            public uint lpMinimumApplicationAddress;
            public uint lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }

        [DllImport("kernel32")]
        private static extern void GetSystemInfo(ref SYSTEM_INFO pSI);

        #endregion CPU Porocessors Number

        public MainForm()
        {
            logPath = Application.StartupPath + "\\logs";
            logFileName = logPath + "\\LogFile-" + DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH'-'mm'-'ss") + ".log";
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

                string audioInfo = MI.Get(StreamKind.Audio, 0, "Inform") + MI.Get(StreamKind.Audio, 1, "Inform") + MI.Get(StreamKind.Audio, 2, "Inform") + MI.Get(StreamKind.Audio, 3, "Inform");
                string videoInfo = MI.Get(StreamKind.Video, 0, "Inform");

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
                    "声道数：" + channel + "\r\n" +
                    "\r\n====详细信息====\r\n" +
                    videoInfo + "\r\n" +
                    audioInfo + "\r\n"
                    ;
                MI.Close();
            }
            return info;
        }

        public string ffmuxbat(string input1, string input2, string output)
        {
            return "\"" + workPath + "\\ffmpeg.exe\" -i \"" + input1 + "\" -i \"" + input2 + "\" -sn -c copy -y \"" + output + "\"\r\n";
        }

        public string boxmuxbat(string input1, string input2, string output)
        {
            return "\"" + workPath + "\\mp4box.exe\" -add \"" + input1 + "#trackID=1:name=\" -add \"" + input2 + "#trackID=1:name=\" -new \"" + output + "\"\r\n";
        }

        public string x264bat(string input, string output, int pass = 1, string sub = "")
        {
            StringBuilder sb = new StringBuilder();
            //keyint设为fps的10倍
            MediaInfo MI = new MediaInfo();
            MI.Open(input);
            string frameRate = MI.Get(StreamKind.Video, 0, "FrameRate");
            double fps;
            string keyint = "-1";
            if (double.TryParse(frameRate, out fps))
            {
                fps = Math.Round(fps);
                keyint = (fps * 10).ToString();
            }

            if (Path.GetExtension(input) == ".avs")
                sb.Append("\"" + workPath + "\\avs4x26x.exe\"" + " -L ");
            sb.Append("\"" + Path.Combine(workPath, x264ExeComboBox.SelectedItem.ToString()) + "\"");
            // 编码模式
            switch (x264mode)
            {
                case 0: // 自定义
                    sb.Append(" " + x264CustomParameterTextBox.Text);
                    break;

                case 1: // crf
                    sb.Append(" --crf " + x264CRFNum.Value);
                    break;

                case 2: // 2pass
                    sb.Append(" --pass " + pass + " --bitrate " + x264BitrateNum.Value);
                    break;
            }
            if (x264mode != 0)
            {
                sb.Append(" --demuxer " + x264DemuxerComboBox.Text + " --threads " + x264ThreadsComboBox.SelectedItem.ToString());
                if (x264extraLine.Text != "")
                    sb.Append(" " + x264extraLine.Text);
                else
                    sb.Append(" --preset 8 " + " -I " + keyint + " -r 4 -b 3 --me umh -i 1 --scenecut 60 -f 1:1 --qcomp 0.5 --psy-rd 0.3:0 --aq-mode 2 --aq-strength 0.8");
                if (x264HeightNum.Value != 0 && x264WidthNum.Value != 0 && !MaintainResolutionCheckBox.Checked)
                    sb.Append(" --vf resize:" + x264WidthNum.Value + "," + x264HeightNum.Value + ",,,,lanczos");
            }
            if (!string.IsNullOrEmpty(sub))
            {
                string x264tmpline = sb.ToString();
                if (x264tmpline.IndexOf("--vf") == -1)
                    sb.Append(" --vf subtitles --sub \"" + sub + "\"");
                else
                {
                    Regex r = new Regex("--vf\\s\\S*");
                    Match m = r.Match(x264tmpline);
                    sb.Insert(m.Index + 5, "subtitles/").Append(" --sub \"" + sub + "\"");
                }
            }
            if (x264SeekNumericUpDown.Value != 0)
                sb.Append(" --seek " + x264SeekNumericUpDown.Value.ToString());
            if (x264FramesNumericUpDown.Value != 0)
                sb.Append(" --frames " + x264FramesNumericUpDown.Value.ToString());
            if (x264mode == 2 && pass == 1)
                sb.Append(" -o NUL");
            else if (!string.IsNullOrEmpty(output))
                sb.Append(" -o " + "\"" + output + "\"");
            if (!string.IsNullOrEmpty(input))
                sb.Append(" \"" + input + "\"");
            return sb.ToString();
        }

        public string x265bat(string input, string output, int pass = 1)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"" + workPath + "\\ffmpeg.exe\"" + " -i \"" + input + "\"");
            if (x264HeightNum.Value != 0 && x264WidthNum.Value != 0 && !MaintainResolutionCheckBox.Checked)
                sb.Append(" -s " + x264WidthNum.Value + "x" + x264HeightNum.Value);
            sb.Append(" -f yuv4mpegpipe -an -v 0 - | ");
            sb.Append(Util.FormatPath(Path.Combine(workPath, x264ExeComboBox.SelectedItem.ToString())) + " --y4m");
            // 编码模式
            switch (x264mode)
            {
                case 0: // 自定义
                    sb.Append(" " + x264CustomParameterTextBox.Text);
                    break;

                case 1: // crf
                    sb.Append(" --crf " + x264CRFNum.Value);
                    break;

                case 2: // 2pass
                    sb.Append(" --pass " + pass + " --bitrate " + x264BitrateNum.Value);
                    break;
            }
            if (x264mode != 0)
            {
                if (x264extraLine.Text != "")
                    sb.Append(" " + x264extraLine.Text);
                else
                    sb.Append(""); // 小丸工具箱除界面设置外的内置参数
            }
            if (x264SeekNumericUpDown.Value != 0)
                sb.Append(" --seek " + x264SeekNumericUpDown.Value.ToString());
            if (x264FramesNumericUpDown.Value != 0)
                sb.Append(" --frames " + x264FramesNumericUpDown.Value.ToString());
            if (x264mode == 2 && pass == 1)
                sb.Append(" -o NUL");
            else if (!string.IsNullOrEmpty(output))
                sb.Append(" -o " + "\"" + output + "\"");
            if (!string.IsNullOrEmpty(input))
                sb.Append(" -");
            return sb.ToString();
        }

        public static bool stringCheck(string str, string info = "")
        {
            if (string.IsNullOrEmpty(str))
            {
                MessageBox.Show("发现空或者无效的字符串 " + info);
            }
            return string.IsNullOrEmpty(str);
        }

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
            int AACbr = 1000 * Convert.ToInt32(AudioBitrateComboBox.Text);
            string br = AACbr.ToString();
            ffmpeg = "\"" + workPath + "\\ffmpeg.exe\" -i \"" + input + "\" -vn -sn -v 0 -c:a pcm_s16le -f wav pipe:|";
            switch (AudioEncoderComboBox.SelectedIndex)
            {
                case 0:
                    if (AudioBitrateRadioButton.Checked)
                    {
                        ffmpeg += "\"" + workPath + "\\neroAacEnc.exe\" -ignorelength -lc -br " + br + " -if - -of \"" + output + "\"";
                    }
                    if (AudioCustomizeRadioButton.Checked)
                    {
                        ffmpeg += "\"" + workPath + "\\neroAacEnc.exe\" -ignorelength " + AudioCustomParameterTextBox.Text.ToString() + " -if - -of \"" + output + "\"";
                    }
                    break;

                case 1:
                    if (AudioBitrateRadioButton.Checked)
                    {
                        ffmpeg += "\"" + workPath + "\\qaac.exe\" -q 2 --ignorelength -c " + AudioBitrateComboBox.Text + " - -o \"" + output + "\"";
                    }
                    if (AudioCustomizeRadioButton.Checked)
                    {
                        ffmpeg += "\"" + workPath + "\\qaac.exe\" --ignorelength " + AudioCustomParameterTextBox.Text.ToString() + " - -o \"" + output + "\"";
                    }
                    break;

                case 2:
                    if (Path.GetExtension(output) == ".aac")
                        output = Util.ChangeExt(output, ".wav");
                    ffmpeg = "\"" + workPath + "\\ffmpeg.exe\" -y -i \"" + input + "\" -f wav \"" + output + "\"";
                    break;

                case 3:
                    ffmpeg += "\"" + workPath + "\\refalac.exe\" --ignorelength - -o \"" + output + "\"";
                    break;

                case 4:
                    ffmpeg += "\"" + workPath + "\\flac.exe\" -f --ignore-chunk-sizes -5 - -o \"" + output + "\"";
                    break;

                case 5:
                    if (AudioBitrateRadioButton.Checked)
                    {
                        ffmpeg += "\"" + workPath + "\\fdkaac.exe\" --ignorelength -b " + AudioBitrateComboBox.Text + " - -o \"" + output + "\"";
                    }
                    if (AudioCustomizeRadioButton.Checked)
                    {
                        ffmpeg += "\"" + workPath + "\\fdkaac.exe\" --ignorelength " + AudioCustomParameterTextBox.Text.ToString() + " - -o \"" + output + "\"";
                    }
                    break;

                case 6:
                    ffmpeg = "\"" + workPath + "\\ffmpeg.exe\" -i \"" + input + "\" -c:a ac3 -b:a " + AudioBitrateComboBox.Text.ToString() + "k \"" + output + "\"";
                    break;

                default:
                    break;
            }
            aac = ffmpeg + "\r\n";
            return aac;
        }

        private string getAudioExt()
        {
            string ext = ".aac";
            switch (AudioEncoderComboBox.SelectedIndex)
            {
                case 0: ext = ".mp4"; break;
                case 1: ext = ".m4a"; break;
                case 2: ext = ".wav"; break;
                case 3: ext = ".m4a"; break;
                case 4: ext = ".flac"; break;
                case 5: ext = ".m4a"; break;
                case 6: ext = ".ac3"; break;
                default: ext = ".aac"; break;
            }
            return ext;
        }

        private void btnaudio_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "音频(*.mp4;*.aac;*.mp2;*.mp3;*.m4a;*.ac3)|*.mp4;*.aac;*.mp2;*.mp3;*.m4a;*.ac3|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                nameaudio = openFileDialog1.FileName;
                txtaudio.Text = nameaudio;
            }
        }

        private void btnvideo_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "视频(*.avi;*.mp4;*.m1v;*.m2v;*.m4v;*.264;*.h264;*.hevc)|*.avi;*.mp4;*.m1v;*.m2v;*.m4v;*.264;*.h264;*.hevc|所有文件(*.*)|*.*";
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
                ShowErrorMessage("请选择视频文件");
                return;
            }
            string inputExt = Path.GetExtension(txtvideo.Text.Trim()).ToLower();
            if (inputExt != ".avi"  //Only MPEG-4 SP/ASP video and MP3 audio supported at the current time. To import AVC/H264 video, you must first extract the avi track.
                    && inputExt != ".mp4" //MPEG-4 Video
                    && inputExt != ".m1v" //MPEG-1 Video
                    && inputExt != ".m2v" //MPEG-2 Video
                    && inputExt != ".m4v" //MPEG-4 Video
                    && inputExt != ".264" //AVC/H264 Video
                    && inputExt != ".h264" //AVC/H264 Video
                    && inputExt != ".hevc") //HEVC/H265 Video
            {
                ShowErrorMessage("输入文件: \r\n\r\n" + txtvideo.Text.Trim() + "\r\n\r\n是一个mp4box不支持的视频文件!");
                return;
            }

            if (nameout == "")
            {
                ShowErrorMessage("请选择输出文件");
                return;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(Util.FormatPath(workPath + "\\mp4box.exe") + " -add \"" + namevideo + "#trackID=1");
            if (Mp4BoxParComboBox.Text != "")
                sb.Append(":par=" + Mp4BoxParComboBox.Text);
            if (cbFPS.Text != "auto" && cbFPS.Text != "")
                sb.Append(":fps=" + cbFPS.Text);
            sb.Append(":name=\""); //输入raw时删除默认添加的gpac字符串
            if (nameaudio != "")
                sb.Append(" -add \"" + nameaudio + ":name=\"");
            sb.Append(" -new \"" + nameout + "\" \r\n cmd");
            mux = sb.ToString();
            batpath = workPath + "\\mux.bat";
            File.WriteAllText(batpath, mux, Encoding.Default);
            LogRecord(mux);
            Process.Start(batpath);
        }

        private void btnaextract_Click(object sender, EventArgs e)
        {
            //MP4 抽取音频1
            ExtractAV(namevideo, "a", 0);
            //if (namevideo == "")
            //{
            //    MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //else
            //{
            //    //aextract = "\"" + workPath + "\\mp4box.exe\" -raw 2 \"" + namevideo + "\"";
            //    aextract = "";
            //    aextract += Cmd.FormatPath(workPath + "\\ffmpeg.exe");
            //    aextract += " -i " + Cmd.FormatPath(namevideo);
            //    aextract += " -vn -sn -c:a:0 copy ";
            //    string outfile = Cmd.GetDir(namevideo) +
            //        Path.GetFileNameWithoutExtension(namevideo) + "_抽取音频1" + Path.GetExtension(namevideo);
            //    aextract += Cmd.FormatPath(outfile);
            //    batpath = workPath + "\\aextract.bat";
            //    File.WriteAllText(batpath, aextract, Encoding.Default);
            //    LogRecord(aextract);
            //    Process.Start(batpath);
            //}
        }

        private void ExtractAV(string namevideo, string av, int streamIndex)
        {
            if (string.IsNullOrEmpty(namevideo))
            {
                ShowErrorMessage("请选择视频文件");
                return;
            }

            string ext = Path.GetExtension(namevideo);
            //aextract = "\"" + workPath + "\\mp4box.exe\" -raw 2 \"" + namevideo + "\"";
            string aextract = "";
            aextract += Util.FormatPath(workPath + "\\ffmpeg.exe");
            aextract += " -i " + Util.FormatPath(namevideo);
            if (av == "a")
            {
                aextract += " -vn -sn -c:a copy -y -map 0:a:" + streamIndex + " ";

                MediaInfo MI = new MediaInfo();
                MI.Open(namevideo);
                string audioFormat = MI.Get(StreamKind.Audio, streamIndex, "Format");
                string audioProfile = MI.Get(StreamKind.Audio, streamIndex, "Format_Profile");
                if (!string.IsNullOrEmpty(audioFormat))
                {
                    if (audioFormat.Contains("MPEG") && audioProfile == "Layer 3")
                        ext = ".mp3";
                    else if (audioFormat.Contains("MPEG") && audioProfile == "Layer 2")
                        ext = ".mp2";
                    else if (audioFormat.Contains("PCM")) //flv support(PCM_U8 * PCM_S16BE * PCM_MULAW * PCM_ALAW * ADPCM_SWF)
                        ext = ".wav";
                    else if (audioFormat == "AAC")
                        ext = ".aac";
                    else if (audioFormat == "AC-3")
                        ext = ".ac3";
                    else if (audioFormat == "ALAC")
                        ext = ".m4a";
                    else
                        ext = ".mka";
                }
                else
                {
                    ShowInfoMessage("该轨道无音频");
                    return;
                }
            }
            else if (av == "v")
            {
                aextract += " -an -sn -c:v copy -y -map 0:v:" + streamIndex + " ";
            }
            else
            {
                throw new Exception("未知流！");
            }
            string suf = "_audio_";
            if (av == "v")
            {
                suf = "_video_";
            }
            suf += "index" + streamIndex;
            string outfile = Util.GetDir(namevideo) +
                Path.GetFileNameWithoutExtension(namevideo) + suf + ext;
            aextract += Util.FormatPath(outfile);
            batpath = workPath + "\\" + av + "extract.bat";
            File.WriteAllText(batpath, aextract, Encoding.Default);
            LogRecord(aextract);
            Process.Start(batpath);
        }

        private string ExtractAudio(string namevideo, string outfile, int streamIndex = 0)
        {
            if (string.IsNullOrEmpty(namevideo))
            {
                return "";
            }
            string ext = Path.GetExtension(namevideo);
            //aextract = "\"" + workPath + "\\mp4box.exe\" -raw 2 \"" + namevideo + "\"";
            string aextract = "";
            aextract += Util.FormatPath(workPath + "\\ffmpeg.exe");
            aextract += " -i " + Util.FormatPath(namevideo);
            aextract += " -vn -sn -c:a copy -y -map 0:a:" + streamIndex + " ";
            MediaInfo MI = new MediaInfo();
            MI.Open(namevideo);
            string audioFormat = MI.Get(StreamKind.Audio, streamIndex, "Format");
            string audioProfile = MI.Get(StreamKind.Audio, streamIndex, "Format_Profile");
            if (!string.IsNullOrEmpty(audioFormat))
            {
                if (audioFormat.Contains("MPEG") && audioProfile == "Layer 3")
                    ext = ".mp3";
                else if (audioFormat.Contains("MPEG") && audioProfile == "Layer 2")
                    ext = ".mp2";
                else if (audioFormat.Contains("PCM")) //flv support(PCM_U8 * PCM_S16BE * PCM_MULAW * PCM_ALAW * ADPCM_SWF)
                    ext = ".wav";
                else if (audioFormat == "AAC")
                    ext = ".aac";
                else if (audioFormat == "AC-3")
                    ext = ".ac3";
                else if (audioFormat == "ALAC")
                    ext = ".m4a";
                else
                    ext = ".mka";
            }
            else
            {
                return "";
            }
            aextract += Util.FormatPath(outfile) + "\r\n";
            return aextract;
        }

        private void ExtractTrack(string namevideo, int streamIndex)
        {
            if (string.IsNullOrEmpty(namevideo))
            {
                ShowErrorMessage("请选择视频文件");
                return;
            }

            string aextract = "";
            aextract += Util.FormatPath(workPath + "\\ffmpeg.exe");
            aextract += " -i " + Util.FormatPath(namevideo);
            aextract += " -map 0:" + streamIndex + " -c copy ";
            string suf = "_抽取流Index" + streamIndex;
            string outfile = Util.GetDir(namevideo) +
                Path.GetFileNameWithoutExtension(namevideo) + suf + '.' +
                FormatExtractor.Extract(workPath, namevideo)[streamIndex].Format;
            aextract += Util.FormatPath(outfile);
            batpath = workPath + "\\mkvextract.bat";
            File.WriteAllText(batpath, aextract, Encoding.Default);
            LogRecord(aextract);
            Process.Start(batpath);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowInfoMessage(string.Format(" \r\n有任何建议或疑问可以通过以下方式联系小丸。\nQQ：57655408\n微博：weibo.com/xiaowan3\n百度贴吧ID：小丸到达\n\n\t\t\t发布日期：2012年10月17日\n\t\t\t- ( ゜- ゜)つロ 乾杯~"), "关于");
        }

        private void btnvextract_Click(object sender, EventArgs e)
        {
            //MP4抽取视频1
            ExtractAV(namevideo, "v", 0);
            //if (namevideo == "")
            //{
            //    MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //else
            //{
            //    //vextract = "\"" + workPath + "\\mp4box.exe\" -raw 1 \"" + namevideo + "\"";
            //    vextract = "";
            //    vextract += Cmd.FormatPath(workPath + "\\ffmpeg.exe");
            //    vextract += " -i " + Cmd.FormatPath(namevideo);
            //    vextract += " -an -sn -c:v:0 copy ";
            //    string outfile = Cmd.GetDir(namevideo) +
            //        Path.GetFileNameWithoutExtension(namevideo) + "_抽取视频1" + Path.GetExtension(namevideo);
            //    vextract += Cmd.FormatPath(outfile);
            //    batpath = workPath + "\\vextract.bat";
            //    File.WriteAllText(batpath, vextract, Encoding.Default);
            //    LogRecord(vextract);
            //    Process.Start(batpath);
            //}
        }

        private void txtvideo_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtvideo.Text.Trim().Length > 0)
                {
                    if (!File.Exists(txtvideo.Text.Trim()))
                    {
                        throw new Exception("输入文件: \r\n\r\n" + txtvideo.Text.Trim() + "\r\n\r\n不存在!");
                    }
                    string inputExt = Path.GetExtension(txtvideo.Text.Trim()).ToLower();
                    //if (inputExt != ".avi"  //Only MPEG-4 SP/ASP video and MP3 audio supported at the current time. To import AVC/H264 video, you must first extract the avi track.
                    //        && inputExt != ".mp4" //MPEG-4 Video
                    //        && inputExt != ".m1v" //MPEG-1 Video
                    //        && inputExt != ".m2v" //MPEG-2 Video
                    //        && inputExt != ".m4v" //MPEG-4 Video
                    //        && inputExt != ".264" //AVC/H264 Video
                    //        && inputExt != ".h264" //AVC/H264 Video
                    //        && inputExt != ".hevc") //HEVC/H265 Video
                    //{
                    //    throw new Exception("输入文件: \r\n\r\n" + txtvideo.Text.Trim() + "\r\n\r\n是一个mp4box不支持的视频文件!");
                    //}
                    if (inputExt == ".264" || inputExt == ".h264" || inputExt == ".hevc")
                    {
                        ShowWarningMessage("H.264或者HEVC流文件mp4box将会自动侦测帧率\r\n如果侦测不到将默认为25fps\r\n如果你知道该文件的帧率建议手动设置");
                    }
                    namevideo = txtvideo.Text;
                    txtout.Text = Util.ChangeExt(txtvideo.Text, "_Mux.mp4");
                }
            }
            catch (Exception ex)
            {
                txtvideo.Text = string.Empty;
                ShowErrorMessage(ex.Message);
            }
        }

        private void txtaudio_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtaudio.Text.Trim().Length > 0)
                {
                    if (!File.Exists(txtaudio.Text.Trim()))
                    {
                        throw new Exception("输入文件: \r\n\r\n" + txtaudio.Text.Trim() + "\r\n\r\n不存在!");
                    }
                    string inputExt = Path.GetExtension(txtaudio.Text.Trim()).ToLower();
                    if (inputExt != ".mp4"
                            && inputExt != ".aac" //ADIF or RAW formats not supported
                            && inputExt != ".mp3"
                            && inputExt != ".m4a"
                            && inputExt != ".mp2"
                            && inputExt != ".ac3")
                    {
                        throw new Exception("输入文件: \r\n\r\n" + txtaudio.Text.Trim() + "\r\n\r\n是一个mp4box不支持的音频文件!");
                    }
                    nameaudio = txtaudio.Text;
                }
            }
            catch (Exception ex)
            {
                txtaudio.Text = string.Empty;
                ShowErrorMessage(ex.Message);
            }
        }

        private void txtout_TextChanged(object sender, EventArgs e)
        {
            nameout = txtout.Text;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            #region Delete Temp Files

            if (SetupDeleteTempFileCheckBox.Checked && !workPath.Equals("!undefined"))
            {
                List<string> deleteFileList = new List<string>();

                string systemDisk = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 3);
                string systemTempPath = systemDisk + @"windows\temp";

                //Delete all BAT files
                DirectoryInfo theFolder = new DirectoryInfo(workPath);
                foreach (FileInfo NextFile in theFolder.GetFiles())
                {
                    if (NextFile.Extension.Equals(".bat"))
                        deleteFileList.Add(NextFile.FullName);
                }

                //string[] deletedfiles = { tempPic, "msg.vbs", tempavspath, "temp.avs", "clip.bat", "aextract.bat", "vextract.bat",
                //                            "x264.bat", "aac.bat", "auto.bat", "mux.bat", "flv.bat", "mkvmerge.bat", "mkvextract.bat", "tmp.stat.mbtree", "tmp.stat" };
                string[] deletedfiles = { "vtemp.hevc", "atemp.m4a" , "atemp.flac" , "atemp.ac3" , "atemp.aac", "vtemp.mp4", "atemp.mp4", "concat.txt", tempPic, tempavspath, workPath + "msg.vbs", startpath + "\\x264_2pass.log.mbtree",
                                            startpath + "\\x264_2pass.log", "temp.hevc", startpath + "\\x265_2pass.log", startpath + "\\x265_2pass.log.cutree" };
                deleteFileList.AddRange(deletedfiles);

                foreach (string file in deleteFileList)
                {
                    File.Delete(file);
                }
            }

            #endregion Delete Temp Files

            #region Save Settings

            SaveSettings();

            #endregion Save Settings
        }

        private void txtvideo4_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtvideo4.Text.ToString()))
            {
                namevideo4 = txtvideo4.Text;
                //string finish = namevideo4.Insert(namevideo4.LastIndexOf(".")-1,"");
                //string ext = namevideo4.Substring(namevideo4.LastIndexOf(".") + 1, 3);
                //finish += "_clip." + ext;
                string finish = namevideo4.Insert(namevideo4.LastIndexOf("."), "_output");
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

        #region Settings

        /// <summary>
        /// 还原默认参数
        /// </summary>
        private void InitParameter()
        {
            #region Video Tab

            x264CRFNum.Value = 23.5m;
            x264BitrateNum.Value = 800;
            x264AudioParameterTextBox.Text = "--abitrate 128";
            x264AudioModeComboBox.SelectedIndex = 0;
            x264DemuxerComboBox.SelectedIndex = 0;
            x264WidthNum.Value = 0;
            x264HeightNum.Value = 0;
            x264CustomParameterTextBox.Text = "";
            x264PriorityComboBox.SelectedIndex = 2;
            x264FramesNumericUpDown.Value = 0;
            x264SeekNumericUpDown.Value = 0;
            x264Mode1RadioButton.Checked = true;
            x264ShutdownCheckBox.Checked = false;

            #endregion Video Tab

            #region Audio Tab

            AudioEncoderComboBox.SelectedIndex = 0;
            AudioPresetComboBox.SelectedIndex = 0;
            AudioBitrateComboBox.Text = "128";
            AudioBitrateRadioButton.Checked = true;

            #endregion Audio Tab

            #region General Tab

            OnePicAudioBitrateNum.Value = 128;
            OnePicFPSNum.Value = 1;
            OnePicCRFNum.Value = 24;

            BlackFPSNum.Value = 1;
            BlackCRFNum.Value = 51;
            BlackBitrateNum.Value = 900;

            maskb.Text = "000000";
            maske.Text = "000020";

            TransposeComboBox.SelectedIndex = 1;

            #endregion General Tab

            #region Mux Tab

            cbFPS.SelectedIndex = 0;
            Mp4BoxParComboBox.SelectedIndex = 0;
            MuxAacEncoderComboBox.SelectedIndex = 0;
            MuxFormatComboBox.Text = "flv";

            #endregion Mux Tab

            #region AVS Tab

            AVSwithAudioCheckBox.Checked = false;

            #endregion AVS Tab

            #region Setup Tab

            SplashScreenCheckBox.Checked = true;
            TrayModeCheckBox.Checked = false;
            x264PriorityComboBox.SelectedIndex = 2;
            x264ThreadsComboBox.SelectedIndex = 0;
            SetupDeleteTempFileCheckBox.Checked = true;
            CheckUpdateCheckBox.Checked = true;

            #endregion Setup Tab
        }

        private void LoadSettings()
        {
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
                x264extraLine.Text = ConfigurationManager.AppSettings["x264ExtraParameter"];
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
                CheckUpdateCheckBox.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["CheckUpdate"]);
                x264ThreadsComboBox.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["x264Threads"]);
                TrayModeCheckBox.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["TrayMode"]);
                SplashScreenCheckBox.Checked = Convert.ToBoolean(ConfigurationManager.AppSettings["SplashScreen"]);
                SetupPlayerTextBox.Text = ConfigurationManager.AppSettings["PreviewPlayer"];
                string SubLangExt = Convert.ToString(ConfigurationManager.AppSettings["SubLanguageExtension"]);
                MuxFormatComboBox.SelectedIndex = Convert.ToInt32(ConfigurationManager.AppSettings["MuxFormat"]);
                x264BatchSubSpecialLanguage.DataSource = SubLangExt.Split(',');
                if (x264ExeComboBox.SelectedIndex == -1)
                {
                    x264ExeComboBox.SelectedIndex = x264ExeComboBox.Items.IndexOf("x264_32-8bit");
                }

                if (int.Parse(ConfigurationManager.AppSettings["LanguageIndex"]) == -1)  //First Startup
                {
                    string culture = Thread.CurrentThread.CurrentCulture.Name;
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

                if (CheckUpdateCheckBox.Checked && Util.IsConnectInternet())
                {
                    DateTime d;
                    bool f;
                    CheckUpadateDelegate checkUpdateDelegate = CheckUpdate;
                    checkUpdateDelegate.BeginInvoke(out d, out f, new AsyncCallback(CheckUpdateCallBack), null);
                }
                x264ExeComboBox_SelectedIndexChanged(null, null);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SaveSettings()
        {
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
            cfa.AppSettings.Settings["x264ExtraParameter"].Value = x264extraLine.Text;
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
            cfa.AppSettings.Settings["CheckUpdate"].Value = CheckUpdateCheckBox.Checked.ToString();
            cfa.AppSettings.Settings["TrayMode"].Value = TrayModeCheckBox.Checked.ToString();
            cfa.AppSettings.Settings["LanguageIndex"].Value = languageComboBox.SelectedIndex.ToString();
            cfa.AppSettings.Settings["SplashScreen"].Value = SplashScreenCheckBox.Checked.ToString();
            cfa.AppSettings.Settings["x264Threads"].Value = x264ThreadsComboBox.SelectedIndex.ToString();
            cfa.AppSettings.Settings["PreviewPlayer"].Value = SetupPlayerTextBox.Text;
            cfa.AppSettings.Settings["MuxFormat"].Value = MuxFormatComboBox.SelectedIndex.ToString(); ;
            cfa.Save();
            ConfigurationManager.RefreshSection("appSettings"); // 刷新命名节，在下次检索它时将从磁盘重新读取它。记住应用程序要刷新节点
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            SYSTEM_INFO pSI = new SYSTEM_INFO();
            GetSystemInfo(ref pSI);
            int processorNumber = (int)pSI.dwNumberOfProcessors;

            x264ThreadsComboBox.Items.Add("auto");
            for (int i = 1; i <= 16; i++)
            {
                x264ThreadsComboBox.Items.Add(i.ToString());
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
            workPath = startpath + "\\tools";
            if (!Directory.Exists(workPath))
            {
                MessageBox.Show("tools文件夹没有解压喔~ 工具箱里没有工具的话运行不起来的喔~", "（这只丸子）",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
            //Directory.CreateDirectory(workPath);
            //string diskSymbol = startpath.Substring(0, 1);

            //string systemDisk = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 3);
            //string systemTempPath = systemDisk + @"windows\temp";
            string systemTempPath = Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine);
            tempavspath = systemTempPath + "\\temp.avs";
            tempPic = systemTempPath + "\\marukotemp.jpg";

            //load x264 exe
            DirectoryInfo folder = new DirectoryInfo(workPath);
            List<string> x264exe = new List<string>();
            foreach (FileInfo FileName in folder.GetFiles())
            {
                if ((FileName.Name.ToLower().Contains("x264") || FileName.Name.ToLower().Contains("xxxx")) && Path.GetExtension(FileName.Name) == ".exe")
                {
                    x264exe.Add(FileName.Name);
                }
            }
            x264exe = x264exe.OrderByDescending(i => i.Substring(7)).ToList();
            x264ExeComboBox.Items.AddRange(x264exe.ToArray());

            //load AVS filter
            DirectoryInfo avspath = new DirectoryInfo(workPath + "\\avsfilter");
            List<string> avsfilters = new List<string>();
            foreach (FileInfo FileName in avspath.GetFiles())
            {
                if (Path.GetExtension(FileName.Name) == ".dll")
                {
                    avsfilters.Add(FileName.Name);
                }
            }
            AVSFilterComboBox.Items.AddRange(avsfilters.ToArray());

            //ReleaseDate = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location); //获得程序编译时间
            ReleaseDatelabel.Text = ReleaseDate.ToString("yyyy-M-d");

            ////load Help Text
            if (File.Exists(startpath + "\\help.rtf"))
            {
                HelpTextBox.LoadFile(startpath + "\\help.rtf");
            }

            LoadSettings();

            //create directory
            string preset = workPath + "\\preset";
            if (!Directory.Exists(preset))
                Directory.CreateDirectory(preset);
            DirectoryInfo TheFolder = new DirectoryInfo(preset);
            foreach (FileInfo FileName in TheFolder.GetFiles())
            {
                VideoPresetComboBox.Items.Add(FileName.Name.Replace(".txt", ""));
            }
            if (File.Exists("preset.xml"))
            {
                xdoc = XDocument.Load("preset.xml");
                XElement xAudios = xdoc.Element("root").Element("Audio");
                foreach (XElement item in xAudios.Elements())
                {
                    AudioPresetComboBox.Items.Add(item.Attribute("Name").Value);
                    AudioPresetComboBox.SelectedIndex = 0;
                }
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (namevideo6 == "")
            {
                ShowErrorMessage("请选择视频文件");
            }
            else
            {
                mkvextract = workPath + "\\ mkvextract.exe tracks \"" + namevideo6 + "\" 1:video.h264 2:audio.aac";
                batpath = workPath + "\\mkvextract.bat";
                File.WriteAllText(batpath, mkvextract, Encoding.Default);
                Process.Start(batpath);
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
                ShowErrorMessage("请选择视频文件");
            }
            else if (nameaudio3 == "")
            {
                ShowErrorMessage("请选择音频文件");
            }
            else if (nameout6 == "")
            {
                ShowErrorMessage("请选择输出文件");
            }
            else
            {
                mkvmerge = workPath + "\\mkvmerge.exe -o \"" + nameout6 + "\"   \"" + namevideo5 + "\"   \"" + nameaudio3 + "\"";
                batpath = workPath + "\\mkvmerge.bat";
                File.WriteAllText(batpath, mkvmerge, Encoding.Default);
                Process.Start(batpath);
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
                ShowErrorMessage("请选择文件");
            }
            else
            {
                if (txtaudio3.Text != "" && txtsub.Text != "")
                {
                    mkvmerge = "\"" + workPath + "\\mkvmerge.exe\" -o \"" + nameout6 + "\" \"" + namevideo5 + "\" \"" + nameaudio3 + "\" \"" + namesub + "\"";
                }
                if (txtaudio3.Text == "" && txtsub.Text == "")
                {
                    mkvmerge = "\"" + workPath + "\\mkvmerge.exe\" -o \"" + nameout6 + "\" \"" + namevideo5 + "\"";
                }
                if (txtaudio3.Text != "" && txtsub.Text == "")
                {
                    mkvmerge = "\"" + workPath + "\\mkvmerge.exe\" -o \"" + nameout6 + "\" \"" + namevideo5 + "\" \"" + nameaudio3 + "\"";
                }
                if (txtaudio3.Text == "" && txtsub.Text != "")
                {
                    mkvmerge = "\"" + workPath + "\\mkvmerge.exe\" -o \"" + nameout6 + "\" \"" + namevideo5 + "\" \"" + namesub + "\"";
                }
                mkvmerge += "\r\ncmd";
                batpath = workPath + "\\mkvmerge.bat";
                File.WriteAllText(batpath, mkvmerge, Encoding.Default);
                LogRecord(mkvmerge);
                Process.Start(batpath);
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
            openFileDialog1.Filter = "音频(*.mp3;*.aac;*.ac3)|*.mp3;*.aac;*.ac3|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                nameaudio3 = openFileDialog1.FileName;
                txtaudio3.Text = nameaudio3;
            }
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "字幕(*.ass;*.ssa;*.srt)|*.ass;*.ssa;*.srt|所有文件(*.*)|*.*";
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
                ShowErrorMessage("请选择视频文件");
            }
            else
            {
                int i = namevideo6.IndexOf(".mkv");
                string mkvname = namevideo6.Remove(i);
                mkvextract = "\"" + workPath + "\\mkvextract.exe\" tracks \"" + namevideo6 + "\" 1:\"" + mkvname + "_video.h264\" 2:\"" + mkvname + "_audio.aac\"";
                batpath = workPath + "\\mkvextract.bat";
                File.WriteAllText(batpath, mkvextract, Encoding.Default);
                Process.Start(batpath);
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
            ListBox listbox = (ListBox)sender;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String s in files)
                {
                    listbox.Items.Add(s);
                }
                return;
            }
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
            ListBox listbox = (ListBox)sender;
            if (e.Data.GetDataPresent(typeof(System.String)) && ((ListBox)sender).Equals(listbox))
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
            if (indexofsource == 65535)
                return;
            if (indexofsource != ListBox.NoMatches)
            {
                ((ListBox)sender).DoDragDrop(((ListBox)sender).Items[indexofsource].ToString(), DragDropEffects.All);
            }
        }

        public string VideoBatch(string input, string output)
        {
            bool hasAudio = false;
            string bat = "";
            string tempVideo = "vtemp.mp4";
            string tempAudio = "atemp" + getAudioExt();

            //检测是否含有音频
            MediaInfo MI = new MediaInfo();
            MI.Open(input);
            string audio = MI.Get(StreamKind.Audio, 0, "Format");
            if (!string.IsNullOrEmpty(audio)) { hasAudio = true; }
            string sub = (x264BatchSubCheckBox.Checked) ? GetSubtitlePath(input) : string.Empty;

            int audioMode = x264AudioModeComboBox.SelectedIndex;
            if (!hasAudio)
                audioMode = 1;
            switch (audioMode)
            {
                case 0:
                    aextract = audiobat(input, tempAudio);
                    break;
                case 1:
                    aextract = "";
                    break;
                case 2:
                    if (audio.ToLower() == "aac")
                    {
                        tempAudio = "atemp.aac";
                        aextract = ExtractAudio(input, tempAudio);
                    }
                    else
                        aextract = audiobat(input, tempAudio);
                    break;
                default:
                    break;
            }

            if (x264ExeComboBox.SelectedItem.ToString().ToLower().Contains("x264"))
            {
                if (x264mode == 2)
                    x264 = x264bat(input, tempVideo, 1, sub) + "\r\n" +
                           x264bat(input, tempVideo, 2, sub);
                else x264 = x264bat(input, tempVideo, 0, sub);
                if (audioMode == 1 || !hasAudio)
                    x264 = x264.Replace(tempVideo, output);
            }
            else if (x264ExeComboBox.SelectedItem.ToString().ToLower().Contains("x265"))
            {
                tempVideo = "vtemp.hevc";
                if (x264mode == 2)
                    x264 = x265bat(input, tempVideo, 1) + "\r\n" +
                           x265bat(input, tempVideo, 2);
                else x264 = x265bat(input, tempVideo, 0);
                if (audioMode == 1 || !hasAudio)
                {
                    x264 += "\r\n\"" + workPath + "\\mp4box.exe\"  -add  \"" + tempVideo + "#trackID=1:name=\" -new \"" + output + "\" \r\n";
                    //x264 += "del \"" + tempVideo + "\"";
                }
            }
            x264 += "\r\n";

            //封装
            if (VideoBatchFormatComboBox.Text == "mp4")
                mux = boxmuxbat(tempVideo, tempAudio, output);
            else
                mux = ffmuxbat(tempVideo, tempAudio, output);
            if (audioMode != 1 && hasAudio) //如果压制音频
                bat += aextract + x264 + mux + " \r\n";
            else
                bat += x264;

            bat += "echo ===== one file is completed! =====\r\n";
            return bat;
        }

        private void btnBatchAuto_Click(object sender, EventArgs e)
        {
            if (lbAuto.Items.Count == 0)
            {
                ShowErrorMessage("请输入视频！");
                return;
            }

            if (x264ExeComboBox.SelectedIndex == -1)
            {
                ShowErrorMessage("请选择X264程序");
                return;
            }

            string bat = "";
            for (int i = 0; i < this.lbAuto.Items.Count; i++)
            {
                string input = lbAuto.Items[i].ToString();
                string output;
                if (Directory.Exists(x264PathTextBox.Text))
                    output = x264PathTextBox.Text + "\\" + Path.GetFileNameWithoutExtension(input) + "_batch." + VideoBatchFormatComboBox.Text;
                else
                    output = Util.ChangeExt(input, "_batch." + VideoBatchFormatComboBox.Text);
                bat += VideoBatch(lbAuto.Items[i].ToString(), output);
            }

            LogRecord(bat);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(GetCultureName());
            WorkingForm wf = new WorkingForm(bat, lbAuto.Items.Count);
            wf.Owner = this;
            wf.Show();
            //batpath = workPath + "\\auto.bat";
            //File.WriteAllText(batpath, bat, Encoding.Default);
            //Process.Start(batpath);
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

        private void btnBatchMP4_Click(object sender, EventArgs e)
        {
            if (lbffmpeg.Items.Count != 0)
            {
                string ext = MuxFormatComboBox.Text;
                string mux = "";
                for (int i = 0; i < lbffmpeg.Items.Count; i++)
                {
                    string filePath = lbffmpeg.Items[i].ToString();
                    //如果是源文件的格式和目标格式相同则跳过
                    if (Path.GetExtension(filePath).Contains(ext))
                        continue;
                    string finish = filePath.Remove(filePath.LastIndexOf(".") + 1) + ext;
                    aextract = "";

                    //检测音频是否需要转换为AAC
                    MediaInfo MI = new MediaInfo();
                    MI.Open(filePath);
                    string audio = MI.Get(StreamKind.Audio, 0, "Format");
                    if (audio.ToLower() != "aac")
                    {
                        mux += "\"" + workPath + "\\ffmpeg.exe\" -y -i \"" + lbffmpeg.Items[i].ToString() + "\" -c:v copy -c:a " + MuxAacEncoderComboBox.Text + " \"" + finish + "\" \r\n";
                    }
                    else
                    {
                        mux += "\"" + workPath + "\\ffmpeg.exe\" -y -i \"" + lbffmpeg.Items[i].ToString() + "\" -c copy \"" + finish + "\" \r\n";
                    }
                }
                mux += "\r\ncmd";
                batpath = workPath + "\\mux.bat";
                File.WriteAllText(batpath, mux, Encoding.Default);
                LogRecord(mux);
                Process.Start(batpath);
            }
            else ShowErrorMessage("请输入视频！");
        }

        private void txtvideo8_TextChanged(object sender, EventArgs e)
        {
            namevideo8 = txtvideo8.Text;
        }

        private void btnvextract8_Click(object sender, EventArgs e)
        {
            //FLV vcopy
            ExtractAV(namevideo8, "v", 0);
            //if (namevideo8 == "")
            //{
            //    MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //else
            //{
            //    vextract = "\"" + workPath + "\\FLVExtractCL.exe\" -v \"" + namevideo8 + "\"";
            //    batpath = workPath + "\\vextract.bat";
            //    File.WriteAllText(batpath, vextract, Encoding.Default);
            //    LogRecord(vextract);
            //    Process.Start(batpath);
            //}
        }

        private void btnaextract8_Click(object sender, EventArgs e)
        {
            //FLV acopy
            ExtractAV(namevideo8, "a", 0);
            //if (namevideo8 == "")
            //{
            //    MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //else
            //{
            //    aextract = "\"" + workPath + "\\FLVExtractCL.exe\" -a \"" + namevideo8 + "\"";
            //    batpath = workPath + "\\aextract.bat";
            //    File.WriteAllText(batpath, aextract, Encoding.Default);
            //    LogRecord(aextract);
            //    Process.Start(batpath);
            //}
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
                string filepath = workPath + "\\temp.avs";
                File.WriteAllText(filepath, AVSScriptTextBox.Text.ToString(), Encoding.Default);
                if (File.Exists(SetupPlayerTextBox.Text))
                {
                    Process.Start(SetupPlayerTextBox.Text, filepath);
                }
                else
                {
                    PreviewForm pf = new PreviewForm();
                    pf.Show();
                    pf.axWindowsMediaPlayer1.URL = filepath;
                }
            }
            else
            {
                ShowErrorMessage("请输入正确的AVS脚本！");
            }
        }

        private void txtout_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtout.Text.ToString()))
            {
                Process.Start(txtout.Text.ToString());
            }
        }

        private void txtout3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(AudioOutputTextBox.Text.ToString()))
            {
                Process.Start(AudioOutputTextBox.Text.ToString());
            }
        }

        private void txtout6_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtout6.Text.ToString()))
            {
                Process.Start(txtout6.Text.ToString());
            }
        }

        private void txtout9_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtout9.Text.ToString()))
            {
                Process.Start(txtout9.Text.ToString());
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
            x264DemuxerComboBox.SelectedIndex = 0; //压制AVS始终使用分离器为auto

            if (string.IsNullOrEmpty(nameout9))
            {
                ShowErrorMessage("请选择输出文件");
                return;
            }
            if (Path.GetExtension(nameout9).ToLower() != ".mp4")
            {
                ShowErrorMessage("仅支持MP4输出", "不支持的输出格式");
                return;
            }
            if (File.Exists(txtout9.Text.Trim()))
            {
                DialogResult dgs = ShowQuestion("目标文件:\r\n\r\n" + txtout9.Text.Trim() + "\r\n\r\n已经存在,是否覆盖继续压制？", "目标文件已经存在");
                if (dgs == DialogResult.No) return;
            }

            string tempVideo = "vtemp.mp4";
            string tempAudio = "atemp" + getAudioExt();
            string filepath = tempavspath;
            //string filepath = workpath + "\\temp.avs";
            File.WriteAllText(filepath, AVSScriptTextBox.Text, Encoding.Default);

            //检测是否含有音频
            bool hasAudio = false;
            MediaInfo MI = new MediaInfo();
            MI.Open(namevideo9);
            string audio = MI.Get(StreamKind.Audio, 0, "Format");
            if (!string.IsNullOrEmpty(audio)) { hasAudio = true; }

            //audio
            if (AVSwithAudioCheckBox.Checked && hasAudio)
            {
                if (!File.Exists(txtvideo9.Text))
                {
                    ShowErrorMessage("请选择视频文件");
                    return;
                }
                aextract = audiobat(namevideo9, tempAudio);
            }
            else
                aextract = "";

            //video
            if (x264ExeComboBox.SelectedItem.ToString().ToLower().Contains("x264"))
            {
                if (x264mode == 2)
                    x264 = x264bat(filepath, tempVideo, 1) + "\r\n" +
                           x264bat(filepath, tempVideo, 2);
                else x264 = x264bat(filepath, tempVideo);
                if (!AVSwithAudioCheckBox.Checked || !hasAudio)
                    x264 = x264.Replace(tempVideo, nameout9);
            }
            else if (x264ExeComboBox.SelectedItem.ToString().ToLower().Contains("x265"))
            {
                tempVideo = "vtemp.hevc";
                if (x264mode == 2)
                    x264 = x265bat(filepath, tempVideo, 1) + "\r\n" +
                           x265bat(filepath, tempVideo, 2);
                else x264 = x265bat(filepath, tempVideo);
                if (!AVSwithAudioCheckBox.Checked || !hasAudio)
                {
                    x264 += "\r\n\"" + workPath + "\\mp4box.exe\"  -add  \"" + tempVideo + "#trackID=1:name=\" -new \"" + nameout9 + "\" \r\n";
                    x264 += "del \"" + tempVideo + "\"";
                }
            }
            //mux
            if (AVSwithAudioCheckBox.Checked && hasAudio) //如果包含音频
                mux = boxmuxbat(tempVideo, tempAudio, nameout9);
            else
                mux = "";

            auto = aextract + x264 + "\r\n" + mux + " \r\n";
            auto += "\r\necho ===== one file is completed! =====\r\n";
            LogRecord(auto);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(GetCultureName());
            WorkingForm wf = new WorkingForm(auto);
            wf.Owner = this;
            wf.Show();
            //auto += "\r\ncmd";
            //batpath = workPath + "\\x264avs.bat";
            //File.WriteAllText(batpath, auto, Encoding.Default);
            //Process.Start(batpath);
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
            avs += string.Format("\r\nLWLibavVideoSource(\"{0}\",23.976,convertFPS=True)\r\nConvertToYV12()\r\nCrop(0,0,0,0)\r\nAddBorders(0,0,0,0)\r\n" + "TextSub(\"{1}\")\r\n#LanczosResize(1280,960)\r\n", namevideo9, namesub9);
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
                Process.Start(txtvideo.Text.ToString());
            }
        }

        private void txtvideo4_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo4.Text.ToString()))
            {
                Process.Start(txtvideo4.Text.ToString());
            }
        }

        private void txtout5_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtout5.Text.ToString()))
            {
                Process.Start(txtout5.Text.ToString());
            }
        }

        private void txtvideo8_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo8.Text.ToString()))
            {
                Process.Start(txtvideo8.Text.ToString());
            }
        }

        private void txtvideo9_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo9.Text.ToString()))
            {
                Process.Start(txtvideo9.Text.ToString());
            }
        }

        private void txtvideo6_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo6.Text.ToString()))
            {
                Process.Start(txtvideo6.Text.ToString());
            }
        }

        private void txtvideo5_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtvideo5.Text.ToString()))
            {
                Process.Start(txtvideo5.Text.ToString());
            }
        }

        private void txtaudio3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtaudio3.Text.ToString()))
            {
                Process.Start(txtaudio3.Text.ToString());
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
            openFileDialog1.Filter = "字幕(*.ass;*.ssa;*.srt)|*.ass;*.ssa;*.srt|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                namesub9 = openFileDialog1.FileName;
                txtsub9.Text = namesub9;
            }
        }

        private void btnvideo9_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "视频(*.mp4;*.flv;*.mkv;*.wmv)|*.mp4;*.flv;*.mkv;*.wmv|所有文件(*.*)|*.*";
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
                ShowErrorMessage("请选择视频文件");
            }
            else if (nameout5 == "")
            {
                ShowErrorMessage("请选择输出文件");
            }
            else
            {
                //int h1 = int.Parse(maskb.Text.ToString().Substring(0, 2));
                //int m1 = int.Parse(maskb.Text.ToString().Substring(3, 2));
                //int s1 = int.Parse(maskb.Text.ToString().Substring(6, 2));
                //int h2 = int.Parse(maske.Text.ToString().Substring(0, 2));
                //int m2 = int.Parse(maske.Text.ToString().Substring(3, 2));
                //int s2 = int.Parse(maske.Text.ToString().Substring(6, 2));
                //clip = "\"" + workPath + "\\ffmpeg.exe\" -ss " + maskb.Text + " -to " + maske.Text + " -i  \"" + namevideo4 + "\" -acodec copy -vcodec copy \"" + nameout5 + "\" \r\ncmd";

                // "<workPath>\ffmpeg.exe" -i "<namevideo4>" -ss <maskb.Text> -to <maske.Text> -c copy "<nameout5>"
                clip = string.Format(@"""{0}\ffmpeg.exe"" -i ""{1}"" -ss {2} -to {3} -y -c copy ""{4}""",
                    workPath, namevideo4, maskb.Text, maske.Text, nameout5) + Environment.NewLine + "cmd";
                batpath = workPath + "\\clip.bat";
                LogRecord(clip);
                File.WriteAllText(batpath, clip, Encoding.Default);
                Process.Start(batpath);
            }
        }

        private void cbX264_SelectedIndexChanged(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(workPath + "\\preset\\" + VideoPresetComboBox.Text + ".txt", System.Text.Encoding.Default);
            x264CustomParameterTextBox.Text = sr.ReadToEnd();
            sr.Close();
        }

        private void cbFPS_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ext = Path.GetExtension(namevideo).ToLower();
            if (cbFPS.SelectedIndex != 0 && ext != ".264" && ext != ".h264" && ext != ".hevc")
            {
                ShowWarningMessage("只有扩展名为.264 .h264 .hevc的流文件设置帧率(fps)才有效");
                cbFPS.SelectedIndex = 0;
            }
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
            //MKV抽0
            ExtractTrack(namevideo6, 0);
        }

        private void MkvExtract1Button_Click(object sender, EventArgs e)
        {
            //MKV 抽1
            ExtractTrack(namevideo6, 1);
        }

        private void MkvExtract2Button_Click(object sender, EventArgs e)
        {
            //MKV 抽2
            ExtractTrack(namevideo6, 2);
        }

        private void MkvExtract3Button_Click(object sender, EventArgs e)
        {
            //MKV 抽3
            ExtractTrack(namevideo6, 3);
        }

        private void MkvExtract4Button_Click(object sender, EventArgs e)
        {
            //MKV 抽4
            ExtractTrack(namevideo6, 4);
        }

        private void txtMI_TextChanged(object sender, EventArgs e)
        {
            MItext = MediaInfoTextBox.Text;
        }

        private void txtAVScreate_Click(object sender, EventArgs e)
        {
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.sosg.net/read.php?tid=480646");
        }

        private void btnaextract2_Click(object sender, EventArgs e)
        {
            //MP4 抽取音频2
            ExtractAV(namevideo, "a", 1);
            //if (namevideo == "")
            //{
            //    MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //else
            //{
            //    //aextract = "\"" + workPath + "\\mp4box.exe\" -raw 3 \"" + namevideo + "\"";
            //    aextract = "";
            //    aextract += Cmd.FormatPath(workPath + "\\ffmpeg.exe");
            //    aextract += " -i " + Cmd.FormatPath(namevideo);
            //    aextract += " -vn -sn -c:a:1 copy ";
            //    string outfile = Cmd.GetDir(namevideo) +
            //        Path.GetFileNameWithoutExtension(namevideo) + "_抽取音频2" + Path.GetExtension(namevideo);
            //    aextract += Cmd.FormatPath(outfile);
            //    batpath = workPath + "\\aextract.bat";
            //    File.WriteAllText(batpath, aextract, Encoding.Default);
            //    LogRecord(aextract);
            //    Process.Start(batpath);
            //}
        }

        private void btnaextract3_Click(object sender, EventArgs e)
        {
            //MP4 抽取音频3
            ExtractAV(namevideo, "a", 2);
            //if (namevideo == "")
            //{
            //    MessageBox.Show("请选择视频文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //}
            //else
            //{
            //    //aextract = "\"" + workPath + "\\mp4box.exe\" -raw 4 \"" + namevideo + "\"";
            //    aextract = "";
            //    aextract += Cmd.FormatPath(workPath + "\\ffmpeg.exe");
            //    aextract += " -i " + Cmd.FormatPath(namevideo);
            //    aextract += " -vn -sn -c:a:2 copy ";
            //    string outfile = Cmd.GetDir(namevideo) +
            //        Path.GetFileNameWithoutExtension(namevideo) + "_抽取音频3" + Path.GetExtension(namevideo);
            //    aextract += Cmd.FormatPath(outfile);
            //    batpath = workPath + "\\aextract.bat";
            //    File.WriteAllText(batpath, aextract, Encoding.Default);
            //    LogRecord(aextract);
            //    Process.Start(batpath);
            //}
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
            DateTime CompileDate = File.GetLastWriteTime(this.GetType().Assembly.Location); //获得程序编译时间
            QQMessageBox.Show(
                this,
                "小丸工具箱 七七版\r\n主页：http://www.maruko.in/ \r\n编译日期：" + CompileDate.ToString(),
                "关于",
                QQMessageBoxIcon.Information,
                QQMessageBoxButtons.OK);
        }

        private void HomePageBtn_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.maruko.in/");
        }

        #endregion 帮助页面

        #region 视频页面

        private void x264VideoBtn_Click(object sender, EventArgs e)
        {
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
            savefile.Filter = "MPEG-4 视频(*.mp4)|*.mp4|Flash 视频(*.flv)|*.flv|Matroska 视频(*.mkv)|*.mkv|AVI 视频(*.avi)|*.avi|H.264 流(*.raw)|*.raw";
            savefile.FileName = Path.GetFileName(x264OutTextBox.Text);
            DialogResult result = savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                nameout2 = savefile.FileName;
                x264OutTextBox.Text = nameout2;
            }
        }

        private void x264SubBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "字幕(*.ass;*.ssa;*.srt)|*.ass;*.ssa;*.srt|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                namesub2 = openFileDialog1.FileName;
                x264SubTextBox.Text = namesub2;
            }
        }

        private void x264StartBtn_Click(object sender, EventArgs e)
        {
            #region validation

            if (string.IsNullOrEmpty(namevideo2))
            {
                ShowErrorMessage("请选择视频文件");
                return;
            }

            if (!string.IsNullOrEmpty(namesub2) && !File.Exists(namesub2))
            {
                ShowErrorMessage("字幕文件不存在，请重新选择");
                return;
            }

            if (string.IsNullOrEmpty(nameout2))
            {
                ShowErrorMessage("请选择输出文件");
                return;
            }

            if (x264ExeComboBox.SelectedIndex == -1)
            {
                ShowErrorMessage("请选择X264程序");
                return;
            }

            //防止未选择 x264 thread
            if (x264ThreadsComboBox.SelectedIndex == -1)
            {
                x264ThreadsComboBox.SelectedIndex = 0;
            }

            //目标文件已经存在提示是否覆盖
            if (File.Exists(x264OutTextBox.Text.Trim()))
            {
                DialogResult dgs = ShowQuestion("目标文件:\r\n\r\n" + x264OutTextBox.Text.Trim() + "\r\n\r\n已经存在,是否覆盖继续压制？", "目标文件已经存在");
                if (dgs == DialogResult.No) return;
            }

            //如果是AVS复制到C盘根目录
            if (Path.GetExtension(x264VideoTextBox.Text) == ".avs")
            {
                //if (File.Exists(tempavspath)) File.Delete(tempavspath);
                File.Copy(x264VideoTextBox.Text, tempavspath, true);
                namevideo2 = tempavspath;
                x264DemuxerComboBox.SelectedIndex = 0; //压制AVS始终使用分离器为auto
            }

            #endregion validation

            string ext = Path.GetExtension(nameout2).ToLower();
            bool hasAudio = false;
            string tempVideo = Util.ChangeExt(namevideo2, "_vtemp.mp4");
            string tempAudio = Util.ChangeExt(namevideo2, "_atemp" + getAudioExt());

            #region Audio

            //检测是否含有音频
            MediaInfo MI = new MediaInfo();
            MI.Open(namevideo2);
            string audio = MI.Get(StreamKind.Audio, 0, "Format");
            if (!string.IsNullOrEmpty(audio))
                hasAudio = true;
            int audioMode = x264AudioModeComboBox.SelectedIndex;
            if (!hasAudio)
            {
                DialogResult r = ShowQuestion("经检测，视频不包含音频流是否采用无音频流方式压制？", "提示");
                if (r == DialogResult.Yes)
                    audioMode = 1;
            }
            switch (audioMode)
            {
                case 0:
                    aextract = audiobat(namevideo2, tempAudio);
                    break;
                case 1:
                    aextract = "";
                    break;
                case 2:
                    if (audio.ToLower() == "aac")
                    {
                        tempAudio = Util.ChangeExt(namevideo2, "_atemp.aac");
                        aextract = ExtractAudio(namevideo2, tempAudio);
                    }
                    else
                    {
                        ShowInfoMessage("因音频编码非AAC故无法复制音频流，音频将被重编码。");
                        aextract = audiobat(namevideo2, tempAudio);
                    }
                    break;
                default:
                    break;
            }

            #endregion

            #region Video
            if (x264ExeComboBox.SelectedItem.ToString().ToLower().Contains("x264"))
            {
                if (x264mode == 2)
                    x264 = x264bat(namevideo2, tempVideo, 1, namesub2) + "\r\n" +
                           x264bat(namevideo2, tempVideo, 2, namesub2);
                else x264 = x264bat(namevideo2, tempVideo, 0, namesub2);
                if (audioMode == 1)
                    x264 = x264.Replace(tempVideo, nameout2);
            }
            else if (x264ExeComboBox.SelectedItem.ToString().ToLower().Contains("x265"))
            {
                tempVideo = Util.ChangeExt(namevideo2, "_vtemp.hevc");
                if (ext != ".mp4")
                {
                    ShowErrorMessage("不支持的格式输出,x265当前工具箱仅支持MP4输出");
                    return;
                }
                if (x264mode == 2)
                    x264 = x265bat(namevideo2, tempVideo, 1) + "\r\n" +
                           x265bat(namevideo2, tempVideo, 2);
                else x264 = x265bat(namevideo2, tempVideo, 0);
                if (audioMode == 1)
                {
                    x264 += "\r\n\"" + workPath + "\\mp4box.exe\" -add  \"" + tempVideo + "#trackID=1:name=\" -new \"" + Util.ChangeExt(nameout2, ".mp4") + "\" \r\n";
                    x264 += "del \"" + tempVideo + "\"";
                }
            }
            x264 += "\r\n";

            #endregion

            #region Mux

            //封装
            if (audioMode != 1)
            {
                if (ext == ".mp4")
                    mux = boxmuxbat(tempVideo, tempAudio, Util.ChangeExt(nameout2, ext));
                else
                    mux = ffmuxbat(tempVideo, tempAudio, Util.ChangeExt(nameout2, ext));
                x264 = aextract + x264 + mux + "\r\n"
                    + "del \"" + tempVideo + "\"\r\n"
                    + "del \"" + tempAudio + "\"\r\n";
            }
            x264 += "\r\necho ===== one file is completed! =====\r\n";

            #endregion

            LogRecord(x264);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(GetCultureName());
            WorkingForm wf = new WorkingForm(x264);
            wf.Owner = this;
            wf.Show();
            //x264 += "\r\ncmd";
            //batpath = workPath + "\\x264.bat";
            //File.WriteAllText(batpath, x264, Encoding.Default);
            //Process.Start(batpath);
        }

        private void x264AddPresetBtn_Click(object sender, EventArgs e)
        {
            //create directory
            string preset = workPath + "\\preset";
            if (!Directory.Exists(preset)) Directory.CreateDirectory(preset);
            //add file
            batpath = workPath + "\\preset\\" + PresetNameTextBox.Text + ".txt";
            File.WriteAllText(batpath, x264CustomParameterTextBox.Text, Encoding.Default);
            //refresh combobox
            VideoPresetComboBox.Items.Clear();
            if (Directory.Exists(workPath + "\\preset"))
            {
                DirectoryInfo TheFolder = new DirectoryInfo(preset);
                foreach (FileInfo FileName in TheFolder.GetFiles())
                {
                    VideoPresetComboBox.Items.Add(FileName.Name.Replace(".txt", ""));
                }
            }
            VideoPresetComboBox.SelectedIndex = VideoPresetComboBox.FindString(PresetNameTextBox.Text);
        }

        private void x264DeletePresetBtn_Click(object sender, EventArgs e)
        {
            if (ShowQuestion("确定要删除这条预设参数？", "提示") == DialogResult.Yes)
            {
                string preset = workPath + "\\preset";
                batpath = workPath + "\\preset\\" + VideoPresetComboBox.Text + ".txt";
                File.Delete(batpath);
                VideoPresetComboBox.Items.Clear();
                if (Directory.Exists(preset))
                {
                    DirectoryInfo TheFolder = new DirectoryInfo(preset);
                    foreach (FileInfo FileName in TheFolder.GetFiles())
                    {
                        VideoPresetComboBox.Items.Add(FileName.Name.Replace(".txt", ""));
                    }
                }
                if (VideoPresetComboBox.Items.Count > 0) VideoPresetComboBox.SelectedIndex = 0;
            }
        }

        private void x264Mode2RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            x264mode = 2;
            lbrate.Visible = true;
            x264BitrateNum.Visible = true;
            label12.Visible = true;
            //x264FpsComboBox.Visible = true;
            //lbFPS2.Visible = true;
            lbwidth.Visible = true;
            lbheight.Visible = true;
            x264WidthNum.Visible = true;
            x264HeightNum.Visible = true;
            MaintainResolutionCheckBox.Visible = true;
            lbcrf.Visible = false;
            x264CRFNum.Visible = false;
            label4.Visible = false;
            x264CustomParameterTextBox.Visible = false;
            VideoPresetComboBox.Visible = false;
            x264AddPresetBtn.Visible = false;
            x264DeletePresetBtn.Visible = false;
            PresetNameTextBox.Visible = false;
        }

        private void x264Mode3RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            x264mode = 0;
            label4.Visible = true;
            x264CustomParameterTextBox.Visible = true;
            VideoPresetComboBox.Visible = true;
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
            //x264FpsComboBox.Visible = false;
            //lbFPS2.Visible = false;
        }

        private void x264Mode1RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            x264mode = 1;
            lbcrf.Visible = true;
            x264CRFNum.Visible = true;
            //x264FpsComboBox.Visible = true;
            //lbFPS2.Visible = true;
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
            VideoPresetComboBox.Visible = false;
            x264AddPresetBtn.Visible = false;
            x264DeletePresetBtn.Visible = false;
            PresetNameTextBox.Visible = false;
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
                x264OutTextBox.Text = Util.ChangeExt(namevideo2, "_x264.mp4");

                if (Path.GetExtension(namevideo2) != ".avs")
                {
                    string[] subExt = { ".ass", ".ssa", ".srt" };
                    foreach (string ext in subExt)
                    {
                        if (File.Exists(Util.ChangeExt(namevideo2, ext)))
                        {
                            x264SubTextBox.Text = Util.ChangeExt(namevideo2, ext);
                            break;
                        }
                    }
                }
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

        #endregion 视频页面

        #region 音频界面

        // <summary>
        /// 是否安装 Apple Application Support
        /// </summary>
        /// <returns>true:安装 false:没有安装</returns>
        private bool isAppleAppSupportInstalled()
        {
            Microsoft.Win32.RegistryKey uninstallNode_1 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Wow6432Node\Apple Inc.\Apple Application Support"); //x64 OS
            Microsoft.Win32.RegistryKey uninstallNode_2 = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"Software\Apple Inc.\Apple Application Support"); //x86 OS
            if (uninstallNode_1 != null || uninstallNode_2 != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AudioEncoderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (AudioEncoderComboBox.SelectedIndex)
            {
                case 0:
                    if (File.Exists(txtaudio2.Text))
                        AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_AAC.mp4");
                    AudioBitrateComboBox.Enabled = true;
                    AudioBitrateRadioButton.Enabled = true;
                    AudioCustomizeRadioButton.Enabled = true;
                    break;

                case 1:
                    if (!isAppleAppSupportInstalled())
                    {
                        if (ShowQuestion("Apple Application Support未安装.\r\n音频编码器QAAC可能无法使用.\r\n\r\n是否前往QuickTime下载页面?", "Apple Application Support未安装") == DialogResult.Yes)
                            Process.Start("http://www.apple.com/cn/quicktime/download");
                    }
                    if (File.Exists(txtaudio2.Text))
                        AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_AAC.m4a");
                    AudioBitrateComboBox.Enabled = true;
                    AudioBitrateRadioButton.Enabled = true;
                    AudioCustomizeRadioButton.Enabled = true;
                    break;

                case 2:
                    if (File.Exists(txtaudio2.Text))
                        AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_WAV.wav");
                    AudioBitrateComboBox.Enabled = false;
                    AudioBitrateRadioButton.Enabled = false;
                    AudioCustomizeRadioButton.Enabled = false;
                    break;

                case 3:
                    if (File.Exists(txtaudio2.Text))
                        AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_ALAC.m4a");
                    AudioBitrateComboBox.Enabled = false;
                    AudioBitrateRadioButton.Enabled = false;
                    AudioCustomizeRadioButton.Enabled = false;
                    break;

                case 4:
                    if (File.Exists(txtaudio2.Text))
                        AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_FLAC.flac");
                    AudioBitrateComboBox.Enabled = false;
                    AudioBitrateRadioButton.Enabled = false;
                    AudioCustomizeRadioButton.Enabled = false;
                    break;

                case 5:
                    if (File.Exists(txtaudio2.Text))
                        AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_AAC.m4a");
                    AudioBitrateComboBox.Enabled = true;
                    AudioBitrateRadioButton.Enabled = true;
                    AudioCustomizeRadioButton.Enabled = true;
                    break;

                case 6:
                    if (File.Exists(txtaudio2.Text))
                        AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_AC3.ac3");
                    AudioBitrateComboBox.Enabled = true;
                    AudioBitrateRadioButton.Enabled = true;
                    AudioCustomizeRadioButton.Enabled = false;
                    break;

                default:
                    break;
            }
        }

        private void AudioListBox_DragDrop(object sender, DragEventArgs e)
        {
            ListBox listbox = (ListBox)sender;
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String s in files)
                {
                    listbox.Items.Add(s);
                }
                return;
            }
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
            ListBox listbox = (ListBox)sender;
            if (e.Data.GetDataPresent(typeof(System.String)) && ((ListBox)sender).Equals(listbox))
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
                string finish, outputExt, codec;
                aac = "";
                switch (AudioEncoderComboBox.SelectedIndex)
                {
                    case 0: outputExt = "mp4"; codec = "AAC"; break;
                    case 1: outputExt = "m4a"; codec = "AAC"; break;
                    case 2: outputExt = "wav"; codec = "WAV"; break;
                    case 3: outputExt = "m4a"; codec = "ALAC"; break;
                    case 4: outputExt = "flac"; codec = "FLAC"; break;
                    case 5: outputExt = "m4a"; codec = "AAC"; break;
                    case 6: outputExt = "ac3"; codec = "AC3"; break;
                    default: outputExt = "aac"; codec = "AAC"; break;
                }
                for (int i = 0; i < this.AudioListBox.Items.Count; i++)
                {
                    string outname = "_" + codec + "." + outputExt;
                    finish = Util.ChangeExt(AudioListBox.Items[i].ToString(), outname);
                    aac += audiobat(AudioListBox.Items[i].ToString(), finish);
                    aac += "\r\n";
                }
                aac += "\r\ncmd";
                batpath = workPath + "\\aac.bat";
                File.WriteAllText(batpath, aac, Encoding.Default);
                LogRecord(aac);
                Process.Start(batpath);
            }
            else ShowErrorMessage("请输入文件！");
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
            savefile.Filter = "所有文件(*.*)|*.*";
            //savefile.Filter = "音频(*.aac;*.wav;*.m4a;*.flac)|*.aac*.wav;*.m4a;*.flac;";
            DialogResult result = savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                nameout3 = savefile.FileName + getAudioExt();
                AudioOutputTextBox.Text = nameout3;
            }
        }

        private void btnaac_Click(object sender, EventArgs e)
        {
            if (nameaudio2 == "")
            {
                ShowErrorMessage("请选择音频文件");
            }
            else if (nameout3 == "")
            {
                ShowErrorMessage("请选择输出文件");
            }
            else
            {
                batpath = workPath + "\\aac.bat";
                File.WriteAllText(batpath, audiobat(nameaudio2, nameout3), Encoding.Default);
                LogRecord(audiobat(nameaudio2, nameout3));
                Process.Start(batpath);
            }
        }

        private void txtaudio2_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(txtaudio2.Text.ToString()))
            {
                nameaudio2 = txtaudio2.Text;
                switch (AudioEncoderComboBox.SelectedIndex)
                {
                    case 0: AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_AAC.mp4"); break;
                    case 1: AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_AAC.m4a"); break;
                    case 2: AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_WAV.wav"); break;
                    case 3: AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_ALAC.m4a"); break;
                    case 4: AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_FLAC.flac"); break;
                    case 5: AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_AAC.m4a"); break;
                    case 6: AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_AC3.ac3"); break;
                    default: AudioOutputTextBox.Text = Util.ChangeExt(txtaudio2.Text, "_AAC.aac"); break;
                }
            }
        }

        private void txtout3_TextChanged(object sender, EventArgs e)
        {
            nameout3 = AudioOutputTextBox.Text;
        }

        private void txtaudio2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (File.Exists(txtaudio2.Text.ToString()))
            {
                Process.Start(txtaudio2.Text.ToString());
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            lbaacrate.Visible = false;
            lbaackbps.Visible = false;
            AudioBitrateComboBox.Visible = false;
            AudioCustomParameterTextBox.Visible = true;
            AudioPresetLabel.Visible = true;
            AudioPresetComboBox.Visible = true;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            lbaacrate.Visible = true;
            lbaackbps.Visible = true;
            AudioBitrateComboBox.Visible = true;
            AudioCustomParameterTextBox.Visible = false;
            AudioPresetLabel.Visible = false;
            AudioPresetComboBox.Visible = false;
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

        #endregion 音频界面

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
            string vsfilterDLLPath = Path.Combine(workPath, @"avsfilter\VSFilter.DLL");
            string LSMASHSourceDLLPath = Path.Combine(workPath, @"avsfilter\LSMASHSource.DLL");
            string undotDLLPath = Path.Combine(workPath, @"avsfilter\UnDot.DLL");
            avsBuilder.AppendLine("LoadPlugin(\"" + vsfilterDLLPath + "\")");
            avsBuilder.AppendLine("LoadPlugin(\"" + LSMASHSourceDLLPath + "\")");

            if (UndotCheckBox.Checked)
                avsBuilder.AppendLine("LoadPlugin(\"" + undotDLLPath + "\")");
            avsBuilder.AppendLine("LWLibavVideoSource(\"" + namevideo9 + "\")");
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

        #endregion 更改AVS

        #endregion AVS页面

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
                str = Util.ChangeExt(str, "_AVS.mp4");
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
            File.WriteAllText(startpath + "\\log.txt", log.ToString(), Encoding.Default);
            p.Close();//关闭进程
            reader.Close();//关闭流
        }

        public void LogRecord(string log)
        {
            Util.ensureDirectoryExists(logPath);
            File.AppendAllText(logFileName,
                "===========" + DateTime.Now.ToString() + "===========\r\n" + log + "\r\n\r\n", Encoding.Default);
        }

        private void DeleteLogButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(logPath))
            {
                Util.DeleteDirectoryIfExists(logPath, true);
                ShowInfoMessage("已经删除日志文件。");
            }
            else ShowInfoMessage("没有找到日志文件。");
        }

        private void ViewLogButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(logFileName))
            {
                Process.Start(logFileName);
            }
            else ShowInfoMessage("没有找到日志文件。");
        }

        private void x264PathButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
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
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            if (form != null)
            {
                ComponentResourceManager resources = new ComponentResourceManager(formType);
                resources.ApplyResources(form, "$this");
                AppLang(form, resources);
            }
        }

        private static void AppLang(Control control, ComponentResourceManager resources)
        {
            foreach (Control c in control.Controls)
            {
                resources.ApplyResources(c, c.Name);
                AppLang(c, resources);
            }
        }

        private void languageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //StreamReader sr;
            x264Mode1RadioButton.Checked = true;
            AudioBitrateRadioButton.Checked = true;
            int x264AudioModeComboBoxIndex = 0;
            switch (languageComboBox.SelectedIndex)
            {
                case 0:
                    SetLang("zh-CN", this, typeof(MainForm));
                    this.Text = string.Format("小丸工具箱 {0}", Assembly.GetExecutingAssembly().GetName().Version.Build);
                    x264PriorityComboBox.Items.Clear();
                    x264PriorityComboBox.Items.AddRange(new string[] { "低", "低于标准", "普通", "高于标准", "高", "实时" });
                    x264PriorityComboBox.SelectedIndex = 2;
                    x264AudioModeComboBoxIndex = x264AudioModeComboBox.SelectedIndex;
                    x264AudioModeComboBox.Items.Clear();
                    x264AudioModeComboBox.Items.Add("压制音频");
                    x264AudioModeComboBox.Items.Add("无音频流");
                    x264AudioModeComboBox.Items.Add("复制音频流");
                    x264AudioModeComboBox.SelectedIndex = x264AudioModeComboBoxIndex;
                    x264VideoTextBox.EmptyTextTip = "可以把文件拖拽到这里";
                    x264SubTextBox.EmptyTextTip = "双击清空字幕文件文本框";
                    //x264OutTextBox.EmptyTextTip = "宽度和高度全为0即不改变分辨率";
                    x264PathTextBox.EmptyTextTip = "字幕文件和视频文件在同一目录下且同名，不同名仅有语言后缀时请在右方选择或输入";
                    //txtvideo3.EmptyTextTip = "音频参数在音频选项卡设定";
                    ExtractMP4TextBox.EmptyTextTip = "抽取的视频或音频在原视频目录下";
                    txtvideo8.EmptyTextTip = "抽取的视频或音频在原视频目录下";
                    txtvideo6.EmptyTextTip = "抽取的视频或音频在原视频目录下";
                    //load Help Text
                    if (File.Exists(startpath + "\\help.rtf"))
                    {
                        HelpTextBox.LoadFile(startpath + "\\help.rtf");
                    }
                    break;

                case 1:
                    SetLang("zh-TW", this, typeof(MainForm));
                    this.Text = string.Format("小丸工具箱 {0}", Assembly.GetExecutingAssembly().GetName().Version.Build);
                    x264PriorityComboBox.Items.Clear();
                    x264PriorityComboBox.Items.AddRange(new string[] { "低", "在標準以下", "標準", "在標準以上", "高", "即時" });
                    x264PriorityComboBox.SelectedIndex = 2;
                    x264AudioModeComboBoxIndex = x264AudioModeComboBox.SelectedIndex;
                    x264AudioModeComboBox.Items.Clear();
                    x264AudioModeComboBox.Items.Add("壓制音頻");
                    x264AudioModeComboBox.Items.Add("無音頻流");
                    x264AudioModeComboBox.Items.Add("拷貝音頻流");
                    x264AudioModeComboBox.SelectedIndex = x264AudioModeComboBoxIndex;
                    x264VideoTextBox.EmptyTextTip = "可以把文件拖拽到這裡";
                    x264SubTextBox.EmptyTextTip = "雙擊清空字幕檔案文本框";
                    //x264OutTextBox.EmptyTextTip = "寬度和高度全為0即不改變解析度";
                    x264PathTextBox.EmptyTextTip = "字幕和視頻在同一資料夾下且同名，不同名僅有語言後綴時請在右方選擇或輸入";
                    //txtvideo3.EmptyTextTip = "音頻參數需在音頻選項卡设定";
                    ExtractMP4TextBox.EmptyTextTip = "新檔案生成在原資料夾";
                    txtvideo8.EmptyTextTip = "新檔案生成在原資料夾";
                    txtvideo6.EmptyTextTip = "新檔案生成在原資料夾";
                    //load Help Text
                    if (File.Exists(startpath + "\\help_zh_tw.rtf"))
                    {
                        HelpTextBox.LoadFile(startpath + "\\help_zh_tw.rtf");
                    }
                    break;

                case 2:
                    SetLang("en-US", this, typeof(MainForm));
                    this.Text = string.Format("Maruko Toolbox {0}", Assembly.GetExecutingAssembly().GetName().Version.Build);
                    x264PriorityComboBox.Items.Clear();
                    x264PriorityComboBox.Items.AddRange(new string[] { "Idle", "BelowNormal", "Normal", "AboveNormal", "High", "RealTime" });
                    x264PriorityComboBox.SelectedIndex = 2;
                    x264AudioModeComboBoxIndex = x264AudioModeComboBox.SelectedIndex;
                    x264AudioModeComboBox.Items.Clear();
                    x264AudioModeComboBox.Items.Add("with audio");
                    x264AudioModeComboBox.Items.Add("no audio");
                    x264AudioModeComboBox.Items.Add("copy audio");
                    x264AudioModeComboBox.SelectedIndex = x264AudioModeComboBoxIndex;
                    x264VideoTextBox.EmptyTextTip = "Drag file here";
                    x264SubTextBox.EmptyTextTip = "Clear subtitle text box by double click";
                    //x264OutTextBox.EmptyTextTip = "Both the width and height equal zero means using original resolution";
                    x264PathTextBox.EmptyTextTip = "Subtitle and Video must be of the same name and in the same folder";
                    //txtvideo3.EmptyTextTip = "It is necessary to set audio parameter in the Audio tab";
                    ExtractMP4TextBox.EmptyTextTip = "New file will be created in the original folder";
                    txtvideo8.EmptyTextTip = "New file will be created in the original folder";
                    txtvideo6.EmptyTextTip = "New file will be created in the original folder";
                    //load Help Text
                    if (File.Exists(startpath + "\\help.rtf"))
                    {
                        HelpTextBox.LoadFile(startpath + "\\help.rtf");
                    }
                    break;

                case 3:
                    SetLang("ja-JP", this, typeof(MainForm));
                    this.Text = string.Format("Maruko Toolbox {0}", Assembly.GetExecutingAssembly().GetName().Version.Build);
                    x264PriorityComboBox.Items.Clear();
                    x264PriorityComboBox.Items.AddRange(new string[] { "低", "通常以下", "通常", "通常以上", "高", "リアルタイム" });
                    x264PriorityComboBox.SelectedIndex = 2;
                    x264AudioModeComboBoxIndex = x264AudioModeComboBox.SelectedIndex;
                    x264AudioModeComboBox.Items.Clear();
                    x264AudioModeComboBox.Items.Add("オーディオ付き");
                    x264AudioModeComboBox.Items.Add("オーディオなし");
                    x264AudioModeComboBox.Items.Add("オーディオ コピー");
                    x264AudioModeComboBox.SelectedIndex = x264AudioModeComboBoxIndex;
                    x264VideoTextBox.EmptyTextTip = "ビデオファイルをここに引きずってください";
                    x264SubTextBox.EmptyTextTip = "ダブルクリックで字幕を削除する";
                    //x264OutTextBox.EmptyTextTip = "Both the width and height equal zero means using original resolution";
                    x264PathTextBox.EmptyTextTip = "字幕とビデオは同じ名前と同じフォルダにある必要があります";
                    //txtvideo3.EmptyTextTip = "It is necessary to set audio parameter in the Audio tab";
                    ExtractMP4TextBox.EmptyTextTip = "新しいファイルはビデオファイルのあるディレクトリに生成する";
                    txtvideo8.EmptyTextTip = "新しいファイルはビデオファイルのあるディレクトリに生成する";
                    txtvideo6.EmptyTextTip = "新しいファイルはビデオファイルのあるディレクトリに生成する";
                    if (File.Exists(startpath + "\\help.rtf"))
                    {
                        HelpTextBox.LoadFile(startpath + "\\help.rtf");
                    }
                    break;

                default:
                    break;
            }
        }

        private string GetCultureName()
        {
            string name = "zh-CN";
            switch (languageComboBox.SelectedIndex)
            {
                case 0:
                    name = "zh-CN";
                    break;

                case 1:
                    name = "zh-TW";
                    break;

                case 2:
                    name = "en-US";
                    break;

                case 3:
                    name = "ja-JP";
                    break;

                default:
                    break;
            }
            return name;
        }

        #endregion globalization

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

        private void AVSSaveButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "AVS(*.avs)|*.avs";
            DialogResult result = savefile.ShowDialog();
            if (result == DialogResult.OK)
            {
                File.WriteAllText(savefile.FileName, AVSScriptTextBox.Text, Encoding.Default);
            }
        }

        private void MuxReplaceAudioButton_Click(object sender, EventArgs e)
        {
            if (namevideo == "")
            {
                ShowErrorMessage("请选择视频文件");
                return;
            }
            if (nameaudio == "")
            {
                ShowErrorMessage("请选择音频文件");
                return;
            }
            if (nameout == "")
            {
                ShowErrorMessage("请选择输出文件");
                return;
            }
            mux = "";
            //mux = "\"" + workPath + "\\ffmpeg.exe\" -y -i \"" + namevideo + "\" -c:v copy -an  \"" + workPath + "\\video_noaudio.mp4\" \r\n";
            //mux += "\"" + workPath + "\\ffmpeg.exe\" -y -i \"" + workPath + "\\video_noaudio.mp4\" -i \"" + nameaudio + "\" -vcodec copy  -acodec copy \"" + nameout + "\" \r\n";
            //mux += "del \"" + workPath + "\\video_noaudio.mp4\" \r\n";
            mux = "\"" + workPath + "\\ffmpeg.exe\" -y -i \"" + namevideo + "\" -i \"" + nameaudio + "\" -map 0:v -c:v copy -map 1:0 -c:a copy  \"" + txtout.Text + "\" \r\n";
            batpath = workPath + "\\mux.bat";
            File.WriteAllText(batpath, mux, Encoding.Default);
            LogRecord(mux);
            Process.Start(batpath);
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
                ShowErrorMessage("请选择图片文件");
            }
            else if (!File.Exists(AudioPicAudioTextBox.Text))
            {
                ShowErrorMessage("请选择音频文件");
            }
            else if (AudioOnePicOutputTextBox.Text == "")
            {
                ShowErrorMessage("请选择输出文件");
            }
            else
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(AudioPicTextBox.Text);
                // if not even number, chop 1 pixel out
                int newWidth = (img.Width % 2 == 0 ? img.Width : img.Width - 1);
                int newHeight = (img.Height % 2 == 0 ? img.Height : img.Height - 1);
                Rectangle cropArea;
                if (img.Width % 2 != 0 || img.Height % 2 != 0)
                {
                    Bitmap bmp = new Bitmap(img);
                    cropArea = new Rectangle(0, 0, newWidth, newHeight);
                    img = (Image)bmp.Clone(cropArea, bmp.PixelFormat);
                }

                //if (img.Width % 2 != 0 || img.Height % 2 != 0)
                //{
                //    MessageBox.Show("图片的长和宽必须是偶数。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    img.Dispose();
                //    return;
                //}
                //if (img.RawFormat.Equals(ImageFormat.Jpeg))
                //{
                //    File.Copy(AudioPicTextBox.Text, tempPic, true);
                //}
                //else
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
                string ffPath = Path.Combine(workPath, "ffmpeg.exe");
                string neroPath = Path.Combine(workPath, "neroaacenc.exe");
                if (AudioCopyCheckBox.Checked)
                {
                    mux = "\"" + ffPath + "\" -loop 1 -r " + OnePicFPSNum.Value.ToString() + " -t " + seconds.ToString() + " -f image2 -i \"" + tempPic + "\" -c:v libx264 -crf " + OnePicCRFNum.Value.ToString() + " -y SinglePictureVideo.mp4\r\n";
                    mux += "\"" + ffPath + "\" -i SinglePictureVideo.mp4 -i \"" + AudioPicAudioTextBox.Text + "\" -c:v copy -c:a copy -y \"" + AudioOnePicOutputTextBox.Text + "\"\r\n";
                    mux += "del SinglePictureVideo.mp4\r\n";
                    mux += "cmd";
                }
                else
                {
                    mux = "\"" + ffPath + "\" -i \"" + AudioPicAudioTextBox.Text + "\" -f wav - |" + neroPath + " -br " + OnePicAudioBitrateNum.Value.ToString() + "000 -ignorelength -if - -of audio.mp4 -lc\r\n";
                    mux += "\"" + ffPath + "\" -loop 1 -r " + OnePicFPSNum.Value.ToString() + " -t " + seconds.ToString() + " -f image2 -i \"" + tempPic + "\" -c:v libx264 -crf " + OnePicCRFNum.Value.ToString() + " -y SinglePictureVideo.mp4\r\n";
                    mux += "\"" + ffPath + "\" -i SinglePictureVideo.mp4 -i audio.mp4 -c:v copy -c:a copy -y \"" + AudioOnePicOutputTextBox.Text + "\"\r\n";
                    mux += "del SinglePictureVideo.mp4\r\ndel audio.mp4\r\n";
                    mux += "cmd";
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
                batpath = Path.Combine(workPath, Path.GetRandomFileName() + ".bat");
                File.WriteAllText(batpath, mux, Encoding.Default);
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
                AudioOnePicOutputTextBox.Text = Util.ChangeExt(AudioPicAudioTextBox.Text, "_SP.flv");
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

        #endregion 一图流

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
            //验证
            if (!File.Exists(BlackVideoTextBox.Text) || Path.GetExtension(BlackVideoTextBox.Text) != ".flv")
            {
                ShowErrorMessage("请选择FLV视频文件");
                return;
            }

            MediaInfo MI = new MediaInfo();
            MI.Open(BlackVideoTextBox.Text);
            double videobitrate = double.Parse(MI.Get(StreamKind.General, 0, "BitRate"));
            double targetBitrate = (double)BlackBitrateNum.Value;

            if (!File.Exists(BlackPicTextBox.Text) && BlackNoPicCheckBox.Checked == false)
            {
                ShowErrorMessage("请选择图片文件或勾选使用黑屏");
                return;
            }
            if (BlackOutputTextBox.Text == "")
            {
                ShowErrorMessage("请选择输出文件");
                return;
            }

            if (videobitrate < 1000000)
            {
                ShowInfoMessage("此视频不需要后黑。");
                return;
            }
            if (videobitrate > 5000000)
            {
                ShowInfoMessage("此视频码率过大，请先压制再后黑。");
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
                    ShowErrorMessage("图片的长和宽必须都是偶数。");
                    img.Dispose();
                    return;
                }
                if (img.Width != videoWidth || img.Height != videoHeight)
                {
                    ShowErrorMessage("图片的长和宽和视频不一致。");
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
            mux = "\"" + workPath + "\\ffmpeg\" -loop 1 -r " + BlackFPSNum.Value.ToString() + " -t " + blackSecond.ToString() + " -f image2 -i \"" + tempPic + "\" -c:v libx264 -crf " + BlackCRFNum.Value.ToString() + " -y black.flv\r\n";
            mux += string.Format("\"{0}\\flvbind\" \"{1}\"  \"{2}\"  black.flv\r\n", workPath, BlackOutputTextBox.Text, BlackVideoTextBox.Text);
            mux += "del black.flv\r\n";

            batpath = Path.Combine(workPath, Path.GetRandomFileName() + ".bat");
            File.WriteAllText(batpath, mux, Encoding.Default);
            LogRecord(mux);
            Process.Start(batpath);
        }

        private void BlackVideoTextBox_TextChanged(object sender, EventArgs e)
        {
            string path = BlackVideoTextBox.Text;
            if (File.Exists(path))
            {
                BlackOutputTextBox.Text = Util.ChangeExt(path, "_black.flv");
            }
        }

        #endregion 后黑

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
            DialogResult dr = ShowQuestion(string.Format("是否将所有界面参数恢复到默认设置？"), "提示");
            if (dr == DialogResult.Yes)
            {
                InitParameter();
                ShowInfoMessage("恢复默认设置完成！");
            }
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

        #region CheckUpdate

        public delegate bool CheckUpadateDelegate(out DateTime newdate, out bool isFullUpdate);

        public void CheckUpdateCallBack(IAsyncResult ar)
        {
            DateTime NewDate;
            bool isFullUpdate;
            AsyncResult result = (AsyncResult)ar;
            CheckUpadateDelegate func = (CheckUpadateDelegate)result.AsyncDelegate;

            try
            {
                bool needUpdate = func.EndInvoke(out NewDate, out isFullUpdate, ar);
                if (needUpdate)
                {
                    if (isFullUpdate)
                    {
                        DialogResult dr = ShowQuestion(string.Format("新版已于{0}发布，是否前往官网下载？", NewDate.ToString("yyyy-M-d")), "喜大普奔");
                        if (dr == DialogResult.Yes)
                        {
                            Process.Start("http://www.maruko.in/");
                        }
                    }
                    else
                    {
                        DialogResult dr = ShowQuestion(string.Format("新版已于{0}发布，是否自动升级？（文件约1.5MB）", NewDate.ToString("yyyy-M-d")), "喜大普奔");
                        if (dr == DialogResult.Yes)
                        {
                            FormUpdater formUpdater = new FormUpdater(startpath, NewDate.ToString());
                            formUpdater.ShowDialog(this);
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("已经是最新版了喵！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception) { }
        }

        public bool CheckUpdate(out DateTime NewDate, out bool isFullUpdate)
        {
            WebRequest request = WebRequest.Create("http://mtbftest.sinaapp.com/version.php");
            WebResponse wrs = request.GetResponse();
            // read the response ...
            Stream dataStream = wrs.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            Regex dateReg = new Regex(@"Date20\S+Date");
            Regex VersionReg = new Regex(@"Version\d+Version");
            Match dateMatch = dateReg.Match(responseFromServer);
            Match versionMatch = VersionReg.Match(responseFromServer);
            NewDate = DateTime.Parse("1990-03-08");
            isFullUpdate = false;
            if (dateMatch.Success)
            {
                string date = dateMatch.Value.Replace("Date", "");
                string version = versionMatch.Value.Replace("Version", "");
                NewDate = DateTime.Parse(date);
                int NewVersion = int.Parse(version);
                int s = DateTime.Compare(NewDate, ReleaseDate);

                int currentVersion = Assembly.GetExecutingAssembly().GetName().Version.Minor;
                if (NewVersion > currentVersion)
                {
                    isFullUpdate = true;
                }
                else
                {
                    isFullUpdate = false;
                }
                //DateTime CompileDate = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location); //获得程序编译时间
                if (s == 1) //NewDate is later than ReleaseDate
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private void CheckUpdateButton_Click(object sender, EventArgs e)
        {
            if (Util.IsConnectInternet())
            {
                WebRequest request = WebRequest.Create("http://mtbftest.sinaapp.com/version.php");
                request.Credentials = CredentialCache.DefaultCredentials;
                // Get the response.
                request.BeginGetResponse(new AsyncCallback(OnResponse), request);
            }
            else
            {
                ShowErrorMessage("这台电脑似乎没有联网呢~");
            }
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
            Regex dateReg = new Regex(@"Date20\S+Date");
            Regex VersionReg = new Regex(@"Version\d+Version");
            Match dateMatch = dateReg.Match(responseFromServer);
            Match versionMatch = VersionReg.Match(responseFromServer);
            DateTime NewDate = DateTime.Parse("1990-03-08");
            bool isFullUpdate = false;
            if (dateMatch.Success)
            {
                string date = dateMatch.Value.Replace("Date", "");
                string version = versionMatch.Value.Replace("Version", "");
                NewDate = DateTime.Parse(date);
                int NewVersion = int.Parse(version);
                int s = DateTime.Compare(NewDate, ReleaseDate);

                int currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
                if (NewVersion > currentVersion)
                {
                    isFullUpdate = true;
                }
                else
                {
                    isFullUpdate = false;
                }
                //DateTime CompileDate = System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location); //获得程序编译时间
                if (s == 1) //NewDate is later than ReleaseDate
                {
                    if (isFullUpdate)
                    {
                        DialogResult dr = ShowQuestion(string.Format("新版已于{0}发布，是否前往官网下载？", NewDate.ToString("yyyy-M-d")), "喜大普奔");
                        if (dr == DialogResult.Yes)
                        {
                            Process.Start("http://www.maruko.in/");
                        }
                    }
                    else
                    {
                        DialogResult dr = ShowQuestion(string.Format("新版已于{0}发布，是否自动升级？（文件约1.5MB）", NewDate.ToString("yyyy-M-d")), "喜大普奔");
                        if (dr == DialogResult.Yes)
                        {
                            FormUpdater formUpdater = new FormUpdater(startpath, date);
                            formUpdater.ShowDialog(this);
                        }
                    }
                }
                else
                {
                    ShowInfoMessage("已经是最新版了喵！");
                }
            }
            else
            {
                ShowInfoMessage("啊咧~似乎未能获取版本信息，请点击软件主页按钮查看最新版本。");
            }
        }

        #endregion CheckUpdate

        private void x264ShutdownCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            shutdownState = x264ShutdownCheckBox.Checked;
        }

        private void TrayModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            trayMode = TrayModeCheckBox.Checked;
        }

        private void ReleaseDatelabel_DoubleClick(object sender, EventArgs e)
        {
            SplashForm sf = new SplashForm();
            sf.Owner = this;
            sf.Show();
        }

        private void AudioCopyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            OnePicAudioBitrateNum.Enabled = !AudioCopyCheckBox.Checked;
        }

        private void HelpTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void labelAudio_Click(object sender, EventArgs e)
        {
            tabControl.SelectedIndex = 1;
        }

        private void SetupAVSPlayerButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "程序(*.exe)|*.exe|所有文件(*.*)|*.*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                SetupPlayerTextBox.Text = openFileDialog1.FileName;
            }
        }

        private void AVSAddFilterButton_Click(object sender, EventArgs e)
        {
            string vsfilterDLLPath = Path.Combine(workPath, @"avsfilter\" + AVSFilterComboBox.Text);
            string text = "LoadPlugin(\"" + vsfilterDLLPath + "\")" + "\r\n";
            AVSScriptTextBox.Text = text + AVSScriptTextBox.Text;
        }

        private void AudioJoinButton_Click(object sender, EventArgs e)
        {
            if (AudioListBox.Items.Count == 0)
            {
                ShowErrorMessage("请输入文件！");
                return;
            }
            else if (AudioOutputTextBox.Text == "")
            {
                ShowErrorMessage("请选择输出文件");
                return;
            }
            StringBuilder sb = new StringBuilder();
            ffmpeg = "";
            string ext = Path.GetExtension(AudioListBox.Items[0].ToString());
            string finish = Util.ChangeExt(AudioOutputTextBox.Text, ext);
            for (int i = 0; i < this.AudioListBox.Items.Count; i++)
            {
                if (Path.GetExtension(AudioListBox.Items[i].ToString()) != ext)
                {
                    ShowErrorMessage("只允许合并相同格式文件。");
                    return;
                }
                sb.AppendLine("file '" + AudioListBox.Items[i].ToString() + "'");
                File.WriteAllText("concat.txt", sb.ToString());
                ffmpeg = "\"" + workPath + "\\ffmpeg.exe\" -f concat  -i concat.txt -y -c copy " + finish;
            }
            ffmpeg += "\r\ncmd";
            batpath = workPath + "\\concat.bat";
            File.WriteAllText(batpath, ffmpeg, Encoding.Default);
            LogRecord(aac);
            Process.Start(batpath);
        }

        #region TabControl

        private void tabControl_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
                Point pt = new Point(e.X + 2, e.Y + 2);
                pt = PointToClient(pt);
                int pi = GetTabPageByTab(pt);
                if (pi != -1)
                {
                    tabControl.SelectedIndex = pi;
                }
            }
            else e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Finds the TabPage whose tab is contains the given point.
        /// </summary>
        /// <param name="pt">The point (given in client coordinates) to look for a TabPage.</param>
        /// <returns>The TabPage whose tab is at the given point (null if there isn't one).</returns>
        private int GetTabPageByTab(Point pt)
        {
            TabPage tp = null;
            int pageIndex = -1;
            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                Rectangle a = tabControl.GetTabRect(i);

                if (tabControl.GetTabRect(i).Contains(pt))
                {
                    tp = tabControl.TabPages[i];
                    pageIndex = i;
                    break;
                }
            }
            return pageIndex;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D1)
            {
                tabControl.SelectedIndex = 0;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D2)
            {
                tabControl.SelectedIndex = 1;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D3)
            {
                tabControl.SelectedIndex = 2;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D4)
            {
                tabControl.SelectedIndex = 3;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D5)
            {
                tabControl.SelectedIndex = 4;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D6)
            {
                tabControl.SelectedIndex = 5;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D7)
            {
                tabControl.SelectedIndex = 6;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D8)
            {
                tabControl.SelectedIndex = 7;
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D9)
            {
                tabControl.SelectedIndex = 8;
            }
        }

        #endregion TabControl

        private void gmkvextractguibButton_Click(object sender, EventArgs e)
        {
            string path = workPath + "\\gMKVExtractGUI.exe";
            if (File.Exists(path))
                Process.Start(path);
            else
                ShowErrorMessage("请检查\r\n\r\n" + path + "\r\n\r\n是否存在", "未找到程序!");
        }

        private void FeedbackButton_Click(object sender, EventArgs e)
        {
            FeedbackForm ff = new FeedbackForm();
            ff.ShowDialog();
        }

        private void x264SubTextBox_DoubleClick(object sender, EventArgs e)
        {
            x264SubTextBox.Clear();
        }

        private void RotateButton_Click(object sender, EventArgs e)
        {
            if (namevideo4 == "")
            {
                ShowErrorMessage("请选择视频文件");
            }
            else if (nameout5 == "")
            {
                ShowErrorMessage("请选择输出文件");
            }
            else
            {
                clip = string.Format(@"""{0}\ffmpeg.exe"" -i ""{1}"" -vf ""transpose={2}"" -y ""{3}""",
                    workPath, namevideo4, TransposeComboBox.SelectedIndex, nameout5) + Environment.NewLine + "cmd";
                batpath = workPath + "\\clip.bat";
                File.WriteAllText(batpath, clip, Encoding.Default);
                Process.Start(batpath);
            }
        }

        private void AudioPresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            XElement x = xdoc.Element("root").Element("Audio").Elements().Where(_ => _.Attribute("Name").Value == AudioPresetComboBox.Text).First();
            AudioCustomParameterTextBox.Text = x.Value;
        }

        private void lbAuto_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= 65535)
                return;

            e.DrawBackground();
            SolidBrush BlueBrush = new SolidBrush(Color.Blue);
            SolidBrush BlackBrush = new SolidBrush(Color.Black);
            Color vColor = Color.Black;
            string input = lbAuto.Items[e.Index].ToString();
            if (!string.IsNullOrEmpty(GetSubtitlePath(input)))
            {
                e.Graphics.DrawString(Convert.ToString(lbAuto.Items[e.Index]), e.Font, BlueBrush, e.Bounds);
            }
            else
            {
                e.Graphics.DrawString(Convert.ToString(lbAuto.Items[e.Index]), e.Font, BlackBrush, e.Bounds);
            }
        }

        private string GetSubtitlePath(string videoPath)
        {
            string sub = "";
            string splang = "";
            string[] subExt = { ".ass", ".ssa", ".srt" };
            if (x264BatchSubSpecialLanguage.Text != "none")
                splang = "." + x264BatchSubSpecialLanguage.Text;
            foreach (string ext in subExt)
            {
                if (File.Exists(videoPath.Remove(videoPath.LastIndexOf(".")) + splang + ext))
                {
                    sub = videoPath.Remove(videoPath.LastIndexOf(".")) + splang + ext;
                    break;
                }
            }
            return sub;
        }

        private void x264ExeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (x264ExeComboBox.SelectedIndex == -1)
                return;

            if (x264ExeComboBox.SelectedItem.ToString().ToLower().Contains("x265"))
            {
                x264SubTextBox.Enabled = false;
                x264SubBtn.Enabled = false;
                x264BatchSubCheckBox.Enabled = false;
                x264BatchSubSpecialLanguage.Enabled = false;
                x264DemuxerComboBox.Enabled = false;
                VideoBatchFormatComboBox.Text = "mp4";
                VideoBatchFormatComboBox.Enabled = false;
            }
            else
            {
                x264SubTextBox.Enabled = true;
                x264SubBtn.Enabled = true;
                x264BatchSubCheckBox.Enabled = true;
                x264BatchSubSpecialLanguage.Enabled = true;
                x264DemuxerComboBox.Enabled = true;
                VideoBatchFormatComboBox.Enabled = true;
            }
        }

        #region Form

        protected String GetCurrentDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        protected void ShowErrorMessage(String argMessage)
        {
            MessageBox.Show(argMessage, "错误!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        protected void ShowErrorMessage(String argMessage, String argTitle)
        {
            MessageBox.Show(argMessage, argTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        protected void ShowWarningMessage(String argMessage)
        {
            MessageBox.Show(argMessage, "警告!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        protected void ShowWarningMessage(String argMessage, String argTitle)
        {
            MessageBox.Show(argMessage, argTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        protected void ShowInfoMessage(String argMessage)
        {
            MessageBox.Show(argMessage, "提示!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected void ShowInfoMessage(String argMessage, String argTitle)
        {
            MessageBox.Show(argMessage, argTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected DialogResult ShowQuestion(String argQuestion, String argTitle)
        {
            return MessageBox.Show(argQuestion, argTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        #endregion
    }
}