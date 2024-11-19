using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public class CustomCircularProgressBar : UserControl
    {
        private Timer _timer;
        private int _angle;
        private float _value;
        private int _maximum = 100;
        private Color _circleColor = Color.Red;
        private Color _textColor = Color.White;
        private int _lineWidth = 5;
        private int _circleSize = 100;
        private bool _showPercentage = true;
        private int _decimalPlaces = 0;
        private bool _isEnabled = true;

        public CustomCircularProgressBar()
        {
            DoubleBuffered = true;
            Size = new Size(_circleSize, _circleSize);

            SetStyle(ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            _timer = new Timer();
            _timer.Interval = 30; // Adjust speed here
            _timer.Tick += Timer_Tick;
            _timer.Start();

            SizeChanged += CustomCircularProgressBar_SizeChanged;
        }

        #region Properties
        [Category("Custom ProgressBar")]
        [DefaultValue(SmoothingMode.Default)]
        [Description("Specifies the smoothing mode for rendering, which affects the quality of lines and edges.")]
        public SmoothingMode SmoothingMode { get; set; }

        [Category("Custom ProgressBar")]
        [DefaultValue(InterpolationMode.Default)]
        [Description("Determines the interpolation mode used for scaling images, which impacts image quality.")]
        public InterpolationMode InterpolationMode { get; set; }

        [Category("Custom ProgressBar")]
        [DefaultValue(PixelOffsetMode.Default)]
        [Description("Sets the pixel offset mode, controlling pixel alignment for improved rendering accuracy.")]
        public PixelOffsetMode PixelOffsetMode { get; set; }

        [Category("Custom ProgressBar")]
        [DefaultValue(true)]
        [Description("Enables or disables double buffering to reduce flickering during rendering.")]
        public bool DoubleBuffereds
        {
            get => DoubleBuffered;
            set => DoubleBuffered = value;
        }

        [Category("Custom ProgressBar")]
        [Description("Color of the progress circle")]
        public Color CircleColor
        {
            get => _circleColor;
            set
            {
                _circleColor = value;
                Invalidate();
            }
        }

        [Category("Custom ProgressBar")]
        [Description("Color of the percentage text")]
        public Color TextColor
        {
            get => _textColor;
            set
            {
                _textColor = value;
                Invalidate();
            }
        }

        [Category("Custom ProgressBar")]
        [Description("Width of the circle line")]
        public int LineWidth
        {
            get => _lineWidth;
            set
            {
                _lineWidth = value;
                Invalidate();
            }
        }

        [Category("Custom ProgressBar")]
        [Description("Size of the progress circle")]
        public int CircleSize
        {
            get => _circleSize;
            set
            {
                _circleSize = Math.Max(12, value); // Prevents size from going below 12

                Size = new Size(_circleSize, _circleSize);
                Invalidate();
            }
        }


        [Category("Custom ProgressBar")]
        [Description("Size of the progress circle")]
        public new Size Size
        {
            get => base.Size;
            set
            {
                // Ensure width and height are at least 12
                int width = Math.Max(12, value.Width);
                int height = Math.Max(12, value.Height);

                // Set the base Size property
                base.Size = new Size(width, height);
                Invalidate(); // Redraws the control to reflect the new size
            }
        }

        [Category("Custom ProgressBar")]
        [Description("The current value of the progress bar")]
        [DefaultValue(0)]
        public float Value
        {
            get => _value;
            set
            {
                _value = Math.Max(0, Math.Min(value, Maximum));
                Invalidate();
            }
        }

        [Category("Custom ProgressBar")]
        [Description("The maximum value of the progress bar")]
        [DefaultValue(100)]
        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = Math.Max(1, value);
                Invalidate();
            }
        }

        [Category("Custom ProgressBar")]
        [Description("Whether to display the progress percentage")]
        [DefaultValue(true)]
        public bool ShowPercentage
        {
            get => _showPercentage;
            set
            {
                _showPercentage = value;
                Invalidate();
            }
        }

        [Category("Custom ProgressBar")]
        [Description("Whether to display the percentage as a decimal")]
        [DefaultValue(0)]
        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set
            {
                _decimalPlaces = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Custom ProgressBar")]
        [Description("Whether the progress bar is enabled")]
        [DefaultValue(true)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                Enabled = _isEnabled;
                Invalidate();
            }
        }

        [Category("Custom ProgressBar")]
        [Description("Gets or sets the font of the text displayed by the control.")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
            }
        }
        #endregion

        #region Method
        private void Timer_Tick(object sender, EventArgs e)
        {
            _angle += 5; // Adjust rotation speed here
            if (_angle >= 360) _angle = 0;
            Invalidate();
        }

        private void CustomCircularProgressBar_SizeChanged(object sender, EventArgs e)
        {
            float newFontSize = (float)Math.Max(10, Width / 8);
            base.Font = new Font(base.Font.FontFamily, newFontSize, base.Font.Style);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0) return;

            try
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode;
                e.Graphics.InterpolationMode = InterpolationMode;
                e.Graphics.PixelOffsetMode = PixelOffsetMode;

                e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // Background circle
                using (Pen bgPen = new Pen(Color.FromArgb(240, 240, 240), _lineWidth + 2))
                {
                    bgPen.StartCap = LineCap.Round;
                    bgPen.EndCap = LineCap.Round;
                    int radius = Math.Min(Width, Height) / 2 - _lineWidth;
                    e.Graphics.DrawArc(bgPen, _lineWidth, _lineWidth, radius * 2, radius * 2, 0, 360);
                }

                // Animated
                using (Pen animPen = new Pen(_circleColor, _lineWidth))
                {
                    animPen.StartCap = LineCap.Round;
                    animPen.EndCap = LineCap.Round;
                    int radius = Math.Min(Width, Height) / 2 - _lineWidth;
                    e.Graphics.DrawArc(animPen, _lineWidth, _lineWidth, radius * 2, radius * 2, _angle, 270);
                }

                // Draw percentage text
                if (_showPercentage)
                {
                    //string percentText = _showDecimal
                    //    ? $"{_value * _maximum / _maximum :F1}%"
                    //    : $"{(int)_value * _maximum / _maximum}%";

                    string percentText = string.Format($"{{0:F{_decimalPlaces}}}%", _value);
                    using (var textBrush = new SolidBrush(_textColor))
                    {
                        SizeF textSize = e.Graphics.MeasureString(percentText, Font);
                        float x = (ClientSize.Width - textSize.Width) / 2;
                        float y = (ClientSize.Height - textSize.Height) / 2;
                        e.Graphics.DrawString(percentText, Font, textBrush, x, y);
                    }
                }
            }
            catch { }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            // Keep the control square
            int min = Math.Min(Width, Height);
            Size = new Size(min, min);
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
                    if (_timer != null)
                    {
                        _timer.Stop();
                        _timer.Tick -= Timer_Tick;
                        _timer.Dispose();
                        _timer = null;
                    }
                }
                SizeChanged -= CustomCircularProgressBar_SizeChanged;

                _isDispose = true;
            }

            base.Dispose(disposing);
        }

        ~CustomCircularProgressBar()
        {
            Dispose(true);
        }
        #endregion
    }
}
