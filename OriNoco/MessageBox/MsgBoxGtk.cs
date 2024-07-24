using Gtk;

namespace OriNoco
{
    internal class MsgBoxGtk
    {
        internal static int Show(string text, string caption, MessageBoxType type, MessageBoxIcon icon)
        {
            MessageType gtkType = MessageType.Other;
            ButtonsType gtkBtnType = ButtonsType.None;
            switch(type)
            {
                case MessageBoxType.Ok:
                    gtkBtnType = ButtonsType.Ok;
                    break;
                case MessageBoxType.OkCancel:
                    gtkBtnType = ButtonsType.OkCancel;
                    break;
                case MessageBoxType.YesNo:
                    gtkBtnType = ButtonsType.YesNo;
                    break;
            }

            switch (icon)
            { 
                case MessageBoxIcon.Error:
                    gtkType = MessageType.Error;
                    break;
                case MessageBoxIcon.Warning:
                    gtkType = MessageType.Warning;
                    break;
                case MessageBoxIcon.Information:
                    gtkType = MessageType.Info;
                    break;
            }

            var dialog = new MessageDialog(null, DialogFlags.Modal, gtkType, gtkBtnType, text);
            int result = dialog.Run();
            dialog.Destroy();
            return result;
        }
    }
}
