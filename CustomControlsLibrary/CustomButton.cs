using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
    public class CustomButton : UserControl
    {
        #region Fields
        private Color _borderColor = Color.MediumSlateBlue;
        private int _borderSize = 2;
        private int _borderRadius = 20;
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
        public int BorderRadius
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
        [Browsable(true)]
        [Description("Sets the text displayed on the button")]
        public override string Text
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
            get => _buttonFont;
            set
            {
                _buttonFont = value;
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

            MouseEnter += CustomButton_MouseEnter;
            MouseLeave += CustomButton_MouseLeave;
            Paint += CustomButton_Paint;
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

        private void CustomButton_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle rectSurface = ClientRectangle;
            Rectangle rectBorder = Rectangle.Inflate(rectSurface, -_borderSize, -_borderSize);
            int smoothSize = _borderSize > 0 ? _borderSize : 1;

            using (GraphicsPath pathSurface = GetFigurePath(rectSurface, _borderRadius))
            using (GraphicsPath pathBorder = GetFigurePath(rectBorder, _borderRadius - _borderSize))
            using (Pen penSurface = new Pen(Parent.BackColor, smoothSize))
            using (Pen penBorder = new Pen(_borderColor, _borderSize))
            using (SolidBrush brushText = new SolidBrush(_textColor))
            {
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
        private GraphicsPath GetFigurePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();

            return path;
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _buttonFont?.Dispose();
                _icon?.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
