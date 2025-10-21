using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.Admin.Events.Candidates.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/events/exams/search";

        #endregion

        #region Properties

        private Guid? EventIdentifier => Guid.TryParse(Request.QueryString["event"], out var id) ? id : (Guid?)null;

        private string OutlineUrl =>
            $"/ui/admin/events/exams/outline?event={EventIdentifier}&panel=candidates";

        private List<Guid> SearchResultDataKeys
        {
            get => (List<Guid>)ViewState[nameof(SearchResultDataKeys)];
            set => ViewState[nameof(SearchResultDataKeys)] = value;
        }

        private HashSet<Guid> SearchSelectedUsers
        {
            get => (HashSet<Guid>)ViewState[nameof(SearchSelectedUsers)];
            set => ViewState[nameof(SearchSelectedUsers)] = value;
        }

        private Guid[] SavedIdentifiers
        {
            get => (Guid[])ViewState[nameof(SavedIdentifiers)];
            set => ViewState[nameof(SavedIdentifiers)] = value;
        }

        private List<PersonExamRegistrationItem> NewCandidates
        {
            get => (List<PersonExamRegistrationItem>)ViewState[nameof(NewCandidates)];
            set => ViewState[nameof(NewCandidates)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriteriaEmployer.AutoPostBack = true;
            CriteriaEmployer.ValueChanged += (s, a) => Search();

            CriteriaSearchButton.Click += (s, a) => Search();
            CriteriaClearButton.Click += (s, a) => { CriteriaClear(); Search(); };

            SearchResultPagination.PageChanged += SearchResultPagination_PageChanged;

            SearchResultRepeater.DataBinding += SearchResultRepeater_DataBinding;
            SearchResultRepeater.ItemDataBound += SearchResultRepeater_ItemDataBound;

            SearchResultSaveButton.Click += SearchResultSaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                if (!CanCreate)
                    HttpResponseHelper.Redirect(SearchUrl);

                var isIta = Organization.Identifier == OrganizationIdentifiers.SkilledTradesBC;
                CriteriaForceRegistration.Visible = isIta;
                CriteriaForceRegistration.Checked = isIta;

                CriteriaEmployer.Filter.GroupType = GroupTypes.Employer;

                Open();
            }
            else
            {
                GetCriteriaResultSelections();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var selectedCount = SearchSelectedUsers != null ? SearchSelectedUsers.Count : 0;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Add),
                "set_count",
                $"$('#{SearchSelectCount.ClientID}').text('{selectedCount.ToString("n0")}');",
                true);
        }

        #endregion

        #region Event handlers

        private void SearchResultPagination_PageChanged(object sender, Pagination.PageChangedEventArgs e)
        {
            SearchResultRepeater.DataBind();
        }

        private void SearchResultRepeater_DataBinding(object sender, EventArgs e)
        {
            SearchResultDataKeys = new List<Guid>();

            SearchResultRepeater.DataSource = GetCriteriaResultData();
        }

        private void SearchResultRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var id = (Guid)DataBinder.Eval(e.Item.DataItem, "UserIdentifier");

            var selectedCheckBox = (ICheckBoxControl)e.Item.FindControl("Selected");
            selectedCheckBox.Checked = SearchSelectedUsers.Contains(id);

            SearchResultDataKeys.Add(id);
        }

        private void SearchResultSaveButton_Click(object sender, EventArgs e)
        {
            if (SearchSelectedUsers.Count == 0)
            {
                ScreenStatus.AddMessage(AlertType.Error, "There are no selected contacts");
                return;
            }

            if (Save())
                HttpResponseHelper.Redirect(OutlineUrl);
        }

        #endregion

        #region Methods (open)

        private void Open()
        {
            var @event = EventIdentifier.HasValue
                ? ServiceLocator.EventSearch.GetEvent(EventIdentifier.Value)
                : null;
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier || @event.EventSchedulingStatus == "Cancelled")
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(
                Page,
                qualifier: $"{@event.EventTitle} <span class='form-text'>scheduled {@event.EventScheduledStart.FormatDateOnly(User.TimeZone)}</span>");

            if (Request.QueryString["userCreated"] == "1" && CreateControl.SavedIdentifiers.IsNotEmpty())
            {
                SavedIdentifiers = CreateControl.SavedIdentifiers;
                SearchSelectedUsers = SavedIdentifiers.ToHashSet();

                CreateControl.SavedIdentifiers = null;
            }
            else
            {
                var allowNewContact = Identity.IsGranted(ActionName.Admin_Registrations_Exams_Add_NewContact);
                var allowUploadContact = Identity.IsGranted(ActionName.Admin_Registrations_Exams_Add_UploadContact);
                var returnUrl = HttpUtility.UrlEncode($"/ui/admin/registrations/exams/add?event={EventIdentifier}");

                NewUserCard.Visible = allowNewContact || allowUploadContact;

                CreateContactLink.Visible = allowNewContact;
                CreateContactLink.NavigateUrl = $"/ui/admin/contacts/people/create?return={returnUrl}&event={EventIdentifier}&action=candidates_add";

                UploadContactLink.Visible = allowUploadContact;
                UploadContactLink.NavigateUrl = $"/ui/admin/contacts/people/upload?return={returnUrl}&event={EventIdentifier}&action=candidates_add";

                CriteriaClear();
            }

            Search();

            SearchResultCloseButton.NavigateUrl = OutlineUrl;
        }

        private void CriteriaClear()
        {
            CriteriaContactName.Text = null;
            CriteriaEmail.Text = null;
            CriteriaEmployer.Value = null;
            CriteriaContactCode.Text = null;

            SearchSelectedUsers = new HashSet<Guid>();
        }

        #endregion

        #region Methods (save)

        private bool Save()
        {
            var @event = EventIdentifier.Value;
            var excludes = ServiceLocator.RegistrationSearch
                .GetRegistrationsByEvent(EventIdentifier.Value)
                .Select(x => x.CandidateIdentifier)
                .ToHashSet();

            foreach (var candidate in SearchSelectedUsers)
            {
                if (excludes.Contains(candidate))
                    continue;

                var user = SelectUser(candidate);
                var request = new RequestRegistration(UniqueIdentifier.Create(), Organization.OrganizationIdentifier, @event, candidate, null, null, null, null, null);
                ServiceLocator.SendCommand(request);

                if (user.EmployerGroup != null)
                {
                    var assign = new AssignEmployer(request.AggregateIdentifier, user.EmployerGroup.GroupIdentifier);
                    ServiceLocator.SendCommand(assign);
                }

                ServiceLocator.SendCommand(new IncludeRegistrationToT2202(request.AggregateIdentifier));
            }

            return true;
        }

        private QPerson SelectUser(Guid candidate)
        {
            var person = ServiceLocator.PersonSearch.GetPerson(candidate, Organization.OrganizationIdentifier, x => x.EmployerGroup);
            if (person != null)
                return person;

            var code = NewCandidates.SingleOrDefault(x => x.UserIdentifier == candidate)?.PersonCode;
            if (code.IsNotEmpty())
                code = StringHelper.Snip(code, 20);

            var u = UserFactory.Create();
            u.FirstName = "-";
            u.FullName = "-";
            u.Email = code + "@itaportal.ca";
            u.LastName = "-";
            u.UserIdentifier = candidate;

            person = UserFactory.CreatePerson(Organization.Identifier);
            person.PersonCode = code;

            UserStore.Insert(u, person);

            person = ServiceLocator.PersonSearch.GetPerson(candidate, Organization.OrganizationIdentifier, x => x.EmployerGroup);

            return person;
        }

        #endregion

        #region Methods (search results)

        private void Search()
        {
            NewCandidates = new List<PersonExamRegistrationItem>();
            {
                var inputCodes = StringHelper.Split(CriteriaContactCode.Text);

                if (CriteriaForceRegistration.Checked && inputCodes.Length > 0)
                {
                    var existCodes = PersonCriteria.Bind(
                        x => x.PersonCode,
                        new PersonFilter
                        {
                            OrganizationIdentifier = Organization.OrganizationIdentifier,
                            CodesExact = inputCodes
                        });

                    foreach (var code in inputCodes.Distinct(StringComparer.OrdinalIgnoreCase))
                    {
                        if (!existCodes.Contains(code, StringComparer.OrdinalIgnoreCase))
                            NewCandidates.Add(new PersonExamRegistrationItem
                            {
                                UserIdentifier = UniqueIdentifier.Create(),
                                Email = null,
                                EmailAlternate = null,
                                EmployerName = null,
                                EmployerIdentifier = null,
                                FullName = "Not Found",
                                PersonCode = code
                            });
                    }
                }
            }

            var foundCount = PersonSearch.Count(GetFilter());
            var totalCount = foundCount + NewCandidates.Count;
            var hasData = totalCount > 0;

            SearchResultPagination.ItemsCount = totalCount;
            SearchResultPagination.PageIndex = 0;
            SearchResultFooter.Visible = SearchResultPagination.PageCount > 1;
            SearchResultCount.InnerText = totalCount.ToString("n0");

            SearchResultUpdatePanel.Visible = hasData;
            SearchResultCount.Visible = hasData;
            SearchNoResultContainer.Visible = !hasData;

            SearchResultRepeater.DataBind();
        }

        private object GetCriteriaResultData()
        {
            var filter = GetFilter();
            filter.Paging = Paging.SetSkipTake(SearchResultPagination.ItemsSkip, SearchResultPagination.ItemsTake);
            filter.OrderBy = "FullName,Email";

            var candidates = PersonSearch.Select(filter);

            if (NewCandidates.Count > 0)
            {
                var pager = SearchResultPagination;
                var existCount = pager.ItemsCount - NewCandidates.Count;
                var lastRowNum = pager.PageIndex * pager.PageSize + pager.PageSize;

                if (lastRowNum > existCount)
                {
                    var existOffset = existCount % pager.PageSize;
                    var newOffset = pager.PageSize - existOffset;
                    var startPageIndex = existCount > 0
                        ? (int)Math.Ceiling((decimal)existCount / pager.PageSize) - 1
                        : 0;

                    if (pager.PageIndex > startPageIndex)
                    {
                        candidates = NewCandidates
                            .Skip(newOffset + (pager.PageIndex - startPageIndex - 1) * pager.PageSize)
                            .Take(pager.PageSize)
                            .ToArray();
                    }
                    else
                    {
                        candidates = candidates.Take(existOffset).Concat(NewCandidates.Take(newOffset)).ToArray();
                    }
                }
            }

            return candidates;
        }

        private void GetCriteriaResultSelections()
        {
            if (!SearchResultRepeater.Visible)
                return;

            foreach (RepeaterItem item in SearchResultRepeater.Items)
            {
                var checkBox = (ICheckBoxControl)item.FindControl("Selected");
                var userId = SearchResultDataKeys[item.ItemIndex];

                if (!checkBox.Checked && SearchSelectedUsers.Contains(userId))
                    SearchSelectedUsers.Remove(userId);
                else if (checkBox.Checked && !SearchSelectedUsers.Contains(userId))
                    SearchSelectedUsers.Add(userId);
            }
        }

        private PersonExamRegistrationFilter GetFilter()
        {
            return new PersonExamRegistrationFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                EmployerGroupIdentifier = CriteriaEmployer.Value,
                SavedIdentifiers = SavedIdentifiers,
                PersonCode = CriteriaContactCode.Text,
                PersonName = CriteriaContactName.Text,
                PersonEmail = CriteriaEmail.Text
            };
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"event={Request.QueryString["event"]}&panel=registrations"
                : null;
        }

        #endregion
    }
}
