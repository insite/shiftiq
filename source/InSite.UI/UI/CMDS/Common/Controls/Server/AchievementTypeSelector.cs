using System.Collections.Generic;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class AchievementTypeSelector : ComboBox
    {
        public string NullText
        {
            get
            {
                var text = ViewState[nameof(NullText)] as string;
                return text == null ? AchievementTypes.OtherAchievement : string.Empty;
            }
            set => ViewState[nameof(NullText)] = value;
        }

        public string ExcludeSubType
        {
            get => (string)ViewState[nameof(ExcludeSubType)];
            set => ViewState[nameof(ExcludeSubType)] = value;
        }

        protected override ComboBoxOption GetEmptyOption()
        {
            return new ComboBoxOption
            {
                Text = NullText
            };
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            var exclusions = new string[0];
            if (!string.IsNullOrEmpty(ExcludeSubType))
                exclusions = StringHelper.Split(ExcludeSubType);

            var organization = CurrentSessionState.Identity.Organization;
            var types = VCmdsAchievementSearch.SelectAchievementLabels(organization.Code, exclusions);

            foreach (var subtype in types.Items)
                list.Add(subtype.Value, subtype.Text);

            return list;
        }

        public static Dictionary<string, string> CreateAchievementLabelMapping(
            string organizationCode, 
            string[] labels = null)
        {
            var organizationId = CurrentSessionState.Identity.Organization.Identifier;

            return VCmdsAchievementSearch.SearchAchievementTypesInUse(organizationId, organizationCode, labels);
        }
    }
}