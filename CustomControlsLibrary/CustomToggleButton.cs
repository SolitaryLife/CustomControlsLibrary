using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public class CustomToggleButton : UserControl
    {
        // Fields
        private bool _checked = false;
        private Color _onBackColor = Color.MediumSlateBlue;
        private Color _onToggleColor = Color.WhiteSmoke;
        private Color _offBackColor = Color.Gray;
        private Color _offToggleColor = Color.Gainsboro;
        private bool _solidStyle = true;
        private int _toggleSize = 20;
        private int _padding = 10;
        private bool _isHovered = false;
        private bool _useAnimation = true;
        private int _animationInterval = 1;
        private Timer animationTimer;

        // Events
        [Category("Custom Toggle Actions")]
        [Description("Occurs when the toggle state changes")]
        public event EventHandler CheckedChanged;

        #region Properties
        // Properties
        // Properties
        [Category("Custom Toggle")]
        [DefaultValue(SmoothingMode.Default)]
        [Description("Specifies the smoothing mode for rendering, which affects the quality of lines and edges.")]
        public SmoothingMode SmoothingMode { get; set; }

        [Category("Custom Toggle")]
        [DefaultValue(InterpolationMode.Default)]
        [Description("Determines the interpolation mode used for scaling images, which impacts image quality.")]
        public InterpolationMode InterpolationMode { get; set; }

        [Category("Custom Toggle")]
        [DefaultValue(PixelOffsetMode.Default)]
        [Description("Sets the pixel offset mode, controlling pixel alignment for improved rendering accuracy.")]
        public PixelOffsetMode PixelOffsetMode { get; set; }

        [Category("Custom Toggle")]
        [DefaultValue(true)]
        [Description("Enables or disables double buffering to reduce flickering during rendering.")]
        public bool DoubleBuffereds
        {
            get => DoubleBuffered;
            set => DoubleBuffered = value;
        }

        [Category("Custom Toggle")]
        [Description("Gets or sets whether the toggle is checked")]
        public bool Checked
        {
            get => _checked;
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        [Category("Custom Toggle")]
        [Description("Gets or sets the background color when toggle is ON")]
        public Color OnBackColor
        {
            get => _onBackColor;
            set
            {
                _onBackColor = value;
                Invalidate();
            }
        }

        [Category("Custom Toggle")]
        [Description("Gets or sets the toggle color when ON")]
        public Color OnToggleColor
        {
            get => _onToggleColor;
            set
            {
                _onToggleColor = value;
                Invalidate();
            }
        }

        [Category("Custom Toggle")]
        [Description("Gets or sets the background color when toggle is OFF")]
        public Color OffBackColor
        {
            get => _offBackColor;
            set
            {
                _offBackColor = value;
                Invalidate();
            }
        }

        [Category("Custom Toggle")]
        [Description("Gets or sets the toggle color when OFF")]
        public Color OffToggleColor
        {
            get => _offToggleColor;
            set
            {
                _offToggleColor = value;
                Invalidate();
            }
        }

        [Category("Custom Toggle")]
        [Description("Gets or sets whether to use solid style or gradient")]
        public bool SolidStyle
        {
            get => _solidStyle;
            set
            {
                _solidStyle = value;
                Invalidate();
            }
        }

        //[Category("Custom Toggle")]
        //[DefaultValue(20)]
        //[Description("Gets or sets the size of the toggle button")]
        //public int ToggleSize
        //{
        //    get => _toggleSize;
        //    set
        //    {
        //        if (value >= 10)
        //        {
        //            _toggleSize = value;
        //            Invalidate();
        //        }
        //    }
        //}

        //[Category("Custom Toggle")]
        //[DefaultValue(2)]
        //[Description("Gets or sets the padding between toggle and border")]
        //public int TogglePadding
        //{
        //    get => _padding;
        //    set
        //    {
        //        if (value >= 0)
        //        {
        //            _padding = Math.Min((Height / 2) - 1, value);
        //            Invalidate();
        //        }
        //    }
        //}

        [Category("Custom Toggle")]
        [DefaultValue(true)]
        [Description("Gets or sets whether to use animation effect")]
        public bool UseAnimation
        {
            get => _useAnimation;
            set => _useAnimation = value;
        }

        [Category("Custom Toggle")]
        [DefaultValue(1)]
        [Description("Gets or sets the animation speed (1-10)")]
        public int AnimationInterval
        {
            get => _animationInterval;
            set
            {
                if (value >= 1 && value <= 10)
                {
                    _animationInterval = value;
                    if (animationTimer != null)
                        animationTimer.Interval = value;
                }
            }
        }
        #endregion

        // Constructor
        public CustomToggleButton()
        {
            DoubleBuffered = true;

            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);

            Size = new Size(50, 25);
            BackColor = Color.Transparent;
            Cursor = Cursors.Hand;

            // Initialize animation timer
            animationTimer = new Timer();
            animationTimer.Interval = _animationInterval;
            animationTimer.Tick += AnimationTimer_Tick;
        }

        // Animation timer event handler
        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
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

                // Calculate dimensions
                //var toggleSize = Math.Min(Height - (2 * _padding), _toggleSize);
                var toggleSize = Math.Min(Height - (2 * _padding), Height - 5);
                var backRectangle = new Rectangle(0, 0, Width, Height);

                // Draw background
                using (var path = GetRoundedRectangle(backRectangle, Height / 2))
                {
                    if (_solidStyle)
                    {
                        g.FillPath(new SolidBrush(_checked ? _onBackColor : _offBackColor), path);
                    }
                    else
                    {
                        // Gradient style
                        using (var brush = new LinearGradientBrush(
                            backRectangle,
                            _checked ? _onBackColor : _offBackColor,
                            Color.FromArgb(50, _checked ? _onBackColor : _offBackColor),
                            LinearGradientMode.Vertical))
                        {
                            g.FillPath(brush, path);
                        }
                    }
                }

                // Calculate toggle position
                float togglePosition;
                if (_checked)
                    togglePosition = Width - toggleSize - _padding;
                else
                    togglePosition = _padding;

                // Draw toggle button
                var toggleRectangle = new Rectangle((int)togglePosition, _padding, toggleSize, toggleSize);

                using (var path = GetRoundedRectangle(toggleRectangle, toggleSize / 2))
                {
                    // Add shadow effect
                    if (_isHovered)
                    {
                        using (var shadowBrush = new SolidBrush(Color.FromArgb(20, Color.Black)))
                        {
                            g.FillPath(shadowBrush, path);
                        }
                    }

                    g.FillPath(new SolidBrush(_checked ? _onToggleColor : _offToggleColor), path);
                }
            }
            catch { }
        }

        private GraphicsPath GetRoundedRectangle(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }

        #region Events
        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;
            base.OnClick(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            base.OnMouseLeave(e);
            Invalidate();
        }
        #endregion

        #region Dispose
        private bool _isDispose = false;
        protected override void Dispose(bool disposing)
        {
            if (!_isDispose)
            {
                if (disposing && animationTimer != null)
                {
                    animationTimer?.Dispose();
                }
                _isDispose = true;
            }

            base.Dispose(disposing);
        }

        ~CustomToggleButton()
        {
            Dispose(true);
        }
        #endregion
    }
}
