using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Standards.Read;
using InSite.Domain.Standards;

namespace InSite.Persistence
{
    public class QStandardStore : IStandardStore
    {
        public void DeleteAll()
        {
            const string sql = @"
DELETE contents.TContent WHERE ContainerIdentifier IN (SELECT StandardIdentifier FROM [standard].QStandard);
TRUNCATE TABLE [standard].QStandard;
TRUNCATE TABLE [standard].QStandardAchievement;
TRUNCATE TABLE [standard].QStandardCategory;
TRUNCATE TABLE [standard].QStandardConnection;
TRUNCATE TABLE [standard].QStandardContainment;
TRUNCATE TABLE [standard].QStandardGroup;
TRUNCATE TABLE [standard].QStandardOrganization;
";

            using (var db = new InternalDbContext())
                db.Database.ExecuteSqlCommand(sql);
        }

        public void DeleteAll(Guid id)
        {
            const string sql = @"
DELETE [standard].QStandard WHERE StandardIdentifier = @ID;
DELETE [standard].QStandardAchievement WHERE StandardIdentifier = @ID;
DELETE [standard].QStandardCategory WHERE StandardIdentifier = @ID;
DELETE [standard].QStandardConnection WHERE FromStandardIdentifier = @ID;
DELETE [standard].QStandardContainment WHERE ParentStandardIdentifier = @ID;
DELETE [standard].QStandardGroup WHERE StandardIdentifier = @ID;
DELETE [standard].QStandardOrganization WHERE StandardIdentifier = @ID;
DELETE contents.TContent WHERE ContainerIdentifier = @ID;
";

            using (var db = new InternalDbContext())
                db.Database.ExecuteSqlCommand(sql, new[]
                {
                    new SqlParameter("ID", id)
                });
        }

        public void Insert(StandardCreated e)
        {
            var state = (StandardState)e.AggregateState;
            var entity = new QStandard
            {
                StandardIdentifier = e.AggregateIdentifier,
                OrganizationIdentifier = state.GetGuidValue(StandardField.OrganizationIdentifier).Value,
                StandardType = e.StandardType,
                AssetNumber = e.AssetNumber,
                Sequence = e.Sequence
            };

            SetEntityContent(entity, state);

            SetTimestamp(entity, state);

            DbContext(db =>
            {
                db.QStandards.Add(entity);
                db.SaveChanges();
            });
        }

        public void Delete(StandardRemoved e)
        {
            DbContext(db =>
            {
                var standard = db.QStandards.FirstOrDefault(x => x.StandardIdentifier == e.AggregateIdentifier);
                if (standard == null)
                    return;

                var achievements = db.QStandardAchievements
                    .Where(x => x.StandardIdentifier == standard.StandardIdentifier)
                    .ToArray();
                var categories = db.QStandardCategories
                    .Where(x => x.StandardIdentifier == standard.StandardIdentifier)
                    .ToArray();
                var connections = db.QStandardConnections
                    .Where(x => x.FromStandardIdentifier == standard.StandardIdentifier)
                    .ToArray();
                var containments = db.QStandardContainments
                    .Where(x => x.ParentStandardIdentifier == standard.StandardIdentifier)
                    .ToArray();
                var groups = db.QStandardGroups
                    .Where(x => x.StandardIdentifier == standard.StandardIdentifier)
                    .ToArray();
                var organizations = db.QStandardOrganizations
                    .Where(x => x.StandardIdentifier == standard.StandardIdentifier)
                    .ToArray();
                var contents = db.TContents
                    .Where(x => x.ContainerIdentifier == standard.StandardIdentifier && x.ContainerType == "Standard")
                    .ToArray();

                db.QStandardAchievements.RemoveRange(achievements);
                db.QStandardCategories.RemoveRange(categories);
                db.QStandardConnections.RemoveRange(connections);
                db.QStandardContainments.RemoveRange(containments);
                db.QStandardGroups.RemoveRange(groups);
                db.QStandardOrganizations.RemoveRange(organizations);
                db.TContents.RemoveRange(contents);
                db.QStandards.Remove(standard);

                db.SaveChanges();
            });
        }

        public void Update(StandardTimestampsModified e) => Update(e, x =>
        {

        });

