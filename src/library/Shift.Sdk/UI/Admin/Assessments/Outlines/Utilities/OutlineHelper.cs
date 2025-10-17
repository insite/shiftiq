namespace Shift.Sdk.UI
{
    public static class OutlineHelper
    {
        public static string DisplayCalculationMethod(string method)
        {
            switch (method)
            {
                case "sum_tier1": return "Summarize by GAC";
                case "sum_tier0": return "Summarize by GAC and Competency";
                case "decaying_average": return "Decaying Average";
                default: return null;
            }
        }
    }
}