using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common.File
{
    [Serializable]
    public class AttemptUploadAnswer
    {
        [Serializable]
        public class Question
        {
            public int Sequence { get; set; }
            public string CurrentAnswer { get; set; }
            public List<string> Answers { get; set; }
            public string CorrectAnswer { get; set; }
            public string FinalAnswer { get; set; }

            public bool IsValid => IsValidAnswer(FinalAnswer);
            public int OptionIndex => IsValid ? FinalAnswer.ToUpper()[0] - 'A' : -1;

            public Question Clone() => new Question
            {
                Sequence = Sequence,
                CurrentAnswer = CurrentAnswer,
                Answers = Answers.Select(x => x).ToList(),
                CorrectAnswer = CorrectAnswer,
                FinalAnswer = FinalAnswer,
            };
        }

        public int Sequence { get; set; }

        public string LearnerCode { get; set; }
        public string LearnerName { get; set; }
        public Guid? LearnerUserIdentifier { get; set; }

        public Guid? FormIdentifier { get; set; }
        public string FormName { get; set; }

        public Question[] Questions { get; set; }
        public DateTimeOffset? AttemptGraded { get; set; }

        public string Errors { get; set; }
        public string Tag { get; set; }
        public decimal Percent { get; set; }

        public bool HasCurrentAttempt { get; set; }
        public bool IsAttended { get; set; }
        public bool IsPassed { get; set; }

        public bool? HasAttemptMatch { get; set; }

        public bool HasEventRegistration { get; set; }
        public bool HasUserAccount { get; set; }

        public int ScanCount => Questions.IsNotEmpty() ? Questions[0].Answers.Count : 0;

        public bool IsValid => Questions.All(x => x.FinalAnswer == "-" || IsValidAnswer(x.FinalAnswer));

        public static bool IsValidAnswer(string answer)
        {
            if (string.IsNullOrEmpty(answer))
                return false;

            var answerLowered = answer.ToLower();

            return answerLowered == "a" || answerLowered == "b" || answerLowered == "c" || answerLowered == "d";
        }

        public AttemptUploadAnswer Clone() => new AttemptUploadAnswer
        {
            Sequence = Sequence,

            LearnerCode = LearnerCode,
            LearnerName = LearnerName,
            LearnerUserIdentifier = LearnerUserIdentifier,

            FormIdentifier = FormIdentifier,
            FormName = FormName,

            Questions = Questions.Select(x => x.Clone()).ToArray(),
            AttemptGraded = AttemptGraded,

            Errors = Errors,
            Tag = Tag,
            Percent = Percent,

            HasCurrentAttempt = HasCurrentAttempt,
            IsAttended = IsAttended,
            IsPassed = IsPassed,

            HasAttemptMatch = HasAttemptMatch,

            HasEventRegistration = HasEventRegistration,
            HasUserAccount = HasUserAccount
        };
    }
}