        public void Update(StandardCategoryAdded e)
        {
            var state = (StandardState)e.AggregateState;
            var orgId = state.GetGuidValue(StandardField.OrganizationIdentifier).Value;

            DbContext(db =>
            {
                foreach (var c in e.Categories)
                    db.QStandardCategories.Add(new QStandardCategory
                    {
                        StandardIdentifier = e.AggregateIdentifier,
                        CategoryIdentifier = c.CategoryId,
                        OrganizationIdentifier = orgId,
                        ClassificationSequence = c.Sequence
                    });
                db.SaveChanges();
            });
        }

        public void Update(StandardCategoryRemoved e)
        {
            DbContext(db =>
            {
                foreach (var id in e.CategoryIds)
                {
                    var entity = new QStandardCategory
                    {
                        StandardIdentifier = e.AggregateIdentifier,
                        CategoryIdentifier = id,
                    };

                    db.Entry(entity).State = EntityState.Deleted;
                }

                db.SaveChanges();
            });
        }

        public void Update(StandardConnectionAdded e)
        {
            var state = (StandardState)e.AggregateState;
            var orgId = state.GetGuidValue(StandardField.OrganizationIdentifier).Value;

            DbContext(db =>
            {
                foreach (var c in e.Connections)
                    db.QStandardConnections.Add(new QStandardConnection
                    {
                        FromStandardIdentifier = e.AggregateIdentifier,
                        ToStandardIdentifier = c.ToStandardId,
                        OrganizationIdentifier = orgId,
                        ConnectionType = c.ConnectionType
                    });
                db.SaveChanges();
            });
        }

        public void Update(StandardConnectionRemoved e)
        {
            DbContext(db =>
            {
                foreach (var id in e.ToStandardIds)
                {
                    var entity = new QStandardConnection
                    {
                        FromStandardIdentifier = e.AggregateIdentifier,
                        ToStandardIdentifier = id
                    };

                    db.Entry(entity).State = EntityState.Deleted;
                }

                db.SaveChanges();
            });
        }

        public void Update(StandardContainmentAdded e)
        {
            var state = (StandardState)e.AggregateState;
            var orgId = state.GetGuidValue(StandardField.OrganizationIdentifier).Value;

            DbContext(db =>
            {
                foreach (var c in e.Containments)
                {
                    var model = state.GetContainment(c.ChildStandardId);

                    db.QStandardContainments.Add(new QStandardContainment
                    {
                        ParentStandardIdentifier = e.AggregateIdentifier,
                        ChildStandardIdentifier = model.ChildStandardId,
                        OrganizationIdentifier = orgId,
                        ChildSequence = model.ChildSequence,
                        CreditHours = model.CreditHours,
                        CreditType = model.CreditType
                    });
                }
                db.SaveChanges();
            });
        }

        public void Update(StandardContainmentModified e)
        {
            var state = (StandardState)e.AggregateState;

            DbContext(db =>
            {
                foreach (var c in e.Containments)
                {
                    var model = state.GetContainment(c.ChildStandardId);
                    var entity = db.QStandardContainments
                        .FirstOrDefault(x => x.ParentStandardIdentifier == e.AggregateIdentifier
                                          && x.ChildStandardIdentifier == c.ChildStandardId);

                    entity.ChildSequence = model.ChildSequence;
                    entity.CreditHours = model.CreditHours;
                    entity.CreditType = model.CreditType;
                }
                db.SaveChanges();
            });
        }

        public void Update(StandardContainmentRemoved e)
        {
            DbContext(db =>
            {
                foreach (var id in e.ChildStandardIds)
                {
                    var entity = new QStandardContainment
                    {
                        ParentStandardIdentifier = e.AggregateIdentifier,
                        ChildStandardIdentifier = id,
                    };

                    db.Entry(entity).State = EntityState.Deleted;
                }

                db.SaveChanges();
            });
        }

        public void Update(StandardContentModified e) => Update(e, x =>
        {
            SetEntityContent(x, (StandardState)e.AggregateState);
        });

        public void Update(StandardOrganizationAdded e)
        {
            var state = (StandardState)e.AggregateState;
            var orgId = state.GetGuidValue(StandardField.OrganizationIdentifier).Value;

            DbContext(db =>
            {
                foreach (var id in e.OrganizationIds)
                    db.QStandardOrganizations.Add(new QStandardOrganization
                    {
                        StandardIdentifier = e.AggregateIdentifier,
                        ConnectedOrganizationIdentifier = id,
                        OrganizationIdentifier = orgId
                    });
                db.SaveChanges();
            });
        }

