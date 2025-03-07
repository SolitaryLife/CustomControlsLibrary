﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using static CustomControlsLibrary.ObjectConverter;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public class CustomButton : UserControl
    {
        #region Fields
        private Color _borderColor = Color.MediumSlateBlue;
        private Color _borderHover = Color.Transparent;
        private int _borderSize = 2;
        private BorderRadius _borderRadius = new BorderRadius(0);
        private Color _buttonColor = Color.MediumSlateBlue;
        private Color _buttonHoverColor = Color.LightSkyBlue;
        private Color _textHoverColor = Color.Black;
        private Color _textColor = Color.White;
        private bool _isHovering = false;
        private bool _underline = false;
        private int _underlineThickness = 2;

        private ButtonType _buttonType = ButtonType.Normal;
        private bool _isToggled = false;
        private Color _toggledColor = Color.DarkSlateBlue;
        private Color _toggledTextColor = Color.White;
        private Color _toggledBorderColor = Color.DarkSlateBlue;

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

        private float _textYoffset = 0;

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

        public enum ButtonType
        {
            Normal,
            Toggle,
            Radio
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
        [DefaultValue(CompositingQuality.Default)]
        [Description("Gets or sets the compositing quality level for drawing operations. Compositing quality determines how drawing operations are blended or composited.")]
        public CompositingQuality CompositingQuality { get; set; }

        [Category("Custom Button")]
        [DefaultValue(true)]
        [Description("Enables or disables double buffering to reduce flickering during rendering.")]
        public bool DoubleBuffereds
        {
            get => DoubleBuffered;
            set => DoubleBuffered = value;
        }

        [Category("Custom Button")]
        [Description("Sets the vertical offset of the text from its default position")]
        public float TextYoffset
        {
            get => _textYoffset;
            set
            {
                _textYoffset = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the type of button (Normal/Toggle)")]
        public ButtonType ButtonTypes
        {
            get => _buttonType;
            set
            {
                _buttonType = value;
                if (_buttonType == ButtonType.Normal)
                {
                    _isToggled = false;
                }
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Gets or sets the toggled state for Toggle type buttons")]
        public bool Checked
        {
            get => _isToggled;
            set
            {
                switch (_buttonType)
                {
                    case ButtonType.Toggle:

                        if (_isToggled == value) return;

                        _isToggled = value;

                        CheckedValueChanged?.Invoke(this, _isToggled);
                        Invalidate();
                        break;
                    case ButtonType.Radio:

                        if (_isToggled == value) return;

                        _isToggled = value;

                        if (_isToggled)
                        {
                            Parent?.Controls?.OfType<CustomButton>()
                            ?.Where(w => w != this && w.ButtonTypes == ButtonType.Radio && w.Checked == true)
                            .ToList()
                            .ForEach(c => c.Checked = false);
                        }

                        if (_isToggled)
                        {
                            CheckedValueChanged?.Invoke(this, _isToggled);
                        }
                        Invalidate();
                        break;
                }
            }
        }

        [Category("Custom Button")]
        [Description("Sets the background color when button is toggled")]
        public Color ToggledColor
        {
            get => _toggledColor;
            set
            {
                _toggledColor = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the text color when button is toggled")]
        public Color ToggledTextColor
        {
            get => _toggledTextColor;
            set
            {
                _toggledTextColor = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the border color when button is toggled")]
        public Color ToggledBorderColor
        {
            get => _toggledBorderColor;
            set
            {
                _toggledBorderColor = value;
                Invalidate();
            }
        }


        [Category("Custom Button")]
        [Description("Enable or disable underline")]
        public bool Underline
        {
            get => _underline;
            set
            {
                _underline = value;
                Invalidate();
            }
        }

        [Category("Custom Button")]
        [Description("Sets the thickness of the underline")]
        public int UnderlineThickness
        {
            get => _underlineThickness;
            set
            {
                _underlineThickness = Math.Max(1, value);
                Invalidate();
            }
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
                _icon?.Dispose();

                if (value != null)
                {
                    _icon = value.Clone() as Image;
                }
                else
                {
                    _icon = null;
                }

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


        /// <summary>
        /// Gets or sets the border color of the button when the mouse hovers over it.
        /// Changing this property invalidates the control to trigger a repaint.
        /// </summary>
        [Category("Custom Button")]
        [Description("Specifies the border color of the button when the mouse hovers over it.")]
        public Color BorderHover
        {
            get => _borderHover;
            set
            {
                _borderHover = value;
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
        [Description("Sets the Texts color when the mouse hovers")]
        public Color TextHoverColor
        {
            get => _textHoverColor;
            set
            {
                _textHoverColor = value;
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

        // [Category("Custom Button")]
        [Description("Sets the text displayed on the button")]
        [DefaultValue("Button")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => base.Text;
            set
            {
                _buttonText = value;
                base.Text = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Indicates whether the control is enabled.
        /// </summary>
        [Description("Indicates whether the control is enabled in the designer.")]
        public new bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
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
            Size = new Size(150, 40);
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 12F);
            _textYoffset = 0;

            _autoEllipsis = false;
            _multiline = false;
            _iconSpacing = 5;
            _iconTextAlignment = IconTextAlignment.TextBeforeIcon;
            _iconTextLayout = IconTextLayout.Horizontal;

            _underline = false;
            _underlineThickness = 2;

            Clicked = CustomClicked;
            Click += Clicked;

            internalHandlerMouseEnter = CustomButton_MouseEnter;
            internalHandlerMouseLeave = CustomButton_MouseLeave;

            MouseEnter += internalHandlerMouseEnter;
            MouseLeave += internalHandlerMouseLeave;
        }


        #endregion

        #region Event Handlers
        private event EventHandler Clicked;
        private void CustomClicked(object sender, EventArgs e)
        {
            switch (_buttonType)
            {
                case ButtonType.Toggle:
                    Checked = !Checked;
                    break;
                case ButtonType.Radio:
                    if (!Checked)
                    {
                        Checked = true;

                        Parent?.Controls?.OfType<CustomButton>()
                            ?.Where(w => w != this && w.ButtonTypes == ButtonType.Radio && w.Checked == true)
                            .ToList()
                            .ForEach(c => c.Checked = false);
                    }
                    break;
            }
        }

        /// <summary>
        /// Occurs when the checked value changes.
        /// </summary>
        [Category("Custom Button")]
        [Description("Occurs when the checked value is changed.")]
        public event EventHandler<bool> CheckedValueChanged;

        private EventHandler internalHandlerMouseEnter;
        private EventHandler internalHandlerMouseLeave;

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
                g.CompositingQuality = CompositingQuality;

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                RectangleF rectSurface = ClientRectangle;
                RectangleF rectBorder = RectangleF.Inflate(rectSurface, -_borderSize, -_borderSize);
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
                {
                    // Determine colors based on button state
                    Color currentBackColor;
                    Color currentBorderColor;
                    Color currentTextColor;

                    if (!Enabled)
                    {
                        currentBackColor = Color.FromArgb(169, 169, 169);
                        currentBorderColor = Color.FromArgb(169, 169, 169);
                        currentTextColor = Color.FromArgb(141, 141, 141);
                    }
                    else
                    {
                        switch (_buttonType)
                        {
                            case ButtonType.Radio:
                            case ButtonType.Toggle:
                            default:
                                if (_isToggled)
                                {
                                    currentBackColor = _toggledColor;
                                    currentBorderColor = _toggledBorderColor;
                                    currentTextColor = _toggledTextColor;
                                }
                                else
                                {
                                    currentBackColor = _isHovering ? _buttonHoverColor : _buttonColor;
                                    currentBorderColor = _isHovering ? _borderHover : _borderColor;
                                    currentTextColor = _isHovering ? _textHoverColor : _textColor;
                                }
                                break;
                        }
                    }


                    using (Pen penSurface = new Pen(Parent.BackColor, smoothSize))
                    using (Pen penBorder = new Pen(!_underline ? currentBorderColor : Color.Transparent, _borderSize))
                    using (SolidBrush brushText = new SolidBrush(currentTextColor))
                    {
                        penSurface.StartCap = LineCap.Round;
                        penSurface.EndCap = LineCap.Round;
                        penSurface.LineJoin = LineJoin.Round;

                        penBorder.StartCap = LineCap.Round;
                        penBorder.EndCap = LineCap.Round;
                        penBorder.LineJoin = LineJoin.Round;

                        // Draw button surface
                        g.FillPath(new SolidBrush(currentBackColor), pathSurface);

                        // Draw border
                        if (_borderSize > 0)
                            g.DrawPath(penBorder, pathBorder);

                        // Calculate content area
                        var startArea = _underline ? _textPadding : 5;
                        RectangleF contentRect = new RectangleF
                        (
                            _underline ? 0 : _borderSize,
                            _underline ? 0 : _borderSize,
                            Width - (_underline ? _textPadding * 2 : _borderSize * 2),
                            Height - (_underline ? _textPadding * 2 : _borderSize * 2)
                        );

                        // Draw content (text and icon)
                        DrawTextAndIcon(g, contentRect, brushText);

                        if (_underline)
                        {
                            float underlineY;

                            // Determine the Y position for underline based on text position
                            if (_icon != null && _iconTextLayout == IconTextLayout.Vertical)
                            {
                                // If icon is in vertical layout, adjust underline position
                                underlineY = contentRect.Bottom - _underlineThickness;
                            }
                            else
                            {
                                // Default vertical center positioning
                                underlineY = contentRect.Bottom - _underlineThickness / 2;
                            }

                            var underlineColor = currentBorderColor;

                            using (Pen underlinePen = new Pen(underlineColor, _underlineThickness))
                            {
                                float underlineX = 0;
                                float underlineWidth = Width;

                                g.DrawLine(underlinePen,
                                    underlineX,
                                    underlineY,
                                    underlineWidth,
                                    underlineY);
                            }
                        }
                    }
                }
            }
            catch { }
        }
        #endregion

        #region Draw Text And Icon
        private void DrawTextAndIcon(Graphics g, RectangleF contentRect, Brush textBrush)
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

                string empty = _icon == null ? "" : "_";
                empty = string.IsNullOrEmpty(_buttonText) ? string.Empty : "_";
                SizeF textSize = g.MeasureString(_buttonText + empty, _buttonFont);
                RectangleF textRect = contentRect;
                RectangleF iconRect = RectangleF.Empty;

                if (_icon != null)
                {
                    // Adjust RectangleFs based on layout
                    if (_iconTextLayout == IconTextLayout.Horizontal)
                    {
                        int totalWidth = (int)textSize.Width + _iconSpacing + _iconSize.Width;
                        float startX = (Width - totalWidth) / 2;

                        if (_iconTextAlignment == IconTextAlignment.IconBeforeText)
                        {
                            iconRect = new RectangleF(startX, (Height - _iconSize.Height) / 2, _iconSize.Width, _iconSize.Height);
                            textRect = new RectangleF(startX + _iconSize.Width + _iconSpacing, contentRect.Y + _textYoffset, (int)textSize.Width, contentRect.Height);
                        }
                        else
                        {
                            textRect = new RectangleF(startX, contentRect.Y + _textYoffset, (int)textSize.Width, contentRect.Height);
                            iconRect = new RectangleF(startX + (int)textSize.Width + _iconSpacing, (Height - _iconSize.Height) / 2, _iconSize.Width, _iconSize.Height);
                        }
                    }
                    else // Vertical layout
                    {
                        int totalHeight = (int)textSize.Height + _iconSpacing + _iconSize.Height;
                        float startY = (Height - totalHeight) / 2;

                        if (_iconTextAlignment == IconTextAlignment.IconBeforeText)
                        {
                            iconRect = new RectangleF((Width - _iconSize.Width) / 2, startY, _iconSize.Width, _iconSize.Height);
                            textRect = new RectangleF(contentRect.X, startY + _iconSize.Height + _iconSpacing + _textYoffset, contentRect.Width, (int)textSize.Height);
                        }
                        else
                        {
                            textRect = new RectangleF(contentRect.X, startY + _textYoffset, contentRect.Width, (int)textSize.Height);
                            iconRect = new RectangleF((Width - _iconSize.Width) / 2, startY + (int)textSize.Height + _iconSpacing, _iconSize.Width, _iconSize.Height);
                        }
                    }

                    // Draw icon
                    if (_icon != null)
                    {
                        g.DrawImage(_icon, iconRect);
                    }
                }
                else
                {
                    // ปรับตำแหน่งข้อความเมื่อไม่มีไอคอน
                    textRect = new RectangleF(
                        contentRect.X,
                        contentRect.Y + _textYoffset,
                        contentRect.Width,
                        contentRect.Height);
                }

                // Draw text
                g.DrawString(_buttonText, _buttonFont, textBrush, textRect, sf);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Simulates a click event on the button by directly invoking the Click event handler.
        /// </summary>
        /// <remarks>
        /// This method is useful for triggering the Click event programmatically,
        /// as if the user had clicked the button manually. It bypasses user input
        /// and directly raises the event with empty event arguments.
        /// </remarks>
        public void PerformClick()
        {
            base.OnClick(EventArgs.Empty);
        }
        #endregion

        #region Helper Methods Graphics
        private GraphicsPath GetFigurePath(RectangleF rect, int radiusTopLeft = 0, int radiusTopRight = 0, int radiusBottomLeft = 0, int radiusBottomRight = 0)
        {
            GraphicsPath path = new GraphicsPath();

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
                    Click -= Clicked;
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
