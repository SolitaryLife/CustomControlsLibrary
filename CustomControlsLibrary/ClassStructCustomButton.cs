using System;
using System.ComponentModel;
using System.Globalization;

namespace CustomControlsLibrary
{
    public partial class CustomButton
    {
        [TypeConverter(typeof(BorderRadiusSTypeConverter))]
        public struct BorderRadius
        {
            private int _radiusTopLeft;
            private int _radiusTopRight;
            private int _radiusBottomLeft;
            private int _radiusBottomRight;

            public int TopLeft
            {
                get => _radiusTopLeft;
                set => _radiusTopLeft = value;
            }

            public int TopRight
            {
                get => _radiusTopRight;
                set => _radiusTopRight = value;
            }

            public int BottomLeft
            {
                get => _radiusBottomLeft;
                set => _radiusBottomLeft = value;
            }

            public int BottomRight
            {
                get => _radiusBottomRight;
                set => _radiusBottomRight = value;
            }

            public BorderRadius(int uniformRadius)
            {
                _radiusTopLeft = uniformRadius;
                _radiusTopRight = uniformRadius;
                _radiusBottomLeft = uniformRadius;
                _radiusBottomRight = uniformRadius;
            }

            public BorderRadius(int topLeft, int topRight, int bottomLeft, int bottomRight)
            {
                _radiusTopLeft = topLeft;
                _radiusTopRight = topRight;
                _radiusBottomLeft = bottomLeft;
                _radiusBottomRight = bottomRight;
            }

            public override string ToString()
            {
                return $"{TopLeft}, {TopRight}, {BottomLeft}, {BottomRight}";
            }
        }
        private class BorderRadiusSTypeConverter : ExpandableObjectConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;
                return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                if (value is string stringValue)
                {
                    string[] parts = stringValue.Split(',');
                    if (parts.Length == 4)
                    {
                        if (int.TryParse(parts[0].Trim(), out int topLeft) &&
                            int.TryParse(parts[1].Trim(), out int topRight) &&
                            int.TryParse(parts[2].Trim(), out int bottomLeft) &&
                            int.TryParse(parts[3].Trim(), out int bottomRight))
                        {
                            return new BorderRadius(topLeft, topRight, bottomLeft, bottomRight);
                        }
                    }
                    else if (parts.Length == 1)
                    {
                        if (int.TryParse(parts[0].Trim(), out int uniformRadius))
                        {
                            return new BorderRadius(uniformRadius);
                        }
                    }
                }
                return base.ConvertFrom(context, culture, value);
            }

            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                return TypeDescriptor.GetProperties(typeof(BorderRadius));
            }
        }
    }
}
