using System;
using System.Runtime.InteropServices;

namespace OriNoco
{
    internal class MsgBoxWin
    {
        [DllImport("user32.dll")]
        internal static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        internal static Result Show(string text, string caption, MessageBoxType type, MessageBoxIcon icon) =>
            (Result)MessageBox(IntPtr.Zero, text, caption, (uint)type | (uint)icon);
    }
}
