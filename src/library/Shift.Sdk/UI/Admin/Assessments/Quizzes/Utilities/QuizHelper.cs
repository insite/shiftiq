using Shift.Constant;

namespace Shift.Sdk.UI
{
    public static class QuizHelper
    {
        public static string TypeFromQueryValue(string value)
        {
            switch (value)
            {
                case "typing-speed": return QuizType.TypingSpeed;
                case "typing-accuracy": return QuizType.TypingAccuracy;
            }

            return null;
        }

        public static string TypeToQueryValue(string type)
        {
            switch (type)
            {
                case QuizType.TypingSpeed: return "typing-speed";
                case QuizType.TypingAccuracy: return "typing-accuracy";
            }

            return null;
        }
    }
}