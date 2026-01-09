using System;
using System.Collections.Generic;

using InSite.Application.Attempts.Read;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Home
{
    public partial class Assessments : PortalBasePage
    {
        private Dictionary<Guid, Form> _forms;
        private Dictionary<MultiKey<Guid, Guid>, TLearnerAttemptSummary> _summaries;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _forms = new Dictionary<Guid, Form>();
            _summaries = new Dictionary<MultiKey<Guid, Guid>, TLearnerAttemptSummary>();

            var filter = new QAttemptFilter
            {
                LearnerUserIdentifier = User.UserIdentifier,
                FormOrganizationIdentifier = Organization.Identifier,
                OrderBy = nameof(QAttempt.AttemptStarted) + " DESC",
                Paging = Paging.SetSkipTake(0, 20)
            };
            var data = ServiceLocator.AttemptSearch.GetAttempts(filter, y => y.Form.BankSpecification);

            AttemptRepeater.DataSource = data;
            AttemptRepeater.DataBind();

            if (data.Count == 0)
                StatusAlert.AddMessage(AlertType.Information, GetDisplayText("There are no Assessments to display related to your learner profile."));

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);
        }

        protected string GetResumeUrl()
        {
            var attempt = (QAttempt)Page.GetDataItem();
            var url = AttemptUrlForm.Create(attempt.FormIdentifier, attempt.AttemptIdentifier);

            return url.GetStartUrl();
        }

        protected string GetRestartUrl()
        {
            var attempt = (QAttempt)Page.GetDataItem();
            var url = AttemptUrlForm.Create(attempt.FormIdentifier);

            return url.GetStartUrl();
        }

        protected string GetResultUrl()
        {
            var attempt = (QAttempt)Page.GetDataItem();
            var url = AttemptUrlForm.Create(attempt.FormIdentifier, attempt.AttemptIdentifier);

            return url.GetResultUrl();
        }

        protected bool IsAllowResume()
        {
            var attempt = (QAttempt)Page.GetDataItem();

            if (!attempt.AttemptStarted.HasValue || attempt.AttemptGraded.HasValue)
                return false;

            var summary = GetSummary(attempt);

            return summary.AttemptLastStartedIdentifier == attempt.AttemptIdentifier;
        }

        protected bool IsAllowView()
        {
            var attempt = (QAttempt)Page.GetDataItem();
            var calcDisclosure = attempt.Form.BankSpecification.CalcDisclosure.ToEnum<DisclosureType>();

            return attempt.AttemptGraded.HasValue && calcDisclosure != DisclosureType.None;
        }

        protected bool IsAllowViewScore()
        {
            var attempt = (QAttempt)Page.GetDataItem();
            var calcDisclosure = attempt.Form.BankSpecification.CalcDisclosure.ToEnum<DisclosureType>();

            return attempt.AttemptGraded.HasValue
                && (calcDisclosure == DisclosureType.Score || calcDisclosure == DisclosureType.Full);
        }

        protected bool IsAllowRestart()
        {
            var attempt = (QAttempt)Page.GetDataItem();

            if (!attempt.AttemptGraded.HasValue || attempt.AttemptIsPassing || attempt.RegistrationIdentifier.HasValue)
                return false;

            var summary = GetSummary(attempt);
            if (summary.AttemptLastSubmittedIdentifier != attempt.AttemptIdentifier)
                return false;

            var form = GetForm(attempt);
            if (form == null)
                return false;

            return summary.AttemptTotalCount == 0
                || summary.AttemptTotalCount < form.Invigilation.AttemptLimit
                || form.Invigilation.AttemptLimit == 0;
        }

        private Form GetForm(QAttempt attempt)
        {
            if (!_forms.TryGetValue(attempt.FormIdentifier, out var result))
            {
                result = ServiceLocator.BankSearch.GetFormData(attempt.FormIdentifier);
                _forms.Add(attempt.FormIdentifier, result);
            }

            return result;
        }

        private TLearnerAttemptSummary GetSummary(QAttempt attempt)
        {
            var key = new MultiKey<Guid, Guid>(attempt.FormIdentifier, attempt.LearnerUserIdentifier);

            if (!_summaries.TryGetValue(key, out var result))
            {
                result = ServiceLocator.LearnerAttemptSummarySearch.GetSummary(attempt.FormIdentifier, attempt.LearnerUserIdentifier);
                _summaries.Add(key, result);
            }

            return result;
        }
    }
}