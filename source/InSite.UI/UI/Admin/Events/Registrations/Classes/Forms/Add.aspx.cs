using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Contacts.Read;
using InSite.Application.Events.Read;
using InSite.Application.Registrations.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using CreateControl = InSite.Admin.Contacts.People.Forms.Create;

namespace InSite.Admin.Events.Registrations.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        private const string SearchUrl = "/ui/admin/registrations/classes/search";

        #endregion

        #region Properties

        private Guid? EventIdentifier => Guid.TryParse(Request["event"], out var id) ? id : (Guid?)null;

        private string OutlineUrl => $"/ui/admin/events/classes/outline?event={EventIdentifier}&panel=registrations";

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

        private Guid[] _registeredCandidates;
        private Guid[] RegisteredCandidates
        {
            get
            {
                if (_registeredCandidates == null)
                    _registeredCandidates = ServiceLocator.RegistrationSearch.GetRegistrationsByEvent(EventIdentifier.Value).Select(x => x.CandidateIdentifier).ToArray();

                return _registeredCandidates;
            }
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

            PaymentSeat.AutoPostBack = true;
            PaymentSeat.ValueChanged += (s, a) => OnPaymentSeatChanged();

            PaymentEmployer.AutoPostBack = true;
            PaymentEmployer.ValueChanged += (s, a) => OnPaymentEmployerChanged();

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

                CriteriaEmployer.Filter.GroupType = GroupTypes.Employer;
                CriteriaEmployer.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;

                PaymentSeat.LoadItems(
                    ServiceLocator.EventSearch.GetSeats(EventIdentifier.Value, false),
                    "SeatIdentifier", "SeatTitle"
                );

                PaymentEmployer.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;
                PaymentEmployer.Filter.GroupType = GroupTypes.Employer;

                FormIdentifier.Filter.EventIdentifier = EventIdentifier;

                RegistrationRequestedBy.Filter.OrganizationIdentifier = Organization.OrganizationIdentifier;

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

        private void OnPaymentSeatChanged()
        {
            var hasData = false;
            var seatId = PaymentSeat.ValueAsGuid;

            if (seatId.HasValue)
            {
                var seat = ServiceLocator.EventSearch.GetSeat(seatId.Value);
                var configuration = JsonConvert.DeserializeObject<SeatConfiguration>(seat.Configuration);

                hasData = (configuration?.BillingCustomers).IsNotEmpty();

                if (hasData)
                    BillingToCustomer.LoadItems(configuration.BillingCustomers);
            }

            BillingToCustomerField.Visible = hasData;
        }

        private void OnPaymentEmployerChanged()
        {
            var employer = PaymentEmployer.HasValue ? ServiceLocator.GroupSearch.GetGroup(PaymentEmployer.Value.Value) : null;
            var status = TCollectionItemCache.GetName(employer?.GroupStatusItemIdentifier);

            EmployerStatus.InnerText = status;
        }

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
            if (@event == null || @event.OrganizationIdentifier != Organization.OrganizationIdentifier)
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
                var allowNewContact = Identity.IsGranted(ActionName.Admin_Registrations_Classes_Add_NewContact);
                var allowUploadContact = Identity.IsGranted(ActionName.Admin_Registrations_Classes_Add_UploadContact);
                var returnUrl = HttpUtility.UrlEncode($"/ui/admin/registrations/classes/add?event={EventIdentifier}");

                NewUserCard.Visible = allowNewContact || allowUploadContact;

                CreateContactLink.Visible = allowNewContact;
                CreateContactLink.NavigateUrl = $"/ui/admin/contacts/people/create?return={returnUrl}&event={EventIdentifier}&action=registrations_add";

                UploadContactLink.Visible = allowUploadContact;
                UploadContactLink.NavigateUrl = $"/ui/admin/contacts/people/upload?return={returnUrl}&aevent={EventIdentifier}&action=registrations_add";

                CriteriaClear();
            }

            BindIncludeInT2202(@event);

            OnPaymentSeatChanged();
            OnPaymentEmployerChanged();

            Search();

            SearchResultCloseButton.NavigateUrl = OutlineUrl;

            AssessmentPanel.Visible = ServiceLocator.EventSearch.GetEventAssessmentForms(EventIdentifier.Value).Count > 0;
        }

        private void BindIncludeInT2202(QEvent @event)
        {
            bool isOneDay = false;

            if (@event.EventScheduledEnd.HasValue)
            {
                var start = TimeZoneInfo.ConvertTime(@event.EventScheduledStart, Identity.User.TimeZone);
                var end = TimeZoneInfo.ConvertTime(@event.EventScheduledEnd.Value, Identity.User.TimeZone);

                if (start.Year == end.Year && start.Month == end.Month && start.Day == end.Day)
                    isOneDay = true;
            }

            IncludeInT2202.Checked = !isOneDay;
        }

        private void CriteriaClear()
        {
            CriteriaContactName.Text = null;
            CriteriaEmail.Text = null;
            CriteriaEmployer.Value = null;

            SearchSelectedUsers = new HashSet<Guid>();
        }

        #endregion

        #region Methods (save)

        private bool Save()
        {
            var contacts = SearchSelectedUsers.Except(RegisteredCandidates);
            var commands = new List<Command>();
            var eventId = EventIdentifier.Value;

            var seatId = PaymentSeat.ValueAsGuid;
            var fee = PaymentRegistrationFee.ValueAsDecimal;
            var includeInTaxForm = IncludeInT2202.Checked;
            var billingTo = BillingToCustomer.Value;
            var paymentEmployerId = PaymentEmployer.HasValue ? ServiceLocator.GroupSearch.GetGroup(PaymentEmployer.Value.Value)?.GroupIdentifier : null;
            var formId = FormIdentifier.Value;

            foreach (var userIdentifier in contacts)
            {
                var employer = PersonCriteria.BindFirst(
                    x => x.EmployerGroup,
                    new PersonFilter
                    {
                        OrganizationIdentifier = Organization.Identifier,
                        UserIdentifier = userIdentifier
                    });

                var registrationId = UniqueIdentifier.Create();

                commands.Add(new RequestRegistration(registrationId, Organization.OrganizationIdentifier, eventId, userIdentifier, null, null, fee, null, null));

                if (RegistrationRequestedBy.Value.HasValue && User.Identifier != RegistrationRequestedBy.Value)
                    commands.Add(new ModifyRegistrationRequestedBy(registrationId, RegistrationRequestedBy.Value));

                Guid? userEmployer = paymentEmployerId ?? employer?.GroupIdentifier;
                if (userEmployer.HasValue)
                    commands.Add(new AssignEmployer(registrationId, userEmployer));

                if (includeInTaxForm)
                    commands.Add(new IncludeRegistrationToT2202(registrationId));

                if (formId.HasValue)
                {
                    commands.Add(new AssignExamForm(registrationId, formId.Value, null));
                    commands.Add(new LimitExamTime(registrationId));
                }

                if (ApprovalStatus.Value.HasValue())
                    commands.Add(new ChangeApproval(registrationId, ApprovalStatus.Value, null, null, null));

                if (seatId.HasValue)
                    commands.Add(new AssignSeat(registrationId, seatId, fee, billingTo));
            }

            ServiceLocator.SendCommands(commands);

            return true;
        }

        #endregion

        #region Methods (search results)

        private void Search()
        {
            var count = ServiceLocator.PersonSearch.CountPersons(CreateFilter());
            var hasData = count > 0;

            SearchResultPagination.ItemsCount = count;
            SearchResultPagination.PageIndex = 0;
            SearchResultFooter.Visible = SearchResultPagination.PageCount > 1;
            SearchResultCount.InnerText = count.ToString("n0");

            SearchResultUpdatePanel.Visible = hasData;
            SearchResultCount.Visible = hasData;
            SearchNoResultContainer.Visible = !hasData;

            SearchResultRepeater.DataBind();
        }

        private object GetCriteriaResultData()
        {
            var filter = CreateFilter();
            filter.OrderBy = "User.FullName,User.Email";
            filter.Paging = Paging.SetStartEnd(SearchResultPagination.StartItem, SearchResultPagination.EndItem);

            return ServiceLocator.PersonSearch.GetPersons(filter, x => x.User, x => x.EmployerGroup).Select(x => new
            {
                UserIdentifier = x.UserIdentifier,
                FullName = x.User.FullName,
                Email = x.User.Email,
                EmailAlternate = x.User.EmailAlternate,
                PersonCode = x.PersonCode,
                EmployerIdentifier = x.EmployerGroup?.GroupIdentifier,
                EmployerName = x.EmployerGroup?.GroupName
            });
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

        private QPersonFilter CreateFilter()
        {
            return new QPersonFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                UserIdentifiers = SavedIdentifiers,
                ExcludeUserIdentifiers = RegisteredCandidates,
                UserNameContains = CriteriaContactName.Text,
                UserEmailContains = CriteriaEmail.Text,
                EmployerGroupIdentifier = CriteriaEmployer.Value
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