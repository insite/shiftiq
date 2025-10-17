using System;
using System.Collections.Generic;

namespace Shift.Common.Colors
{
    public static class Palette
    {
        private const int DefaultRandomSeed = 1;
        private const double DefaultPaletteSaturation = 0.61;
        private const double DefaultGreenBrightnessCompensationValue = 0.775;

        public static IReadOnlyList<System.Drawing.Color> GenerateColorPalette(int count) =>
            GenerateColorPalette(count, DefaultRandomSeed, DefaultPaletteSaturation, DefaultGreenBrightnessCompensationValue);

        public static IReadOnlyList<System.Drawing.Color> GenerateColorPalette(int count, int randomSeed) =>
            GenerateColorPalette(count, randomSeed, DefaultPaletteSaturation, DefaultGreenBrightnessCompensationValue);

        public static IReadOnlyList<System.Drawing.Color> GenerateColorPalette(int count, double saturation) =>
            GenerateColorPalette(count, DefaultRandomSeed, saturation, DefaultGreenBrightnessCompensationValue);

        public static IReadOnlyList<System.Drawing.Color> GenerateColorPalette(int count, double saturation, double greenValue) =>
            GenerateColorPalette(count, DefaultRandomSeed, saturation, greenValue);

        public static IReadOnlyList<System.Drawing.Color> GenerateColorPalette(int count, int randomSeed, double saturation) =>
            GenerateColorPalette(count, randomSeed, saturation, DefaultGreenBrightnessCompensationValue);

        public static IReadOnlyList<System.Drawing.Color> GenerateColorPalette(int count, int randomSeed, double saturation, double greenValue)
        {
            if (count <= 0)
                return new System.Drawing.Color[0];

            if (greenValue > 1)
                greenValue = 1;
            else if (greenValue < 0)
                greenValue = 0;

            var greenMax = (double)Hsv.GetGreen(120, saturation, 1);
            var greenMin = (double)Hsv.GetGreen(120, saturation, greenValue);
            var greenRange = greenMax - greenMin;
            var greenValueRange = 1 - greenValue;

            var step = 360D / count;
            var hsv = new Hsv(0, saturation, 1);
            var palette = new List<System.Drawing.Color>(count);

            for (var hue = 0D; hue < 360D; hue += step)
            {
                hsv.Hue = hue;
                hsv.Value = 1;

                var green = (double)hsv.GetGreen();
                if (green == greenMax)
                    hsv.Value = greenValue;
                else if (green > greenMin)
                    hsv.Value = 1 - greenValueRange * ((green - greenMin) / greenRange);

                palette.Add(hsv.GetColor());
            }

            if (randomSeed >= 0)
            {
                var random = new Random(randomSeed);

                for (var i = 0; i < palette.Count - 1; i++)
                {
                    var index = random.Next(i, palette.Count);

                    var buffer = palette[i];
                    palette[i] = palette[index];
                    palette[index] = buffer;
                }
            }

            return palette.ToArray();
        }
    }
}