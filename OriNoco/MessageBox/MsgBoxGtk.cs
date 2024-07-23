using Gtk;

namespace OriNoco
{
    internal class MsgBoxGtk
    {
        internal static Result Show(string text, string caption, MessageBoxType type, MessageBoxIcon icon)
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
                case MessageBoxIcon.None:
                    gtkType = MessageType.Other;
                    break;
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
            var result = (ResponseType)dialog.Run();
            dialog.Destroy();
            switch (result)
            {
                case ResponseType.Ok:
                    return Result.Ok;
                case ResponseType.Cancel:
                    return Result.Cancel;
                case ResponseType.Yes:
                    return Result.Yes;
                case ResponseType.No:
                    return Result.No;
                default:
                    return Result.Unknown;
            }
        }
    }
}
