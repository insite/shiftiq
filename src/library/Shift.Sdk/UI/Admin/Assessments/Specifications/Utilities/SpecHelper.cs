using Shift.Constant;

namespace Shift.Sdk.UI
{
    public static class SpecHelper
    {
        public static string GetTitle(SpecificationType type)
        {
            switch (type)
            {
                case SpecificationType.Dynamic:
                    return "Dynamic (generated randomly per attempt)";
                case SpecificationType.Static:
                    return "Static (fixed identically for all attempts)";
                default:
                    return null;
            }
        }

        public static string GetDescription(SpecificationType type)
        {
            switch (type)
            {
                case SpecificationType.Dynamic:
                    return "A form with this specification does not have a fixed set " +
                        "of questions. If two learners start an attempt on this type of form, then each learner " +
                        "may be presented with a different set of questions and/or questions in a different sequence.";
                case SpecificationType.Static:
                    return "A form with this specification has a fixed set of questions in " +
                        "a fixed sequence, which is determined by the author of the form. Every learner who starts " +
                        "an attempt on form with a Static specification sees exactly the same set of questions " +
                        "in exactly the same sequence.";
                default:
                    return null;
            }
        }
    }
}