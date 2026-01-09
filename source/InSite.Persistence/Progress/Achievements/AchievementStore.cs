using System;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Constant;

namespace InSite.Persistence
{
    public class AchievementStore : IAchievementStore
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(true) { EnablePrepareToSaveChanges = false };

        public void DeleteAll()
        {
            using (var db = CreateContext())
            {
                var sql = @"
TRUNCATE TABLE achievements.QAchievement;
TRUNCATE TABLE achievements.QCredential;
";
                db.Database.ExecuteSqlCommand(sql);
            }
        }

        public void DeleteAll(Guid organization)
        {
            using (var db = CreateContext())
            {
                var sql = @"
DELETE FROM achievements.QCredential WHERE EXISTS (SELECT * FROM achievements.QAchievement AS X WHERE X.AchievementIdentifier = QCredential.AchievementIdentifier AND X.OrganizationIdentifier = @OrganizationIdentifier);
DELETE FROM achievements.QAchievement WHERE OrganizationIdentifier = @OrganizationIdentifier;
";
                db.Database.ExecuteSqlCommand(sql, new SqlParameter("@OrganizationIdentifier", organization));
            }
        }

        public void Delete(Guid achievement)
        {
            using (var db = CreateContext())
            {
                var sql = @"
DELETE FROM achievements.QCredential WHERE AchievementIdentifier = @AchievementIdentifier;
DELETE FROM achievements.QAchievement WHERE AchievementIdentifier = @AchievementIdentifier;
";
                db.Database.ExecuteSqlCommand(sql, new SqlParameter("@AchievementIdentifier", achievement));
            }
        }

        #region Achievements

        public void InsertAchievement(AchievementCreated e)
        {
            using (var db = CreateContext())
            {
                var achievement = new QAchievement
                {
                    AchievementIdentifier = e.AggregateIdentifier,
                    OrganizationIdentifier = e.Tenant,
                    AchievementDescription = e.Description,
                    AchievementLabel = e.Label,
                    AchievementTitle = e.Title,
                    AchievementIsEnabled = true,
                };
                SetExpiration(achievement, e.Expiration);
                db.QAchievements.Add(achievement);
                db.SaveChanges();
            }
        }

        private void Update(IChange e, Action<QAchievement> change)
        {
            using (var db = CreateContext())
            {
                var achievement = db.QAchievements
                    .FirstOrDefault(x => x.AchievementIdentifier == e.AggregateIdentifier);

                if (achievement == null)
                    return;

                change(achievement);

                db.SaveChanges();
            }
        }

        public void UpdateAchievement(AchievementDescribed e)
        {
            Update(e, achievement =>
            {
                achievement.AchievementDescription = e.Description;
                achievement.AchievementLabel = e.Label;
                achievement.AchievementTitle = e.Title;
                achievement.AchievementAllowSelfDeclared = e.AllowSelfDeclared;
            });
        }

        public void UpdateAchievement(CertificateLayoutChanged e)
        {
            Update(e, achievement =>
            {
                achievement.CertificateLayoutCode = e.Code;
            });
        }

        public void UpdateAchievement(AchievementLocked e)
        {
            Update(e, achievement =>
            {
                achievement.AchievementIsEnabled = false;
            });
        }

        public void UpdateAchievement(AchievementUnlocked e)
        {
            Update(e, achievement =>
            {
                achievement.AchievementIsEnabled = true;
            });
        }

        public void UpdateAchievement(AchievementExpiryChanged e)
        {
            Update(e, (QAchievement achievement) =>
            {
                SetExpiration(achievement, e.Expiration);
            });
        }

        public void UpdateAchievement(AchievementTenantChanged e)
        {
            Update(e, achievement =>
            {
                achievement.OrganizationIdentifier = e.Tenant;
            });
        }

        public void UpdateAchievement(AchievementBadgeImageChanged e)
        {
            Update(e, achievement =>
            {
                achievement.BadgeImageUrl = e.BadgeImageUrl;
            });
        }

        public void UpdateAchievement(AchievementBadgeImageDisabled e)
        {
            Update(e, achievement =>
            {
                achievement.HasBadgeImage = false;
            });
        }

