using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Surveys.Read
{
    public class QResponseSession : ISurveyResponse
    {
        public Guid? GroupIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }
        public Guid RespondentUserIdentifier { get; set; }
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyFormIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string RespondentLanguage { get; set; }
        public string ResponseSessionStatus { get; set; }

        public bool ResponseIsLocked { get; set; }

        public DateTimeOffset? ResponseSessionCompleted { get; set; }
        public DateTimeOffset? ResponseSessionCreated { get; set; }
        public DateTimeOffset? ResponseSessionStarted { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public Guid LastChangeUser { get; set; }
        public Guid? LastAnsweredQuestionIdentifier { get; set; }

        public virtual VUser Respondent { get; set; }
        public virtual QSurveyForm SurveyForm { get; set; }

        public virtual ICollection<QResponseAnswer> QResponseAnswers { get; set; }
        public virtual ICollection<QResponseOption> QResponseOptions { get; set; }

        #region ISurveyResponse

        [NotMapped]
        public bool SurveyIsConfidential { get => SurveyForm?.EnableUserConfidentiality ?? false; set { } }

        [NotMapped]
        public string SurveyName { get => SurveyForm?.SurveyFormName; set { } }

        [NotMapped]
        public int SurveyNumber { get => SurveyForm?.AssetNumber ?? 0; set { } }

        [NotMapped]
        public string RespondentEmail { get => Respondent?.UserEmail; set { } }

        [NotMapped]
        public string RespondentName { get => Respondent?.UserFullName; set { } }

        [NotMapped]
        public string RespondentNameFirst { get => Respondent?.UserFirstName; set { } }

        [NotMapped]
        public string RespondentNameLast { get => Respondent?.UserLastName; set { } }

        [NotMapped]
        public string GroupName { get => null; set { } }

        [NotMapped]
        public string PeriodName { get => null; set { } }

        [NotMapped]
        public string FirstCommentText { get => null; set { } }

        [NotMapped]
        public string FirstSelectionText { get => null; set { } }

        #endregion

        public QResponseSession()
        {
            QResponseAnswers = new HashSet<QResponseAnswer>();
            QResponseOptions = new HashSet<QResponseOption>();
        }
    }

    public interface ISurveyResponse
    {
        string FirstCommentText { get; set; }
        string FirstSelectionText { get; set; }
        Guid? GroupIdentifier { get; set; }
        string GroupName { get; set; }
        Guid? LastAnsweredQuestionIdentifier { get; set; }
        DateTimeOffset LastChangeTime { get; set; }
        string LastChangeType { get; set; }
        Guid LastChangeUser { get; set; }
        Guid OrganizationIdentifier { get; set; }
        Guid? PeriodIdentifier { get; set; }
        string PeriodName { get; set; }
        string RespondentEmail { get; set; }
        string RespondentLanguage { get; set; }
        string RespondentName { get; set; }
        string RespondentNameFirst { get; set; }
        string RespondentNameLast { get; set; }
        Guid RespondentUserIdentifier { get; set; }
        bool ResponseIsLocked { get; set; }
        DateTimeOffset? ResponseSessionCompleted { get; set; }
        DateTimeOffset? ResponseSessionCreated { get; set; }
        Guid ResponseSessionIdentifier { get; set; }
        DateTimeOffset? ResponseSessionStarted { get; set; }
        string ResponseSessionStatus { get; set; }
        Guid SurveyFormIdentifier { get; set; }
        bool SurveyIsConfidential { get; set; }
        string SurveyName { get; set; }
        int SurveyNumber { get; set; }
    }

    public class VResponse : ISurveyResponse
    {
        public Guid? GroupIdentifier { get; set; }
        public Guid? LastAnsweredQuestionIdentifier { get; set; }
        public Guid LastChangeUser { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }
        public Guid RespondentUserIdentifier { get; set; }
        public Guid ResponseSessionIdentifier { get; set; }
        public Guid SurveyFormIdentifier { get; set; }

        public string GroupName { get; set; }
        public string LastChangeType { get; set; }
        public string PeriodName { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentLanguage { get; set; }
        public string RespondentName { get; set; }
        public string RespondentNameFirst { get; set; }
        public string RespondentNameLast { get; set; }
        public string ResponseSessionStatus { get; set; }
        public string SurveyName { get; set; }

        public string FirstCommentText { get; set; }
        public string FirstSelectionText { get; set; }

        public bool ResponseIsLocked { get; set; }
        public bool SurveyIsConfidential { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public DateTimeOffset? ResponseSessionCompleted { get; set; }
        public DateTimeOffset? ResponseSessionCreated { get; set; }
        public DateTimeOffset? ResponseSessionStarted { get; set; }

        public int SurveyNumber { get; set; }
    }

    public class VResponseFirstSelection
    {
        public Guid ResponseIdentifier { get; set; }
        public Guid SurveyIdentifier { get; set; }

        public string AnswerText { get; set; }
        public string QuestionType { get; set; }

        public int QuestionSequence { get; set; }
    }

    public class VResponseFirstComment
    {
        public Guid ResponseIdentifier { get; set; }
        public Guid SurveyIdentifier { get; set; }

        public string AnswerText { get; set; }
        public string QuestionType { get; set; }

        public int QuestionSequence { get; set; }
    }
}
