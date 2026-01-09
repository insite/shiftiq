using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// A question bank is a planned library of question items designed to fulfill specific predetermined purposes. 
    /// 
    /// For example, a bank can contain hundreds of questions for each chapter in a published resource. This provides 
    /// an opportunity for teachers to select from a wide variety of questions when developing an exam, developing more 
    /// than one exam for a specific assessment strategy and rotate them between terms, or to draw a random set of 
    /// questions into an exam. 
    /// 
    /// The question items in a bank are grouped int question sets. This makes it easier for instructors and 
    /// administrators to work with large banks, and it allows for the creation of exam forms containing questions that
    /// naturally group together - and for which different display rules might need to apply. 
    /// 
    /// For example, consider an exam form that contain 20 questions, where the first 10 questions relate to topic A 
    /// and the second 10 questions relate to topic B. An instructor might want students to see all A questions before
    /// any B questions, and also want to randomize the question items in each of the two groups.
    /// </summary>
    [Serializable]
    public class BankState : AggregateState, IHasAssetNumber
    {
        #region Properties

        /// <summary>
        /// Banks are organized into types. Certain functions apply to one type and not to another type.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public BankType Type { get; set; }

        /// <summary>
        /// Uniquely identifies the bank itself.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Uniquely identifies the standard (e.g. competency framework) evaluated by the questions in the bank. If the
        /// bank does not evaluate any standard then Guid.Empty is assumed as a sentinel value.
        /// </summary>
        public Guid Standard { get; set; }

        /// <summary>
        /// Uniquely identifies the organization that owns the bank.
        /// </summary>
        public Guid Tenant { get; set; }
        public Guid Department { get; set; }

        /// <summary>
        /// The level (type and number) to which the questions in the bank apply.
        /// </summary>
        public Level Level { get; set; }

        /// <summary>
        /// The internal name used to uniquely identify this bank for filing purposes.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The current publication status of the bank.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Changes to questions contained in a locked bank are not permitted.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Bank Status Activity.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Every bank is assigned a unique asset number in the organization's inventory.
        /// </summary>
        public int Asset { get; set; }

        /// <summary>
        /// Banks need support for multilingual titles, summaries, materials, etc.
        /// </summary>
        public ContentExamBank Content { get; set; }

        /// <summary>
        /// What is the edition number on this bank?
        /// </summary>
        public Edition Edition { get; set; }

        /// <summary>
        /// A bank can have any number of attached files. Each file is stored in the file system with a physical file 
        /// name that looks like this - \Data\Files\Core\UploadIdentifier.extension - and information about the 
        /// uploaded file is stored in the database table Upload. The convention that should be followed for the 
        /// Upload.NavigateUrl property (used to view/download the file) is this:
        ///    /Assessments/{Bank.Asset}/Attachments/{Upload.Name}
        /// </summary>
        public List<Attachment> Attachments { get; set; }

        /// <summary>
        /// A bank can have any number of comments. Comments might be posted by instructors authoring questions,
        /// subject-matter-experts reviewing and revising questions, students submitting exams, etc.
        /// </summary>
        public List<Comment> Comments { get; set; }

        /// <summary>
        /// The question items in a bank are grouped into logical sets.
        /// </summary>
        public List<Set> Sets { get; set; }

        /// <summary>
        /// A bank can be used by any number of specifications for generating any number of exam forms.
        /// </summary>
        public List<Specification> Specifications { get; set; }

        #endregion

        #region Fields

        [JsonIgnore]
        public bool IsAdvanced => Type != BankType.Basic;

        [JsonProperty(PropertyName = "NextOptionNumber", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int? _nextOptionNumber;

        private readonly Dictionary<Guid, Comment> _commentIndex = new Dictionary<Guid, Comment>();
        private readonly Dictionary<Guid, Attachment> _attachmentIndex = new Dictionary<Guid, Attachment>();
        private readonly Dictionary<Guid, Field> _fieldIndex = new Dictionary<Guid, Field>();
        private readonly Dictionary<Guid, Form> _formIndex = new Dictionary<Guid, Form>();
        private readonly Dictionary<int, Option> _optionIndex = new Dictionary<int, Option>();
        private readonly Dictionary<Guid, Question> _questionIndex = new Dictionary<Guid, Question>();
        private readonly Dictionary<Guid, Section> _sectionIndex = new Dictionary<Guid, Section>();
        private readonly Dictionary<Guid, Set> _setIndex = new Dictionary<Guid, Set>();
        private readonly Dictionary<Guid, Criterion> _criterionIndex = new Dictionary<Guid, Criterion>();
        private readonly Dictionary<Guid, Specification> _specificationIndex = new Dictionary<Guid, Specification>();

        #endregion

        #region Construction

        /// <summary>
        /// Constructs an empty bank.
        /// </summary>
        public BankState()
        {
            Attachments = new List<Attachment>();
            Comments = new List<Comment>();
            Content = new ContentExamBank();
            Level = new Level();
            Sets = new List<Set>();
            Specifications = new List<Specification>();
            Edition = new Edition();
        }

        public BankState Clone()
        {
            var clone = new BankState();

            clone.Copy(this);

            return clone;
        }

        private void Copy(BankState bank)
        {
            bank.ShallowCopyTo(this);

            Level = bank.Level.Clone();
            Content = bank.Content.Clone();
            Edition = bank.Edition.Clone();
            Attachments = bank.Attachments.EmptyIfNull().Select(x => x.Clone()).ToList();
            Comments = bank.Comments.EmptyIfNull().Select(x => x.Clone()).ToList();
            Sets = bank.Sets.EmptyIfNull().Select(x => x.Clone()).ToList();
            Specifications = bank.Specifications.EmptyIfNull().Select(x => x.Clone()).ToList();

            _commentIndex.Clear();
            _attachmentIndex.Clear();
            _fieldIndex.Clear();
            _formIndex.Clear();
            _optionIndex.Clear();
            _questionIndex.Clear();
            _sectionIndex.Clear();
            _setIndex.Clear();
            _criterionIndex.Clear();
            _specificationIndex.Clear();

            _nextOptionNumber = null;

            RestoreReferences();
        }

        #endregion

        #region Modification

        private void OnBankCreated(Change _, BankState bank)
        {
            Copy(bank);

            _nextOptionNumber = null;

            if (bank._nextOptionNumber.HasValue && bank._nextOptionNumber.Value >= 1)
            {
                GetNextOptionNumber();

                _nextOptionNumber -= 1;

                if (bank._nextOptionNumber.Value > _nextOptionNumber.Value)
                    _nextOptionNumber = bank._nextOptionNumber;

            }

            RestoreReferences();

            foreach (var s in Specifications)
            {
                s.RestoreReferences();

                foreach (var f in s.EnumerateAllForms())
                    f.RestoreReferences();
            }

            foreach (var s in Sets)
            {
                s.RestoreReferences();

                foreach (var q in s.EnumerateAllQuestions())
                {
                    q.RestoreReferences();

                    if (q.Type == QuestionItemType.Likert)
                    {
                        foreach (var o in q.Likert.Options)
                        {
                            if (o.Number == default)
                                o.Number = GetNextOptionNumber();
                        }
                    }
                    else if (q.Type.IsHotspot())
                    {
                        foreach (var o in q.Hotspot.Options)
                        {
                            if (o.Number == default)
                                o.Number = GetNextOptionNumber();
                        }
                    }
                    else if (q.Type == QuestionItemType.Ordering)
                    {
                        foreach (var o in q.Ordering.Options)
                        {
                            if (o.Number == default)
                                o.Number = GetNextOptionNumber();
                        }
                    }
                    else
                    {
                        foreach (var o in q.Options)
                        {
                            if (o.Number == default)
                                o.Number = GetNextOptionNumber();
                        }
                    }
                }
            }
        }

        private void OnFormAdded(Specification spec, Form form)
        {
            _formIndex.Add(form.Identifier, form);

            form.Specification = spec;
        }

        private void OnQuestionAdded(Set set, Question question)
        {
            _questionIndex.Add(question.Identifier, question);

            question.Set = set;
            question.CalculatePoints();

            OnQuestionListUpdated();
        }

        private void OnQuestionListUpdated()
        {
            var index = 0;

            foreach (var s in Sets)
            {
                foreach (var q in s.Questions)
                {
                    foreach (var v in q.EnumerateAllVersions())
                        v.BankIndex = index;

                    index++;
                }
            }
        }

        private void OnFormPublished(Form form, Change change)
        {
            var questions = form.GetQuestions();
            var addendumItems = form.Addendum.EnumerateAllItems().ToList();

            foreach (var question in questions)
            {
                question.PublicationStatus = PublicationStatus.Published;

                if (!question.FirstPublished.HasValue)
                    question.FirstPublished = change.ChangeTime;

                foreach (var identifier in question.AttachmentIdentifiers)
                {
                    var attachment = FindAttachment(identifier);

                    attachment.PublicationStatus = PublicationStatus.Published;

                    if (attachment.FirstPublished.HasValue)
                        continue;

                    var attachmentAsset = attachment.Asset;
                    var attachmentVersion = attachment.AssetVersion;
                    var addendumIndex = addendumItems.FindIndex(x => x.Asset == attachmentAsset && x.Version == attachmentVersion);

                    attachment.FirstPublished = change.ChangeTime;

                    if (addendumIndex == -1)
                        continue;

                    addendumItems[addendumIndex].Version = attachment.AssetVersion;
                    addendumItems.RemoveAt(addendumIndex);
                }
            }

            if (addendumItems.Count > 0)
            {
                var allAttachments = EnumerateAllAttachments().ToDictionary(x => (x.Asset, x.AssetVersion), x => x);
                foreach (var addendum in addendumItems)
                {
                    if (!allAttachments.TryGetValue((addendum.Asset, addendum.Version), out var attachment) || attachment.FirstPublished.HasValue)
                        continue;

                    attachment.FirstPublished = change.ChangeTime;
                    addendum.Version = attachment.AssetVersion;
                }
            }
        }

        public void SwapFields(Guid a, Guid b)
        {
            var x = FindField(a);
            var y = FindField(b);

            var i = x.Section.Fields.IndexOf(x);
            var j = y.Section.Fields.IndexOf(y);

            x.Section.Fields.RemoveAt(i);
            y.Section.Fields.RemoveAt(j);

            x.Section.Fields.Insert(i, y);
            y.Section.Fields.Insert(j, x);

            (y.Section, x.Section) = (x.Section, y.Section);
        }

        private void AddForm(Specification spec, Form form)
        {
            spec.Forms.Add(form);

            OnFormAdded(spec, form);
        }

        private void AddQuestion(Set set, Question question)
        {
            set.Questions.Add(question);

            OnQuestionAdded(set, question);
        }

        private void InsertQuestion(Set set, int index, Question question)
        {
            set.Questions.Insert(index, question);

            OnQuestionAdded(set, question);
        }

        private void AddSet(Set set)
        {
            _setIndex.Add(set.Identifier, set);

            Sets.Add(set);
            set.Bank = this;
        }

        private void AddSection(Section section)
        {
            _sectionIndex.Add(section.Identifier, section);

            section.Criterion.Sections.Add(section);
            section.Form.Sections.Add(section);
        }

        private void AddField(Field field)
        {
            field.Section.Fields.Add(field);
            field.Question.Fields.Add(field);
        }

        private void InsertField(int index, Field field)
        {
            field.Section.Fields.Insert(index, field);
            field.Question.Fields.Add(field);
        }

        #endregion

        #region Modification (apply events to change state)

        public void When(BankMemorized e) { }

        public void When(AssessmentQuestionOrderVerified change)
        {
            var form = FindForm(change.Form);
            form.StaticQuestionOrder = change.Questions;
            form.StaticQuestionOrderVerified = change.ChangeTime;
        }

        public void When(AssessmentHookChanged e)
        {
            var form = FindForm(e.Form);
            form.Hook = e.Hook;
        }

        public void When(SectionsAsTabsEnabled e)
        {
            var spec = FindSpecification(e.Specification);
            spec.SectionsAsTabsEnabled = true;
            spec.TabNavigationEnabled = true;
            spec.SingleQuestionPerTabEnabled = false;
        }

        public void When(SectionsAsTabsDisabled e)
        {
            var spec = FindSpecification(e.Specification);
            spec.SectionsAsTabsEnabled = false;
            spec.TabNavigationEnabled = true;
            spec.SingleQuestionPerTabEnabled = false;
        }

        public void When(TabNavigationEnabled e)
        {
            var spec = FindSpecification(e.Specification);
            spec.TabNavigationEnabled = true;
            spec.SingleQuestionPerTabEnabled = false;
        }

        public void When(TabNavigationDisabled e)
        {
            var spec = FindSpecification(e.Specification);
            spec.TabNavigationEnabled = false;
            spec.SingleQuestionPerTabEnabled = false;
        }

        public void When(SingleQuestionPerTabEnabled e)
        {
            var spec = FindSpecification(e.Specification);
            spec.SingleQuestionPerTabEnabled = true;
        }

        public void When(SingleQuestionPerTabDisabled e)
        {
            var spec = FindSpecification(e.Specification);
            spec.SingleQuestionPerTabEnabled = false;
        }

        public void When(AttachmentAdded e)
        {
            var attachment = new Attachment
            {
                Identifier = e.Attachment,
                Asset = e.Asset,
                Author = e.Author,
                Content = e.Content,
                Condition = e.Condition,
                Type = e.Type,
                Upload = e.Upload,
                Image = e.Image,
                Uploaded = e.ChangeTime
            };

            Attachments.Add(attachment);
        }

        public void When(AttachmentAddedToQuestion e)
        {
            var attachment = FindAttachment(e.Attachment);
            attachment.QuestionIdentifiers.Add(e.Question);

            var question = FindQuestion(e.Question);
            question.AttachmentIdentifiers.Add(e.Attachment);
        }

        public void When(AttachmentChanged e)
        {
            var attachment = FindAttachment(e.Attachment);

            attachment.Condition = e.Condition;
            attachment.Content.Title.Set(e.Content.Title);

            if (attachment.Type == AttachmentType.Image)
            {
                attachment.Image.IsColor = e.Image.IsColor;
                attachment.Image.Resolution = e.Image.Resolution;
                attachment.Image.TargetOnline = e.Image.TargetOnline?.Clone();
                attachment.Image.TargetPaper = e.Image.TargetPaper?.Clone();
            }
        }

        public void When(AttachmentImageChanged e)
        {
            var attachment = FindAttachment(e.Attachment);

            if (e.Upload != Guid.Empty && attachment.Upload != e.Upload)
            {
                attachment.Author = e.Author;
                attachment.Uploaded = e.ChangeTime;
            }

            attachment.Upload = e.Upload;
            attachment.Image.Actual = e.ActualDimension.Clone();
        }

        public void When(BankAttachmentDeleted e)
        {
            RemoveAttachment(e.Attachment);
        }

        public void When(AttachmentDeletedFromQuestion e)
        {
            var attachment = FindAttachment(e.Attachment);
            attachment.QuestionIdentifiers.Remove(e.Question);

            var question = FindQuestion(e.Question);
            question.AttachmentIdentifiers.Remove(e.Attachment);
        }

        public void When(AttachmentUpgraded e)
        {
            var current = FindAttachment(e.CurrentAttachment);

            var upgraded = current.Clone();

            upgraded.Identifier = e.UpgradedAttachment;
            upgraded.PublicationStatus = PublicationStatus.Drafted;
            upgraded.Condition = "Unassigned";
            upgraded.QuestionIdentifiers = new HashSet<Guid>();

            Attachments.SetNewVersion(current, upgraded);
        }

        public void When(BankAnalyzed _) { }



        public void When(BankDeleted _)
        {
            Status = "Archived";
        }

        public void When(BankContentChanged e)
        {
            Content = e.Content;
        }

        public void When(BankLocked _) => IsLocked = true;

        public void When(BankLevelChanged e)
        {
            Level = e.Level;
        }

        public void When(BankOpened e)
        {
            OnBankCreated(e, e.Bank);

            foreach (var spec in Specifications)
            {
                foreach (var form in spec.EnumerateAllForms())
                {
                    if (form.Publication.Status != PublicationStatus.Published || !form.Publication.IsPublished)
                        continue;

                    if (!form.Publication.FirstPublished.HasValue)
                        form.Publication.FirstPublished = e.ChangeTime;

                    OnFormPublished(form, e);
                }
            }
        }

        public void When(BankRenamed e)
        {
            Name = e.Name;
        }

        public void When(BankStandardChanged e)
        {
            var isChanged = e.Standard == Guid.Empty || Standard != e.Standard;

            Standard = e.Standard;

            if (isChanged)
            {
                foreach (var set in Sets)
                {
                    set.Standard = Guid.Empty;

                    foreach (var q in set.EnumerateAllQuestions())
                    {
                        q.Standard = Guid.Empty;
                        q.SubStandards = null;

                        foreach (var o in q.Options)
                            o.Standard = Guid.Empty;
                    }
                }
            }
        }

        public void When(BankTypeChanged e)
        {
            if (StringHelper.Equals(e.Type, "Basic"))
                Type = BankType.Basic;
            else
                Type = BankType.Advanced;
        }

        public void When(BankUnlocked _) => IsLocked = false;

        public void When(BankEditionChanged e)
        {
            Edition = new Edition(e.Major, e.Minor);
        }

        public void When(BankStatusChanged e)
        {
            IsActive = e.IsActive;
        }

        public void When(CommentAuthorRoleChanged e)
        {
            var comment = FindComment(e.Comment);

            comment.AuthorRole = e.AuthorRole;
        }

        public void When(CommentDuplicated e)
        {
            var sourceComment = FindComment(e.SourceComment);

            var destinationComment = sourceComment.Clone();
            destinationComment.Identifier = e.DestinationComment;
            destinationComment.Subject = e.DestinationSubject;
            destinationComment.Type = e.DestinationType;

            Comments.Add(destinationComment);
        }

        public void When(CommentMoved e)
        {
            var comment = FindComment(e.Comment);

            comment.Type = e.Type;
            comment.Subject = e.Subject;
        }

        public void When(BankCommentPosted e)
        {
            var comment = new Comment
            {
                Identifier = e.Comment,
                Flag = e.Flag,
                Category = e.Category,
                Type = e.Type,
                Subject = e.Subject,
                Author = e.Author,
                AuthorRole = e.AuthorRole,
                Text = e.Text,
                Posted = e.Posted ?? e.ChangeTime,
                Instructor = e.Instructor,
                EventDate = e.EventDate,
                EventFormat = e.EventFormat
            };

            Comments.Add(comment);
        }

        public void When(CommentRejected e)
        {
            RemoveComments(c => c.Identifier == e.Comment);
        }

        public void When(CommentRetracted e)
        {
            RemoveComments(c => c.Identifier == e.Comment);
        }

        public void When(BankCommentModified e)
        {
            var comment = FindComment(e.Comment);

            comment.Flag = e.Flag;
            comment.Category = e.Category;
            comment.Text = e.Text;
            comment.Revisor = e.Author;
            comment.Revised = e.Revised;
            comment.Instructor = e.Instructor;
            comment.EventDate = e.EventDate;
            comment.EventFormat = e.EventFormat;
        }

        public void When(CommentVisibilityChanged e)
        {
            var comment = FindComment(e.Comment);

            comment.IsHidden = e.IsHidden;
        }

        public void When(FieldAdded e)
        {
            // Do not add the field if it already exists.
            var field = FindField(e.Identifier);
            if (field != null)
                return;

            field = new Field
            {
                Identifier = e.Identifier,
                QuestionIdentifier = e.Question,
                Section = FindSection(e.Section),
                Question = FindQuestion(e.Question)
            };

            // Do not add the field if the form already contains the same question.
            if (field.Section.Form.ContainsQuestion(e.Question))
                return;

            if (e.Index >= 0 && e.Index < field.Section.Fields.Count)
                InsertField(e.Index, field);
            else
                AddField(field);
        }

        public void When(FieldDeleted e)
        {
            var question = FindQuestion(e.Question);
            var field = question.Fields.Where(x => x.Identifier == e.Field && x.Section.Form.Identifier == e.Form).FirstOrDefault();

            if (field != null)
                RemoveField(field);
        }

        public void When(FieldsDeleted e)
        {
            RemoveFields(e.Form, e.Question);
        }

        public void When(FieldsReordered e)
        {
            var section = FindSection(e.Section);

            // Determine the unique numbers for the fields to be resequenced ...

            var sequences = new Dictionary<Guid, int>();

            foreach (var key in e.Sequences.Keys)
                if (key <= section.Fields.Count)
                    sequences.Add(section.Fields[key - 1].QuestionIdentifier, e.Sequences[key]);

            foreach (var field in section.Fields)
                if (!sequences.ContainsKey(field.QuestionIdentifier))
                    sequences.Add(field.QuestionIdentifier, field.Sequence);

            // ... then sort the options.

            section.Fields.Sort((a, b) => sequences[a.QuestionIdentifier].CompareTo(sequences[b.QuestionIdentifier]));
        }

        public void When(FieldsSwapped e)
        {
            SwapFields(e.A, e.B);
        }

        public void When(FormAdded e)
        {
            var spec = FindSpecification(e.Specification);

            var form = new Form
            {
                Identifier = e.Identifier,
                Code = e.Code,
                Name = e.Name,
                Asset = e.Asset,
            };

            form.Content.Title.Default = e.Name;
            form.Invigilation.TimeLimit = e.TimeLimit ?? 0;

            AddForm(spec, form);
        }

        public void When(FormAddendumChanged e)
        {
            var form = FindForm(e.Form);

            SetAddendum(form.Addendum.Acronyms, e.Acronyms);
            SetAddendum(form.Addendum.Formulas, e.Formulas);
            SetAddendum(form.Addendum.Figures, e.Figures);

            if (e.RemoveObsolete)
                form.Addendum.Obsolete.Clear();
            else if (e.Addendum != null)
                SetAddendum(form.Addendum.Obsolete, e.Addendum.Select(x => new FormAddendumItem { Asset = x, Version = -1 }).ToArray());

            void SetAddendum(List<FormAddendumItem> stateValue, FormAddendumItem[] changeValue)
            {
                if (changeValue == null)
                    return;

                stateValue.Clear();

                if (changeValue.Length > 0)
                    stateValue.AddRange(changeValue);
            }
        }

        public void When(FormAnalyzed _) { }

        public void When(FormMessageConnected e)
        {
            var form = FindForm(e.Form);
            var bank = form.Specification.Bank;

            switch (e.MessageType)
            {
                case FormMessageType.WhenAttemptStartedNotifyAdmin:
                    form.WhenAttemptStartedNotifyAdminMessageIdentifier = e.MessageIdentifier;
                    break;

                case FormMessageType.WhenAttemptCompletedNotifyAdmin:
                    form.WhenAttemptCompletedNotifyAdminMessageIdentifier = e.MessageIdentifier;
                    break;
                default:
                    return;
            }
        }

        public void When(FormArchived e)
        {
            var form = FindForm(e.Form);
            var bank = form.Specification.Bank;

            form.Publication.Status = PublicationStatus.Archived;

            if (!e.Questions)
                return;

            foreach (var field in form.Sections.SelectMany(x => x.Fields))
            {
                field.Question.PublicationStatus = PublicationStatus.Archived;

                if (!e.Attachments)
                    continue;

                foreach (var identifier in field.Question.AttachmentIdentifiers)
                {
                    var attachment = bank.FindAttachment(identifier);
                    attachment.PublicationStatus = PublicationStatus.Archived;
                }
            }
        }

        public void When(FormAssetChanged e)
        {
            var form = FindForm(e.Form);
            form.Asset = e.Asset;
        }

        public void When(FormClassificationChanged e)
        {
            var form = FindForm(e.Form);
            form.Classification.Instrument = e.Instrument;
            form.Classification.Theme = e.Theme;
        }

        public void When(FormCodeChanged e)
        {
            var form = FindForm(e.Form);
            form.Code = e.Code;
            form.Source = e.Source;
            form.Origin = e.Origin;
        }

        public void When(FormContentChanged e)
        {
            var form = FindForm(e.Form);
            form.Content = e.Content;
            form.HasDiagrams = e.HasDiagrams;
            form.HasReferenceMaterials = e.HasReferenceMaterials;
        }

        public void When(FormGradebookChanged e)
        {
            var form = FindForm(e.Form);

            if (form.Gradebook == e.Gradebook)
                return;

            form.Gradebook = e.Gradebook;

            ResetQuestionGradeItems(form);
        }

        public void When(FormInvigilationChanged e)
        {
            var form = FindForm(e.Form);
            form.Invigilation = e.Invigilation.Clone();
        }

        public void When(FormLanguagesModified e)
        {
            var form = FindForm(e.Form);
            form.Languages = e.Languages.NullIfEmpty()?.Select(x => x.ToLower()).Distinct().OrderBy(x => x).ToArray();
        }

        public void When(FormNameChanged e)
        {
            var form = FindForm(e.Form);
            form.Name = e.Name;
        }

        public void When(FormPublished e)
        {
            var form = FindForm(e.Form);
            var publication = form.Publication;

            publication.Status = PublicationStatus.Published;
            publication.IsPublished = true;

            if (e.Publication != null)
            {
                publication.AllowFeedback = e.Publication.AllowFeedback;
                publication.AllowRationaleForCorrectAnswers = e.Publication.AllowRationaleForCorrectAnswers;
                publication.AllowDownloadAssessmentsQA = e.Publication.AllowDownloadAssessmentsQA;
                publication.AllowRationaleForIncorrectAnswers = e.Publication.AllowRationaleForIncorrectAnswers;
            }

            if (!form.Publication.FirstPublished.HasValue || e.Publication?.FirstPublished != null)
                form.Publication.FirstPublished = e.Publication?.FirstPublished ?? e.ChangeTime;

            OnFormPublished(form, e);
        }

        public void When(FormDeleted e)
        {
            RemoveForm(e.Form);
        }

        public void When(FormUnarchived e)
        {
            var form = FindForm(e.Form);
            var bank = form.Specification.Bank;

            form.Publication.Status = PublicationStatus.Drafted;

            if (e.Questions)
            {
                foreach (var section in form.Sections)
                {
                    foreach (var field in section.Fields)
                    {
                        field.Question.PublicationStatus = PublicationStatus.Drafted;

                        if (e.Attachments)
                        {
                            foreach (var identifier in field.Question.AttachmentIdentifiers)
                            {
                                var attachment = bank.FindAttachment(identifier);
                                attachment.PublicationStatus = PublicationStatus.Drafted;
                            }
                        }
                    }
                }
            }
        }

        public void When(FormUnpublished e)
        {
            var form = FindForm(e.Form);
            var publication = form.Publication;

            publication.Status = PublicationStatus.Unpublished;
            publication.IsPublished = false;
        }

        public void When(FormUpgraded e)
        {
            var srcForm = FindForm(e.Source);
            var spec = srcForm.Specification;

            var dstForm = srcForm.Clone();

            dstForm.Identifier = e.Destination;
            dstForm.Name = e.NewName;
            dstForm.Source = null;
            dstForm.Invigilation.Opened = null;
            dstForm.Invigilation.Closed = null;
            dstForm.Publication.Status = PublicationStatus.Drafted;
            dstForm.Publication.IsPublished = false;
            dstForm.Publication.IsProgram = false;
            dstForm.Publication.NavigateUrl = null;
            dstForm.Content.Title.Default += $" v.{dstForm.AssetVersion}";

            dstForm.Sections.Clear();

            spec.Forms.SetNewVersion(srcForm, dstForm);

            OnFormAdded(spec, dstForm);
        }

        public void When(FormVersionChanged _)
        {

        }

        public void When(OptionAdded e)
        {
            var question = FindQuestion(e.Question);

            var option = new Option
            {
                Number = GetNextOptionNumber(),
                Question = question,
                Content = e.Content,
                CutScore = e.CutScore,
                IsTrue = e.IsTrue,
                Points = e.Points,
                Standard = e.Standard ?? Guid.Empty
            };

            question.Options.Add(option);

            question.CalculatePoints();
        }

        public void When(OptionChanged e)
        {
            var option = FindOption(e.Question, e.Number);

            option.Points = e.Points;
            option.IsTrue = e.IsTrue;
            option.CutScore = e.CutScore;
            option.Standard = e.Standard ?? Guid.Empty;
            option.Content.Title.Set(e.Content?.Title);

            option.Question.CalculatePoints();
        }

        public void When(OptionDeleted e)
        {
            var option = FindOption(e.Question, e.Option);

            _optionIndex.Remove(option.Number);

            option.Question.Options.Remove(option);
            option.Question.CalculatePoints();
        }

        public void When(OptionsReordered e)
        {
            var question = FindQuestion(e.Question);

            // Determine the unique numbers for the options to be resequenced ...

            var sequences = new Dictionary<int, int>();

            foreach (var key in e.Sequences.Keys)
                if (key <= question.Options.Count)
                    sequences.Add(question.Options[key - 1].Number, e.Sequences[key]);

            foreach (var option in question.Options)
                if (!sequences.ContainsKey(option.Number))
                    sequences.Add(option.Number, option.Sequence);

            // ... then sort the options.

            question.Options.Sort((a, b) => sequences[a.Number].CompareTo(sequences[b.Number]));
        }

        public void When(QuestionAdded e)
        {
            var set = FindSet(e.Set);

            var question = new Question
            {
                Identifier = e.Question,
                Standard = e.Standard,
                SubStandards = null,
                Source = e.Source,

                Type = e.Type,
                CalculationMethod = e.Method,

                Condition = e.Condition,

                Asset = e.Asset,
                Points = e.Points,

                Content = e.Content
            };

            if (e.Type.IsHotspot())
                question.Hotspot.SetQuestionType(e.Type);

            AddQuestion(set, question);
        }

        public void When(QuestionClassificationChanged e)
        {
            var question = FindQuestion(e.Question);
            question.Classification.Copy(e.Classification);
        }

        public void When(QuestionComposedVoiceChanged e)
        {
            var question = FindQuestion(e.Question);
            e.ComposedVoice.CopyTo(question.ComposedVoice);
        }

        public void When(QuestionContentChanged e)
        {
            var question = FindQuestion(e.Question);

            question.Content.Title.Set(e.Content.Title);
            question.Content.Rationale.Set(e.Content.Rationale);
            question.Content.RationaleOnCorrectAnswer.Set(e.Content.RationaleOnCorrectAnswer);
            question.Content.RationaleOnIncorrectAnswer.Set(e.Content.RationaleOnIncorrectAnswer);
            question.Content.Description.Set(e.Content.Description);
            question.Content.Exemplar.Set(e.Content.Exemplar);
        }

        public void When(QuestionDuplicated2 e)
        {
            var source = FindQuestion(e.SourceQuestion);

            var destination = source.Clone();

            destination.Asset = e.DestinationAsset;
            destination.Identifier = e.DestinationQuestion;
            destination.PublicationStatus = PublicationStatus.Drafted;
            destination.FirstPublished = null;
            destination.Condition = "Unassigned";
            destination.Source = e.SourceQuestion;
            destination.Fields = new List<Field>();
            destination.GradeItems = new Dictionary<Guid, Guid>();
            destination.AttachmentIdentifiers = new HashSet<Guid>(source.AttachmentIdentifiers);

            foreach (var o in destination.Options)
            {
                o.Question = destination;
                o.Number = GetNextOptionNumber();
            }

            InsertQuestion(source.Set, source.Sequence, destination);
        }

        public void When(QuestionFlagChanged e)
        {
            var question = FindQuestion(e.Question);
            question.Flag = e.Flag;
        }

        public void When(QuestionGradeItemChanged2 e)
        {
            var form = FindForm(e.Form);
            var question = form.GetQuestions().Find(x => x.Identifier == e.Question);

            if (e.GradeItem.HasValue)
                question.GradeItems[form.Identifier] = e.GradeItem.Value;
            else
                question.GradeItems.Remove(form.Identifier);
        }

        public void When(QuestionLikertRowGradeItemChanged e)
        {
            var form = FindForm(e.Form);
            var question = form.GetQuestions().Find(x => x.Identifier == e.Question);
            var likertRow = question.Likert.GetRow(e.LikertRow);

            if (e.GradeItem.HasValue)
                likertRow.GradeItems[form.Identifier] = e.GradeItem.Value;
            else
                likertRow.GradeItems.Remove(form.Identifier);
        }

        public void When(QuestionLayoutChanged e)
        {
            var question = FindQuestion(e.Question);
            question.Layout.Copy(e.Layout);
        }

        public void When(QuestionHotspotImageChanged e)
        {
            var question = FindQuestion(e.QuestionIdentifier);
            question.Hotspot.Image.Set(e.Image);
        }

        public void When(QuestionHotspotOptionAdded e)
        {
            var question = FindQuestion(e.QuestionIdentifier);

            var option = question.Hotspot.AddOption(e.OptionIdentifier, e.Shape);
            option.Number = GetNextOptionNumber();
            option.Content = e.Content == null ? new ContentTitle() : e.Content.Clone();
            option.Points = e.Points;

            question.CalculatePoints();
        }

        public void When(QuestionHotspotOptionChanged e)
        {
            var question = FindQuestion(e.QuestionIdentifier);
            var option = question.Hotspot.GetOption(e.OptionIdentifier);

            e.Shape.CopyTo(option.Shape);
            option.Content.Title.Set(e.Content?.Title);
            option.Points = e.Points;

            question.CalculatePoints();
        }

        public void When(QuestionHotspotOptionDeleted e)
        {
            var question = FindQuestion(e.QuestionIdentifier);

            question.Hotspot.RemoveOption(e.OptionIdentifier);

            question.CalculatePoints();
        }

        public void When(QuestionHotspotOptionsReordered e)
        {
            var question = FindQuestion(e.QuestionIdentifier);

            question.Hotspot.ReorderOptions(e.Order);
        }

        public void When(QuestionHotspotPinLimitChanged e)
        {
            var question = FindQuestion(e.QuestionIdentifier);

            if (question.Type == QuestionItemType.HotspotCustom)
                question.Hotspot.PinLimit = e.PinLimit;

            question.CalculatePoints();
        }

        public void When(QuestionHotspotShowShapesChanged e)
        {
            var question = FindQuestion(e.QuestionIdentifier);

            if (question.Type == QuestionItemType.HotspotCustom)
                question.Hotspot.ShowShapes = e.ShowShapes;
        }

        public void When(QuestionLikertColumnAdded e)
        {
            var question = FindQuestion(e.QuestionIdentifier);
            var column = question.Likert.AddColumn(e.ColumnIdentifier);
            column.Content = e.Content == null ? new ContentTitle() : e.Content.Clone();
            question.CalculatePoints();

            foreach (var option in column.GetOptions())
                option.Number = GetNextOptionNumber();
        }

        public void When(QuestionLikertColumnChanged e)
        {
            var question = FindQuestion(e.QuestionIdentifier);
            var column = question.Likert.GetColumn(e.ColumnIdentifier);
            column.Content = e.Content == null ? new ContentTitle() : e.Content.Clone();
        }

        public void When(QuestionLikertColumnDeleted e)
        {
            var question = FindQuestion(e.QuestionIdentifier);
            question.Likert.RemoveColumn(e.ColumnIdentifier);
            question.CalculatePoints();
        }

        public void When(QuestionLikertOptionsChanged e)
        {
            var question = FindQuestion(e.QuestionIdentifier);

            foreach (var option in e.Options)
            {
                var o = question.Likert.GetOption(option);
                o.Points = option.Points;
            }

            question.CalculatePoints();
        }

        public void When(QuestionLikertReordered e)
        {
            var question = FindQuestion(e.QuestionIdentifier);

            if (e.RowsOrder.IsNotEmpty())
                question.Likert.ReorderRows(e.RowsOrder);

            if (e.ColumnsOrder.IsNotEmpty())
                question.Likert.ReorderColumns(e.ColumnsOrder);
        }

        public void When(QuestionLikertRowAdded e)
        {
            var question = FindQuestion(e.QuestionIdentifier);
            var row = question.Likert.AddRow(e.RowIdentifier);
            row.Standard = e.Standard ?? Guid.Empty;
            row.SubStandards = e.SubStandards.EmptyIfNull();
            row.Content = e.Content == null ? new ContentTitle() : e.Content.Clone();
            question.CalculatePoints();

            foreach (var option in row.GetOptions())
                option.Number = GetNextOptionNumber();
        }

        public void When(QuestionLikertRowChanged e)
        {
            var question = FindQuestion(e.QuestionIdentifier);
            var row = question.Likert.GetRow(e.RowIdentifier);
            row.Standard = e.Standard ?? Guid.Empty;
            row.SubStandards = e.SubStandards.EmptyIfNull();
            row.Content = e.Content == null ? new ContentTitle() : e.Content.Clone();
        }

        public void When(QuestionLikertRowDeleted e)
        {
            var question = FindQuestion(e.QuestionIdentifier);
            question.Likert.RemoveRow(e.RowIdentifier);
            question.CalculatePoints();
        }

        public void When(QuestionMatchesChanged e)
        {
            var question = FindQuestion(e.Question);
            question.Matches = e.Matches.Clone();
            question.CalculatePoints();
        }

        public void When(QuestionMoved e)
        {
            var set = FindSet(e.DestinationSet);
            var srcQuestion = FindQuestion(e.Question);
            var dstQuestion = srcQuestion.Clone();

            dstQuestion.Asset = e.Asset;
            dstQuestion.Standard = e.DestinationCompetency;
            dstQuestion.SubStandards = null;
            dstQuestion.PublicationStatus = PublicationStatus.Drafted;
            dstQuestion.Condition = "Unassigned";

            foreach (var o in dstQuestion.Options)
                o.Standard = Guid.Empty;

            RemoveQuestion(srcQuestion, false);

            set.Questions.Add(dstQuestion);

            OnQuestionAdded(set, dstQuestion);
        }

        public void When(QuestionMovedIn e)
        {
            var set = FindSet(e.DestinationSet);

            var question = e.Question.Clone(true);

            question.Asset = e.Asset;
            question.Standard = e.DestinationCompetency;
            question.SubStandards = null;
            question.PublicationStatus = PublicationStatus.Drafted;
            question.Condition = "Unassigned";

            foreach (var o in question.Options)
            {
                o.Question = question;
                o.Number = GetNextOptionNumber();
                o.Standard = Guid.Empty;
            }

            set.Questions.Add(question);

            OnQuestionAdded(set, question);

            if (e.Comments != null)
            {
                foreach (var comment in e.Comments)
                    Comments.Add(comment);
            }
        }

        public void When(QuestionMovedOut e)
        {
            RemoveQuestion(e.Question, true);

            OnQuestionListUpdated();
        }

        public void When(QuestionOrderingOptionAdded e)
        {
            var question = FindQuestion(e.Question);

            var option = question.Ordering.AddOption(e.Option);
            option.Number = GetNextOptionNumber();
            option.Content.Title.Set(e.Content?.Title);

            question.CalculatePoints();
        }

        public void When(QuestionOrderingSolutionAdded e)
        {
            var question = FindQuestion(e.Question);

            var solution = question.Ordering.AddSolution(e.Solution);
            solution.Points = e.Points;
            solution.CutScore = e.CutScore;

            question.CalculatePoints();
        }

        public void When(QuestionOrderingLabelChanged e)
        {
            var question = FindQuestion(e.Question);
            question.Ordering.Label.Show = e.Show;
            question.Ordering.Label.TopContent.Title.Set(e.TopContent?.Title);
            question.Ordering.Label.BottomContent.Title.Set(e.BottomContent?.Title);
        }

        public void When(QuestionOrderingOptionChanged e)
        {
            var question = FindQuestion(e.Question);
            var option = question.Ordering.GetOption(e.Option);

            option.Content.Title.Set(e.Content?.Title);

            question.CalculatePoints();
        }

        public void When(QuestionOrderingSolutionChanged e)
        {
            var question = FindQuestion(e.Question);
            var solution = question.Ordering.GetSolution(e.Solution);

            solution.Points = e.Points;
            solution.CutScore = e.CutScore;

            question.CalculatePoints();
        }

        public void When(QuestionOrderingOptionDeleted e)
        {
            var question = FindQuestion(e.Question);

            question.Ordering.RemoveOption(e.Option);

            question.CalculatePoints();
        }

        public void When(QuestionOrderingSolutionDeleted e)
        {
            var question = FindQuestion(e.Question);

            question.Ordering.RemoveSolution(e.Solution);

            question.CalculatePoints();
        }

        public void When(QuestionOrderingOptionsReordered e)
        {
            var question = FindQuestion(e.Question);

            question.Ordering.ReorderOptions(e.Order);
        }

        public void When(QuestionOrderingSolutionsReordered e)
        {
            var question = FindQuestion(e.Question);

            question.Ordering.ReorderSolutions(e.Order);
        }

        public void When(QuestionOrderingSolutionOptionsReordered e)
        {
            var question = FindQuestion(e.Question);
            var solution = question.Ordering.GetSolution(e.Solution);

            solution.ReorderOptions(e.Order);
        }

        public void When(QuestionPublicationStatusChanged e)
        {
            var question = FindQuestion(e.Question);

            question.PublicationStatus = e.Status;

            if (e.Status == PublicationStatus.Published && !question.FirstPublished.HasValue)
                question.FirstPublished = e.ChangeTime;
        }

        public void When(QuestionRandomizationChanged e)
        {
            var question = FindQuestion(e.Question);
            question.Randomization.Copy(e.Randomization);
        }

        public void When(QuestionRubricConnected e)
        {
            var question = FindQuestion(e.Question);
            question.Rubric = e.Rubric;
        }

        public void When(QuestionRubricDisconnected e)
        {
            var question = FindQuestion(e.Question);
            question.Rubric = null;
        }

        public void When(QuestionDeleted e)
        {
            var question = FindQuestion(e.Question);

            if (e.RemoveAllVersions)
            {
                foreach (var version in question.EnumerateAllVersions().ToArray())
                    RemoveQuestion(version, true);
            }
            else
            {
                RemoveQuestion(question, true);
            }

            OnQuestionListUpdated();
        }

        public void When(QuestionScoringChanged e)
        {
            var question = FindQuestion(e.Question);
            question.Points = e.Points;
            question.CutScore = e.CutScore;
            question.CalculationMethod = e.CalculationMethod;
            question.CalculatePoints();
        }

        public void When(QuestionSetChanged e)
        {
            var question = FindQuestion(e.Question).GetLastVersion();
            var oldSet = question.Set;
            var newSet = oldSet.Bank.FindSet(e.Set);

            oldSet.Questions.Remove(question);

            foreach (var version in question.EnumerateAllVersions())
            {
                version.Set = newSet;
                version.Standard = Guid.Empty;
                version.SubStandards = null;

                foreach (var o in version.Options)
                    o.Standard = Guid.Empty;
            }

            newSet.Questions.Add(question);

            OnQuestionListUpdated();
        }

        public void When(QuestionStandardChanged e)
        {
            var question = FindQuestion(e.Question);
            question.Standard = e.Standard;
            question.SubStandards = e.SubStandards;

            foreach (var o in question.Options)
                o.Standard = Guid.Empty;
        }

        public void When(QuestionUpgraded e)
        {
            var current = FindQuestion(e.CurrentQuestion);
            var set = current.Set;

            var upgraded = current.Clone();

            upgraded.Identifier = e.UpgradedQuestion;
            upgraded.PublicationStatus = PublicationStatus.Drafted;
            upgraded.FirstPublished = null;
            upgraded.Condition = "Unassigned";
            upgraded.Fields = new List<Field>();
            upgraded.GradeItems = new Dictionary<Guid, Guid>();
            upgraded.AttachmentIdentifiers = new HashSet<Guid>(current.AttachmentIdentifiers);

            foreach (var o in upgraded.Options)
            {
                o.Question = upgraded;
                o.Number = GetNextOptionNumber();
            }

            set.Questions.SetNewVersion(current, upgraded);

            OnQuestionAdded(set, upgraded);
        }

        public void When(QuestionConditionChanged e)
        {
            var question = FindQuestion(e.Question);

            question.Condition = e.Condition;
        }

        public void When(QuestionsReordered e)
        {
            var set = FindSet(e.Set);

            // Determine the asset numbers for the questions to be sorted...

            var sequences = new Dictionary<int, int>();

            foreach (var key in e.Sequences.Keys)
                if (key <= set.Questions.Count)
                    sequences.Add(set.Questions[key - 1].Asset, e.Sequences[key]);

            foreach (var question in set.Questions)
                if (!sequences.ContainsKey(question.Asset))
                    sequences.Add(question.Asset, question.Sequence);

            // ... then sort the questions.

            set.Questions.Sort((a, b) => sequences[a.Asset].CompareTo(sequences[b.Asset]));

            OnQuestionListUpdated();
        }

        public void When(SectionAdded e)
        {
            var section = new Section
            {
                Identifier = e.Section,
                CriterionIdentifier = e.Criterion,
                Criterion = FindCriterion(e.Criterion),
                Form = FindForm(e.Form),
            };

            AddSection(section);
        }

        public void When(SectionDeleted e)
        {
            RemoveSection(e.Section);
        }

        public void When(SectionReconfigured e)
        {
            var section = FindSection(e.Section);

            section.WarningOnNextTabEnabled = e.WarningOnNextTabEnabled;
            section.BreakTimerEnabled = e.BreakTimerEnabled;
            section.TimeLimit = e.TimeLimit;
            section.TimerType = e.TimerType;
        }

        public void When(SectionsReordered e)
        {
            var form = FindForm(e.Form);

            // Determine the unique numbers for the sections to be resequenced ...

            var sequences = new Dictionary<Guid, int>();

            foreach (var key in e.Sequences.Keys)
                if (key <= form.Sections.Count)
                    sequences.Add(form.Sections[key - 1].Identifier, e.Sequences[key]);

            foreach (var field in form.Sections)
                if (!sequences.ContainsKey(field.Identifier))
                    sequences.Add(field.Identifier, field.Sequence);

            // ... then sort the options.

            form.Sections.Sort((a, b) => sequences[a.Identifier].CompareTo(sequences[b.Identifier]));
        }

        public void When(SectionContentChanged e)
        {
            var section = FindSection(e.Section);

            section.Content = e.Content;
        }

        public void When(ThirdPartyAssessmentEnabled e)
        {
            var form = FindForm(e.Form);
            form.ThirdPartyAssessmentIsEnabled = true;
        }

        public void When(ThirdPartyAssessmentDisabled e)
        {
            var form = FindForm(e.Form);
            form.ThirdPartyAssessmentIsEnabled = false;
        }

        public void When(SetAdded e)
        {
            var set = new Set
            {
                Identifier = e.Set,
                Name = e.Name,
                Standard = e.Standard
            };

            AddSet(set);
        }

        public void When(SetImported e)
        {
            AddSet(e.Set);
        }

        public void When(SetRandomizationChanged e)
        {
            var s = FindSet(e.Set);
            s.Randomization.Copy(e.Randomization);
        }

        public void When(SetDeleted e)
        {
            RemoveSet(e.Set);

            OnQuestionListUpdated();
        }

        public void When(SetRenamed e)
        {
            var set = FindSet(e.Set);

            set.Name = e.Name;
        }

        public void When(SetsMerged e)
        {
            var removals = new List<Set>();
            var merge = FindSet(e.Set);

            foreach (var set in Sets)
            {
                if (set.Identifier == merge.Identifier)
                    continue;

                foreach (var question in set.Questions)
                {
                    set.Questions.Remove(question);

                    foreach (var vq in question.EnumerateAllVersions())
                    {
                        vq.Set = merge;
                        vq.Standard = Guid.Empty;
                        vq.SubStandards = null;

                        foreach (var o in vq.Options)
                            o.Standard = Guid.Empty;
                    }

                    merge.Questions.Add(question);
                }

                removals.Add(set);
            }

            foreach (var removal in removals)
                RemoveSet(removal);

            OnQuestionListUpdated();
        }

        public void When(SetsReordered e)
        {
            // Determine the unique numbers for the sets to be resequenced ...

            var sequences = new Dictionary<Guid, int>();

            foreach (var key in e.Sequences.Keys)
                if (key <= Sets.Count)
                    sequences.Add(Sets[key - 1].Identifier, e.Sequences[key]);

            foreach (var set in Sets)
                if (!sequences.ContainsKey(set.Identifier))
                    sequences.Add(set.Identifier, set.Sequence);

            // ... then sort the options.

            Sets.Sort((a, b) => sequences[a.Identifier].CompareTo(sequences[b.Identifier]));
        }

        public void When(SetStandardChanged e)
        {
            var set = FindSet(e.Set);

            set.Standard = e.Standard;

            foreach (var q in set.EnumerateAllQuestions())
            {
                q.Standard = Guid.Empty;
                q.SubStandards = null;

                foreach (var o in q.Options)
                    o.Standard = Guid.Empty;
            }
        }

        public void When(CriterionAdded e)
        {
            var criterion = new Criterion
            {
                Identifier = e.Identifier,
                Specification = FindSpecification(e.Specification),
                SetWeight = e.Weight,
                QuestionLimit = e.QuestionLimit,
                TagFilter = e.TagFilter,
                PivotFilter = e.PivotFilter,
                Name = e.Name
            };

            foreach (var id in e.Sets)
            {
                var set = FindSet(id);
                criterion.SetIdentifiers.Add(id);
                criterion.Sets.Add(set);
                set.Criteria.Add(criterion);
            }

            _criterionIndex.Add(criterion.Identifier, criterion);

            criterion.Specification.Criteria.Add(criterion);
            criterion.UpdateFilterType();
        }

        public void When(CriterionFilterChanged e)
        {
            var criterion = FindCriterion(e.Criterion);
            criterion.SetWeight = e.SetWeight;

            if (e.QuestionLimit.HasValue)
                criterion.QuestionLimit = e.QuestionLimit.Value;

            criterion.TagFilter = e.TagFilter;
            criterion.PivotFilter = e.PivotFilter;
            criterion.UpdateFilterType();
        }

        public void When(CriterionFilterDeleted e)
        {
            var criterion = FindCriterion(e.Criterion);
            criterion.TagFilter = null;
            criterion.PivotFilter = null;
            criterion.UpdateFilterType();
        }

        public void When(CriterionDeleted e)
        {
            RemoveCriterion(e);
        }

        public void When(SpecificationAdded e)
        {
            var spec = new Specification
            {
                Type = e.Type,
                Consequence = e.Consequence,
                Identifier = e.Specification,
                Name = e.Name,
                Asset = e.Asset,
                FormLimit = e.FormLimit,
                QuestionLimit = e.QuestionLimit,
                Calculation = e.Calculation,
            };

            spec.Content.Title.Default = e.Name;

            _specificationIndex.Add(spec.Identifier, spec);

            Specifications.Add(spec);
            spec.Bank = this;
        }

        public void When(SpecificationCalculationChanged e)
        {
            var spec = FindSpecification(e.Specification);
            spec.Calculation = e.Calculation;
        }

        public void When(SpecificationContentChanged e)
        {
            var spec = FindSpecification(e.Specification);
            spec.Content = e.Content;
        }

        public void When(SpecificationTabTimeLimitChanged e)
        {
            var spec = FindSpecification(e.Specification);
            spec.TabTimeLimit = e.TabTimeLimit;
        }

        public void When(SpecificationReconfigured e)
        {
            var spec = FindSpecification(e.Specification);

            if (e.Consequence.HasValue)
                spec.Consequence = e.Consequence.Value;

            spec.FormLimit = e.FormLimit;
            spec.QuestionLimit = e.QuestionLimit;
        }

        public void When(SpecificationDeleted e)
        {
            RemoveSpecification(e.Specification);
        }

        public void When(SpecificationRenamed e)
        {
            var spec = FindSpecification(e.Specification);
            spec.Name = e.Name;
        }

        public void When(SpecificationRetyped e)
        {
            var spec = FindSpecification(e.Specification);
            spec.Type = e.Type;

            if (spec.Type != SpecificationType.Static)
            {
                spec.SectionsAsTabsEnabled = false;
                spec.TabNavigationEnabled = true;
            }
        }

        public void When(SerializedChange _)
        {
            // Obsolete changes go here
        }

        #endregion

        #region Interrogation

        public Comment FindComment(Guid comment)
        {
            if (!_commentIndex.TryGetValue(comment, out var result))
            {
                result = Comments.SingleOrDefault(x => x.Identifier == comment);
                if (result != null)
                    _commentIndex.Add(comment, result);
            }

            return result;
        }

        public Attachment FindAttachment(Guid attachment)
        {
            if (!_attachmentIndex.TryGetValue(attachment, out var result))
            {
                result = EnumerateAllAttachments().SingleOrDefault(x => x.Identifier == attachment);
                if (result != null)
                    _attachmentIndex.Add(attachment, result);
            }

            return result;
        }

        public Field FindField(Guid id)
        {
            if (!_fieldIndex.TryGetValue(id, out var result))
            {
                result = Specifications
                    .SelectMany(x => x.EnumerateAllForms())
                    .SelectMany(x => x.Sections)
                    .SelectMany(x => x.Fields)
                    .FirstOrDefault(x => x.Identifier == id);

                if (result != null)
                    _fieldIndex.Add(id, result);
            }

            return result;
        }

        public Form FindForm(Guid id)
        {
            return _formIndex.GetOrDefault(id);
        }

        public Form FindForm(Guid specId, Guid formId)
        {
            var form = FindForm(formId);

            return form != null && form.Specification.Identifier == specId ? form : null;
        }

        public Option FindOption(Guid question, int number)
        {
            if (!_optionIndex.TryGetValue(number, out var result))
            {
                result = Sets
                    .SelectMany(x => x.EnumerateAllQuestions())
                    .SelectMany(x => x.Options)
                    .Where(x => x.Number == number)
                    .FirstOrDefault();

                if (result != null)
                {
                    foreach (var o in result.Question.Options)
                    {
                        if (_optionIndex.ContainsKey(o.Number))
                            _optionIndex.Add(o.Number, o);
                    }
                }
            }

            return result != null && result.Question.Identifier == question ? result : null;
        }

        /// <summary>
        /// Searches the sets in a bank for a specific question. 
        /// </summary>
        /// <returns>
        /// Returns null if the question is not found.
        /// </returns>
        public Question FindQuestion(Guid id)
        {
            return _questionIndex.TryGetValue(id, out var result) ? result : null;
        }

        public Question[] GetAllQuestions()
        {
            return _questionIndex.Values.ToArray();
        }

        public Section FindSection(Guid id)
        {
            return _sectionIndex.TryGetValue(id, out var result) ? result : null;
        }

        public Set FindSet(Guid id)
        {
            return _setIndex.TryGetValue(id, out var result) ? result : null;
        }

        public Criterion FindCriterion(Guid id)
        {
            return _criterionIndex.TryGetValue(id, out var result) ? result : null;
        }

        public Specification FindSpecification(Guid id)
        {
            return _specificationIndex.TryGetValue(id, out var result) ? result : null;
        }

        private int GetNextOptionNumber()
        {
            if (!_nextOptionNumber.HasValue)
            {
                var maxOptionNumber = 0;

                foreach (var question in Sets.SelectMany(x => x.EnumerateAllQuestions()))
                {
                    if (question.Type == QuestionItemType.Likert)
                    {
                        foreach (var option in question.Likert.Options)
                            if (maxOptionNumber < option.Number)
                                maxOptionNumber = option.Number;
                    }
                    else if (question.Type.IsHotspot())
                    {
                        foreach (var option in question.Hotspot.Options)
                            if (maxOptionNumber < option.Number)
                                maxOptionNumber = option.Number;
                    }
                    else if (question.Type == QuestionItemType.Ordering)
                    {
                        foreach (var option in question.Ordering.Options)
                            if (maxOptionNumber < option.Number)
                                maxOptionNumber = option.Number;
                    }
                    else
                    {
                        foreach (var option in question.Options)
                            if (maxOptionNumber < option.Number)
                                maxOptionNumber = option.Number;
                    }
                }

                _nextOptionNumber = maxOptionNumber + 1;
            }

            var result = _nextOptionNumber.Value;

            _nextOptionNumber = result + 1;

            return result;
        }

        #endregion

        #region Destruction

        public void RemoveAttachment(Guid attachment)
        {
            var a = FindAttachment(attachment);
            if (a != null)
                RemoveAttachment(a);
        }

        private void RemoveAttachment(Attachment attachment)
        {
            var isSingleVersion = attachment.IsSingleVersion();

            Attachments.RemoveVersion(attachment);

            _attachmentIndex.Remove(attachment.Identifier);

            var questions = Sets.SelectMany(x => x.EnumerateAllQuestions());
            foreach (var question in questions)
                question.AttachmentIdentifiers?.Remove(attachment.Identifier);

            if (!isSingleVersion)
            {
                foreach (var form in Specifications.SelectMany(x => x.EnumerateAllForms()))
                {
                    form.Addendum.Acronyms.RemoveAll(x => x.Asset == attachment.Asset && x.Version == attachment.AssetVersion);
                    form.Addendum.Formulas.RemoveAll(x => x.Asset == attachment.Asset && x.Version == attachment.AssetVersion);
                    form.Addendum.Figures.RemoveAll(x => x.Asset == attachment.Asset && x.Version == attachment.AssetVersion);
                }
            }
            else
            {
                foreach (var form in Specifications.SelectMany(x => x.EnumerateAllForms()))
                    form.Addendum.Obsolete.RemoveAll(x => x.Asset == attachment.Asset);
            }
        }

        public void RemoveFields(Guid form, Guid question)
        {
            var f = FindForm(form);

            foreach (var section in f.Sections.ToArray())
            {
                var field = section.Fields.FirstOrDefault(x => x.QuestionIdentifier == question);
                if (field != null)
                    RemoveField(field);
            }
        }

        public void RemoveField(Field field)
        {
            _fieldIndex.Remove(field.Identifier);

            field.Question.GradeItems.Remove(field.Section.Form.Identifier);
            field.Question.Fields.Remove(field);
            field.Section.Fields.Remove(field);

            foreach (var comment in Comments)
            {
                if (comment.Type == CommentType.Field && comment.Subject == field.Identifier)
                {
                    comment.Type = CommentType.Question;
                    comment.Subject = field.QuestionIdentifier;
                }
            }
        }

        public void RemoveForm(Guid form)
        {
            var entity = FindForm(form);
            if (entity != null)
                RemoveForm(entity);
        }

        private void RemoveForm(Form form)
        {
            foreach (var section in form.Sections.ToArray())
                RemoveSection(section);

            if (form.Specification.Type != SpecificationType.Static)
            {
                // No need to do this for static because RemoveSection reset all grade items
                ResetQuestionGradeItems(form);
            }

            _formIndex.Remove(form.Identifier);
            form.Specification.Forms.RemoveVersion(form);

            RemoveComments(x => x.Type == CommentType.Form && x.Subject == form.Identifier);
        }

        private void ResetQuestionGradeItems(Form form)
        {
            var questions = form.GetQuestions();
            foreach (var question in questions)
                question.GradeItems.Remove(form.Identifier);
        }

        private void RemoveQuestion(Guid question, bool removeComments)
        {
            var q = FindQuestion(question);

            RemoveQuestion(q, removeComments);
        }

        private void RemoveQuestion(Question question, bool removeComments)
        {
            var fields = question.Fields.ToArray();
            foreach (var field in fields)
                RemoveField(field);

            var options = question.Options.ToArray();
            foreach (var option in options)
                _optionIndex.Remove(option.Number);

            question.Set.Questions.RemoveVersion(question);

            _questionIndex.Remove(question.Identifier);

            if (removeComments)
                RemoveComments(x => x.Type == CommentType.Question && x.Subject == question.Identifier);
        }

        public void RemoveSection(Guid id)
        {
            var section = FindSection(id);
            if (section != null)
                RemoveSection(section);
        }

        public void RemoveSection(Section section)
        {
            if (section?.Criterion?.Sections == null)
                return;

            _sectionIndex.Remove(section.Identifier);

            section.Criterion.Sections.Remove(section);

            foreach (var field in section.Fields.ToArray())
                RemoveField(field);

            section.Form.Sections.Remove(section);
        }

        public void RemoveSet(Guid set)
        {
            var s = Sets.FirstOrDefault(x => x.Identifier == set);
            if (s == null)
                return;

            RemoveSet(s);

            OnQuestionListUpdated();
        }

        private void RemoveSet(Set set)
        {
            _setIndex.Remove(set.Identifier);

            foreach (var q in set.EnumerateAllQuestions().ToArray())
                RemoveQuestion(q, true);

            Sets.Remove(set);

            foreach (var spec in Specifications)
            {
                var sections = spec.EnumerateAllForms().SelectMany(x => x.Sections.Where(y => y.Criterion.SetIdentifiers.Contains(set.Identifier))).ToArray();
                foreach (var section in sections)
                    RemoveSection(section);

                var criteria = spec.Criteria.Where(x => x.SetIdentifiers.Contains(set.Identifier)).ToArray();
                foreach (var criterion in criteria)
                    RemoveCriterion(criterion);
            }

            RemoveComments(x => x.Type == CommentType.Set && x.Subject == set.Identifier);
        }

        public Criterion RemoveCriterion(CriterionDeleted e)
        {
            var s = FindCriterion(e.Criterion);

            if (s != null)
                RemoveCriterion(s);

            return s;
        }

        private void RemoveCriterion(Criterion criterion)
        {
            var formQuestions = criterion.Specification.Type == SpecificationType.Dynamic
                ? criterion.Specification
                    .EnumerateAllForms()
                    .Select(x => (x, x.GetQuestions()))
                    .ToList()
                : null;

            _criterionIndex.Remove(criterion.Identifier);

            var sections = criterion.Specification.EnumerateAllForms().SelectMany(x => x.Sections).Where(x => x.CriterionIdentifier == criterion.Identifier).ToArray();
            foreach (var section in sections)
                RemoveSection(section);

            criterion.Specification.Criteria.Remove(criterion);

            foreach (var set in criterion.Sets)
                set.Criteria.RemoveAll(x => x.Identifier == criterion.Identifier);

            RemoveComments(x => x.Type == CommentType.Criterion && x.Subject == criterion.Identifier);

            if (formQuestions != null)
                RemoveCriterionGradeItems(formQuestions);
        }

        private void RemoveCriterionGradeItems(IEnumerable<(Form, List<Question>)> formQuestions)
        {
            foreach (var (form, prevQuestions) in formQuestions)
            {
                var currentQuestions = form.GetQuestions();
                var deletedQuestions = prevQuestions
                    .Where(q1 => !currentQuestions.Any(q2 => q2.Identifier == q1.Identifier))
                    .ToList();

                foreach (var q in deletedQuestions)
                {
                    q.GradeItems.Remove(form.Identifier);
                    if (q.Likert == null)
                        continue;

                    foreach (var row in q.Likert.Rows)
                        row.GradeItems.Remove(form.Identifier);
                }
            }
        }

        public void RemoveSpecification(Guid specification)
        {
            var s = Specifications.SingleOrDefault(x => x.Identifier == specification);
            if (s == null)
                return;

            RemoveSpecification(s);
        }

        private void RemoveSpecification(Specification spec)
        {
            _specificationIndex.Remove(spec.Identifier);

            foreach (var criterion in spec.Criteria.ToArray())
                RemoveCriterion(criterion);

            foreach (var form in spec.EnumerateAllForms().ToArray())
                RemoveForm(form);

            Specifications.Remove(spec);
        }

        private void RemoveComments(Func<Comment, bool> match)
        {
            Comments.RemoveAll(x =>
            {
                var isMatch = match(x);

                if (isMatch)
                    _commentIndex.Remove(x.Identifier);

                return isMatch;
            });
        }

        #endregion

        #region Serialization

        public string Serialize()
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            settings.Converters.Add(new StringEnumConverter());

            return JsonConvert.SerializeObject(this, settings);
        }

        public bool ShouldSerializeAttachments()
        {
            return Attachments.Count > 0;
        }

        public bool ShouldSerializeComments()
        {
            return Comments.Count > 0;
        }

        public bool ShouldSerializeEdition()
        {
            return Edition.Major.HasValue() || Edition.Minor.HasValue();
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            RestoreReferences();
        }

        internal void RestoreReferences()
        {
            var criterionMapping = new Dictionary<Guid, List<Criterion>>();
            var fieldMapping = new Dictionary<Guid, List<Field>>();

            foreach (var spec in Specifications)
            {
                spec.Bank = this;

                if (!_specificationIndex.ContainsKey(spec.Identifier))
                    _specificationIndex.Add(spec.Identifier, spec);

                foreach (var criterion in spec.Criteria)
                {
                    if (!_criterionIndex.ContainsKey(criterion.Identifier))
                        _criterionIndex.Add(criterion.Identifier, criterion);

                    criterion.Sets.Clear();

                    foreach (var setId in criterion.SetIdentifiers)
                    {
                        if (!criterionMapping.ContainsKey(setId))
                            criterionMapping.Add(setId, new List<Criterion>());

                        criterionMapping[setId].Add(criterion);
                    }
                }

                foreach (var form in spec.EnumerateAllForms())
                {
                    if (!_formIndex.ContainsKey(form.Identifier))
                        _formIndex.Add(form.Identifier, form);

                    foreach (var section in form.Sections)
                    {
                        if (!_sectionIndex.ContainsKey(section.Identifier))
                            _sectionIndex.Add(section.Identifier, section);

                        foreach (var field in section.Fields)
                        {
                            if (!fieldMapping.ContainsKey(field.QuestionIdentifier))
                                fieldMapping.Add(field.QuestionIdentifier, new List<Field>());

                            fieldMapping[field.QuestionIdentifier].Add(field);
                        }
                    }
                }
            }

            // Rehydrate the references between sets and criteria, and also between questions and fields.

            var qIndex = 0;
            var attachmentMapping = new Dictionary<Guid, List<Question>>();

            foreach (var set in Sets)
            {
                set.Bank = this;
                set.Criteria.Clear();

                if (!_setIndex.ContainsKey(set.Identifier))
                    _setIndex.Add(set.Identifier, set);

                foreach (var qRoot in set.Questions)
                {
                    foreach (var question in qRoot.EnumerateAllVersions())
                    {
                        question.BankIndex = qIndex;

                        if (!_questionIndex.ContainsKey(question.Identifier))
                            _questionIndex.Add(question.Identifier, question);

                        foreach (var attachmentId in question.AttachmentIdentifiers)
                        {
                            if (!attachmentMapping.ContainsKey(attachmentId))
                                attachmentMapping.Add(attachmentId, new List<Question>());

                            attachmentMapping[attachmentId].Add(question);
                        }

                        if (fieldMapping.ContainsKey(question.Identifier))
                        {
                            foreach (var field in fieldMapping[question.Identifier])
                            {
                                if (field.Question == null)
                                {
                                    field.Question = question;
                                    question.Fields.Add(field);
                                }
                            }
                        }
                    }

                    qIndex++;
                }

                if (criterionMapping.ContainsKey(set.Identifier))
                {
                    foreach (var criterion in criterionMapping[set.Identifier])
                    {
                        criterion.Sets.Add(set);
                        set.Criteria.Add(criterion);
                    }
                }
            }

            foreach (var attachment in EnumerateAllAttachments())
            {
                attachment.QuestionIdentifiers.Clear();

                if (attachmentMapping.TryGetValue(attachment.Identifier, out var attachmentQuestions))
                {
                    foreach (var question in attachmentQuestions)
                        attachment.QuestionIdentifiers.Add(question.Identifier);

                    attachmentMapping.Remove(attachment.Identifier);
                }
            }

            foreach (var brokenMapping in attachmentMapping)
            {
                foreach (var question in brokenMapping.Value)
                    question.AttachmentIdentifiers.Remove(brokenMapping.Key);
            }

            // if (Version != null && (Edition == null || !Edition.Major.HasValue() && !Edition.Minor.HasValue()))
            //    Edition = Version;
            // else
            if (Edition == null)
                Edition = new Edition();
        }

        public override string ToString()
        {
            if (Content.Title != null)
                return Content.Title.Default;

            return Name.IfNullOrEmpty("(Untitled)");
        }

        #endregion

        #region Enumeration

        public IEnumerable<Attachment> EnumerateAllAttachments() =>
            Attachments.SelectMany(x => x.EnumerateAllVersions());

        #endregion
    }
}