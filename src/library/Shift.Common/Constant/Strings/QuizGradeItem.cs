namespace Shift.Constant
{
    public sealed class QuizGradeItem
    {
        #region Static

        public static QuizGradeItem Mistakes { get; }
        public static QuizGradeItem WordsPerMin { get; }
        public static QuizGradeItem CharsPerMin { get; }
        public static QuizGradeItem Accuracy { get; }
        public static QuizGradeItem Speed { get; }
        public static QuizGradeItem KeystrokesPerHour { get; }

        static QuizGradeItem()
        {
            Mistakes = new QuizGradeItem("Mistakes", "Mistakes", "1", GradeItemFormat.Number);
            WordsPerMin = new QuizGradeItem("Words per Minute", "WPM", "2", GradeItemFormat.Number);
            CharsPerMin = new QuizGradeItem("Characters per Minute", "CPM", "3", GradeItemFormat.Number);
            Accuracy = new QuizGradeItem("Accuracy", "Accuracy", "4", GradeItemFormat.Percent);
            Speed = new QuizGradeItem("Speed", "Speed", "5", GradeItemFormat.Percent);
            KeystrokesPerHour = new QuizGradeItem("Keystrokes Per Hour", "KPH", "6", GradeItemFormat.Number);
        }

        #endregion

        #region Instance

        public string FullName { get; }
        public string ShortName { get; }
        public string Code { get; }
        public GradeItemFormat Format { get; }

        private QuizGradeItem(string fullName, string shortName, string code, GradeItemFormat format)
        {
            FullName = fullName;
            ShortName = shortName;
            Code = code;
            Format = format;
        }

        #endregion
    }
}
