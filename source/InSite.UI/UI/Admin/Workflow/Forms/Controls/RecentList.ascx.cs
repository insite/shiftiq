using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Admin.Surveys.Controls
{
    public partial class RecentList : BaseUserControl
    {
        public int ItemCount
        {
            get => (int?)ViewState[nameof(ItemCount)] ?? 0;
            set => ViewState[nameof(ItemCount)] = value;
        }

        public void LoadData(int count)
        {
            var data = ServiceLocator.SurveySearch.GetSurveyForms(new QSurveyFormFilter
            {
                OrganizationIdentifier = Organization.Identifier,
                OrderBy = "LastChangeTime DESC",
                Paging = Paging.SetSkipTake(0, count)
            });

            ItemCount = data.Count;

            SurveyRepeater.DataSource = data;
            SurveyRepeater.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            SurveyRepeater.ItemDataBound += Repeater_ItemDataBound;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var survey = (QSurveyForm)e.Item.DataItem;
            var filter = new QResponseSessionFilter 
            { 
                OrganizationIdentifier = Organization.Identifier,
                SurveyFormIdentifier = survey.SurveyFormIdentifier 
            };
            var countResponses = ServiceLocator.SurveySearch.CountResponseSessions(filter);
            var link = (ITextControl)e.Item.FindControl("ResponseAnalysisLink");
            link.Text = $"<a href='/ui/admin/workflow/forms/submissions/search?form={survey.SurveyFormIdentifier}'><i class='far fa-search me-1'></i>Submission Search ({countResponses:n0})</a>";

            var lastChange = (ITextControl)e.Item.FindControl("LastChange");
            lastChange.Text = UserSearch.GetTimestampHtml(survey.LastChangeUser, survey.LastChangeType, null, survey.LastChangeTime);
        }
    }
}