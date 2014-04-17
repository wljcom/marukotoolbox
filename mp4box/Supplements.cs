using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Text;

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// Method Extension .NET 2.0 walkaround
    /// http://msdn.microsoft.com/en-us/library/system.runtime.compilerservices.extensionattribute(v=vs.110).aspx
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Assembly|AttributeTargets.Class|AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}

namespace mp4box
{
    namespace Extentions
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

        /// <summary>
        /// Handmade progress bar. Currently not in use.
        /// </summary>
        class ProgressBarEx : ProgressBar
        {
            public ProgressBarEx()
            {
                // Modify the ControlStyles flags
                //http://msdn.microsoft.com/en-us/library/system.windows.forms.controlstyles.aspx
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
                // Initialize Property
                CustomText = "";
                Format = new StringFormat(StringFormatFlags.NoWrap);
                Format.Alignment = StringAlignment.Center;
                Format.LineAlignment = StringAlignment.Center;
            }

            public String CustomText { get; set; }

            private StringFormat Format { get; set; }

            protected override void OnPaint(PaintEventArgs e)
            {
                // let the system do base work first
                base.OnPaint(e);
                // grab some tools
                var rect = ClientRectangle;
                var g = e.Graphics;
                // draw the background
                ProgressBarRenderer.DrawHorizontalBar(g, rect);
                rect.Inflate(-3, -3);
                // then chunk
                if (Value > 0)
                {
                    Rectangle clip = new Rectangle(rect.X, rect.Y, Value * rect.Width / Maximum, rect.Height);
                    ProgressBarRenderer.DrawHorizontalChunks(g, clip);
                }
                // append text
                g.DrawString(CustomText, SystemFonts.DefaultFont, SystemBrushes.ControlText,
                    this.DisplayRectangle, Format);
            }
        }
    }
}
