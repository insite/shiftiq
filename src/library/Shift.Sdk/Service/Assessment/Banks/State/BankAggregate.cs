using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// The term "assessment" is understood to encapsulate the administrative side of a learning interaction. For 
    /// example, in the case of a competency evaluation it is the *definition* of an exam form. The term "examination" 
    /// is understood to encapsulate the user side of a learning interaction. In the example of a competency evaluation 
    /// it is the *instantiation* of an exam form for a specific candidate. Put in simplest terms, an Assessment is the
    /// definition of exam-related assets, and an Examination is the instantiation of exam-related assets.
    /// </summary>
    public class BankAggregate : AggregateRoot
    {
        #region Properties (state)

        public override AggregateState CreateState() => new BankState();
        public BankState Data => (BankState)State;

        #endregion

        #region Methods (commands)

        /// <summary>
        /// Overwrites the current aggregate state with the input data.
        /// </summary>
        public void MemorizeBank(BankState data)
        {
            Apply(new BankMemorized(data));
        }

        /// <summary>
        /// If the aggregate state was memorized at a specific time in history then this becomes its
        /// its current state automatically.
        /// </summary>
        override protected void ApplyChange(IChange change)
        {
            if (change is BankMemorized memorized)
                State = memorized.Data;
            else
                base.ApplyChange(change);
        }

        public void AddAttachment(Attachment attachment)
        {
            // The caller must ensure a valid asset number is assigned.
            if (attachment.Asset == 0)
                throw new MissingAssetNumberException();

            if (attachment.Identifier == Guid.Empty || attachment.Upload == Guid.Empty || Data.FindAttachment(attachment.Identifier) != null)
                return;

            if (string.IsNullOrEmpty(attachment.Content.Title?.Default))
                return;

            var e = new AttachmentAdded(attachment.Identifier, attachment.Asset,
                attachment.Author, attachment.Content.Clone(), attachment.Condition,
                attachment.Type, attachment.Upload, attachment.Image?.Clone());

            Apply(e);
        }

        public void AddAttachmentToQuestion(Guid attachmentId, Guid questionId)
        {
            if (attachmentId == Guid.Empty || questionId == Guid.Empty)
                return;

            var attachment = Data.FindAttachment(attachmentId);
            var question = Data.FindQuestion(questionId);

            if (attachment == null || question == null || attachment.QuestionIdentifiers.Contains(questionId) && question.AttachmentIdentifiers.Contains(attachmentId))
                return;

            var e = new AttachmentAddedToQuestion(attachmentId, questionId);

            Apply(e);
        }

        public void AddField(Guid identifier, Guid section, Guid question, int index)
        {
            if (identifier == Guid.Empty || Data.FindField(identifier) != null)
                return;

            var s = Data.FindSection(section);
            if (s == null || s.Form.Sections.Any(x => x.Fields.Any(y => y.QuestionIdentifier == question)) || !s.Criterion.Sets.Any(x => x.EnumerateAllQuestions().Any(y => y.Identifier == question)))
                return;

            if (index != -1 && (index < 0 || index >= s.Fields.Count))
                index = -1;

            // If the form already contains the same question then throw an exception. Code that invokes this method 
            // should never allow this scenario to occur (or it should catch the exception).

            if (s.Form.ContainsQuestion(question))
                throw new DuplicateQuestionException($"This form ({s.Form.Identifier}) contains another field that references this question {question}. Questions cannot be duplicated on the same form.");

            Apply(new FieldAdded(identifier, section, question, index));
        }

        public void AddForm(Guid spec, Guid identifier, string name, int asset, int? timeLimit)
        {
            if (identifier == Guid.Empty || asset <= 0)
                return;

            var e = new FormAdded(spec, identifier, name, asset, timeLimit);

            Apply(e);
        }

        public void AddOption(Guid question, ContentTitle content, decimal points, bool? isTrue, decimal? cutScore, Guid? standard)
        {
            var q = Data.FindQuestion(question);
            if (q == null)
                return;

            var e = new OptionAdded(question, content, points, isTrue, cutScore, standard);

            Apply(e);
        }

        public void AddQuestionOrderingOption(Guid question, Guid option, ContentTitle content)
        {
            if (option == Guid.Empty)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Ordering || q.Ordering.GetOption(option) != null)
                return;

            var e = new QuestionOrderingOptionAdded(question, option, content);

            Apply(e);
        }

        public void AddQuestionOrderingSolution(Guid question, Guid solution, decimal points, decimal? cutScore)
        {
            if (solution == Guid.Empty)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Ordering || q.Ordering.GetSolution(solution) != null)
                return;

            var e = new QuestionOrderingSolutionAdded(question, solution, points, cutScore);

            Apply(e);
        }

        public void AddQuestion(Guid set, Question question)
        {
            // The caller must ensure a valid asset number is assigned.
            if (question.Asset == 0)
                throw new MissingAssetNumberException();

            if (question.Identifier == Guid.Empty || Data.FindSet(set) == null || Data.FindQuestion(question.Identifier) != null)
                return;

            Apply(new QuestionAdded(
                set,
                question.Identifier,
                question.Standard,
                question.Source,
                question.Type,
                question.CalculationMethod,
                question.Condition,
                question.Asset,
                question.Points,
                question.Content));

            if (question.Rubric.HasValue)
                Apply(new QuestionRubricConnected(question.Identifier, question.Rubric.Value));

            if (question.Randomization.Enabled)
                Apply(new QuestionRandomizationChanged(question.Identifier, question.Randomization));

            if (question.Layout.Type == OptionLayoutType.Table && question.Layout.Columns.IsNotEmpty())
                Apply(new QuestionLayoutChanged(question.Identifier, question.Layout));

            if (!question.Classification.IsEmpty)
                Apply(new QuestionClassificationChanged(question.Identifier, question.Classification));

            if (!question.ComposedVoice.IsEmpty)
                Apply(new QuestionComposedVoiceChanged(question.Identifier, question.ComposedVoice));

            if (question.CutScore.HasValue)
                Apply(new QuestionScoringChanged(question.Identifier, question.Points, question.CutScore, question.CalculationMethod));

            if (question.Flag != default)
                Apply(new QuestionFlagChanged(question.Identifier, question.Flag));

            if (question.Type == QuestionItemType.Matching)
            {
                if (!question.Matches.IsEmpty)
                    Apply(new QuestionMatchesChanged(question.Identifier, question.Matches));
            }
            else if (question.Type == QuestionItemType.Likert)
            {
                if (!question.Likert.IsEmpty)
                {
                    foreach (var row in question.Likert.Rows)
                        Apply(new QuestionLikertRowAdded(question.Identifier, row.Identifier, row.Standard, row.SubStandards, row.Content));

                    foreach (var column in question.Likert.Columns)
                        Apply(new QuestionLikertColumnAdded(question.Identifier, column.Identifier, column.Content));

                    if (question.Likert.HasOptions)
                        Apply(new QuestionLikertOptionsChanged(question.Identifier, question.Likert.Options));
                }
            }
            else if (question.Type.IsHotspot())
            {
                var hotspot = question.Hotspot;

                if (hotspot.PinLimit > Hotspot.MinPinLimit)
                    Apply(new QuestionHotspotPinLimitChanged(question.Identifier, hotspot.PinLimit));

                if (hotspot.ShowShapes)
                    Apply(new QuestionHotspotShowShapesChanged(question.Identifier, hotspot.ShowShapes));

                if (hotspot.Image.Url.IsNotEmpty())
                {
                    Apply(new QuestionHotspotImageChanged(question.Identifier, hotspot.Image.Clone()));

                    foreach (var o in hotspot.Options)
                        Apply(new QuestionHotspotOptionAdded(question.Identifier, o.Identifier, o.Shape, o.Content, o.Points));
                }
            }
            else if (question.Type == QuestionItemType.Ordering)
            {
                var ordering = question.Ordering;

                var label = question.Ordering.Label;
                if (!label.IsEmpty)
                    Apply(new QuestionOrderingLabelChanged(question.Identifier, label.Show, label.TopContent, label.BottomContent));

                foreach (var option in ordering.Options)
                    Apply(new QuestionOrderingOptionAdded(question.Identifier, option.Identifier, option.Content));

                foreach (var solution in ordering.Solutions)
                {
                    Apply(new QuestionOrderingSolutionAdded(question.Identifier, solution.Identifier, solution.Points, solution.CutScore));
                    Apply(new QuestionOrderingSolutionOptionsReordered(
                        question.Identifier, solution.Identifier,
                        solution.Options.Select((id, index) => (id, index)).ToDictionary(x => x.id, x => x.index)));
                }
            }
            else if (question.Options != null)
            {
                foreach (var option in question.Options)
                    Apply(new OptionAdded(question.Identifier, option.Content, option.Points, option.IsTrue, option.CutScore, option.Standard));
            }
        }

        public void AddQuestion2(Guid set,
                Guid question,
                QuestionItemType Type,
                string Condition,
                int Asset,
                Guid Standard,
                Guid? Source,
                decimal? Points,
                QuestionCalculationMethod CalculationMethod,
                ContentExamQuestion Content
            )
        {
            // The caller must ensure a valid asset number is assigned.
            if (Asset == 0)
                throw new MissingAssetNumberException();

            if (question == Guid.Empty || Data.FindSet(set) == null || Data.FindQuestion(question) != null)
                return;

            Apply(new QuestionAdded(
                set,
                question,
                Standard,
                Source,
                Type,
                CalculationMethod,
                Condition,
                Asset,
                Points,
                Content));
        }

        public void AddSection(Guid form, Guid identifier, Guid criteria)
        {
            if (identifier == Guid.Empty || Data.FindSection(identifier) != null)
                return;

            var f = Data.FindForm(form);
            if (f == null)
                return;

            var c = f.Specification.Criteria.SingleOrDefault(x => x.Identifier == criteria);
            if (c == null)
                return;

            var e = new SectionAdded(form, identifier, criteria);

            Apply(e);
        }

        public void AddSet(Guid set, string name, Guid standard)
        {
            if (string.IsNullOrEmpty(name) || set == Guid.Empty || Data.FindSet(set) != null)
                return;

            var e = new SetAdded(set, name, standard);

            Apply(e);
        }

        public void AddCriterion(Guid specificationId, Guid[] setIds, Guid sieveId, string name, decimal weight, int questionLimit, string basicFilter, PivotTable advancedFilter)
        {
            var spec = Data.FindSpecification(specificationId);
            if (spec == null || sieveId == Guid.Empty || Data.FindCriterion(sieveId) != null)
                return;

            var sets = new HashSet<Guid>(Data.Sets.Select(x => x.Identifier));
            var specSets = new HashSet<Guid>(spec.Criteria.SelectMany(x => x.SetIdentifiers).Distinct());

            setIds = setIds.Where(x => sets.Contains(x)).ToArray();
            if (setIds.Length == 0)
                return;

            weight = Number.CheckRange(weight, 0, 1);
            questionLimit = Number.CheckRange(questionLimit, 0);

            var e = new CriterionAdded(specificationId, setIds, sieveId, name, weight, questionLimit, basicFilter, advancedFilter);

            Apply(e);
        }

        public void AddSpecification(SpecificationType type, ConsequenceType consequence, Guid identifier, string name, int asset, int formLimit, int questionLimit, ScoreCalculation calculation)
        {
            if (asset <= 0 || string.IsNullOrEmpty(name) || identifier == Guid.Empty || Data.FindSpecification(identifier) != null)
                return;

            formLimit = Number.CheckRange(formLimit, 0);
            questionLimit = Number.CheckRange(questionLimit, 0);
            calculation.PassingScore = Number.CheckRange(calculation.PassingScore, 0, 1);
            calculation.FailureWeight = Number.CheckRange(calculation.FailureWeight, 0, 1);
            calculation.SuccessWeight = Number.CheckRange(calculation.SuccessWeight, 0, 1);

            var e = new SpecificationAdded(type, consequence, identifier, name, asset, formLimit, questionLimit, calculation);

            Apply(e);
        }

        public void AddQuestionHotspotOption(Guid question, Guid option, HotspotShape shape, ContentTitle content, decimal points)
        {
            var q = Data.FindQuestion(question);
            if (q == null || !q.Type.IsHotspot())
                return;

            if (Data.GetAllQuestions().Any(x => x.Type.IsHotspot() && x.Hotspot.GetOption(option) != null))
                return;

            var e = new QuestionHotspotOptionAdded(question, option, shape, content, points);

            Apply(e);
        }

        public void AddQuestionLikertColumn(Guid question, Guid column, ContentTitle content)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Likert)
                return;

            if (Data.GetAllQuestions().Any(x => x.Type == QuestionItemType.Likert && x.Likert.GetColumn(column) != null))
                return;

            var e = new QuestionLikertColumnAdded(question, column, content);

            Apply(e);
        }

        public void AddQuestionLikertRow(Guid question, Guid row, Guid standard, Guid[] subStandards, ContentTitle content)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Likert)
                return;

            if (Data.GetAllQuestions().Any(x => x.Identifier == row || x.Type == QuestionItemType.Likert && x.Likert.GetRow(row) != null))
                return;

            var e = new QuestionLikertRowAdded(question, row, standard, subStandards, content);

            Apply(e);
        }

        public void AnalyzeBank()
        {
            Apply(new BankAnalyzed());
        }

        public void AnalyzeForm(Guid identifier)
        {
            Apply(new FormAnalyzed(identifier));
        }

        public void DeleteBank()
        {
            Apply(new BankDeleted());
        }

        public void ConnectFormMessage(Guid form, FormMessageType type, Guid? messageIdentifier)
        {
            // Ignore the command if there is no matching form.
            if (Data.FindForm(form) == null)
                return;

            Apply(new FormMessageConnected(form, type, messageIdentifier));
        }

        public void ConnectQuestionRubric(Guid question, Guid rubric)
        {
            var model = Data.FindQuestion(question);
            if (model == null || model.Rubric == rubric)
                return;

            Apply(new QuestionRubricConnected(question, rubric));
        }

        public void ArchiveForm(Guid form, bool questions, bool attachments)
        {
            // Ignore the command if there is no matching form.
            if (Data.FindForm(form) == null)
                return;

            var e = new FormArchived(form, questions, attachments);

            Apply(e);
        }

        public void ChangeAssessmentHook(Guid form, string hook)
        {
            var f = Data.FindForm(form);
            if (f == null || hook == f.Hook)
                return;

            var e = new AssessmentHookChanged(form, hook);

            Apply(e);
        }

        public void ChangeAttachment(Guid attachment, string status, ContentTitle content, AttachmentImage image)
        {
            var a = Data.FindAttachment(attachment);
            if (a == null)
                return;

            if (content == null || string.IsNullOrEmpty(content.Title?.Default) || a.Type == AttachmentType.Image && image == null)
                return;

            if (a.Content.IsEqual(content) && (a.Type != AttachmentType.Image || a.Image.Equals(image)) && (string.IsNullOrEmpty(status) && string.IsNullOrEmpty(a.Condition) || status == a.Condition))
                return;

            var e = new AttachmentChanged(attachment, status, content, image?.Clone());

            Apply(e);
        }

        public void ChangeAttachmentImage(Guid attachment, Guid upload, Guid author, ImageDimension actualDimension)
        {
            var a = Data.FindAttachment(attachment);
            if (a == null || a.Type != AttachmentType.Image || upload == Guid.Empty || actualDimension == null || !actualDimension.HasValue)
                return;

            var e = new AttachmentImageChanged(attachment, upload, author, actualDimension.Clone());

            Apply(e);
        }

        public void ChangeBankContent(ContentExamBank content)
        {
            // Ignore the command if there is no content.
            if (content == null)
                return;

            // Ignore the command if the content has not actually changed.
            if (Data.Content.IsEqual(content))
                return;

            var e = new BankContentChanged(content);

            Apply(e);
        }

        public void ChangeBankLevel(Level level)
        {
            // Ignore the command if the level is not actually changed.
            if (StringHelper.Equals(Data.Level.ToString(), level.ToString()))
                return;

            Apply(new BankLevelChanged(level));
        }

        public void ChangeBankStandard(Guid standard)
        {
            // Ignore the command if the level is not actually changed.
            if (Data.Standard == standard)
                return;

            var e = new BankStandardChanged(standard);

            Apply(e);
        }

        public void ChangeBankType(string type)
        {
            Apply(new BankTypeChanged(type));
        }

        public void ChangeSectionContent(Guid section, ContentExamSection content)
        {
            // Ignore the command if there is no content.
            if (content == null)
                return;

            // Ignore the command if there is no matching set.
            var s = Data.FindSection(section);
            if (s == null)
                return;

            // Ignore the command if the content is not actually changed.
            if (s.Content.IsEqual(content))
                return;

            var e = new SectionContentChanged(section, content);

            Apply(e);
        }

        public void ChangeSetStandard(Guid set, Guid standard)
        {
            if (standard == Guid.Empty)
                return;

            var s = Data.Sets.SingleOrDefault(x => x.Identifier == set);
            if (s == null || s.Standard == standard)
                return;

            var e = new SetStandardChanged(set, standard);

            Apply(e);
        }

        public void ChangeCriterionFilter(Guid criterion, decimal setWeight, int? questionLimit, string tagFilter, PivotTable pivotFilter)
        {
            var s = Data.FindCriterion(criterion);
            if (s == null)
                return;

            var e = new CriterionFilterChanged(criterion, setWeight, questionLimit, tagFilter, pivotFilter);

            Apply(e);
        }

        public void ChangeBankEdition(string major, string minor)
        {
            // Ignore the command if either part of the version number is missing.
            if (string.IsNullOrEmpty(major) || string.IsNullOrEmpty(minor))
                return;

            Apply(new BankEditionChanged(major, minor));
        }

        public void ChangeBankStatus(bool isActive)
        {
            Apply(new BankStatusChanged(isActive));
        }

        public void ChangeCommentAuthorRole(Guid commentId, string authorRole)
        {
            var comment = Data.FindComment(commentId);
            if (comment == null || string.Equals(comment.AuthorRole, authorRole, StringComparison.OrdinalIgnoreCase))
                return;

            Apply(new CommentAuthorRoleChanged(commentId, authorRole));
        }

        public void ChangeCommentVisibility(Guid commentId, bool isHidden)
        {
            var comment = Data.FindComment(commentId);
            if (comment == null || comment.IsHidden == isHidden)
                return;

            Apply(new CommentVisibilityChanged(commentId, isHidden));
        }

        public void ChangeOption(Guid question, int number, ContentTitle content, decimal points, bool? isTrue, decimal? cutScore, Guid? standard)
        {
            var o = Data.FindOption(question, number);
            if (o == null)
                return;

            var e = new OptionChanged(question, number, content, points, isTrue, cutScore, standard);

            Apply(e);
        }

        public void ChangeFormAsset(Guid form, int asset)
        {
            var f = Data.FindForm(form);
            if (f == null || f.Asset == asset)
                return;

            var e = new FormAssetChanged(form, asset);

            Apply(e);
        }

        public void ChangeFormClassification(Guid form, string instrument, string theme)
        {
            var f = Data.FindForm(form);
            if (f == null || f.Classification.Instrument == instrument && f.Classification.Theme == theme)
                return;

            var e = new FormClassificationChanged(form, instrument, theme);

            Apply(e);
        }

        public void ChangeFormAddendum(Guid form, FormAddendumItem[] acronyms, FormAddendumItem[] formulas, FormAddendumItem[] figures, bool removeObsolete)
        {
            var f = Data.FindForm(form);
            if (f == null)
                return;

            var attachments = new HashSet<(int Asset, int Version)>(Data.EnumerateAllAttachments().Select(x => (x.Asset, x.AssetVersion)));

            acronyms = Validate(acronyms, f.Addendum.Acronyms);
            formulas = Validate(formulas, f.Addendum.Formulas);
            figures = Validate(figures, f.Addendum.Figures);

            if (!removeObsolete && acronyms == null && formulas == null && figures == null)
                return;

            var e = new FormAddendumChanged(form, acronyms, formulas, figures, removeObsolete);

            Apply(e);

            FormAddendumItem[] Validate(FormAddendumItem[] changeValue, List<FormAddendumItem> stateValue)
            {
                var result = changeValue.Where(x => attachments.Contains((x.Asset, x.Version))).Distinct().ToArray();

                if (result.Length == 0 && stateValue.Count == 0)
                    return null;

                if (result.Length != stateValue.Count)
                    return result;

                return result.Zip(stateValue, (i1, i2) => i1.IsEqual(i2)).All(x => x)
                    ? null
                    : result;
            }
        }

        public void ChangeFormCode(Guid form, string code, string source, string origin)
        {
            var f = Data.FindForm(form);
            if (f == null)
                return;

            Apply(new FormCodeChanged(form, code, source, origin));
        }

        public void ChangeFormContent(Guid form, ContentExamForm content, bool hasDiagrams, ReferenceMaterialType hasReferenceMaterials)
        {
            var f = Data.FindForm(form);
            if (f == null || f.Content.IsEqual(content) && f.HasDiagrams == hasDiagrams && f.HasReferenceMaterials == hasReferenceMaterials)
                return;

            var e = new FormContentChanged(form, content, hasDiagrams, hasReferenceMaterials);

            Apply(e);
        }

        public void ChangeFormGradebook(Guid form, Guid? gradebook)
        {
            var f = Data.FindForm(form);
            if (f == null || f.Gradebook == gradebook)
                return;

            var e = new FormGradebookChanged(form, gradebook);

            Apply(e);
        }

        public void ChangeFormInvigilation(Guid form, FormInvigilation invigilation)
        {
            if (invigilation == null
                || invigilation.AttemptLimit < 0 || invigilation.AttemptLimitPerSession < 0
                || invigilation.TimeLimit < 0 || invigilation.TimeLimitPerLockout < 0 || invigilation.TimeLimitPerSession < 0)
                return;

            var f = Data.FindForm(form);
            if (f == null || f.Invigilation.Equals(invigilation))
                return;

            var e = new FormInvigilationChanged(form, invigilation);

            Apply(e);
        }

        public void ChangeFormName(Guid form, string name)
        {
            var f = Data.FindForm(form);
            if (f == null || name == f.Name)
                return;

            var e = new FormNameChanged(form, name);

            Apply(e);
        }

        public void ChangeFormVersion(Guid form, string major, string minor)
        {
            var f = Data.FindForm(form);
            if (f == null)
                return;

            var e = new FormVersionChanged(form, major, minor);

            Apply(e);
        }

        public void ChangeQuestionClassification(Guid question, QuestionClassification classification)
        {
            if (classification == null)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Classification.Equals(classification))
                return;

            var e = new QuestionClassificationChanged(question, classification);

            Apply(e);
        }

        public void ChangeQuestionComposedVoice(Guid question, ComposedVoice composedVoice)
        {
            if (composedVoice == null)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.ComposedVoice || q.ComposedVoice.IsEqual(composedVoice))
                return;

            var e = new QuestionComposedVoiceChanged(question, composedVoice);

            Apply(e);
        }

        public void ChangeQuestionContent(Guid question, ContentExamQuestion content)
        {
            if (content == null)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Content.IsEqual(content))
                return;

            var e = new QuestionContentChanged(question, content);

            Apply(e);
        }

        public void ChangeQuestionFlag(Guid question, FlagType flag)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Flag == flag)
                return;

            var e = new QuestionFlagChanged(question, flag);

            Apply(e);
        }

        public void ChangeQuestionGradeItem2(Guid form, Guid question, Guid? gradeItem)
        {
            var f = Data.FindForm(form);
            if (f == null)
                return;

            var questions = f.GetQuestions();

            var q = questions.Find(x => x.Identifier == question);
            if (q == null
                || gradeItem.HasValue && q.GradeItems.TryGetValue(form, out var existing) && existing == gradeItem
                || gradeItem == null && !q.GradeItems.ContainsKey(form)
                )
            {
                return;
            }

            var e = new QuestionGradeItemChanged2(form, question, gradeItem);

            Apply(e);
        }

        public void ChangeQuestionLikertRowGradeItem(Guid form, Guid question, Guid likertRow, Guid? gradeItem)
        {
            var f = Data.FindForm(form);
            if (f == null)
                return;

            var questions = f.GetQuestions();

            var q = questions.Find(x => x.Identifier == question);
            if (q == null)
                return;

            var r = q.Likert?.GetRow(likertRow);
            if (r == null
                || gradeItem.HasValue && r.GradeItems.TryGetValue(form, out var existing) && existing == gradeItem
                || gradeItem == null && !r.GradeItems.ContainsKey(form)
                )
            {
                return;
            }

            var e = new QuestionLikertRowGradeItemChanged(form, question, likertRow, gradeItem);

            Apply(e);
        }

        public void ChangeQuestionHotspotImage(Guid question, HotspotImage image)
        {
            if (image.Url.IsEmpty() || image.Width <= 0 || image.Height <= 0)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || !q.Type.IsHotspot())
                return;

            if (q.Hotspot.Image.IsEqual(image))
                return;

            var e = new QuestionHotspotImageChanged(question, image);

            Apply(e);
        }

        public void ChangeQuestionHotspotOption(Guid question, Guid option, HotspotShape shape, ContentTitle content, decimal points)
        {
            var q = Data.FindQuestion(question);
            if (q == null || !q.Type.IsHotspot())
                return;

            var o = q.Hotspot.GetOption(option);
            if (o == null || shape.GetType() != o.Shape.GetType())
                return;

            var isChanged = o.Points != points || !o.Shape.IsEqual(shape) || !o.Content.IsEqual(content);
            if (!isChanged)
                return;

            var e = new QuestionHotspotOptionChanged(question, option, shape, content, points);

            Apply(e);
        }

        public void ChangeQuestionHotspotPinLimit(Guid question, int pinLimit)
        {
            if (pinLimit < Hotspot.MinPinLimit)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.HotspotCustom || q.Hotspot.PinLimit == pinLimit)
                return;

            var e = new QuestionHotspotPinLimitChanged(question, pinLimit);

            Apply(e);
        }

        public void ChangeQuestionHotspotShowShapes(Guid question, bool showShapes)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.HotspotCustom || q.Hotspot.ShowShapes == showShapes)
                return;

            var e = new QuestionHotspotShowShapesChanged(question, showShapes);

            Apply(e);
        }

        public void ChangeQuestionLayout(Guid question, OptionLayout layout)
        {
            if (layout == null)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Layout.Equals(layout))
                return;

            if ((q.Layout.Type == OptionLayoutType.None || q.Layout.Type == OptionLayoutType.List) && (layout.Type == OptionLayoutType.None || layout.Type == OptionLayoutType.List))
                return;

            var e = new QuestionLayoutChanged(question, layout);

            Apply(e);
        }

        public void ChangeQuestionLikertColumn(Guid question, Guid column, ContentTitle content)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Likert)
                return;

            var c = q.Likert.GetColumn(column);
            if (c == null || c.Content.IsEqual(content))
                return;

            var e = new QuestionLikertColumnChanged(question, column, content);

            Apply(e);
        }

        public void ChangeQuestionLikertOptions(Guid question, IEnumerable<LikertOption> options)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Likert)
                return;

            var changedOptions = new List<LikertOption>();

            foreach (var option in options)
            {
                var o = q.Likert.GetOption(option);
                if (o == null)
                    continue;

                if (!o.IsEqual(option))
                    changedOptions.Add(option);
            }

            if (changedOptions.Count == 0)
                return;

            var e = new QuestionLikertOptionsChanged(question, changedOptions);

            Apply(e);
        }

        public void ChangeQuestionLikertRow(Guid question, Guid row, Guid standard, Guid[] subStandards, ContentTitle content)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Likert)
                return;

            var r = q.Likert.GetRow(row);
            if (r == null)
                return;

            var isSame = r.Standard == standard
                && r.SubStandards.EmptyIfNull().OrderBy(x => x).SequenceEqual(subStandards.EmptyIfNull().OrderBy(x => x))
                && r.Content.IsEqual(content); ;

            if (isSame)
                return;

            var e = new QuestionLikertRowChanged(question, row, standard, subStandards, content);

            Apply(e);
        }

        public void ChangeQuestionMatches(Guid question, MatchingList matches)
        {
            if (matches == null)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Matches.Equals(matches))
                return;

            var e = new QuestionMatchesChanged(question, matches);

            Apply(e);
        }

        public void ChangeQuestionPublicationStatus(Guid question, PublicationStatus status)
        {
            if (!Enum.IsDefined(typeof(PublicationStatus), status))
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.PublicationStatus == status)
                return;

            var e = new QuestionPublicationStatusChanged(question, status);

            Apply(e);
        }

        public void ChangeQuestionOrderingLabel(Guid question, OrderingLabel label)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Ordering)
                return;

            if (label == null)
                label = new OrderingLabel();

            if (q.Ordering.Label.IsEqual(label))
                return;

            var e = new QuestionOrderingLabelChanged(question, label.Show, label.TopContent, label.BottomContent);

            Apply(e);
        }

        public void ChangeQuestionOrderingOption(Guid question, Guid option, ContentTitle content)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Ordering)
                return;

            if (content == null)
                content = new ContentTitle();

            var o = q.Ordering.GetOption(option);
            if (o == null || o.Content.IsEqual(content))
                return;

            var e = new QuestionOrderingOptionChanged(question, option, content);

            Apply(e);
        }

        public void ChangeQuestionOrderingSolution(Guid question, Guid solution, decimal points, decimal? cutScore)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Ordering)
                return;

            var s = q.Ordering.GetSolution(solution);
            if (s == null || s.Points == points && s.CutScore == cutScore)
                return;

            var e = new QuestionOrderingSolutionChanged(question, solution, points, cutScore);

            Apply(e);
        }

        public void ChangeQuestionRandomization(Guid question, Randomization randomization)
        {
            if (randomization == null)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Randomization.Equals(randomization))
                return;

            var e = new QuestionRandomizationChanged(question, randomization);

            Apply(e);
        }

        public void ChangeQuestionScoring(Guid question, decimal? points, decimal? cutScore, QuestionCalculationMethod calculationMethod)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Points == points && q.CutScore == cutScore && q.CalculationMethod == calculationMethod)
                return;

            var e = new QuestionScoringChanged(question, points, cutScore, calculationMethod);

            Apply(e);
        }

        public void ChangeQuestionSet(Guid question, Guid set)
        {
            if (set == Guid.Empty)
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Set.Identifier == set)
                return;

            var e = new QuestionSetChanged(question, set);

            Apply(e);
        }

        public void ChangeQuestionStandard(Guid question, Guid standard, Guid[] subStandards)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Standard == standard && q.SubStandards.EmptyIfNull().Length == subStandards.EmptyIfNull().Length && (subStandards.IsEmpty() || subStandards.All(x => q.SubStandards.Contains(x))))
                return;

            var e = new QuestionStandardChanged(question, standard, subStandards);

            Apply(e);
        }

        public void ChangeQuestionCondition(Guid question, string condition)
        {
            if (string.IsNullOrEmpty(condition))
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Condition == condition)
                return;

            var e = new QuestionConditionChanged(question, condition);

            Apply(e);
        }

        public void ChangeSetRandomization(Guid set, Randomization randomization)
        {
            if (randomization == null)
                return;

            var q = Data.FindSet(set);
            if (q == null || q.Randomization.Equals(randomization))
                return;

            var e = new SetRandomizationChanged(set, randomization);

            Apply(e);
        }

        public void ChangeSpecificationCalculation(Guid spec, ScoreCalculation calculation)
        {
            // Ignore the command if there is no matching set.
            var s = Data.Specifications.SingleOrDefault(x => x.Identifier == spec);
            if (s == null)
                return;

            var e = new SpecificationCalculationChanged(spec, calculation);

            Apply(e);
        }

        public void ChangeSpecificationContent(Guid spec, ContentExamSpecification content)
        {
            var s = Data.FindSpecification(spec);
            if (s == null)
                return;

            var e = new SpecificationContentChanged(spec, content);
            Apply(e);
        }

        public void ChangeSpecificationTabTimeLimit(Guid spec, SpecificationTabTimeLimit tabTimeLimit)
        {
            var s = Data.FindSpecification(spec);
            if (s == null || !s.IsTabTimeLimitAllowed || s.TabTimeLimit == tabTimeLimit)
                return;

            var e = new SpecificationTabTimeLimitChanged(spec, tabTimeLimit);
            Apply(e);
        }

        public void EnableThirdPartyAssessment(Guid form)
        {
            var f = Data.FindForm(form);
            if (f == null)
                return;

            Apply(new ThirdPartyAssessmentEnabled(form));
        }

        public void DisableThirdPartyAssessment(Guid form)
        {
            var f = Data.FindForm(form);
            if (f == null)
                return;

            Apply(new ThirdPartyAssessmentDisabled(form));
        }

        public void DisconnectQuestionRubric(Guid question)
        {
            var q = Data.FindQuestion(question);
            if (q?.Rubric == null)
                return;

            Apply(new QuestionRubricDisconnected(question));
        }

        public void OpenBank(BankState bank)
        {
            Apply(new BankOpened(bank.Clone()));
        }

        public void DeleteQuestionLikertColumn(Guid question, Guid column)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Likert)
                return;

            var c = q.Likert.GetColumn(column);
            if (c == null)
                return;

            var e = new QuestionLikertColumnDeleted(question, column);

            Apply(e);
        }

        public void DeleteQuestionLikertRow(Guid question, Guid row)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Likert)
                return;

            var r = q.Likert.GetRow(row);
            if (r == null)
                return;

            var e = new QuestionLikertRowDeleted(question, row);

            Apply(e);
        }

        public void DeleteQuestionOrderingOption(Guid question, Guid option)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Ordering || q.Ordering.GetOption(option) == null)
                return;

            var e = new QuestionOrderingOptionDeleted(question, option);

            Apply(e);
        }

        public void DeleteQuestionOrderingSolution(Guid question, Guid solution)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Ordering || q.Ordering.GetSolution(solution) == null)
                return;

            var e = new QuestionOrderingSolutionDeleted(question, solution);

            Apply(e);
        }

        public void DuplicateQuestion(Guid sourceId, Guid destinationId, int destinationAsset)
        {
            if (sourceId == Guid.Empty || Data.FindQuestion(sourceId) == null || Data.FindQuestion(destinationId) != null)
                return;

            Apply(new QuestionDuplicated2(sourceId, destinationId, destinationAsset));

            var comments = Data.Comments.Where(x => x.Subject == sourceId).ToArray();
            foreach (var comment in comments)
                Apply(new CommentDuplicated(comment.Identifier, sourceId, UuidFactory.Create(), destinationId, CommentType.Question));
        }

        public void ImportSet(Set set) => Apply(new SetImported(set));

        public void LockBank() => Apply(new BankLocked());

        public void MergeSets(Guid set)
        {
            var e = new SetsMerged(set);

            Apply(e);
        }

        public void ModifyFormLanguages(Guid formId, string[] languages)
        {
            var form = Data.FindForm(formId);
            if (form == null)
                return;

            var count1 = form.Languages.EmptyIfNull().Length;
            var count2 = languages.EmptyIfNull().Length;
            if (count1 == count2 && (count1 == 0 || form.Languages.All(x => languages.Contains(x, StringComparer.OrdinalIgnoreCase))))
                return;

            var e = new FormLanguagesModified(formId, languages);

            Apply(e);
        }

        public void MoveComment(Guid comment, CommentType type, Guid subject)
        {
            if (Data.FindComment(comment) == null)
                return;

            Apply(new CommentMoved(comment, type, subject));
        }

        public void MoveQuestion(Guid set, Guid competency, int asset, Guid question)
        {
            if (asset == 0)
                throw new MissingAssetNumberException();

            if (question == Guid.Empty || Data.FindSet(set) == null || Data.FindQuestion(question) == null)
                return;

            var e = new QuestionMoved(set, competency, asset, question);

            Apply(e);
        }

        public bool CanMoveQuestionIn(Guid set, Guid question)
        {
            return question != Guid.Empty
                && Data.FindSet(set) != null
                && Data.FindQuestion(question) == null;
        }

        public void MoveQuestionIn(Guid bank, Guid set, Guid competency, int asset, Question question, Comment[] comments)
        {
            if (asset == 0)
                throw new MissingAssetNumberException();

            if (!CanMoveQuestionIn(set, question.Identifier))
                return;

            var e = new QuestionMovedIn(bank, set, competency, asset, question, comments);

            Apply(e);
        }

        public bool CanMoveQuestionOut(Guid question)
        {
            var entity = Data.FindQuestion(question);
            return CanMoveQuestionOutInternal(entity);
        }

        private static bool CanMoveQuestionOutInternal(Question question) =>
            question != null && question.IsLastVersion() && question.Fields.Count == 0;

        public Tuple<Question, Comment[]> MoveQuestionOut(Guid bank, Guid set, Guid competency, Guid question)
        {
            var entity = Data.FindQuestion(question);
            if (!CanMoveQuestionOutInternal(entity))
                return null;

            var result = new Tuple<Question, Comment[]>(entity.Clone(), entity.Comments.Select(x => x.Clone()).ToArray());

            result.Item1.Set = null;
            result.Item1.Fields.Clear();
            result.Item1.AttachmentIdentifiers.Clear();
            result.Item1.BankIndex = -1;
            result.Item1.Standard = Guid.Empty;
            result.Item1.SubStandards = null;

            var e = new QuestionMovedOut(bank, set, competency, question);

            Apply(e);

            return result;
        }

        public void PostComment(Guid comment, FlagType flag, CommentType type, Guid subject, Guid author, string authorRole, string category, string text, Guid? instructor, DateTimeOffset? eventDate, string format, DateTimeOffset posted)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Apply(new BankCommentPosted(comment, flag, type, subject, author, authorRole, category, text, instructor, eventDate, format, posted));
        }

        public void PublishForm(Guid form, FormPublication publication)
        {
            var f = Data.FindForm(form);
            if (f == null)
                return;

            if (f.Specification.Type == SpecificationType.Static)
                Apply(new AssessmentQuestionOrderVerified(form, f.GetStaticFormQuestionIdentifiersInOrder()));

            Apply(new FormPublished(form, publication));
        }

        public void ReconfigureSection(Guid sectionId, bool warningOnNextTabEnabled, bool breakTimerEnabled, int timeLimit, FormSectionTimeType timerType)
        {
            if (timeLimit < 0 || timeLimit > 1440)
                return;

            var section = Data.FindSection(sectionId);
            if (section == null)
                return;

            var specification = section.Form.Specification;
            if (!specification.SectionsAsTabsEnabled || specification.TabNavigationEnabled)
                return;

            var isChanged = section.WarningOnNextTabEnabled != warningOnNextTabEnabled
                || section.BreakTimerEnabled != breakTimerEnabled
                || section.TimeLimit != timeLimit
                || section.TimerType != timerType;
            if (!isChanged)
                return;

            var e = new SectionReconfigured(sectionId, warningOnNextTabEnabled, breakTimerEnabled, timeLimit, timerType);

            Apply(e);
        }

        public void ReconfigureSpecification(Guid spec, ConsequenceType? consequence, int formLimit, int questionLimit)
        {
            // Ignore the command if there is no matching specification.
            var a = Data.FindSpecification(spec);
            if (a == null)
                return;

            var e = new SpecificationReconfigured(spec, consequence, formLimit, questionLimit);

            Apply(e);
        }

        public void DeleteAttachment(Guid attachment)
        {
            // Ignore the command if there is no matching attachment.
            var a = Data.FindAttachment(attachment);
            if (a == null)
                return;

            var e = new BankAttachmentDeleted(attachment);

            Apply(e);
        }

        public void DeleteAttachmentFromQuestion(Guid attachmentId, Guid questionId)
        {
            if (attachmentId == Guid.Empty || questionId == Guid.Empty)
                return;

            var attachment = Data.FindAttachment(attachmentId);
            var question = Data.FindQuestion(questionId);

            if (attachment == null || question == null || !attachment.QuestionIdentifiers.Contains(questionId) && !question.AttachmentIdentifiers.Contains(attachmentId))
                return;

            var e = new AttachmentDeletedFromQuestion(attachmentId, questionId);

            Apply(e);
        }

        public void RejectComment(Guid comment)
        {
            var c = Data.FindComment(comment);
            if (c == null)
                return;

            var e = new CommentRejected(comment);

            Apply(e);
        }

        public void RetractComment(Guid comment)
        {
            var c = Data.FindComment(comment);
            if (c == null)
                return;

            var e = new CommentRetracted(comment);

            Apply(e);
        }

        public void DeleteField(Guid field, Guid form, Guid question)
        {
            // Ignore the command if there is no matching field.
            var f = Data.FindForm(form);
            if (f == null)
                return;

            var ff = f.Sections.SelectMany(x => x.Fields.Where(y => y.Identifier == field && y.QuestionIdentifier == question)).FirstOrDefault();
            if (ff == null)
                return;

            var e = new FieldDeleted(field, form, question);

            Apply(e);
        }

        public void DeleteFields(Guid form, Guid question)
        {
            // Ignore the command if there is no matching field.
            var f = Data.FindQuestion(question);
            if ((f?.Fields).IsEmpty())
                return;

            var e = new FieldsDeleted(form, question);

            Apply(e);
        }

        public void DeleteForm(Guid form)
        {
            // Ignore the command if there is no matching form.
            var f = Data.FindForm(form);
            if (f == null)
                return;

            var e = new FormDeleted(form);

            Apply(e);
        }

        public void DeleteOption(Guid question, int option)
        {
            var o = Data.FindOption(question, option);
            if (o == null)
                return;

            var e = new OptionDeleted(question, option);

            Apply(e);
        }

        public void DeleteQuestion(Guid question, bool removeAllVersions)
        {
            // Ignore the command if there is no matching question.
            var q = Data.FindQuestion(question);
            if (q == null)
                return;

            var e = new QuestionDeleted(question, removeAllVersions);

            Apply(e);
        }

        public void DeleteSection(Guid section)
        {
            // Ignore the command if there is no matching set.
            var s = Data.FindSection(section);
            if (s == null)
                return;

            var e = new SectionDeleted(section);

            Apply(e);
        }

        public void DeleteSet(Guid set)
        {
            // Ignore the command if there is no matching set.
            var s = Data.Sets.SingleOrDefault(x => x.Identifier == set);
            if (s == null)
                return;

            var e = new SetDeleted(set);

            Apply(e);
        }

        public void DeleteCriterion(Guid criterion)
        {
            var s = Data.FindCriterion(criterion);
            if (s == null)
                return;

            var e = new CriterionDeleted(criterion);

            Apply(e);
        }

        public void DeleteSpecification(Guid specification)
        {
            var s = Data.FindSpecification(specification);
            if (s == null)
                return;

            var e = new SpecificationDeleted(specification);

            Apply(e);
        }

        public void DeleteQuestionHotspotOption(Guid question, Guid option)
        {
            var q = Data.FindQuestion(question);
            if (q == null || !q.Type.IsHotspot() || q.Hotspot.GetOption(option) == null)
                return;

            var e = new QuestionHotspotOptionDeleted(question, option);

            Apply(e);
        }

        public void DisableSectionsAsTabs(Guid specification)
        {
            var s = Data.FindSpecification(specification);
            if (s == null || s.Type != SpecificationType.Static || !s.SectionsAsTabsEnabled)
                return;

            var e = new SectionsAsTabsDisabled(specification);

            Apply(e);
        }

        public void EnableSectionsAsTabs(Guid specification)
        {
            var s = Data.FindSpecification(specification);
            if (s == null || s.Type != SpecificationType.Static || s.SectionsAsTabsEnabled)
                return;

            var e = new SectionsAsTabsEnabled(specification);

            Apply(e);
        }

        public void DisableTabNavigation(Guid specification)
        {
            var s = Data.FindSpecification(specification);
            if (s == null || s.Type != SpecificationType.Static || !s.SectionsAsTabsEnabled || !s.TabNavigationEnabled)
                return;

            var e = new TabNavigationDisabled(specification);

            Apply(e);
        }

        public void EnableTabNavigation(Guid specification)
        {
            var s = Data.FindSpecification(specification);
            if (s == null || s.Type != SpecificationType.Static || !s.SectionsAsTabsEnabled || s.TabNavigationEnabled)
                return;

            var e = new TabNavigationEnabled(specification);

            Apply(e);
        }

        public void DisableSingleQuestionPerTab(Guid specification)
        {
            var s = Data.FindSpecification(specification);
            if (s == null || s.Type != SpecificationType.Static || !s.SectionsAsTabsEnabled || s.TabNavigationEnabled || !s.SingleQuestionPerTabEnabled)
                return;

            var e = new SingleQuestionPerTabDisabled(specification);

            Apply(e);
        }

        public void EnableSingleQuestionPerTab(Guid specification)
        {
            var s = Data.FindSpecification(specification);
            if (s == null || s.Type != SpecificationType.Static || !s.SectionsAsTabsEnabled || s.TabNavigationEnabled || s.SingleQuestionPerTabEnabled)
                return;

            var e = new SingleQuestionPerTabEnabled(specification);

            Apply(e);
        }

        public void RenameBank(string name)
        {
            // Ignore the command if there is no name.
            if (string.IsNullOrEmpty(name))
                return;

            // Ignore the command if the name is not actually being changed.
            if (StringHelper.Equals(name, Data.Name))
                return;

            var e = new BankRenamed(name);

            Apply(e);
        }

        public void RenameSet(Guid set, string name)
        {
            // Ignore the command if there is no name.
            if (string.IsNullOrEmpty(name))
                return;

            // Ignore the command if there is no matching set.
            var s = Data.Sets.SingleOrDefault(x => x.Identifier == set);
            if (s == null)
                return;

            // Ignore the command if the name is not actually changed.
            if (StringHelper.Equals(name, s.Name))
                return;

            var e = new SetRenamed(set, name);

            Apply(e);
        }

        public void RenameSpecification(Guid specification, string name)
        {
            var s = Data.FindSpecification(specification);
            if (s == null || s.Name == name)
                return;

            Apply(new SpecificationRenamed(specification, name));
        }

        public void RetypeSpecification(Guid specification, SpecificationType type)
        {
            var s = Data.FindSpecification(specification);
            if (s == null || s.Type == type)
                return;

            Apply(new SpecificationRetyped(specification, type));
        }

        public void ReorderFields(Guid section, Dictionary<int, int> sequences)
        {
            if (sequences.IsEmpty())
                return;

            var s = Data.FindSection(section);
            if (s == null)
                return;

            var fieldsCount = 0;
            var changesCount = 0;

            foreach (var o in s.Fields)
            {
                if (!sequences.TryGetValue(o.Sequence, out var n))
                    continue;

                fieldsCount++;

                if (o.Sequence != n)
                    changesCount++;
            }

            if (changesCount == 0 || fieldsCount != sequences.Count)
                return;

            var e = new FieldsReordered(section, sequences);

            Apply(e);
        }

        public void ReorderQuestionHotspotOptions(Guid question, Dictionary<Guid, int> optionsOrder)
        {
            var q = Data.FindQuestion(question);
            if (q == null || !q.Type.IsHotspot())
                return;

            var changeOrder = new Dictionary<int, int>();

            for (var i = 0; i < q.Hotspot.Options.Count; i++)
            {
                var option = q.Hotspot.Options[i];
                if (optionsOrder.TryGetValue(option.Identifier, out var index) && i != index)
                    changeOrder[option.Number] = index;
            }

            if (changeOrder.Count == 0)
                return;

            var e = new QuestionHotspotOptionsReordered(question, changeOrder);

            Apply(e);
        }

        public void ReorderQuestionLikert(Guid question, Dictionary<Guid, int> rows, Dictionary<Guid, int> columns)
        {
            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Likert)
                return;

            var rowsOrder = new Dictionary<Guid, int>();
            if (rows.IsNotEmpty())
            {
                foreach (var row in q.Likert.Rows)
                {
                    if (rows.TryGetValue(row.Identifier, out var index) && row.Index != index)
                        rowsOrder[row.Identifier] = index;
                }
            }

            var columnsOrder = new Dictionary<Guid, int>();
            if (columns.IsNotEmpty())
            {
                foreach (var column in q.Likert.Columns)
                {
                    if (columns.TryGetValue(column.Identifier, out var index) && column.Index != index)
                        columnsOrder[column.Identifier] = index;
                }
            }

            if (rowsOrder.Count == 0 && columnsOrder.Count == 0)
                return;

            var e = new QuestionLikertReordered(question, rowsOrder, columnsOrder);

            Apply(e);
        }

        public void ReorderQuestionOrderingOptions(Guid question, Guid[] optionsOrder)
        {
            if (optionsOrder.IsEmpty())
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Ordering)
                return;

            var ordering = q.Ordering;
            var options = ordering.Options;

            if (options.Count != optionsOrder.Length || optionsOrder.Any(x => ordering.GetOption(x) == null))
                return;

            var changeOrder = options
                .Zip(optionsOrder, (a, b) => (a, b))
                .Select((x, i) => (Source: x.a.Identifier, Destination: x.b, Index: i))
                .Where(x => x.Source != x.Destination)
                .ToDictionary(x => x.Destination, x => x.Index);

            if (changeOrder.Count == 0)
                return;

            var e = new QuestionOrderingOptionsReordered(question, changeOrder);

            Apply(e);
        }

        public void ReorderQuestionOrderingSolutions(Guid question, Guid[] solutionsOrder)
        {
            if (solutionsOrder.IsEmpty())
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Ordering)
                return;

            var ordering = q.Ordering;
            var solutions = ordering.Solutions;

            if (solutions.Count != solutionsOrder.Length || solutionsOrder.Any(x => ordering.GetSolution(x) == null))
                return;

            var changeOrder = solutions
                .Zip(solutionsOrder, (a, b) => (a, b))
                .Select((x, i) => (Source: x.a.Identifier, Destination: x.b, Index: i))
                .Where(x => x.Source != x.Destination)
                .ToDictionary(x => x.Destination, x => x.Index);

            if (changeOrder.Count == 0)
                return;

            var e = new QuestionOrderingSolutionsReordered(question, changeOrder);

            Apply(e);
        }

        public void ReorderQuestionOrderingSolutionOptions(Guid question, Guid solution, Guid[] optionsOrder)
        {
            if (optionsOrder.IsEmpty())
                return;

            var q = Data.FindQuestion(question);
            if (q == null || q.Type != QuestionItemType.Ordering)
                return;

            var ordering = q.Ordering;
            if (ordering.Options.Count != optionsOrder.Length || optionsOrder.Any(x => ordering.GetOption(x) == null))
                return;

            var qSolution = ordering.GetSolution(solution);
            if (qSolution == null)
                return;

            var changeOrder = qSolution.Options
                .Zip(optionsOrder, (a, b) => (a, b))
                .Select((x, i) => (Source: x.a, Destination: x.b, Index: i))
                .Where(x => x.Source != x.Destination)
                .ToDictionary(x => x.Destination, x => x.Index);

            if (changeOrder.Count == 0)
                return;

            var e = new QuestionOrderingSolutionOptionsReordered(question, solution, changeOrder);

            Apply(e);
        }

        public void ReorderOptions(Guid question, Dictionary<int, int> sequences)
        {
            if (sequences.IsEmpty())
                return;

            var q = Data.FindQuestion(question);
            if (q == null)
                return;

            var optionsCount = 0;
            var changesCount = 0;

            foreach (var o in q.Options)
            {
                if (!sequences.TryGetValue(o.Sequence, out var n))
                    continue;

                optionsCount++;

                if (o.Sequence != n)
                    changesCount++;
            }

            if (changesCount == 0 || optionsCount != sequences.Count)
                return;

            var e = new OptionsReordered(question, sequences);

            Apply(e);
        }

        public void ReorderQuestions(Guid set, Dictionary<int, int> sequences)
        {
            if (sequences.IsEmpty())
                return;

            var s = Data.FindSet(set);
            if (s == null)
                return;

            var questionsCount = 0;
            var changesCount = 0;

            foreach (var q in s.Questions)
            {
                if (!sequences.TryGetValue(q.Sequence, out var n))
                    continue;

                questionsCount++;

                if (q.Sequence != n)
                    changesCount++;
            }

            if (changesCount == 0 || questionsCount != sequences.Count)
                return;

            var e = new QuestionsReordered(set, sequences);

            Apply(e);
        }

        public void ReorderSections(Guid form, Dictionary<int, int> sequences)
        {
            if (sequences.IsEmpty())
                return;

            var f = Data.FindForm(form);
            if (f == null)
                return;

            var sectionsCount = 0;
            var changesCount = 0;

            foreach (var o in f.Sections)
            {
                if (!sequences.TryGetValue(o.Sequence, out var n))
                    continue;

                sectionsCount++;

                if (o.Sequence != n)
                    changesCount++;
            }

            if (changesCount == 0 || sectionsCount != sequences.Count)
                return;

            var e = new SectionsReordered(form, sequences);

            Apply(e);
        }

        public void ReorderSets(Dictionary<int, int> sequences)
        {
            if (sequences.IsEmpty())
                return;

            var setsCount = 0;
            var changesCount = 0;

            foreach (var set in Data.Sets)
            {
                if (!sequences.TryGetValue(set.Sequence, out var n))
                    continue;

                setsCount++;

                if (set.Sequence != n)
                    changesCount++;
            }

            if (changesCount == 0 || setsCount != sequences.Count)
                return;

            var e = new SetsReordered(sequences);

            Apply(e);
        }

        public void ReviseComment(Guid comment, Guid author, FlagType flag, string category, string text, Guid? instructor, DateTimeOffset? exam, string format, DateTimeOffset revised)
        {
            var c = Data.FindComment(comment);
            if (c == null)
                return;

            var e = new BankCommentModified(comment, author, flag, category, text, instructor, exam, format, revised);

            Apply(e);
        }

        public void SwapFields(Guid a, Guid b)
        {
            if (a == Guid.Empty || b == Guid.Empty || a == b)
                return;

            if (Data.FindField(a) == null || Data.FindField(b) == null)
                return;

            var e = new FieldsSwapped(a, b);

            Apply(e);
        }

        public void UnarchiveForm(Guid form, bool questions, bool attachments)
        {
            if (Data.FindForm(form) == null)
                return;

            Apply(new FormUnarchived(form, questions, attachments));
        }

        public void UnlockBank() => Apply(new BankUnlocked());

        public void UnpublishForm(Guid form)
        {
            // Ignore the command if there is no matching form.
            if (Data.FindForm(form) == null)
                return;

            var e = new FormUnpublished(form);

            Apply(e);
        }

        public void UpgradeAttachment(Guid currentId, Guid upgradedId)
        {
            if (currentId == Guid.Empty || upgradedId == Guid.Empty || Data.FindAttachment(upgradedId) != null)
                return;

            var current = Data.FindAttachment(currentId);
            if (current == null || !current.IsLastVersion())
                return;

            var e = new AttachmentUpgraded(currentId, upgradedId);

            Apply(e);
        }

        public void UpgradeForm(Guid source, Guid destination, string newName)
        {
            var srcForm = Data.FindForm(source);
            if (!srcForm.IsLastVersion())
                throw new CannotUpgradeOldFormException();

            Apply(new FormUpgraded(source, destination, newName));

            foreach (var srcSection in srcForm.Sections)
            {
                var dstSection = UuidFactory.Create();

                Apply(new SectionAdded(destination, dstSection, srcSection.CriterionIdentifier));

                foreach (var srcField in srcSection.Fields)
                {
                    var dstField = UuidFactory.Create();

                    Apply(new FieldAdded(dstField, dstSection, srcField.QuestionIdentifier, -1));
                }
            }
        }

        public void UpgradeQuestion(Guid currentId, Guid upgradedId)
        {
            if (currentId == Guid.Empty || upgradedId == Guid.Empty || Data.FindQuestion(upgradedId) != null)
                return;

            var current = Data.FindQuestion(currentId);
            if (current == null || !current.IsLastVersion())
                return;

            var e = new QuestionUpgraded(currentId, upgradedId);

            Apply(e);
        }

        public void VerifyAssessmentFormFields(Guid form, Guid[] questions)
        {
            Apply(new AssessmentQuestionOrderVerified(form, questions));
        }

        #endregion
    }
}