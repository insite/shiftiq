using System;
using System.Collections.Generic;
using System.ComponentModel;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Attempts;

using Shift.Constant;

namespace InSite.Application.Attempts.Write
{
    public class AnalyzeAttempt : Command
    {
        [DefaultValue(true)]
        public bool AllowTakeAttendance { get; set; } = true;

        public AnalyzeAttempt(Guid aggregate, bool allowTakeAttendance)
        {
            AggregateIdentifier = aggregate;
            AllowTakeAttendance = allowTakeAttendance;
        }
    }

    public class AnswerComposedQuestion : Command
    {
        public Guid Question { get; set; }
        public string Answer { get; set; }

        public AnswerComposedQuestion(Guid aggregate, Guid question, string answer)
        {
            AggregateIdentifier = aggregate;
            Question = question;
            Answer = answer;
        }
    }

    public class AnswerMatchingQuestion : Command
    {
        public Guid Question { get; set; }
        public IDictionary<int, string> Matches { get; set; }

        public AnswerMatchingQuestion(Guid aggregate, Guid question, IDictionary<int, string> matches)
        {
            AggregateIdentifier = aggregate;
            Question = question;
            Matches = matches;
        }
    }

    public class AnswerMultipleChoiceQuestion : Command
    {
        public Guid Question { get; set; }
        public int Option { get; set; }

        public AnswerMultipleChoiceQuestion(Guid aggregate, Guid question, int option)
        {
            AggregateIdentifier = aggregate;
            Question = question;
            Option = option;
        }
    }

    public class AnswerMultipleCorrectQuestion : Command
    {
        public Guid Question { get; set; }
        public int[] Options { get; set; }

        public AnswerMultipleCorrectQuestion(Guid aggregate, Guid question, int[] options)
        {
            AggregateIdentifier = aggregate;
            Question = question;
            Options = options;
        }
    }

    public class AnswerTrueOrFalseQuestion : Command
    {
        public Guid Question { get; set; }
        public int Option { get; set; }

        public AnswerTrueOrFalseQuestion(Guid aggregate, Guid question, int option)
        {
            AggregateIdentifier = aggregate;
            Question = question;
            Option = option;
        }
    }

    public class AnswerBooleanTableQuestion : Command
    {
        public Guid Question { get; set; }
        public IDictionary<int, bool> Options { get; set; }

        public AnswerBooleanTableQuestion(Guid aggregate, Guid question, IDictionary<int, bool> options)
        {
            AggregateIdentifier = aggregate;
            Question = question;
            Options = options;
        }
    }

    public class AnswerHotspotQuestion : Command
    {
        public Guid Question { get; set; }
        public AttemptHotspotPinAnswer[] Pins { get; set; }

        public AnswerHotspotQuestion(Guid aggregate, Guid question, AttemptHotspotPinAnswer[] pins)
        {
            AggregateIdentifier = aggregate;
            Question = question;
            Pins = pins;
        }
    }

    public class AnswerOrderingQuestion : Command
    {
        public Guid Question { get; set; }
        public int[] OptionsOrder { get; set; }

        public AnswerOrderingQuestion(Guid aggregate, Guid question, int[] optionsOrder)
        {
            AggregateIdentifier = aggregate;
            Question = question;
            OptionsOrder = optionsOrder;
        }
    }

    public class AuthorComment : Command
    {
        public Guid Question { get; }
        public string Comment { get; }

        public AuthorComment(Guid aggregate, Guid question, string comment)
        {
            AggregateIdentifier = aggregate;
            Question = question;
            Comment = comment;
        }
    }

    public class GradeAttempt : Command
    {
        public GradeAttempt(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }

    public class FixAttempt : Command
    {
        public int? Points { get; set; }
        public int? Score { get; set; }
        public bool? IsPassing { get; set; }
        public Guid? Registration { get; set; }

        public FixAttempt(Guid aggregate, int? points, int? score, bool? isPassing, Guid? registration)
        {
            AggregateIdentifier = aggregate;
            Points = points;
            Score = score;
            IsPassing = isPassing;
            Registration = registration;
        }
    }

    public class ImportAttempt : Command
    {
        public Guid Tenant { get; set; }
        public AnswerHandle[] Answers { get; set; }
        public DateTimeOffset? Started { get; set; }
        public DateTimeOffset? Completed { get; set; }
        public string Tag { get; set; }
        public Guid Bank { get; set; }
        public Guid Form { get; set; }
        public Guid Candidate { get; set; }
        public Guid? Registration { get; set; }
        public bool IsAttended { get; set; }
        public string Language { get; set; }

        public ImportAttempt(
            Guid aggregate,
            Guid tenant,
            AnswerHandle[] answers,
            DateTimeOffset? started,
            DateTimeOffset? completed,
            string tag,
            Guid bank,
            Guid form,
            Guid candidate,
            Guid? registration,
            bool isAttended,
            string language
            )
        {
            AggregateIdentifier = aggregate;
            Tenant = tenant;
            Answers = answers;
            Started = started;
            Completed = completed;
            Tag = tag;
            Bank = bank;
            Form = form;
            Candidate = candidate;
            Registration = registration;
            IsAttended = isAttended;
            Language = language;
        }
    }

