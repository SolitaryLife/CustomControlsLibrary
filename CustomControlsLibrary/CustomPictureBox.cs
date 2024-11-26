using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static CustomControlsLibrary.ObjectConverter;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public class CustomPictureBox : UserControl
    {
        private Image _image;
        private BorderRadius _radius = new BorderRadius(0);
        private bool _isCircle = false;
        private PictureBoxSizeMode _sizeMode = PictureBoxSizeMode.Normal;

        public CustomPictureBox()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw, true);

            BackColor = Color.Transparent;
        }

        [Category("Custom PictureBox")]
        [DefaultValue(SmoothingMode.Default)]
        [Description("Specifies the smoothing mode for rendering, which affects the quality of lines and edges.")]
        public SmoothingMode SmoothingMode { get; set; }

        [Category("Custom PictureBox")]
        [DefaultValue(InterpolationMode.Default)]
        [Description("Determines the interpolation mode used for scaling images, which impacts image quality.")]
        public InterpolationMode InterpolationMode { get; set; }

        [Category("Custom PictureBox")]
        [DefaultValue(PixelOffsetMode.Default)]
        [Description("Sets the pixel offset mode, controlling pixel alignment for improved rendering accuracy.")]
        public PixelOffsetMode PixelOffsetMode { get; set; }

        [Category("Custom PictureBox")]
        [DefaultValue(CompositingQuality.Default)]
        [Description("Gets or sets the compositing quality level for drawing operations. Compositing quality determines how drawing operations are blended or composited.")]
        public CompositingQuality CompositingQuality { get; set; }

        [Category("Custom PictureBox")]
        [DefaultValue(true)]
        [Description("Enables or disables double buffering to reduce flickering during rendering.")]
        public bool DoubleBuffereds
        {
            get => DoubleBuffered;
            set => DoubleBuffered = value;
        }

        [Category("Custom PictureBox")]
        [Description("Picture to display")]
        public Image Image
        {
            get { return _image; }
            set
            {
                _image?.Dispose();
                _image = value;
                Invalidate();
            }
        }

        [Category("Custom PictureBox")]
        [Description("Image display format")]
        public PictureBoxSizeMode SizeMode
        {
            get { return _sizeMode; }
            set
            {
                _sizeMode = value;
                Invalidate();
            }
        }

        [Category("Custom PictureBox")]
        [Description("Set it to circle.")]
        public bool IsCircle
        {
            get { return _isCircle; }
            set
            {
                _isCircle = value;
                Invalidate();
            }
        }

        [Category("Custom PictureBox")]
        [Description("Sets the corner radius of the image")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BorderRadius ImageRadius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                Invalidate();
            }
        }

        private Rectangle GetImageRectangle()
        {
            if (_image == null) return ClientRectangle;

            Rectangle rect = ClientRectangle;
            Size imageSize = _image.Size;

            switch (_sizeMode)
            {
                case PictureBoxSizeMode.Normal:
                    float scale = Math.Min(
                        (float)rect.Width / imageSize.Width,
                        (float)rect.Height / imageSize.Height
                    );
                    int newWidth = (int)(imageSize.Width * scale);
                    int newHeight = (int)(imageSize.Height * scale);

                    rect.X = (rect.Width - newWidth) / 2;
                    rect.Y = (rect.Height - newHeight) / 2;
                    rect.Width = newWidth;
                    rect.Height = newHeight;
                    break;
                case PictureBoxSizeMode.StretchImage:
                    // Use full size control
                    break;
                case PictureBoxSizeMode.AutoSize:
                    this.Size = imageSize;
                    break;
                case PictureBoxSizeMode.CenterImage:
                    float centerScale = Math.Min(
                        (float)rect.Width / imageSize.Width,
                        (float)rect.Height / imageSize.Height
                    );
                    newWidth = (int)(imageSize.Width * centerScale);
                    newHeight = (int)(imageSize.Height * centerScale);

                    rect.X = (rect.Width - newWidth) / 2;
                    rect.Y = (rect.Height - newHeight) / 2;
                    rect.Width = newWidth;
                    rect.Height = newHeight;
                    break;
                case PictureBoxSizeMode.Zoom:
                    float zoomScale = Math.Min(
                        (float)rect.Width / imageSize.Width,
                        (float)rect.Height / imageSize.Height
                    );
                    newWidth = (int)(imageSize.Width * zoomScale);
                    newHeight = (int)(imageSize.Height * zoomScale);

                    rect.X = (rect.Width - newWidth) / 2;
                    rect.Y = (rect.Height - newHeight) / 2;
                    rect.Width = newWidth;
                    rect.Height = newHeight;

                    break;
            }

            return rect;
        }

        private Color GetParentBackgroundColor()
        {
            if (Parent != null && Parent.BackColor != Color.Transparent)
                return Parent.BackColor;

            Control control = Parent;
            while (control?.Parent != null)
            {
                control = control.Parent;
                if (control.BackColor != Color.Transparent)
                    return control.BackColor;
            }

            return Color.Transparent; // Default if no parent with a solid color
        }

        private void CreateRoundedCornersPath(Rectangle rect, GraphicsPath path)
        {
            // Create a path for a curved corner.
            int diameter;

            // Top Left
            diameter = _radius.TopLeft * 2;
            if (_radius.TopLeft > 0)
                path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            else
                path.AddLine(rect.X, rect.Y, rect.X + diameter, rect.Y);

            // Top Right
            diameter = _radius.TopRight * 2;
            if (_radius.TopRight > 0)
                path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            else
                path.AddLine(rect.Right - diameter, rect.Y, rect.Right, rect.Y);

            // Bottom Right
            diameter = _radius.BottomRight * 2;
            if (_radius.BottomRight > 0)
                path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            else
                path.AddLine(rect.Right, rect.Bottom - diameter, rect.Right, rect.Bottom);

            // Bottom Left
            diameter = _radius.BottomLeft * 2;
            if (_radius.BottomLeft > 0)
                path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            else
                path.AddLine(rect.X, rect.Bottom, rect.X, rect.Bottom - diameter);

            path.CloseFigure();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width < 0 || Height < 0) return;

            try
            {
                using (BufferedGraphics buffered = BufferedGraphicsManager.Current.Allocate(
                    e.Graphics, ClientRectangle))
                {
                    Graphics g = buffered.Graphics;

                    // Set drawing quality
                    g.SmoothingMode = SmoothingMode;
                    g.InterpolationMode = InterpolationMode;
                    g.CompositingQuality = CompositingQuality;
                    g.PixelOffsetMode = PixelOffsetMode;

                    // Handle transparent background
                    Color backColor = BackColor == Color.Transparent ? GetParentBackgroundColor() : BackColor;
                    using (SolidBrush backBrush = new SolidBrush(backColor))
                    {
                        g.FillRectangle(backBrush, ClientRectangle);
                    }

                    // Create the path for the shape
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        Rectangle rect = ClientRectangle;

                        if (_isCircle)
                        {
                            path.AddEllipse(rect);
                        }
                        else
                        {
                            CreateRoundedCornersPath(rect, path);
                        }

                        // Drawing in BufferedGraphics
                        if (_image != null)
                        {
                            // Create a TextureBrush from an image
                            using (TextureBrush brush = new TextureBrush(_image, WrapMode.Clamp))
                            {
                                // Adjust the size and position of the brush according to SizeMode.
                                Rectangle imageRect = GetImageRectangle();

                                // Scale image to fit the rectangle
                                Matrix transform = new Matrix();
                                transform.Translate(imageRect.X, imageRect.Y);
                                transform.Scale((float)imageRect.Width / _image.Width,
                                                (float)imageRect.Height / _image.Height);
                                brush.Transform = transform;

                                // Fill the path with the texture brush (image)
                                g.FillPath(brush, path);
                            }
                        }

                        // Render the buffered graphics to the screen
                        buffered.Render();
                    }
                }
            }
            catch { }
        }

        #region Dispose
        private bool _isDispose = false;
        protected override void Dispose(bool disposing)
        {
            if (!_isDispose)
            {
                if (disposing)
                {
                    _image?.Dispose();
                }
                _isDispose = true;
            }

            base.Dispose(disposing);
        }
        ~CustomPictureBox()
        {
            Dispose(true);
        }
        #endregion
    }
}
