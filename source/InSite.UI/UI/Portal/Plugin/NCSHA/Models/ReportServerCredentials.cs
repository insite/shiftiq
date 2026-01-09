using System;
using System.Net;
using System.Security.Principal;

using Microsoft.Reporting.WebForms;

namespace InSite.UI.Variant.NCSHA.Reports
{
    [Serializable]
    public class ReportServerCredentials : IReportServerCredentials
    {
        readonly string _userDomain;
        readonly string _userName;
        readonly string _userPassword;

        public ReportServerCredentials()
        {
            var ssrs = ServiceLocator.AppSettings.Integration.SSRS;
            _userDomain = ssrs.Domain;
            _userName = ssrs.UserName;
            _userPassword = ssrs.Password;
        }

        public ICredentials NetworkCredentials
        {
            get
            {
                NetworkCredential credentials = new NetworkCredential(_userName, _userPassword, _userDomain);
                return credentials;
            }
        }

        public WindowsIdentity ImpersonationUser
        {
            get { return null; }
        }

        public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
        {
            authCookie = null;
            user = password = authority = null;
            return false;
        }
    }
}