using System;
using System.Collections.Generic;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Sections.Controls
{
    public partial class QuestionList : BaseUserControl
    {
        #region Classes

        private class CriterionInfo
        {
            #region Properties

            public string Title { get; private set; }

            public List<Question> Questions { get; }

            #endregion

            #region Construction

            public CriterionInfo(Criterion criterion)
            {
                Title = criterion.ToString();
                Questions = new List<Question>();
            }

            #endregion
        }

        #endregion

        #region Fields

        private Dictionary<Guid, int> _questionIndexes;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriterionRepeater.ItemDataBound += CriterionRepeater_ItemDataBound;
        }

        #endregion

        #region Public methods

        public void LoadData(Criterion sieve, List<Question> questions)
        {
            LoadData(new Tuple<Criterion, List<Question>>[]
            {
                new Tuple<Criterion, List<Question>>(sieve, questions)
            });
        }

        public void LoadData(IEnumerable<Tuple<Criterion, List<Question>>> sieves)
        {
            var sieveInfos = new List<CriterionInfo>();

            _questionIndexes = new Dictionary<Guid, int>();

            foreach (var sieveTuple in sieves)
            {
                var sieveInfo = new CriterionInfo(sieveTuple.Item1);

                sieveInfos.Add(sieveInfo);

                foreach (var question in sieveTuple.Item2)
                {
                    _questionIndexes.Add(question.Identifier, _questionIndexes.Count);
                    sieveInfo.Questions.Add(question);
                }
            }

            CriterionRepeater.DataSource = sieveInfos;
            CriterionRepeater.DataBind();
        }

        #endregion

        #region Event handlers

        private void CriterionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var criterion = (CriterionInfo)e.Item.DataItem;
            var hasQuestions = criterion.Questions.Count > 0;
            var questionRepeater = (QuestionRepeater)e.Item.FindControl("QuestionRepeater");
            var noQuestions = (HtmlGenericControl)e.Item.FindControl("NoQuestions");

            questionRepeater.Visible = hasQuestions;
            noQuestions.Visible = !hasQuestions;

            if (hasQuestions)
            {
                questionRepeater.LoadData(criterion.Questions, new QuestionRepeater.BindSettings
                {
                    GetFormIndex = q => _questionIndexes[q.Identifier]
                });
            }
        }

        #endregion
    }
}