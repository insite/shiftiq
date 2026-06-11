using System;

namespace InSite.Application.Attempts.Read
{
    public class QAttemptPin
    {
        public Guid AttemptIdentifier { get; set; }
        public Guid QuestionIdentifier { get; set; }
        public int QuestionSequence { get; set; }
        public int? OptionKey { get; set; }
        public decimal? OptionPoints { get; set; }
        public int? OptionSequence { get; set; }
        public string OptionText { get; set; }
        public int PinSequence { get; set; }
        public int PinX { get; set; }
        public int PinY { get; set; }

        public bool HasCoordinates => PinX >= 0 && PinY >= 0;

        public virtual QAttempt Attempt { get; set; }

        public QAttemptPin()
        {

        }

        public QAttemptPin(QAttemptQuestion question) : this()
        {
            AttemptIdentifier = question.AttemptIdentifier;
            QuestionIdentifier = question.QuestionIdentifier;
            QuestionSequence = question.QuestionSequence;
            PinX = -1;
            PinY = -1;
        }
    }
}