        public void UpdateAchievement(AchievementBadgeImageEnabled e)
        {
            Update(e, achievement =>
            {
                achievement.HasBadgeImage = true;
            });
        }

        public void UpdateAchievement(AchievementTypeChanged e)
        {
            Update(e, achievement =>
            {
                achievement.AchievementType = e.Type;
            });
        }

        public void UpdateAchievement(AchievementPrerequisiteAdded e)
        {
            using (var db = CreateContext())
            {
                if (db.QAchievementPrerequisites.Any(x => x.PrerequisiteIdentifier == e.Prerequisite))
                    return;

                foreach (var condition in e.Conditions)
                {
                    var prerequisite = new QAchievementPrerequisite
                    {
                        AchievementIdentifier = e.AggregateIdentifier,
                        PrerequisiteIdentifier = e.Prerequisite,
                        PrerequisiteAchievementIdentifier = condition
                    };

                    db.QAchievementPrerequisites.Add(prerequisite);
                }
                db.SaveChanges();
            }
        }

        public void UpdateAchievement(AchievementPrerequisiteDeleted e)
        {
            using (var db = CreateContext())
            {
                var prerequisites = db.QAchievementPrerequisites.Where(x => x.PrerequisiteIdentifier == e.Prerequisite);
                if (prerequisites.Count() == 0)
                    return;

                db.QAchievementPrerequisites.RemoveRange(prerequisites);
                db.SaveChanges();
            }
        }

        public void UpdateAchievement(AchievementReportingDisabled e)
        {
            Update(e, achievement =>
            {
                achievement.AchievementReportingDisabled = true;
            });
        }

        public void UpdateAchievement(AchievementReportingEnabled e)
        {
            Update(e, achievement =>
            {
                achievement.AchievementReportingDisabled = false;
            });
        }

        public void DeleteAchievement(AchievementDeleted e)
        {
            using (var db = CreateContext())
            {
                var entity = db.QAchievements.FirstOrDefault(x => x.AchievementIdentifier == e.AggregateIdentifier);
                if (entity != null)
                    db.QAchievements.Remove(entity);

                db.SaveChanges();
            }
        }

        #endregion

        #region Credentials

        public void InsertCredential(CredentialCreated e, CredentialStatus status)
        {
            using (var db = CreateContext())
            {
                var credential = db.QCredentials
                    .FirstOrDefault(x => x.CredentialIdentifier == e.AggregateIdentifier);

                if (credential == null)
                {
                    credential = new QCredential
                    {
                        CredentialIdentifier = e.AggregateIdentifier,
                        AchievementIdentifier = e.Achievement,
                        UserIdentifier = e.User,
                    };
                    db.QCredentials.Add(credential);
                }

                credential.CredentialAssigned = e.Assigned;
                credential.CredentialStatus = status.ToString();

                db.SaveChanges();
            }
        }

        private void Update(IChange e, string actionType, DateTimeOffset? actionDate, decimal? score, Action<QCredential> change)
        {
            using (var db = CreateContext())
            {
                var credential = db.QCredentials
                    .FirstOrDefault(x => x.CredentialIdentifier == e.AggregateIdentifier);

                if (credential == null)
                    return;

                change(credential);

                if (!string.IsNullOrEmpty(actionType))
                {
                    var history = new QCredentialHistory
                    {
                        AggregateIdentifier = credential.CredentialIdentifier,
                        AggregateVersion = e.AggregateVersion,
                        AchievementIdentifier = credential.AchievementIdentifier,
                        UserIdentifier = credential.UserIdentifier,
                        ChangeTime = e.ChangeTime,
                        ChangeBy = e.OriginUser,
                        CredentialActionDate = actionDate,
                        CredentialActionType = actionType,
                        CredentialScore = score
                    };
                    db.QCredentialHistories.Add(history);
                }

                db.SaveChanges();
            }
        }

        public void UpdateCredential(CredentialGranted3 e, CredentialStatus status)
        {
            Update(e, "Granted", e.Granted, e.Score, credential =>
            {
                var expiration = new Expiration(credential.ExpirationType, credential.ExpirationFixedDate, credential.ExpirationLifetimeQuantity, credential.ExpirationLifetimeUnit);

                credential.CredentialGranted = e.Granted;
                credential.CredentialGrantedDescription = e.Description;
                credential.CredentialGrantedScore = e.Score;

                credential.CredentialExpired = null;

                credential.CredentialRevoked = null;
                credential.CredentialRevokedReason = null;
                credential.CredentialRevokedScore = null;

                credential.CredentialExpirationExpected = CredentialState.CalculateExpectedExpiry(expiration, e.Granted);
                credential.CredentialStatus = status.ToString();

                ResetReminders(credential);
            });
        }

