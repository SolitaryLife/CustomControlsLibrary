using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace CustomControlsLibrary
{
    [ToolboxItem(true)]
    public class CustomToolTip : ToolTip
    {
        private string description = "";
        private Color backgroundColor = Color.LightYellow;
        private Color textColor = Color.Black;
        private Font tooltipFont = new Font("Segoe UI", 9f);
        private Dictionary<Control, string> tooltipTexts = new Dictionary<Control, string>();

        public CustomToolTip()
        {
            OwnerDraw = true;
            Popup += new PopupEventHandler(OnPopup);
            Draw += new DrawToolTipEventHandler(OnDraw);
        }

        [Category("Custom ToolTip")]
        [DefaultValue(SmoothingMode.Default)]
        [Description("Specifies the smoothing mode for rendering, which affects the quality of lines and edges.")]
        public SmoothingMode SmoothingMode { get; set; }

        [Category("Custom ToolTip")]
        [DefaultValue(InterpolationMode.Default)]
        [Description("Determines the interpolation mode used for scaling images, which impacts image quality.")]
        public InterpolationMode InterpolationMode { get; set; }

        [Category("Custom ToolTip")]
        [DefaultValue(PixelOffsetMode.Default)]
        [Description("Sets the pixel offset mode, controlling pixel alignment for improved rendering accuracy.")]
        public PixelOffsetMode PixelOffsetMode { get; set; }

        [Category("Custom ToolTip")]
        [DefaultValue(CompositingQuality.Default)]
        [Description("Gets or sets the compositing quality level for drawing operations. Compositing quality determines how drawing operations are blended or composited.")]
        public CompositingQuality CompositingQuality { get; set; }

        [Category("Custom ToolTip")]
        [Description("Additional description text to show in tooltip")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [Category("Custom ToolTip")]
        [Description("Background color of the tooltip")]
        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }

        [Category("Custom ToolTip")]
        [Description("Text color of the tooltip")]
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        [Category("Custom ToolTip")]
        [Description("Font used in the tooltip")]
        public Font TooltipFont
        {
            get { return tooltipFont; }
            set { tooltipFont = value; }
        }

        /// <summary>
        /// Sets the tooltip text for the specified control and stores the tooltip text in a dictionary.
        /// </summary>
        /// <param name="control">The control to which the tooltip is applied.</param>
        /// <param name="caption">The text to display as the tooltip.</param>
        public new void SetToolTip(Control control, string caption)
        {
            base.SetToolTip(control, caption);
            tooltipTexts[control] = caption;
        }

        private void OnPopup(object sender, PopupEventArgs e)
        {
            string text = tooltipTexts.ContainsKey(e.AssociatedControl)
            ? tooltipTexts[e.AssociatedControl]
            : "";

            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                SizeF titleSize = g.MeasureString(text, tooltipFont);
                SizeF descSize = !string.IsNullOrEmpty(description)
                    ? g.MeasureString(description, tooltipFont)
                    : SizeF.Empty;

                int padding = 15;
                int width = Math.Max((int)titleSize.Width, (int)descSize.Width) + padding;
                int height = (int)titleSize.Height;
                if (!string.IsNullOrEmpty(description))
                {
                    height += (int)descSize.Height + 6;
                }
                height += 10;

                e.ToolTipSize = new Size(width, height);
            }
        }

        private void OnDraw(object sender, DrawToolTipEventArgs e)
        {
            try
            {
                string text = tooltipTexts.ContainsKey(e.AssociatedControl)
            ? tooltipTexts[e.AssociatedControl]
            : "";

                e.Graphics.SmoothingMode = SmoothingMode;
                e.Graphics.InterpolationMode = InterpolationMode;
                e.Graphics.PixelOffsetMode = PixelOffsetMode;
                e.Graphics.CompositingQuality = CompositingQuality;

                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                // Draw a background with a gradient for beauty.
                using (LinearGradientBrush brush =
                    new LinearGradientBrush(
                        e.Bounds,
                        backgroundColor,
                        Color.FromArgb(Math.Max(0, backgroundColor.R - 20),
                                     Math.Max(0, backgroundColor.G - 20),
                                     Math.Max(0, backgroundColor.B - 20)),
                        LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }

                // Draw edges with high contrast.
                using (Pen borderPen = new Pen(Color.FromArgb(128, Color.Gray), 1))
                {
                    Rectangle borderRect = new Rectangle(
                        e.Bounds.X,
                        e.Bounds.Y,
                        e.Bounds.Width - 1,
                        e.Bounds.Height - 1
                    );
                    e.Graphics.DrawRectangle(borderPen, borderRect);
                }

                // Calculate the area for displaying text.
                Rectangle titleRect = new Rectangle(
                    e.Bounds.X + 5,
                    e.Bounds.Y + 5,
                    e.Bounds.Width - 10,
                    (int)e.Graphics.MeasureString(text, tooltipFont).Height
                );

                // Draw main text
                using (SolidBrush textBrush = new SolidBrush(textColor))
                {
                    e.Graphics.DrawString(text, tooltipFont, textBrush, titleRect);

                    // Draw a description if available.
                    if (!string.IsNullOrEmpty(description))
                    {
                        Rectangle descRect = new Rectangle(
                            e.Bounds.X + 5,
                            titleRect.Bottom + 3,
                            e.Bounds.Width - 10,
                            e.Bounds.Height - titleRect.Height - 8
                        );
                        e.Graphics.DrawString(description, tooltipFont, textBrush, descRect);
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
                    tooltipFont?.Dispose();
                    tooltipTexts.Clear();
                }
                _isDispose = true;
            }
            base.Dispose(disposing);
        }

        ~CustomToolTip()
        {
            Dispose(true);
        }

        #endregion
    }
}
