using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
    public class FormMover
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public static void Move(Control control)
        {
            control.MouseDown += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Form form = control.FindForm();
                    if (form != null)
                    {
                        ReleaseCapture();
                        SendMessage(form.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                    }
                }
            };
        }
    }
}
