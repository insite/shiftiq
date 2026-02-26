using System;
using System.Data;

using InSite.Common.Web.UI;
using InSite.Domain.Foundations;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class RoleSelector : ComboBox
    {
        private static bool IsCollegeAdministrator(ISecurityFramework current)
            => current.IsInRole(CmdsRole.CollegeAdministrators);

        private static bool IsOfficeAdministrator(ISecurityFramework current)
            => current.IsInRole(CmdsRole.OfficeAdministrators);

        private static bool IsProgrammer(ISecurityFramework current)
            => current.IsInRole(CmdsRole.Programmers);

        private static bool IsSystemAdministrator(ISecurityFramework current)
            => current.IsInRole(CmdsRole.SystemAdministrators);

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var current = CurrentSessionState.Identity;
            var data = ContactRepository3.SelectForRoleSelector();

            foreach (DataRow row in data.Rows)
            {
                var id = (Guid)row["Value"];
                var name = (string)row["Text"];

                switch (name)
                {
                    case "Office Administrators":
                        if (IsProgrammer(current) || IsSystemAdministrator(current) || IsCollegeAdministrator(current) || IsOfficeAdministrator(current))
                            list.Add(id.ToString(), name);
                        break;

                    case "College Administrators":
                        if (IsProgrammer(current) || IsSystemAdministrator(current) || IsCollegeAdministrator(current))
                            list.Add(id.ToString(), name);
                        break;

                    case "System Administrators":
                        if (IsProgrammer(current) || IsSystemAdministrator(current))
                            list.Add(id.ToString(), name);
                        break;

                    case "Impersonators":
                    case "Programmers":
                    case "Super Validators":
                        if (IsProgrammer(current))
                            list.Add(id.ToString(), name);
                        break;

                    case GroupNames.Trainee + " Administrators":
                    case GroupNames.Trainee + " Developers":
                    case GroupNames.Trainee + " Instructors":
                    case GroupNames.Trainee + " Users":
                        if (current.IsInRole(name))
                            list.Add(id.ToString(), name);
                        break;

                    default:
                        list.Add(id.ToString(), name);
                        break;
                }
            }

            return list;
        }
    }
}