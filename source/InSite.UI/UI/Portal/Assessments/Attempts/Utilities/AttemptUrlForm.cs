using System;
using System.IO;

using InSite.Common.Web;

using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    [Serializable]
    public class AttemptUrlForm : AttemptUrlBase
    {
        #region Properties

        public override Guid FormIdentifier => _formId;

        #endregion

        #region Fields

        private Guid _formId;

        #endregion

        #region Constrution

        private AttemptUrlForm(AttemptActionType currentAction, Guid formId, Guid? attemptId)
            : base(currentAction, attemptId)
        {
            _formId = formId;
        }

        private AttemptUrlForm(AttemptActionType currentAction, Guid formId, string attempt)
            : base(currentAction)
        {
            _formId = formId;

            SetAttempt(attempt);
        }

        #endregion

        #region Methods

        public static AttemptUrlForm Create(Guid formId, Guid? attemptId = null) =>
            new AttemptUrlForm(AttemptActionType.Start, formId, attemptId);

        public static string GetStartUrl(Guid formId)
            => GetStartUrl(formId, false);

        public static string GetStartUrl(Guid formId, bool autoStart)
        {
            var url = $"/ui/portal/assessments/attempts/start?form={formId}";

            if (autoStart)
                url += "&auto-start=enabled";

            return url;
        }

        public static AttemptUrlForm Load(AttemptActionType currentAction, Guid formId, string attempt)
        {
            try
            {
                return new AttemptUrlForm(currentAction, formId, attempt);
            }
            catch (Exception ex)
            {
                AppSentry.SentryError(ex);
            }

            return null;
        }

        public string GetAttemptHash()
        {
            return GetAttempt(AttemptID, true);
        }

        internal override string GetReturnUrl() => RelativeUrl.PortalHomeUrl;

        internal override string GetStartUrl(Guid? attemptId = null, bool autoStart = false)
        {
            var url = GetStartUrl(_formId, autoStart);

            if (attemptId.HasValue)
                url = HttpResponseHelper.BuildUrl(url, $"attempt={attemptId.Value}");

            return url;
        }

        protected override string GetAnswerUrlInternal(string attempt) =>
            $"/ui/portal/assessments/attempts/answer?form={_formId}&attempt={attempt}";

        protected override string GetResultUrlInternal(string attempt) =>
            $"/ui/portal/assessments/attempts/result?form={_formId}&attempt={attempt}";

        protected override byte[] GetKey() => _formId.ToByteArray();

        protected override void WriteData(BinaryWriter writer) { }

        protected override void ReadData(BinaryReader reader) { }

        #endregion
    }
}