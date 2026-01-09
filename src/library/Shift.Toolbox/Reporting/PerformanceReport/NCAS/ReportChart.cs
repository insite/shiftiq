using System;
using System.IO;
using System.Text;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

using SkiaSharp;

namespace Shift.Toolbox.Reporting.PerformanceReport.NCAS
{
    internal class ReportChart : IComponent
    {
        const float LineWidth = 15;
        const float SectionYOffset = 5f;

        private readonly DocumentText.Text _text;
        private readonly decimal _emergentScore;
        private readonly decimal _consistentScore;
        private readonly decimal _maxScore;
        private readonly ChartItem[] _chartItems;
        private readonly string _fontFamilyName;

        private readonly TextStyle _normalText;
        private readonly SKFont _labelFont;
        private readonly SKPaint _labelPaint;
        private readonly SKPaint _barAreaPaint;
        private readonly SKPaint _barPaint;
        private readonly SKPaint _dashedLinePaint;

        public ReportChart(
            DocumentText.Text text,
            decimal emergentScore,
            decimal consistentScore,
            decimal maxScore,
            ChartItem[] chartItems,
            string fontFamilyName
            )
        {
            _text = text;
            _emergentScore = emergentScore;
            _consistentScore = consistentScore;
            _maxScore = maxScore;
            _chartItems = chartItems;

            _fontFamilyName = fontFamilyName;

            _normalText = TextStyle.Default
                .FontFamily(_fontFamilyName)
                .FontColor(Colors.Black)
                .FontSize(10);

            _labelFont = new SKFont(SKTypeface.FromFamilyName(_fontFamilyName), 8);
            _labelPaint = new SKPaint { Color = SKColor.Parse("#564D4D") };

            _barAreaPaint = new SKPaint { Color = SKColor.Parse("#EBEBEB") };
            _barPaint = new SKPaint { Color = SKColor.Parse("#4F94CD") };

            _dashedLinePaint = new SKPaint
            {
                Color = SKColor.Parse("#4D4D4D"),
                PathEffect = SKPathEffect.CreateDash(new[] { 5f, 3f }, 0),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 0.5f
            };
        }

        void IComponent.Compose(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(20);
                    columns.RelativeColumn();
                });

                table.Cell()
                    .RotateRight()
                    .RotateRight()
                    .RotateRight()
                    .AlignCenter()
                    .Text(_text.CompetencyDimension)
                    .Style(_normalText);

