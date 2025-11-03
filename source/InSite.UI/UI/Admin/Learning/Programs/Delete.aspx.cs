using System;

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
                    Search.Redirect();

                ProgramDetails.BindProgram(program);

                LearnersCount.Text = program.Enrollments?.Count.ToString();
                TasksCount.Text = program.Tasks?.Count.ToString();

                CancelButton.NavigateUrl = Outline.GetNavigateUrl(ProgramID.Value);

                PageHelper.AutoBindHeader(Page, null, program.ProgramName);
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            ProgramStore.Delete(ProgramID.Value);
            Search.Redirect();
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramID}"
                : null;
        }
    }
}