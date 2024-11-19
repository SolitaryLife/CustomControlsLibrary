using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public partial class CustomButton : UserControl
    {
        #region Fields
        private Color _borderColor = Color.MediumSlateBlue;
        private int _borderSize = 2;
        private BorderRadius _borderRadius;
        private Color _buttonColor = Color.MediumSlateBlue;
        private Color _buttonHoverColor = Color.LightSkyBlue;
        private Color _textColor = Color.White;
        private bool _isHovering = false;

        private bool _autoEllipsis = false;
        private bool _multiline = false;
        private Image _icon = null;
        private Size _iconSize = new Size(24, 24);
        private int _iconSpacing = 5;
        private IconTextAlignment _iconTextAlignment = IconTextAlignment.TextBeforeIcon;
        private IconTextLayout _iconTextLayout = IconTextLayout.Horizontal;

        private string _buttonText = "Button";
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
        private Font _buttonFont = new Font("Segoe UI", 12F);
        private int _textPadding = 5;
        private EventHandler internalHandlerMouseEnter;
        private EventHandler internalHandlerMouseLeave;

        #region Enums
        public enum IconTextAlignment
        {
            TextBeforeIcon,
            IconBeforeText
        }

        public enum IconTextLayout
        {
            Horizontal,
            Vertical
        }
        #endregion

        #endregion

        #region Properties
        [Category("Custom Button")]
        [DefaultValue(SmoothingMode.Default)]
        [Description("Specifies the smoothing mode for rendering, which affects the quality of lines and edges.")]
        public SmoothingMode SmoothingMode { get; set; }

        [Category("Custom Button")]
        [DefaultValue(InterpolationMode.Default)]
        [Description("Determines the interpolation mode used for scaling images, which impacts image quality.")]
        public InterpolationMode InterpolationMode { get; set; }

        [Category("Custom Button")]
        [DefaultValue(PixelOffsetMode.Default)]
        [Description("Sets the pixel offset mode, controlling pixel alignment for improved rendering accuracy.")]
        public PixelOffsetMode PixelOffsetMode { get; set; }

        [Category("Custom Button")]
        [DefaultValue(true)]
        [Description("Enables or disables double buffering to reduce flickering during rendering.")]
        public bool DoubleBuffereds
        {
            get => DoubleBuffered;
            set => DoubleBuffered = value;
        }

        [Category("Custom Button")]
        [Description("Enable or disable text wrapping")]
        public bool AutoEllipsis
        {
            get => _autoEllipsis;
            set
            {
                _autoEllipsis = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Enable or disable multiline text")]
        public bool Multiline
        {
            get => _multiline;
            set
            {
                _multiline = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Button icon")]
        public Image Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Icon size")]
        public Size IconSize
        {
            get => _iconSize;
            set
            {
                _iconSize = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Spacing between icon and text")]
        public int IconSpacing
        {
            get => _iconSpacing;
            set
            {
                _iconSpacing = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Icon and text alignment")]
        public IconTextAlignment IconTextAlign
        {
            get => _iconTextAlignment;
            set
            {
                _iconTextAlignment = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Icon and text layout (Horizontal/Vertical)")]
        public IconTextLayout IconTextLayouts
        {
            get => _iconTextLayout;
            set
            {
                _iconTextLayout = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the button's border color")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the border thickness")]
        public int BorderSize
        {
            get => _borderSize;
            set
            {
                _borderSize = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the corner radius of the button")]
        public BorderRadius Borderradius
        {
            get => _borderRadius;
            set
            {
                _borderRadius = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the background color of the button")]
        public Color ButtonColor
        {
            get => _buttonColor;
            set
            {
                _buttonColor = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the background color when the mouse hovers")]
        public Color ButtonHoverColor
        {
            get => _buttonHoverColor;
            set
            {
                _buttonHoverColor = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the text color")]
        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the text displayed on the button")]
        public  string Texts
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the alignment of the text")]
        public ContentAlignment TextAlign
        {
            get => _textAlign;
            set
            {
                _textAlign = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the padding between the text and button border")]
        public int TextPadding
        {
            get => _textPadding;
            set
            {
                _textPadding = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the font style of the text")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                _buttonFont = value;
                base.Font = value;
                Invalidate();
            }
        }
        #endregion

        #region Constructor
        public CustomButton()
        {
            DoubleBuffered = true;
            MinimumSize = new Size(100, 40);
            Size = new Size(150, 40);
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 12F);

            _autoEllipsis = false;
            _multiline = false;
            _iconSpacing = 5;
            _iconTextAlignment = IconTextAlignment.TextBeforeIcon;
            _iconTextLayout = IconTextLayout.Horizontal;

            internalHandlerMouseEnter = CustomButton_MouseEnter;
            internalHandlerMouseLeave = CustomButton_MouseLeave;

            MouseEnter += internalHandlerMouseEnter;
            MouseLeave += internalHandlerMouseLeave;
        }


        #endregion

        #region Event Handlers
        private void CustomButton_MouseEnter(object sender, EventArgs e)
        {
            _isHovering = true;
            Invalidate();
        }

        private void CustomButton_MouseLeave(object sender, EventArgs e)
        {
            _isHovering = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0) return;

            try
            {
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode;
                g.InterpolationMode = InterpolationMode;
                g.PixelOffsetMode = PixelOffsetMode;

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                Rectangle rectSurface = ClientRectangle;
                Rectangle rectBorder = Rectangle.Inflate(rectSurface, -_borderSize, -_borderSize);
                int smoothSize = _borderSize > 0 ? _borderSize : 1;

                using (GraphicsPath pathSurface = GetFigurePath(rectSurface,
                        _borderRadius.TopLeft,
                        _borderRadius.TopRight,
                        _borderRadius.BottomLeft,
                        _borderRadius.BottomRight))
                using (GraphicsPath pathBorder = GetFigurePath(rectBorder,
                        _borderRadius.TopLeft - _borderSize,
                        _borderRadius.TopRight - _borderSize,
                        _borderRadius.BottomLeft - _borderSize,
                        _borderRadius.BottomRight - _borderSize))
                using (Pen penSurface = new Pen(Parent.BackColor, smoothSize))
                using (Pen penBorder = new Pen(_borderColor, _borderSize))
                using (SolidBrush brushText = new SolidBrush(_textColor))
                {
                    penSurface.StartCap = LineCap.Round;
                    penSurface.EndCap = LineCap.Round;
                    penSurface.LineJoin = LineJoin.Round;

                    penBorder.StartCap = LineCap.Round;
                    penBorder.EndCap = LineCap.Round;
                    penBorder.LineJoin = LineJoin.Round;

                    // Draw button surface
                    g.FillPath(new SolidBrush(_isHovering ? _buttonHoverColor : _buttonColor), pathSurface);

                    // Draw border
                    if (_borderSize > 0)
                        g.DrawPath(penBorder, pathBorder);

                    // Calculate content area
                    Rectangle contentRect = new Rectangle(
                        _textPadding,
                        _textPadding,
                        Width - (_textPadding * 2),
                        Height - (_textPadding * 2)
                    );

                    // Draw content (text and icon)
                    DrawTextAndIcon(g, contentRect, brushText);
                }
            }
            catch { }
        }
        #endregion

        #region Draw Text And Icon
        private void DrawTextAndIcon(Graphics g, Rectangle contentRect, Brush textBrush)
        {
            // Prepare text format
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                if (_multiline)
                {
                    sf.FormatFlags &= ~StringFormatFlags.NoWrap;
                }
                else
                {
                    sf.FormatFlags |= StringFormatFlags.NoWrap;
                }

                if (_autoEllipsis)
                {
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                }

                // Calculate sizes and positions
                SizeF textSize = g.MeasureString(_buttonText, _buttonFont);
                Rectangle textRect = contentRect;
                Rectangle iconRect = Rectangle.Empty;

                if (_icon != null)
                {
                    // Adjust rectangles based on layout
                    if (_iconTextLayout == IconTextLayout.Horizontal)
                    {
                        int totalWidth = (int)textSize.Width + _iconSpacing + _iconSize.Width;
                        int startX = (Width - totalWidth) / 2;

                        if (_iconTextAlignment == IconTextAlignment.IconBeforeText)
                        {
                            iconRect = new Rectangle(startX, (Height - _iconSize.Height) / 2, _iconSize.Width, _iconSize.Height);
                            textRect = new Rectangle(startX + _iconSize.Width + _iconSpacing, contentRect.Y, (int)textSize.Width, contentRect.Height);
                        }
                        else
                        {
                            textRect = new Rectangle(startX, contentRect.Y, (int)textSize.Width, contentRect.Height);
                            iconRect = new Rectangle(startX + (int)textSize.Width + _iconSpacing, (Height - _iconSize.Height) / 2, _iconSize.Width, _iconSize.Height);
                        }
                    }
                    else // Vertical layout
                    {
                        int totalHeight = (int)textSize.Height + _iconSpacing + _iconSize.Height;
                        int startY = (Height - totalHeight) / 2;

                        if (_iconTextAlignment == IconTextAlignment.IconBeforeText)
                        {
                            iconRect = new Rectangle((Width - _iconSize.Width) / 2, startY, _iconSize.Width, _iconSize.Height);
                            textRect = new Rectangle(contentRect.X, startY + _iconSize.Height + _iconSpacing, contentRect.Width, (int)textSize.Height);
                        }
                        else
                        {
                            textRect = new Rectangle(contentRect.X, startY, contentRect.Width, (int)textSize.Height);
                            iconRect = new Rectangle((Width - _iconSize.Width) / 2, startY + (int)textSize.Height + _iconSpacing, _iconSize.Width, _iconSize.Height);
                        }
                    }

                    // Draw icon
                    if (_icon != null)
                    {
                        g.DrawImage(_icon, iconRect);
                    }
                }

                // Draw text
                g.DrawString(_buttonText, _buttonFont, textBrush, textRect, sf);
            }
        }
        #endregion

        #region Helper Methods
        private GraphicsPath GetFigurePath(Rectangle rect, int radiusTopLeft = 0, int radiusTopRight = 0, int radiusBottomLeft = 0, int radiusBottomRight = 0)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                float curveSize = radiusTopLeft;
                path.StartFigure();

                // Top Left
                if (radiusTopLeft > 0)
                    path.AddArc(rect.X, rect.Y, radiusTopLeft * 2, radiusTopLeft * 2, 180, 90);
                else
                    path.AddLine(rect.X, rect.Y, rect.X, rect.Y);

                // Top Right
                if (radiusTopRight > 0)
                    path.AddArc(rect.Right - (radiusTopRight * 2), rect.Y, radiusTopRight * 2, radiusTopRight * 2, 270, 90);
                else
                    path.AddLine(rect.Right, rect.Y, rect.Right, rect.Y);

                // Bottom Right
                if (radiusBottomRight > 0)
                    path.AddArc(rect.Right - (radiusBottomRight * 2), rect.Bottom - (radiusBottomRight * 2),
                        radiusBottomRight * 2, radiusBottomRight * 2, 0, 90);
                else
                    path.AddLine(rect.Right, rect.Bottom, rect.Right, rect.Bottom);

                // Bottom Left
                if (radiusBottomLeft > 0)
                    path.AddArc(rect.X, rect.Bottom - (radiusBottomLeft * 2),
                        radiusBottomLeft * 2, radiusBottomLeft * 2, 90, 90);
                else
                    path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom);

                path.CloseFigure();
                return path;
            }   
        }
        #endregion

        #region Overridden Methods
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
        #endregion

        #region Dispose
        private bool _isDispose = false;
        protected override void Dispose(bool disposing)
        {
            if (!_isDispose)
            {
                if (disposing)
                {
                    MouseEnter -= internalHandlerMouseEnter;
                    MouseLeave -= internalHandlerMouseLeave;
                    _buttonFont?.Dispose();
                    _icon?.Dispose();
                }
                _isDispose = true;
            }
            
            base.Dispose(disposing);
        }
        ~CustomButton()
        {
            Dispose(true);
        }
        #endregion
    }
}
