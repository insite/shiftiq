using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.UI.Portal.Assessments.Attempts.Utilities;

using Shift.Common;

namespace InSite.UI.Portal.Assessments.Attempts.Controls
{
    public partial class AnswerQuestionNav : BaseUserControl
    {
        [Serializable]
        private class NavItemInfo
        {
            public int QuestionsCount { get; }

            public NavItemInfo(int questions)
            {
                QuestionsCount = questions;
            }
        }

        private class NavItemData
        {
            public AttemptSectionInfo Section { get; set; }
            public NavItemControls Controls { get; set; }
        }

        private class NavItemControls
        {
            public NavItem NavItem { get; set; }
            public Container Container { get; set; }
            public LiteralControl Alert { get; set; }
            public HtmlGenericControl Summary { get; set; }
            public List<AnswerQuestionOutput> Outputs { get; set; }
        }

        public string NavClientID => Nav.ClientID;

        public int NavItemsCount => _data.Length;

        private List<NavItemInfo> NavItems
        {
            get => (List<NavItemInfo>)ViewState[nameof(NavItems)];
            set => ViewState[nameof(NavItems)] = value;
        }

        private NavItemData[] _data;

        protected override void CreateChildControls()
        {
            if (NavItems.IsNotEmpty() && Nav.ItemsCount == 0)
            {
                foreach (var item in NavItems)
                    AddSectionNavItem(item);
            }

            base.CreateChildControls();
        }

        public void LoadData(AttemptInfo attempt, bool includeQuestionSeparator)
        {
            var lang = attempt.Attempt.AttemptLanguage;

            _data = attempt.GetSections().Select(x => new NavItemData
            {
                Section = x
            }).ToArray();

            Nav.ClearItems();
            NavItems = new List<NavItemInfo>();

            for (var i = 0; i < _data.Length; i++)
            {
                var item = _data[i];
                var navItemInfo = new NavItemInfo(item.Section.Questions.Count);
                var controls = item.Controls = AddSectionNavItem(navItemInfo);
                var questions = item.Section.Questions;

                string title = null, summary = null;

                if (item.Section.BankSection != null)
                {
                    var content = item.Section.BankSection.Content;
                    title = content.Title.Get(lang);
                    summary = content.Summary.Get(lang);

                    if (summary.IsNotEmpty())
                        summary = attempt.GetHtml(item.Section.Questions.FirstOrDefault(), summary);
                }

                controls.NavItem.Title = title.IfNullOrEmpty("(Untitled)");
                controls.Summary.Visible = summary.IsNotEmpty();
                controls.Summary.InnerHtml = summary;

                if (item.Section.AttemptSection?.IsBreakTimer == true)
                {
                    var message = item.Section.AttemptSection.TimeLimit > 0
                        ? $"Your assessment has been paused for {item.Section.AttemptSection.TimeLimit.Value.Minutes().Humanize()}."
                        : "Your assessment has been paused.";

                    controls.Alert.Text = "<div class=\"alert alert-warning\">" +
                        "<i class=\"far fa-exclamation-triangle me-2\"></i>" + message +
                        "</div>";
                }

                for (var k = 0; k < item.Section.Questions.Count; k++)
                    controls.Outputs[k].LoadData(attempt, questions[k], includeQuestionSeparator);

                NavItems.Add(navItemInfo);
            }
        }

        public void SetActiveSection(int index)
        {
            var activeNavItem = _data.FirstOrDefault(x => x.Section.AttemptSection.SectionIndex == index)
                ?? _data[0];

            activeNavItem.Controls.NavItem.IsSelected = true;
        }

        public void SetActiveQuestion(int index)
        {
            AnswerQuestionOutput activeOutput = null;
            foreach (var output in _data.SelectMany(x => x.Controls.Outputs))
            {
                output.Visible = false;
                if (output.QuestionIndex <= index)
                    activeOutput = output;
            }

            if (activeOutput != null)
                activeOutput.Visible = true;
        }

        public int GetQuestionNumber(int index)
        {
            var output = _data.SelectMany(x => x.Controls.Outputs).LastOrDefault(x => x.QuestionIndex <= index && x.QuestionNumber.HasValue);

            return output != null ? output.QuestionNumber.Value : -1;
        }

        public int GetQuestionCount()
        {
            return _data.SelectMany(x => x.Controls.Outputs).Count();
        }

        public void HideInactive()
        {
            foreach (var item in _data)
            {
                if (!item.Controls.NavItem.IsSelected)
                    item.Controls.Container.Visible = false;
            }
        }

        public (int Index, string Html) GetSectionData(int navItemIndex)
        {
            var item = _data[navItemIndex];
            var container = item.Controls.Container;
            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                var htmlWriter = new HtmlTextWriter(writer);
                container.RenderControl(htmlWriter);
            }

            return (item.Section.AttemptSection.SectionIndex, result.ToString());
        }

        private NavItemControls AddSectionNavItem(NavItemInfo info)
        {
            var result = new NavItemControls();

            result.NavItem = new NavItem();
            Nav.AddItem(result.NavItem);

            result.Container = new Container();
            result.NavItem.Controls.Add(result.Container);

            result.Alert = new LiteralControl();
            result.Container.Controls.Add(result.Alert);

            result.Summary = new HtmlGenericControl { TagName = "div" };
            result.Summary.Attributes["class"] = "mt-2 mb-4";
            result.Container.Controls.Add(result.Summary);

            result.Outputs = new List<AnswerQuestionOutput>();

            for (var i = 0; i < info.QuestionsCount; i++)
            {
                var output = (AnswerQuestionOutput)LoadControl("~/UI/Portal/Assessments/Attempts/Controls/AnswerQuestionOutput.ascx");
                result.Container.Controls.Add(output);
                result.Outputs.Add(output);
            }

            return result;
        }
    }
}