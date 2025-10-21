using System;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Programs
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? ProgramID => Guid.TryParse(Request["id"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            DeleteButton.Click += DeleteButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var program = ProgramID.HasValue ? ProgramSearch.GetProgram(ProgramID.Value, x => x.Tasks, x => x.Enrollments) : null;
                if (program == null || program.OrganizationIdentifier != Organization.Identifier)
                    HttpResponseHelper.Redirect($"/ui/admin/learning/programs/search");

                ProgramDetails.BindProgram(program);

                LearnersCount.Text = program.Enrollments?.Count.ToString();
                TasksCount.Text = program.Tasks?.Count.ToString();

                CancelButton.NavigateUrl = $"/ui/admin/learning/programs/outline?id={ProgramID}";

                PageHelper.AutoBindHeader(Page, null, program.ProgramName);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ProgramStore.Delete(ProgramID.Value);
            HttpResponseHelper.Redirect($"/ui/admin/learning/programs/search");
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramID}"
                : null;
        }
    }
}