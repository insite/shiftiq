using System;
using System.IO;

using Shift.Constant;

namespace Shift.Common.Attempts
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

        public abstract string GetReturnUrl();

        public abstract string GetStartUrl(Guid? attemptId = null, bool autoStart = false);

        public string GetAnswerUrl(Guid? attemptId, bool aiProctoringEnabled)
        {
            var attempt = GetAttempt(attemptId, true);

            return GetAnswerUrlInternal(attempt, aiProctoringEnabled);
        }

        protected abstract string GetAnswerUrlInternal(string attempt, bool aiProctoringEnabled);

        public string GetResultUrl(Guid? attemptId = null)
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
            var bytes = UrlTokenDecode(data);
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
                throw new InvalidOperationException("AttemptID is null");

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

                return UrlTokenEncode(data);
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

        protected static string BuildUrl(string path, string parameters)
        {
            return parameters.IsNotEmpty()
                ? path.Contains("?") ? path + "&" + parameters : path + "?" + parameters
                : path;
        }

        #endregion

        #region UrlToken

        private static byte[] UrlTokenDecode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return Array.Empty<byte>();

            int paddingCount = input[input.Length - 1] - '0';

            if (paddingCount < 0 || paddingCount > 2)
                throw new FormatException("Invalid URL token.");

            string base64 = input
                .Substring(0, input.Length - 1)
                .Replace('-', '+')
                .Replace('_', '/');

            switch (paddingCount)
            {
                case 1:
                    base64 += "=";
                    break;

                case 2:
                    base64 += "==";
                    break;
            }

            return Convert.FromBase64String(base64);
        }

        private static string UrlTokenEncode(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length == 0)
                return string.Empty;

            string base64 = Convert.ToBase64String(input);

            int paddingCount = 0;

            while (base64.EndsWith("=", StringComparison.Ordinal))
            {
                paddingCount++;
                base64 = base64.Substring(0, base64.Length - 1);
            }

            base64 = base64
                .Replace('+', '-')
                .Replace('/', '_');

            return base64 + paddingCount.ToString();
        }

        #endregion
    }
}