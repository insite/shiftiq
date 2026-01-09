using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Shift.Utility.Assessments.Models;
using SkiaSharp;
using System.Linq;

namespace Shift.Toolbox.Assessments 
{
    internal class AssessmentOrderItem : IComponent
    {
        private IOrderedEnumerable<OptionItem> _optionItems;
        private Image _correctSqr;
        private Image _incorrectSqr;
        private decimal? _answerPoints;

        public AssessmentOrderItem(IOrderedEnumerable<OptionItem> optionItems, Image correctSqr, Image incorrectSqr, decimal? answerPoints)
        {
            _optionItems = optionItems;
            _correctSqr = correctSqr;
            _incorrectSqr = incorrectSqr;
            _answerPoints = answerPoints;
        }

        public void Compose(IContainer container)
        {
            container.Row(row =>
            {
                row.AutoItem()
                .AlignLeft()
                .Width(470)
                .PaddingLeft(20)
                .Element(ComposeOrderedSequence);

                row.AutoItem()
                    .AlignRight()
                    .PaddingLeft(20)
                    .Width(20)
                    .ShowOnce()
                    .Image(((!_answerPoints.HasValue || _answerPoints.Value == 0) ? _incorrectSqr : _correctSqr ));

            });
        }

        void ComposeOrderedSequence(IContainer container)
        {
            container.Column(column =>
            {
                foreach (var item in _optionItems)
                {
                    column.Item()
                    .Layers(layers =>
                    {
                        layers.Layer().SkiaSharpCanvas((canvas, size) =>
                        {
                            var cardBorder = SKColor.Parse("#e3e9ef");

                            DrawRoundedRectangle(Colors.White, false);
                            DrawRoundedRectangle(item.HasPoints ? Colors.Green.Lighten2 : Colors.Red.Lighten2, true);

                            void DrawRoundedRectangle(string color, bool isStroke)
                            {
                                using (var paint = new SKPaint()
                                {
                                    IsStroke = isStroke,
                                    StrokeWidth = 1,
                                    IsAntialias = true,

                                })
                                {
                                    paint.Color = SKColor.Parse(color);
                                    canvas.DrawRoundRect(0, 0, size.Width, size.Height, 10, 10, paint);
                                }
                            }
                        });

                        layers.PrimaryLayer().AlignMiddle().Padding(5).Text(item.Text).Style(QuestPdfText.NormalText);


                    });

                    column.Spacing(10);
                }

            });
        }

    }
}
