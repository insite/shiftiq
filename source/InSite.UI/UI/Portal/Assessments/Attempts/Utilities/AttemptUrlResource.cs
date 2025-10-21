using System;
using System.IO;

using InSite.Common.Web;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Assessments.Attempts.Utilities
{
    [Serializable]
    public class AttemptUrlResource : AttemptUrlBase
    {
        #region Properties

        public Guid PageIdentifier { get; private set; }
        public override Guid FormIdentifier => _formId;

        #endregion

        #region Fields

        private Guid _formId;

        #endregion

        #region Constrution

        private AttemptUrlResource(AttemptActionType currentAction, Guid pageId, Guid form, Guid? attemptId)
            : base(currentAction, attemptId)
        {
            PageIdentifier = pageId;
            _formId = form;
        }

        private AttemptUrlResource(AttemptActionType currentAction, Guid pageId, string attempt)
            : base(currentAction)
        {
            PageIdentifier = pageId;
            SetAttempt(attempt);
        }

        #endregion

        #region Methods

        public static AttemptUrlResource Create(Guid pageId, Guid formId, Guid? attemptId = null) =>
            new AttemptUrlResource(AttemptActionType.Start, pageId, formId, attemptId);

        public static string GetStartUrl(Guid pageId, bool autoStart = false)
        {
            var url = $"/ui/portal/assessments/attempts/start?resource={pageId}";

            if (autoStart)
                url += "&auto-start=enabled";

            return url;
        }

        public static AttemptUrlResource Load(AttemptActionType currentAction, Guid pageId, string attempt)
        {
            try
            {
                return new AttemptUrlResource(currentAction, pageId, attempt);
            }
            catch (Exception)
            {

            }

            return null;
        }

        internal override string GetReturnUrl() => RelativeUrl.PortalHomeUrl;

        internal override string GetStartUrl(Guid? attemptId = null, bool autoStart = false)
        {
            var url = GetStartUrl(PageIdentifier, autoStart);

            if (attemptId.HasValue)
                url = HttpResponseHelper.BuildUrl(url, $"attempt={attemptId.Value}");

            return url;
        }

        protected override string GetAnswerUrlInternal(string attempt) =>
            $"/ui/portal/assessments/attempts/answer?resource={PageIdentifier}&attempt={attempt}";

        protected override string GetResultUrlInternal(string attempt) =>
            $"/ui/portal/assessments/attempts/result?resource={PageIdentifier}&attempt={attempt}";

        protected override byte[] GetKey() => PageIdentifier.ToByteArray();

        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(_formId);
        }

        protected override void ReadData(BinaryReader reader)
        {
            _formId = reader.ReadGuid();
        }

        #endregion
    }
}