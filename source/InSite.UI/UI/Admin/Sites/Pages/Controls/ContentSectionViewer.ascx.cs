using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class ContentSectionViewer : SectionBase
    {
        public sealed class SectionSettings : AssetContentSection.ContentSectionViewer
        {
            public override string ControlPath => "~/UI/Admin/Sites/Pages/Controls/ContentSectionViewer.ascx";

            public SectionSettings(string id)
                : base(id)
            {

            }
        }

        protected string Title
        {
            get => (string)ViewState[nameof(Title)];
            set => ViewState[nameof(Title)] = value;
        }

        protected string Description
        {
            get => (string)ViewState[nameof(Description)];
            set => ViewState[nameof(Description)] = value;
        }

        private MultilingualString Value
        {
            get => (MultilingualString)ViewState[nameof(Value)];
            set => ViewState[nameof(Value)] = value;
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        public override void SetOptions(AssetContentSection options)
        {
            if (!(options is SectionSettings data))
                throw new NotImplementedException("Section type: " + options.GetType().FullName);

            Title = data.Label;
            Description = data.Description;
            EditLink.NavigateUrl = data.EditUrl;
            Value = data.Value.Clone();

            var languageValues = data.Value
                .Select(x => new
                {
                    Language = x.Key,
                    Text = LoadHtml(x.Value)
                })
                .OrderBy(x => string.Equals(x.Language, "en", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(x => x.Language)
                .ToList();

            ValueRepeater.DataSource = languageValues;
            ValueRepeater.DataBind();
        }

        public override void SetValidationGroup(string groupName)
        {

        }

        public override MultilingualString GetValue()
        {
            return Value.Clone();
        }

        public override MultilingualString GetValue(string id) => throw new NotImplementedException();

        public override IEnumerable<MultilingualString> GetValues() => throw new NotImplementedException();

        public override void GetValues(MultilingualDictionary dictionary) => throw new NotImplementedException();

        public override void OpenTab(string id) { }

        public override void SetLanguage(string lang) { }

        private string LoadHtml(string item)
        {
            var html = string.Empty;
            if (!string.IsNullOrWhiteSpace(item))
                html = Markdown.ToHtml(item); // return item.Replace("\r\n", "<br>");
            if (string.IsNullOrWhiteSpace(html))
                html = "(No Content)";
            return html;
        }
    }
}