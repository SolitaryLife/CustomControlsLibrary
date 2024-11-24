using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public class CustomTextBox : UserControl, IDisposable
    {
        // Fields
        private TextBox textBox;
        private Color _borderColor = Color.MediumSlateBlue;
        private Color _borderFocusColor = Color.HotPink;
        private Color _borderColorChange = Color.Transparent;
        private int _borderSize = 2;
        private bool _underlinedStyle = false;
        private Color _placeholderColor = Color.DarkGray;
        private string _placeholderText = "";
        private string _textValue = "";
        private bool _isPlaceholder = false;
        private bool _isPasswordChar = false;
        private EventHandler internalTextChangedHandler;

        private PictureBox pictureBox;
        private ContentAlignment _iconAlign = ContentAlignment.MiddleLeft;
        private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
        private bool _showIcon = false;
        private int _iconPadding = 5;

        // Constructor
        public CustomTextBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            DoubleBuffered = true;

            textBox = new TextBox();
            pictureBox = new PictureBox();

            SuspendLayout();
            // 
            // textBox
            // 
            textBox.BorderStyle = BorderStyle.None;
            textBox.Location = new Point(7, 7);
            textBox.Name = "textBox";
            textBox.Size = new Size(186, 15);
            textBox.ScrollBars = ScrollBars.None;
            textBox.TabIndex = 0;
            textBox.Enter += new EventHandler(textBox_Enter);
            textBox.Leave += new EventHandler(textBox_Leave);
            textBox.MouseHover += textBox_MouseHover;
            textBox.MouseLeave += textBox_MouseLeave;
            internalTextChangedHandler = new EventHandler(InternalTextBox_TextChanged);
            textBox.TextChanged += internalTextChangedHandler;
            //
            // pictureBox
            //
            pictureBox.Size = new Size(16, 16);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
            pictureBox.Visible = _showIcon;
            pictureBox.Name = "pictureBox";
            // 
            // CustomTextBox
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Window;
            Controls.Add(this.textBox);
            Controls.Add(pictureBox);
            Font = new Font("Microsoft Sans Serif", 9.5F);
            ForeColor = Color.DimGray;
            Margin = new Padding(4);
            Name = "CustomTextBox";
            Padding = new Padding(7);
            Size = new Size(200, 30);
            ResumeLayout(false);
            PerformLayout();
        }

        #region Properties
        // Properties
        //[Category("Custom TextBox")]
        //public Size pictureBoxSize
        //{
        //    get => pictureBox.Size;
        //    set
        //    {

        //        int W = value.Width;
        //        int H = Math.Max(Size.Height - 14, value.Height);

        //        pictureBox.Size = new Size(W, H);
        //    }
        //}

        [Category("Custom TextBox")]
        [DefaultValue(SmoothingMode.Default)]
        [Description("Specifies the smoothing mode for rendering, which affects the quality of lines and edges.")]
        public SmoothingMode SmoothingMode { get; set; }

        [Category("Custom TextBox")]
        [DefaultValue(InterpolationMode.Default)]
        [Description("Determines the interpolation mode used for scaling images, which impacts image quality.")]
        public InterpolationMode InterpolationMode { get; set; }

        [Category("Custom TextBox")]
        [DefaultValue(PixelOffsetMode.Default)]
        [Description("Sets the pixel offset mode, controlling pixel alignment for improved rendering accuracy.")]
        public PixelOffsetMode PixelOffsetMode { get; set; }


        [Category("Custom TextBox")]
        [DefaultValue(true)]
        [Description("Enables or disables double buffering to reduce flickering during rendering.")]
        public bool DoubleBuffereds
        {
            get => DoubleBuffered;
            set => DoubleBuffered = value;
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the padding between icon and text.")]
        public int IconPadding
        {
            get => _iconPadding;
            set
            {
                if (value >= 0)
                {
                    _iconPadding = value;
                    UpdateIconPosition();
                }
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the alignment of the icon.")]
        public ContentAlignment IconAlign
        {
            get => _iconAlign;
            set
            {
                _iconAlign = value;
                UpdateIconPosition();
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the alignment of the text.")]
        public HorizontalAlignment TextAlign
        {
            get => _textAlign;
            set
            {
                _textAlign = value;
                textBox.TextAlign = value;
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the border color of the control.")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColorChange  = _borderColor = value;
                this.Invalidate();
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets whether the icon should be displayed.")]
        public bool ShowIcon
        {
            get => _showIcon;
            set
            {
                _showIcon = value;
                if (Multiline && _showIcon)
                {
                    Multiline = false;
                }
                pictureBox.Visible = _showIcon;
                pictureBox.BringToFront();
                textBox.Dock = _showIcon ? DockStyle.None : DockStyle.Fill;
                UpdateIconPosition();
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the icon to be displayed.")]
        public Image Icon
        {
            get => pictureBox.Image;
            set
            {
                pictureBox.Image = value;
                UpdateIconPosition();
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the border color when the control is focused.")]
        public Color BorderFocusColor
        {
            get => _borderFocusColor;
            set
            {
                _borderFocusColor = value;
                this.Invalidate();
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the size of the border around the control.")]
        public int BorderSize
        {
            get => _borderSize;
            set
            {
                _borderSize = value;
                this.Invalidate();
            }
        }

        [Category("Custom TextBox")]
        [Description("Determines whether the control displays an underlined style.")]
        public bool UnderlinedStyle
        {
            get => _underlinedStyle;
            set
            {
                _underlinedStyle = value;
                this.Invalidate();
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets whether the text box should mask characters (password input).")]
        public bool PasswordChar
        {
            get { return _isPasswordChar; }
            set
            {
                _isPasswordChar = value;
                if (!_isPlaceholder)
                    textBox.UseSystemPasswordChar = value;
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets whether the text box should support multiple lines of text.")]
        public bool Multiline
        {
            get { return textBox.Multiline; }
            set
            {
                if (ShowIcon && value)
                {
                    ShowIcon = false;
                }

                textBox.Multiline = value;
                textBox.ScrollBars = value ? ScrollBars.Both : ScrollBars.None;
                textBox.Dock = value ? DockStyle.Fill : DockStyle.None;
                UpdateControlHeight();
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the background color of the control.")]
        public override Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                textBox.BackColor = value;
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the text color of the control.")]
        public override Color ForeColor
        {
            get => base.ForeColor;
            set
            {
                base.ForeColor = value;
                textBox.ForeColor = value;
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the font of the text displayed by the control.")]
        public override Font Font
        {
            get => base.Font;
            set
            {
                base.Font = value;
                textBox.Font = value;
                if (this.DesignMode)
                    UpdateControlHeight();
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the text associated with the control.")]
        public string Texts
        {
            get
            {
                return _textValue;
            }
            set
            {

                _textValue = value;
                if (!string.IsNullOrEmpty(_textValue))
                {
                    RemovePlaceholder();
                }
                else
                {
                    SetPlaceholder();
                }
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the placeholder text displayed in the control when empty.")]
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                if (string.IsNullOrEmpty(_textValue))
                    SetPlaceholder();
            }
        }

        [Category("Custom TextBox")]
        [Description("Gets or sets the color of the placeholder text.")]
        public Color PlaceholderColor
        {
            get => _placeholderColor;
            set
            {
                _placeholderColor = value;
                if (_isPlaceholder)
                    textBox.ForeColor = value;
            }
        }
        #endregion

        #region Override Methods
        // Override Methods
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0) return;
            try
            {
                base.OnPaint(e);
                Graphics g = e.Graphics;
                g.SmoothingMode = SmoothingMode;
                g.InterpolationMode = InterpolationMode;
                g.PixelOffsetMode = PixelOffsetMode;

                using (Pen penBorder = new Pen(_borderColorChange, _borderSize))
                {
                    penBorder.StartCap = LineCap.Round;
                    penBorder.EndCap = LineCap.Round;
                    penBorder.LineJoin = LineJoin.Round;

                    if (_underlinedStyle) // Line Style
                    {
                        g.DrawLine(penBorder, 0, this.Height - 1, this.Width, this.Height - 1);
                    }
                    else // Normal Rectangle Style
                    {
                        g.DrawRectangle(penBorder, 0.5F, 0.5F, this.Width - 0.5F, this.Height - 0.5F);
                    }
                }
            }
            catch { }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.DesignMode)
                UpdateControlHeight();
            UpdateIconPosition();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateControlHeight();
        }
        #endregion

        #region Private Methods
        // Private Methods
        private void UpdateControlHeight()
        {
            if (!textBox.Multiline)
            {
                int txtHeight = TextRenderer.MeasureText("Text", this.Font).Height + 1;
                textBox.Multiline = true;
                textBox.MinimumSize = new Size(0, txtHeight);
                textBox.Multiline = false;
                this.Height = textBox.Height + this.Padding.Top + this.Padding.Bottom;
            }
        }

        private void UpdateIconPosition()
        {
            if (_showIcon && pictureBox.Image != null)
            {
                int textBoxNewWidth;
                int iconSpace = pictureBox.Width + _iconPadding;

                switch (_iconAlign)
                {
                    case ContentAlignment.MiddleLeft:
                        pictureBox.Location = new Point(this.Padding.Left, (this.Height - pictureBox.Height) / 2);
                        textBox.Location = new Point(this.Padding.Left + iconSpace, textBox.Top);
                        textBoxNewWidth = this.Width - this.Padding.Left - iconSpace - this.Padding.Right;
                        textBox.Width = Math.Max(0, textBoxNewWidth);
                        break;

                    case ContentAlignment.MiddleRight:
                        pictureBox.Location = new Point(this.Width - pictureBox.Width - this.Padding.Right, (this.Height - pictureBox.Height) / 2);
                        textBoxNewWidth = this.Width - this.Padding.Left - this.Padding.Right - iconSpace;
                        textBox.Width = Math.Max(0, textBoxNewWidth);
                        textBox.Location = new Point(this.Padding.Left, textBox.Top);
                        break;

                    case ContentAlignment.TopLeft:
                        pictureBox.Location = new Point(this.Padding.Left, this.Padding.Top);
                        textBox.Location = new Point(pictureBox.Right + this.Padding.Left, this.Padding.Top);
                        textBoxNewWidth = this.Width - pictureBox.Right - this.Padding.Left - this.Padding.Right - iconSpace;
                        textBox.Width = Math.Max(0, textBoxNewWidth);
                        break;

                    case ContentAlignment.TopRight:
                        pictureBox.Location = new Point(this.Width - pictureBox.Width - this.Padding.Right, this.Padding.Top);
                        textBoxNewWidth = pictureBox.Left - this.Padding.Left - this.Padding.Right - iconSpace;
                        textBox.Width = Math.Max(0, textBoxNewWidth);
                        textBox.Location = new Point(this.Padding.Left, this.Padding.Top);
                        break;

                    case ContentAlignment.BottomLeft:
                        pictureBox.Location = new Point(this.Padding.Left, this.Height - pictureBox.Height - this.Padding.Bottom);
                        textBox.Location = new Point(pictureBox.Right + this.Padding.Left, this.Padding.Top);
                        textBoxNewWidth = this.Width - pictureBox.Right - this.Padding.Left - this.Padding.Right - iconSpace;
                        textBox.Width = Math.Max(0, textBoxNewWidth);
                        break;

                    case ContentAlignment.BottomRight:
                        pictureBox.Location = new Point(this.Width - pictureBox.Width - this.Padding.Right, this.Height - pictureBox.Height - this.Padding.Bottom);
                        textBoxNewWidth = pictureBox.Left - this.Padding.Left - this.Padding.Right - iconSpace;
                        textBox.Width = Math.Max(0, textBoxNewWidth);
                        textBox.Location = new Point(this.Padding.Left, this.Padding.Top);
                        break;
                }
            }
            else
            {
                textBox.Location = new Point(this.Padding.Left, textBox.Top);
                textBox.Width = this.Width - this.Padding.Left - this.Padding.Right;
            }

            int H = Size.Height - 14;
            int W = H - 1;

            pictureBox.Size = new Size(W, H);
        }

        private void SetPlaceholder()
        {
            _isPlaceholder = true;
            textBox.Text = _placeholderText;
            textBox.ForeColor = _placeholderColor;
            if (_isPasswordChar)
                textBox.UseSystemPasswordChar = false;
            UpdateIconPosition();
        }

        private void RemovePlaceholder()
        {
            _isPlaceholder = false;
            textBox.Text = _textValue;
            textBox.ForeColor = this.ForeColor;
            if (_isPasswordChar)
                textBox.UseSystemPasswordChar = true;
            UpdateIconPosition();
        }
        #endregion

        #region Event Methods
        // Event Methods
        private void textBox_Enter(object sender, EventArgs e)
        {
            _borderColorChange = _borderFocusColor;
            Invalidate();

            RemovePlaceholder();
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            _borderColorChange = _borderColor;
            Invalidate();
            _textValue = textBox.Text;
            if (string.IsNullOrEmpty(_textValue))
            {
                SetPlaceholder();
            }
        }

        private void textBox_MouseHover(object sender, EventArgs e)
        {
            _borderColorChange = _borderFocusColor;
            Invalidate();
        }

        private void textBox_MouseLeave(object sender, EventArgs e)
        {
            _borderColorChange = _borderColor;
            Invalidate();
        }

        private void InternalTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_isPlaceholder)
            {
                _textValue = textBox.Text;
            }
        }

        #endregion

        #region Public Methods to Access TextBox Events
        // Public Methods to Access TextBox Events
        public void Clear()
        {
            textBox.Clear();
        }

        public void Select(int start, int length)
        {
            textBox.Select(start, length);
        }

        public void SelectAll()
        {
            textBox.SelectAll();
        }
        #endregion

        #region Events
        // Events
        public new event EventHandler TextChanged
        {
            add { textBox.TextChanged += value; }
            remove { textBox.TextChanged -= value; }
        }

        public new event KeyEventHandler KeyDown
        {
            add { textBox.KeyDown += value; }
            remove { textBox.KeyDown -= value; }
        }

        public new event KeyEventHandler KeyUp
        {
            add { textBox.KeyUp += value; }
            remove { textBox.KeyUp -= value; }
        }

        public new event KeyPressEventHandler KeyPress
        {
            add { textBox.KeyPress += value; }
            remove { textBox.KeyPress -= value; }
        }
        #endregion

        #region Override the Dispose method
        private bool disposedValue;

        // Override the Dispose method
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Release managed resources
                    if (textBox != null)
                    {
                        textBox.TextChanged -= internalTextChangedHandler;
                        textBox.Enter -= textBox_Enter;
                        textBox.Leave -= textBox_Leave;
                        textBox.MouseHover -= textBox_MouseHover;
                        textBox.MouseLeave -= textBox_MouseLeave;
                        pictureBox?.Dispose();
                        textBox?.Dispose();
                    }
                }

                disposedValue = true;
            }

            // Call base class Dispose
            base.Dispose(disposing);
        }

        // Implement IDisposable
        public new void Dispose()
        {
            Dispose(disposing: true);
            GC.Collect();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
