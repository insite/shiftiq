using System;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Programs
{
    public partial class Describe : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? ProgramIdentifier => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var program = ProgramIdentifier.HasValue ? ProgramSearch.GetProgram(ProgramIdentifier.Value) : null;
                if (program == null)
                    Search.Redirect();

                PageHelper.AutoBindHeader(this, null, program.ProgramName);

                ProgramCode.Text = program.ProgramCode;
                ProgramName.Text = program.ProgramName;
                ProgramTag.Text = program.ProgramTag;
                DepartmentIdentifier.Value = program.GroupIdentifier;
                IsHidden.Checked = program.IsHidden;
                ProgramDescription.Text = program.ProgramDescription;

                CancelButton.NavigateUrl = Outline.GetNavigateUrl(ProgramIdentifier.Value);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
                return;

            var program = ProgramSearch.GetProgram(ProgramIdentifier.Value);

            DetectChanges(program);

            ProgramStore.Update(program, User.Identifier);

            Outline.Redirect(ProgramIdentifier.Value);
        }

        private void DetectChanges(TProgram program)
        {
            if (!StringHelper.Equals(program.ProgramCode, ProgramCode.Text))
                program.ProgramCode = ProgramCode.Text;

            if (!StringHelper.Equals(program.ProgramName, ProgramName.Text))
                program.ProgramName = ProgramName.Text;

            if (!StringHelper.Equals(program.ProgramTag, ProgramTag.Text))
                program.ProgramTag = ProgramTag.Text;

            if (program.GroupIdentifier != DepartmentIdentifier.Value)
                program.GroupIdentifier = DepartmentIdentifier.Value;

            if (program.IsHidden != IsHidden.Checked)
                program.IsHidden = IsHidden.Checked;

            if (!StringHelper.Equals(program.ProgramCode, ProgramDescription.Text))
                program.ProgramDescription = ProgramDescription.Text;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramIdentifier}"
                : null;
        }
    }
}
