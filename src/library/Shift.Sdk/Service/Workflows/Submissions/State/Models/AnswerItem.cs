using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Constant;

namespace InSite.Domain.Surveys.Sessions
{
    public class AnswerGroup
    {
        public Guid Question { get; set; }
        public string QuestionHeader { get; set; }
        public bool IsQuestionHeaderVisible { get; set; }
        public int QuestionSequence { get; set; }
        public string QuestionBody { get; set; }
        public bool QuestionIsRequired => AnswerPackets.Any(x => x.QuestionIsRequired);

        public List<AnswerItem> AnswerPackets { get; set; }

        public AnswerGroup(AnswerItem packet)
        {
            Question = packet.Question;
            QuestionHeader = packet.QuestionHeader;
            IsQuestionHeaderVisible = packet.IsQuestionHeaderVisible;
            QuestionSequence = packet.QuestionSequence;
            QuestionBody = packet.QuestionBody;

            AnswerPackets = new List<AnswerItem> { packet };
        }
    }

    public class AnswerItem
    {
        public Guid Question { get; set; }
        public string QuestionHeader { get; set; }
        public bool IsQuestionHeaderVisible { get; set; } = true;
        public int QuestionSequence { get; set; }
        public string QuestionBody { get; set; }
        public bool QuestionIsNested { get; set; }
        public bool QuestionIsHidden { get; set; }
        public bool QuestionIsRequired { get; set; }

        public int AnswerColumnSize
        {
            get
            {
                if (AnswerInputType == SurveyQuestionType.Date || AnswerInputType == SurveyQuestionType.Number)
                    return 3;
                if (AnswerInputType == SurveyQuestionType.Upload || AnswerInputType == SurveyQuestionType.Selection)
                    return 6;
                return 12;
            }
        }
        public SurveyQuestionType AnswerInputType { get; set; }
        public string AnswerText { get; set; }

        public string[] AnswerOptions { get; set; }
        public string[] LikertScales { get; set; }
        public decimal? LikertScaleHighestPoints { get; set; }
    }
}