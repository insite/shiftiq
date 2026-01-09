using System;
using System.Data;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class EmployeeProfileSelector : ComboBox
    {
        public bool IncludePrimary
        {
            get { return ViewState[nameof(IncludePrimary)] == null ? true : (bool)ViewState[nameof(IncludePrimary)]; }
            set { ViewState[nameof(IncludePrimary)] = value; }
        }

        public Guid OrganizationIdentifier
        {
            get { return ViewState[nameof(OrganizationIdentifier)] == null ? CurrentIdentityFactory.ActiveOrganizationIdentifier : (Guid)ViewState[nameof(OrganizationIdentifier)]; }
            set { ViewState[nameof(OrganizationIdentifier)] = value; }
        }

        public Guid UserIdentifier
        {
            get { return ViewState[nameof(UserIdentifier)] == null ? CurrentSessionState.Identity.User.UserIdentifier : (Guid)ViewState[nameof(UserIdentifier)]; }
            set { ViewState[nameof(UserIdentifier)] = value; }
        }

        public UserProfileKey ValueAsKey
        {
            get => string.IsNullOrEmpty(Value) ? null : DeserializeSelectorKey(Value);
            set => Value = value != null ? SerializeSelectorKey(value) : null;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var data = UserProfileRepository.SelectForSelector(UserIdentifier, OrganizationIdentifier, IncludePrimary);
            foreach (DataRow row in data.Rows)
                list.Add(row["Value"].ToString(), row["Text"].ToString());

            return list;
        }

        private static UserProfileKey DeserializeSelectorKey(string text)
        {
            var parts = text.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            return new UserProfileKey
            {
                UserIdentifier = Guid.Parse(parts[0]),
                ProfileStandardIdentifier = Guid.Parse(parts[1]),
                DepartmentIdentifier = Guid.Parse(parts[2])
            };
        }

        private static string SerializeSelectorKey(UserProfileKey key)
        {
            return string.Format("{0};{1};{2}", key.UserIdentifier, key.ProfileStandardIdentifier, key.DepartmentIdentifier);
        }
    }
}
