using System;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contents.Read;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Common;

namespace InSite.Persistence
{
    public class RecordStore : IRecordStore
    {
        public RecordStore() { }

        public void InsertRecord(GradebookCreated change)
        {
            using (var db = CreateContext())
            {
                var gradebook = new QGradebook
                {
                    GradebookIdentifier = change.AggregateIdentifier,
                    OrganizationIdentifier = change.Tenant,
                    FrameworkIdentifier = change.Framework,
                    EventIdentifier = change.Event,
                    AchievementIdentifier = change.Achievement,
                    GradebookTitle = change.Name,
                    GradebookType = change.Type.ToString(),
                    GradebookCreated = change.ChangeTime
                };

                db.QGradebooks.Add(gradebook);

                if (change.Event.HasValue)
                {
                    db.QGradebookEvents.Add(new QGradebookEvent
                    {
                        GradebookIdentifier = change.AggregateIdentifier,
                        EventIdentifier = change.Event.Value
                    });
                }

                SetLastChange(gradebook, change);
                db.SaveChanges();
            }
        }

        public void DeleteRecord(GradebookDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QGradebooks.FirstOrDefault(x => x.GradebookIdentifier == e.AggregateIdentifier);

                if (entity != null)
                {
                    var items = db.QGradeItems.Where(x => x.GradebookIdentifier == e.AggregateIdentifier).ToList();

                    if (items.Count > 0)
                        db.QGradeItems.RemoveRange(items);

                    db.QGradebooks.Remove(entity);
                    db.SaveChanges();
                }
            }
        }

        public void DeleteRecord(Guid record)
        {
            using (var db = CreateContext())
            {
                var sql = @"
DELETE FROM records.QGradebookCompetencyValidation WHERE GradebookIdentifier = @GradebookIdentifier;
DELETE FROM records.QProgress WHERE GradebookIdentifier = @GradebookIdentifier;
DELETE FROM records.QEnrollment WHERE GradebookIdentifier = @GradebookIdentifier;

DELETE FROM records.QGradebook WHERE GradebookIdentifier = @GradebookIdentifier;
DELETE FROM records.QGradeItem WHERE GradebookIdentifier = @GradebookIdentifier;
DELETE FROM records.QGradeItemCompetency WHERE GradebookIdentifier = @GradebookIdentifier;
";
                db.Database.ExecuteSqlCommand(sql, new System.Data.SqlClient.SqlParameter("@GradebookIdentifier", record));
            }
        }

        private void Update(IChange change, Action<QGradebook, InternalDbContext> action)
        {
            using (var db = CreateContext())
            {
                var gradebook = db.QGradebooks.Single(x => x.GradebookIdentifier == change.AggregateIdentifier);
                action(gradebook, db);
                SetLastChange(gradebook, change);
                db.SaveChanges();
            }
        }

        private void SetLastChange(QGradebook gradebook, IChange change)
        {
            gradebook.LastChangeTime = change.ChangeTime;
            gradebook.LastChangeType = change.GetType().Name;
            gradebook.LastChangeUser = UserSearch.GetFullName(change.OriginUser);
        }

        public void UpdateRecord(GradebookCalculated c)
        {
            Update(c, (x, db) => { });
        }

        public void UpdateRecord(GradebookLocked c)
        {
            Update(c, (x, db) =>
            {
                x.IsLocked = true;
            });
        }

        public void UpdateRecord(GradebookUnlocked c)
        {
            Update(c, (x, db) =>
            {
                x.IsLocked = false;
            });
        }

        public void UpdateRecord(GradebookReferenced c)
        {
            Update(c, (x, db) =>
            {
                x.Reference = c.Reference;
            });
        }

        public void UpdateRecord(GradebookRenamed c)
        {
            Update(c, (x, db) =>
            {
                x.GradebookTitle = c.Name;
            });
        }

        public void UpdateRecord(GradebookAchievementChanged c)
        {
            Update(c, (x, db) =>
            {
                x.AchievementIdentifier = c.Achievement;
            });
        }

        public void UpdateRecord(GradebookEventChanged c)
        {
            Update(c, (x, db) =>
            {
                if (x.EventIdentifier == c.Event)
                    return;

                var events = db.QGradebookEvents.Where(y => y.GradebookIdentifier == x.GradebookIdentifier).ToList();
                db.QGradebookEvents.RemoveRange(events);

                x.EventIdentifier = c.Event;

                if (c.Event.HasValue)
                {
                    db.QGradebookEvents.Add(new QGradebookEvent
                    {
                        GradebookIdentifier = x.GradebookIdentifier,
                        EventIdentifier = c.Event.Value
                    });
                }
            });
        }