        public void UpdateCredential(CredentialExpired2 e, CredentialStatus status)
        {
            Update(e, "Expired", e.Expired, null, credential =>
            {
                credential.CredentialExpired = e.Expired;
                credential.CredentialExpirationExpected = e.Expired;
                credential.CredentialStatus = status.ToString();
            });
        }

        public void UpdateCredential(CredentialAuthorityChanged e)
        {
            Update(e, null, null, null, credential =>
            {
                credential.AuthorityIdentifier = e.AuthorityIdentifier;
                credential.AuthorityName = e.AuthorityName;
                credential.AuthorityType = e.AuthorityType;
                credential.AuthorityLocation = e.Location;
                credential.AuthorityReference = e.Reference;

                credential.CredentialHours = e.Hours;
            });
        }

        public void UpdateCredential(CredentialExpirationChanged e)
        {
            Update(e, null, null, null, (QCredential credential) =>
            {
                SetExpiration(credential, e.Expiration);
            });
        }

        public void UpdateCredential(CredentialTagged e)
        {
            Update(e, null, null, null, credential =>
            {
                credential.CredentialNecessity = e.Necessity;
                credential.CredentialPriority = e.Priority;
            });
        }

        public void UpdateCredential(CredentialRevoked2 e, CredentialStatus status)
        {
            Update(e, "Revoked", e.Revoked, e.Score, credential =>
            {
                credential.CredentialRevoked = e.Revoked;
                credential.CredentialRevokedReason = e.Reason;
                credential.CredentialGranted = null;
                credential.CredentialGrantedDescription = null;
                credential.CredentialGrantedScore = null;
                credential.CredentialRevokedScore = e.Score;
                credential.CredentialStatus = status.ToString();

                ResetReminders(credential);
            });
        }

        public void UpdateCredential(CredentialEmployerChanged e)
        {
            Update(e, null, null, null, credential =>
            {
                credential.EmployerGroupIdentifier = e.EmployerGroup;
                credential.EmployerGroupStatus = e.EmployerGroupStatus;
            });
        }

        public void UpdateCredential(ExpirationReminderRequested2 e)
        {
            Update(e, null, null, null, credential =>
            {
                credential.CredentialReminderType = e.Type.ToString();

                if (e.Type == ReminderType.Today)
                {
                    credential.CredentialExpirationReminderRequested0 = e.Requested;
                    credential.CredentialExpirationReminderDelivered0 = null;
                }
                else if (e.Type == ReminderType.InOneMonth)
                {
                    credential.CredentialExpirationReminderRequested1 = e.Requested;
                    credential.CredentialExpirationReminderDelivered1 = null;
                }
                else if (e.Type == ReminderType.InTwoMonths)
                {
                    credential.CredentialExpirationReminderRequested2 = e.Requested;
                    credential.CredentialExpirationReminderDelivered2 = null;
                }
                else if (e.Type == ReminderType.InThreeMonths)
                {
                    credential.CredentialExpirationReminderRequested3 = e.Requested;
                    credential.CredentialExpirationReminderDelivered3 = null;
                }
            });
        }

        public void UpdateCredential(ExpirationReminderDelivered2 e)
        {
            Update(e, null, null, null, credential =>
            {
                credential.CredentialReminderType = e.Type.ToString();

                if (e.Type == ReminderType.Today)
                    credential.CredentialExpirationReminderDelivered0 = e.Delivered;
                else if (e.Type == ReminderType.InOneMonth)
                    credential.CredentialExpirationReminderDelivered1 = e.Delivered;
                else if (e.Type == ReminderType.InTwoMonths)
                    credential.CredentialExpirationReminderDelivered2 = e.Delivered;
                else if (e.Type == ReminderType.InThreeMonths)
                    credential.CredentialExpirationReminderDelivered3 = e.Delivered;

            });
        }

