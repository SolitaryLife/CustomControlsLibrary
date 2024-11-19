using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public class CustomFormMover : Component
    {
        private Form _targetForm = null;
        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        private Control _propertieControl = null;

        private List<Control> _controls = new List<Control>();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [Category("Custom FormMover")]
        [Description("The control that will be used to move the form.")]
        public Control TargetControl
        {
            get
            {
                return _propertieControl;
            }
            set
            {
                _propertieControl = value;

                if (_propertieControl != null)
                {
                    if (_controls.Contains(_propertieControl))
                    {
                        _propertieControl.MouseDown -= Control_MouseDown;
                        _controls.Remove(_propertieControl);
                    }

                    if (!_controls.Contains(_propertieControl))
                    {
                        _propertieControl.MouseDown += Control_MouseDown;
                        _controls.Add(_propertieControl);
                    }
                }

                if (value != null && !_controls.Contains(value))
                {

                    value.MouseDown += Control_MouseDown;
                    _propertieControl = value;
                    _controls.Add(value);
                }
            }
        }

        public void Move(Control control)
        {
            if (!_controls.Contains(control))
            {
                control.MouseDown += Control_MouseDown;
                _controls.Add(control);
            }
        }

        private void Control_MouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Control control && e.Button == MouseButtons.Left)
            {
                if (_targetForm == null)
                {
                    _targetForm = control.FindForm();
                }
                if (_targetForm != null)
                {
                    ReleaseCapture();
                    SendMessage(_targetForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }

        private bool _isDispose = false;
        protected override void Dispose(bool disposing)
        {
            if (!_isDispose)
            {
                if (disposing)
                {
                    foreach (var control in _controls)
                    {
                        control.MouseDown -= Control_MouseDown;
                    }
                    _controls.Clear();
                    _propertieControl = null;
                    _targetForm = null;
                }

                _isDispose = true;
            }
            base.Dispose(disposing);
        }

        ~CustomFormMover()
        {
            Dispose(true);
        }
    }
}
