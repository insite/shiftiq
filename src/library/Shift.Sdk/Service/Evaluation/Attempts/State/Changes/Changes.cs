using System;
using System.Collections.Generic;
using System.ComponentModel;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Attempts
{
    [Serializable]
    public class AttemptHotspotPinAnswer
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }

        public AttemptHotspotPinAnswer(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }
    }

    [Serializable]
    public class AnswerHandle
    {
        public Guid Question { get; set; }
        public int[] Options { get; set; }
        public int? Answer { get; set; }

    }
    public class AttemptAnalyzed : Change
    {
        [DefaultValue(true)]
        public bool AllowTakeAttendance { get; set; } = true;

        public AttemptAnalyzed(bool allowTakeAttendance)
        {
            AllowTakeAttendance = allowTakeAttendance;
        }
    }

    public class AttemptGraded : Change
    {
        public string UserAgent { get; }

        public AttemptGraded(string userAgent)
        {
            UserAgent = userAgent;
        }
    }

    public class AttemptGradedDateChanged : Change
    {
        public DateTimeOffset Completed { get; set; }

        public AttemptGradedDateChanged(DateTimeOffset completed)
        {
            Completed = completed;
        }
    }

    public class AttemptFixed : Change
    {
        public int? Points { get; set; }
        public int? Score { get; set; }
        public bool? IsPassing { get; set; }
        public Guid? Registration { get; set; }

        public AttemptFixed(int? points, int? score, bool? isPassing, Guid? registration)
        {
            Points = points;
            Score = score;
            IsPassing = isPassing;
            Registration = registration;
        }
    }

    public class AttemptImported : Change
    {
        public Guid Tenant { get; set; }
        public AnswerHandle[] Answers { get; set; }
        public DateTimeOffset? Started { get; set; }
        public DateTimeOffset? Completed { get; set; }
        public string Tag { get; set; }
        public Guid Form { get; set; }
        public Guid Candidate { get; set; }
        public Guid? Registration { get; set; }
        public bool IsAttended { get; set; }
        public string Language { get; set; }

        public AttemptImported(
            Guid tenant,
            AnswerHandle[] answers,
            DateTimeOffset? started,
            DateTimeOffset? completed,
            string tag,
            Guid form,
            Guid candidate,
            Guid? registration,
            bool isAttended,
            string language
            )
        {
            Tenant = tenant;
            Answers = answers;
            Started = started;
            Completed = completed;
            Tag = tag;
            Form = form;
            Candidate = candidate;
            Registration = registration;
            IsAttended = isAttended;
            Language = language;
        }
    }

    public class AttemptPinged : Change
    {
        public AttemptPinged()
        {

        }
    }

    public class AttemptResumed : Change
    {
        public int? PingInterval { get; set; }

        public AttemptResumed(int? pingInterval)
        {
            PingInterval = pingInterval;
        }
    }

    public class AttemptTagged : Change
    {
        public string Tag { get; set; }

        public AttemptTagged(string tag)
        {
            Tag = tag;
        }
    }

    public class AttemptVoided : Change
    {
        public string Reason { get; set; }

        public AttemptVoided(string reason)
        {
            Reason = reason;
        }
    }

    public class AttemptCommentPosted : Change
    {
        public Guid Question { get; }
        public string Comment { get; }

        public AttemptCommentPosted(Guid question, string comment)
        {
            Question = question;
            Comment = comment;
        }
    }

    public class ComposedQuestionAnswered : Change
    {
        public Guid Question { get; set; }
        public string Answer { get; set; }
        public int? Attempt { get; set; }

        public ComposedQuestionAnswered(Guid question, string answer)
        {
            Question = question;
            Answer = answer;
        }
    }

    public class ComposedQuestionAttemptStarted : Change
    {
        public Guid Question { get; set; }

        public ComposedQuestionAttemptStarted(Guid question)
        {
            Question = question;
        }
    }

    public class MatchingQuestionAnswered : Change
    {
        public Guid Question { get; set; }
        public IDictionary<int, string> Matches { get; set; }

        public MatchingQuestionAnswered(Guid question, IDictionary<int, string> matches)
        {
            Question = question;
            Matches = matches;
        }
    }

    public class MultipleChoiceQuestionAnswered : Change
    {
        public Guid Question { get; set; }
        public int Option { get; set; }

        public MultipleChoiceQuestionAnswered(Guid question, int option)
        {
            Question = question;
            Option = option;
        }
    }

    public class MultipleCorrectQuestionAnswered : Change
    {
        public Guid Question { get; set; }
        public int[] Options { get; set; }

        public MultipleCorrectQuestionAnswered(Guid question, int[] options)
        {
            Question = question;
            Options = options;
        }
    }

    public class QuestionVoided : Change
    {
        public Guid Question { get; set; }

        public QuestionVoided(Guid question)
        {
            Question = question;
        }
    }

    public class QuestionRegraded : Change
    {
        public Guid Form { get; set; }
        public Guid Question { get; set; }
        public List<OldOption> OldOptions { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public RegradeOption RegradeOption { get; set; }

        public QuestionRegraded(Guid form, Guid question, List<OldOption> oldOptions, RegradeOption regradeOption)
        {
            Form = form;
            Question = question;
            OldOptions = oldOptions;
            RegradeOption = regradeOption;
        }
    }

    public class ScoreCalculated : Change
    {
        public decimal Points { get; set; }
        public decimal Score { get; set; }
        public string Grade { get; set; }
        public bool IsPassing { get; set; }

        public ScoreCalculated(decimal points, decimal score, string grade, bool isPassing)
        {
            Points = points;
            Score = score;
            Grade = grade;
            IsPassing = isPassing;
        }
    }

    public class BooleanTableQuestionAnswered : Change
    {
        public Guid Question { get; set; }
        public IDictionary<int, bool> Options { get; set; }

        public BooleanTableQuestionAnswered(Guid question, IDictionary<int, bool> options)
        {
            Question = question;
            Options = options;
        }
    }

    public class TrueOrFalseQuestionAnswered : Change
    {
        public Guid Question { get; set; }
        public int Option { get; set; }

        public TrueOrFalseQuestionAnswered(Guid question, int option)
        {
            Question = question;
            Option = option;
        }
    }

    public class HotspotQuestionAnswered : Change
    {
        public Guid Question { get; set; }
        public AttemptHotspotPinAnswer[] Pins { get; set; }

        public HotspotQuestionAnswered(Guid question, AttemptHotspotPinAnswer[] pins)
        {
            Question = question;
            Pins = pins;
        }
    }

    public class OrderingQuestionAnswered : Change
    {
        public Guid Question { get; set; }
        public int[] OptionsOrder { get; set; }

        public OrderingQuestionAnswered(Guid question, int[] optionsOrder)
        {
            Question = question;
            OptionsOrder = optionsOrder;
        }
    }

    public class AttemptSectionSwitched : Change
    {
        public int NextSectionIndex { get; set; }

        public AttemptSectionSwitched(int nextSectionIndex)
        {
            NextSectionIndex = nextSectionIndex;
        }
    }

    public class AttemptQuestionSwitched : Change
    {
        public int NextSectionIndex { get; set; }
        public int NextQuestionIndex { get; set; }

        public AttemptQuestionSwitched(int nextSectionIndex, int nextQuestionIndex)
        {
            NextSectionIndex = nextSectionIndex;
            NextQuestionIndex = nextQuestionIndex;
        }
    }

    public class AttemptGradingAssessorAssigned : Change
    {
        public Guid? GradingAssessor { get; set; }

        public AttemptGradingAssessorAssigned(Guid? gradingAssessor)
        {
            GradingAssessor = gradingAssessor;
        }
    }

    public class AttemptRubricPointsUpdated : Change
    {
        public Guid RubricId { get; }
        public Guid[] QuestionIds { get; }
        public decimal RubricPoints { get; }

        public AttemptRubricPointsUpdated(Guid[] questionIds, Guid rubricId, decimal rubricPoints)
        {
            RubricId = rubricId;
            QuestionIds = questionIds;
            RubricPoints = rubricPoints;
        }
    }

    public class AttemptRubricChanged : Change
    {
        public Guid[] QuestionIds { get; }
        public Guid OldRubricId { get; }
        public Guid NewRubricId { get; }
        public decimal NewRubricPoints { get; }

        public AttemptRubricChanged(Guid[] questionIds, Guid oldRubricId, Guid newRubricId, decimal newRubricPoints)
        {
            QuestionIds = questionIds;
            OldRubricId = oldRubricId;
            NewRubricId = newRubricId;
            NewRubricPoints = newRubricPoints;
        }
    }

    public class AttemptQuestionRubricChanged : Change
    {
        public Guid QuestionId { get; }
        public AttemptQuestionRubric Rubric { get; }

        public AttemptQuestionRubricChanged(Guid questionId, AttemptQuestionRubric rubric)
        {
            QuestionId = questionId;
            Rubric = rubric;
        }
    }

    public class AttemptQuestionRubricInited : Change
    {
        public Guid QuestionId { get; }
        public AttemptQuestionRubric Rubric { get; }

        public AttemptQuestionRubricInited(Guid questionId, AttemptQuestionRubric rubric)
        {
            QuestionId = questionId;
            Rubric = rubric;
        }
    }
}