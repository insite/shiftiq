using System;
using System.Data;
using System.Linq;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Custom.CMDS.Common.Controls.Server
{
    public class SelfAssessmentStatusInput : InSite.Common.Web.UI.RadioButtonList
    {
        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var organization = CurrentSessionState.Identity.Organization;
            if (!Page.IsPostBack)
                LoadData(organization.Identifier);
        }

        #endregion

        #region Load data

        public void LoadData(Guid organization)
        {
            if (Items.Count > 0)
                return;

            DataTextField = "Text";
            DataValueField = "Value";

            // First check to see if there is a collection for the organization. If not, then check the enterprise.

            var data = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = organization,
                CollectionName = CollectionName.Validations_SelfAssessment_Status
            });

            if (data.Count == 0 && organization != ServiceLocator.Partition.Identifier)
                data = TCollectionItemCache.Select(new TCollectionItemFilter
                {
                    OrganizationIdentifier = ServiceLocator.Partition.Identifier,
                    CollectionName = CollectionName.Validations_SelfAssessment_Status
                });

            DataSource = data.Select(x => new
            {
                Value = x.ItemName,
                Text = StringHelper.Equals(x.ItemName, "Self Assessed (Yes)")
                    ? "Self Assessed - Yes, I have the knowledge, skills and experience to work independently."
                    : x.ItemName
            });

            DataBind();
        }

        #endregion
    }
}
