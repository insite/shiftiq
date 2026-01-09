using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;

using Shift.Sdk.UI;

namespace InSite.UI.Admin.Assessments.Attempts.Controls
{
    public partial class RubricSummary : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RubricRepeater.ItemDataBound += RubricRepeater_ItemDataBound;
        }

        private void RubricRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var rubric = (GradeRubricItem)e.Item.DataItem;

            var criteriaRepeater = (Repeater)e.Item.FindControl("CriteriaRepeater");
            criteriaRepeater.DataSource = rubric.Criteria.Select(c =>
            {
                var rating = c.SelectedRating;
                var result = new
                {
                    CriterionTitle = c.Title,
                    RatingTitle = rating.Title,
                    Points = rating.SelectedPoints.Value
                };

                return result;
            });
            criteriaRepeater.DataBind();
        }

        public void LoadData(IEnumerable<GradeRubricItem> data)
        {
            RubricRepeater.DataSource = data;
            RubricRepeater.DataBind();
        }
    }
}