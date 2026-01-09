using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Domain.Foundations;
using InSite.Persistence.Content;

using Shift.Common;

namespace InSite.UI.Portal.Learning.Programs.Controls
{
    public partial class ProgramCardRepeater : BaseUserControl
    {
        public delegate void ProgramsSubmittedHandler(List<Guid> ids);
        public event ProgramsSubmittedHandler ProgramsSubmitted;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SubmitButton.Click += SubmitButton_Click;
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            var idsText = !string.IsNullOrEmpty(SubmittedProgramIds.Value)
                ? SubmittedProgramIds.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : new string[0];

            var ids = idsText
                .Select(x => Guid.Parse(x))
                .ToList();

            ProgramsSubmitted?.Invoke(ids);
        }

        public void BindModelToControls(List<LaunchCard> cards, ISecurityFramework identity)
        {
            CardRepeater.DataSource = cards;
            CardRepeater.DataBind();
        }

        protected string GetProgramDescription()
        {
            var item = (LaunchCard)Page.GetDataItem();
            var summary = !string.IsNullOrEmpty(item.Summary) ? item.Summary : "No Description";

            return HttpUtility.HtmlEncode(Markdown.ToHtml(summary));
        }
    }
}