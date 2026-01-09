using Humanizer.Inflections;

namespace Shift.Toolbox
{
    public static class HumanizerVocabulary
    {
        public static void Initialize()
        {
            Vocabularies.Default.AddPlural("Examination Feedback", "Examination Feedback");

            Vocabularies.Default.AddPlural("Code of Practice", "Codes of Practice");
            Vocabularies.Default.AddSingular("Codes of Practice", "Code of Practice");

            Vocabularies.Default.AddPlural("e-Learning Module", "e-Learning Modules");
            Vocabularies.Default.AddSingular("e-Learning Modules", "e-Learning Module");
            Vocabularies.Default.AddPlural("e-Learning Modules", "e-Learning Modules");

            Vocabularies.Default.AddPlural("HR Learning Module", "HR Learning Modules");
            Vocabularies.Default.AddSingular("HR Learning Modules", "HR Learning Module");
            Vocabularies.Default.AddPlural("HR Learning Modules", "HR Learning Modules");

            Vocabularies.Default.AddPlural("HSE Policy", "HSE Policies");
            Vocabularies.Default.AddSingular("HSE Policies", "HSE Policy");
        }
    }
}