using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common.Timeline.Changes;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.Contents.Read;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class BankStore : IBankStore
    {
        private readonly ILearnerAttemptSummarySearch _attemptSummary;

        public BankStore(ILearnerAttemptSummarySearch attemptSummary)
        {
            _attemptSummary = attemptSummary;
        }

        internal InternalDbContext CreateContext() => new InternalDbContext(true);

        #region Methods (cursor)

        private void InsertCursor(IChange e, BankState data, Action<InternalDbContext> change)
        {
            using (var db = CreateContext())
            {
                change?.Invoke(db);

                var query = new QBank();
                BindBankQuery(query, data, ObjectSize(data));
                db.Banks.Add(query);

                SetLastChange(query, e);

                db.SaveChanges();
            }
        }

        private void UpdateCursor(IChange e, Action<InternalDbContext, BankState> change)
        {
            using (var db = CreateContext())
            {
                var data = (BankState)e.AggregateState;
                change?.Invoke(db, data);

                var query = db.Banks.FirstOrDefault(x => x.BankIdentifier == e.AggregateIdentifier);
                if (query != null)
                {
                    BindBankQuery(query, data, ObjectSize(data));
                    SetLastChange(query, e);
                }

                db.SaveChanges();
            }
        }

        private int ObjectSize(object _)
        {
            return 0;
        }

        #endregion

        #region Methods (delete)

        public void DeleteAll()
        {
            using (var db = CreateContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE banks.QBank");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE banks.QBankForm");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE banks.QBankOption");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE banks.QBankQuestion");
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE banks.QBankSpecification");
            }
        }

        public void Delete(Guid bank)
        {
            using (var db = CreateContext())
            {
                var sql = @"
delete from banks.QBank WHERE BankIdentifier = @Aggregate;
delete from assets.QComment WHERE AssessmentBankIdentifier = @Aggregate;
delete from banks.QBankForm WHERE BankIdentifier = @Aggregate;
delete from banks.QBankOption WHERE BankIdentifier = @Aggregate;
delete from banks.QBankQuestion WHERE BankIdentifier = @Aggregate;
delete from banks.QBankSpecification WHERE BankIdentifier = @Aggregate;
";
                db.Database.ExecuteSqlCommand(sql, new SqlParameter("Aggregate", bank));
            }
        }

        private static void DeleteQuestion(InternalDbContext db, Guid questionId) =>
            DeleteQuestions(db, x => x.QuestionIdentifier == questionId);

        private static void DeleteQuestions(InternalDbContext db, Expression<Func<QBankQuestion, bool>> filter) =>
            DeleteQuestions(db, db.BankQuestions.Where(filter).ToArray());

        private static void DeleteQuestions(InternalDbContext db, QBankQuestion[] questions)
        {
            if (questions.Length == 0)
                return;

            var ids = questions.Select(x => x.QuestionIdentifier).ToArray();
            var subQuestions = db.BankQuestions.Where(x => ids.Contains(x.ParentQuestionIdentifier.Value)).ToArray();
            var subIds = subQuestions.Select(x => x.QuestionIdentifier).ToArray();
            var allIds = ids.Union(subIds);

            var subCompetencies = db.BankQuestionSubCompetencies.Where(x => allIds.Contains(x.QuestionIdentifier));
            db.BankQuestionSubCompetencies.RemoveRange(subCompetencies);

            db.BankQuestions.RemoveRange(subQuestions);

            db.BankQuestions.RemoveRange(questions);

            DeleteOptions(db, x => ids.Contains(x.QuestionIdentifier));

            var questionGradeItems = db.BankQuestionGradeItems
                .Where(x => allIds.Contains(x.QuestionIdentifier))
                .ToList();

            db.BankQuestionGradeItems.RemoveRange(questionGradeItems);
        }

        private static void DeleteOptions(InternalDbContext db, Expression<Func<QBankOption, bool>> filter)
        {
            var options = db.BankOptions.Where(filter).ToArray();
            db.BankOptions.RemoveRange(options);
        }

        #endregion

        #region Methods (insert)

        public void Insert(BankOpened e)
        {
            Insert(e, e.Bank);
        }

        private void Insert(IChange e, BankState bank)
        {
            InsertCursor(e, bank, (db) =>
            {
                SyncQuestions(db, bank, true, true, true);

                foreach (var specification in bank.Specifications)
                {
                    var allForms = specification.EnumerateAllForms().ToArray();

                    var specQuery = new QBankSpecification
                    {
                        BankIdentifier = bank.Identifier,
                        CalcDisclosure = specification.Calculation.Disclosure.ToString(),
                        CalcPassingScore = specification.Calculation.PassingScore,
                        SpecFormCount = allForms.Length,
                        SpecIdentifier = specification.Identifier,
                        SpecType = specification.Type.ToString(),
                        SpecName = specification.Name,
                        SpecConsequence = specification.Consequence.ToString(),
                        SpecAsset = specification.Asset,
                        SpecFormLimit = specification.FormLimit,
                        SpecQuestionLimit = specification.QuestionLimit,
                        OrganizationIdentifier = bank.Tenant
                    };

                    db.BankSpecifications.Add(specQuery);

                    foreach (var f in allForms)
                    {
                        var query = new QBankForm();
                        BindFormQuery(query, bank, specification, f);
                        db.BankForms.Add(query);
                    }
                }

                foreach (var comment in bank.Comments)
                {
                    var query = new QComment();
                    BindCommentQuery(query, bank, comment);
                    db.QComments.Add(query);
                }
            });
        }

        private static void InsertOptions(InternalDbContext db, BankState bank, IEnumerable<Option> options)
        {
            foreach (var option in options)
            {
                var query = new QBankOption();

                BindOptionQuery(query, bank, option);

                db.BankOptions.Add(query);
            }
        }

        #endregion

        #region Methods (update)

        private void SetLastChange(QBank bank, IChange change)
        {
            if (change is FormAnalyzed)
                return;

            bank.LastChangeTime = change.ChangeTime;
            bank.LastChangeType = change.GetType().Name;
            bank.LastChangeUser = UserSearch.GetFullName(change.OriginUser);
        }

        public void Update(QBankQuestion entity)
        {
            using (var db = CreateContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Update(QComment entity)
        {
            using (var db = CreateContext())
            {
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Update(AssessmentHookChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
            });
        }

        public void Update(SectionsAsTabsDisabled e)
        {
            UpdateCursor(e, null);
        }

        public void Update(SectionsAsTabsEnabled e)
        {
            UpdateCursor(e, null);
        }

        public void Update(TabNavigationDisabled e)
        {
            UpdateCursor(e, null);
        }

        public void Update(TabNavigationEnabled e)
        {
            UpdateCursor(e, null);
        }

        public void Update(SingleQuestionPerTabDisabled e)
        {
            UpdateCursor(e, null);
        }

        public void Update(SingleQuestionPerTabEnabled e)
        {
            UpdateCursor(e, null);
        }

        public void Update(AttachmentAdded e) => UpdateCursor(e, null);

        public void Update(AttachmentAddedToQuestion e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var question = bank.FindQuestion(e.Question);
                SyncQuestionAttachments(db, question);
            });
        }

        public void Update(AttachmentChanged e) => UpdateCursor(e, null);

        public void Update(AttachmentImageChanged e) => UpdateCursor(e, null);

        public void Update(BankAttachmentDeleted e) => UpdateCursor(e, null);

        public void Update(AttachmentDeletedFromQuestion e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var question = bank.FindQuestion(e.Question);
                SyncQuestionAttachments(db, question);
            });
        }

        public void Update(AttachmentUpgraded e) => UpdateCursor(e, null);

        public void Update(BankAnalyzed e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var forms = db.BankForms.Where(x => x.BankIdentifier == e.AggregateIdentifier);
                foreach (var form in forms)
                {
                    var summary = _attemptSummary.GetFormSummary(form.FormIdentifier);

                    form.AttemptStartedCount = summary.AttemptStartedCount;
                    form.AttemptSubmittedCount = summary.AttemptSubmittedCount;
                    form.AttemptGradedCount = summary.AttemptGradedCount;
                    form.AttemptPassedCount = summary.AttemptPassedCount;
                }
            });
        }

        public void Update(BankContentChanged e) => UpdateCursor(e, null);

        public void Update(BankLevelChanged e) => UpdateCursor(e, null);

        public void Update(BankLocked e) => UpdateCursor(e, null);

        public void Update(BankRenamed e) => UpdateCursor(e, null);

        public void Update(BankStandardChanged e) => UpdateCursor(e, (db, bank) =>
        {
            foreach (var entity in db.BankQuestions.Where(x => x.BankIdentifier == e.AggregateIdentifier).ToArray())
            {
                entity.CompetencyIdentifier = null;

                var subCompetencies = db.BankQuestionSubCompetencies.Where(x => x.QuestionIdentifier == entity.QuestionIdentifier).ToArray();
                db.BankQuestionSubCompetencies.RemoveRange(subCompetencies);
            }

            foreach (var entity in db.BankOptions.Where(x => x.BankIdentifier == e.AggregateIdentifier).ToArray())
                entity.CompetencyIdentifier = null;
        });

        public void Update(BankTypeChanged e) => UpdateCursor(e, null);

        public void Update(BankUnlocked e) => UpdateCursor(e, null);

        public void Update(BankEditionChanged e) => UpdateCursor(e, null);

        public void Update(BankStatusChanged e) => UpdateCursor(e, null);

        public void Update(CommentAuthorRoleChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var comment = bank.FindComment(e.Comment);
                var query = db.QComments.Single(x => x.CommentIdentifier == e.Comment);

                BindCommentQuery(query, bank, comment);
            });
        }

        public void Update(CommentDuplicated e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var comment = new QComment();
                BindCommentQuery(comment, bank, bank.FindComment(e.DestinationComment));
                db.QComments.Add(comment);
            });
        }

        public void Update(CommentMoved e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var comment = bank.FindComment(e.Comment);
                var query = db.QComments.Single(x => x.CommentIdentifier == e.Comment);

                BindCommentQuery(query, bank, comment);
            });
        }

        public void Update(BankCommentPosted e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var query = db.QComments.FirstOrDefault(x => x.CommentIdentifier == e.Comment);
                if (query == null)
                {
                    query = new QComment();
                    db.QComments.Add(query);
                }

                BindCommentQuery(query, bank, bank.FindComment(e.Comment));
            });
        }

        public void Update(CommentRejected e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var comment = db.QComments.Single(x => x.CommentIdentifier == e.Comment);
                db.QComments.Remove(comment);
            });
        }

        public void Update(CommentRetracted e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var comment = db.QComments.Single(x => x.CommentIdentifier == e.Comment);
                db.QComments.Remove(comment);
            });
        }

        public void Update(BankCommentModified e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var comment = bank.FindComment(e.Comment);
                var query = db.QComments.Single(x => x.CommentIdentifier == e.Comment);

                BindCommentQuery(query, bank, comment);
            });
        }

        public void Update(CommentVisibilityChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var comment = bank.FindComment(e.Comment);
                var query = db.QComments.Single(x => x.CommentIdentifier == e.Comment);

                BindCommentQuery(query, bank, comment);
            });
        }

        public void Update(FieldAdded e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindSection(e.Section).Form;
                var summary = db.BankForms.Single(x => x.FormIdentifier == form.Identifier);
                summary.FieldCount++;
            });
        }

        public void Update(FieldDeleted e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var allIds = db.BankQuestions
                    .Where(x => x.ParentQuestionIdentifier == e.Question)
                    .Select(x => x.QuestionIdentifier)
                    .ToList();

                allIds.Add(e.Question);

                var questionGradeItems = db.BankQuestionGradeItems
                    .Where(x => x.FormIdentifier == e.Form && allIds.Contains(x.QuestionIdentifier))
                    .ToList();

                if (questionGradeItems.Count > 0)
                    db.BankQuestionGradeItems.RemoveRange(questionGradeItems);

                var form = bank.FindForm(e.Form);
                var summary = db.BankForms.Single(x => x.FormIdentifier == e.Form);
                summary.FieldCount--;

                var dbFieldType = CommentType.Field.GetName();
                var dbQuestionType = CommentType.Question.GetName();

                SyncComments(
                    db,
                    bank,
                    db.QComments
                        .Where(x => x.AssessmentBankIdentifier == bank.Identifier
                                && (x.ContainerSubtype == dbFieldType && x.AssessmentFieldIdentifier == e.Field
                                 || x.ContainerSubtype == dbQuestionType && x.AssessmentQuestionIdentifier == e.Question))
                        .ToArray(),
                    bank.Comments
                        .Where(x => x.Type == CommentType.Field && x.Subject == e.Field
                                 || x.Type == CommentType.Question && x.Subject == e.Question)
                        .ToArray()
                );
            });
        }

        public void Update(FieldsDeleted e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var allIds = db.BankQuestions
                    .Where(x => x.ParentQuestionIdentifier == e.Question)
                    .Select(x => x.QuestionIdentifier)
                    .ToList();

                allIds.Add(e.Question);

                var questionGradeItems = db.BankQuestionGradeItems
                    .Where(x => x.FormIdentifier == e.Form && allIds.Contains(x.QuestionIdentifier))
                    .ToList();

                if (questionGradeItems.Count != 0)
                    db.BankQuestionGradeItems.RemoveRange(questionGradeItems);

                var form = bank.FindForm(e.Form);
                var summary = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                UpdateFormCounts(form, summary);

                var dbFieldType = CommentType.Field.GetName();
                var dbQuestionType = CommentType.Question.GetName();

                SyncComments(
                    db,
                    bank,
                    db.QComments
                        .Where(x => x.AssessmentBankIdentifier == bank.Identifier
                                && (x.ContainerSubtype == dbFieldType || x.ContainerSubtype == dbQuestionType))
                        .ToArray(),
                    bank.Comments
                        .Where(x => x.Type == CommentType.Field || x.Type == CommentType.Question)
                        .ToArray()
                );
            });
        }

        public void Update(FieldsReordered e) => UpdateCursor(e, null);

        public void Update(FieldsSwapped e) => UpdateCursor(e, null);

        public void Update(FormAdded e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Identifier);
                var query = new QBankForm();

                BindFormQuery(query, form);

                db.BankForms.Add(query);

                var specSummary = db.BankSpecifications.Single(x => x.SpecIdentifier == e.Specification);
                specSummary.SpecFormCount = form.Specification.EnumerateAllForms().Count();
            });
        }

        public void Update(FormAddendumChanged e) => UpdateCursor(e, null);

        public void Update(FormAnalyzed e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = db.BankForms.Where(x => x.FormIdentifier == e.Identifier).FirstOrDefault();
                if (form == null)
                    return;

                var summary = _attemptSummary.GetFormSummary(form.FormIdentifier);

                form.AttemptStartedCount = summary.AttemptStartedCount;
                form.AttemptSubmittedCount = summary.AttemptSubmittedCount;
                form.AttemptGradedCount = summary.AttemptGradedCount;
                form.AttemptPassedCount = summary.AttemptPassedCount;
            });
        }

        public void Update(FormMessageConnected e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
            });
        }

        public void Update(FormArchived e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
                UpdateQuestions(db, form.GetQuestions());
            });
        }

        public void Update(FormAssetChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
            });
        }

        public void Update(FormClassificationChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
            });
        }

        public void Update(FormCodeChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
            });
        }

        public void Update(FormContentChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
            });
        }

        public void Update(FormGradebookChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                if (qForm.GradebookIdentifier != form.Gradebook)
                    ClearFormGradeItems(db, qForm.FormIdentifier);

                BindFormQuery(qForm, form);
            });
        }

        private static void ClearFormGradeItems(InternalDbContext db, Guid formId)
        {
            var list = db.BankQuestionGradeItems
                .Where(x => x.FormIdentifier == formId)
                .ToList();

            db.BankQuestionGradeItems.RemoveRange(list);
        }

        public void Update(FormInvigilationChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
            });
        }

        public void Update(FormLanguagesModified e)
        {
            UpdateCursor(e, null);
        }

        public void Update(FormNameChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
            });
        }

        public void Update(FormPublished e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var query = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(query, form);
                UpdateQuestions(db, form.GetQuestions());
            });
        }

        public void Update(FormDeleted e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var specs = db.BankSpecifications.Where(x => x.BankIdentifier == bank.Identifier);
                foreach (var spec in specs)
                    spec.SpecFormCount = bank.FindSpecification(spec.SpecIdentifier).EnumerateAllForms().Count();

                var form = db.BankForms.Single(x => x.FormIdentifier == e.Form);
                db.BankForms.Remove(form);

                var commentType = CommentType.Form.ToString();
                db.QComments.RemoveRange(
                    db.QComments.Where(
                        x => x.AssessmentBankIdentifier == form.BankIdentifier
                          && x.ContainerSubtype == commentType
                          && x.AssessmentFormIdentifier == form.FormIdentifier));

                var questionGradeItems = db.BankQuestionGradeItems
                    .Where(x => x.FormIdentifier == e.Form)
                    .ToList();

                db.BankQuestionGradeItems.RemoveRange(questionGradeItems);
            });
        }

        public void Update(FormUnarchived e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
                UpdateQuestions(db, form.GetQuestions());
            });
        }

        public void Update(FormUnpublished e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
            });
        }

        public void Update(FormUpgraded e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var destination = bank.FindForm(e.Destination);
                var qForm = new QBankForm();

                BindFormQuery(qForm, destination);

                db.BankForms.Add(qForm);

                var specSummary = db.BankSpecifications.Single(x => x.SpecIdentifier == destination.Specification.Identifier);
                specSummary.SpecFormCount = bank.FindSpecification(destination.Specification.Identifier).EnumerateAllForms().Count();
            });
        }

        public void Update(FormVersionChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);

                BindFormQuery(qForm, form);
            });
        }

        public void Update(OptionAdded e) => UpdateCursor(e, (db, bank) =>
        {
            var existOptions = db.BankOptions
                .Where(x => x.QuestionIdentifier == e.Question)
                .Select(x => x.OptionKey)
                .ToHashSet();
            var question = bank.FindQuestion(e.Question);

            InsertOptions(db, bank, question.Options.Where(x => !existOptions.Contains(x.Number)));
        });

        public void Update(OptionChanged e) => UpdateCursor(e, (db, bank) =>
        {
            var question = db.BankQuestions.FirstOrDefault(x => x.QuestionIdentifier == e.Question)
                ?? throw new ArgumentException($"The question {e.Question} does not exist");

            SetLastChange(question, e);

            var model = bank.FindOption(e.Question, e.Number)
                ?? throw new ArgumentException($"The option Key = {e.Number}, Question = {e.Question} does not exist");

            var option = db.BankOptions.FirstOrDefault(x => x.QuestionIdentifier == e.Question && x.OptionKey == e.Number);
            if (option == null)
                db.BankOptions.Add(option = new QBankOption());

            BindOptionQuery(option, bank, model);
        });

        public void Update(OptionDeleted e) => UpdateCursor(e, (db, bank) =>
        {
            DeleteOptions(db, x => x.QuestionIdentifier == e.Question && x.OptionKey == e.Option);
        });

        public void Update(OptionsReordered e) => UpdateCursor(e, null);

        public void Update(QuestionAdded e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var query = new QBankQuestion();
                var question = bank.FindQuestion(e.Question);

                BindQuestionQuery(db, query, question);

                db.BankQuestions.Add(query);

                UpdateQuestions(
                    db,
                    bank.Sets.SelectMany(x => x.Questions)
                        .SelectMany(x => x.EnumerateAllVersions())
                        .Where(x => x.Identifier != e.Question));
                SyncQuestionSubCompetencies(db, question);

                UpdateFormCounts(db, bank);
            });
        }

        public void Update(QuestionClassificationChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var query = db.BankQuestions.Single(x => x.QuestionIdentifier == e.Question);

                query.QuestionCode = e.Classification.Code;
                query.QuestionTag = e.Classification.Tag;
                query.SetQuestionTags(e.Classification.Tags);
                query.QuestionTaxonomy = e.Classification.Taxonomy;
                query.QuestionDifficulty = e.Classification.Difficulty;
                query.QuestionLikeItemGroup = e.Classification.LikeItemGroup;
            });
        }

        public void Update(QuestionComposedVoiceChanged e) => UpdateCursor(e, null);

        public void Update(QuestionContentChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var query = db.BankQuestions.Single(x => x.QuestionIdentifier == e.Question);

                query.QuestionText = e.Content.Title?.Default;

                SetLastChange(query, e);
            });
        }

        public void Update(QuestionDuplicated2 e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var query = new QBankQuestion();
                var question = bank.FindQuestion(e.DestinationQuestion);

                BindQuestionQuery(db, query, question);

                db.BankQuestions.Add(query);

                UpdateQuestions(
                    db,
                    bank.Sets.SelectMany(x => x.Questions)
                        .SelectMany(x => x.EnumerateAllVersions())
                        .Where(x => x.Identifier != e.DestinationQuestion));
                SyncQuestionSubCompetencies(db, question);

                InsertOptions(db, bank, question.Options);

                UpdateFormCounts(db, bank);
            });
        }

        public void Update(QuestionFlagChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var query = db.BankQuestions.Single(x => x.QuestionIdentifier == e.Question);

                query.QuestionFlag = e.Flag.ToString();
            });
        }

        public void Update(QuestionGradeItemChanged2 e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var question = db.BankQuestions.Single(x => x.QuestionIdentifier == e.Question);

                var questionGradeItem = db.BankQuestionGradeItems
                    .Where(x => x.QuestionIdentifier == e.Question && x.FormIdentifier == e.Form)
                    .FirstOrDefault();

                if (e.GradeItem == null)
                {
                    if (questionGradeItem != null)
                        db.BankQuestionGradeItems.Remove(questionGradeItem);
                }
                else if (questionGradeItem == null)
                {
                    db.BankQuestionGradeItems.Add(new QBankQuestionGradeItem
                    {
                        QuestionIdentifier = e.Question,
                        FormIdentifier = e.Form,
                        GradeItemIdentifier = e.GradeItem.Value,
                        OrganizationIdentifier = question.OrganizationIdentifier
                    });
                }

                SetLastChange(question, e);
            });
        }

        public void Update(QuestionLikertRowGradeItemChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var question = db.BankQuestions.Single(x => x.QuestionIdentifier == e.LikertRow);

                var questionGradeItem = db.BankQuestionGradeItems
                    .Where(x => x.QuestionIdentifier == e.LikertRow && x.FormIdentifier == e.Form)
                    .FirstOrDefault();

                if (e.GradeItem == null)
                {
                    if (questionGradeItem != null)
                        db.BankQuestionGradeItems.Remove(questionGradeItem);
                }
                else if (questionGradeItem == null)
                {
                    db.BankQuestionGradeItems.Add(new QBankQuestionGradeItem
                    {
                        QuestionIdentifier = e.LikertRow,
                        FormIdentifier = e.Form,
                        GradeItemIdentifier = e.GradeItem.Value,
                        OrganizationIdentifier = question.OrganizationIdentifier
                    });
                }

                SetLastChange(question, e);
            });
        }

        public void Update(QuestionHotspotImageChanged e) => UpdateCursor(e, null);

        public void Update(QuestionHotspotOptionAdded e) => UpdateCursor(e, null);

        public void Update(QuestionHotspotOptionChanged e) => UpdateCursor(e, null);

        public void Update(QuestionHotspotOptionDeleted e) => UpdateCursor(e, null);

        public void Update(QuestionHotspotOptionsReordered e) => UpdateCursor(e, null);

        public void Update(QuestionHotspotPinLimitChanged e) => UpdateCursor(e, null);

        public void Update(QuestionHotspotShowShapesChanged e) => UpdateCursor(e, null);

        public void Update(QuestionLayoutChanged e) => UpdateCursor(e, null);

        public void Update(QuestionLikertColumnAdded e) => UpdateCursor(e, null);

        public void Update(QuestionLikertColumnChanged e) => UpdateCursor(e, null);

        public void Update(QuestionLikertColumnDeleted e) => UpdateCursor(e, null);

        public void Update(QuestionLikertOptionsChanged e) => UpdateCursor(e, null);

        public void Update(QuestionLikertReordered e) => UpdateCursor(e, (db, bank) =>
        {
            if (e.RowsOrder.IsEmpty())
                return;

            var likert = bank.FindQuestion(e.QuestionIdentifier).Likert;
            var queries = db.BankQuestions.Where(x => x.ParentQuestionIdentifier == e.QuestionIdentifier).ToArray();

            foreach (var query in queries)
                query.BankSubIndex = likert.GetRow(query.QuestionIdentifier).Index;
        });

        public void Update(QuestionLikertRowAdded e) => UpdateCursor(e, (db, bank) =>
        {
            var query = new QBankQuestion();
            var row = bank.FindQuestion(e.QuestionIdentifier).Likert.GetRow(e.RowIdentifier);

            BindLikertRowQuery(query, row);
            SetLastChange(query, e);

            db.BankQuestions.Add(query);

            SyncQuestionSubCompetencies(db, row.Identifier, row.SubStandards);
        });

        public void Update(QuestionLikertRowChanged e) => UpdateCursor(e, (db, bank) =>
        {
            var query = db.BankQuestions.FirstOrDefault(x => x.QuestionIdentifier == e.RowIdentifier)
                ?? throw new ArgumentException($"The question is not found: {e.RowIdentifier}");

            var row = bank.FindQuestion(e.QuestionIdentifier).Likert.GetRow(e.RowIdentifier);

            BindLikertRowQuery(query, row);
            SetLastChange(query, e);
            SyncQuestionSubCompetencies(db, row.Identifier, row.SubStandards);
        });

        public void Update(QuestionLikertRowDeleted e) => UpdateCursor(e, (db, bank) =>
        {
            var deleteEntity = db.BankQuestions.Single(x => x.QuestionIdentifier == e.RowIdentifier);
            db.BankQuestions.Remove(deleteEntity);

            var subCompetencies = db.BankQuestionSubCompetencies.Where(x => x.QuestionIdentifier == e.RowIdentifier).ToArray();
            db.BankQuestionSubCompetencies.RemoveRange(subCompetencies);

            var questionGradeItems = db.BankQuestionGradeItems.Where(x => x.QuestionIdentifier == e.RowIdentifier).ToArray();
            db.BankQuestionGradeItems.RemoveRange(questionGradeItems);

            var otherRows = bank.FindQuestion(e.QuestionIdentifier).Likert.Rows.ToArray();
            var otherIds = otherRows.Select(x => x.Identifier).ToArray();
            var otherEntities = db.BankQuestions.Where(x => otherIds.Contains(x.QuestionIdentifier));

            foreach (var otherRow in otherRows)
            {
                var otherEntity = otherEntities.FirstOrDefault(x => x.QuestionIdentifier == otherRow.Identifier);
                if (otherEntity != null)
                    BindLikertRowQuery(otherEntity, otherRow);
            }
        });

        public void Update(QuestionMatchesChanged e) => UpdateCursor(e, null);

        public void Update(QuestionMoved e) => UpdateCursor(e, (db, bank) =>
        {
            var question = bank.FindQuestion(e.Question);

            UpdateQuestions(
                db,
                bank.Sets.SelectMany(x => x.Questions)
                    .SelectMany(x => x.EnumerateAllVersions()));
            SyncQuestionSubCompetencies(db, question);

            foreach (var oQuery in db.BankOptions.Where(x => x.QuestionIdentifier == question.Identifier).ToArray())
                oQuery.CompetencyIdentifier = null;

            UpdateFormCounts(db, bank);
        });

        public void Update(QuestionMovedIn e) => UpdateCursor(e, (db, bank) =>
        {
            {
                var question = bank.FindQuestion(e.Question.Identifier);
                var query = new QBankQuestion();

                BindQuestionQuery(db, query, question);

                db.BankQuestions.Add(query);

                UpdateQuestions(
                    db,
                    bank.Sets.SelectMany(x => x.Questions)
                        .SelectMany(x => x.EnumerateAllVersions())
                        .Where(x => x.Identifier != e.Question.Identifier));
                SyncQuestionSubCompetencies(db, question);

                InsertOptions(db, bank, question.Options);
            }

            if (e.Comments.IsNotEmpty())
            {
                var filter = e.Comments.Select(x => x.Identifier).ToArray();
                if (db.QComments.Any(x => filter.Contains(x.CommentIdentifier)))
                {
                    var invalidIds = new HashSet<Guid>();

                    foreach (var comment in e.Comments)
                    {
                        if (invalidIds.Contains(comment.Identifier))
                            continue;

                        if (db.QComments.Any(x => x.CommentIdentifier == comment.Identifier))
                            invalidIds.Add(comment.Identifier);
                    }

                    throw new ArgumentException(
                        $"{Shift.Common.Humanizer.ToQuantity(invalidIds.Count, "comment")} ('{string.Join("', '", invalidIds)}')" +
                        $" already exists in the bank ({bank.Identifier}). It can be revised but it cannot be reposted.");
                }

                foreach (var comment in e.Comments)
                {
                    var query = new QComment();
                    BindCommentQuery(query, bank, comment);
                    db.QComments.Add(query);
                }

                UpdateFormCounts(db, bank);
            }
        });

        public void Update(QuestionMovedOut e) => UpdateCursor(e, (db, bank) => OnQuestionRemoved(db, bank, e.Question));

        public void Update(QuestionOrderingOptionAdded e) => UpdateCursor(e, null);

        public void Update(QuestionOrderingSolutionAdded e) => UpdateCursor(e, null);

        public void Update(QuestionOrderingLabelChanged e) => UpdateCursor(e, null);

        public void Update(QuestionOrderingOptionChanged e) => UpdateCursor(e, null);

        public void Update(QuestionOrderingSolutionChanged e) => UpdateCursor(e, null);

        public void Update(QuestionOrderingOptionDeleted e) => UpdateCursor(e, null);

        public void Update(QuestionOrderingSolutionDeleted e) => UpdateCursor(e, null);

        public void Update(QuestionOrderingOptionsReordered e) => UpdateCursor(e, null);

        public void Update(QuestionOrderingSolutionsReordered e) => UpdateCursor(e, null);

        public void Update(QuestionOrderingSolutionOptionsReordered e) => UpdateCursor(e, null);

        public void Update(QuestionPublicationStatusChanged e) => UpdateCursor(e, (db, bank) =>
        {
            var question = bank.FindQuestion(e.Question);
            var query = db.BankQuestions.Single(x => x.QuestionIdentifier == e.Question);

            BindQuestionQuery(db, query, question);
        });

        public void Update(QuestionRandomizationChanged e) => UpdateCursor(e, null);

        public void Update(QuestionRubricConnected e) => UpdateCursor(e, (db, bank) =>
        {
            var question = bank.FindQuestion(e.Question);
            var query = db.BankQuestions.Single(x => x.QuestionIdentifier == e.Question);

            BindQuestionQuery(db, query, question);
        });

        public void Update(QuestionRubricDisconnected e) => UpdateCursor(e, (db, bank) =>
        {
            var question = bank.FindQuestion(e.Question);
            var query = db.BankQuestions.Single(x => x.QuestionIdentifier == e.Question);

            BindQuestionQuery(db, query, question);
        });

        public void Update(QuestionDeleted e) => UpdateCursor(e, (db, bank) => OnQuestionRemoved(db, bank, e.RemoveAllVersions ? (Guid?)null : e.Question));

        private void OnQuestionRemoved(InternalDbContext db, BankState bank, Guid? questionId)
        {
            UpdateFormCounts(db, bank);

            if (questionId.HasValue)
            {
                DeleteQuestion(db, questionId.Value);
            }
            else
            {
                SyncQuestions(
                    db, bank,
                    insert: false,
                    update: false,
                    delete: true);
            }

            var dbFieldType = CommentType.Field.GetName();
            var dbQuestionType = CommentType.Question.GetName();

            SyncComments(
                db,
                bank,
                db.QComments
                    .Where(x => x.AssessmentBankIdentifier == bank.Identifier
                            && (x.ContainerSubtype == dbFieldType || x.ContainerSubtype == dbQuestionType))
                    .ToArray(),
                bank.Comments
                    .Where(x => x.Type == CommentType.Field || x.Type == CommentType.Question)
                    .ToArray()
            );
        }

        public void Update(QuestionScoringChanged e) => UpdateCursor(e, (db, bank) =>
        {
            var query = db.BankQuestions.Single(x => x.QuestionIdentifier == e.Question);

            SetLastChange(query, e);
        });

        public void Update(QuestionSetChanged e) => UpdateCursor(e, (db, bank) =>
        {
            UpdateQuestions(
                db,
                true,
                bank.Sets.SelectMany(x => x.Questions)
                    .SelectMany(x => x.EnumerateAllVersions()));

            var question = bank.FindQuestion(e.Question);
            var questionIds = question.EnumerateAllVersions().Select(x => x.Identifier).ToArray();

            foreach (var oQuery in db.BankOptions.Where(x => questionIds.Contains(x.QuestionIdentifier)).ToArray())
                oQuery.CompetencyIdentifier = null;

            UpdateFormCounts(db, bank);
        });


        public void Update(QuestionStandardChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var query = db.BankQuestions.Single(x => x.QuestionIdentifier == e.Question);
                var question = bank.FindQuestion(e.Question);
                BindQuestionQuery(db, query, question);
                SyncQuestionSubCompetencies(db, question);

                foreach (var oQuery in db.BankOptions.Where(x => x.QuestionIdentifier == e.Question).ToArray())
                    oQuery.CompetencyIdentifier = null;
            });
        }

        public void Update(QuestionConditionChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var query = db.BankQuestions.Single(x => x.QuestionIdentifier == e.Question);

                query.QuestionCondition = e.Condition;
            });
        }

        public void Update(QuestionsReordered e) => UpdateCursor(e, (db, bank) =>
        {
            UpdateQuestions(
                db,
                bank.Sets.SelectMany(x => x.Questions)
                    .SelectMany(x => x.EnumerateAllVersions()));
        });

        public void Update(QuestionUpgraded e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var query = new QBankQuestion();
                var question = bank.FindQuestion(e.UpgradedQuestion);

                BindQuestionQuery(db, query, question);
                SyncQuestionSubCompetencies(db, question);

                db.BankQuestions.Add(query);

                UpdateQuestions(
                    db,
                    bank.Sets.SelectMany(x => x.Questions)
                        .SelectMany(x => x.EnumerateAllVersions())
                        .Where(x => x.Identifier != e.UpgradedQuestion));

                InsertOptions(db, bank, question.Options);
            });
        }

        public void Update(SectionAdded e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var summary = db.BankForms.Single(x => x.FormIdentifier == form.Identifier);

                UpdateFormCounts(form, summary);
            });
        }

        public void Update(SectionDeleted e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                SyncGradeItems(db, bank);

                UpdateFormCounts(db, bank);
            });
        }

        public void Update(SectionReconfigured e) => UpdateCursor(e, null);

        public void Update(SectionsReordered e) => UpdateCursor(e, null);

        public void Update(SectionContentChanged e) => UpdateCursor(e, null);

        public void Update(ThirdPartyAssessmentEnabled e)
            => UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);
                BindFormQuery(qForm, form);
            });

        public void Update(ThirdPartyAssessmentDisabled e)
            => UpdateCursor(e, (db, bank) =>
            {
                var form = bank.FindForm(e.Form);
                var qForm = db.BankForms.Single(x => x.FormIdentifier == e.Form);
                BindFormQuery(qForm, form);
            });

        public void Update(SetAdded e) => UpdateCursor(e, null);

        public void Update(SetImported e) => UpdateCursor(e, null);

        public void Update(SetRandomizationChanged e) => UpdateCursor(e, null);

        public void Update(SetDeleted e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                UpdateFormCounts(db, bank);

                var specs = db.BankSpecifications.Where(x => x.BankIdentifier == bank.Identifier);
                foreach (var spec in specs)
                    UpdateCounts(spec, bank);

                DeleteQuestions(db, x => x.SetIdentifier == e.Set);

                var dbSetType = CommentType.Set.GetName();
                var dbFieldType = CommentType.Field.GetName();
                var dbQuestionType = CommentType.Question.GetName();

                SyncComments(
                    db,
                    bank,
                    db.QComments
                        .Where(x => x.AssessmentBankIdentifier == bank.Identifier
                                && (x.ContainerSubtype == dbSetType || x.ContainerSubtype == dbFieldType || x.ContainerSubtype == dbQuestionType))
                        .ToArray(),
                    bank.Comments
                        .Where(x => x.Type == CommentType.Set || x.Type == CommentType.Field || x.Type == CommentType.Question)
                        .ToArray()
                );
            });
        }

        public void Update(SetRenamed e) => UpdateCursor(e, null);

        public void Update(SetsMerged e) => UpdateCursor(e, (db, bank) =>
        {
            var questions = bank.Sets
                .SelectMany(x => x.Questions)
                .SelectMany(x => x.EnumerateAllVersions())
                .ToArray();

            UpdateQuestions(db, true, questions);

            var questionIds = questions.Select(x => x.Identifier).ToArray();
            foreach (var oQuery in db.BankOptions.Where(x => questionIds.Contains(x.QuestionIdentifier)).ToArray())
                oQuery.CompetencyIdentifier = null;
        });

        public void Update(SetsReordered e) => UpdateCursor(e, null);

        public void Update(SetStandardChanged e) => UpdateCursor(e, (db, bank) =>
        {
            foreach (var entity in db.BankQuestions.Where(x => x.SetIdentifier == e.Set).ToArray())
            {
                entity.CompetencyIdentifier = null;

                var subCompetencies = db.BankQuestionSubCompetencies.Where(x => x.QuestionIdentifier == entity.QuestionIdentifier).ToArray();
                db.BankQuestionSubCompetencies.RemoveRange(subCompetencies);
            }

            foreach (var entity in db.BankOptions.Where(x => x.SetIdentifier == e.Set).ToArray())
                entity.CompetencyIdentifier = null;
        });

        public void Update(CriterionAdded e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var spec = db.BankSpecifications.Single(x => x.SpecIdentifier == e.Specification);

                UpdateCounts(spec, bank);
                UpdateFormCounts(db, bank);
            });
        }

        public void Update(CriterionFilterChanged e) => UpdateCursor(e, null);

        public void Update(CriterionFilterDeleted e) => UpdateCursor(e, null);

        public void Update(CriterionDeleted e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                SyncGradeItems(db, bank);

                UpdateFormCounts(db, bank);

                foreach (var spec in db.BankSpecifications.Where(x => x.BankIdentifier == bank.Identifier))
                    UpdateCounts(spec, bank);

                var commentType = CommentType.Criterion.ToString();
                db.QComments.RemoveRange(
                    db.QComments.Where(
                        x => x.AssessmentBankIdentifier == bank.Identifier
                          && x.ContainerSubtype == commentType
                          && x.ContainerIdentifier == e.Criterion));
            });
        }

        public void Update(SpecificationAdded e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var spec = new QBankSpecification
                {
                    BankIdentifier = e.AggregateIdentifier,
                    CalcDisclosure = e.Calculation.Disclosure.ToString(),
                    CalcPassingScore = e.Calculation.PassingScore,
                    SpecFormCount = 0,
                    SpecIdentifier = e.Specification,
                    SpecType = e.Type.ToString(),
                    SpecName = e.Name,
                    SpecConsequence = e.Consequence.ToString(),
                    SpecAsset = e.Asset,
                    SpecFormLimit = e.FormLimit,
                    SpecQuestionLimit = e.QuestionLimit,
                    OrganizationIdentifier = bank.Tenant
                };

                db.BankSpecifications.Add(spec);
            });
        }

        public void Update(SpecificationCalculationChanged e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var spec = db.BankSpecifications.Single(x => x.SpecIdentifier == e.Specification);
                spec.CalcDisclosure = e.Calculation.Disclosure.ToString();
                spec.CalcPassingScore = e.Calculation.PassingScore;

                var forms = db.BankForms.Where(x => x.SpecIdentifier == e.Specification);
                foreach (var form in forms)
                    form.FormPassingScore = spec.CalcPassingScore;
            });
        }

        public void Update(SpecificationContentChanged e) => UpdateCursor(e, null);

        public void Update(SpecificationTabTimeLimitChanged e) => UpdateCursor(e, null);

        public void Update(SpecificationReconfigured e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var spec = db.BankSpecifications.Single(x => x.SpecIdentifier == e.Specification);

                if (e.Consequence.HasValue)
                    spec.SpecConsequence = e.Consequence.Value.ToString();

                spec.SpecFormLimit = e.FormLimit;
                spec.SpecQuestionLimit = e.QuestionLimit;

                var forms = db.BankForms.Where(x => x.SpecIdentifier == e.Specification);
                foreach (var form in forms)
                    form.SpecQuestionLimit = e.QuestionLimit;
            });
        }

        public void Update(SpecificationDeleted e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var spec = db.BankSpecifications.Single(x => x.SpecIdentifier == e.Specification);
                db.BankSpecifications.Remove(spec);

                var forms = db.BankForms.Where(x => x.SpecIdentifier == spec.SpecIdentifier).ToArray();
                db.BankForms.RemoveRange(forms);

                var formIds = forms.Select(x => x.FormIdentifier).ToList();

                var questionGradeItems = db.BankQuestionGradeItems
                    .Where(x => formIds.Contains(x.FormIdentifier))
                    .ToList();

                db.BankQuestionGradeItems.RemoveRange(questionGradeItems);

                var dbBankType = CommentType.Bank.GetName();

                SyncComments(
                    db,
                    bank,
                    db.QComments
                        .Where(x => x.AssessmentBankIdentifier == bank.Identifier && x.ContainerSubtype != dbBankType)
                        .ToArray(),
                    bank.Comments
                        .Where(x => x.Type != CommentType.Bank)
                        .ToArray()
                );
            });
        }

        public void Update(SpecificationRenamed e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var spec = db.BankSpecifications.Single(x => x.SpecIdentifier == e.Specification);
                spec.SpecName = e.Name;
            });
        }

        public void Update(SpecificationRetyped e)
        {
            UpdateCursor(e, (db, bank) =>
            {
                var spec = db.BankSpecifications.Single(x => x.SpecIdentifier == e.Specification);
                spec.SpecType = e.Type.ToString();
            });
        }

        private void UpdateCounts(QBankSpecification spec, BankState bank)
        {
            var calc = new SpecCalculator(bank);
            spec.CriterionAllCount = calc.CountCriteria(spec.SpecIdentifier);
            spec.CriterionTagCount = calc.CountTags(spec.SpecIdentifier);
            spec.CriterionPivotCount = calc.CountPivots(spec.SpecIdentifier);
        }

        private void SetLastChange(QBankQuestion question, IChange e)
        {
            // Only changes made to the Question Stem and Question Options/Answers should affect the LastChange value

            question.LastChangeTime = e.ChangeTime;
            question.LastChangeType = e.GetType().Name;
            question.LastChangeUser = UserSearch.GetFullName(e.OriginUser);
        }

        #endregion

        #region Methods (other)

        public void SyncFormCounts(BankState bank)
        {
            using (var db = CreateContext())
            {
                UpdateFormCounts(db, bank);

                db.SaveChanges();
            }
        }


        private void SyncQuestionAttachments(InternalDbContext db, Question question)
        {
            var attachments = db.BankQuestionAttachments
                .Where(x => x.QuestionIdentifier == question.Identifier)
                .ToList();

            foreach (var uploadId in question.AttachmentIdentifiers)
            {
                if (attachments.Any(x => x.UploadIdentifier == uploadId))
                    continue;

                db.BankQuestionAttachments.Add(new QBankQuestionAttachment
                {
                    QuestionIdentifier = question.Identifier,
                    UploadIdentifier = uploadId
                });
            }

            foreach (var attachment in attachments)
            {
                if (!question.AttachmentIdentifiers.Contains(attachment.UploadIdentifier))
                    db.BankQuestionAttachments.Remove(attachment);
            }
        }

        public void SyncComments(BankState bank, Expression<Func<QComment, bool>> entityFilter, Func<Comment, bool> modelFilter)
        {
            var models = bank.Comments.Where(modelFilter).ToArray();

            using (var db = CreateContext())
            {
                var ids = models.Select(x => x.Identifier).ToList();
                var orphanComments = db.QComments
                    .Where(x => ids.Contains(x.CommentIdentifier) && x.AssessmentBankIdentifier == null)
                    .ToList();

                db.QComments.RemoveRange(orphanComments);
                db.SaveChanges();

                var entities = db.QComments
                    .Where(x => x.AssessmentBankIdentifier == bank.Identifier)
                    .Where(entityFilter).ToArray();

                SyncComments(db, bank, entities, models);

                db.SaveChanges();
            }
        }

        public void SyncLikertRows(BankState bank)
        {
            var models = bank.Sets
                .SelectMany(x => x.Questions)
                .SelectMany(x => x.EnumerateAllVersions())
                .ToArray();

            using (var db = CreateContext())
            {
                var entities = db.BankQuestions
                    .Where(x => x.BankIdentifier == bank.Identifier)
                    .ToArray();

                foreach (var model in models)
                    SyncLikertRows(db, entities, model);

                db.SaveChanges();
            }
        }

        private static void SyncLikertRows(InternalDbContext db, QBankQuestion[] entities, Question question)
        {
            foreach (var row in question.Likert.Rows)
            {
                var entity = entities.FirstOrDefault(x => x.QuestionIdentifier == row.Identifier);
                if (entity != null)
                    continue;

                db.BankQuestions.Add(entity = new QBankQuestion());

                BindLikertRowQuery(entity, row);
                SyncQuestionSubCompetencies(db, row.Identifier, row.SubStandards);
            }
        }

        private static void SyncComments(InternalDbContext db, BankState bank, QComment[] entities, Comment[] models)
        {
            foreach (var entity in entities)
            {
                if (!models.Any(x => x.Identifier == entity.CommentIdentifier))
                    db.QComments.Remove(entity);
            }

            var ids = models.Select(x => x.Identifier).ToList();
            var orphanComments = db.QComments
                .Where(x => ids.Contains(x.CommentIdentifier) && x.AssessmentBankIdentifier == null)
                .ToList();

            db.QComments.RemoveRange(orphanComments);
            db.SaveChanges();

            foreach (var model in models)
            {
                var entity = entities.FirstOrDefault(x => x.CommentIdentifier == model.Identifier);
                if (entity == null)
                    db.QComments.Add(entity = new QComment());

                BindCommentQuery(entity, bank, model);
            }
        }

        private void UpdateQuestions(InternalDbContext db, IEnumerable<Question> data) =>
            UpdateQuestions(db, false, data);

        private void UpdateQuestions(InternalDbContext db, bool syncSubCompetencies, IEnumerable<Question> data)
        {
            var questions = data.ToDictionary(x => x.Identifier);
            foreach (var qQuestion in db.BankQuestions.Where(x => questions.Keys.Contains(x.QuestionIdentifier)).ToArray())
            {
                var mQuestion = questions[qQuestion.QuestionIdentifier];

                BindQuestionQuery(db, qQuestion, mQuestion);

                if (syncSubCompetencies)
                    SyncQuestionSubCompetencies(db, mQuestion);
            }
        }

        private static void SyncQuestions(InternalDbContext db, BankState bank, bool insert, bool update, bool delete)
        {
            var entities = db.BankQuestions.Where(x => x.BankIdentifier == bank.Identifier).ToArray();
            var models = bank.Sets.SelectMany(x => x.Questions).SelectMany(x => x.EnumerateAllVersions()).ToArray();

            if (delete)
            {
                var ids = GetQuestionIds(bank);
                var remove = entities
                    .Where(entity => !ids.Contains(entity.QuestionIdentifier))
                    .ToArray();

                DeleteQuestions(db, remove);
            }

            if (insert || update)
                SyncRootQuestions(db, bank, insert, update);
        }

        private static void SyncRootQuestions(InternalDbContext db, BankState bank, bool insert, bool update)
        {
            var entities = db.BankQuestions.Where(x => x.BankIdentifier == bank.Identifier).ToArray();
            var models = bank.Sets.SelectMany(x => x.Questions).SelectMany(x => x.EnumerateAllVersions()).ToArray();

            foreach (var model in models)
            {
                var entity = entities.FirstOrDefault(x => x.QuestionIdentifier == model.Identifier);
                var exists = entity != null;

                if (exists && !update || !exists && !insert)
                    continue;

                if (!exists)
                {
                    db.BankQuestions.Add(entity = new QBankQuestion());

                    InsertOptions(db, bank, model.Options);
                }

                BindQuestionQuery(db, entity, model);
                SyncQuestionSubCompetencies(db, model);
                SyncLikertRows(db, entities, model);
            }
        }

        private static List<Guid> GetQuestionIds(BankState bank)
        {
            var questions = bank.Sets
                .SelectMany(x => x.Questions)
                .SelectMany(x => x.EnumerateAllVersions())
                .ToList();

            var ids = questions.Select(x => x.Identifier).ToList();

            foreach (var question in questions)
                ids.AddRange(question.Likert.Rows.Select(x => x.Identifier));

            return ids;
        }

        private static void SyncQuestionSubCompetencies(InternalDbContext db, Question question) =>
            SyncQuestionSubCompetencies(db, question.Identifier, question.SubStandards);

        private static void SyncQuestionSubCompetencies(InternalDbContext db, Guid question, IEnumerable<Guid> subCompetencies)
        {
            var entities = db.BankQuestionSubCompetencies.Where(x => x.QuestionIdentifier == question).ToArray();
            var values = subCompetencies.EmptyIfNull();

            var remove = entities
                .Where(entity => !values.Contains(entity.SubCompetencyIdentifier))
                .ToArray();

            db.BankQuestionSubCompetencies.RemoveRange(remove);

            foreach (var id in values)
            {
                if (entities.Any(x => x.SubCompetencyIdentifier == id))
                    continue;

                db.BankQuestionSubCompetencies.Add(new QBankQuestionSubCompetency
                {
                    QuestionIdentifier = question,
                    SubCompetencyIdentifier = id
                });
            }
        }

        private static void SyncGradeItems(InternalDbContext db, BankState bank)
        {
            var aggregateGradeItems = GetBankGradeItems(bank);

            var databaseGradeItems = db.BankQuestionGradeItems
                .Where(x => x.Form.BankIdentifier == bank.Identifier)
                .ToList();

            var deletedQuestions = databaseGradeItems
                .Where(x => !aggregateGradeItems.Contains((x.QuestionIdentifier, x.FormIdentifier)))
                .ToList();

            db.BankQuestionGradeItems.RemoveRange(deletedQuestions);
        }

        private static HashSet<(Guid Question, Guid Form)> GetBankGradeItems(BankState bank)
        {
            var questions = bank.GetAllQuestions();

            return questions
                .SelectMany(q => q.GradeItems.Select(g => (q.Identifier, g.Key)))
                .Union(
                    questions
                        .Where(x => x.Likert != null)
                        .SelectMany(q => q.Likert.Rows.SelectMany(r => r.GradeItems.Select(g => (r.Identifier, g.Key))))
                )
                .ToHashSet();
        }

        #endregion

        #region Methods (binding)

        private static void BindBankQuery(QBank query, BankState bank, int size)
        {
            query.BankIdentifier = bank.Identifier;
            query.AssetNumber = bank.Asset;
            query.BankSize = size;
            query.BankLevel = bank.Level.ToString();
            query.BankName = bank.Name.IfNullOrEmpty("Unnamed");

            query.BankTitle = bank.Content.Title?.Default;
            if (query.BankTitle != null && query.BankTitle.Length > 200)
                query.BankTitle = query.BankTitle.Substring(0, 200);

            query.BankStatus = bank.Status;
            query.BankType = bank.Type.ToString();
            query.BankEdition = bank.Edition?.ToString();

            query.IsActive = bank.IsActive;

            query.OrganizationIdentifier = bank.Tenant;
            query.FrameworkIdentifier = bank.Standard;

            query.SetCount = bank.Sets.Count;
            query.QuestionCount = 0;
            query.OptionCount = 0;

            foreach (var set in bank.Sets)
            {
                query.QuestionCount += set.Questions.Count;

                foreach (var question in set.Questions)
                    query.OptionCount += question.Options.Count;
            }

            query.CommentCount = bank.Comments.Count;
            query.AttachmentCount = bank.Attachments.Count;

            query.SpecCount = bank.Specifications.Count;
            query.FormCount = bank.Specifications.Sum(spec => spec.EnumerateAllForms().Count());
        }

        private static void BindFormQuery(QBankForm query, Form form) =>
            BindFormQuery(query, form.Specification.Bank, form.Specification, form);

        private static void BindFormQuery(QBankForm query, BankState bank, Specification spec, Form form)
        {
            query.BankIdentifier = bank.Identifier;
            query.BankLevelType = bank.Level?.Type;

            query.FormIdentifier = form.Identifier;
            query.FormAsset = form.Asset;
            query.FormAssetVersion = form.AssetVersion;
            query.FormAttemptLimit = form.Invigilation.AttemptLimit;
            query.FormClassificationInstrument = form.Classification.Instrument;
            query.FormCode = form.Code;
            query.FormHook = form.Hook;
            query.FormSource = form.Source;
            query.FormOrigin = form.Origin;
            query.FormName = form.Name.IfNullOrEmpty("None");
            query.FormPassingScore = spec.Calculation.PassingScore;
            query.FormPublicationStatus = form.Publication.Status.ToString();
            query.FormFirstPublished = form.Publication.FirstPublished;
            query.FormTimeLimit = form.Invigilation.TimeLimit;
            query.FormTitle = (form.Content.Title?.Default).IfNullOrEmpty("None");
            query.FormSummary = form.Content.Summary?.Default;
            query.FormIntroduction = form.Content.Introduction?.Default;
            query.FormMaterialsForParticipation = form.Content.MaterialsForParticipation?.Default;
            query.FormMaterialsForDistribution = form.Content.MaterialsForDistribution?.Default;
            query.FormInstructionsForOnline = form.Content.InstructionsForOnline?.Default;
            query.FormInstructionsForPaper = form.Content.InstructionsForPaper?.Default;
            query.FormHasDiagrams = form.HasDiagrams;
            query.FormHasReferenceMaterials = form.HasReferenceMaterials == ReferenceMaterialType.None
                ? null
                : form.HasReferenceMaterials.GetDescription();
            query.FormThirdPartyAssessmentIsEnabled = form.ThirdPartyAssessmentIsEnabled;
            query.FormType = bank.Level.Type;

            query.SpecIdentifier = spec.Identifier;
            query.SpecQuestionLimit = spec.QuestionLimit;

            query.OrganizationIdentifier = bank.Tenant;

            UpdateFormCounts(form, query);

            query.GradebookIdentifier = form.Gradebook;

            query.WhenAttemptStartedNotifyAdminMessageIdentifier = form.WhenAttemptStartedNotifyAdminMessageIdentifier;
            query.WhenAttemptCompletedNotifyAdminMessageIdentifier = form.WhenAttemptCompletedNotifyAdminMessageIdentifier;
        }

        private static void BindQuestionQuery(InternalDbContext db, QBankQuestion query, Question question) =>
            BindQuestionQuery(db, query, question.Set.Bank, question.Set.Identifier, question);

        private static void BindQuestionQuery(InternalDbContext db, QBankQuestion query, BankState bank, Guid set, Question question)
        {
            query.OrganizationIdentifier = bank.Tenant;
            query.BankIdentifier = bank.Identifier;
            query.BankIndex = question.BankIndex;
            query.QuestionIdentifier = question.Identifier;
            query.QuestionAssetNumber = $"{question.Asset}.{question.AssetVersion}";
            query.QuestionText = question.Content.Title?.Default;
            query.QuestionCode = question.Classification.Code;
            query.QuestionReference = question.Classification.Reference;
            query.QuestionTag = question.Classification.Tag;
            query.SetQuestionTags(question.Classification.Tags);
            query.QuestionTaxonomy = question.Classification.Taxonomy;
            query.QuestionDifficulty = question.Classification.Difficulty;
            query.QuestionLikeItemGroup = question.Classification.LikeItemGroup;
            query.CompetencyIdentifier = question.Standard.NullIfEmpty();
            query.SetIdentifier = set;
            query.QuestionCondition = question.Condition;
            query.QuestionFirstPublished = question.FirstPublished;
            query.QuestionFlag = question.Flag.ToString();
            query.QuestionType = question.Type.ToString();
            query.RubricIdentifier = question.Rubric;
            query.QuestionPublicationStatus = question.PublicationStatus.GetName();

            BindQuestionQuerySource(db, query, question);
        }

        private static void BindQuestionQuerySource(InternalDbContext db, QBankQuestion query, Question question)
        {
            if (!question.Source.HasValue)
            {
                query.QuestionSourceIdentifier = null;
                query.QuestionSourceAssetNumber = null;

                return;
            }

            var sourceQuestion = question.Set.Bank.FindQuestion(question.Source.Value);
            if (sourceQuestion != null)
            {
                query.QuestionSourceIdentifier = sourceQuestion.Identifier;
                query.QuestionSourceAssetNumber = $"{sourceQuestion.Asset}.{sourceQuestion.AssetVersion}";

                return;
            }

            var qSourceQuestion = db.BankQuestions.AsNoTracking().FirstOrDefault(x => x.QuestionIdentifier == question.Source.Value);
            if (qSourceQuestion != null)
            {
                query.QuestionSourceIdentifier = qSourceQuestion.QuestionIdentifier;
                query.QuestionSourceAssetNumber = qSourceQuestion.QuestionAssetNumber;

                return;
            }

            query.QuestionSourceIdentifier = question.Source.Value;
            query.QuestionSourceAssetNumber = null;
        }

        private static void BindLikertRowQuery(QBankQuestion query, LikertRow row)
        {
            var q = row.Matrix.Question;
            var set = q.Set;
            var bank = set.Bank;

            query.OrganizationIdentifier = bank.Tenant;
            query.BankIdentifier = bank.Identifier;
            query.ParentQuestionIdentifier = q.Identifier;
            query.QuestionIdentifier = row.Identifier;
            query.BankIndex = q.BankIndex;
            query.BankSubIndex = row.Index;
            query.QuestionAssetNumber = $"{q.Asset}.{q.AssetVersion}";
            query.QuestionText = row.Content.Title?.Default;
            query.QuestionCode = q.Classification.Code;
            query.QuestionReference = q.Classification.Reference;
            query.QuestionTag = q.Classification.Tag;
            query.SetQuestionTags(q.Classification.Tags);
            query.QuestionTaxonomy = q.Classification.Taxonomy;
            query.QuestionDifficulty = q.Classification.Difficulty;
            query.QuestionLikeItemGroup = q.Classification.LikeItemGroup;
            query.CompetencyIdentifier = row.Standard.NullIfEmpty();
            query.SetIdentifier = set.Identifier;
            query.QuestionFirstPublished = q.FirstPublished;
            query.QuestionFlag = q.Flag.ToString();
        }

        private static void BindOptionQuery(QBankOption query, BankState bank, Option option)
        {
            query.OrganizationIdentifier = bank.Tenant;
            query.BankIdentifier = bank.Identifier;
            query.SetIdentifier = option.Question.Set.Identifier;
            query.QuestionIdentifier = option.Question.Identifier;
            query.OptionKey = option.Number;
            query.CompetencyIdentifier = option.Standard.NullIfEmpty();
            query.OptionText = option.Content.Title.Default;
        }

        public static void BindCommentQuery(QComment query, BankState bank, Comment comment)
        {
            query.AssessmentBankIdentifier = bank.Identifier;
            query.CommentIdentifier = comment.Identifier;
            query.OrganizationIdentifier = bank.Tenant;

            query.CommentFlag = comment.Flag.ToString();
            query.CommentPosted = comment.Posted;
            query.CommentRevised = comment.Revised;
            query.CommentText = comment.Text;
            query.CommentCategory = comment.Category;
            query.CommentIsHidden = comment.IsHidden;

            query.ContainerIdentifier = bank.Identifier;
            query.ContainerType = "Assessment Bank";
            query.ContainerDescription = comment.GetSubjectTitle(bank);
            query.ContainerSubtype = comment.Type.ToString();

            if (comment.Type == CommentType.Field)
                query.AssessmentFieldIdentifier = comment.Subject;
            else if (comment.Type == CommentType.Form)
                query.AssessmentFormIdentifier = comment.Subject;
            else if (comment.Type == CommentType.Question)
                query.AssessmentQuestionIdentifier = comment.Subject;
            else if (comment.Type == CommentType.Specification)
                query.AssessmentSpecificationIdentifier = comment.Subject;

            query.AuthorUserIdentifier = comment.Author;
            query.AuthorUserRole = comment.AuthorRole;
            query.TrainingProviderGroupIdentifier = comment.Instructor;
            query.RevisorUserIdentifier = comment.Revisor;

            query.EventStarted = comment.EventDate;
            query.EventFormat = comment.EventFormat;
        }

        private static void UpdateFormCounts(InternalDbContext db, BankState bank)
        {
            if (bank.Specifications == null)
                return;

            var forms = bank.Specifications.SelectMany(x => x.EnumerateAllForms());
            var queryForms = db.BankForms.Where(x => x.BankIdentifier == bank.Identifier).ToList();

            foreach (var form in forms)
            {
                var queryForm = queryForms.FirstOrDefault(x => x.FormIdentifier == form.Identifier);
                if (queryForm != null)
                    UpdateFormCounts(form, queryForm);
            }
        }

        private static void UpdateFormCounts(Form form, QBankForm queryForm)
        {
            int sectionCount, fieldCount;

            switch (form.Specification.Type)
            {
                case SpecificationType.Static:
                    sectionCount = form.Sections.Count;
                    fieldCount = form.Sections.Sum(x => x.Fields.Count);
                    break;
                case SpecificationType.Dynamic:
                    sectionCount = form.Specification.Criteria.Count > 0 ? form.Specification.Criteria.Count : 1;
                    fieldCount = form.Specification.Criteria.Count > 0
                        ? form.Specification.Criteria.SelectMany(x => x.Sets.SelectMany(y => y.Questions)).Count()
                        : form.Specification.Bank.Sets.SelectMany(x => x.Questions).Count();
                    break;
                default:
                    throw new ArgumentException($"Unknown specification type: {form.Specification.Type}");
            }

            if (sectionCount != queryForm.SectionCount || fieldCount != queryForm.FieldCount)
            {
                queryForm.SectionCount = sectionCount;
                queryForm.FieldCount = fieldCount;
            }
        }

        #endregion
    }
}