        public void Update(StandardOrganizationRemoved e)
        {
            DbContext(db =>
            {
                foreach (var id in e.OrganizationIds)
                {
                    var entity = new QStandardOrganization
                    {
                        StandardIdentifier = e.AggregateIdentifier,
                        ConnectedOrganizationIdentifier = id
                    };

                    db.Entry(entity).State = EntityState.Deleted;
                }

                db.SaveChanges();
            });
        }

        public void Update(StandardAchievementAdded e)
        {
            var state = (StandardState)e.AggregateState;
            var orgId = state.GetGuidValue(StandardField.OrganizationIdentifier).Value;

            DbContext(db =>
            {
                foreach (var id in e.AchievementIds)
                    db.QStandardAchievements.Add(new QStandardAchievement
                    {
                        StandardIdentifier = e.AggregateIdentifier,
                        AchievementIdentifier = id,
                        OrganizationIdentifier = orgId
                    });
                db.SaveChanges();
            });
        }

        public void Update(StandardAchievementRemoved e)
        {
            DbContext(db =>
            {
                foreach (var id in e.AchievementIds)
                {
                    var entity = new QStandardAchievement
                    {
                        StandardIdentifier = e.AggregateIdentifier,
                        AchievementIdentifier = id
                    };

                    db.Entry(entity).State = EntityState.Deleted;
                }

                db.SaveChanges();
            });
        }

        public void Update(StandardGroupAdded e)
        {
            var state = (StandardState)e.AggregateState;
            var orgId = state.GetGuidValue(StandardField.OrganizationIdentifier).Value;

            DbContext(db =>
            {
                foreach (var g in e.Groups)
                    db.QStandardGroups.Add(new QStandardGroup
                    {
                        StandardIdentifier = e.AggregateIdentifier,
                        GroupIdentifier = g.GroupId,
                        OrganizationIdentifier = orgId,
                        Assigned = g.Assigned ?? e.ChangeTime
                    });
                db.SaveChanges();
            });
        }

        public void Update(StandardGroupRemoved e)
        {
            DbContext(db =>
            {
                foreach (var id in e.GroupIds)
                {
                    var entity = new QStandardGroup
                    {
                        StandardIdentifier = e.AggregateIdentifier,
                        GroupIdentifier = id
                    };

                    db.Entry(entity).State = EntityState.Deleted;
                }

                db.SaveChanges();
            });
        }

        public void Update(StandardFieldTextModified e) => Update(e, x =>
        {
            SetEntityField(x, (StandardState)e.AggregateState, e.Field);
        });

        public void Update(StandardFieldDateOffsetModified e) => Update(e, x =>
        {
            SetEntityField(x, (StandardState)e.AggregateState, e.Field);
        });

        public void Update(StandardFieldBoolModified e) => Update(e, x =>
        {
            SetEntityField(x, (StandardState)e.AggregateState, e.Field);
        });

        public void Update(StandardFieldIntModified e) => Update(e, x =>
        {
            SetEntityField(x, (StandardState)e.AggregateState, e.Field);
        });

        public void Update(StandardFieldDecimalModified e) => Update(e, x =>
        {
            SetEntityField(x, (StandardState)e.AggregateState, e.Field);
        });

        public void Update(StandardFieldGuidModified e) => Update(e, x =>
        {
            SetEntityField(x, (StandardState)e.AggregateState, e.Field);
        });

        public void Update(StandardFieldsModified e) => Update(e, x =>
        {
            var state = (StandardState)e.AggregateState;
            foreach (var field in e.Fields.Keys)
                SetEntityField(x, state, field);
        });

        private void Update(Change e, Action<QStandard> action)
        {
            DbContext(db =>
            {
                var entity = _isMaintenanceMode
                    ? db.QStandards.Find(e.AggregateIdentifier)
                    : db.QStandards.FirstOrDefault(x => x.StandardIdentifier == e.AggregateIdentifier);

                if (entity == null)
                    return;

                action(entity);

                SetTimestamp(entity, e);

                db.SaveChanges();
            });
        }

        private static void SetTimestamp(QStandard entity, Change e)
        {
            SetTimestamp(entity, (StandardState)e.AggregateState);
        }

        private static void SetTimestamp(QStandard entity, StandardState state)
        {
            entity.Created = state.GetDateOffsetValue(StandardField.Created).Value;
            entity.CreatedBy = state.GetGuidValue(StandardField.CreatedBy).Value;
            entity.Modified = state.GetDateOffsetValue(StandardField.Modified).Value;
            entity.ModifiedBy = state.GetGuidValue(StandardField.ModifiedBy).Value;
        }

