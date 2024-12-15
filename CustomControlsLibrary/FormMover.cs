using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public class CustomFormMover : Component
    {
        private Form _targetForm = null;
        private Control _propertieControl = null;
        private MouseEventHandler _mouseDownEventArgs;
        private MouseEventHandler _mouseUpEventArgs;
        private MouseEventHandler _mouseMoveEventArgs;
        private Point _mouseDownLocation;
        private bool _isMoving = false;

        private List<Control> _controls = new List<Control>();

        private CustomControlBox _customControl = null;

        public CustomFormMover()
        {
            _mouseDownEventArgs = Control_MouseDown;
            _mouseUpEventArgs = Control_MouseUp;
            _mouseMoveEventArgs = Control_MouseMove;
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
                        _propertieControl.MouseDown -= _mouseDownEventArgs;
                        _propertieControl.MouseMove -= _mouseMoveEventArgs;
                        _propertieControl.MouseUp -= _mouseUpEventArgs;
                        _controls.Remove(_propertieControl);
                    }

                    if (!_controls.Contains(_propertieControl))
                    {
                        _propertieControl.MouseDown += _mouseDownEventArgs;
                        _propertieControl.MouseMove += _mouseMoveEventArgs;
                        _propertieControl.MouseUp += _mouseUpEventArgs;
                        _controls.Add(_propertieControl);
                    }
                }

                if (value != null && !_controls.Contains(value))
                {
                    value.MouseDown += _mouseDownEventArgs;
                    value.MouseMove += _mouseMoveEventArgs;
                    value.MouseUp += _mouseUpEventArgs;
                    _propertieControl = value;
                    _controls.Add(value);
                }
            }
        }

        public void Move(Control control)
        {
            if (!_controls.Contains(control))
            {
                control.MouseDown += _mouseDownEventArgs;
                control.MouseMove += _mouseMoveEventArgs;
                control.MouseUp += _mouseUpEventArgs;
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
                }

                _mouseDownLocation = e.Location;
                _isMoving = true;
            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isMoving) return;
            if (_mouseDownLocation == new Point(e.X, e.Y)) return;

            // Check the original size from CustomControlBox
            if (_targetForm != null)
            {
                if (_customControl != null && !_customControl.IsDisposed)
                {
                    var originalSize = _customControl.OriginalBounds?.Size;
                    if (originalSize != null && originalSize != _targetForm.Size)
                    {
                        _targetForm.Size = (Size)originalSize;
                    }
                }

                // Calculate the new position of the form
                Point newLocation = new Point(
                    _targetForm.Left + e.X - _mouseDownLocation.X,
                    _targetForm.Top + e.Y - _mouseDownLocation.Y
                );

                _targetForm.Location = newLocation;
            }
        }

        private void Control_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isMoving = false;
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
                        control.MouseDown -= _mouseDownEventArgs;
                        control.MouseUp -= _mouseUpEventArgs;
                        control.MouseMove -= _mouseMoveEventArgs;
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
