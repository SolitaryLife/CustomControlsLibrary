using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CustomControlsLibrary
{
    public partial class CustomTextBox : UserControl, IDisposable
    {
        // Fields
        private TextBox textBox;
        private Color _borderColor = Color.MediumSlateBlue;
        private Color _borderFocusColor = Color.HotPink;
        private int _borderSize = 2;
        private bool _underlinedStyle = false;
        private Color _placeholderColor = Color.DarkGray;
        private string _placeholderText = "";
        private bool _isPlaceholder = false;
        private bool _isPasswordChar = false;

        // Constructor
        public CustomTextBox()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.textBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Location = new System.Drawing.Point(7, 7);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(186, 15);
            this.textBox.TabIndex = 0;
            this.textBox.Enter += new System.EventHandler(this.textBox_Enter);
            this.textBox.Leave += new System.EventHandler(this.textBox_Leave);
            // 
            // CustomTextBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.textBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F);
            this.ForeColor = System.Drawing.Color.DimGray;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CustomTextBox";
            this.Padding = new System.Windows.Forms.Padding(7);
            this.Size = new System.Drawing.Size(200, 30);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #region Properties
        // Properties
        [Category("Custom TextBox")]
        [Description("Gets or sets the border color of the control.")]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                this.Invalidate();
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
            set { textBox.Multiline = value; }
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
                if (_isPlaceholder) return "";
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
                SetPlaceholder();
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
                if (string.IsNullOrEmpty(textBox.Text))
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
            base.OnPaint(e);
            Graphics g = e.Graphics;

            using (Pen penBorder = new Pen(_borderColor, _borderSize))
            {
                if (_underlinedStyle) // Line Style
                {
                    g.DrawLine(penBorder, 0, this.Height - 1, this.Width, this.Height - 1);
                }
                else // Normal Rectangle Style
                {
                    g.DrawRectangle(penBorder, 0, 0, this.Width - 0.5F, this.Height - 0.5F);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.DesignMode)
                UpdateControlHeight();
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

        private void SetPlaceholder()
        {
            if (string.IsNullOrEmpty(textBox.Text) && !string.IsNullOrEmpty(_placeholderText))
            {
                _isPlaceholder = true;
                textBox.Text = _placeholderText;
                textBox.ForeColor = _placeholderColor;
                if (_isPasswordChar)
                    textBox.UseSystemPasswordChar = false;
            }
        }

        private void RemovePlaceholder()
        {
            if (_isPlaceholder && _placeholderText == textBox.Text)
            {
                _isPlaceholder = false;
                textBox.Text = "";
                textBox.ForeColor = this.ForeColor;
                if (_isPasswordChar)
                    textBox.UseSystemPasswordChar = true;
            }
        }
        #endregion

        #region Event Methods
        // Event Methods
        private void textBox_Enter(object sender, EventArgs e)
        {
            this.BorderColor = _borderFocusColor;
            this.Invalidate();
            RemovePlaceholder();
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            this.BorderColor = _borderColor;
            this.Invalidate();
            SetPlaceholder();
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
        public event EventHandler TextChanged
        {
            add { textBox.TextChanged += value; }
            remove { textBox.TextChanged -= value; }
        }

        public event KeyEventHandler KeyDown
        {
            add { textBox.KeyDown += value; }
            remove { textBox.KeyDown -= value; }
        }

        public event KeyEventHandler KeyUp
        {
            add { textBox.KeyUp += value; }
            remove { textBox.KeyUp -= value; }
        }

        public event KeyPressEventHandler KeyPress
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
                        textBox.Enter -= textBox_Enter;
                        textBox.Leave -= textBox_Leave;
                        textBox.Dispose();
                    }
                }

                // Release unmanaged resources (if any)

                disposedValue = true;
            }

            // Call base class Dispose
            base.Dispose(disposing);
        }

        // Implement IDisposable
        public new void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
