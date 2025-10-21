using System;
using System.IO;
using System.Web;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    [Serializable]
    public abstract class AttemptUrlBase
    {
        #region Properties

        public AttemptActionType CurrentAction { get; }

        public abstract Guid FormIdentifier { get; }

        public Guid? AttemptID { get; private set; }

        #endregion

        #region Constrution

        internal AttemptUrlBase(AttemptActionType currentAction, Guid? attemptId)
            : this(currentAction)
        {
            AttemptID = attemptId;
        }

        protected AttemptUrlBase(AttemptActionType currentAction)
        {
            CurrentAction = currentAction;
        }

        #endregion

        #region Methods

        internal abstract string GetReturnUrl();

        internal abstract string GetStartUrl(Guid? attemptId = null, bool autoStart = false);

        internal string GetAnswerUrl(Guid? attemptId = null)
        {
            var attempt = GetAttempt(attemptId, true);

            return GetAnswerUrlInternal(attempt);
        }

        protected abstract string GetAnswerUrlInternal(string attempt);

        internal string GetResultUrl(Guid? attemptId = null)
        {
            var attempt = GetAttempt(attemptId, true);

            return GetResultUrlInternal(attempt);
        }

        protected abstract string GetResultUrlInternal(string attempt);

        #endregion

        #region Helper methods

        protected abstract byte[] GetKey();

        protected void SetAttempt(string data)
        {
            if (data.HasNoValue())
                return;

            var key = GetKey();
            var keyHash = CalculateHashCode(key);
            var bytes = HttpServerUtility.UrlTokenDecode(data);
            bytes = EncryptionHelper.EncodeXor(bytes, key);

            using (var ms = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(ms))
                {
                    if (reader.ReadInt32() != keyHash)
                        throw ApplicationError.Create("Invalid data");

                    ReadData(reader);

                    AttemptID = reader.ReadGuidNullable();

                    if (ms.Position < ms.Length)
                        throw ApplicationError.Create("Data too long");
                }
            }
        }

        protected string GetAttempt(Guid? attemptId, bool isRequired)
        {
            if (!attemptId.HasValue)
                attemptId = AttemptID;

            if (isRequired && !attemptId.HasValue)
                throw new ApplicationError("AttemptID is null");

            var key = GetKey();
            var keyHash = CalculateHashCode(key);

            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(keyHash);

                    WriteData(writer);

                    writer.WriteNullable(attemptId);
                }

                var data = EncryptionHelper.EncodeXor(ms.ToArray(), key);

                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        protected abstract void WriteData(BinaryWriter writer);

        protected abstract void ReadData(BinaryReader reader);

        private static int CalculateHashCode(byte[] value)
        {
            unchecked // Overflow is fine, just wrap
            {
                var hashCode = 41;

                for (var i = 0; i < value.Length; i++)
                    hashCode = hashCode * 59 + value[i];

                return hashCode;
            }
        }

        #endregion
    }
}