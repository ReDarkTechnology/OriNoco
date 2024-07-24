using System;
using System.Runtime.InteropServices;

namespace OriNoco
{
    public static class MessageBox
    {
        public static Result Show(string text, string caption, MessageBoxType type, MessageBoxIcon icon)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return MsgBoxWin.Show(text, caption, type, icon);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return MsgBoxGtk.Show(text, caption, type, icon);
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }
    }

    public enum MessageBoxType : uint
    {
        Ok = 0,
        OkCancel = 1,
        YesNo = 4
    }

    public enum MessageBoxIcon : uint
    {
        None = 0,
        Error = 16,
        Warning = 48,
        Information = 64
    }

    public enum Result
    {
        Ok = 1,
        Cancel = 2,
        Yes = 6,
        No = 7,
        Unknown = 128
    }
}
