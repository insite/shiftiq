using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contents.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Assets.Contents.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/accounts/organizations/search";

        private Guid ContentId => Guid.TryParse(Request.QueryString["id"], out var id) ? id : Guid.Empty;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
            TranslateButton.Click += TranslateButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                ContentLanguage.Settings.IncludeLanguage = OrganizationSearch.Select(Organization.Identifier).Languages.Select(x => x.TwoLetterISOLanguageName).ToArray();
                ContentLanguage.RefreshData();

                CancelButton.NavigateUrl = SearchUrl;

                Open();
            }

            base.OnLoad(e);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            Save();

            SetStatus(EditorStatus, StatusType.Saved);
        }

        protected void TranslateButton_Click(object sender, EventArgs e)
        {
            var _content = ServiceLocator.ContentSearch.Select(ContentId);
            var contents = ServiceLocator.ContentSearch.SelectContainer(_content.ContainerIdentifier)
                .OrderBy(x => x.ContentLabel).ThenBy(x => x.ContentLanguage).ToList();
            var languages = OrganizationSearch.Select(Organization.Identifier).Languages.Select(x => x.TwoLetterISOLanguageName)
                .Where(x => x != "en").ToList();

            var results = new List<TContent>();

            foreach (var content in contents.Where(x => x.ContentLanguage == "en").ToList())
            {
                var contentLanguages = contents.Where(x => x.ContentLabel == content.ContentLabel
                                        && x.ContainerType == content.ContainerType
                                        && x.ContentLanguage != "en").ToList();
                if (contentLanguages.Count < languages.Count)
                {
                    results.AddRange(AddTranslation(content, languages, contentLanguages));
                }
            }
            ServiceLocator.ContentStore.Save(results);

            SetStatus(EditorStatus, StatusType.Translated);

            LabelSearch.Refresh();
            Open();
        }

        private List<TContent> AddTranslation(TContent content, List<string> languages, List<TContent> contentLanguages)
        {
            var results = new List<TContent>();
            foreach (var language in languages)
            {
                var _content = contentLanguages.Where(x => x.ContentLanguage == language).FirstOrDefault();
                if (_content == null)
                {
                    _content = new TContent()
                    {
                        ContentLanguage = language,
                        ContainerType = content.ContainerType,
                        ContentLabel = content.ContentLabel,
                        ContentText = Translate("en", language, content.ContentText),
                        ContentSnip = Translate("en", language, content.ContentSnip),
                        OrganizationIdentifier = content.OrganizationIdentifier,
                        ContainerIdentifier = content.ContainerIdentifier,
                        ContentIdentifier = UniqueIdentifier.Create()
                    };
                    results.Add(_content);
                }
            }
            return results;
        }

        private void Open()
        {
            var content = ServiceLocator.ContentSearch.Select(ContentId);
            if (content == null)
                HttpResponseHelper.Redirect(SearchUrl);

            PageHelper.AutoBindHeader(this, null, content.ContentLabel);

            OrganizationIdentifier.Value = content.OrganizationIdentifier;
            ContainerIdentifier.Text = content.ContainerIdentifier.ToString();
            ContentLabel.Text = content.ContentLabel;
            ContentLanguage.Value = content.ContentLanguage;
            ContentText.Text = content.ContentText;
            ContentHtml.Text = content.ContentHtml;
            ContentIdentifier.Text = content.ContentIdentifier.ToString();
            ContainerType.Text = content.ContainerType;

            var contents = ServiceLocator.ContentSearch.SelectContainer(content.ContainerIdentifier)
                .Where(x => x.ContentIdentifier != content.ContentIdentifier).OrderBy(x => x.ContentLabel).ThenBy(x => x.ContentLanguage).ToList();

            TranslationRepeater.DataSource = contents;
            TranslationRepeater.DataBind();
        }

        private void Save()
        {
            var content = ServiceLocator.ContentSearch.Select(ContentId);

            content.OrganizationIdentifier = OrganizationIdentifier.Value.Value;
            content.ContainerIdentifier = Guid.Parse(ContainerIdentifier.Text);
            content.ContentIdentifier = Guid.Parse(ContentIdentifier.Text);
            content.ContentLabel = ContentLabel.Text;
            content.ContentLanguage = ContentLanguage.Value;
            content.ContentText = ContentText.Text;
            content.ContentHtml = ContentHtml.Text;
            content.ContainerType = ContainerType.Text;

            ServiceLocator.ContentStore.Save(content);
            LabelSearch.Refresh();
        }
    }
}