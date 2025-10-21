using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence.Plugin.CMDS;

namespace InSite.Cmds.Controls.Contacts.Companies.Files
{
    public partial class UploadCompetencyChooser : UserControl
    {
        #region CompetencyItem class

        [Serializable]
        private class CompetencyItem
        {
            public Guid CompetencyStandardIdentifier { get; set; }
            public string CompetencyNumber { get; set; }
        }

        #endregion

        #region Properties

        public bool Enabled
        {
            set
            {
                CompetencyStandardIdentifier.Enabled = value;
                AddButton.Style["display"] = value ? "" : "none";
            }
        }

        private List<CompetencyItem> Competencies => (List<CompetencyItem>)(ViewState[nameof(Competencies)] 
            ?? (ViewState[nameof(Competencies)] = new List<CompetencyItem>()));

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddButton.Click += AddButton_Click;
            CompetencyList.ItemCommand += CompetencyList_ItemCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                CompetencyStandardIdentifier.Filter.OrganizationIdentifier = CurrentIdentityFactory.ActiveOrganizationIdentifier;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            AttributeList.Attributes["IsAttributeList"] = "true";
            AttributeList.Attributes["CompetencyStandardIdentifier"] = CompetencyStandardIdentifier.ClientID;
            AttributeList.Attributes["AddButton"] = AddButton.ClientID;
        }

        #endregion

        #region Event handlers

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (CompetencyStandardIdentifier.Value == null)
                return;

            var info = CompetencyRepository.Select(CompetencyStandardIdentifier.Value.Value);

            var item = new CompetencyItem()
            {
                CompetencyStandardIdentifier = info.StandardIdentifier,
                CompetencyNumber = info.Number
            };

            Competencies.Add(item);

            LoadData();
        }

        private void CompetencyList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "DeleteCompetency")
            {
                var competencyID = Guid.Parse((string)e.CommandArgument);
                var index = Competencies.FindIndex(c => c.CompetencyStandardIdentifier == competencyID);

                Competencies.RemoveAt(index);

                LoadData();
            }
        }

        #endregion

        #region Public methods

        public Guid[] GetValues()
        {
            if (Competencies.Count == 0)
                return null;

            List<Guid> values = new List<Guid>();

            foreach (CompetencyItem item in Competencies)
                values.Add(item.CompetencyStandardIdentifier);

            return values.ToArray();
        }

        public void ClearValues()
        {
            if (Competencies.Count == 0)
                return;

            Competencies.Clear();

            LoadData();
        }

        #endregion

        #region Helper methods

        private void LoadData()
        {
            CompetencyList.DataSource = Competencies;
            CompetencyList.DataBind();
            CompetencyList.Visible = Competencies.Count > 0;

            CompetencyStandardIdentifier.Filter.ExcludeCompetencies = GetValues();
            CompetencyStandardIdentifier.Value = null;
        }

        #endregion
    }
}