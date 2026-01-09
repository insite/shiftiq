using System;

namespace Shift.Common
{
    public class ExceptionHandlerData
    {
        public Type Type { get; }
        public string Message { get; }
        public string RequestPath { get; }

        public ExceptionHandlerData(Exception ex, string requestPath)
        {
            Type = ex.GetType();
            Message = ex.Message;
            RequestPath = (requestPath).EmptyIfNull();
        }
    }
}