using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    [Serializable]
    public class JournalState : AggregateState
    {
        public Guid Identifier { get; set; }
        public Guid JournalSetup { get; set; }
        public Guid User { get; set; }
        public List<Experience> Experiences { get; set; } = new List<Experience>();
        public List<Comment> Comments { get; set; } = new List<Comment>();

        public Experience FindExperience(Guid experience)
            => Experiences.Find(x => x.Identifier == experience);

        public Comment FindComment(Guid comment)
            => Comments.Find(x => x.Identifier == comment);

        public bool ShouldSerializeExperiences() => Experiences.Count > 0;
        public bool ShouldSerializeComments() => Comments.Count > 0;

        public void When(CommentAdded e)
        {
            Comments.Add(new Comment
            {
                Identifier = e.Comment,
                Author = e.Author,
                Subject = e.Subject,
                SubjectType = e.SubjectType,
                Text = e.Text,
                Posted = e.Posted,
                IsPrivate = e.IsPrivate
            });
        }

        public void When(CommentChanged e)
        {
            var comment = Comments.Find(x => x.Identifier == e.Comment);
            if (comment == null)
                throw new ArgumentException($"Comment does not exist {e.Comment}");

            comment.Text = e.Text;
            comment.Revised = e.Revised;
            comment.IsPrivate = e.IsPrivate;
        }

        public void When(CommentDeleted e)
        {
            var comment = Comments.Find(x => x.Identifier == e.Comment);
            if (comment == null)
                throw new ArgumentException($"Comment does not exist {e.Comment}");

            Comments.Remove(comment);
        }

        public void When(ExperienceAdded e)
        {
            Experiences.Add(new Experience
            {
                Identifier = e.Experience,
                Sequence = Experiences.Count > 0
                    ? Experiences.Max(x => x.Sequence) + 1
                    : 1
            });
        }

        public void When(ExperienceCompetencyAdded e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.Competencies.Add(new ExperienceCompetency
                {
                    Competency = e.Competency,
                    Hours = e.Hours,
                });
            });
        }

        public void When(ExperienceCompetencyChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                var competency = experience.Competencies.Find(x => x.Competency == e.Competency);
                if (competency == null)
                    throw new ArgumentException($"Competency {e.Competency} does not exist");

                competency.Hours = e.Hours;
            });
        }

        public void When(ExperienceCompetencyDeleted e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                var competency = experience.Competencies.Find(x => x.Competency == e.Competency);
                if (competency == null)
                    throw new ArgumentException($"Competency {e.Competency} does not exist");

                experience.Competencies.Remove(competency);
            });
        }

        public void When(ExperienceCompetencySatisfactionLevelChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                var competency = experience.Competencies.Find(x => x.Competency == e.Competency);
                if (competency == null)
                    throw new ArgumentException($"Competency {e.Competency} does not exist");

                competency.SatisfactionLevel = e.SatisfactionLevel;
            });
        }

        public void When(ExperienceCompetencySkillRatingChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                var competency = experience.Competencies.Find(x => x.Competency == e.Competency);
                if (competency == null)
                    throw new ArgumentException($"Competency {e.Competency} does not exist");

                competency.SkillRating = e.SkillRating;
            });
        }

        public void When(ExperienceDeleted e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                Experiences.Remove(experience);
            });

            Comments.RemoveAll(x => x.Subject == e.Experience);
        }

        public void When(ExperienceCompletedChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.Completed = e.Completed;
            });
        }

        public void When(ExperienceEmployerChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.Employer = e.Employer;
            });
        }

        public void When(ExperienceEvidenceChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.Evidence = e.Evidence;
            });
        }

        public void When(ExperienceHoursChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.Hours = e.Hours;
            });
        }

        public void When(ExperienceInstructorChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.Instructor = e.Instructor;
            });
        }

        public void When(ExperienceSupervisorChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.Supervisor = e.Supervisor;
            });
        }

        public void When(ExperienceTimeChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.Started = e.Started;
                experience.Stopped = e.Stopped;
            });
        }

        public void When(ExperienceTrainingChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.TrainingLevel = e.Level;
                experience.TrainingLocation = e.Location;
                experience.TrainingProvider = e.Provider;
                experience.TrainingCourseTitle = e.CourseTitle;
                experience.TrainingComment = e.Comment;
                experience.TrainingType = e.Type;
            });
        }

        public void When(ExperienceValidated e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.Validator = e.Validator;
                experience.Validated = e.Validated;
                experience.SkillRating = e.SkillRating;
            });
        }

        public void When(ExperienceMediaEvidenceChanged e)
        {
            ProcessExperience(e.Experience, (experience) =>
            {
                experience.MediaEvidenceName = e.Name;
                experience.MediaEvidenceType = e.Type;
                experience.MediaEvidenceFileIdentifier = e.FileIdentifier;
            });
        }

        public void When(ExperienceCapturedEvidenceChanged e)
        {
            // Obsolete
        }

        public void When(JournalCreated e)
        {
            JournalSetup = e.JournalSetup;
            User = e.User;
        }

        public void When(JournalDeleted _)
        {
        }

        private void ProcessExperience(Guid experienceIdentifier, Action<Experience> action)
        {
            var experience = Experiences.Find(x => x.Identifier == experienceIdentifier);
            if (experience == null)
                throw new ArgumentException($"Experience does not exist {experienceIdentifier}");

            action(experience);
        }
    }
}
