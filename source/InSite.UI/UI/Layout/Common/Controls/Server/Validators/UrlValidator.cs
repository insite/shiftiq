namespace InSite.Common.Web.UI
{
    public class UrlValidator : PatternValidator
    {
        public UrlValidator() { ValidationExpression = @"((https?):((//)|(\\\\))+[\w\d:#@%/;$()~_?\+-=\\\.&]*)"; }
    }
}