        public void UpdateRecord(GradebookEventAdded c)
        {
            Update(c, (x, db) =>
            {
                if (!db.QGradebookEvents.Any(y => y.GradebookIdentifier == x.GradebookIdentifier && y.EventIdentifier == c.Event))
                {
                    db.QGradebookEvents.Add(new QGradebookEvent
                    {
                        GradebookIdentifier = x.GradebookIdentifier,
                        EventIdentifier = c.Event
                    });
                }

                if (c.IsPrimary)
                    x.EventIdentifier = c.Event;
            });
        }

        public void UpdateRecord(GradebookEventRemoved c)
        {
            Update(c, (x, db) =>
            {
                var remove = db.QGradebookEvents.FirstOrDefault(y => y.GradebookIdentifier == x.GradebookIdentifier && y.EventIdentifier == c.Event);
                if (remove != null)
                    db.QGradebookEvents.Remove(remove);

                x.EventIdentifier = c.NewPrimaryEvent;
            });
        }

        public void UpdateRecord(GradebookTypeChanged c)
        {
            Update(c, (x, db) =>
            {
                x.FrameworkIdentifier = c.Framework;
                x.GradebookType = c.Type.ToString();
            });
        }

        public void UpdateRecord(GradebookPeriodChanged c)
        {
            Update(c, (x, db) =>
            {
                x.PeriodIdentifier = c.Period;
            });
        }

        public void UpdateRecord(GradebookWarningAdded c)
        {
            Update(c, (x, db) => { });

            var comment = new QComment
            {
                ContainerType = "Gradebook",
                ContainerIdentifier = c.AggregateIdentifier,

                CommentCategory = "Timeline",
                CommentSubCategory = "Warning",

                CommentIdentifier = UniqueIdentifier.Create(),
                CommentPosted = c.ChangeTime,
                CommentText = c.Warning ?? "-",

                AuthorUserIdentifier = c.OriginUser,
                TimestampCreated = DateTimeOffset.Now
            };
            QCommentStore.Insert(comment);
        }

