using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Records
{
    [Serializable]
    public class JournalSetupState : AggregateState
    {
        public static class ContentLabels
        {
            public const string Title = "Title";
            public const string Instructions = "Instructions";
        };

        public Guid Identifier { get; set; }
        public Guid Tenant { get; set; }
        public Guid? Framework { get; set; }
        public Guid? Achievement { get; set; }
        public Guid? Event { get; set; }
        public string Name { get; set; }
        public bool IsValidationRequired { get; set; }
        public bool? AllowRecordDownload { get; set; }
        public Guid? ValidatorMessage { get; set; }
        public Guid? LearnerMessage { get; set; }
        public Guid? LearnerAddedMessage { get; set; }
        public DateTimeOffset? JournalSetupLocked { get; set; }

        public Shift.Common.ContentContainer Content { get; set; }

        public List<JournalSetupField> Fields { get; set; } = new List<JournalSetupField>();
        public List<CompetencyRequirement> CompetencyRequirements { get; set; } = new List<CompetencyRequirement>();
        public List<JournalSetupUser> Users { get; set; } = new List<JournalSetupUser>();
        public List<JournalSetupGroup> Groups { get; set; } = new List<JournalSetupGroup>();
        public List<JournalSetupAreaRequirement> AreaRequirements { get; set; } = new List<JournalSetupAreaRequirement>();

        public JournalSetupField FindField(Guid field)
            => Fields.Find(x => x.Identifier == field);

        public JournalSetupField FindField(JournalSetupFieldType type)
            => Fields.Find(x => x.Type == type);

        public CompetencyRequirement FindCompetencyRequirement(Guid competency)
            => CompetencyRequirements.Find(x => x.Competency == competency);

        public JournalSetupAreaRequirement FindAreaRequirement(Guid area)
            => AreaRequirements.Find(x => x.Area == area);

        public bool ContainsUser(Guid user, JournalSetupUserRole role)
            => Users.Any(x => x.Identifier == user && x.Role == role);

        public bool ContainsGroup(Guid group)
            => Groups.Any(x => x.Identifier == group);

        public bool ShouldSerializeContent() => Content != null && !Content.IsEmpty;
        public bool ShouldSerializeFields() => Fields.Count > 0;
        public bool ShouldSerializeCompetencyRequirements() => CompetencyRequirements.Count > 0;
        public bool ShouldSerializeContentLabel() => false;
        public bool ShouldSerializeIsValidationRequired() => IsValidationRequired == true;
        public bool ShouldSerializeValidatorMessage() => ValidatorMessage.HasValue;
        public bool ShouldSerializeLearnerMessage() => LearnerMessage.HasValue;
        public bool ShouldSerializeLearnerAddedMessage() => LearnerAddedMessage.HasValue;
        public bool ShouldSerializeAreaRequirements() => AreaRequirements.Count > 0;

        public void When(CompetencyRequirementAdded e)
        {
            CompetencyRequirements.Add(new CompetencyRequirement
            {
                Competency = e.Competency,
                Hours = e.Hours,
                JournalItems = e.JournalItems,
                SkillRating = e.SkillRating,
                IncludeHoursToArea = e.IncludeHoursToArea ?? (e.Hours ?? 0) > 0
            });
        }

        public void When(CompetencyRequirementChanged e)
        {
            var competency = CompetencyRequirements.Find(x => x.Competency == e.Competency);
            if (competency == null)
                throw new ArgumentException($"Competency does not exist {e.Competency}");

            competency.Hours = e.Hours;
            competency.JournalItems = e.JournalItems;
            competency.SkillRating = e.SkillRating;
            competency.IncludeHoursToArea = e.IncludeHoursToArea ?? (e.Hours ?? 0) > 0;
        }

        public void When(CompetencyRequirementDeleted e)
        {
            var competency = CompetencyRequirements.Find(x => x.Competency == e.Competency);
            if (competency == null)
                throw new ArgumentException($"Competency does not exist {e.Competency}");

            CompetencyRequirements.Remove(competency);
        }

        public void When(JournalSetupAchievementChanged e)
        {
            Achievement = e.Achievement;
        }

        public void When(JournalSetupAreaHoursModified e)
        {
            var requirement = FindAreaRequirement(e.Area);
            if (requirement == null)
                AreaRequirements.Add(requirement = new JournalSetupAreaRequirement
                {
                    Area = e.Area
                });

            requirement.Hours = e.Hours;
        }

        public void When(JournalSetupContentChanged e)
        {
            Content = e.Content;
        }

        public void When(JournalSetupCreated e)
        {
            Identifier = e.AggregateIdentifier;
            Tenant = e.Tenant;
            Name = e.Name;
        }

        public void When(JournalSetupDeleted _)
        {
        }

        public void When(JournalSetupEventChanged e)
        {
            Event = e.Event;
        }

        public void When(JournalSetupFieldAdded e)
        {
            Fields.Add(new JournalSetupField
            {
                Identifier = e.Field,
                Type = e.Type,
                Sequence = e.Sequence,
                IsRequired = e.IsRequired
            });
        }

        public void When(JournalSetupFieldChanged e)
        {
            var field = Fields.Find(x => x.Identifier == e.Field);
            if (field == null)
                throw new ArgumentException($"Field does not exist {e.Field}");

            field.IsRequired = e.IsRequired;
        }

        public void When(JournalSetupFieldContentChanged e)
        {
            var field = Fields.Find(x => x.Identifier == e.Field);
            if (field == null)
                throw new ArgumentException($"Field does not exist {e.Field}");

            field.Content = e.Content;
        }

        public void When(JournalSetupFieldDeleted e)
        {
            var field = Fields.Find(x => x.Identifier == e.Field);
            if (field == null)
                throw new ArgumentException($"Field does not exist {e.Field}");

            Fields.Remove(field);
        }

        public void When(JournalSetupFieldsReordered e)
        {
            foreach (var (fieldId, sequence) in e.Fields)
            {
                var field = Fields.Find(x => x.Identifier == fieldId);
                if (field != null)
                    field.Sequence = sequence;
            }
        }

        public void When(JournalSetupFrameworkChanged e)
        {
            Framework = e.Framework;
        }

        public void When(JournalSetupIsValidationRequiredChanged e)
        {
            IsValidationRequired = e.IsValidationRequired;
        }

        public void When(LogbookDownloadAllowed e)
        {

        }

        public void When(LogbookDownloadDisallowed e)
        {

        }

        public void When(LockUnlockJournalSetupChanged e)
        {
            JournalSetupLocked = e.JournalSetupLocked;
        }

        public void When(JournalSetupMessagesChanged e)
        {
            ValidatorMessage = e.ValidatorMessage;
            LearnerMessage = e.LearnerMessage;
            LearnerAddedMessage = e.LearnerAddedMessage;
        }

        public void When(JournalSetupRenamed e)
        {
            Name = e.Name;
        }

        public void When(JournalSetupUserAdded e)
        {
            Users.Add(new JournalSetupUser { Identifier = e.User, Role = e.Role });
        }

        public void When(JournalSetupUserDeleted e)
        {
            var index = Users.FindIndex(x => x.Identifier == e.User && x.Role == e.Role);
            if (index < 0)
                throw new ArgumentException($"User with role '{e.Role}' does not exist {e.User}");

            Users.RemoveAt(index);
        }

        public void When(JournalSetupGroupCreated e)
        {
            Groups.Add(new JournalSetupGroup { Identifier = e.Group });
        }

        public void When(JournalSetupGroupRemoved e)
        {
            var index = Groups.FindIndex(x => x.Identifier == e.Group);
            if (index < 0)
                throw new ArgumentException($"Group does not exist {e.Group}");

            Groups.RemoveAt(index);
        }
    }
}
