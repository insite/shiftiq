namespace Shift.Common
{
    public class ExceptionHandlerSettings
    {
        public string Action { get; set; }
        public string TypeEquals { get; set; }
        public string TypeAssignableFrom { get; set; }
        public string MessageStartsWith { get; set; }
        public string MessagePattern { get; set; }
        public string MessageContains { get; set; }
        public string MessageEquals { get; set; }
        public string RequestPath { get; set; }
        public string RedirectUrl { get; set; }
        public bool? ClearError { get; set; }
    }
}
