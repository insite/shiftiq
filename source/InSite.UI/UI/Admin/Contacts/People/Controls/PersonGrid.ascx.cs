using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Contacts.People.Controls
{
    public partial class PersonGrid : SearchResultsGridViewController<PersonFilter>
    {
        #region Classes

        [Serializable]
        public class CommandButtonData
        {
            public string IconName { get; set; }
            public string ToolTip { get; set; }
            public string ConfirmText { get; set; }
            public string CommandName { get; set; }
            public string OnClientClickHandler { get; set; }
        }

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        private CommandButtonData[] CommandButtons
        {
            get { return (CommandButtonData[])ViewState[nameof(CommandButtons)]; }
            set { ViewState[nameof(CommandButtons)] = value; }
        }

        #endregion

        #region Fields

        private List<List<IconButton>> _commandButtons = new List<List<IconButton>>();
        private int _commandButtonsColumnIndex = -1;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCreated += Grid_ItemCreated;
            Grid.RowDataBound += Grid_ItemDataBound;
        }

        #endregion

        #region Event handlers

        private void Grid_ItemCreated(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            if (CommandButtons != null)
            {
                var itemCommandButtons = new List<IconButton>();

                foreach (CommandButtonData data in CommandButtons)
                {
                    var commandButton = new IconButton
                    {
                        Name = data.IconName,
                        CommandName = string.Format("CommandButton_{0:00}_{1:00}", _commandButtons.Count, itemCommandButtons.Count),
                        ToolTip = data.ToolTip,
                        ConfirmText = data.ConfirmText
                    };

                    e.Row.Cells[_commandButtonsColumnIndex].Controls.Add(commandButton);

                    itemCommandButtons.Add(commandButton);
                }

                if (itemCommandButtons.Count > 0)
                    e.Row.Width = Unit.Pixel(itemCommandButtons.Count * 28);

                _commandButtons.Add(itemCommandButtons);
            }
        }

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var name = (ITextControl)e.Row.FindControl("FullName");

            var row = e.Row.DataItem;
            var organization = (Guid)DataBinder.Eval(row, "OrganizationIdentifier");
            var user = (Guid)DataBinder.Eval(row, "UserIdentifier");
            var fullName = (string)DataBinder.Eval(row, "FullName");

            if (organization != Organization.OrganizationIdentifier)
                name.Text = $"{fullName}";
            else
                name.Text = $"<a href='/ui/admin/contacts/people/edit?contact={user}'>{fullName}</a>";

            if (CommandButtons != null)
            {
                var icons = _commandButtons[e.Row.RowIndex].GetEnumerator();
                var data = CommandButtons.GetEnumerator();

                while (icons.MoveNext() && data.MoveNext())
                {
                    var buttonData = (CommandButtonData)data.Current;
                    if (!string.IsNullOrEmpty(buttonData.OnClientClickHandler))
                        icons.Current.OnClientClick = buttonData.OnClientClickHandler.Replace("{PersonID}", DataBinder.Eval(row, "ContactID").ToString());
                }
            }
        }

        #endregion

        #region Public methods

        public void LoadData(PersonFilter filter, string[] visibleColumns, CommandButtonData[] commandButtons)
        {
            foreach (DataControlField column in Grid.Columns)
                column.Visible = false;

            foreach (var column in visibleColumns)
                Grid.Columns.FindByHeaderText(column).Visible = true;

            CommandButtons = commandButtons;

            Search(filter);
        }

        public void DisablePaging()
        {
            Grid.PagerSettings.Visible = false;
        }

        #endregion

        #region Search results

        protected override int SelectCount(PersonFilter filter)
        {
            return PersonCriteria.Count(filter);
        }

        protected override IListSource SelectData(PersonFilter filter)
        {
            return PersonCriteria.SelectForGrid(filter);
        }

        #endregion
    }
}