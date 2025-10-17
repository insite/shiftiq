using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Shift.Utility.Assessments.Models;
using SkiaSharp;
using System.Linq;

namespace Shift.Toolbox.Assessments
{
    internal class AssessmentItem : IComponent
    {
        private readonly QuestionItem _questionItem;
        private readonly Image _correct;
        private readonly Image _incorrect;
        private readonly Image _correctSqr;
        private readonly Image _incorrectSqr;
        private readonly Image _empty;
        private readonly Image _audio;

        public AssessmentItem(QuestionItem questionItem, Image correct, Image incorrect, Image empty, Image audio, Image correctSqr, Image incorrectSqr)
        {
            _questionItem = questionItem;
            _correct = correct;
            _incorrect = incorrect;
            _empty = empty;
            _audio = audio;
            _correctSqr = correctSqr;
            _incorrectSqr = incorrectSqr;
        }

        public void Compose(IContainer container)
        {
            container.Column(column =>
            {
                column.Item()
                .Layers(layers =>
                {
                    layers.Layer().SkiaSharpCanvas((canvas, size) =>
                    {
                        DrawRoundedRectangle(Colors.White, false);
                        DrawRoundedRectangle(QuestPdfStyle.CardBorderColor, true);

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

                    if (_questionItem.IsListBox)
                        layers.PrimaryLayer().Element(ComposeOptionList);
                    else if (_questionItem.IsComposedEssay)
                        layers.PrimaryLayer().Element(ComposeEssay);
                    else if (_questionItem.IsComposedVoice)
                        layers.PrimaryLayer().Element(ComposeVoice);
                    else if (_questionItem.IsOrdering)
                        layers.PrimaryLayer().Element(ComposeOrder);
                    else
                        layers.PrimaryLayer().Element(NotSupportedYet);

                });
            });
        }

        void ComposeOptionList(IContainer container)
        {
            container.Column(column =>
            {
                QuestionHeader(column);

                if (_questionItem.Options == null || _questionItem.Options.Count == 0)
                    return;

                foreach (var option in _questionItem.Options)
                    column.Item().ShrinkVertical().Component(new AssessmentListOptionItem(option, _correct, _incorrect, _empty));

                AnswerFooter(column);
            });
        }

        void ComposeEssay(IContainer container)
        {
            container.Column(column =>
            {
                QuestionHeader(column);

                column.Item().Component(new AssessmentEssayItem(_questionItem.AnswerText, _correctSqr, _incorrectSqr, _questionItem.AnswerPoints));

                AnswerFooter(column);
            });
        }

        void ComposeVoice(IContainer container)
        {
            container.Column(column =>
            {
                QuestionHeader(column);

                column.Item().Component(new AssessmentVoiceItem(_audio, _correctSqr, _incorrectSqr, _questionItem.AnswerPoints));

                AnswerFooter(column);
            });
        }

        void ComposeOrder(IContainer container)
        {
            container.Column(column =>
            {
                QuestionHeader(column);

                column.Item().Component(new AssessmentOrderItem(_questionItem.Options.OrderBy(x => x.AnswerSequence).ThenBy(x => x.Sequence), _correctSqr, _incorrectSqr, _questionItem.AnswerPoints));

                AnswerFooter(column);
            });
        }

        void NotSupportedYet(IContainer container)
        {
            string paragraph1 = "This type of question/answer is not supported yet.";

            container.Column(column =>
            {
                QuestionHeader(column);
                column.Item().Text(paragraph1);
                AnswerFooter(column);
            });
        }

        private void QuestionHeader(ColumnDescriptor column)
        {
            column.Item().PaddingHorizontal(10).PaddingTop(15).Element(ComposeQuestionHeader);
            column.Item().Padding(5).PaddingBottom(10).LineHorizontal(1).LineColor(QuestPdfStyle.CardBorderColor);
        }

        private void AnswerFooter(ColumnDescriptor column)
        {
            column.Item().PaddingTop(10);

            if (!(_questionItem.HasRationale || _questionItem.HasRationaleForCorrect || _questionItem.HasRationaleForIncorrect || _questionItem.HasFeedbackPoints))
                return;

            column.Item().Padding(5).LineHorizontal(1).LineColor(QuestPdfStyle.CardBorderColor);

            if (_questionItem.HasRationale)
                column.Item().PaddingHorizontal(10).PaddingTop(10).Element(ComposeAnswerFooterRationale);

            if (_questionItem.HasRationaleForCorrect)
                column.Item().PaddingHorizontal(10).PaddingTop(10).Element(ComposeAnswerFooterRationaleForCorrect);

            if (_questionItem.HasRationaleForIncorrect)
                column.Item().PaddingHorizontal(10).PaddingTop(10).Element(ComposeAnswerFooterRationaleForIncorrect);

            if (_questionItem.HasFeedbackPoints)
                column.Item().PaddingHorizontal(10).PaddingTop(10).Element(ComposeAnswerFooterPoints);
        }

        void ComposeQuestionHeader(IContainer container)
        {
            container.Column(column =>
            {
                column.Spacing(10);
                column.Item().Text(text =>
                {
                    text.Line($"Question {_questionItem.Number}").Style(QuestPdfText.BoldText);
                    text.Line(_questionItem.Text).Style(QuestPdfText.NormalText);
                });
            });
        }

        void ComposeAnswerFooterRationale(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Line("Rationale:").Style(QuestPdfText.BoldText);
                    text.Line(_questionItem.RationaleText)
                    .Style(QuestPdfText.NormalText);
                });
            });
        }

        void ComposeAnswerFooterRationaleForCorrect(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Line("Rationale:").Style(QuestPdfText.BoldText);
                    text.Line(_questionItem.RationaleTextOnCorrectAnswer)
                    .Style(QuestPdfText.NormalText);
                });
            });
        }

        void ComposeAnswerFooterRationaleForIncorrect(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Line("Rationale:").Style(QuestPdfText.BoldText);
                    text.Line(_questionItem.RationaleTextOnIncorrectAnswer)
                    .Style(QuestPdfText.NormalText);
                });
            });
        }

        void ComposeAnswerFooterPoints(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Line("Points:").Style(QuestPdfText.BoldText);
                    text.Line($"{string.Format("{0:n2}", _questionItem.AnswerPoints)}/{string.Format("{0:n2}", _questionItem.Points)}")
                    .Style(QuestPdfText.NormalText);
                });
            });
        }


    }
}
