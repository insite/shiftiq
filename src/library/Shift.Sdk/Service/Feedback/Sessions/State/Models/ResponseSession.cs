using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using InSite.Domain.Surveys.Forms;

using Shift.Constant;

namespace InSite.Domain.Surveys.Sessions
{
    /// <summary>
    /// A survey response session (previously a survey submission form) is the list of answers input by a respondent
    /// on a single specific instance of a survey form.
    /// </summary>
    [Serializable]
    public class ResponseSession
    {
        /// <summary>
        /// Created | Started | Completed
        /// </summary>
        public ResponseSessionStatus Status { get; set; }

        /// <summary>
        /// The survey form for the session.
        /// </summary>
        public Guid Form { get; set; }

        /// <summary>
        /// Uniquely identifies the session.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// The respondent/user answering questions.
        /// </summary>
        public Guid User { get; set; }

        /// <summary>
        /// The organization account in which the session occurs.
        /// </summary>
        public Guid Tenant { get; set; }

        /// <summary>
        /// The language preference indicated by the respondent/user.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Changes to the answers in a session are disallowed when the session is locked.
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// When was the session created?
        /// </summary>
        public DateTimeOffset? Created { get; set; }

        /// <summary>
        /// When was the session started? If a session is started, paused, and restarted at a later time, then this 
        /// property stores the date/time when the session was first started.
        /// </summary>
        public DateTimeOffset? Started { get; set; }

        /// <summary>
        /// When was the session started? If a session is completed, unlocked, and recompleted at a later time, then 
        /// this property stores the date/time when the session was last completed.
        /// </summary>
        public DateTimeOffset? Completed { get; set; }

        /// <summary>
        /// The list of answers in the session.
        /// </summary>
        public List<ResponseAnswer> Answers { get; set; }

        /// <summary>
        /// The last question answered by the respondent.
        /// </summary>
        public Guid? LastQuestion { get; set; }

        /// <summary>
        /// Information about the source of this data (e.g. Authored|Uploaded|Upgraded|Copied).
        /// </summary>
        public string Source { get; set; }

        public Guid? Group { get; set; }
        public Guid? Period { get; set; }

        #region Construction

        public ResponseSession()
        {
            Answers = new List<ResponseAnswer>();
        }

        public void Initialize()
        {
            foreach (var answer in Answers)
                answer.Session = this;
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            Initialize();
        }

        #endregion

        #region Navigation and Interrogation

        public bool ContainsAnswer(Guid question)
        {
            return Answers.Any(x => x.Question == question);
        }

        public bool ContainsOption(Guid option)
        {
            return FlattenOptionItems().Any(x => x.Item == option);
        }

        public ResponseAnswer FindAnswer(Guid question)
        {
            return Answers.FirstOrDefault(x => x.Question == question);
        }

        public ResponseOption FindOption(Guid optionId) =>
            FlattenOptionItems().FirstOrDefault(x => x.Item == optionId);

        public bool FindAnswerAndOption(Guid optionId, out ResponseAnswer answer, out ResponseOption option)
        {
            answer = default;
            option = default;

            foreach (var a in Answers)
            {
                foreach (var o in a.Options)
                {
                    if (o.Item == optionId)
                    {
                        answer = a;
                        option = o;

                        return true;
                    }
                }
            }

            return false;
        }

        public ResponseOption[] FlattenOptionItems()
            => Answers.SelectMany(x => x.Options).ToArray();

        public ResponseOption[] FindOptions(List<SurveyOptionItem> items)
            => FlattenOptionItems().Where(x => items.Any(item => item.Identifier == x.Item)).ToArray();

        #endregion
    }
}
