using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Linq;

namespace InSite.Admin.Records.Reports.LearnerActivity.Controls
{
    public partial class SearchResults : SearchResultsGridViewController<VLearnerActivityFilter>
    {
        public class ExportDataItem
        {
            public string UserFullName { get; set; }
            public string UserEmail { get; set; }
            public string UserGender { get; set; }
            public string UserPhone { get; set; }
            public DateTime? UserBirthdate { get; set; }
            public string PersonCode { get; set; }
            public string ProgramName { get; set; }
            public string Gradebook { get; set; }
            public string AchievementGranted { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.RowDataBound += Grid_RowDataBound;
        }

        private void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (var i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "Person Code")
                        e.Row.Cells[i].Text = LabelHelper.GetLabelContentText("Person Code");
                }
            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var item = (VLearnerActivityUserProgram)e.Row.DataItem;
                
                var gradebookRepeater = (Repeater)e.Row.FindControl("GradebookRepeater");
                gradebookRepeater.Visible = item.Gradebooks.Count > 0;
                gradebookRepeater.DataSource = item.Gradebooks;
                gradebookRepeater.DataBind();

                var achievementRepeater = (Repeater)e.Row.FindControl("AchievementRepeater");
                achievementRepeater.Visible = item.Credentials.Count > 0;
                achievementRepeater.DataSource = item.Credentials;
                achievementRepeater.DataBind();
            }
        }

        protected override int SelectCount(VLearnerActivityFilter filter)
        {
            return VLearnerActivitySearch.Count(filter);
        }

        protected override IListSource SelectData(VLearnerActivityFilter filter)
        {
            return VLearnerActivitySearch.GetUserPrograms(filter).ToSearchResult();
        }

        public override IListSource GetExportData(VLearnerActivityFilter filter, bool empty)
        {
            if (empty)
                return new List<ExportDataItem>().ToSearchResult();

            var data = VLearnerActivitySearch.GetUserPrograms(filter);

            return data
                .Select(x => new ExportDataItem
                {
                    UserFullName = x.UserFullName,
                    UserEmail = x.UserEmail,
                    UserGender = x.UserGender,
                    UserPhone = x.UserPhone,
                    UserBirthdate = x.UserBirthdate,
                    PersonCode = x.PersonCode,
                    ProgramName = x.ProgramName,
                    Gradebook = string.Join(", ", x.Gradebooks.Select(y => y.GradebookTitle)),
                    AchievementGranted = string.Join(", ", x.Credentials.Select(y => LocalizeDate(y.CredentialGranted)))
                })
                .ToList()
                .ToSearchResult();
        }
    }
}