        private static void SetEntityContent(QStandard entity, StandardState state)
        {
            entity.ContentDescription = state.Content.Description.Text.Default;
            entity.ContentSummary = state.Content.Summary.Text.Default;
            entity.ContentTitle = state.Content.Title.Text.Default;
        }

        private static void SetEntityField(QStandard entity, StandardState state, StandardField field)
        {
            switch (field)
            {
                // string

                case StandardField.AuthorName:
                    entity.AuthorName = state.GetTextValue(field);
                    break;
                case StandardField.CanvasIdentifier:
                    entity.CanvasIdentifier = state.GetTextValue(field);
                    break;
                case StandardField.Code:
                    entity.Code = state.GetTextValue(field);
                    break;
                case StandardField.CompetencyScoreCalculationMethod:
                    entity.CompetencyScoreCalculationMethod = state.GetTextValue(field);
                    break;
                case StandardField.CompetencyScoreSummarizationMethod:
                    entity.CompetencyScoreSummarizationMethod = state.GetTextValue(field);
                    break;
                case StandardField.ContentName:
                    entity.ContentName = state.GetTextValue(field);
                    break;
                case StandardField.ContentSettings:
                    entity.ContentSettings = state.GetTextValue(field);
                    break;
                case StandardField.CreditIdentifier:
                    entity.CreditIdentifier = state.GetTextValue(field);
                    break;
                case StandardField.DocumentType:
                    entity.DocumentType = state.GetTextValue(field);
                    break;
                case StandardField.Icon:
                    entity.Icon = state.GetTextValue(field);
                    break;
                case StandardField.Language:
                    entity.Language = state.GetTextValue(field);
                    break;
                case StandardField.LevelCode:
                    entity.LevelCode = state.GetTextValue(field);
                    break;
                case StandardField.LevelType:
                    entity.LevelType = state.GetTextValue(field);
                    break;
                case StandardField.MajorVersion:
                    entity.MajorVersion = state.GetTextValue(field);
                    break;
                case StandardField.MinorVersion:
                    entity.MinorVersion = state.GetTextValue(field);
                    break;
                case StandardField.SourceDescriptor:
                    entity.SourceDescriptor = state.GetTextValue(field);
                    break;
                case StandardField.StandardAlias:
                    entity.StandardAlias = state.GetTextValue(field);
                    break;
                case StandardField.StandardLabel:
                    entity.StandardLabel = state.GetTextValue(field);
                    break;
                case StandardField.StandardPrivacyScope:
                    entity.StandardPrivacyScope = state.GetTextValue(field);
                    break;
                case StandardField.StandardStatus:
                    entity.StandardStatus = state.GetTextValue(field);
                    break;
                case StandardField.StandardTier:
                    entity.StandardTier = state.GetTextValue(field);
                    break;
                case StandardField.StandardType:
                    entity.StandardType = state.GetTextValue(field);
                    break;
                case StandardField.Tags:
                    entity.Tags = state.GetTextValue(field);
                    break;
                case StandardField.StandardHook:
                    entity.StandardHook = state.GetTextValue(field);
                    break;

                // DateTimeOffset

                case StandardField.AuthorDate:
                    entity.AuthorDate = state.GetDateOffsetValue(field);
                    break;
                case StandardField.Created:
                    entity.Created = state.GetDateOffsetValue(field).Value;
                    break;
                case StandardField.DatePosted:
                    entity.DatePosted = state.GetDateOffsetValue(field);
                    break;
                case StandardField.Modified:
                    entity.Modified = state.GetDateOffsetValue(field).Value;
                    break;
                case StandardField.StandardStatusLastUpdateTime:
                    entity.StandardStatusLastUpdateTime = state.GetDateOffsetValue(field);
                    break;
                case StandardField.UtcPublished:
                    entity.UtcPublished = state.GetDateOffsetValue(field);
                    break;

                // bool

                case StandardField.IsCertificateEnabled:
                    entity.IsCertificateEnabled = state.GetBoolValue(field).Value;
                    break;
                case StandardField.IsFeedbackEnabled:
                    entity.IsFeedbackEnabled = state.GetBoolValue(field).Value;
                    break;
                case StandardField.IsHidden:
                    entity.IsHidden = state.GetBoolValue(field).Value;
                    break;
                case StandardField.IsLocked:
                    entity.IsLocked = state.GetBoolValue(field).Value;
                    break;
                case StandardField.IsPractical:
                    entity.IsPractical = state.GetBoolValue(field).Value;
                    break;
                case StandardField.IsPublished:
                    entity.IsPublished = state.GetBoolValue(field).Value;
                    break;
                case StandardField.IsTemplate:
                    entity.IsTemplate = state.GetBoolValue(field).Value;
                    break;
                case StandardField.IsTheory:
                    entity.IsTheory = state.GetBoolValue(field).Value;
                    break;

                // int

                case StandardField.AssetNumber:
                    entity.AssetNumber = state.GetIntValue(field).Value;
                    break;
                case StandardField.CalculationArgument:
                    entity.CalculationArgument = state.GetIntValue(field);
                    break;
                case StandardField.Sequence:
                    entity.Sequence = state.GetIntValue(field).Value;
                    break;

                // decimal

                case StandardField.CertificationHoursPercentCore:
                    entity.CertificationHoursPercentCore = state.GetDecimalValue(field);
                    break;
                case StandardField.CertificationHoursPercentNonCore:
                    entity.CertificationHoursPercentNonCore = state.GetDecimalValue(field);
                    break;
                case StandardField.CreditHours:
                    entity.CreditHours = state.GetDecimalValue(field);
                    break;
                case StandardField.MasteryPoints:
                    entity.MasteryPoints = state.GetDecimalValue(field);
                    break;
                case StandardField.PassingScore:
                    entity.PassingScore = state.GetDecimalValue(field);
                    break;
                case StandardField.PointsPossible:
                    entity.PointsPossible = state.GetDecimalValue(field);
                    break;

                // Guid

                case StandardField.BankIdentifier:
                    entity.BankIdentifier = state.GetGuidValue(field);
                    break;
                case StandardField.BankSetIdentifier:
                    entity.BankSetIdentifier = state.GetGuidValue(field);
                    break;
                case StandardField.CategoryItemIdentifier:
                    entity.CategoryItemIdentifier = state.GetGuidValue(field);
                    break;
                case StandardField.CreatedBy:
                    entity.CreatedBy = state.GetGuidValue(field).Value;
                    break;
                case StandardField.DepartmentGroupIdentifier:
                    entity.DepartmentGroupIdentifier = state.GetGuidValue(field);
                    break;
                case StandardField.IndustryItemIdentifier:
                    entity.IndustryItemIdentifier = state.GetGuidValue(field);
                    break;
                case StandardField.ModifiedBy:
                    entity.ModifiedBy = state.GetGuidValue(field).Value;
                    break;
                case StandardField.OrganizationIdentifier:
                    entity.OrganizationIdentifier = state.GetGuidValue(field).Value;
                    break;
                case StandardField.ParentStandardIdentifier:
                    entity.ParentStandardIdentifier = state.GetGuidValue(field);
                    break;
                case StandardField.StandardStatusLastUpdateUser:
                    entity.StandardStatusLastUpdateUser = state.GetGuidValue(field);
                    break;

                default:
                    throw new ArgumentException($"Unsupported standard field: {field}");
            }
        }

