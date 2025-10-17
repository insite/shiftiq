namespace Shift.Toolbox.Integration.DirectAccess
{
    public class DirectAccessApiError
    {
        public int StatusCode { get; set; }
        
        public string ExceptionType { get; set; }
        
        public string Message { get; set; }

        public string CallStack { get; set; }
    }
}