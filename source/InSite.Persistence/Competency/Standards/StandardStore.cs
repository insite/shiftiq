using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common.Timeline.Commands;

using InSite.Application.Courses.Write;
using InSite.Application.Standards.Read;
using InSite.Application.Standards.Write;
using InSite.Application.StandardValidations.Write;
using InSite.Domain.Standards;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class StandardStore
    {
        #region Initialization

        private static Action<ICommand> _sendCommand;
        private static Action<IEnumerable<ICommand>> _sendCommands;

        public static void Initialize(Action<ICommand> sendCommand, Action<IEnumerable<ICommand>> sendCommands)
        {
            _sendCommand = sendCommand;
            _sendCommands = sendCommands;
        }

        #endregion

        #region Insert

        public static void Insert(QStandard entity, ContentContainer content = null)
        {
            CheckDepthOnInsert(null, entity);

            var hasContent = content != null;

            if (hasContent)
            {
                if (entity.ContentTitle.IsEmpty())
                    entity.ContentTitle = content.Title.Text.Default.IfNullOrEmpty("(Untitled)");

                if (entity.ContentDescription.IsEmpty())
                    entity.ContentDescription = content.Description.Text.Default.NullIfEmpty();

                if (entity.ContentSummary.IsEmpty())
                    entity.ContentSummary = content.Summary.Text.Default.NullIfEmpty();

                content.Title.Text.Default = null;
                content.Description.Text.Default = null;
                content.Summary.Text.Default = null;
            }
            else
            {
                if (entity.ContentTitle.IsEmpty())
                    entity.ContentTitle = "(Untitled)";
            }

            if (entity.StandardHook.IsEmpty())
                entity.StandardHook = GetIntegrationHook();

            var commands = StandardCommandCreator.Create(null, entity);

            if (hasContent)
            {
                var createCommand = (CreateStandard)commands[0];
                createCommand.Content.Set(content, ContentContainer.SetNullAction.None);
            }

            foreach (var command in commands)
            {
                command.OriginOrganization = entity.OrganizationIdentifier;
                command.OriginUser = entity.CreatedBy;
            }

            _sendCommands(commands);
        }

        #endregion

        #region Update

        public static void Update(QStandard standard)
        {
            CheckDepthOnUpdate(null, standard);

            QStandard oldEntity;
            using (var db = new InternalDbContext(false))
                oldEntity = db.QStandards.FirstOrDefault(x => x.StandardIdentifier == standard.StandardIdentifier);

            var commands = StandardCommandCreator.Create(oldEntity, standard);
            _sendCommands(commands);
        }

        public static QStandard Update(Guid id, Action<QStandard> change)
        {
            QStandard oldEntity = null, newEntity = null;

            using (var db = new InternalDbContext(false))
            {
                oldEntity = db.QStandards.FirstOrDefault(x => x.StandardIdentifier == id);
                newEntity = oldEntity.Clone();

                change(newEntity);

                CheckDepthOnUpdate(db, newEntity);
            }

            var commands = StandardCommandCreator.Create(oldEntity, newEntity);
            _sendCommands(commands);

            return newEntity;
        }

        public static List<QStandard> Update(Expression<Func<QStandard, bool>> filter, Action<QStandard> change)
        {
            var result = new List<QStandard>();
            var commands = new List<ICommand>();

            using (var db = new InternalDbContext(false))
            {
                var entities = db.QStandards.Where(filter).ToList();

                foreach (var oldEntity in entities)
                {
                    var newEntity = oldEntity.Clone();

                    change(newEntity);

                    CheckDepthOnUpdate(db, newEntity);

                    var entityCommands = StandardCommandCreator.Create(oldEntity, newEntity);
                    commands.AddRange(entityCommands);

                    result.Add(newEntity);
                }
            }

            _sendCommands(commands);

            return result;
        }

        #endregion

        #region Update (resequence)

        public static bool Resequence(Guid parentId)
        {
            List<ICommand> commands;
            using (var db = new InternalDbContext(false))
                commands = Resequence(parentId, null, db);

            _sendCommands(commands);

            return commands.IsNotEmpty();
        }

        private static List<ICommand> Resequence(Guid parentId, string[] subtypeOrder, InternalDbContext context)
        {
            var commands = new List<ICommand>();
            var unsortedAssets = context.QStandards
                .Where(x => x.ParentStandardIdentifier == parentId)
                .Select(x => new
                {
                    x.StandardIdentifier,
                    x.StandardType,
                    x.Sequence,
                    x.ContentTitle
                })
                .ToArray();

            var subtypeSequence = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (subtypeOrder != null)
            {
                for (var i = 0; i < subtypeOrder.Length; i++)
                {
                    var subtype = subtypeOrder[i];
                    if (!subtypeSequence.ContainsKey(subtype))
                        subtypeSequence.Add(subtype, i);
                }
            }

            var assets = unsortedAssets
                .OrderBy(x => subtypeSequence.ContainsKey(x.StandardType) ? subtypeSequence[x.StandardType] : subtypeSequence.Count)
                .ThenBy(x => x.Sequence)
                .ThenBy(x => x.ContentTitle)
                .ToArray();

            for (var i = 0; i < assets.Length; i++)
            {
                var asset = assets[i];
                var sequence = i + 1;

                if (asset.Sequence != sequence)
                    commands.Add(new ModifyStandardFieldInt(asset.StandardIdentifier, StandardField.Sequence, i + 1));
            }

            return commands;
        }

        #endregion

        #region Delete

        public static bool Delete(Guid standardId)
        {
            var commands = new List<ICommand>();

            using (var db = new InternalDbContext(false))
            {
                var hierarchyRealtionships = db.Database
                    .SqlQuery<StandardRelationshipInfo>("SELECT * FROM [standard].GetStandardDescendentRelationships(@StandardIdentifier,1,0,0)", new SqlParameter("@StandardIdentifier", standardId))
                    .ToArray();

                if (hierarchyRealtionships.Length == 0)
                    return false;

                var idFilter = hierarchyRealtionships
                    .Select(x => x.ChildStandardIdentifier)
                    .ToArray();

                // Generate commands
                var validations = db.QStandardValidations
                    .Where(x => idFilter.Contains(x.StandardIdentifier))
                    .Select(x => x.StandardValidationIdentifier)
                    .ToArray();
                foreach (var validationId in validations)
                    commands.Add(new RemoveStandardValidation(validationId));

                var toConnections = db.QStandardConnections
                    .Where(x => idFilter.Contains(x.ToStandardIdentifier))
                    .Select(x => new
                    {
                        x.FromStandardIdentifier,
                        x.ToStandardIdentifier,
                    })
                    .ToArray();
                foreach (var group in toConnections.GroupBy(x => x.FromStandardIdentifier))
                    commands.Add(new RemoveStandardConnection(group.Key, group.Select(x => x.ToStandardIdentifier).ToArray()));

                var toContainments = db.QStandardContainments
                    .Where(x => idFilter.Contains(x.ChildStandardIdentifier))
                    .Select(x => new
                    {
                        x.ParentStandardIdentifier,
                        x.ChildStandardIdentifier
                    })
                    .ToArray();
                foreach (var group in toContainments.GroupBy(x => x.ParentStandardIdentifier))
                    commands.Add(new RemoveStandardContainment(group.Key, group.Select(x => x.ChildStandardIdentifier).ToArray()));

                foreach (var item in hierarchyRealtionships.Where(x => x.ParentStandardIdentifier.HasValue).OrderByDescending(x => x.Depth))
                    commands.Add(new RemoveStandard(item.ChildStandardIdentifier));

                commands.Add(new RemoveStandard(standardId));

                RemoveRelatedEntities(db, idFilter, commands);
            }

            _sendCommands(commands);

            return true;
        }

        private static void RemoveRelatedEntities(InternalDbContext db, Guid[] idFilter, List<ICommand> commands)
        {
            var activityCompetencies = db.QActivityCompetencies
                .Where(x => idFilter.Contains(x.CompetencyStandardIdentifier))
                .Select(x => new { x.Activity.Module.Unit.CourseIdentifier, x.ActivityIdentifier, x.CompetencyStandardIdentifier })
                .ToList()
                .GroupBy(x => x.CourseIdentifier)
                .Select(a => new
                {
                    CourseId = a.Key,
                    Activities = a
                        .GroupBy(b => b.ActivityIdentifier)
                        .Select(b => new { ActivityId = b.Key, Competencies = b.Select(c => c.CompetencyStandardIdentifier).ToArray() })
                        .ToList()
                })
                .ToList();

            foreach (var course in activityCompetencies)
            {
                var removeCommands = course.Activities.Select(x => new RemoveCourseActivityCompetencies(course.CourseId, x.ActivityId, x.Competencies)).ToArray();

                commands.Add(new RunCommands(course.CourseId, removeCommands));
            }

            var departmentProfileCompetencies = db.DepartmentProfileCompetencies
                .Where(x => idFilter.Contains(x.CompetencyStandardIdentifier) || idFilter.Contains(x.ProfileStandardIdentifier))
                .ToArray();

            db.DepartmentProfileCompetencies.RemoveRange(departmentProfileCompetencies);

            db.SaveChanges();
        }

        #endregion

        #region Depth Constraint

        public const int MaxDepth = 6;

        public static readonly string DepthLimitErrorText = $"You have exceeded the allowed nesting depth. Please limit the structure of the standards to {MaxDepth} levels.";

        private static void CheckDepthOnInsert(InternalDbContext db, QStandard entity) =>
            CheckDepth(db, null, entity.ParentStandardIdentifier);

        private static void CheckDepthOnUpdate(InternalDbContext db, QStandard entity) =>
            CheckDepth(db, entity.StandardIdentifier, entity.ParentStandardIdentifier);

        private static void CheckDepth(InternalDbContext db, Guid? id, Guid? parentId)
        {
            if (!parentId.HasValue)
                return;

            var parentDepth = db == null
                ? StandardSearch.CalculateExpectedStandardDepth(parentId.Value, id)
                : StandardSearch.CalculateExpectedStandardDepth(db, parentId.Value, id);

            if (!id.HasValue)
                parentDepth += 1;

            if (parentDepth > MaxDepth)
                throw ApplicationError.Create(
                    $"The standard exceeds the depth limit of {MaxDepth} (Standard={id}, Parent={parentId.Value})");
        }

        public static bool IsDepthLimitException(ApplicationError ex) => ex.Message.StartsWith("The standard exceeds the depth limit");

        #endregion

        #region Helpers

        public static string GetIntegrationHook()
        {
            using (var db = new InternalDbContext())
            {
                for (var i = 0; i < 10; i++)
                {
                    var chars = RandomStringGenerator.Create(RandomStringType.Alphanumeric, 8);
                    var hook = chars.Substring(0, 4).ToUpper() + "-" + chars.Substring(4, 4).ToUpper();

                    if (!db.Standards.Any(x => x.StandardHook == hook))
                        return hook;
                }
            }

            throw new ApplicationError("Unable to generate a unique integration hook");
        }

        #endregion
    }
}
