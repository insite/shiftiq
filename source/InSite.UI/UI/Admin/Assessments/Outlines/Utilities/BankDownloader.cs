using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;
using Shift.Toolbox;

namespace InSite.Admin.Assessments.Outlines.Utilities
{
    public class BankDownloader
    {
        private readonly TimeZoneInfo _tz;

        public BankDownloader(TimeZoneInfo tz)
        {
            _tz = tz;
        }

        public int DownloadCommentsToExcel(BankState bank, HttpResponse response)
        {
            var comments = GetComments(bank);
            if (comments.Count == 0)
                return 0;

            var bytes = GetExportData(comments);
            var filename = string.Format("{0} Comments {1:yyyy-MM-dd}",
                StringHelper.Sanitize(bank.Name, '_'), DateTime.Now);

            response.SendFile(filename, "xlsx", bytes);

            return comments.Count;
        }

        private List<BankExportCommentItem> GetComments(BankState bank)
        {
            var analyzer = new BankAnalyzer(bank);

            var comments = ServiceLocator.BankSearch.GetComments(bank.Identifier);

            var items = comments
                .OrderByDescending(x => x.CommentPosted)
                .Select(x => new BankExportCommentItem
                {
                    CommentPosted = x.CommentPosted.Format(_tz),
                    CommentRevised = x.CommentRevised.Format(_tz, nullValue: string.Empty),
                    CommentText = x.CommentText,
                    CommentCategory = x.CommentCategory,
                    CommentFlag = x.CommentFlag,
                    CommentAuthorType = x.AuthorUserRole,

                    AuthorName = x.AuthorUserName,
                    RevisorName = x.RevisorUserName,

                    ContainerSubtype = x.ContainerSubtype,
                    ContainerDescription = x.ContainerDescription,
                    BankIdentifier = x.AssessmentBankIdentifier.Value,
                    FieldIdentifier = x.AssessmentFieldIdentifier,
                    FormIdentifier = x.AssessmentFormIdentifier,
                    QuestionIdentifier = x.AssessmentQuestionIdentifier,
                    SpecificationIdentifier = x.AssessmentSpecificationIdentifier,

                    InstructorName = x.TrainingProviderGroupName,
                    EventDate = x.EventStarted.FormatDateOnly(_tz, nullValue: string.Empty),
                    EventFormat = x.EventFormat
                })
                .ToList();

            SetSubjectFields(bank, analyzer, items);

            return items;
        }

        private void SetSubjectFields(BankState bank, BankAnalyzer analyzer, List<BankExportCommentItem> items)
        {
            foreach (var item in items)
            {
                var type = item.ContainerSubtype.ToEnum<CommentType>();

                if (type == CommentType.Question)
                {
                    var question = bank.FindQuestion(item.QuestionIdentifier.Value);
                    if (question != null)
                    {
                        item.QuestionSequenceInBank = $"{question.BankIndex + 1}";
                        item.QuestionFormName = GetFormNames(question);
                        item.QuestionSequenceInForm = GetFormSequences(question, analyzer);
                        item.QuestionAsset = $"{question.Asset}.{question.AssetVersion}";

                        var competency = GetCompetency(question.Standard);
                        if (competency != null)
                        {
                            item.QuestionCompetencyArea = competency.AreaCode;
                            item.QuestionCompetencyItem = $"{competency.AreaCode}{competency.CompetencyCode}";
                        }
                    }
                }
                else if (type == CommentType.Field)
                {
                    var field = bank.FindField(item.FieldIdentifier.Value);
                    if (field != null)
                    {
                        item.QuestionSequenceInBank = $"{field.Question.BankIndex + 1}";
                        item.QuestionFormName = field.Section.Form.Name;
                        item.QuestionSequenceInForm = field.FormSequence.ToString();
                        item.QuestionAsset = $"{field.Question.Asset}.{field.Question.AssetVersion}";

                        var competency = GetCompetency(field.Question.Standard);
                        if (competency != null)
                        {
                            item.QuestionCompetencyArea = competency.AreaCode;
                            item.QuestionCompetencyItem = $"{competency.AreaCode}{competency.CompetencyCode}";
                        }
                    }
                }
                else if (type == CommentType.Form)
                {
                    var form = bank.FindForm(item.FormIdentifier.Value);
                    if (form != null)
                        item.QuestionFormName = form.Name;
                }
            }
        }

        private byte[] GetExportData(List<BankExportCommentItem> items)
        {
            var helper = new XlsxExportHelper();

            helper.Map("QuestionAsset", "Question Asset #", 16, HorizontalAlignment.Right);
            helper.Map("QuestionSequenceInBank", "Bank Question #", 16, HorizontalAlignment.Right);
            helper.Map("QuestionFormName", "Form Name", 30, HorizontalAlignment.Left);
            helper.Map("QuestionSequenceInForm", "Form Question #", 16, HorizontalAlignment.Right);
            helper.Map("QuestionCompetencyArea", Shift.Constant.StandardType.Area, 12, HorizontalAlignment.Center);
            helper.Map("QuestionCompetencyItem", "Competency", 12, HorizontalAlignment.Center);

            helper.Map("ContainerSubtype", "Subject Type", 15, HorizontalAlignment.Left);
            helper.Map("ContainerDescription", "Subject Title", 40, HorizontalAlignment.Left);

            helper.Map("CommentText", "Comment", 40, HorizontalAlignment.Left);
            helper.Map("CommentCategory", "Category", 20, HorizontalAlignment.Left);
            helper.Map("CommentFlag", "Flag", 6, HorizontalAlignment.Left);

            helper.Map("InstructorName", "Instructor", 20, HorizontalAlignment.Left);

            helper.Map("EventDate", "Event Date", 12, HorizontalAlignment.Left);
            helper.Map("EventFormat", "Event Format", 20, HorizontalAlignment.Left);

            helper.Map("CommentAuthorType", "Author Type", 15, HorizontalAlignment.Left);
            helper.Map("AuthorName", "Author", 20, HorizontalAlignment.Left);
            helper.Map("RevisorName", "Revisor", 20, HorizontalAlignment.Left);

            helper.Map("CommentPosted", "Posted", 26, HorizontalAlignment.Left);
            helper.Map("CommentRevised", "Revised", 26, HorizontalAlignment.Left);

            return helper.GetXlsxBytes(items, "Comments");
        }

        private string GetFormNames(Question q)
        {
            var sb = new StringBuilder();

            foreach (var field in q.Fields)
            {
                if (field.Section != null && field.Section.Form != null)
                {
                    if (sb.Length > 0)
                        sb.Append(", ");

                    sb.Append(field.Section.Form.Name);
                }
            }

            return sb.ToString();
        }

        private string GetFormSequences(Question q, BankAnalyzer analyzer)
        {
            var sb = new StringBuilder();

            foreach (var field in q.Fields)
            {
                if (field.Section != null && field.Section.Form != null)
                {
                    if (sb.Length > 0) sb.Append(", ");
                    sb.Append(1 + analyzer.GetQuestionIndex(q, field.Section.Form));
                }
            }

            return sb.ToString();
        }

        private VCompetency GetCompetency(Guid standard) =>
            standard != Guid.Empty ? ServiceLocator.OldStandardSearch.GetCompetency(standard) : null;
    }
}