                table.Cell()
                    .ExtendHorizontal()
                    .Height(GetChartHeight())
                    .Svg(size => DrawChart(size));
            });
        }

        float GetChartHeight()
        {
            var barAreaHeight = _chartItems.Length * LineWidth;
            return barAreaHeight + SectionYOffset + _labelFont.Size;
        }

        string DrawChart(Size size)
        {
            using (var stream = new MemoryStream())
            {
                using (var canvas = SKSvgCanvas.Create(new SKRect(0, 0, size.Width, size.Height), stream))
                    DrawChart(canvas, size);

                var svgData = stream.ToArray();
                return Encoding.UTF8.GetString(svgData);
            }
        }

        void DrawChart(SKCanvas canvas, Size size)
        {
            const float barWidth = 11;
            const float barAreaXOffset = 3;

            var font = _labelFont;
            var paint = _labelPaint;
            var labelWidth = CalcLabelWidth(font, size.Width);
            var textYOffset = (LineWidth + font.Size) / 2;
            var barYOffset = (LineWidth - barWidth) / 2;
            var y = 0f;
            var barAreaX = labelWidth + barAreaXOffset;
            var barXOffset = 0.5f;
            var barX = barAreaX + barXOffset;
            var barAreaWidth = size.Width - barAreaX;
            var barAreaHeight = LineWidth * _chartItems.Length;
            var barMaxWidth = barAreaWidth - 2 * barXOffset;
            var gridCellWidth = barMaxWidth / 10;

            canvas.DrawRect(barAreaX, 0, barAreaWidth, barAreaHeight, _barAreaPaint);

            DrawGrid(canvas, size, barAreaX + barXOffset, 0, gridCellWidth, LineWidth);

            foreach (var item in _chartItems)
            {
                var (adjustedLabel, width) = AdjustLabelText(font, item.Label ?? "Unnamed", labelWidth);
                var textX = Math.Max(labelWidth - width, 0);
                var barHeight = barMaxWidth * (float)item.Score / (float)_maxScore;

                canvas.DrawText(adjustedLabel, textX, y + textYOffset, font, paint);
                canvas.DrawRect(barX, y + barYOffset, barHeight, barWidth, _barPaint);

                y += LineWidth;
            }

            DrawSections(canvas, font, paint, barAreaHeight, barX, barMaxWidth);
        }

        float CalcLabelWidth(SKFont font, float areaWidth)
        {
            const float maxLabelWidthRatio = 0.7f;
            const float minLabelWidthRatio = 0.4f;

            float labelWidth = 0;

            foreach (var item in _chartItems)
            {
                var currentLabelWidth = font.MeasureText(item.Label);
                if (labelWidth < currentLabelWidth)
                    labelWidth = currentLabelWidth;
            }

            float maxLabelWidth = areaWidth * maxLabelWidthRatio;
            if (labelWidth > maxLabelWidth)
                return maxLabelWidth;

            float minLabelWidth = areaWidth * minLabelWidthRatio;
            if (labelWidth < minLabelWidth)
                return minLabelWidth;

            return labelWidth;
        }

        (string, float) AdjustLabelText(SKFont font, string label, float maxWidth)
        {
            var width = font.MeasureText(label);
            if (width <= maxWidth)
                return (label, width);

            int charCount = (int)(maxWidth / width * label.Length);
            var subText = label.Substring(0, charCount) + " ...";

            var newWidth = font.MeasureText(subText);
            while (newWidth < maxWidth)
            {
                charCount++;
                subText = label.Substring(0, charCount) + " ...";
                newWidth = font.MeasureText(subText);
            }

            while (newWidth > maxWidth)
            {
                charCount--;
                subText = label.Substring(0, charCount) + " ...";
                newWidth = font.MeasureText(subText);
            }

            return (subText, newWidth);
        }

        void DrawGrid(SKCanvas canvas, Size size, float x, float offset, float gridCellWidth, float gridCellHeight)
        {
            var paint = new SKPaint { Color = SKColor.Parse("#fff"), Style = SKPaintStyle.Stroke, StrokeWidth = 0.5f };
            var y = 0f;

            while (y < size.Height)
            {
                canvas.DrawLine(x, y + offset, size.Width, y + offset, paint);
                y += gridCellHeight;
            }

            while (x < size.Width)
            {
                canvas.DrawLine(x + offset, 0, x + offset, size.Height, paint);
                x += gridCellWidth;
            }
        }

        void DrawSections(SKCanvas canvas, SKFont font, SKPaint paint, float barAreaHeight, float barX, float barMaxWidth)
        {
            var sectionWidth1 = barMaxWidth * (float)_emergentScore;
            var sectionWidth2 = barMaxWidth * (float)(_consistentScore - _emergentScore);
            var sectionWidth3 = barMaxWidth - sectionWidth1 - sectionWidth2;

            var sectionX1 = barX + sectionWidth1;
            var sectionX2 = sectionX1 + sectionWidth2;

            var labelY = barAreaHeight + SectionYOffset + font.Size;

            if (!string.IsNullOrEmpty(_text.Undemonstrated))
            {
                var sectionLabelX1 = barX + (sectionWidth1 - font.MeasureText(_text.Undemonstrated)) / 2;
                canvas.DrawText(_text.Undemonstrated, sectionLabelX1, labelY, font, paint);
            }

            if (!string.IsNullOrEmpty(_text.Emergent))
            {
                var sectionLabelX2 = sectionX1 + (sectionWidth2 - font.MeasureText(_text.Emergent)) / 2;
                canvas.DrawText(_text.Emergent, sectionLabelX2, labelY, font, paint);
            }

            if (!string.IsNullOrEmpty(_text.Consistent))
            {
                var sectionLabelX3 = sectionX2 + (sectionWidth3 - font.MeasureText(_text.Consistent)) / 2;
                canvas.DrawText(_text.Consistent, sectionLabelX3, labelY, font, paint);
            }

            canvas.DrawLine(sectionX1, 0, sectionX1, barAreaHeight, _dashedLinePaint);
            canvas.DrawLine(sectionX2, 0, sectionX2, barAreaHeight, _dashedLinePaint);
        }
    }
}
