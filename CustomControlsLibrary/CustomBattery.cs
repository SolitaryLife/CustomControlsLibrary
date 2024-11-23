using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public class CustomBattery : UserControl
    {
        public enum BatteryShape
        {
            Rectangle,
            RoundedRectangle,
            Capsule
        }

        public enum AnimationType
        {
            Linear,
            Smooth,
            Bounce
        }

        private int _value = 100;
        private int valueChange = 100;
        private Color _batteryColor = Color.LimeGreen;
        private Color _outlineColor = Color.Black;
        private int _borderOutLine = 10;
        private bool _showPercentage = true;
        private Font _percentageFont;
        private Color _textColor = Color.Black;

        private Timer animationTimer = new Timer();
        private bool _enableAnimation = true;
        private int targetValue = 0;
        private const int ANIMATION_STEP = 1;
        private const int ANIMATION_INTERVAL = 10;
        private EventHandler _eventTick;

        private BatteryShape _batteryShape = BatteryShape.Rectangle;
        private AnimationType _animationType = AnimationType.Linear;
        private float _cornerRadius = 15f;
        private bool _glowEffect = false;
        private Color _glowColor = Color.Yellow;
        private bool _charging = false;
        // private readonly Timer _chargingAnimationTimer;
        // private int _chargingAnimationFrame = 0;

        public CustomBattery()
        {
            DoubleBuffered = true;

            SetStyle(ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint, true);

            this.MinimumSize = new Size(30, 15);
            this.Size = new Size(330, 150);
            this._percentageFont = new Font(this.Font.FontFamily, 8f);
            this.BackColor = Color.Transparent;

            animationTimer.Interval = ANIMATION_INTERVAL;
            _eventTick = AnimationTimer_Tick;
            animationTimer.Tick += _eventTick;

            EnableAnimation = true;

            // _chargingAnimationTimer = new Timer { Interval = 100 };
            // _chargingAnimationTimer.Tick += ChargingAnimationTimer_Tick;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            float step = ANIMATION_STEP;

            if (valueChange != targetValue)
            {
                switch (_animationType)
                {
                    case AnimationType.Smooth:
                        step = Math.Max(1, Math.Abs(targetValue - valueChange) / 10f);
                        break;
                    case AnimationType.Bounce:
                        step = Math.Max(1, Math.Abs(targetValue - valueChange) / 5f);
                        if (Math.Abs(targetValue - valueChange) < step * 2)
                            step = Math.Max(1, step / 2);
                        break;
                }

                if (valueChange < targetValue)
                {
                    valueChange = Math.Min(valueChange + (int)step, targetValue);
                }
                else if (valueChange > targetValue)
                {
                    valueChange = Math.Max(valueChange - (int)step, targetValue);
                }
            }

            //if (valueChange == targetValue)
            //{
            //    animationTimer.Stop();
            //    //OnValueChanged(EventArgs.Empty);
            //}

            Invalidate();
        }

        // Properties
        #region Properties
        [Category("Custom Battery")]
        [DefaultValue(SmoothingMode.Default)]
        [Description("Specifies the smoothing mode for rendering, which affects the quality of lines and edges.")]
        public SmoothingMode SmoothingMode { get; set; }

        [Category("Custom Battery")]
        [DefaultValue(InterpolationMode.Default)]
        [Description("Determines the interpolation mode used for scaling images, which impacts image quality.")]
        public InterpolationMode InterpolationMode { get; set; }

        [Category("Custom Battery")]
        [DefaultValue(PixelOffsetMode.Default)]
        [Description("Sets the pixel offset mode, controlling pixel alignment for improved rendering accuracy.")]
        public PixelOffsetMode PixelOffsetMode { get; set; }

        [Category("Custom Battery")]
        [Description("The color of the outline for the custom battery.")]
        public Color BorderColor
        {
            get => _outlineColor;
            set
            {
                _outlineColor = value;
                Invalidate();
            }
        }

        [Category("Custom Battery")]
        [Description("The thickness of the outline for the custom battery. The minimum value is 1.")]
        public int BorderWidth
        {
            get => _borderOutLine;
            set
            {
                _borderOutLine = Math.Max(1, value);
                Invalidate();
            }
        }

        // [Category("Custom Battery")]
        // [Description("Shape of the battery control")]
        private BatteryShape BatteryShapes
        {
            get => _batteryShape;
            set
            {
                _batteryShape = value;
                Invalidate();
            }
        }

        //[Category("Custom Battery")]
        //[Description("Corner radius for rounded shapes")]
        private float CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = Math.Max(0, value);
                Invalidate();
            }
        }

        [Category("Custom Battery")]
        [Description("Type of animation for value changes")]
        public AnimationType AnimationTypes
        {
            get => _animationType;
            set
            {
                _animationType = value;
                Invalidate();
            }
        }

        [Category("Custom Battery")]
        [Description("Enable glow effect")]
        public bool GlowEffect
        {
            get => _glowEffect;
            set
            {
                _glowEffect = value;
                Invalidate();
            }
        }

        [Category("Custom Battery")]
        [Description("Color of the glow effect")]
        public Color GlowColor
        {
            get => _glowColor;
            set
            {
                _glowColor = value;
                Invalidate();
            }
        }

        [Category("Custom Battery")]
        [Description("Show charging animation")]
        public bool Charging
        {
            get => _charging;
            set
            {
                _charging = value;
                //if (_charging)
                //    _chargingAnimationTimer.Start();
                //else
                //    _chargingAnimationTimer.Stop();
                //OnChargingStateChanged(EventArgs.Empty);
                Invalidate();
            }
        }

        [Category("Custom Battery")]
        [Description("Font used for percentage text")]
        public Font PercentageFont
        {
            get { return _percentageFont; }
            set
            {
                _percentageFont?.Dispose();
                _percentageFont = value;
                Invalidate();
            }
        }

        [Category("Custom Battery")]
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

        [Category("Custom Battery")]
        [Description("Enable or disable value change animation")]
        [DefaultValue(true)]
        public bool EnableAnimation
        {
            get => _enableAnimation;
            set
            {
                _enableAnimation = value;
                if (_enableAnimation && !animationTimer.Enabled)
                {
                    animationTimer.Start();
                }
                else if (!_enableAnimation && animationTimer.Enabled)
                {
                    animationTimer.Stop();
                }
            }
        }

        [Category("Custom Battery")]
        [Description("The battery charge level (0-100)")]
        public int Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    if (EnableAnimation)
                    {
                        _value = value;
                        targetValue = Math.Max(0, Math.Min(100, value));
                        // animationTimer.Start();
                        // OnValueChanging(new ValueChangingEventArgs(valueChange, targetValue));
                        //OnValueChanging(new ValueChangingEventArgs(_value, targetValue));
                    }
                    else
                    {
                        _value = valueChange = Math.Max(0, Math.Min(100, value));
                        // OnValueChanged(EventArgs.Empty);
                        Invalidate();
                    }
                }
            }
        }

        [Category("Custom Battery")]
        [Description("The color of the battery charge indicator")]
        public Color BatteryColor
        {
            get { return _batteryColor; }
            set
            {
                _batteryColor = value;
                Invalidate();
            }
        }

        [Category("Custom Battery")]
        [Description("Show or hide the percentage text")]
        public bool ShowPercentage
        {
            get { return _showPercentage; }
            set
            {
                _showPercentage = value;
                Invalidate();
            }
        }
        #endregion

        // Custom Event
        // public event EventHandler ValueChanged;
        // public event EventHandler<ValueChangingEventArgs> ValueChanging;
        //protected virtual void OnValueChanged(EventArgs e)
        //{
        //    ValueChanged?.Invoke(this, e);
        //}

        //protected virtual void OnValueChanging(ValueChangingEventArgs e)
        //{
        //    ValueChanging?.Invoke(this, e);
        //}

        //public class ValueChangingEventArgs : EventArgs
        //{
        //    public int OldValue { get; }
        //    public int NewValue { get; }

        //    public ValueChangingEventArgs(int oldValue, int newValue)
        //    {
        //        OldValue = oldValue;
        //        NewValue = newValue;
        //    }
        //}

        //public event EventHandler ChargingStateChanged;

        //protected virtual void OnChargingStateChanged(EventArgs e)
        //{
        //    ChargingStateChanged?.Invoke(this, e);
        //}

        //private void ChargingAnimationTimer_Tick(object sender, EventArgs e)
        //{
        //    _chargingAnimationFrame = (_chargingAnimationFrame + 1) % 3;
        //    Invalidate();
        //}


        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 0 || Height < 0) return;

            try
            {
                base.OnPaint(e);
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode;
                g.InterpolationMode = InterpolationMode;
                g.PixelOffsetMode = PixelOffsetMode;

                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Calculate dimensions
                int batteryBody = (int)(Width * 0.9);
                int batteryTip = (int)(Width * 0.1) - 10;
                int batteryHeight = Height;
                int margin = 2;

                // Draw glow effect if enabled
                if (GlowEffect && valueChange > 20)
                {
                    using (var path = GetBatteryPath(batteryBody, batteryHeight))
                    using (var glow = new PathGradientBrush(path))
                    {
                        glow.CenterColor = Color.FromArgb(100, GlowColor);
                        glow.SurroundColors = new[] { Color.FromArgb(0, GlowColor) };
                        g.FillPath(glow, path);
                    }
                }

                // Draw battery level
                if (valueChange > 0)
                {
                    int fillWidth = (int)((batteryBody - margin * 2) * (valueChange / 100.0));
                    using (var fillBrush = new SolidBrush(GetBatteryColor()))
                    {
                        //if (Charging)
                        //{
                        //    // Draw charging animation
                        //    fillWidth = (int)((batteryBody - margin * 2) * ((_chargingAnimationFrame + 1) / 3.0));
                        //}

                        switch (BatteryShapes)
                        {
                            case BatteryShape.RoundedRectangle:
                                g.FillPath(fillBrush,
                                    GetRoundedRectangle(margin, margin,
                                    fillWidth, batteryHeight - margin * 2,
                                    Math.Max(0, CornerRadius - margin)));
                                break;
                            case BatteryShape.Capsule:
                                g.FillPath(fillBrush,
                                    GetCapsulePath(margin, margin,
                                    fillWidth, batteryHeight - margin * 2));
                                break;
                            default:
                                g.FillRectangle(fillBrush,
                                    margin, margin,
                                    fillWidth, batteryHeight - margin * 2);
                                break;
                        }
                    }
                }

                // Draw battery outline based on shape
                using (var pen = new Pen(_outlineColor, _borderOutLine))
                {
                    switch (BatteryShapes)
                    {
                        case BatteryShape.RoundedRectangle:
                            g.DrawPath(pen, GetRoundedRectangle(0, 0, batteryBody - 1, batteryHeight - 1, CornerRadius));
                            break;
                        case BatteryShape.Capsule:
                            g.DrawPath(pen, GetCapsulePath(0, 0, batteryBody - 1, batteryHeight - 1));
                            break;
                        default:
                            g.DrawRectangle(pen, 0, 0, batteryBody - 1, batteryHeight - 1);
                            break;
                    }

                    // Battery tip
                    using (Brush brush = new SolidBrush(_outlineColor))
                    {
                        g.FillRectangle(brush,
                            batteryBody,
                            (int)(batteryHeight * 0.25),
                            batteryTip,
                            (int)(batteryHeight * 0.5));
                    }
                }

                // Draw percentage text
                if (ShowPercentage)
                {
                    string percentText = Charging ? "⚡" + valueChange.ToString() + "%" : valueChange.ToString() + "%";
                    SizeF textSize = g.MeasureString(percentText, PercentageFont);

                    using (var textBrush = new SolidBrush(_textColor))
                    {
                        g.DrawString(percentText,
                            PercentageFont,
                            textBrush,
                            (batteryBody - textSize.Width) / 2,
                            (batteryHeight - textSize.Height) / 2);
                    }
                }
            }
            catch { }
        }

        // Helper methods for shapes
        private GraphicsPath GetRoundedRectangle(float x, float y, float width, float height, float radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(new RectangleF(x, y, width, height));
                return path;
            }

            radius = Math.Min(radius, Math.Min(width / 2, height / 2));

            path.AddArc(x, y, radius * 2, radius * 2, 180, 90);
            path.AddArc(x + width - radius * 2, y, radius * 2, radius * 2, 270, 90);
            path.AddArc(x + width - radius * 2, y + height - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(x, y + height - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();

            return path;
        }

        private GraphicsPath GetCapsulePath(float x, float y, float width, float height)
        {
            float radius = height / 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(x, y, height, height, 90, 180);
            path.AddArc(x + width - height, y, height, height, 270, 180);
            path.CloseFigure();

            return path;
        }

        private GraphicsPath GetBatteryPath(int width, int height)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddRectangle(new Rectangle(-5, -5, width + 10, height + 10));
            return path;
        }

        private Color GetBatteryColor()
        {
            if (valueChange <= 20)
                return Color.Red;
            else if (valueChange <= 50)
                return Color.Orange;
            return _batteryColor;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        private bool _isDispose = false;
        // Clean up resources
        protected override void Dispose(bool disposing)
        {
            if (!_isDispose)
            {
                if (disposing && _percentageFont != null)
                {
                    _percentageFont?.Dispose();
                    animationTimer?.Stop();
                    animationTimer.Tick -= _eventTick;
                    animationTimer?.Dispose();
                    // _chargingAnimationTimer?.Dispose();
                }
                _isDispose = true;
            }

            base.Dispose(disposing);
        }

        ~CustomBattery()
        {
            Dispose(true);
        }
    }
}