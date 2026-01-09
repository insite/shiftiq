using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Application.Registrations.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Common.Linq;

using CheckBox = System.Web.UI.WebControls.CheckBox;

namespace InSite.UI.Admin.Records.Gradebooks.Controls
{
    public partial class AddLearnerGrid : SearchResultsGridViewController<QUserFilter>
    {
        #region Classes

        private class PersonItem
        {
            public Guid UserIdentifier { get; set; }
            public string UserFullName { get; set; }
            public string ApprovalStatus { get; set; }
            public string UserEmail { get; set; }
            public string UserEmailAlternate { get; set; }
        }

        #endregion

        public HashSet<Guid> SelectedContacts => (HashSet<Guid>)(ViewState[nameof(SelectedContacts)] ?? (ViewState[nameof(SelectedContacts)] = new HashSet<Guid>()));
        public Guid? RegistrationEventIdentifier
        {
            get => (Guid?)ViewState[nameof(RegistrationEventIdentifier)];
            set => ViewState[nameof(RegistrationEventIdentifier)] = value;
        }

        protected override bool IsFinder => false;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowCreated += Grid_ItemCreated;
            Grid.RowDataBound += Grid_ItemDataBound;
        }

        public List<Guid> GetUsers()
        {
            var result = new List<Guid>();

            for (int i = 0; i < Grid.DataKeys.Count; i++)
                result.Add(Grid.GetDataKey<Guid>(i));

            return result;
        }

        protected override int SelectCount(QUserFilter filter)
        {
            if (!RegistrationEventIdentifier.HasValue)
                return ServiceLocator.ContactSearch.Count(filter);

            return GetRegistrations(filter).Count();
        }

        protected override IListSource SelectData(QUserFilter filter)
        {
            filter.OrderBy = "UserFullName";

            if (!RegistrationEventIdentifier.HasValue)
                return ServiceLocator.ContactSearch
                    .Bind(x => x, filter)
                    .Select(x => new PersonItem()
                    {
                        UserIdentifier = x.UserIdentifier,
                        UserFullName = x.UserFullName,
                        UserEmail = x.UserEmail,
                        UserEmailAlternate = x.UserEmailAlternate
                    })
                    .ToList().ToSearchResult();

            return GetRegistrations(filter).ToSearchResult();
        }

        private List<PersonItem> GetRegistrations(QUserFilter filter)
        {
            var result = new List<PersonItem>();
            var learnerData = ServiceLocator.ContactSearch
                .Bind(x => x, filter)
                .Select(x => x.UserIdentifier)
                .ToArray();

            if (learnerData.IsEmpty())
                return result;

            var registrationFilter = new QRegistrationFilter()
            {
                OrganizationIdentifier = Organization.Identifier,
                EventIdentifier = RegistrationEventIdentifier.Value,
                ApprovalStatus = "Registered",
                CandidateIdentifiers = learnerData,
                HasCandidate = true
            };

            result = ServiceLocator.RegistrationSearch.GetRegistrations(
                registrationFilter,
                x => x.Candidate
            )
            .Select(x => new PersonItem()
            {
                UserIdentifier = x.CandidateIdentifier,
                UserFullName = x.Candidate.UserFullName,
                UserEmail = x.Candidate.UserEmail,
                UserEmailAlternate = x.Candidate.UserEmailAlternate,
                ApprovalStatus = x.ApprovalStatus
            })
            .OrderBy(x => x.UserFullName)
            .ToList();

            return result;
        }

        private void Grid_ItemCreated(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var selectedCheckBox = (CheckBox)e.Row.FindControl("Selected");
            selectedCheckBox.CheckedChanged += Selected_CheckedChanged;
        }

        private void Grid_ItemDataBound(object sender, GridViewRowEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var id = (Guid)DataBinder.Eval(e.Row.DataItem, "UserIdentifier");

            var selectedCheckBox = (CheckBox)e.Row.FindControl("Selected");
            selectedCheckBox.Checked = SelectedContacts.Contains(id);
        }

        private void Selected_CheckedChanged(object sender, EventArgs e)
        {
            var chk = (CheckBox)sender;
            var row = (GridViewRow)chk.NamingContainer;
            var grid = (Grid)row.NamingContainer;
            var id = grid.GetDataKey<Guid>(row);

            if (!chk.Checked && SelectedContacts.Contains(id))
                SelectedContacts.Remove(id);
            else if (chk.Checked && !SelectedContacts.Contains(id))
                SelectedContacts.Add(id);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (DataControlField column in Grid.Columns)
            {
                if (column is System.Web.UI.WebControls.TemplateField tf && tf.HeaderText == "Status")
                {
                    tf.Visible = RegistrationEventIdentifier.HasValue;
                    break;
                }
            }
        }
    }
}