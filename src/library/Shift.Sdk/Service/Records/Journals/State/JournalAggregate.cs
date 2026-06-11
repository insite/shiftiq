using System;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class JournalAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new JournalState();
        public JournalState Data => (JournalState)State;

        public void AddComment(Guid comment, Guid author, Guid subject, string subjectType, string text, DateTimeOffset posted, bool isPrivate)
        {
            Apply(new CommentAdded(comment, author, subject, subjectType, text, posted, isPrivate));
        }

        public void ChangeComment(Guid comment, string text, DateTimeOffset revised, bool isPrivate)
        {
            Apply(new CommentChanged(comment, text, revised, isPrivate));
        }

        public void DeleteComment(Guid comment)
        {
            Apply(new CommentDeleted(comment));
        }

        public void AddExperience(Guid experience)
        {
            Apply(new ExperienceAdded(experience));
        }

        public void AddExperienceCompetency(Guid experience, Guid competency, decimal? hours)
        {
            Apply(new ExperienceCompetencyAdded(experience, competency, hours));
        }

        public void ChangeExperienceCompetency(Guid experience, Guid competency, decimal? hours)
        {
            Apply(new ExperienceCompetencyChanged(experience, competency, hours));
        }

        public void ChangeExperienceCompetencySatisfactionLevel(Guid experience, Guid competency, ExperienceCompetencySatisfactionLevel satisfactionLevel)
        {
            Apply(new ExperienceCompetencySatisfactionLevelChanged(experience, competency, satisfactionLevel));
        }

        public void ChangeExperienceCompetencySkillRating(Guid experience, Guid competency, int? skillRating)
        {
            Apply(new ExperienceCompetencySkillRatingChanged(experience, competency, skillRating));
        }

        public void DeleteExperienceCompetency(Guid experience, Guid competency)
        {
            Apply(new ExperienceCompetencyDeleted(experience, competency));
        }

        public void DeleteExperience(Guid experience)
        {
            Apply(new ExperienceDeleted(experience));
        }

        public void ChangeExperienceCompleted(Guid experience, DateTime? completed)
        {
            Apply(new ExperienceCompletedChanged(experience, completed));
        }

        public void ChangeExperienceEmployer(Guid experience, string employer)
        {
            Apply(new ExperienceEmployerChanged(experience, employer));
        }

        public void ChangeExperienceEvidence(Guid experience, string evidence)
        {
            Apply(new ExperienceEvidenceChanged(experience, evidence));
        }

        public void ChangeExperienceHours(Guid experience, decimal? hours)
        {
            Apply(new ExperienceHoursChanged(experience, hours));
        }

        public void ChangeExperienceInstructor(Guid experience, string instructor)
        {
            Apply(new ExperienceInstructorChanged(experience, instructor));
        }

        public void ChangeExperienceSupervisor(Guid experience, string supervisor)
        {
            Apply(new ExperienceSupervisorChanged(experience, supervisor));
        }

        public void ChangeExperienceTime(Guid experience, DateTime? started, DateTime? stopped)
        {
            Apply(new ExperienceTimeChanged(experience, started, stopped));
        }

        public void ChangeExperienceTraining(Guid experience, string level, string location, string provider, string courseTitle, string comment, string type)
        {
            Apply(new ExperienceTrainingChanged(experience, level, location, provider, courseTitle, comment, type));
        }

        public void ChangeExperienceMediaEvidence(Guid experience, string name, string type, Guid? fileIdentifier)
        {
            Apply(new ExperienceMediaEvidenceChanged(experience, name, type, fileIdentifier));
        }

        public void ValidateExperience(Guid experience, Guid? supervisor, DateTimeOffset? validated, int? skillRating)
        {
            Apply(new ExperienceValidated(experience, supervisor, validated, skillRating));
        }

        public void Create(Guid journalSetup, Guid user)
        {
            Apply(new JournalCreated(journalSetup, user));
        }

        public void Delete()
        {
            Apply(new JournalDeleted());
        }
    }
}
