using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
        private MouseEventHandler _mouseEventArgs;

        private List<Control> _controls = new List<Control>();

        private CustomControlBox _customControl = null;


        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        public CustomFormMover()
        {
            _mouseEventArgs = Control_MouseDown;
        }

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
                        _propertieControl.MouseDown -= _mouseEventArgs;
                        _controls.Remove(_propertieControl);
                    }

                    if (!_controls.Contains(_propertieControl))
                    {
                        _propertieControl.MouseDown += _mouseEventArgs;
                        _controls.Add(_propertieControl);
                    }
                }

                if (value != null && !_controls.Contains(value))
                {

                    value.MouseDown += _mouseEventArgs;
                    _propertieControl = value;
                    _controls.Add(value);
                }
            }
        }

        public void Move(Control control)
        {
            if (!_controls.Contains(control))
            {
                control.MouseDown += _mouseEventArgs;
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

                    if (_customControl == null || _customControl.IsDisposed)
                    {
                        #region Queue
                        //var queue = new Queue<Control>(_targetForm.Controls.Cast<Control>());
                        //while (queue.Count > 0)
                        //{
                        //    // Remove Control from Queue
                        //    var qControl = queue.Dequeue();

                        //    // Verify that the Control is CustomControlBox and has the required BoxType.
                        //    if (qControl is CustomControlBox2 customControl &&
                        //        customControl.BoxType == CustomControlBox2.ControlBoxType.MaximizeBox)
                        //    {
                        //        _customControl = customControl;
                        //        break;
                        //    }

                        //    // Add all Child Controls to the Queue.
                        //    foreach (Control child in qControl.Controls)
                        //    {
                        //        queue.Enqueue(child);
                        //    }
                        //}

                        //queue.Clear();
                        #endregion

                        #region Stack
                        var stack = new Stack<Control>(_targetForm.Controls.Cast<Control>());

                        while (stack.Count > 0)
                        {
                            // Remove Control from Stack
                            var qControl = stack.Pop();

                            // Verify that the Control is CustomControlBox and has the required BoxType.
                            if (qControl is CustomControlBox customControl &&
                                customControl.BoxType == CustomControlBox.ControlBoxType.MaximizeBox)
                            {
                                _customControl = customControl;
                                break;
                            }

                            // Add all Child Controls to the Stack.
                            foreach (Control child in qControl.Controls)
                            {
                                stack.Push(child);
                            }
                        }
                        stack.Clear();

                        #endregion
                    }

                    if (_customControl != null && !_customControl.IsDisposed)
                    {
                        var valueSize = _customControl.OriginalBounds?.Size;
                        if (valueSize != null)
                        {
                            _targetForm.Size = (Size)valueSize;
                        }
                    }

                    ReleaseCapture();
                    SendMessage(_targetForm.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            }
        }


        #region Dispose
        private bool _isDispose = false;
        protected override void Dispose(bool disposing)
        {
            if (!_isDispose)
            {
                if (disposing)
                {
                    foreach (var control in _controls)
                    {
                        control.MouseDown -= _mouseEventArgs;
                    }
                    _controls.Clear();
                    _propertieControl = null;
                    _targetForm = null;
                    _customControl = null;
                }
                _isDispose = true;
            }


            base.Dispose(disposing);
        }

        ~CustomFormMover()
        {
            Dispose(true);
        }

        #endregion
    }
}
