using System;
using System.Runtime.Serialization;
using System.Security;

namespace Shift.Common
{
    [Serializable]
    public class ApplicationError : Exception, ISerializable
    {
        #region Constructors

        public ApplicationError()
        {
        }

        public ApplicationError(string message)
            : base(message)
        {
        }

        public ApplicationError(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ApplicationError(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion

        #region Interface (ISerializable)

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            GetObjectData(info, context);
        }

        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion

        public static ApplicationError Create(string text, params object[] args)
        {
            var message = args.Length == 0 ? text : string.Format(text, args);
            var ex = new ApplicationError(message);
            return ex;
        }

        public static ApplicationError Create(Exception inner, string text, params object[] args)
        {
            var message = args.Length == 0 ? text : string.Format(text, args);
            var ex = new ApplicationError(message, inner);
            return ex;
        }
    }
}