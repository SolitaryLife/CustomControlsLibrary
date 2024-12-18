﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public class CustomControlBox : UserControl
    {
        public enum ControlBoxType
        {
            CloseBox,
            MaximizeBox,
            MinimizeBox
        }

        private Color hoverColor = Color.Red;
        private Color normalColor = Color.Gray;
        private bool isHovered = false;
        private ControlBoxType boxType = ControlBoxType.CloseBox;
        private int buttonSize = 32;
        private int _iconSize = 12;
        private Cursor customCursor = Cursors.Hand;
        private Color iconColor = Color.White;
        private float iconScale = 1.0f;
        private float iconThickness = 1.5f;
        private Form targetForm = null;
        private event EventHandler CustomMove;

        #region Properties
        // Properties
        [Category("Custom ControlBox")]
        [DefaultValue(SmoothingMode.Default)]
        [Description("Specifies the smoothing mode for rendering, which affects the quality of lines and edges.")]
        public SmoothingMode SmoothingMode { get; set; }

        [Category("Custom ControlBox")]
        [DefaultValue(InterpolationMode.Default)]
        [Description("Determines the interpolation mode used for scaling images, which impacts image quality.")]
        public InterpolationMode InterpolationMode { get; set; }

        [Category("Custom ControlBox")]
        [DefaultValue(PixelOffsetMode.Default)]
        [Description("Sets the pixel offset mode, controlling pixel alignment for improved rendering accuracy.")]
        public PixelOffsetMode PixelOffsetMode { get; set; }

        [Category("Custom ControlBox")]
        [DefaultValue(CompositingQuality.Default)]
        [Description("Gets or sets the compositing quality level for drawing operations. Compositing quality determines how drawing operations are blended or composited.")]
        public CompositingQuality CompositingQuality { get; set; }

        [Category("Custom ControlBox")]
        [DefaultValue(true)]
        [Description("Enables or disables double buffering to reduce flickering during rendering.")]
        public bool DoubleBuffereds
        {
            get => DoubleBuffered;
            set => DoubleBuffered = value;
        }

        [Category("Custom ControlBox")]
        [Description("The size of the icon.")]
        [DefaultValue(12)]
        public int IconSize
        {
            get => _iconSize;
            set
            {
                _iconSize = value;
                Invalidate();
            }
        }

        [Category("Custom ControlBox")]
        [Description("The thickness of the icon lines, limited between 0 and 5.")]
        public float IconThickness
        {
            get { return iconThickness; }
            set
            {
                if (iconThickness != value && value > 0f && value <= 5f)  // Limit thickness between 0 and 5
                {
                    iconThickness = value;
                    Invalidate();
                }
            }
        }

        [Category("Custom ControlBox")]
        [Description("The color of the icon displayed in the custom control box.")]
        public Color IconColor
        {
            get { return iconColor; }
            set
            {
                if (iconColor != value)
                {
                    iconColor = value;
                    Invalidate();
                }
            }
        }

        [Category("Custom ControlBox")]
        [DefaultValue(1.0f)]
        public float IconScale
        {
            get { return iconScale; }
            set
            {
                if (iconScale != value && value >= 0.1f && value <= 2.0f)  // Limit size between 10% and 200%
                {
                    iconScale = value;
                    Invalidate();
                }
            }
        }

        [Category("Custom ControlBox")]
        [Description("The color displayed when the button is hovered over.")]
        public Color HoverColor
        {
            get { return hoverColor; }
            set
            {
                hoverColor = value;
                Invalidate();
            }
        }

        [Category("Custom ControlBox")]
        [Description("The default color of the button.")]
        public Color NormalColor
        {
            get { return normalColor; }
            set
            {
                normalColor = value;
                Invalidate();
            }
        }

        [Category("Custom ControlBox")]
        [Description("The size of the button.")]
        public int ButtonSize
        {
            get { return buttonSize; }
            set
            {
                buttonSize = value;
                Size = new Size(value, value);
                Invalidate();
            }
        }

        [Category("Custom ControlBox")]
        [Description("The type of control box used by the button.")]
        public ControlBoxType BoxType
        {
            get { return boxType; }
            set
            {
                boxType = value;
                AddEventinForm();
                Invalidate();
            }
        }

        [Category("Custom ControlBox")]
        [Description("The custom cursor displayed when hovering over the button.")]
        public override Cursor Cursor
        {
            get { return customCursor; }
            set
            {
                customCursor = value;
                base.Cursor = value;
            }
        }
        #endregion

        // Constructor
        public CustomControlBox()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint, true);

            Size = new Size(buttonSize, buttonSize);
            BackColor = Color.Transparent;
            Cursor = customCursor;

            CustomMove = ParentForm_Move;


            DoubleBuffered = true;
        }

        // Paint event
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0) return;

            try
            {
                Graphics g = e.Graphics;
                //if (BackColor != Color.Transparent)
                //{
                //    g.Clear(BackColor);
                //}
                g.SmoothingMode = SmoothingMode;
                g.InterpolationMode = InterpolationMode;
                g.PixelOffsetMode = PixelOffsetMode;
                g.CompositingQuality = CompositingQuality;

                // Draw background
                using (var brush = new SolidBrush(isHovered ? hoverColor : normalColor))
                {
                    g.FillRectangle(brush, 0, 0, Width, Height);
                }

                // Draw Icon with modified calculations
                using (var pen = new Pen(iconColor, iconThickness))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;

                    // Calculate icon size based on control size and IconSize property
                    float actualIconSize = Math.Min(Width, Height) * (_iconSize / 32f) * iconScale;
                    float centerX = Width / 2.0f;
                    float centerY = Height / 2.0f;
                    float halfSize = actualIconSize / 2.0f;

                    switch (boxType)
                    {
                        case ControlBoxType.CloseBox:
                            using (var path = new GraphicsPath())
                            {
                                path.StartFigure();
                                path.AddLine(
                                    centerX - halfSize, centerY - halfSize,
                                    centerX + halfSize, centerY + halfSize
                                );

                                path.StartFigure();
                                path.AddLine(
                                    centerX + halfSize, centerY - halfSize,
                                    centerX - halfSize, centerY + halfSize
                                );

                                g.DrawPath(pen, path);
                            }
                            break;

                        case ControlBoxType.MaximizeBox:
                            if (targetForm == null)
                            {
                                targetForm = FindForm();
                            }
                            bool isMaximized = _isMaximized;

                            if (isMaximized)
                            {
                                float smallRectOffset = actualIconSize * 0.15f;

                                using (var path = new GraphicsPath())
                                {
                                    RectangleF backRect = new RectangleF(
                                        centerX - halfSize,
                                        centerY - halfSize,
                                        actualIconSize - smallRectOffset,
                                        actualIconSize - smallRectOffset
                                    );
                                    path.AddRectangle(backRect);
                                    g.DrawPath(pen, path);
                                }

                                using (var path = new GraphicsPath())
                                {
                                    RectangleF frontRect = new RectangleF(
                                        centerX - halfSize + smallRectOffset,
                                        centerY - halfSize + smallRectOffset,
                                        actualIconSize - smallRectOffset,
                                        actualIconSize - smallRectOffset
                                    );
                                    path.AddRectangle(frontRect);
                                    g.DrawPath(pen, path);
                                }
                            }
                            else
                            {
                                using (var path = new GraphicsPath())
                                {
                                    RectangleF rect = new RectangleF(
                                        centerX - halfSize,
                                        centerY - halfSize,
                                        actualIconSize,
                                        actualIconSize
                                    );
                                    path.AddRectangle(rect);
                                    g.DrawPath(pen, path);
                                }
                            }
                            break;

                        case ControlBoxType.MinimizeBox:
                            using (var path = new GraphicsPath())
                            {
                                path.StartFigure();
                                path.AddLine(
                                    centerX - halfSize, centerY,
                                    centerX + halfSize, centerY
                                );

                                g.DrawPath(pen, path);
                            }
                            break;
                    }
                }
            }
            catch { }
        }

        // Mouse events
        protected override void OnMouseEnter(EventArgs e)
        {
            isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }


        private bool _isMaximized = false;
        private bool _isMinimized = false;
        private Rectangle? originalBounds;

        /// <summary>
        /// Gets the original bounds of the window before it was maximized.
        /// </summary>
        public Rectangle? OriginalBounds
        {
            get { return originalBounds; }
        }

        /// <summary>
        /// Simulates a click on the control, triggering the OnClick event.
        /// This method is typically used to programmatically invoke a click.
        /// </summary>
        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        private void AddEventinForm()
        {
            if (targetForm == null)
            {
                targetForm = FindForm();
            }

            switch (boxType)
            {
                case ControlBoxType.MaximizeBox:
                    if (targetForm != null)
                    {
                        targetForm.Move -= CustomMove;
                        targetForm.Move += CustomMove;
                        if (originalBounds == null)
                        {
                            originalBounds = targetForm.Bounds;
                        }
                    }
                    break;
                default:
                    if (targetForm != null)
                    {
                        targetForm.Move -= CustomMove;
                        _isMaximized = false;
                    }
                    break;
            }
        }


        private void ParentForm_Move(object sender, EventArgs e)
        {
            if (targetForm == null || !_isMaximized) return;
            if (targetForm.WindowState == FormWindowState.Minimized)
            {
                _isMinimized = true;
            }
            else if (_isMinimized)
            {
                _isMinimized = false;
            }
            else if (!_isMinimized && targetForm.WindowState == FormWindowState.Normal)
            {
                _isMaximized = false;
                Invalidate();
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            AddEventinForm();
        }

        protected override void OnClick(EventArgs e)
        {
            AddEventinForm();
            if (targetForm != null)
            {
                switch (boxType)
                {
                    case ControlBoxType.CloseBox:
                        targetForm.Close();
                        break;

                    case ControlBoxType.MaximizeBox:
                        if (!_isMaximized)
                        {
                            // Save the original window bounds and maximize
                            targetForm.WindowState = FormWindowState.Normal;
                            originalBounds = targetForm.Bounds;
                            targetForm.Bounds = Screen.FromControl(targetForm).WorkingArea;
                            _isMaximized = true;
                        }
                        else
                        {
                            // Restore the original window bounds
                            targetForm.Bounds = (Rectangle)originalBounds;
                            _isMaximized = false;
                        }
                        break;

                    case ControlBoxType.MinimizeBox:
                        targetForm.WindowState = FormWindowState.Minimized;
                        break;
                }
            }
            // Ensure the base class's OnClick is still called to propagate the event chain
            base.OnClick(e);
        }

        #region Dispose
        private bool _isDispose = false;
        protected override void Dispose(bool disposing)
        {
            if (!_isDispose)
            {
                if (disposing)
                {
                    if (targetForm != null)
                    {
                        targetForm.Move -= CustomMove;
                    }

                    _isDispose = true;
                }
            }
            base.Dispose(disposing);
        }

        ~CustomControlBox()
        {
            Dispose(true);
        }
        #endregion
    }
}