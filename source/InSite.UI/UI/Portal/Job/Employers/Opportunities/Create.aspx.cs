using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Contacts.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Constant;

namespace InSite.UI.Portal.Jobs.Employers.Opportunities
{
    public class CreateModel
    {
        public string OutlineUrl => "/ui/portal/job/employers/opportunities/edit";
        public Person Person { get; set; }
        public Guid EmployerGroupIdentifier { get; set; }
        public QGroup Employer { get; set; }
        public List<Domain.Foundations.Group> MyDepartments { get; set; }
        public Guid? DefaultDepartmentGroupIdentifier { get; set; }
    }

    public partial class Create : PortalBasePage
    {
        private CreateModel _model;

        private Guid? GroupIdentifier => Guid.TryParse(Page.Request.QueryString["group"], out Guid id) ? id : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!GroupIdentifier.HasValue)
                HttpResponseHelper.Redirect("/ui/portal/job/employers/profile/view");

            CreateModel();

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);

            BindModelToControls();
        }

        private void CreateModel()
        {
            _model = new CreateModel
            {
                Person = PersonSearch.Select(Organization.Identifier, User.Identifier),
            };

            _model.EmployerGroupIdentifier = GroupIdentifier.Value;
            _model.Employer = ServiceLocator.GroupSearch.GetGroup(_model.EmployerGroupIdentifier, x => x.Parent);

            _model.MyDepartments = Identity.Groups.Where(x => x.Type == GroupType.Department).ToList();

            if (_model.MyDepartments.Count == 1)
                _model.DefaultDepartmentGroupIdentifier = _model.MyDepartments[0].Identifier;
        }

        private bool ValidateModel()
        {
            if (_model.EmployerGroupIdentifier == null)
            {
                CreateDetail.Visible = false;
                StatusAlert.AddMessage(AlertType.Error, "No employer is selected for your account. Please contact your administrator to resolve this.");
                return false;
            }

            return true;
        }

        private void BindModelToControls()
        {
            if (!ValidateModel())
                return;

            if (_model.Employer == null)
                return;

            BindDefaultDepartment();
        }

        /// <summary>
        /// If I am assigned to one department only, then my department must be selected by default - and I am not permitted to modify my selection.
        /// </summary>
        private void BindDefaultDepartment()
        {
            _model.DefaultDepartmentGroupIdentifier = Guid.Empty;

            OnDepartmentSelected();
        }

        private void OnDepartmentSelected()
        {
            Details.SetUserData(_model.Employer);
            MultiView.SetActiveView(DefaultView);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var job = new TOpportunity
            {
                OrganizationIdentifier = Organization.Identifier,
                EmployerUserIdentifier = User.UserIdentifier,
                WhenCreated = DateTimeOffset.Now
            };

            Details.GetInputValues(job);

            TOpportunityStore.Insert(job);

            HttpResponseHelper.Redirect($"{_model.OutlineUrl}?id={job.OpportunityIdentifier}");
        }
    }
}