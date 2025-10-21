using System;
using System.Collections.Generic;
using System.Web.UI;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;

using Shift.Common;
using Shift.Sdk.UI;
namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class ContentSectionHtmlCode : SectionBase
    {
        public sealed class SectionSettings : AssetContentSection.TextSection
        {
            public override string ControlPath => "~/UI/Admin/Sites/Pages/Controls/ContentSectionHtmlCode.ascx";

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

        protected override void OnInit(EventArgs e)
        {
            CommonScript.ContentKey = typeof(ContentSectionHtmlCode).FullName;

            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            RequiredValidator.ErrorMessage = string.IsNullOrEmpty(Title)
                ? string.Empty
                : $"Required field: {Title}";

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(ContentSectionHtmlCode),
                "init",
                $"sectionHtmlCode.init('textarea#{Value.ClientID}');",
                true);

            base.OnPreRender(e);
        }

        public override void SetOptions(AssetContentSection options)
        {
            if (options is SectionSettings data)
            {
                Title = data.Label;
                Description = data.Description;
                RequiredValidator.Visible = data.IsRequired;
                Value.AllowHtml = true;
                Value.Text = data.Value.Default;
            }
            else
            {
                throw new NotImplementedException("Section type: " + options.GetType().FullName);
            }
        }

        public override void SetValidationGroup(string groupName)
        {
            RequiredValidator.ValidationGroup = groupName;
        }

        public override MultilingualString GetValue()
        {
            var result = new MultilingualString();

            result.Default = Value.Text;

            return result;
        }

        public override MultilingualString GetValue(string id) => throw new NotImplementedException();

        public override IEnumerable<MultilingualString> GetValues() => throw new NotImplementedException();

        public override void GetValues(MultilingualDictionary dictionary) => throw new NotImplementedException();

        public override void OpenTab(string id) { }

        public override void SetLanguage(string lang) { }
    }
}