        public void DeleteCredential(CredentialDeleted2 e)
        {
            using (var db = CreateContext())
            {
                var credential = db.QCredentials.FirstOrDefault(x => x.CredentialIdentifier == e.AggregateIdentifier);
                if (credential == null)
                    return;

                db.QCredentials.Remove(credential);

                ProgramStore.RemoveEnrollmentTaskCompletionDate(credential.UserIdentifier, credential.AchievementIdentifier);

                var history = new QCredentialHistory
                {
                    AggregateIdentifier = credential.CredentialIdentifier,
                    AggregateVersion = e.AggregateVersion,
                    AchievementIdentifier = credential.AchievementIdentifier,
                    UserIdentifier = credential.UserIdentifier,
                    ChangeTime = e.ChangeTime,
                    ChangeBy = e.OriginUser,
                    CredentialActionType = "Deleted"
                };
                db.QCredentialHistories.Add(history);

                db.SaveChanges();
            }
        }

        public void DeleteCredential(Guid credential)
        {
            using (var db = CreateContext())
            {
                var sql = @"
DELETE FROM achievements.QCredential WHERE CredentialIdentifier = @CredentialIdentifier;
DELETE FROM records.QCredentialHistory WHERE AggregateIdentifier = @CredentialIdentifier;
";
                db.Database.ExecuteSqlCommand(sql, new SqlParameter("@CredentialIdentifier", credential));
            }
        }

        public void UpdateCredential(CredentialDescribed2 e)
        {
            Update(e, null, null, null, credential =>
            {
                credential.CredentialDescription = e.Description;
            });
        }

        #endregion

        private void ResetReminders(QCredential credential)
        {
            credential.CredentialExpirationReminderRequested0 = null;
            credential.CredentialExpirationReminderRequested1 = null;
            credential.CredentialExpirationReminderRequested2 = null;
            credential.CredentialExpirationReminderRequested3 = null;

            credential.CredentialExpirationReminderDelivered0 = null;
            credential.CredentialExpirationReminderDelivered1 = null;
            credential.CredentialExpirationReminderDelivered2 = null;
            credential.CredentialExpirationReminderDelivered3 = null;
        }

        private void SetExpiration(QAchievement achievement, Expiration expiration)
        {
            if (expiration != null && expiration.Type != ExpirationType.None)
            {
                achievement.ExpirationType = expiration.Type.ToString();
                achievement.ExpirationFixedDate = expiration.Date;
                if (expiration.Lifetime != null)
                {
                    achievement.ExpirationLifetimeQuantity = expiration.Lifetime.Quantity;
                    achievement.ExpirationLifetimeUnit = expiration.Lifetime.Unit;
                }
            }
            else
            {
                achievement.ExpirationType = ExpirationType.None.ToString();
                achievement.ExpirationFixedDate = null;
                achievement.ExpirationLifetimeQuantity = null;
                achievement.ExpirationLifetimeUnit = null;
            }
        }

        private void SetExpiration(QCredential credential, Expiration expiration)
        {
            if (expiration != null && expiration.Type != ExpirationType.None)
            {
                credential.ExpirationType = expiration.Type.ToString();
                credential.ExpirationFixedDate = expiration.Date;
                if (expiration.Lifetime != null)
                {
                    credential.ExpirationLifetimeQuantity = expiration.Lifetime.Quantity;
                    credential.ExpirationLifetimeUnit = expiration.Lifetime.Unit;
                }
                credential.CredentialExpirationExpected = CredentialState.CalculateExpectedExpiry(expiration, credential.CredentialGranted);

                if (credential.CredentialExpirationExpected.HasValue)
                {
                    if (credential.CredentialExpirationExpected < DateTimeOffset.UtcNow)
                        credential.CredentialStatus = CredentialStatus.Expired.ToString();
                    else
                        credential.CredentialStatus = CredentialStatus.Valid.ToString();
                }
            }
            else
            {
                credential.ExpirationType = ExpirationType.None.ToString();
                credential.ExpirationFixedDate = null;
                credential.ExpirationLifetimeQuantity = null;
                credential.ExpirationLifetimeUnit = null;
                credential.CredentialExpirationExpected = null;
            }
        }
    }
}
