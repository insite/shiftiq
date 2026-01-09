using System;
using System.Collections.Generic;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Shift.Toolbox.Progress
{
    internal class LogbookResultDocument : IDocument
    {
        private readonly string _logoPath;
        private readonly string _personFullName;
        private readonly string _personEmail;
        private readonly string _personLogbookTitle;
        private readonly string _numberOfHoursText;

        private readonly List<Experience> _experiences;
        private readonly List<Area> _areas;
        private readonly List<Comment> _comments;

        private readonly bool _showSkillRating;

        public LogbookResultDocument(LogbookModel logbookModel)
        {
            if (logbookModel == null)
                throw new ArgumentNullException("logbookModel");

            if (string.IsNullOrEmpty(logbookModel.PersonFullName))
                throw new ArgumentNullException("logbookModel.PersonFullName");

            if (string.IsNullOrEmpty(logbookModel.PersonEmail))
                throw new ArgumentNullException("logbookModel.PersonEmail");

            if (string.IsNullOrEmpty(logbookModel.LogbookTitle))
                throw new ArgumentNullException("logbookModel.LogbookTitle");

            if (string.IsNullOrEmpty(logbookModel.OrganizationLogoPath))
                throw new ArgumentNullException("logbookModel.OrganizationLogoPath");


            _logoPath = logbookModel.OrganizationLogoPath;
            _personFullName = logbookModel.PersonFullName;
            _personEmail = logbookModel.PersonEmail;
            _personLogbookTitle = logbookModel.LogbookTitle;
            _experiences = logbookModel.Experiences;
            _areas = logbookModel.Areas;
            _comments = logbookModel.Comments;
            _showSkillRating = logbookModel.ShowSkillRating;
            _numberOfHoursText = logbookModel.NumberOfHoursText;
        }

        #region IDocument

        DocumentMetadata IDocument.GetMetadata() => DocumentMetadata.Default;
        DocumentSettings IDocument.GetSettings() => DocumentSettings.Default;

        void IDocument.Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.MarginHorizontal(30);
                    page.MarginVertical(20);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().Text(text => text.CurrentPageNumber().Style(QuestPdfText.NormalText));
                });
        }

        #endregion

        #region Compose Pdf

        void ComposeContent(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Element(ComposeLogo);
                column.Item().Element(ComposeAssessmentInfo);
                column.Item().Element(ComposePersonInfo);
                column.Item().Element(ComposeAdditionalInfo);

                Header(column, "Logbook Entries");
                foreach (var item in _experiences)
                    column.Item().PaddingBottom(10).Component(new LogbookExperienceItem(item));

                Header(column, "Progress");
                column.Item().Element(ComposeProgressTable);

                Header(column, "Comments");
                foreach (var item in _comments)
                    column.Item().PaddingBottom(10).Component(new LogbookCommentItem(item));

            });

        }

        private void Header(ColumnDescriptor column, string value)
        {
            column.Item().Element(HeaderStyle).AlignCenter().Text(value);
            column.Item().PaddingTop(10);
        }

        private void ComposeProgressTable(IContainer container)
        {
            if (_areas == null || _areas.Count == 0)
                return;

            container.Table(table =>
            {
                table.Header(header =>
                {
                    header.Cell().Padding(5).Text("Competency").Style(QuestPdfText.BoldText);
                    header.Cell().Padding(5).Text(_numberOfHoursText).Style(QuestPdfText.BoldText);
                    header.Cell().Padding(5).Text("Number of Log Entries").Style(QuestPdfText.BoldText);

                    if (_showSkillRating)
                        header.Cell().Padding(5).Text("Required Skill Rating").Style(QuestPdfText.BoldText);
                });

                table.ColumnsDefinition(columns =>
                {
                    if (_showSkillRating)
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    }
                    else
                    {
                        columns.RelativeColumn(5);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    }
                });

                var cellCount = _showSkillRating ? 4u : 3u;

                foreach (var item in _areas)
                {
                    table.Cell().ColumnSpan(cellCount).Component(new LogbookAreaItem(item, _showSkillRating));
                }
            });
        }

        void ComposeLogo(IContainer container)
        {
            try
            {
                var logo = QuestPDFImageHelper.GetLogo(_logoPath);

                container.Column(column =>
                {
                    column.Item().Width(150).Image(logo).FitWidth();
                });
            }
            catch (Exception) { }
        }

        IContainer HeaderStyle(IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderTop(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Background(Colors.Grey.Lighten4)
                .Padding(10);
        }

        void ComposePersonInfo(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Line(_personFullName).Style(QuestPdfText.BoldText);
                    text.Line(_personEmail).Style(QuestPdfText.NormalText);
                });
            });
        }

        void ComposeAssessmentInfo(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Line(_personLogbookTitle).Style(QuestPdfText.ReportTitle);
                });
            });
        }

        void ComposeAdditionalInfo(IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Text(text =>
                {
                    text.Line($"Date: {DateTime.Now.ToString("MMMM dd, yyyy")}").Style(QuestPdfText.NormalText);
                });
            });
        }

        #endregion
    }
}