        public void InsertItem(GradeItemAdded c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var item = db.QGradeItems.FirstOrDefault(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item);
                if (item != null)
                    return;

                item = new QGradeItem
                {
                    GradebookIdentifier = c.AggregateIdentifier,
                    GradeItemIdentifier = c.Item,
                    ParentGradeItemIdentifier = c.Parent,
                    GradeItemSequence = c.Sequence,
                    GradeItemCode = c.Code,
                    GradeItemName = c.Name,
                    GradeItemShortName = c.ShortName,
                    GradeItemFormat = c.Format.ToString(),
                    GradeItemType = c.Type.ToString(),
                    GradeItemWeighting = c.Weighting.ToString(),
                    GradeItemIsReported = c.IsReported,
                    GradeItemPassPercent = c.PassPercent
                };

                db.QGradeItems.Add(item);
                db.SaveChanges();
            }
        }

        public void UpdateItem(GradeItemAchievementChanged c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var item = db.QGradeItems.FirstOrDefault(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item);

                if (item != null)
                {
                    item.AchievementIdentifier = c.Achievement?.Achievement;
                    item.AchievementWhenChange = c.Achievement?.WhenChange.ToString();
                    item.AchievementWhenGrade = c.Achievement?.WhenGrade.ToString();
                    item.AchievementThenCommand = c.Achievement?.ThenCommand.ToString();
                    item.AchievementElseCommand = c.Achievement?.ElseCommand.ToString();
                    item.AchievementFixedDate = c.Achievement?.AchievementFixedDate;

                    db.SaveChanges();
                }
            }
        }

        public void UpdateItem(GradeItemNotificationsChanged c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var item = db.QGradeItems.FirstOrDefault(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item);
                if (item == null)
                    return;

                if (c.Notifications.IsEmpty())
                    return;

                var first = c.Notifications.FirstOrDefault(x => x.Change == "ProgressCompleted");

                if (first == null)
                    return;

                item.ProgressCompletedMessageIdentifier = first.Message;
                db.SaveChanges();
            }
        }

        public void UpdateItem(GradeItemHookChanged c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var item = db.QGradeItems.FirstOrDefault(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item);

                if (item != null)
                {
                    item.GradeItemHook = c.Hook;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateItem(GradeItemChanged c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var item = db.QGradeItems.FirstOrDefault(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item);

                if (item != null)
                {
                    item.ParentGradeItemIdentifier = c.Parent;
                    item.GradeItemCode = c.Code;
                    item.GradeItemName = c.Name;
                    item.GradeItemFormat = c.Format.ToString();
                    item.GradeItemType = c.Type.ToString();
                    item.GradeItemWeighting = c.Weighting.ToString();
                    item.GradeItemIsReported = c.IsReported;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateItem(GradeItemPassPercentChanged c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var item = db.QGradeItems.FirstOrDefault(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item);

                if (item != null)
                {
                    item.GradeItemPassPercent = c.PassPercent;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateItem(GradeItemMaxPointsChanged c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var item = db.QGradeItems.FirstOrDefault(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item);

                if (item != null)
                {
                    item.GradeItemMaxPoints = c.MaxPoints;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateItem(GradeItemReferenced c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var item = db.QGradeItems.FirstOrDefault(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item);

                if (item != null)
                {
                    item.GradeItemReference = c.Reference;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateItem(GradeItemCompetenciesChanged c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var existing = db.QGradeItemCompetencies.Where(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item).ToList();

                if (c.Competencies.IsEmpty())
                {
                    db.QGradeItemCompetencies.RemoveRange(existing);
                }
                else
                {
                    foreach (var entity in existing)
                    {
                        if (!c.Competencies.Any(x => x == entity.CompetencyIdentifier))
                            db.QGradeItemCompetencies.Remove(entity);
                    }

                    foreach (var standard in c.Competencies)
                    {
                        if (!existing.Any(x => x.CompetencyIdentifier == standard))
                        {
                            db.QGradeItemCompetencies.Add(new QGradeItemCompetency
                            {
                                GradebookIdentifier = c.AggregateIdentifier,
                                GradeItemIdentifier = c.Item,
                                CompetencyIdentifier = standard
                            });
                        }
                    }
                }

                db.SaveChanges();
            }
        }

        public void DeleteItem(GradeItemDeleted c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var item = db.QGradeItems.FirstOrDefault(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item);

                if (item != null)
                {
                    var standards = db.QGradeItemCompetencies.Where(x => x.GradebookIdentifier == c.AggregateIdentifier && x.GradeItemIdentifier == c.Item).ToList();
                    db.QGradeItemCompetencies.RemoveRange(standards);
                    db.QGradeItems.Remove(item);
                    db.SaveChanges();
                }
            }
        }

        public void ReorderItems(GradeItemReordered c)
        {
            Update(c, (x, db) => { });

            using (var db = CreateContext())
            {
                var items = db.QGradeItems.Where(x => x.GradebookIdentifier == c.AggregateIdentifier && x.ParentGradeItemIdentifier == c.Parent).ToList();

                for (int i = 0; i < c.Children.Length; i++)
                {
                    var key = c.Children[i];
                    var entity = items.Find(x => x.GradeItemIdentifier == key);

                    if (entity != null)
                        entity.GradeItemSequence = i + 1;
                }

                db.SaveChanges();
            }
        }

        public void InsertEnrollment(EnrollmentStarted e)
        {
            using (var db = CreateContext())
            {
                var enrollment = db.QEnrollments
                    .FirstOrDefault(x => x.GradebookIdentifier == e.AggregateIdentifier && x.LearnerIdentifier == e.Learner);

                if (enrollment == null)
                {
                    enrollment = new QEnrollment
                    {
                        GradebookIdentifier = e.AggregateIdentifier,
                        LearnerIdentifier = e.Learner,
                        PeriodIdentifier = e.Period,
                        EnrollmentIdentifier = e.Enrollment,
                        EnrollmentStarted = e.Started,
                        EnrollmentComment = e.Comment
                    };
                    db.QEnrollments.Add(enrollment);
                }

                var starts = db.QEnrollmentHistories.Count(
                    x => x.GradebookIdentifier == enrollment.GradebookIdentifier
                        && x.LearnerIdentifier == enrollment.LearnerIdentifier
                        && x.EnrollmentType == "Started");
                enrollment.EnrollmentRestart = starts;

                var history = new QEnrollmentHistory
                {
                    AggregateIdentifier = enrollment.GradebookIdentifier,
                    AggregateVersion = e.AggregateVersion,

                    ChangeTime = e.ChangeTime,
                    ChangeBy = e.OriginUser,

                    GradebookIdentifier = enrollment.GradebookIdentifier,
                    LearnerIdentifier = enrollment.LearnerIdentifier,

                    EnrollmentIdentifier = enrollment.EnrollmentIdentifier,
                    EnrollmentType = "Started",
                    EnrollmentTime = e.Started,
                };
                db.QEnrollmentHistories.Add(history);

                db.SaveChanges();
            }
        }

        public void DeleteEnrollment(GradebookUserDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QEnrollments.FirstOrDefault(x => x.GradebookIdentifier == e.AggregateIdentifier && x.LearnerIdentifier == e.User);
                if (entity == null)
                    return;

                var standardScores = db.QGradebookCompetencyValidations.Where(x => x.GradebookIdentifier == e.AggregateIdentifier && x.UserIdentifier == e.User).ToList();

                db.QGradebookCompetencyValidations.RemoveRange(standardScores);
                db.QEnrollments.Remove(entity);

                var history = new QEnrollmentHistory
                {
                    AggregateIdentifier = entity.GradebookIdentifier,
                    AggregateVersion = e.AggregateVersion,

                    ChangeTime = e.ChangeTime,
                    ChangeBy = e.OriginUser,

                    GradebookIdentifier = entity.GradebookIdentifier,
                    LearnerIdentifier = entity.LearnerIdentifier,

                    EnrollmentIdentifier = entity.EnrollmentIdentifier,
                    EnrollmentType = "Deleted",
                    EnrollmentTime = e.ChangeTime
                };
                db.QEnrollmentHistories.Add(history);

                db.SaveChanges();
            }
        }

        public void UpdateEnrollment(EnrollmentCompleted e)
        {
            using (var db = CreateContext())
            {
                var enrollment = db.QEnrollments.FirstOrDefault(x => x.GradebookIdentifier == e.AggregateIdentifier && x.LearnerIdentifier == e.Learner);
                if (enrollment != null)
                {
                    enrollment.EnrollmentCompleted = e.Completed;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateEnrollment(EnrollmentRestarted e)
        {
            using (var db = CreateContext())
            {
                var enrollment = db.QEnrollments.FirstOrDefault(x => x.GradebookIdentifier == e.AggregateIdentifier && x.LearnerIdentifier == e.Learner);

                if (enrollment != null)
                {
                    var restarts = db.QEnrollmentHistories.Count(
                    x => x.GradebookIdentifier == enrollment.GradebookIdentifier
                        && x.LearnerIdentifier == enrollment.LearnerIdentifier
                        && (x.EnrollmentType == "Started" || x.EnrollmentType == "Restarted"));

                    var history = new QEnrollmentHistory
                    {
                        AggregateIdentifier = enrollment.GradebookIdentifier,
                        AggregateVersion = e.AggregateVersion,

                        ChangeTime = e.ChangeTime,
                        ChangeBy = e.OriginUser,

                        GradebookIdentifier = enrollment.GradebookIdentifier,
                        LearnerIdentifier = enrollment.LearnerIdentifier,

                        EnrollmentIdentifier = enrollment.EnrollmentIdentifier,
                        EnrollmentType = "Restarted",
                        EnrollmentTime = e.Restarted,
                    };
                    db.QEnrollmentHistories.Add(history);

                    enrollment.EnrollmentRestart = restarts;
                    enrollment.EnrollmentStarted = e.Restarted;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateEnrollment(GradebookUserNoted e)
        {
            using (var db = CreateContext())
            {
                var enrollment = db.QEnrollments.FirstOrDefault(x => x.GradebookIdentifier == e.AggregateIdentifier && x.LearnerIdentifier == e.User);

                if (enrollment != null)
                {
                    enrollment.EnrollmentComment = e.Note;
                    if (e.Added != null)
                        enrollment.EnrollmentStarted = e.Added;
                    db.SaveChanges();
                }
            }
        }

        public void UpdateEnrollment(GradebookUserPeriodChanged e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QEnrollments.FirstOrDefault(x => x.GradebookIdentifier == e.AggregateIdentifier && x.LearnerIdentifier == e.User);
                if (entity != null)
                {
                    entity.PeriodIdentifier = e.Period;
                    db.SaveChanges();
                }
            }
        }

        public void InsertValidation(GradebookValidationAdded e)
        {
            SaveStandardScore(e.AggregateIdentifier, e.Competency, e.User, e.Points);
        }

        public void UpdateValidation(GradebookValidationChanged e)
        {
            SaveStandardScore(e.AggregateIdentifier, e.Competency, e.User, e.Points);
        }

        private void SaveStandardScore(Guid gradebookIdentifier, Guid standardIdentifier, Guid userIdentifier, decimal? point)
        {
            using (var db = CreateContext())
            {
                var entity = db.QGradebookCompetencyValidations.FirstOrDefault(x =>
                    x.GradebookIdentifier == gradebookIdentifier
                    && x.CompetencyIdentifier == standardIdentifier
                    && x.UserIdentifier == userIdentifier
                );

                if (entity == null)
                {
                    entity = new QGradebookCompetencyValidation
                    {
                        ValidationIdentifier = UniqueIdentifier.Create(),
                        GradebookIdentifier = gradebookIdentifier,
                        UserIdentifier = userIdentifier,
                        CompetencyIdentifier = standardIdentifier
                    };

                    db.QGradebookCompetencyValidations.Add(entity);
                }
                else if (entity.ValidationPoints == point)
                {
                    return;
                }

                entity.ValidationPoints = point;

                db.SaveChanges();
            }
        }

        public void InsertProgress(ProgressAdded e)
        {
            using (var db = CreateContext())
            {
                var entity = new QProgress
                {
                    ProgressIdentifier = e.AggregateIdentifier,
                    GradebookIdentifier = e.Record,
                    GradeItemIdentifier = e.Item,
                    UserIdentifier = e.User,
                    ProgressAdded = e.ChangeTime
                };

                db.QProgresses.Add(entity);

                db.SaveChanges();
            }
        }

        public void UpdateProgress(ProgressCommentChanged e)
        {
            UpdateProgress(e, null, null, x => x.ProgressComment = e.Comment);
        }

        public void UpdateProgress(ProgressCompleted2 e)
        {
            UpdateProgress(e, "Completed", e.Completed, x =>
            {
                x.ProgressStatus = ProgressCompleted2.Status;
                x.ProgressCompleted = e.Completed;
                x.ProgressElapsedSeconds = e.ElapsedSeconds;
                x.ProgressIsCompleted = true;

                if (e.Pass.HasValue)
                    x.ProgressPassOrFail = e.Pass.Value ? "Pass" : "Fail";

                x.ProgressGraded = e.Completed;
                x.ProgressPercent = e.Percent.HasValue
                    ? Math.Round(e.Percent.Value, 4, MidpointRounding.AwayFromZero)
                    : (decimal?)null;
            });
        }

        public void DeleteProgress(ProgressDeleted e)
        {
            using (var db = CreateContext())
            {
                var progress = db.QProgresses.FirstOrDefault(x => x.ProgressIdentifier == e.AggregateIdentifier);
                if (progress == null)
                    return;

                db.QProgresses.Remove(progress);

                var history = new QProgressHistory
                {
                    AggregateIdentifier = progress.ProgressIdentifier,
                    AggregateVersion = e.AggregateVersion,

                    ChangeTime = e.ChangeTime,
                    ChangeBy = e.OriginUser,

                    GradebookIdentifier = progress.GradebookIdentifier,
                    GradeItemIdentifier = progress.GradeItemIdentifier,
                    UserIdentifier = progress.UserIdentifier,

                    ProgressType = "Deleted",
                    ProgressTime = e.ChangeTime
                };
                db.QProgressHistories.Add(history);

                db.SaveChanges();
            }
        }

        public void UpdateProgress(ProgressHidden e)
        {
            throw new NotImplementedException();
        }

        public void UpdateProgress(ProgressLocked e)
        {
            throw new NotImplementedException();
        }

        public void UpdateProgress(ProgressNumberChanged e)
        {
            UpdateProgress(e, null, null, x =>
            {
                x.ProgressNumber = e.Number;
                x.ProgressGraded = e.Graded;
            });
        }

        public void UpdateProgress(ProgressPercentChanged e)
        {
            UpdateProgress(e, null, null, x =>
            {
                x.ProgressPercent = e.Percent.HasValue
                    ? Math.Round(e.Percent.Value, 4, MidpointRounding.AwayFromZero)
                    : (decimal?)null;
                x.ProgressGraded = e.Graded;
            });
        }

        public void UpdateProgress(ProgressPointsChanged e)
        {
            UpdateProgress(e, null, null, x =>
            {
                x.ProgressPoints = e.Points;
                x.ProgressMaxPoints = e.MaxPoints;
                x.ProgressGraded = e.Graded;
            });
        }

        public void UpdateProgress(ProgressPublished e)
        {
            UpdateProgress(e, null, null, x =>
            {
                x.ProgressIsPublished = true;
            });
        }

        public void UpdateProgress(ProgressShowed e)
        {
            throw new NotImplementedException();
        }

        public void UpdateProgress(ProgressStarted e)
        {
            UpdateProgress(e, "Started", e.Started, x =>
            {
                x.ProgressStatus = ProgressStarted.Status;
                x.ProgressStarted = e.Started;
                x.ProgressCompleted = null;
                x.ProgressElapsedSeconds = null;
                x.ProgressIsCompleted = false;
                x.ProgressPassOrFail = null;
                x.ProgressGraded = null;
                x.ProgressPercent = null;
            });
        }

        public void UpdateProgress(ProgressTextChanged e)
        {
            UpdateProgress(e, null, null, x =>
            {
                x.ProgressText = e.Text;
                x.ProgressGraded = e.Graded;
            });
        }

        public void UpdateProgress(ProgressUnlocked e)
        {
            throw new NotImplementedException();
        }

        public void UpdateProgress(ProgressIncompleted e)
        {
            UpdateProgress(e, "Incomplete", e.ChangeTime, x =>
            {
                x.ProgressStatus = ProgressIncompleted.Status;
                x.ProgressStarted = null;
                x.ProgressCompleted = null;
                x.ProgressElapsedSeconds = null;
                x.ProgressIsCompleted = false;
                x.ProgressPassOrFail = null;
                x.ProgressGraded = null;
                x.ProgressPercent = null;
            });
        }

        public void UpdateProgress(ProgressIgnored e)
        {
            UpdateProgress(e, null, null, x =>
            {
                x.ProgressIsIgnored = e.IsIgnored;
            });
        }

        public void DeleteOne(Guid gradebook)
        {
            DeleteRecord(gradebook);
        }

        public void DeleteAll()
        {
            using (var db = CreateContext())
            {
                var sql = @"
TRUNCATE TABLE records.QGradebookScore;
TRUNCATE TABLE records.QGradebookStandardScore;
TRUNCATE TABLE records.QGradebookItemStandard;
TRUNCATE TABLE records.QGradebookItem;
TRUNCATE TABLE records.QGradebookStudent;
TRUNCATE TABLE records.QGradebook;
";
                db.Database.ExecuteSqlCommand(sql);
            }
        }

        private static InternalDbContext CreateContext()
        {
            return new InternalDbContext(true);
        }

        private void UpdateProgress(Change e, string progressType, DateTimeOffset? progressTime, Action<QProgress> change)
        {
            using (var db = CreateContext())
            {
                var progress = db.QProgresses
                    .FirstOrDefault(x => x.ProgressIdentifier == e.AggregateIdentifier);

                if (progress == null)
                    return;

                change(progress);

                if (!string.IsNullOrEmpty(progressType))
                {
                    if (progressType == "Started")
                    {
                        var starts = db.QProgressHistories.Count(
                            x => x.GradeItemIdentifier == progress.GradeItemIdentifier
                              && x.UserIdentifier == progress.UserIdentifier
                              && x.ProgressType == progressType);
                        progress.ProgressRestartCount = starts;
                    }

                    var history = new QProgressHistory
                    {
                        AggregateIdentifier = progress.ProgressIdentifier,
                        AggregateVersion = e.AggregateVersion,

                        ChangeTime = e.ChangeTime,
                        ChangeBy = e.OriginUser,

                        GradebookIdentifier = progress.GradebookIdentifier,
                        GradeItemIdentifier = progress.GradeItemIdentifier,
                        UserIdentifier = progress.UserIdentifier,

                        ProgressType = progressType,
                        ProgressTime = progressTime,
                    };
                    db.QProgressHistories.Add(history);
                }

                db.SaveChanges();
            }
        }
    }
}