        #region Context

        private bool _isMaintenanceMode = false;
        private MaintenanceContext _maintenanceContext = null;

        /// <summary>
        /// Used for bulk inserts in InSite.Maintenance
        /// </summary>
        public void EnableMaintenanceMode()
        {
            if (_isMaintenanceMode)
                return;

            _maintenanceContext = new MaintenanceContext();
            _isMaintenanceMode = true;
        }

        public void FlushMaintenanceContext()
        {
            if (!_isMaintenanceMode)
                return;

            _maintenanceContext.FlushChanges();
            _maintenanceContext.Dispose();
            _maintenanceContext = new MaintenanceContext();

        }

        public void DisableMaintenanceMode()
        {
            if (!_isMaintenanceMode)
                return;

            _maintenanceContext.FlushChanges();
            _maintenanceContext.Dispose();
            _maintenanceContext = null;
            _isMaintenanceMode = false;
        }

        private void DbContext(Action<InternalDbContext> action)
        {
            if (_isMaintenanceMode)
            {
                action(_maintenanceContext);
            }
            else
            {
                using (var db = new InternalDbContext())
                    action(db);
            }
        }

        private class MaintenanceContext : InternalDbContext
        {
            public MaintenanceContext()
                : base(false, false)
            {
                base.EnablePrepareToSaveChanges = false;
                base.Configuration.AutoDetectChangesEnabled = false;
                base.Configuration.ValidateOnSaveEnabled = false;
            }

            public override int SaveChanges()
            {
                return 0;
            }

            public int FlushChanges()
            {
                return base.SaveChanges();
            }
        }

        #endregion
    }
}
