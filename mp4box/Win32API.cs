// ------------------------------------------------------------------
// Copyright (C) 2015 Maruko Toolbox Project
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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace mp4box
{
    namespace Win32API
    {
        /// <summary>
        /// Class hosting Native Win32 APIs
        /// </summary>
        public partial class NativeMethods
        {
            #region <WinUser.h>

            /// <summary>
            /// Specifies the type of scroll bar.
            /// </summary>
            public enum ScrollBarConsts : int
            {
                /// <summary>
                /// Window's standard horizontal scroll bar.
                /// </summary>
                SB_HORZ = 0,
                /// <summary>
                /// Window's standard vertical scroll bar.
                /// </summary>
                SB_VERT = 1,
                /// <summary>
                /// Retrieves the position of the scroll box in a scroll bar control.
                ///     The hWnd parameter must be the handle to the scroll bar control.
                /// </summary>
                SB_CTL = 2,
                /// <summary>
                /// Window's standard scroll bar.
                /// </summary>
                SB_BOTH = 3
            }

            /// <summary>
            /// Specifies the commands to scroll bar.
            /// </summary>
            public enum ScrollBarCmds : int
            {
                /// <summary>
                /// Scrolls one line down.
                /// </summary>
                SB_LINEUP = 0,
                /// <summary>
                /// Scrolls left by one unit.
                /// </summary>
                SB_LINELEFT = 0,
                /// <summary>
                /// Scrolls one line up.
                /// </summary>
                SB_LINEDOWN = 1,
                /// <summary>
                /// Scrolls right by one unit.
                /// </summary>
                SB_LINERIGHT = 1,
                /// <summary>
                /// Scrolls one page up.
                /// </summary>
                SB_PAGEUP = 2,
                /// <summary>
                /// Scrolls left by the width of the window.
                /// </summary>
                SB_PAGELEFT = 2,
                /// <summary>
                /// Scrolls one page down.
                /// </summary>
                SB_PAGEDOWN = 3,
                /// <summary>
                /// Scrolls right by the width of the window.
                /// </summary>
                SB_PAGERIGHT = 3,
                /// <summary>
                /// The user has dragged the scroll box (thumb) and released the mouse button.
                ///     The HIWORD indicates the position of the scroll box at the end of the drag operation.
                /// </summary>
                SB_THUMBPOSITION = 4,
                /// <summary>
                /// The user is dragging the scroll box. This message is sent repeatedly until the user releases the mouse button.
                ///     The HIWORD indicates the position that the scroll box has been dragged to.
                /// </summary>
                SB_THUMBTRACK = 5,
                /// <summary>
                /// Scrolls to the upper left.
                /// </summary>
                SB_TOP = 6,
                /// <summary>
                /// Scrolls to the upper left.
                /// </summary>
                SB_LEFT = 6,
                /// <summary>
                /// Scrolls to the lower right.
                /// </summary>
                SB_BOTTOM = 7,
                /// <summary>
                /// Scrolls to the lower right.
                /// </summary>
                SB_RIGHT = 7,
                /// <summary>
                /// Ends scroll.
                /// </summary>
                SB_ENDSCROLL = 8
            }

            /// <summary>
            /// Indicate the parameters to set or get.
            /// </summary>
            public enum ScrollBarFlags : uint
            {
                /// <summary>
                /// The nMin and nMax members contain the minimum and maximum values for the scrolling range.
                /// </summary>
                SIF_RANGE = 0x1,
                /// <summary>
                /// The nPage member contains the page size for a proportional scroll bar.
                /// </summary>
                SIF_PAGE = 0x2,
                /// <summary>
                /// The nPos member contains the scroll box position, which is not updated while the user drags the scroll box.
                /// </summary>
                SIF_POS = 0x4,
                /// <summary>
                /// This value is used only when setting a scroll bar's parameters.
                /// If the scroll bar's new parameters make the scroll bar unnecessary, disable the scroll bar instead of removing it.
                /// </summary>
                SIF_DISABLENOSCROLL = 0x8,
                /// <summary>
                /// The nTrackPos member contains the current position of the scroll box while the user is dragging it.
                /// </summary>
                SIF_TRACKPOS = 0x10,
                /// <summary>
                /// Combination of SIF_PAGE, SIF_POS, SIF_RANGE, and SIF_TRACKPOS.
                /// </summary>
                SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS)
            }

            /// <summary>
            /// The flash status.
            /// </summary>
            public enum FlashWindowFlags : uint
            {
                /// <summary>
                /// Stop flashing. The system restores the window to its original state.
                /// </summary>
                FLASHW_STOP = 0,
                /// <summary>
                /// Flash the window caption.
                /// </summary>
                FLASHW_CAPTION = 0x1,
                /// <summary>
                /// Flash the taskbar button.
                /// </summary>
                FLASHW_TRAY = 0x2,
                /// <summary>
                /// Flash both the window caption and taskbar button.
                /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
                /// </summary>
                FLASHW_ALL = (FLASHW_CAPTION | FLASHW_TRAY),
                /// <summary>
                /// Flash continuously, until the FLASHW_STOP flag is set.
                /// </summary>
                FLASHW_TIMER = 0x4,
                /// <summary>
                /// Flash continuously until the window comes to the foreground.
                /// </summary>
                FLASHW_TIMERNOFG = 0xC
            }

            /// <summary>
            /// Window Messages
            /// </summary>
            public enum Win32Msgs : uint
            {
                /// <summary>
                /// The WM_HSCROLL message is sent to a window when a scroll event occurs in the window's standard horizontal scroll bar.
                ///     This message is also sent to the owner of a horizontal scroll bar control when a scroll event occurs in the control.
                /// </summary>
                WM_HSCROLL = 0x114,
                /// <summary>
                /// The WM_VSCROLL message is sent to a window when a scroll event occurs in the window's standard vertical scroll bar.
                ///     This message is also sent to the owner of a vertical scroll bar control when a scroll event occurs in the control.
                /// </summary>
                WM_VSCROLL = 0x115,
                /// <summary>
                /// The WM_CTLCOLORSCROLLBAR message is sent to the parent window of a scroll bar control when the control is about to be drawn.
                ///     By responding to this message, the parent window can use the display context handle to set the background color of the scroll bar control.
                /// </summary>
                WM_CTLCOLORSCROLLBAR = 0x137,
                /// <summary>
                /// Used to define private messages for use by private window classes, usually of the form WM_USER+x, where x is an integer value.
                /// </summary>
                WM_USER = 0x400
            }

            /// <summary>
            /// Contains scroll bar parameters to be set by the SetScrollInfo function, or retrieved by the GetScrollInfo function.
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/bb787537(v=vs.85).aspx</para>
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct SCROLLINFO
            {
                /// <summary>
                /// Specifies the size, in bytes, of this structure.
                /// </summary>
                public uint cbSize;
                /// <summary>
                /// Specifies the scroll bar parameters to set or retrieve.
                /// </summary>
                public ScrollBarFlags fMask;
                /// <summary>
                /// Specifies the minimum scrolling position. 
                /// </summary>
                public int nMin;
                /// <summary>
                /// Specifies the maximum scrolling position. 
                /// </summary>
                public int nMax;
                /// <summary>
                /// Specifies the page size, in device units.
                ///     A scroll bar uses this value to determine the appropriate size of the proportional scroll box. 
                /// </summary>
                public uint nPage;
                /// <summary>
                /// Specifies the position of the scroll box. 
                /// </summary>
                public int nPos;
                /// <summary>
                /// Specifies the immediate position of a scroll box that the user is dragging.
                ///     An application cannot set the immediate scroll position; the SetScrollInfo function ignores this member. 
                /// </summary>
                public int nTrackPos;
            }

            /// <summary>
            /// Contains the flash status for a window and the number of times the system should flash the window.
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/ms679348(v=vs.85).aspx</para>
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct FLASHWINFO
            {
                /// <summary>
                /// The size of the structure, in bytes.
                /// </summary>
                public uint cbSize;
                /// <summary>
                /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
                /// </summary>
                public IntPtr hwnd;
                /// <summary>
                /// The Flash Status.
                /// </summary>
                public FlashWindowFlags dwFlags;
                /// <summary>
                /// The number of times to Flash the window.
                /// </summary>
                public uint uCount;
                /// <summary>
                /// The rate at which the Window is to be flashed, in milliseconds.
                ///     If Zero, the function uses the default cursor blink rate.
                /// </summary>
                public uint dwTimeout;
            }

            /// <summary>
            /// Creates a value for use as a wParam parameter in a message.
            ///     The macro concatenates the specified values.
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/ms632664(v=vs.85).aspx</para>
            /// </summary>
            /// <param name="wLow">The low-order word of the new value.</param>
            /// <param name="wHi">The high-order word of the new value.</param>
            /// <returns>The return value is a WPARAM value.</returns>
            public static UIntPtr MakeWParam(uint wLow, uint wHi)
            {
                return (UIntPtr)MakeLong(wLow, wHi);
            }

            /// <summary>
            /// Creates a value for use as an lParam parameter in a message.
            ///     The macro concatenates the specified values.
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/ms632661(v=vs.85).aspx</para>
            /// </summary>
            /// <param name="wLow">The low-order word of the new value.</param>
            /// <param name="wHi">The high-order word of the new value. </param>
            /// <returns>The return value is an LPARAM value. </returns>
            public static IntPtr MakeLParam(uint wLow, uint wHi)
            {
                return (IntPtr)MakeLong(wLow, wHi);
            }

            #endregion

            #region <WinDef.h>

            /// <summary>
            /// Retrieves the high-order word from the specified 32-bit value.
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/ms632657(v=vs.85).aspx</para>
            /// </summary>
            /// <param name="ptr">The value to be converted.</param>
            /// <returns>The return value is the high-order word of the specified value.</returns>
            public static uint HiWord(IntPtr ptr)
            {
                return ((uint)ptr >> 16) & 0xFFFFu;
            }

            /// <summary>
            /// Retrieves the low-order word from the specified value.
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/ms632659(v=vs.85).aspx</para>
            /// </summary>
            /// <param name="ptr">The value to be converted.</param>
            /// <returns>The return value is the low-order word of the specified value.</returns>
            public static uint LoWord(IntPtr ptr)
            {
                return (uint)ptr & 0xFFFFu;
            }

            /// <summary>
            /// Creates a LONG value by concatenating the specified values.
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/ms632660(v=vs.85).aspx</para>
            /// </summary>
            /// <param name="wLow">The low-order word of the new value.</param>
            /// <param name="wHi">The high-order word of the new value.</param>
            /// <returns>The return value is a LONG value.</returns>
            public static uint MakeLong(uint wLow, uint wHi)
            {
                return (wLow & 0xFFFFu) | ((wHi & 0xFFFFu) << 16);
            }

            #endregion

            #region <CommCtrl.h>

            /// <summary>
            /// Windows Common Controls Messages
            /// </summary>
            public enum Win32CommCtrlMsgs : uint
            {
                /// <summary>
                /// Edit control messages
                /// </summary>
                ECM_FIRST = 0x1500,

                /// <summary>
                /// Set the cue banner with the lParm = LPCWSTR
                /// </summary>
                EM_SETCUEBANNER = ECM_FIRST + 1,

                /// <summary>
                /// Sets the state of the progress bar.
                /// </summary>
                PBM_SETSTATE = Win32Msgs.WM_USER + 16
            }

            /// <summary>
            /// ProgressBar States
            /// </summary>
            public enum ProgressBarState : uint
            {
                /// <summary>
                /// In progress.
                /// </summary>
                PBST_NORMAL = 0x1,
                /// <summary>
                /// Error.
                /// </summary>
                PBST_ERROR = 0x2,
                /// <summary>
                /// Paused.
                /// </summary>
                PBST_PAUSED = 0x3
            }

            #endregion

            /// <summary>
            /// The GetScrollInfo function retrieves the parameters of a scroll bar,
            ///     including the minimum and maximum scrolling positions, the page size,
            ///     and the position of the scroll box (thumb).
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/bb787583(v=vs.85).aspx</para>
            /// </summary>
            /// <param name="hWnd">Handle to a scroll bar control or a window with a standard scroll bar,
            ///     depending on the value of the fnBar parameter. </param>
            /// <param name="fnBar">Specifies the type of scroll bar for which to retrieve parameters.</param>
            /// <param name="lpsi">Pointer to a SCROLLINFO structure. Before calling GetScrollInfo, set the cbSize member to sizeof(SCROLLINFO),
            ///     and set the fMask member to specify the scroll bar parameters to retrieve.
            ///     Before returning, the function copies the specified parameters to the appropriate members of the structure.</param>
            /// <returns>If the function retrieved any values, the return value is nonzero.</returns>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetScrollInfo(
                IntPtr hWnd,
                ScrollBarConsts fnBar,
                ref SCROLLINFO lpsi);

            /// <summary>
            /// The SetScrollInfo function sets the parameters of a scroll bar, including the minimum and maximum scrolling positions,
            ///     the page size, and the position of the scroll box (thumb). The function also redraws the scroll bar, if requested.
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/bb787595(v=vs.85).aspx</para>
            /// </summary>
            /// <param name="hWnd">Handle to a scroll bar control or a window with a standard scroll bar,
            ///     depending on the value of the fnBar parameter. </param>
            /// <param name="fnBar">Specifies the type of scroll bar for which to set parameters.</param>
            /// <param name="lpsi">Pointer to a SCROLLINFO structure. Before calling SetScrollInfo, set the cbSize member of the structure to sizeof(SCROLLINFO),
            ///     set the fMask member to indicate the parameters to set, and specify the new parameter values in the appropriate members.</param>
            /// <param name="fRedraw">Specifies whether the scroll bar is redrawn to reflect the changes to the scroll bar.
            ///     If this parameter is TRUE, the scroll bar is redrawn, otherwise, it is not redrawn. </param>
            /// <returns>The current position of the scroll box.</returns>
            [DllImport("user32.dll")]
            public static extern int SetScrollInfo(
                IntPtr hWnd,
                ScrollBarConsts fnBar,
                ref SCROLLINFO lpsi,
                [MarshalAs(UnmanagedType.Bool)] bool fRedraw);

            /// <summary>
            /// Flashes the specified window. It does not change the active state of the window.
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/ms679347(v=vs.85).aspx</para>
            /// </summary>
            /// <param name="pwfi">A pointer to a FLASHWINFO structure.</param>
            /// <returns>The return value specifies the window's state before the call to the FlashWindowEx function.
            ///     If the window caption was drawn as active before the call, the return value is nonzero. Otherwise, the return value is zero.</returns>
            [DllImport("user32.dll")]
            public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

            /// <summary>
            /// Places (posts) a message in the message queue associated with the thread that created
            ///     the specified window and returns without waiting for the thread to process the message.
            /// <para>http://msdn.microsoft.com/en-us/library/windows/desktop/ms644944(v=vs.85).aspx</para>
            /// </summary>
            /// <param name="hWnd">A handle to the window whose window procedure is to receive the message.</param>
            /// <param name="Msg">The message to be posted.</param>
            /// <param name="wParam">Additional message-specific information.</param>
            /// <param name="lParam">Additional message-specific information.</param>
            /// <returns>If the function succeeds, the return value is nonzero.</returns>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PostMessage(
                IntPtr hWnd,
                Win32Msgs Msg,
                [MarshalAs(UnmanagedType.SysUInt)] UIntPtr wParam,
                [MarshalAs(UnmanagedType.SysInt)] IntPtr lParam);

            /// <summary>
            /// Another overload for PostMessage.
            /// </summary>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PostMessage(
                IntPtr hWnd,
                Win32CommCtrlMsgs Msg,
                [MarshalAs(UnmanagedType.SysUInt)] UIntPtr wParam,
                [MarshalAs(UnmanagedType.SysInt)] IntPtr lParam);

            /// <summary>
            /// Sets the text that is displayed as the textual cue, or tip, for an edit control.
            /// <para>https://msdn.microsoft.com/en-us/library/windows/desktop/bb761701%28v=vs.85%29.aspx</para>
            /// </summary>
            /// <param name="hWnd">A handle to the edit control.</param>
            /// <param name="lpcwText">A pointer to a Unicode string that contains the text to set as the textual cue.</param>
            /// <returns>If the method succeeds, it returns true. Otherwise it returns false.</returns>
            public static bool Edit_SetCueBannerText(IntPtr hWnd, string lpcwText)
            {
                var lp = Marshal.StringToHGlobalAnsi(lpcwText);
                var result = PostMessage(hWnd, Win32CommCtrlMsgs.EM_SETCUEBANNER,
                    new UIntPtr(0), Marshal.StringToHGlobalUni(lpcwText));
                Marshal.FreeHGlobal(lp);
                return result;
            }
        }
    }
}
