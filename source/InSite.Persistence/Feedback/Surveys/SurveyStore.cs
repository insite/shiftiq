using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contents.Read;
using InSite.Application.Surveys.Read;
using InSite.Domain.Surveys.Forms;
using InSite.Domain.Surveys.Sessions;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class SurveyStore : ISurveyStore
    {
        private readonly TContentStore ContentStore;

        public const string CommentContainerType = "Survey Form";

        public SurveyStore()
        {
            ContentStore = new TContentStore();
        }

        internal InternalDbContext CreateContext() => new InternalDbContext(true) { EnablePrepareToSaveChanges = false };

        #region Surveys

        public void InsertSurvey(SurveyFormCreated e)
        {
            using (var db = CreateContext())
            {
                var form = new QSurveyForm
                {
                    SurveyFormIdentifier = e.AggregateIdentifier,
                    OrganizationIdentifier = e.OriginOrganization,
                    SurveyFormLanguage = e.Language,

                    AssetNumber = e.Asset,
                    SurveyFormName = e.Name,
                    SurveyFormStatus = e.Status.GetName(),

                    UserFeedback = UserFeedbackType.Disabled.GetName()
                };

                SetLastChange(form, e);

                db.QSurveyForms.Add(form);
                db.SaveChanges();
            }
        }

        public void DeleteSurvey(SurveyFormDeleted e)
        {
            var changes = new ChangeInfo();

            using (var db = CreateContext())
            {
                var entity = db.QSurveyForms.FirstOrDefault(x => x.SurveyFormIdentifier == e.AggregateIdentifier);
                if (entity != null)
                    db.QSurveyForms.Remove(entity);

                var questions = db.QSurveyQuestions.Where(x => x.SurveyFormIdentifier == e.AggregateIdentifier).ToArray();
                foreach (var question in questions)
                    Remove(db, changes, question);

                var mailouts = db.Mailouts.Where(x => x.SurveyIdentifier == e.AggregateIdentifier).ToArray();
                foreach (var mailout in mailouts)
                    mailout.SurveyIdentifier = null;

                var messages = db.Messages.Where(x => x.SurveyFormIdentifier == e.AggregateIdentifier).ToArray();
                foreach (var message in messages)
                    message.SurveyFormIdentifier = null;

                var comments = db.QComments
                    .Where(x => x.ContainerIdentifier == e.AggregateIdentifier
                             && x.ContainerType == CommentContainerType)
                    .ToArray();
                db.QComments.RemoveRange(comments);

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyBranchAdded e)
        {
            var isAdded = false;

            using (var db = CreateContext())
            {
                var item = db.QSurveyOptionItems
                    .FirstOrDefault(x => x.SurveyOptionItemIdentifier == e.FromItem);

                if (item != null)
                {
                    isAdded = !item.BranchToQuestionIdentifier.HasValue;

                    item.BranchToQuestionIdentifier = e.ToQuestion;

                    db.SaveChanges();
                }
            }

            UpdateSurvey(e, survey =>
            {
                if (isAdded)
                    survey.BranchCount += 1;
            });
        }

        public void UpdateSurvey(SurveyBranchDeleted e)
        {
            var isRemoved = false;

            using (var db = CreateContext())
            {
                var item = db.QSurveyOptionItems
                    .FirstOrDefault(x => x.SurveyOptionItemIdentifier == e.Item);

                isRemoved = item != null;

                if (isRemoved)
                {
                    item.BranchToQuestionIdentifier = null;

                    db.SaveChanges();
                }
            }

            UpdateSurvey(e, survey =>
            {
                if (isRemoved)
                    survey.BranchCount -= 1;
            });
        }

        public void UpdateSurvey(SurveyCommentDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = new QComment
                {
                    CommentIdentifier = e.Comment
                };

                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyCommentModified e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QComments.FirstOrDefault(
                    x => x.CommentIdentifier == e.Comment
                      && x.ContainerIdentifier == e.AggregateIdentifier
                      && x.ContainerType == CommentContainerType);
                if (entity == null)
                    return;

                entity.CommentText = e.Text;
                entity.CommentPosted = e.ChangeTime;
                entity.CommentFlag = e.Flag?.GetName();
                entity.CommentResolved = e.Resolved;
                entity.TimestampModified = e.ChangeTime;
                entity.TimestampModifiedBy = e.OriginUser;

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyCommentPosted e)
        {
            using (var db = CreateContext())
            {
                var entity = new QComment
                {
                    CommentIdentifier = e.Comment,
                    AuthorUserIdentifier = e.OriginUser,
                    OrganizationIdentifier = e.OriginOrganization,

                    ContainerIdentifier = e.AggregateIdentifier,
                    ContainerType = CommentContainerType,

                    CommentText = e.Text,
                    CommentPosted = e.ChangeTime,
                    CommentFlag = e.Flag?.GetName(),
                    CommentResolved = e.Resolved,

                    TimestampCreated = e.ChangeTime,
                    TimestampCreatedBy = e.OriginUser
                };

                db.QComments.Add(entity);
                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyConditionAdded e)
        {
            var addedCount = 0;

            using (var db = CreateContext())
            {
                foreach (var question in e.MaskedQuestions)
                {
                    var exists = db.QSurveyConditions
                        .Any(x => x.MaskingSurveyOptionItemIdentifier == e.Item
                               && x.MaskedSurveyQuestionIdentifier == question);

                    if (exists)
                        continue;

                    db.QSurveyConditions.Add(new QSurveyCondition
                    {
                        MaskingSurveyOptionItemIdentifier = e.Item,
                        MaskedSurveyQuestionIdentifier = question
                    });

                    addedCount++;
                }

                if (addedCount > 0)
                    db.SaveChanges();
            }

            UpdateSurvey(e, survey =>
            {
                survey.ConditionCount += addedCount;
            });
        }

        public void UpdateSurvey(SurveyConditionDeleted e)
        {
            var changes = new ChangeInfo();

            using (var db = CreateContext())
            {
                var conditions = db.QSurveyConditions
                    .Where(x => x.MaskingSurveyOptionItemIdentifier == e.MaskingItem
                             && e.MaskedQuestions.Contains(x.MaskedSurveyQuestionIdentifier))
                    .ToArray();

                foreach (var condition in conditions)
                    Remove(db, changes, condition);

                db.SaveChanges();
            }

            UpdateSurvey(e, survey =>
            {
                changes.Setup(survey);
            });
        }

        public void UpdateSurvey(SurveyFormAssetChanged e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.AssetNumber = e.Asset;
            });
        }

        public void UpdateSurvey(SurveyFormContentChanged e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.SurveyFormTitle = e.Content.Title.Text.Default.NullIfEmpty()?.MaxLength(256);
            });

            ContentStore.SaveContainer(e.OriginOrganization, ContentContainerType.SurveyForm, e.AggregateIdentifier, e.Content);
        }

        public void UpdateSurvey(SurveyFormLanguagesChanged e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.SurveyFormLanguage = e.Language;
                if (e.Translations.IsEmpty())
                    survey.SurveyFormLanguageTranslations = null;
                else
                    survey.SurveyFormLanguageTranslations = string.Join(",", e.Translations);
            });
        }

        public void UpdateSurvey(SurveyFormLocked e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.SurveyFormLocked = e.Locked;
            });
        }

        public void UpdateSurvey(SurveyFormMessageAdded e)
        {
            var message = e.Message;

            UpdateSurvey(e, survey =>
            {

            });

            using (var db = CreateContext())
            {
                var form = db.QSurveyForms.FirstOrDefault(x => x.SurveyFormIdentifier == e.AggregateIdentifier);
                if (form == null)
                    return;

                if (message.Type == SurveyMessageType.Invitation)
                    form.SurveyMessageInvitation = message.Identifier;
                else if (message.Type == SurveyMessageType.ResponseCompleted)
                    form.SurveyMessageResponseCompleted = message.Identifier;
                else if (message.Type == SurveyMessageType.ResponseConfirmed)
                    form.SurveyMessageResponseConfirmed = message.Identifier;
                else if (message.Type == SurveyMessageType.ResponseStarted)
                    form.SurveyMessageResponseStarted = message.Identifier;

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyFormMessagesChanged e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            using (var db = CreateContext())
            {
                var form = db.QSurveyForms.FirstOrDefault(x => x.SurveyFormIdentifier == e.AggregateIdentifier);
                if (form == null)
                    return;

                form.SurveyMessageInvitation = null;
                form.SurveyMessageResponseCompleted = null;
                form.SurveyMessageResponseConfirmed = null;
                form.SurveyMessageResponseStarted = null;

                foreach (var message in e.Messages)
                {
                    if (message.Type == SurveyMessageType.Invitation)
                        form.SurveyMessageInvitation = message.Identifier;
                    else if (message.Type == SurveyMessageType.ResponseCompleted)
                        form.SurveyMessageResponseCompleted = message.Identifier;
                    else if (message.Type == SurveyMessageType.ResponseConfirmed)
                        form.SurveyMessageResponseConfirmed = message.Identifier;
                    else if (message.Type == SurveyMessageType.ResponseStarted)
                        form.SurveyMessageResponseStarted = message.Identifier;
                }

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyFormRenamed e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.SurveyFormName = e.Name;
            });
        }

        public void UpdateSurvey(SurveyHookChanged e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.SurveyFormHook = e.Hook;
            });
        }

        public void UpdateSurvey(SurveyFormScheduleChanged e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.SurveyFormOpened = e.Opened;
                survey.SurveyFormClosed = e.Closed;
            });
        }

        public void UpdateSurvey(SurveyFormSettingsChanged e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.EnableUserConfidentiality = e.EnableUserConfidentiality;
                survey.UserFeedback = e.UserFeedback.GetName();
                survey.RequireUserIdentification = e.RequireUserIdentification;
                survey.RequireUserAuthentication = e.RequireUserAuthentication;
                survey.ResponseLimitPerUser = e.ResponseLimitPerUser;
                survey.SurveyFormDurationMinutes = e.DurationMinutes;
            });
        }

        public void UpdateSurvey(SurveyDisplaySummaryChartChanged e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.DisplaySummaryChart = e.DisplaySummaryChart;
            });
        }

        public void UpdateSurvey(SurveyFormStatusChanged e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.SurveyFormStatus = e.Status.ToString();
            });
        }

        public void UpdateSurvey(SurveyFormUnlocked e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.SurveyFormLocked = null;
            });
        }

        public void UpdateSurvey(SurveyOptionItemAdded e)
        {
            using (var db = CreateContext())
            {
                // First, ensure the list exists.

                var list = db.QSurveyOptionLists.FirstOrDefault(x => x.SurveyOptionListIdentifier == e.List);
                if (list == null)
                    db.QSurveyOptionLists.Add(list = new QSurveyOptionList
                    {
                        SurveyOptionListIdentifier = e.List,
                        SurveyOptionListSequence = 1
                    });

                // Next, add the item if it is not already added.

                var item = db.QSurveyOptionItems.FirstOrDefault(x => x.SurveyOptionItemIdentifier == e.Item);
                if (item == null)
                    db.QSurveyOptionItems.Add(item = new QSurveyOptionItem
                    {
                        SurveyOptionItemIdentifier = e.Item,
                        SurveyOptionListIdentifier = e.List,
                        SurveyOptionItemSequence = 1 + db.QSurveyOptionItems.Count(x => x.SurveyOptionListIdentifier == e.List)

                    });

                db.SaveChanges();
            }

            UpdateSurvey(e, survey =>
            {

            });
        }

        public void UpdateSurvey(SurveyOptionItemContentChanged e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            ContentStore.SaveContainer(e.OriginOrganization, ContentContainerType.SurveyOptionItem, e.Item, e.Content);
        }

        public void UpdateSurvey(SurveyOptionItemDeleted e)
        {
            var changes = new ChangeInfo();

            using (var db = CreateContext())
            {
                var item = db.QSurveyOptionItems
                    .FirstOrDefault(x => x.SurveyOptionItemIdentifier == e.Item);

                if (item != null)
                {
                    Remove(db, changes, item);

                    db.SaveChanges();
                }
            }

            UpdateSurvey(e, survey =>
            {
                changes.Setup(survey);
            });
        }

        public void UpdateSurvey(SurveyOptionItemSettingsChanged e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            using (var db = CreateContext())
            {
                var item = db.QSurveyOptionItems
                    .FirstOrDefault(x => x.SurveyOptionItemIdentifier == e.Item);

                if (item == null)
                    return;

                item.SurveyOptionItemCategory = e.Category.MaxLength(90);
                item.SurveyOptionItemPoints = e.Points;

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyOptionItemsReordered e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            using (var db = CreateContext())
            {
                var items = db.QSurveyOptionItems
                    .Where(x => e.Sequences.Keys.Contains(x.SurveyOptionItemIdentifier));
                foreach (var item in items)
                    item.SurveyOptionItemSequence = e.Sequences[item.SurveyOptionItemIdentifier];

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyOptionListAdded e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            using (var db = CreateContext())
            {
                if (!db.QSurveyOptionLists.Any(x => x.SurveyOptionListIdentifier == e.List))
                {
                    var nextSequence = 1 + db.QSurveyOptionLists.Count(x => x.SurveyQuestionIdentifier == e.Question);

                    db.QSurveyOptionLists.Add(new QSurveyOptionList
                    {
                        SurveyQuestionIdentifier = e.Question,
                        SurveyOptionListIdentifier = e.List,
                        SurveyOptionListSequence = nextSequence
                    });

                    db.SaveChanges();
                }
            }
        }

        public void UpdateSurvey(SurveyOptionListContentChanged e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            ContentStore.SaveContainer(e.OriginOrganization, ContentContainerType.SurveyOptionList, e.List, e.Content);
        }

        public void UpdateSurvey(SurveyOptionListDeleted e)
        {
            var changes = new ChangeInfo();

            using (var db = CreateContext())
            {
                var list = db.QSurveyOptionLists
                    .FirstOrDefault(x => x.SurveyOptionListIdentifier == e.List);

                if (list != null)
                {
                    Remove(db, changes, list);

                    db.SaveChanges();
                }
            }

            UpdateSurvey(e, survey =>
            {
                changes.Setup(survey);
            });
        }

        public void UpdateSurvey(SurveyOptionListsReordered e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            using (var db = CreateContext())
            {
                var lists = db.QSurveyOptionLists
                    .Where(x => e.Sequences.Keys.Contains(x.SurveyOptionListIdentifier));
                foreach (var list in lists)
                    list.SurveyOptionListSequence = e.Sequences[list.SurveyOptionListIdentifier];

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyQuestionAdded e)
        {
            var changes = new ChangeInfo();

            using (var db = CreateContext())
            {
                db.QSurveyQuestions.Add(new QSurveyQuestion
                {
                    SurveyFormIdentifier = e.AggregateIdentifier,
                    SurveyQuestionIdentifier = e.Question,
                    SurveyQuestionType = e.Type.ToString(),
                    SurveyQuestionSequence = 1 + db.QSurveyQuestions.Count(x => x.SurveyFormIdentifier == e.AggregateIdentifier),
                    SurveyQuestionCode = e.Code,
                    SurveyQuestionIndicator = e.Indicator,
                    SurveyQuestionSource = e.Source
                });

                changes.Questions++;

                if (e.Type == SurveyQuestionType.BreakPage)
                    changes.PageBreaks++;

                db.SaveChanges();
            }

            UpdateSurvey(e, survey =>
            {
                changes.Setup(survey);
            });
        }

        public void UpdateSurvey(SurveyQuestionAttributed e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            using (var db = CreateContext())
            {
                var question = db.QSurveyQuestions
                    .FirstOrDefault(x => x.SurveyQuestionIdentifier == e.Question);

                if (question == null)
                    return;

                question.SurveyQuestionAttribute = e.Attribute;

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyQuestionContentChanged e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            ContentStore.SaveContainer(e.OriginOrganization, ContentContainerType.SurveyQuestion, e.Question, e.Content);
        }

        public void UpdateSurvey(SurveyQuestionRecoded e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            using (var db = CreateContext())
            {
                var question = db.QSurveyQuestions
                    .FirstOrDefault(x => x.SurveyQuestionIdentifier == e.Question);

                if (question == null)
                    return;

                question.SurveyQuestionCode = e.Code;
                question.SurveyQuestionIndicator = e.Indicator;

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyQuestionDeleted e)
        {
            var changes = new ChangeInfo();

            using (var db = CreateContext())
            {
                var question = db.QSurveyQuestions
                    .FirstOrDefault(x => x.SurveyQuestionIdentifier == e.Question);

                if (question != null)
                {
                    Remove(db, changes, question);

                    db.SaveChanges();
                }
            }

            UpdateSurvey(e, survey =>
            {
                changes.Setup(survey);
            });
        }

        public void UpdateSurvey(SurveyQuestionSettingsChanged e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            using (var db = CreateContext())
            {
                var question = db.QSurveyQuestions
                    .FirstOrDefault(x => x.SurveyQuestionIdentifier == e.Question);

                if (question == null)
                    return;

                question.SurveyQuestionIsRequired = e.IsRequired;
                question.SurveyQuestionIsNested = e.IsNested;
                question.SurveyQuestionListEnableRandomization = e.ListEnableRandomization;
                question.SurveyQuestionListEnableOtherText = e.ListEnableOtherText;
                question.SurveyQuestionListEnableBranch = e.ListEnableBranch;
                question.SurveyQuestionListEnableGroupMembership = e.ListEnableGroupMembership;
                question.SurveyQuestionTextLineCount = e.TextLineCount;
                question.SurveyQuestionTextCharacterLimit = e.TextCharacterLimit;
                question.SurveyQuestionNumberEnableStatistics = e.NumberEnableStatistics;

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyQuestionsReordered e)
        {
            UpdateSurvey(e, survey =>
            {

            });

            using (var db = CreateContext())
            {
                var questions = db.QSurveyQuestions
                    .Where(x => e.Sequences.Keys.Contains(x.SurveyQuestionIdentifier));
                foreach (var question in questions)
                    question.SurveyQuestionSequence = e.Sequences[question.SurveyQuestionIdentifier];

                db.SaveChanges();
            }
        }

        public void UpdateSurvey(SurveyWorkflowConfigured e)
        {
            UpdateSurvey(e, survey =>
            {
                survey.HasWorkflowConfiguration = e.Configuration != null;
            });
        }

        #endregion

        #region Helpers (surveys)

        private void UpdateSurvey(IChange e, Action<QSurveyForm> change)
        {
            using (var db = CreateContext())
            {
                var form = db.QSurveyForms
                    .FirstOrDefault(x => x.SurveyFormIdentifier == e.AggregateIdentifier);

                if (form == null)
                    return;

                change(form);

                SetLastChange(form, e);

                db.SaveChanges();
            }
        }

        private void SetLastChange(QSurveyForm entity, IChange change)
        {
            entity.LastChangeTime = change.ChangeTime;
            entity.LastChangeType = change.GetType().Name;
            entity.LastChangeUser = change.OriginUser;
        }

        private class ChangeInfo
        {
            public int PageBreaks { get; set; }
            public int Questions { get; set; }
            public int Lists { get; set; }
            public int Items { get; set; }
            public int Branches { get; set; }
            public int Conditions { get; set; }

            internal void Setup(QSurveyForm survey)
            {
                survey.QuestionCount += Questions;
                survey.BranchCount += Branches;
                survey.ConditionCount += Conditions;

                if (survey.QuestionCount <= 0)
                {
                    survey.PageCount = 0;
                }
                else
                {
                    if (survey.PageCount == 0)
                        survey.PageCount = 1;

                    survey.PageCount += PageBreaks;
                }
            }
        }

        private static void Remove(InternalDbContext db, ChangeInfo changes, QSurveyQuestion question)
        {
            var conditions = db.QSurveyConditions.Where(x => x.MaskedSurveyQuestionIdentifier == question.SurveyQuestionIdentifier).ToArray();
            foreach (var condition in conditions)
                Remove(db, changes, condition);

            var branchItems = db.QSurveyOptionItems.Where(x => x.BranchToQuestionIdentifier == question.SurveyQuestionIdentifier).ToArray();
            foreach (var bi in branchItems)
            {
                bi.BranchToQuestionIdentifier = null;

                changes.Branches--;
            }

            var lists = db.QSurveyOptionLists.Where(x => x.SurveyQuestionIdentifier == question.SurveyQuestionIdentifier).ToArray();
            foreach (var list in lists)
                Remove(db, changes, list);

            if (question.SurveyQuestionType == SurveyQuestionType.BreakPage.ToString())
                changes.PageBreaks--;

            db.QSurveyQuestions.Remove(question);

            changes.Questions--;
        }

        private static void Remove(InternalDbContext db, ChangeInfo changes, QSurveyCondition condition)
        {
            db.QSurveyConditions.Remove(condition);

            changes.Conditions--;
        }

        private static void Remove(InternalDbContext db, ChangeInfo changes, QSurveyOptionList list)
        {
            var items = db.QSurveyOptionItems.Where(x => x.SurveyOptionListIdentifier == list.SurveyOptionListIdentifier).ToArray();
            foreach (var item in items)
                Remove(db, changes, item);

            db.QSurveyOptionLists.Remove(list);

            changes.Lists--;
        }

        private static void Remove(InternalDbContext db, ChangeInfo changes, QSurveyOptionItem item)
        {
            var conditions = db.QSurveyConditions.Where(x => x.MaskingSurveyOptionItemIdentifier == item.SurveyOptionItemIdentifier).ToArray();
            foreach (var condition in conditions)
                Remove(db, changes, condition);

            if (item.BranchToQuestionIdentifier.HasValue)
                changes.Branches--;

            db.QSurveyOptionItems.Remove(item);

            changes.Items--;
        }

        #endregion

        #region Responses

        public void InsertResponse(ResponseSessionCreated e)
        {
            using (var db = CreateContext())
            {
                var session = new QResponseSession
                {
                    ResponseSessionIdentifier = e.AggregateIdentifier,
                    OrganizationIdentifier = e.Tenant,
                    SurveyFormIdentifier = e.Form,

                    ResponseSessionStatus = ResponseSessionStatus.Created.ToString(),
                    ResponseSessionCreated = e.ChangeTime,
                    RespondentUserIdentifier = e.User
                };

                SetLastChange(session, e);

                db.QResponseSessions.Add(session);
                db.SaveChanges();
            }
        }

        private void UpdateResponse(IChange e, Action<QResponseSession> change)
        {
            using (var db = CreateContext())
            {
                var session = db.QResponseSessions
                    .FirstOrDefault(x => x.ResponseSessionIdentifier == e.AggregateIdentifier);

                if (session == null)
                    return;

                change(session);

                SetLastChange(session, e);

                db.SaveChanges();
            }
        }

        private void SetLastChange(QResponseSession entity, IChange change)
        {
            entity.LastChangeTime = change.ChangeTime;
            entity.LastChangeType = change.GetType().Name;
            entity.LastChangeUser = change.OriginUser;
        }

        public void UpdateResponse(ResponseAnswerAdded e)
        {
            UpdateResponse(e, response =>
            {

            });

            using (var db = CreateContext())
            {
                var answer = new QResponseAnswer
                {
                    ResponseSessionIdentifier = e.AggregateIdentifier,
                    SurveyQuestionIdentifier = e.Question
                };
                db.QResponseAnswers.Add(answer);
                db.SaveChanges();
            }
        }

        public void UpdateResponse(ResponseAnswerChanged e)
        {
            UpdateResponse(e, response =>
            {
                response.LastAnsweredQuestionIdentifier = e.Question;
            });

            using (var db = CreateContext())
            {
                var answer = db.QResponseAnswers
                    .FirstOrDefault(x => x.ResponseSessionIdentifier == e.AggregateIdentifier && x.SurveyQuestionIdentifier == e.Question);

                if (answer != null)
                {
                    answer.ResponseAnswerText = e.Answer;
                    db.SaveChanges();
                }
            }

            AnnotateRespondent(e.OriginOrganization, e.OriginUser, e.Question, e.Answer);
        }

        public void UpdateResponse(ResponseGroupChanged e)
        {
            UpdateResponse(e, response =>
            {
                response.GroupIdentifier = e.Group;
            });
        }

        public void UpdateResponse(ResponsePeriodChanged e)
        {
            UpdateResponse(e, response =>
            {
                response.PeriodIdentifier = e.Period;
            });
        }

        public void UpdateResponse(ResponseUserChanged e)
        {
            UpdateResponse(e, response =>
            {
                response.RespondentUserIdentifier = e.User;
            });
        }

        private void AnnotateRespondent(Guid organization, Guid user, Guid questionId, string answerText)
        {
            QSurveyQuestion question;

            using (var db = CreateContext())
            {
                question = db.QSurveyQuestions
                    .FirstOrDefault(x => x.SurveyQuestionIdentifier == questionId);
            }

            if (string.IsNullOrWhiteSpace(question?.SurveyQuestionAttribute))
                return;

            if (!string.IsNullOrWhiteSpace(answerText))
                TPersonFieldStore.Save(organization, user, question.SurveyQuestionAttribute, answerText);
            else if (!(question.SurveyQuestionType == "RadioList" && question.SurveyQuestionListEnableOtherText))
                TPersonFieldStore.Delete(organization, user, question.SurveyQuestionAttribute);
        }

        private void AnnotateRespondent(Guid organization, Guid user, Guid optionId, bool isSelected)
        {
            string attribute;
            TContent content;

            using (var db = CreateContext())
            {
                var option = db.QSurveyOptionItems
                    .Include(x => x.SurveyOptionList.SurveyQuestion)
                    .FirstOrDefault(x => x.SurveyOptionItemIdentifier == optionId);

                attribute = option?.SurveyOptionList?.SurveyQuestion?.SurveyQuestionAttribute;

                if (string.IsNullOrWhiteSpace(attribute))
                    return;

                content = isSelected
                    ? db.TContents.FirstOrDefault(x => x.ContainerIdentifier == optionId && x.ContentLanguage == "en" && x.ContentLabel == "Title")
                    : null;
            }

            if (isSelected)
            {
                if (content?.ContentText != null)
                    TPersonFieldStore.Save(organization, user, attribute, content.ContentText);
            }
            else
                TPersonFieldStore.Delete(organization, user, attribute);
        }

        public void UpdateResponse(ResponseOptionSelected e)
        {
            Guid? questionId = null;

            using (var db = CreateContext())
            {
                var option = db.QResponseOptions
                    .FirstOrDefault(x => x.ResponseSessionIdentifier == e.AggregateIdentifier && x.SurveyOptionIdentifier == e.Item);

                if (option != null)
                {
                    questionId = option.SurveyQuestionIdentifier;
                    option.ResponseOptionIsSelected = true;
                    db.SaveChanges();
                }
            }

            UpdateResponse(e, response =>
            {
                response.LastAnsweredQuestionIdentifier = questionId;
            });

            AnnotateRespondent(e.OriginOrganization, e.OriginUser, e.Item, true);
        }

        public void UpdateResponse(ResponseOptionUnselected e)
        {
            Guid? questionId = null;

            using (var db = CreateContext())
            {
                var option = db.QResponseOptions
                    .FirstOrDefault(x => x.ResponseSessionIdentifier == e.AggregateIdentifier && x.SurveyOptionIdentifier == e.Item);

                if (option != null)
                {
                    questionId = option.SurveyQuestionIdentifier;
                    option.ResponseOptionIsSelected = false;
                    db.SaveChanges();
                }
            }

            UpdateResponse(e, response =>
            {
                response.LastAnsweredQuestionIdentifier = questionId;
            });

            AnnotateRespondent(e.OriginOrganization, e.OriginUser, e.Item, false);
        }

        public void UpdateResponse(ResponseOptionsAdded e)
        {
            UpdateResponse(e, response =>
            {

            });

            using (var db = CreateContext())
            {
                for (var i = 0; i < e.Items.Length; i++)
                {
                    var item = e.Items[i];
                    var option = new QResponseOption
                    {
                        ResponseSessionIdentifier = e.AggregateIdentifier,
                        SurveyOptionIdentifier = item,
                        SurveyQuestionIdentifier = e.Question,
                        OptionSequence = i + 1,
                    };
                    db.QResponseOptions.Add(option);
                }
                db.SaveChanges();
            }
        }

        public void UpdateResponse(ResponseSessionCompleted e)
        {
            UpdateResponse(e, response =>
            {
                response.ResponseSessionCompleted = e.Completed;
                response.ResponseSessionStatus = ResponseSessionStatus.Completed.ToString();
                response.ResponseIsLocked = true;
            });

            AssignRespondentToGroups(e.OriginOrganization, e.AggregateIdentifier);
        }

        private void AssignRespondentToGroups(Guid organizationId, Guid responseId)
        {
            using (var db = CreateContext())
            {
                var response = db.QResponseSessions.FirstOrDefault(x => x.ResponseSessionIdentifier == responseId);
                if (response == null)
                    return;

                var respondentCount = db.QResponseSessions
                    .Where(x => x.SurveyFormIdentifier == response.SurveyFormIdentifier)
                    .Select(x => x.RespondentUserIdentifier)
                    .Distinct()
                    .Count(); ;

                var questions = db.QSurveyQuestions.Where(x => x.SurveyFormIdentifier == response.SurveyFormIdentifier && x.SurveyQuestionListEnableGroupMembership).ToList();
                var answers = db.QResponseOptions
                    .Where(x => x.ResponseSessionIdentifier == responseId && x.ResponseOptionIsSelected && x.SurveyOptionItem.SurveyOptionList.SurveyQuestion.SurveyQuestionListEnableGroupMembership)
                    .Select(x => new { x.ResponseSessionIdentifier, x.SurveyOptionItem.SurveyOptionList.SurveyQuestionIdentifier, x.SurveyOptionIdentifier, x.SurveyOptionItem.SurveyOptionItemCategory })
                    .ToList();

                foreach (var question in questions)
                {
                    var questionAnswers = answers
                        .Where(x => x.SurveyQuestionIdentifier == question.SurveyQuestionIdentifier)
                        .ToList();

                    foreach (var answer in questionAnswers)
                    {
                        var groupName = SelectSequentialGroupName(answer.SurveyOptionItemCategory, respondentCount);
                        if (string.IsNullOrEmpty(groupName))
                            continue;

                        var group = db.QGroups.FirstOrDefault(x => x.OrganizationIdentifier == organizationId && x.GroupName == groupName && !x.OnlyOperatorCanAddUser);
                        if (group == null)
                            continue;

                        MembershipStore.Save(MembershipFactory.Create(response.RespondentUserIdentifier, group.GroupIdentifier, organizationId, "Survey Respondent", null));
                    }
                }
            }

            string SelectSequentialGroupName(string category, int respondentCount)
            {
                try
                {
                    if (string.IsNullOrEmpty(category))
                        return null;

                    if (!category.Contains(";"))
                        return category;

                    var groupNames = StringHelper.Split(category, ';');
                    var index = respondentCount % groupNames.Length;
                    var groupName = groupNames[index];
                    return groupName;
                }
                catch { }
                return null;
            }
        }

        public void UpdateResponse(ResponseSessionConfirmed e)
        {
            UpdateResponse(e, response =>
            {

            });
        }

        public void UpdateResponse(ResponseSessionLocked e)
        {
            UpdateResponse(e, response =>
            {
                response.ResponseIsLocked = true;
            });
        }

        public void UpdateResponse(ResponseSessionReviewed e)
        {
            UpdateResponse(e, response =>
            {

            });
        }

        public void UpdateResponse(ResponseSessionStarted e)
        {
            UpdateResponse(e, response =>
            {
                response.ResponseSessionStarted = e.Started;

                if (e.NoStatusChange != true)
                    response.ResponseSessionStatus = ResponseSessionStatus.Started.ToString();
            });
        }

        public void UpdateResponse(ResponseSessionUnlocked e)
        {
            UpdateResponse(e, response =>
            {
                response.ResponseIsLocked = false;
            });
        }

        public void UpdateResponse(ResponseSessionFormConsent e)
        {
            UpdateResponse(e, response =>
            {

            });
        }

        public void DeleteResponse(ResponseSessionDeleted e)
        {
            using (var db = CreateContext())
            {
                var session = db.QResponseSessions.FirstOrDefault(x => x.ResponseSessionIdentifier == e.AggregateIdentifier);
                if (session != null)
                {
                    db.QResponseSessions.Remove(session);
                    db.QResponseAnswers.RemoveRange(db.QResponseAnswers.Where(x => x.ResponseSessionIdentifier == e.AggregateIdentifier));
                    db.QResponseOptions.RemoveRange(db.QResponseOptions.Where(x => x.ResponseSessionIdentifier == e.AggregateIdentifier));
                    db.SaveChanges();
                }
            }
        }

        public void DeleteAll()
        {
            const string sql = @"
DELETE FROM assets.QComment WHERE ContainerType = '" + CommentContainerType + @"'
                              AND ContainerIdentifier IN (SELECT SurveyFormIdentifier FROM surveys.QSurveyForm);
TRUNCATE TABLE surveys.QSurveyCondition
TRUNCATE TABLE surveys.QSurveyForm
TRUNCATE TABLE surveys.QSurveyOptionItem
TRUNCATE TABLE surveys.QSurveyOptionList
TRUNCATE TABLE surveys.QSurveyQuestion
TRUNCATE TABLE surveys.QSurveyRespondent
";

            using (var db = CreateContext())
                db.Database.ExecuteSqlCommand(sql);
        }

        public void DeleteAll(Guid id)
        {
            const string sql = @"
delete from assets.QComment where ContainerIdentifier = @Survey and ContainerType = '" + CommentContainerType + @"'
delete from surveys.QSurveyCondition where MaskedSurveyQuestionIdentifier in (select SurveyQuestionIdentifier from surveys.QSurveyQuestion where SurveyFormIdentifier = @Survey)
delete from surveys.QSurveyOptionItem where SurveyOptionListIdentifier in (select SurveyOptionListIdentifier from surveys.QSurveyOptionList where SurveyQuestionIdentifier in (select SurveyQuestionIdentifier from surveys.QSurveyQuestion where SurveyFormIdentifier = @Survey))
delete from surveys.QSurveyOptionList where SurveyQuestionIdentifier in (select SurveyQuestionIdentifier from surveys.QSurveyQuestion where SurveyFormIdentifier = @Survey)
delete from surveys.QSurveyQuestion where SurveyFormIdentifier = @Survey
delete from surveys.QSurveyRespondent where SurveyFormIdentifier = @Survey
delete from surveys.QSurveyForm where SurveyFormIdentifier = @Survey
";

            using (var db = CreateContext())
                db.Database.ExecuteSqlCommand(sql, new[]
                {
                    new SqlParameter("Survey", id)
                });
        }

        public void DeleteAllResponses()
        {
            const string sql = @"
truncate surveys.QResponseAnswer
truncate surveys.QResponseOption
truncate surveys.QResponseSession
";

            using (var db = CreateContext())
                db.Database.ExecuteSqlCommand(sql);
        }

        public void DeleteAllResponses(Guid id)
        {
            const string sql = @"
delete surveys.QResponseAnswer where ResponseSessionIdentifier = @Session
delete surveys.QResponseOption where ResponseSessionIdentifier = @Session
delete surveys.QResponseSession where ResponseSessionIdentifier = @Session
";

            using (var db = CreateContext())
                db.Database.ExecuteSqlCommand(sql, new[]
                {
                    new SqlParameter("Session", id)
                });
        }

        #endregion
    }
}