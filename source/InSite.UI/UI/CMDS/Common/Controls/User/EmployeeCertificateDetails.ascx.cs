using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Profiles.EmployeeCertificates
{
    public partial class EmployeeCertificateDetails : UserControl
    {
        #region Properties

        private Guid? OriginalEmployeeID
        {
            get { return (Guid?)ViewState[nameof(OriginalEmployeeID)]; }
            set { ViewState[nameof(OriginalEmployeeID)] = value; }
        }

        private Guid? OriginalCertificateID
        {
            get { return (Guid?)ViewState[nameof(OriginalCertificateID)]; }
            set { ViewState[nameof(OriginalCertificateID)] = value; }
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CertificateExistValidator.ServerValidate += CertificateExistValidator_ServerValidate;

            ShowArchivedPeople.AutoPostBack = true;
            ShowArchivedPeople.CheckedChanged += ShowArchivedPeople_CheckedChanged;
        }

        #endregion

        #region Event handlers

        private void CertificateExistValidator_ServerValidate(Object source, ServerValidateEventArgs args)
        {
            if (!EmployeeID.HasValue || !CertificateID.HasValue
                || (EmployeeID.Value == OriginalEmployeeID && CertificateID.Value == OriginalCertificateID)
                )
            {
                return;
            }

            var employeeCertificate = ProfileCertificationRepository.Select(EmployeeID.Value.Value, CertificateID.Value.Value);

            args.IsValid = employeeCertificate == null;
        }

        private void ShowArchivedPeople_CheckedChanged(Object sender, EventArgs e)
        {
            EmployeeID.Filter.EnableIsArchived = !ShowArchivedPeople.Checked;
        }

        #endregion

        #region Getting and setting input values

        public void GetInputValues(TCollegeCertificate info)
        {
            info.LearnerIdentifier = EmployeeID.Value.Value;
            info.ProfileIdentifier = CertificateID.Value.Value;
            info.CertificateAuthority = InstitutionID.Value;
            info.CertificateTitle = StandardSearch.BindFirst(x => x.ContentTitle, x => x.StandardIdentifier == info.ProfileIdentifier) ?? "Missing Title";
            info.DateRequested = DateRequested.Value;
            info.DateGranted = DateGranted.Value;
            info.DateSubmitted = DateSubmitted.Value;
        }

        public void SetInputValues(TCollegeCertificate info)
        {
            OriginalEmployeeID = info.LearnerIdentifier;
            OriginalCertificateID = info.ProfileIdentifier;

            EmployeeID.Value = info.LearnerIdentifier;
            CertificateID.Value = info.ProfileIdentifier;
            InstitutionID.Value = info.CertificateAuthority;

            DateRequested.Value = info.DateRequested.HasValue ? info.DateRequested.Value.Date : (DateTime?)null;
            DateGranted.Value = info.DateGranted.HasValue ? info.DateGranted.Value.Date : (DateTime?)null;
            DateSubmitted.Value = info.DateSubmitted.HasValue ? info.DateSubmitted.Value.Date : (DateTime?)null;
        }

        #endregion
    }
}