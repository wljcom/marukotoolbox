// ------------------------------------------------------------------
// Copyright (C) 2011-2015 Maruko Toolbox Project
// 
//  Authors: LunarShaddow <aflyhorse@hotmail.com>
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;

namespace mp4box
{
    namespace Extension
    {
        /// <summary>
        /// A wrapper to make Invoke more easy by using Method Extension.
        /// </summary>
        static class Invoker
        {
            public static void InvokeIfRequired(this ISynchronizeInvoke control, MethodInvoker action)
            {
                if (control.InvokeRequired)
                    control.Invoke(action, null);
                else
                    action();
            }
        }
    }

    public static class FormatExtractor
    {
        /// <summary>
        /// Regex pattern. ffmpeg -i sample output:
        /// <para>Stream #0:0: Audio: mp3, 44100 Hz, stereo, s16p, 320 kb/s (default)</para>
        /// <para>Stream #0:1: Audio: mp3, 44100 Hz, stereo, s16p, 320 kb/s</para>
        /// <para>Stream #0:2: Video: h264 (High), yuv420p, 1280x960, SAR 1:1 DAR 4:3, 24 fps, 24 tbr, 1k tbn, 48 tbc (default)</para>
        /// <para>Stream #0:3: Subtitle: ass (default)</para>
        /// </summary>
        private static readonly Regex ffmpegReg
            = new Regex(@"Stream #0:\d+: (?<type>.+): (?<metadata>.+)");

        public enum StreamType
        {
            Video, Audio, Subtitle, Unknown
        }

        public class StreamProperty
        {
            public StreamType Type;
            public string Format;
            public string RawFormat;

            /// <summary>
            /// Helper method to turn string into StreamType Enum
            /// </summary>
            /// <param name="strType">StreamType in string</param>
            /// <returns>StreamType Enum</returns>
            public static StreamType Str2Type(string strType)
            {
                try
                {
                    return (StreamType)Enum.Parse(typeof(StreamType), strType);
                }
                catch
                {
                    return StreamType.Unknown;
                }
            }

            /// <summary>
            /// deliminators of raw format string
            /// </summary>
            private static readonly char[] delim = { ' ', ',' };

            public StreamProperty(string strType, string rawFormat)
                : this(Str2Type(strType), rawFormat) { }

            public StreamProperty(StreamType type, string rawFormat)
            {
                Type = type;
                RawFormat = rawFormat;
                Format = rawFormat.Split(delim)[0];
            }
        }

        /// <summary>
        /// Extract stream infomation from a media file
        /// </summary>
        /// <param name="workPath">work path which contains ffmpeg</param>
        /// <param name="filename">target media file</param>
        /// <returns>a list of StreamProperty containing stream type and format.</returns>
        public static List<StreamProperty> Extract(string workPath, string filename)
        {
            var output = GetFFmpegOutput(workPath, filename);
            var list = new List<StreamProperty>(5);
            var match = ffmpegReg.Match(output);
            while (match.Success)
            {
                list.Add(new StreamProperty(
                    match.Groups["type"].Value, match.Groups["metadata"].Value));
                match = match.NextMatch();
            }
            return list;
        }

        /// <summary>
        /// ffmpeg output wrapper
        /// </summary>
        /// <param name="workPath">work path which contains ffmpeg</param>
        /// <param name="filename">target media file</param>
        /// <returns>ffmpeg output info</returns>
        private static string GetFFmpegOutput(string workPath, string filename)
        {
            var processInfo = new System.Diagnostics.ProcessStartInfo(
                System.IO.Path.Combine(workPath, "ffmpeg.exe"), "-i " + Cmd.FormatPath(filename));
            processInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            var proc = System.Diagnostics.Process.Start(processInfo);
            string output = proc.StandardError.ReadToEnd();
            proc.WaitForExit();
            return output;
        }
    }
}
