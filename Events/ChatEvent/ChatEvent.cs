// Copyright 2016-2040 Nino Crudele
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#region Usings

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using SnapGate.Framework.Base;
using SnapGate.Framework.Contracts.Attributes;
using SnapGate.Framework.Contracts.Events;
using SnapGate.Framework.Contracts.Globals;

#endregion

namespace SnapGate.Framework.ChatEvent
{
    /// <summary>
    ///     The chat event.
    /// </summary>
    [EventContract("{90662D0F-9BBD-4E74-A12D-79BCC0B76BAA}", "Chat Event", "Create a P2P chat bridge.", true)]
    public class ChatEvent : IEventType
    {
        /// <summary>
        ///     The e m_ replacesel.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Reviewed. Suppression is OK here.")]
        // ReSharper disable once InconsistentNaming
        private const int EM_REPLACESEL = 0x00C2;

        /// <summary>
        ///     The e m_ setsel.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        private readonly int EM_SETSEL = 0x00B1;

        /// <summary>
        ///     The w m_ gettextlength.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        private readonly int WM_GETTEXTLENGTH = 0x000E;

        /// <summary>
        ///     Gets or sets the context.
        /// </summary>
        public ActionContext Context { get; set; }

        /// <summary>
        ///     Gets or sets the set event action event.
        /// </summary>
        public ActionEvent ActionEvent { get; set; }

        /// <summary>
        ///     Gets or sets the data context.
        /// </summary>
        [EventPropertyContract("DataContext", "Event Default Main Data")]
        public byte[] DataContext { get; set; }

        /// <summary>
        ///     The execute.
        /// </summary>
        /// <param name="actionEvent">
        ///     The set event action event.
        /// </param>
        /// <param name="context">
        ///     The context.
        /// </param>
        [EventActionContract("{{3C670559-B77F-498F-9855-BC5C8E22C758}", "Main action", "Main action description")]
        public async Task Execute(ActionEvent actionEvent, ActionContext context)
        {
            var content = EncodingDecoding.EncodingBytes2String(DataContext);
            var notepads = Process.GetProcessesByName("notepad");

            if (notepads.Length == 0)
            {
                return;
            }

            if (notepads[0] != null && notepads[0].MainWindowTitle.ToUpper() == "CHAT.TXT - NOTEPAD")
            {
                EmptyClipboard();
                var child = FindWindowEx(notepads[0].MainWindowHandle, new IntPtr(0), "Edit", null);
                var length = SendMessageGetTextLength(child, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
                SendMessage(child, EM_SETSEL, length, length); // search end of file position
                content += "\r\n";
                SendMessage(child, EM_REPLACESEL, 1, content); // append new line
            }

            return;
        }

        /// <summary>
        ///     The send message.
        /// </summary>
        /// <param name="hWnd">
        ///     The h wnd.
        /// </param>
        /// <param name="uMsg">
        ///     The u msg.
        /// </param>
        /// <param name="wParam">
        ///     The w param.
        /// </param>
        /// <param name="lParam">
        ///     The l param.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        [DllImport("User32.dll")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);

        /// <summary>
        ///     The find window ex.
        /// </summary>
        /// <param name="hwndParent">
        ///     The hwnd parent.
        /// </param>
        /// <param name="hwndChildAfter">
        ///     The hwnd child after.
        /// </param>
        /// <param name="lpszClass">
        ///     The lpsz class.
        /// </param>
        /// <param name="lpszWindow">
        ///     The lpsz window.
        /// </param>
        /// <returns>
        ///     The <see cref="IntPtr" />.
        /// </returns>
        [DllImport("user32.dll", EntryPoint = "FindWindowEx")]
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements",
            Justification = "Reviewed. Suppression is OK here.")]
        public static extern IntPtr FindWindowEx(
            IntPtr hwndParent,
            IntPtr hwndChildAfter,
            string lpszClass,
            string lpszWindow);

        /// <summary>
        ///     The send message.
        /// </summary>
        /// <param name="hWnd">
        ///     The h wnd.
        /// </param>
        /// <param name="uMsg">
        ///     The u msg.
        /// </param>
        /// <param name="wParam">
        ///     The w param.
        /// </param>
        /// <param name="lParam">
        ///     The l param.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        [DllImport("User32.dll")]
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:StaticElementsMustAppearBeforeInstanceElements",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        private static extern int SendMessageGetTextLength(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EmptyClipboard();
    }
}