    public class PingAttempt : Command
    {
        public PingAttempt(Guid aggregate)
        {
            AggregateIdentifier = aggregate;
        }
    }

    public class ResumeAttempt : Command
    {
        public int? PingInterval { get; set; }

        public ResumeAttempt(Guid aggregate, int? pingInterval)
        {
            AggregateIdentifier = aggregate;
            PingInterval = pingInterval;
        }
    }

    public class StartAttempt : Command
    {
        public Guid OrganizationIdentifier { get; set; }
        public Guid BankIdentifier { get; set; }
        public Guid FormIdentifier { get; set; }
        public Guid AssessorUserIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }
        public string UserAgent { get; set; }

        public AttemptConfiguration Configuration { get; set; }
        public AttemptSection[] Sections { get; set; }
        public AttemptQuestion[] Questions { get; set; }

        public StartAttempt(Guid attempt, Guid organizationId, Guid bankId, Guid formId, Guid assessorId, Guid learnerId, Guid? registrationId, string userAgent, AttemptConfiguration config, AttemptSection[] sections, AttemptQuestion[] questions)
        {
            AggregateIdentifier = attempt;
            OrganizationIdentifier = organizationId;
            BankIdentifier = bankId;
            FormIdentifier = formId;
            AssessorUserIdentifier = assessorId;
            LearnerUserIdentifier = learnerId;
            RegistrationIdentifier = registrationId;
            UserAgent = userAgent;

            Configuration = config;
            Sections = sections;
            Questions = questions;
        }
    }

    public class StartComposedQuestionAttempt : Command
    {
        public Guid Question { get; set; }

        public StartComposedQuestionAttempt(Guid aggregate, Guid question)
        {
            AggregateIdentifier = aggregate;
            Question = question;
        }
    }

    public class TagAttempt : Command
    {
        public string Tag { get; set; }

        public TagAttempt(Guid aggregate, string tag)
        {
            AggregateIdentifier = aggregate;
            Tag = tag;
        }
    }

    public class VoidAttempt : Command
    {
        public string Reason { get; set; }

        public VoidAttempt(Guid aggregate, string reason)
        {
            AggregateIdentifier = aggregate;
            Reason = reason;
        }
    }

    public class VoidQuestion : Command
    {
        public Guid Question { get; set; }

        public VoidQuestion(Guid aggregate, Guid question)
        {
            AggregateIdentifier = aggregate;
            Question = question;
        }
    }

    public class RegradeQuestion : Command
    {
        public Guid Form { get; set; }
        public Guid Question { get; set; }
        public List<OldOption> OldOptions { get; set; }
        public RegradeOption RegradeOption { get; set; }

        public RegradeQuestion(Guid attempt, Guid form, Guid question, List<OldOption> oldOptions, RegradeOption regradeOption)
        {
            AggregateIdentifier = attempt;
            Form = form;
            Question = question;
            OldOptions = oldOptions;
            RegradeOption = regradeOption;
        }
    }

    public class ChangeAttempGradedDate : Command
    {
        public DateTimeOffset Completed { get; set; }

        public ChangeAttempGradedDate(Guid aggregate, DateTimeOffset completed)
        {
            AggregateIdentifier = aggregate;
            Completed = completed;
        }
    }

    public class SwitchAttemptSection : Command
    {
        public int NextSectionIndex { get; set; }

        public SwitchAttemptSection(Guid aggregate, int sectionIndex)
        {
            AggregateIdentifier = aggregate;
            NextSectionIndex = sectionIndex;
        }
    }

    public class SwitchAttemptQuestion : Command
    {
        public int NextQuestionIndex { get; set; }

        public SwitchAttemptQuestion(Guid aggregate, int questionIndex)
        {
            AggregateIdentifier = aggregate;
            NextQuestionIndex = questionIndex;
        }
    }

    public class ChangeAttempGradingAssessor : Command
    {
        public Guid? GradingAssessor { get; set; }

        public ChangeAttempGradingAssessor(Guid aggregate, Guid? gradingAssessor)
        {
            AggregateIdentifier = aggregate;
            GradingAssessor = gradingAssessor;
        }
    }

    public class UpdateAttemptRubricPoints : Command
    {
        public Guid RubricId { get; }
        public decimal RubricPoints { get; }

        public UpdateAttemptRubricPoints(Guid attemptId, Guid rubricId, decimal rubricPoints)
        {
            AggregateIdentifier = attemptId;
            RubricId = rubricId;
            RubricPoints = rubricPoints;
        }
    }

    public class ChangeAttemptQuestionRubric : Command
    {
        public Guid QuestionId { get; }
        public AttemptQuestionRubric Rubric { get; }

        public ChangeAttemptQuestionRubric(Guid attemptId, Guid questionId, AttemptQuestionRubric rubric)
        {
            AggregateIdentifier = attemptId;
            QuestionId = questionId;
            Rubric = rubric;
        }
    }

    public class InitAttemptQuestionRubric : Command
    {
        public Guid QuestionId { get; }
        public AttemptQuestionRubric Rubric { get; }

        public InitAttemptQuestionRubric(Guid attemptId, Guid questionId, AttemptQuestionRubric rubric)
        {
            AggregateIdentifier = attemptId;
            QuestionId = questionId;
            Rubric = rubric;
        }
    }
}
