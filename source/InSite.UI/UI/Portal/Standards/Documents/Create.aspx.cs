using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Portal;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using DocumentTypeConst = Shift.Sdk.UI.DocumentType;

namespace InSite.UI.Portal.Standards.Documents
{
    public partial class Create : PortalBasePage, IHasParentLinkParameters
    {
        private string DefaultDocumentType => Request.QueryString["type"];
        private string PageAction => Request.QueryString["action"];

        private bool CopyContent
        {
            get => (bool)(ViewState[nameof(CopyContent)] ?? false);
            set => ViewState[nameof(CopyContent)] = value;
        }

        private bool CopyCompetencies
        {
            get => (bool)(ViewState[nameof(CopyCompetencies)] ?? false);
            set => ViewState[nameof(CopyCompetencies)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DocumentType.AutoPostBack = true;
            DocumentType.ValueChanged += DocumentType_ValueChanged;

            BaseStandardTypeSelector.AutoPostBack = true;
            BaseStandardTypeSelector.ValueChanged += BaseStandardTypeSelector_ValueChanged;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void DocumentType_ValueChanged(object sender, EventArgs e)
            => OnDocumentType();

        private void OnDocumentType()
        {
            var docType = DocumentType.Value;

            var isJobDescription = docType == DocumentTypeConst.JobDescription;
            var isSkillsChecklist = docType == DocumentTypeConst.SkillsChecklist;
            var isCustomizedSkillsChecklist = docType == DocumentTypeConst.CustomizedSkillsChecklist;
            var isCustomizedOccupationProfile = docType == DocumentTypeConst.CustomizedOccupationProfile;
            var isOccupationProfile = docType == DocumentTypeConst.OccupationProfile;

            var isDerivedType = isJobDescription
                || isSkillsChecklist
                || isCustomizedSkillsChecklist
                || isCustomizedOccupationProfile
                || isOccupationProfile;

            CopyContent = isCustomizedOccupationProfile;
            CopyCompetencies = isDerivedType;
            BaseStandardField.Visible = isDerivedType;

            BaseStandardTypeSelector.Visible = false;
            BaseStandardTypeSelector.ClearSelection();

            BaseStandardSelector.Visible = true;
            BaseStandardSelector.Value = null;
            BaseStandardSelector.Filter.StandardTypes = new[] { Shift.Constant.StandardType.Document };
            BaseStandardSelector.Filter.IsPortal = true;
            BaseStandardSelector.Filter.CreatedBy = User.UserIdentifier;

            if (isJobDescription || isSkillsChecklist || isOccupationProfile)
            {
                var baseStandardTypes = new List<string>();

                if (isOccupationProfile || isJobDescription && Organization.Toolkits.Standards.EnableOccupationProfileForJobDescription)
                    baseStandardTypes.Add(DocumentTypeConst.OccupationProfile);
                else
                    baseStandardTypes.Add(DocumentTypeConst.CustomizedOccupationProfile);

                baseStandardTypes.Add(DocumentTypeConst.NationalOccupationStandard);

                BaseStandardLabel.Text = Translate("Document Based On");
                BaseStandardTypeSelector.Visible = true;
                BaseStandardTypeSelector.LoadItems(baseStandardTypes);

                Translate(BaseStandardTypeSelector);

                OnBaseStandardTypeSelected();
            }
            else if (isCustomizedOccupationProfile)
            {
                BaseStandardLabel.Text = Translate(DocumentTypeConst.NationalOccupationStandard);
                BaseStandardSelector.Filter.DocumentType = new[] { DocumentTypeConst.NationalOccupationStandard };
            }
            else if (isCustomizedSkillsChecklist)
            {
                BaseStandardLabel.Text = Translate(DocumentTypeConst.SkillsChecklist);
                BaseStandardSelector.Filter.DocumentType = new[] { DocumentTypeConst.SkillsChecklist };
            }

            if (isDerivedType)
                BaseStandardSelector.Value = null;
        }

        private void BaseStandardTypeSelector_ValueChanged(object sender, EventArgs e)
        {
            OnBaseStandardTypeSelected();
        }

        private void OnBaseStandardTypeSelected()
        {
            var hasType = BaseStandardTypeSelector.Value.IsNotEmpty();

            BaseStandardSelector.Visible = hasType;
            BaseStandardSelector.Value = null;

            if (hasType)
                BaseStandardSelector.Filter.DocumentType = new[]
                {
                    BaseStandardTypeSelector.Value
                };
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
                return;

            var document = StandardFactory.Create(Shift.Constant.StandardType.Document);
            document.StandardIdentifier = UniqueIdentifier.Create();
            document.Language = CurrentSessionState.Identity.Language;
            document.DocumentType = DocumentType.Value;

            var isCopDoc = document.DocumentType == DocumentTypeConst.CustomizedOccupationProfile;

            var content = new ContentContainer();
            var baseStandardIdentifier = BaseStandardSelector.Value;
            var hasBaseStandard = baseStandardIdentifier.HasValue;

            if (hasBaseStandard && CopyContent)
            {
                var baseStandardId = StandardSearch.BindFirst(x => x.StandardIdentifier, x => x.StandardIdentifier == baseStandardIdentifier.Value);
                var baseContent = ServiceLocator.ContentSearch.GetBlock(baseStandardId);

                if (isCopDoc)
                {
                    const string key1 = "Copyright";
                    const string key2 = "StandardsDevelopmentProcess";
                    const string key3 = "CertificationDevelopmentProcess";


                    content[key1] = baseContent[key1];
                    content[key2] = baseContent[key2];
                    content[key3] = baseContent[key3];

                    if (document.DocumentType.Equals("Customized Occupation Profile"))
                    {
                        const string key4 = "Job Definition";
                        content[key4] = baseContent[key4];
                    }

                    if (content[key1].IsEmpty)
                        content[key1].Html.Default = GetEmbededHelpContent("#" + key1);

                    if (content[key2].IsEmpty)
                        content[key2].Html.Default = GetEmbededHelpContent("#" + key2);

                    if (content[key3].IsEmpty)
                        content[key3].Html.Default = GetEmbededHelpContent("#" + key3);
                }
                else
                {
                    content = baseContent;
                }
            }
            else if (hasBaseStandard && document.DocumentType.Equals("Job Description"))
            {
                var baseStandardId = StandardSearch.BindFirst(x => x.StandardIdentifier, x => x.StandardIdentifier == baseStandardIdentifier.Value);
                var baseContent = ServiceLocator.ContentSearch.GetBlock(baseStandardId);

                const string keyFrom = "Job Definition";
                const string keyTo = "Purpose of Job";

                content[keyTo] = baseContent[keyFrom];
            }
            else
            {
                content = new ContentContainer();
            }

            content.Title.Text.Default = TitleInput.Text;

            if (!string.Equals(document.Language, "en", StringComparison.OrdinalIgnoreCase))
                content.Title.Text[document.Language] = TitleInput.Text;

            if (isCopDoc)
            {
                var contentSettings = new ContentSettings();
                contentSettings.Locked.Add("Copyright");
                contentSettings.Locked.Add("StandardsDevelopmentProcess");
                contentSettings.Locked.Add("CertificationDevelopmentProcess");
                document.ContentSettings = contentSettings.Serialize();
            }

            StandardStore.Insert(document, content);

            if (hasBaseStandard && CopyCompetencies)
            {
                var containments = StandardContainmentSearch.SelectCompetencyContainments(baseStandardIdentifier.Value);
                if (containments.Count > 0)
                {
                    foreach (var containment in containments)
                        containment.ParentStandardIdentifier = document.StandardIdentifier;

                    StandardContainmentStore.Insert(containments);
                }
            }

            var url = $"/ui/portal/standards/documents/outline?standard={document.StandardIdentifier}";

            if (PageAction.IsNotEmpty() && PageAction.Equals("Job Comparison Tool"))
                url += string.Format("&action={0}", PageAction);

            HttpResponseHelper.Redirect(url);
        }

        private void LoadData()
        {
            BuildBreadcrumbs();

            LoadDocumentTypes();

            if (DefaultDocumentType.IsNotEmpty())
            {
                foreach (ComboBoxOption option in DocumentType.Items)
                {
                    if (option.Value != null && option.Value.Equals(DefaultDocumentType, StringComparison.OrdinalIgnoreCase))
                    {
                        option.Selected = true;

                        DocumentTypeField.Visible = false;

                        OnDocumentType();

                        break;
                    }
                }
            }

            if (PageAction.IsNotEmpty())
            {
                if (PageAction.Equals("Job Comparison Tool"))
                    CancelButton.NavigateUrl = string.Format("/ui/portal/standards/documents/analysis?action={0}", PageAction);
            }

            if (string.IsNullOrEmpty(CancelButton.NavigateUrl))
            {
                CancelButton.NavigateUrl = !string.IsNullOrEmpty(DefaultDocumentType)
                    ? $"/ui/portal/standards/documents/search?type={DefaultDocumentType}"
                    : $"/ui/portal/standards/documents/search";
            }

            CancelButton.NavigateUrl = AddFolderToUrl(CancelButton.NavigateUrl);
        }

        private void BuildBreadcrumbs()
        {
            var parentLinkTitle = Search.GetPageTitle(DefaultDocumentType, Translator);
            AutoBindFolderHeader(null, null, null, new[] { parentLinkTitle });
        }

        private void LoadDocumentTypes()
        {
            var collectionName = CollectionName.Standards_Document_Type;

            var items = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                CollectionName = collectionName
            });

            if (items.Count == 0)
                items = TCollectionItemCache.Select(new TCollectionItemFilter
                {
                    OrganizationIdentifier = OrganizationIdentifiers.Global,
                    CollectionName = collectionName
                });

            var itemNames = items.Select(x => x.ItemName).ToArray();

            DocumentType.LoadItems(itemNames);

            Translate(DocumentType);
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/search") && !string.IsNullOrEmpty(DefaultDocumentType)
                ? $"type={HttpUtility.UrlEncode(DefaultDocumentType)}"
                : null;
        }
    }
}