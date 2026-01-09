using System;
using System.Collections.Generic;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Shift.Utility.Assessments.Models;

namespace Shift.Toolbox.Assessments
{
    internal class AssessmentResultDocument : IDocument
    {
        #region Constructor and fields

        private readonly List<QuestionItem> _questionItems;

        private readonly Image _correct;
        private readonly Image _incorrect;
        private readonly Image _correctSqr;
        private readonly Image _incorrectSqr;
        private readonly Image _empty;
        private readonly Image _audio;

        private readonly string _logoPath;
        private readonly string _personFullName;
        private readonly string _personEmail;
        private readonly string _personAssessmentTitle;

        public AssessmentResultDocument(PersonAssessment personAssessmentModel)
        {
            if (personAssessmentModel == null)
                throw new ArgumentNullException("personAssessmentModel");

            if (string.IsNullOrEmpty(personAssessmentModel.PersonFullName))
                throw new ArgumentNullException("personAssessmentModel.PersonFullName");

            if (string.IsNullOrEmpty(personAssessmentModel.PersonEmail))
                throw new ArgumentNullException("personAssessmentModel.PersonEmail");

            if (string.IsNullOrEmpty(personAssessmentModel.AssessmentTitle))
                throw new ArgumentNullException("personAssessmentModel.AssessmentTitle");

            if (string.IsNullOrEmpty(personAssessmentModel.OrganizationLogoPath))
                throw new ArgumentNullException("personAssessmentModel.OrganizationLogoPath");

            if (personAssessmentModel.QuestionItems == null)
                throw new ArgumentNullException("personAssessmentModel.QuestionItems");

            _questionItems = personAssessmentModel.QuestionItems;
            _logoPath = personAssessmentModel.OrganizationLogoPath;
            _personFullName = personAssessmentModel.PersonFullName;
            _personEmail = personAssessmentModel.PersonEmail;
            _personAssessmentTitle = personAssessmentModel.AssessmentTitle;

            _correct = GetImage(QuestPdfImage.CorrectResource);
            _incorrect = GetImage(QuestPdfImage.IncorrectResource);
            _correctSqr = GetImage(QuestPdfImage.CorrectSqrResource);
            _incorrectSqr = GetImage(QuestPdfImage.IncorrectSqrResource);
            _empty = GetImage(QuestPdfImage.EmptyResource);
            _audio = GetImage(QuestPdfImage.AudioResource);

        }

        #endregion

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
                column.Spacing(5);
                column.Item().Element(ComposeLogo);
                column.Item().Element(ComposeAssessmentInfo);
                column.Item().Element(ComposePersonInfo);
                column.Item().Element(ComposeAdditionalInfo);
                foreach (var item in _questionItems)
                    column.Item().PaddingBottom(10).Component(new AssessmentItem(item, _correct, _incorrect, _empty, _audio, _correctSqr, _incorrectSqr));
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
            catch(Exception){}
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
                    text.Line(_personAssessmentTitle).Style(QuestPdfText.ReportTitle);
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

        #region Helper Methods

        Image GetImage(string value)
        {
            using (var stream = GetType().Assembly.GetManifestResourceStream(value))
            {
                stream.Position = 0;
                return Image.FromStream(stream);
            }
        }

        #endregion
    }
}