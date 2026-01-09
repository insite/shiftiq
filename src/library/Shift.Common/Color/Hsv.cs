using System;

namespace Shift.Common.Colors
{
    public class Hsv
    {
        #region Properties

        public double Hue
        {
            get => _hue;
            set => _hue = GetHue(value);
        }

        public double Saturation
        {
            get => _saturation;
            set => _saturation = GetPercent(value);
        }

        public double Value
        {
            get => _value;
            set => _value = GetPercent(value);
        }

        #endregion

        #region Fields

        private double _hue;
        private double _saturation;
        private double _value;

        #endregion

        #region Construction

        public Hsv(double hue, double saturation, double value)
        {
            Hue = hue;
            Saturation = saturation;
            Value = value;
        }

        #endregion

        #region Public methods

        public System.Drawing.Color GetColor() =>
            GetColorInternal(Hue, Saturation, Value);

        public static System.Drawing.Color GetColor(double hue, double saturation, double value) =>
            GetColorInternal(GetHue(hue), GetPercent(saturation), GetPercent(value));

        public byte GetRed() =>
            GetRedInternal(Hue, Saturation, Value);

        public static byte GetRed(double hue, double saturation, double value) =>
            GetRedInternal(GetHue(hue), GetPercent(saturation), GetPercent(value));

        public byte GetGreen() =>
            GetGreenInternal(Hue, Saturation, Value);

        public static byte GetGreen(double hue, double saturation, double value) =>
            GetGreenInternal(GetHue(hue), GetPercent(saturation), GetPercent(value));

        public byte GetBlue() =>
            GetBlueInternal(Hue, Saturation, Value);

        public static byte GetBlue(double hue, double saturation, double value) =>
            GetBlueInternal(GetHue(hue), GetPercent(saturation), GetPercent(value));

        #endregion

        #region Methods (calculation)

        private static System.Drawing.Color GetColorInternal(double hue, double saturation, double value)
        {
            if (value == 0)
                return System.Drawing.Color.Black;

            double red;
            double green;
            double blue;

            if (saturation > 0)
            {
                GetPosition(hue, out int primaryIndex, out double secondaryValue);

                switch (primaryIndex)
                {
                    // Red

                    case 0:
                        red = value;
                        green = GetSecondaryAsc(value, secondaryValue, saturation);
                        blue = GetTint(value, saturation);
                        break;
                    case 5:
                        red = value;
                        green = GetTint(value, saturation);
                        blue = GetSecondaryDesc(value, secondaryValue, saturation);
                        break;

                    // Green

                    case 1:
                        red = GetSecondaryDesc(value, secondaryValue, saturation);
                        green = value;
                        blue = GetTint(value, saturation);
                        break;
                    case 2:
                        red = GetTint(value, saturation);
                        green = value;
                        blue = GetSecondaryAsc(value, secondaryValue, saturation);
                        break;

                    // Blue

                    case 3:
                        red = GetTint(value, saturation);
                        green = GetSecondaryDesc(value, secondaryValue, saturation);
                        blue = value;
                        break;
                    case 4:
                        red = GetSecondaryAsc(value, secondaryValue, saturation);
                        green = GetTint(value, saturation);
                        blue = value;
                        break;

                    default:
                        throw new ApplicationException($"Invalid hue value: {hue}");
                }
            }
            else
            {
                red = value;
                green = value;
                blue = value;
            }

            return System.Drawing.Color.FromArgb((int)(red * 255D), (int)(green * 255D), (int)(blue * 255D));
        }

        private static byte GetRedInternal(double hue, double saturation, double value)
        {
            if (value == 0)
                return 0;

            double red;

            if (saturation > 0)
            {
                GetPosition(hue, out int primaryIndex, out double secondaryValue);

                switch (primaryIndex)
                {
                    // Red

                    case 0:
                        red = value;
                        break;
                    case 5:
                        red = value;
                        break;

                    // Green

                    case 1:
                        red = GetSecondaryDesc(value, secondaryValue, saturation);
                        break;
                    case 2:
                        red = GetTint(value, saturation);
                        break;

                    // Blue

                    case 3:
                        red = GetTint(value, saturation);
                        break;
                    case 4:
                        red = GetSecondaryAsc(value, secondaryValue, saturation);
                        break;

                    default:
                        throw new ApplicationException($"Invalid hue value: {hue}");
                }
            }
            else
                red = value;

            return (byte)(red * 255D);
        }

        private static byte GetGreenInternal(double hue, double saturation, double value)
        {
            if (value == 0)
                return 0;

            double green;

            if (saturation > 0)
            {
                GetPosition(hue, out int primaryIndex, out double secondaryValue);

                switch (primaryIndex)
                {
                    // Red

                    case 0:
                        green = GetSecondaryAsc(value, secondaryValue, saturation);
                        break;
                    case 5:
                        green = GetTint(value, saturation);
                        break;

                    // Green

                    case 1:
                        green = value;
                        break;
                    case 2:
                        green = value;
                        break;

                    // Blue

                    case 3:
                        green = GetSecondaryDesc(value, secondaryValue, saturation);
                        break;
                    case 4:
                        green = GetTint(value, saturation);
                        break;

                    default:
                        throw new ApplicationException($"Invalid hue value: {hue}");
                }
            }
            else
            {
                green = value;
            }

            return (byte)(green * 255D);
        }

        private static byte GetBlueInternal(double hue, double saturation, double value)
        {
            if (value == 0)
                return 0;

            double blue;

            if (saturation > 0)
            {
                GetPosition(hue, out int primaryIndex, out double secondaryValue);

                switch (primaryIndex)
                {
                    // Red

                    case 0:
                        blue = GetTint(value, saturation);
                        break;
                    case 5:
                        blue = GetSecondaryDesc(value, secondaryValue, saturation);
                        break;

                    // Green

                    case 1:
                        blue = GetTint(value, saturation);
                        break;
                    case 2:
                        blue = GetSecondaryAsc(value, secondaryValue, saturation);
                        break;

                    // Blue

                    case 3:
                        blue = value;
                        break;
                    case 4:
                        blue = value;
                        break;

                    default:
                        throw new ApplicationException($"Invalid hue value: {hue}");
                }
            }
            else
            {
                blue = value;
            }

            return (byte)(blue * 255D);
        }

        private static void GetPosition(double hue, out int sectionIndex, out double secondaryValue)
        {
            var position = hue / 60D;

            sectionIndex = (int)position;
            secondaryValue = position % 1;
        }

        private static double GetSecondaryAsc(double primary, double secondary, double saturation) =>
            primary * (1D - saturation * (1D - secondary));

        private static double GetSecondaryDesc(double primary, double secondary, double saturation) => 
            primary * (1D - saturation * secondary);

        private static double GetTint(double primary, double saturation) =>
            primary * (1D - saturation);

        private static double GetHue(double value)
        {
            if (value >= 360)
                return value % 360;

            if (value < 0)
                return 360 - Math.Abs(value) % 360;

            return value;
        }

        private static double GetPercent(double value)
        {
            if (value < 0)
                return 0;

            if (value > 1)
                return 1;

            return value;
        }

        #endregion
    }
}