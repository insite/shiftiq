using System;
using System.Runtime.Serialization;

namespace Shift.Common
{
    [Serializable]
    public class ApplicationError : Exception, ISerializable
    {
        public ApplicationError(string message) : base(message) { }

        public static ApplicationError Create(string text, params object[] args)
        {
            var message = args.Length == 0 ? text : string.Format(text, args);

            var ex = new ApplicationError(message);

            return ex;
        }
    }
}