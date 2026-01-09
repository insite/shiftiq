namespace Shift.Constant
{
    public static class FlagTypeExtensions
    {
        public static string ToIconHtml(this FlagType value)
        {
            var cssClass = value.GetContextualClass();
            if (string.IsNullOrEmpty(cssClass))
                return string.Empty;

            return $"<span class=\"text-{cssClass}\"><i class=\"fas fa-flag\"></i></span>";
        }
    }
}