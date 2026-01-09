using System;
using System.Web;
using System.Web.UI;

using Shift.Common;

namespace InSite.Admin.Records.Programs.Controls
{
    public partial class ProgramContent : UserControl
    {
        private class SectionInfo
        {
            public string Title { get; }
            public string Label { get; }

            public SectionInfo(string title, string label)
            {
                Title = title;
                Label = label;
            }
        }

        private ContentContainer Content
        {
            get => (ContentContainer)ViewState[nameof(Content)];
            set => ViewState[nameof(Content)] = value;
        }

        private static readonly SectionInfo[] _sections = new[]
        {
            new SectionInfo("Title", "Title"),
            new SectionInfo("Summary", "Summary"),
        };

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Language.AutoPostBack = true;
            Language.ValueChanged += (s, a) => ContentRepeater.DataBind();

            ContentRepeater.DataBinding += (s, a) => ContentRepeater.DataSource = _sections;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            Language.LoadItems(CurrentSessionState.Identity.Organization.Languages, "TwoLetterISOLanguageName", "EnglishName");
        }

        public void LoadData(Guid programId)
        {
            EditLink.NavigateUrl = ModifyContent.GetNavigateUrl(programId);

            Content = ServiceLocator.ContentSearch.GetBlock(programId)
                ?? new ContentContainer();
            ContentRepeater.DataBind();
        }

        protected string GetText(string label)
        {
            var lang = Language.Value.IfNullOrEmpty(Shift.Common.Language.Default);
            var text = Content[label].GetText(lang);

            return text.IsNotEmpty()
                ? $"<div style='white-space:pre-wrap;'>{HttpUtility.HtmlEncode(text)}</div>"
                : "<div class='fst-italic'>None</div>";
        }
    }
}