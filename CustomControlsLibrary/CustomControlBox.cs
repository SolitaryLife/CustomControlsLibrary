using System;
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
        private Cursor customCursor = Cursors.Hand;
        private Color iconColor = Color.White;
        private float iconScale = 1.0f;
        private float iconThickness = 1.5f;
        private Form targetForm = null;

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
        [DefaultValue(true)]
        [Description("Enables or disables double buffering to reduce flickering during rendering.")]
        public bool DoubleBuffereds
        {
            get => DoubleBuffered;
            set => DoubleBuffered = value;
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

                // Draw background
                using (var brush = new SolidBrush(isHovered ? hoverColor : normalColor))
                {
                    g.FillRectangle(brush, 0, 0, Width, Height);
                }

                // Draw Icon
                using (var pen = new Pen(iconColor, iconThickness))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;

                    // Calculate new padding according to scale
                    float basePadding = buttonSize / 4.0f;
                    float scaledPadding = basePadding / iconScale;

                    switch (boxType)
                    {
                        case ControlBoxType.CloseBox:
                            using (var path = new GraphicsPath())
                            {
                                float centerX = Width / 2.0f;
                                float centerY = Height / 2.0f;
                                float halfSize = ((Width - (2 * scaledPadding)) / 2.0f) * iconScale;

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
                            float rectSize = Width - (2 * scaledPadding);
                            float rectX = (Width - rectSize) / 2.0f;
                            float rectY = (Height - rectSize) / 2.0f;

                            if (targetForm == null)
                            {
                                targetForm = FindForm();
                            }
                            bool isMaximized = targetForm?.WindowState == FormWindowState.Maximized;

                            if (isMaximized)
                            {
                                // Draw a front (small) square.
                                float smallRectOffset = rectSize * 0.15f;  // distance between squares

                                using (var path = new GraphicsPath())
                                {
                                    RectangleF backRect = new RectangleF(
                                        rectX,
                                        rectY,
                                        rectSize - smallRectOffset,
                                        rectSize - smallRectOffset
                                    );
                                    path.AddRectangle(backRect);
                                    g.DrawPath(pen, path);
                                    path.AddRectangle(backRect);
                                    g.DrawPath(pen, path);
                                }

                                // Draw next to it (large)
                                using (var path = new GraphicsPath())
                                {
                                    RectangleF frontRect = new RectangleF(
                                        rectX + smallRectOffset,
                                        rectY + smallRectOffset,
                                        rectSize - smallRectOffset,
                                        rectSize - smallRectOffset
                                    );
                                    path.AddRectangle(frontRect);
                                    g.DrawPath(pen, path);
                                }
                            }
                            else
                            {
                                using (var path = new GraphicsPath())
                                {
                                    RectangleF rect = new RectangleF(rectX, rectY, rectSize, rectSize);
                                    path.AddRectangle(rect);
                                    g.DrawPath(pen, path);
                                }
                            }
                            break;

                        case ControlBoxType.MinimizeBox:

                            using (var path = new GraphicsPath())
                            {
                                float lineY = Height / 2.0f;
                                float lineWidth = (Width - (2 * scaledPadding)) * iconScale;
                                float lineX = (Width - lineWidth) / 2.0f;

                                path.StartFigure();
                                path.AddLine(
                                    lineX, lineY,
                                    lineX + lineWidth, lineY
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

        protected override void OnClick(EventArgs e)
        {
            if (targetForm == null)
            {
                targetForm = FindForm();
            }
            if (targetForm != null)
            {
                switch (boxType)
                {
                    case ControlBoxType.CloseBox:
                        targetForm.Close();
                        break;

                    case ControlBoxType.MaximizeBox:
                        if (targetForm.WindowState == FormWindowState.Normal)
                            targetForm.WindowState = FormWindowState.Maximized;
                        else
                            targetForm.WindowState = FormWindowState.Normal;
                        break;

                    case ControlBoxType.MinimizeBox:
                        targetForm.WindowState = FormWindowState.Minimized;
                        break;
                }
            }
            base.OnClick(e);
        }

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{

            //}
            base.Dispose(disposing);
        }

        ~CustomControlBox()
        {
            Dispose(true);
        }
        #endregion
    }
}