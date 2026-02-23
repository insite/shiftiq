using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Shift.Common.Timeline.Changes;
using Shift.Common.Timeline.Commands;

using InSite.Application.Banks.Write;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Pages.Write;
using InSite.Application.Records.Read;
using InSite.Application.Resources.Read;
using InSite.Application.Sites.Read;
using InSite.Domain.Banks;

using Shift.Common;

namespace InSite.Application.Banks.Read
{
    /// <summary>
    /// Implements the process manager for Bank events. 
    /// </summary>
    /// <remarks>
    /// A process manager (sometimes called a saga in CQRS) is an independent component that reacts to domain events in 
    /// a cross-aggregate, eventually consistent manner. Time can be a trigger. Process managers are sometimes purely 
    /// reactive, and sometimes represent workflows. From an implementation perspective, a process manager is a state 
    /// machine that is driven forward by incoming events (which may come from many aggregates). Some states will have 
    /// side effects, such as sending commands, talking to external web services, or sending emails.
    /// </remarks>
    public class BankChangeProcessor
    {
        private readonly Regex _questionAttachmentPattern;

        private readonly ICommander _commander;

        private readonly IBankSearch _banks;
        private readonly IPageSearch _pages;
        private readonly IRecordSearch _records;
        private readonly IUploadSearch _uploads;

        public BankChangeProcessor(ICommander commander, IChangeQueue publisher, IBankSearch banks, IPageSearch pages, IRecordSearch records, IUploadSearch uploads, string domain)
        {
            _commander = commander;

            _banks = banks;
            _pages = pages;
            _records = records;
            _uploads = uploads;

            _questionAttachmentPattern = new Regex(
                $"!?\\[[^[]+]\\((?<Url>(?<Host>https?://[a-z-]+\\.{domain.Replace(".", "\\.")})?/files(?<UploadUrl>/[^(\"]+?))(?: \"[^(\"]+\")?\\)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled
            );

            publisher.Subscribe<FormDeleted>(Handle);
            publisher.Subscribe<QuestionAdded>(Handle);
            publisher.Subscribe<QuestionContentChanged>(Handle);
            publisher.Subscribe<QuestionOrderingOptionAdded>(Handle);
            publisher.Subscribe<QuestionOrderingOptionChanged>(Handle);
            publisher.Subscribe<QuestionOrderingOptionDeleted>(Handle);
            publisher.Subscribe<SpecificationCalculationChanged>(Handle);
        }

        private void Handle(FormDeleted deleted)
        {
            // When an assessment form is deleted from a bank, it is important to ensure there are no web pages that
            // reference the deleted form. Any such orphan references need to be removed without deleting the page(s)
            // that contain those references.

            var pages = _pages
                .Bind(x => x.PageIdentifier,
                      x => x.ObjectType == "Assessment" && x.ObjectIdentifier == deleted.Form)
                .ToList();

            foreach (var page in pages)
            {
                var command = new ChangePageAssessment(page, null);
                _commander.Send(command);
            }
        }

        public void Handle(QuestionAdded e)
        {
            OnQuestionContentChanged(e.Question);
        }

        public void Handle(QuestionContentChanged e)
        {
            OnQuestionContentChanged(e.Question);
        }

        public void Handle(QuestionOrderingOptionAdded e)
        {
            OnQuestionContentChanged(e.Question);
        }

        public void Handle(QuestionOrderingOptionChanged e)
        {
            OnQuestionContentChanged(e.Question);
        }

        public void Handle(QuestionOrderingOptionDeleted e)
        {
            OnQuestionContentChanged(e.Question);
        }

        public void Handle(SpecificationCalculationChanged change)
        {
            // If the passing score has changed, then send a command to update the passing score on all related grade
            // items.
            var spec = _banks.GetSpecification(change.Specification);
            var forms = _banks.GetForms(new QBankFormFilter { SpecIdentifier = spec.SpecIdentifier });
            foreach (var form in forms)
            {
                var gradeitems = _records.GetGradeItems(new QGradeItemFilter { AssessmentFormIdentifier = form.FormIdentifier }).ToArray();
                foreach (var gradeitem in gradeitems)
                {
                    if (gradeitem.GradeItemPassPercent != change.Calculation.PassingScore)
                        _commander.Send(new ChangeGradeItemPassPercent(gradeitem.GradebookIdentifier, gradeitem.GradeItemIdentifier, change.Calculation.PassingScore));
                }
            }
        }

        #region Methods (questions attachments)

        private void OnQuestionContentChanged(Guid questionId)
        {
            var question = _banks.GetQuestionData(questionId);
            var commands = new List<ICommand>();

            GetQuestionAttachmentCommands(question, commands);

            foreach (var command in commands)
                _commander.Send(command);
        }

        private void GetQuestionAttachmentCommands(Question question, List<ICommand> commands)
        {
            var bank = question.Set.Bank;

            var attachmentMapping = new Dictionary<Guid, Guid>();
            var uploadMapping = new Dictionary<Guid, Guid>();

            foreach (var attachment in bank.EnumerateAllAttachments())
            {
                attachmentMapping.Add(attachment.Identifier, attachment.Upload);
                uploadMapping[attachment.Upload] = attachment.Identifier;
            }

            var uploads = GetQuestionUploads(question, uploadMapping);

            foreach (var attachmentId in question.AttachmentIdentifiers)
            {
                var isRemove = true;

                if (attachmentMapping.ContainsKey(attachmentId))
                {
                    var uploadId = attachmentMapping[attachmentId];
                    if (uploads.Contains(uploadId))
                    {
                        isRemove = false;
                        uploads.Remove(uploadId);
                    }
                }

                if (isRemove)
                    commands.Add(new DeleteAttachmentFromQuestion(bank.Identifier, attachmentId, question.Identifier));
            }

            foreach (var uploadId in uploads)
            {
                if (uploadMapping.ContainsKey(uploadId))
                    commands.Add(new AddAttachmentToQuestion(bank.Identifier, uploadMapping[uploadId], question.Identifier));
            }
        }

        private ICollection<Guid> GetQuestionUploads(Question question, Dictionary<Guid, Guid> uploadMapping)
        {
            if (uploadMapping.Count == 0)
                return new Guid[0];

            var strings = question.Content.GetItems().ToList();

            if (question.Type == QuestionItemType.Ordering)
                strings.AddRange(question.Ordering.Options.Select(x => x.Content.Title));

            var text = ToSingleString(strings);
            if (text.IsEmpty())
                return new Guid[0];

            var matches = _questionAttachmentPattern.Matches(text);
            if (matches.Count == 0)
                return new Guid[0];

            var urlFilter = matches.Cast<Match>().Select(m => m.Groups["UploadUrl"].Value)
                .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

            var values = _uploads
                .Bind(x => x.UploadIdentifier, x => uploadMapping.Keys.Contains(x.UploadIdentifier) && urlFilter.Contains(x.NavigateUrl));

            return new HashSet<Guid>(values);
        }

        private static string ToSingleString(IEnumerable<MultilingualString> input)
        {
            var result = new StringBuilder();

            foreach (var str in input)
            {
                if (str == null)
                    continue;

                foreach (var item in str)
                {
                    if (string.IsNullOrEmpty(item.Value))
                        continue;

                    if (result.Length > 0)
                        result.AppendLine().AppendLine();

                    result.Append(item.Value);
                }
            }

            return result.ToString();
        }

        #endregion
    }
}