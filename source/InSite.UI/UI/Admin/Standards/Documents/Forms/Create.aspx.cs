using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Standards.Documents.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/standards/documents/outline";
        private const string SearchUrl = "/ui/admin/standards/documents/search";

        private class StandardInfo
        {
            public Guid Key { get; set; }
            public Guid Identifier { get; set; }
            public string Title { get; set; }
            public int Sequence { get; set; }
            public ConnectionDirection Direction { get; set; }
        }

        private string Action => Request.QueryString["action"];
        protected Guid? AssetId => Guid.TryParse(Request.QueryString["asset"], out var result) ? result : (Guid?)null;

        private bool CopyContent
        {
            get => (bool)(ViewState[nameof(CopyContent)] ?? false);
            set => ViewState[nameof(CopyContent)] = value;
        }

        private bool CopyCopemtencies
        {
            get => (bool)(ViewState[nameof(CopyCopemtencies)] ?? false);
            set => ViewState[nameof(CopyCopemtencies)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            DocumentType.AutoPostBack = true;
            DocumentType.ValueChanged += DocumentType_ValueChanged;

            DocumentTypeDuplicate.AutoPostBack = true;
            DocumentTypeDuplicate.ValueChanged += DocumentTypeDuplicate_ValueChanged;

            DocumentStandardSelector.AutoPostBack = true;
            DocumentStandardSelector.ValueChanged += DocumentStandardSelector_ValueChanged;

            BaseStandardTypeSelector.AutoPostBack = true;
            BaseStandardTypeSelector.ValueChanged += BaseStandardTypeSelector_ValueChanged;

            SaveButton.Click += SaveButton_Click;

            CopyButton.Click += CopyButton_Click;

            JsonFileUploadExtensionValidator.ServerValidate += JsonFileUploadExtensionValidator_ServerValidate;
            JsonFileUpload.FileUploaded += JsonFileUpload_FileUploaded;
            UploadSaveButton.Click += UploadSaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Upload);

            SectionInfo.BindComboBox(DocumentTypeDuplicate);

            if (Action == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;

                if (AssetId.HasValue)
                {
                    var entity = AssetId.HasValue ? StandardSearch.SelectFirst(x => x.StandardIdentifier == AssetId.Value) : null;
                    if (entity != null)
                    {
                        DocumentStandardSelectorUpdate(entity);
                        DocumentStandardSelectorSelected(entity);

                    }
                }
            }
            else
            {
                DocumentStandardSelectorUpdate();
            }

            PageHelper.AutoBindHeader(this);

            OnCreationTypeSelected();

            SectionInfo.BindComboBox(DocumentType);

            CancelButton.NavigateUrl = SearchUrl;
            CopyCancelButton.NavigateUrl = SearchUrl;
            UploadCancelButton.NavigateUrl = SearchUrl;
        }

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void DocumentStandardSelector_ValueChanged(object sender, EventArgs e)
        {
            DocumentStandardSelectorSelected();
        }

        private void DocumentStandardSelectorSelected(Standard entity = null)
        {
            if (entity != null)
            {
                TitleDuplicate.Text = entity.ContentTitle;
                DocumentTypeDuplicateUpdate(entity);
            }
            else if (DocumentStandardSelector.HasValue)
            {
                var documentStandard = StandardSearch.SelectFirst(x => x.StandardIdentifier == DocumentStandardSelector.Value.Value);
                if (documentStandard != null)
                {
                    TitleDuplicate.Text = documentStandard.ContentTitle;
                    DocumentTypeDuplicateUpdate(documentStandard);
                }
            }
            else
            {
                TitleDuplicate.Text = null;
                DocumentTypeDuplicateUpdate();
            }
        }

        private void DocumentStandardSelectorUpdate(Standard entity = null)
        {
            DocumentStandardSelector.Filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            DocumentStandardSelector.Filter.StandardTypes = new[] { StandardType.Document };
            DocumentStandardSelector.Filter.DocumentType = entity != null
                ? new[] { entity.DocumentType }
                : DocumentTypeDuplicate.Value.IsNotEmpty()
                    ? new[] { DocumentTypeDuplicate.Value }
                    : null;
            DocumentStandardSelector.Value = null;

            TitleDuplicate.Text = null;

            if (entity != null)
                DocumentStandardSelector.Value = entity.StandardIdentifier;
        }

        private void OnCreationTypeSelected()
        {
            var creationType = CreationType.ValueAsEnum;

            NewSection.Visible = creationType == CreationTypeEnum.One;
            DuplicateSection.Visible = creationType == CreationTypeEnum.Duplicate;
            UploadSection.Visible = creationType == CreationTypeEnum.Upload;
        }

        private void DocumentTypeDuplicateUpdate(Standard entity = null)
        {
            DocumentTypeDuplicate.Value = entity?.DocumentType;

            if (Action == "duplicate")
                DocumentTypeDuplicate.Enabled = false;
        }

        private void DocumentTypeDuplicate_ValueChanged(object sender, EventArgs e)
        {
            DocumentStandardSelectorUpdate();
        }

        private void DocumentType_ValueChanged(object sender, EventArgs e)
        {
            var docType = DocumentType.Value;

            var isJobDescription = docType == Shift.Sdk.UI.DocumentType.JobDescription;
            var isSkillsChecklist = docType == Shift.Sdk.UI.DocumentType.SkillsChecklist;
            var isCustomizedSkillsChecklist = docType == Shift.Sdk.UI.DocumentType.CustomizedSkillsChecklist;
            var isCustomizedOccupationProfile = docType == Shift.Sdk.UI.DocumentType.CustomizedOccupationProfile;

            var isDerivedType = isJobDescription
                || isSkillsChecklist
                || isCustomizedSkillsChecklist
                || isCustomizedOccupationProfile;

            CopyContent = isCustomizedOccupationProfile;
            CopyCopemtencies = isDerivedType;
            BaseStandardField.Visible = isDerivedType;

            BaseStandardTypeSelector.Visible = false;
            BaseStandardTypeSelector.Items.Clear();

            BaseStandardSelector.Visible = true;
            BaseStandardSelector.Value = null;
            BaseStandardSelector.Filter.StandardTypes = new[] { StandardType.Document };

            if (isJobDescription || isSkillsChecklist)
            {
                BaseStandardLabel.Text = "Document Based On";
                BaseStandardTypeSelector.Visible = true;
                BaseStandardTypeSelector.LoadItems(new[] {
                    Shift.Sdk.UI.DocumentType.CustomizedOccupationProfile,
                    Shift.Sdk.UI.DocumentType.NationalOccupationStandard
                });

                OnBaseStandardTypeSelected();
            }
            else if (isCustomizedOccupationProfile)
            {
                BaseStandardLabel.Text = Shift.Sdk.UI.DocumentType.NationalOccupationStandard;
                BaseStandardSelector.Filter.DocumentType = new[] { Shift.Sdk.UI.DocumentType.NationalOccupationStandard };
            }
            else if (isCustomizedSkillsChecklist)
            {
                BaseStandardLabel.Text = Shift.Sdk.UI.DocumentType.SkillsChecklist;
                BaseStandardSelector.Filter.DocumentType = new[] { Shift.Sdk.UI.DocumentType.SkillsChecklist };
            }

            if (isDerivedType)
                BaseStandardSelector.Value = null;
        }

        private void BaseStandardTypeSelector_ValueChanged(object sender, EventArgs e) =>
            OnBaseStandardTypeSelected();

        private void OnBaseStandardTypeSelected()
        {
            var baseType = BaseStandardTypeSelector.Value;
            var hasType = baseType.IsNotEmpty();

            BaseStandardSelector.Visible = hasType;
            BaseStandardSelector.Value = null;

            if (hasType)
                BaseStandardSelector.Filter.DocumentType = new[] { baseType };
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            var documentTypeSelected = DocumentTypeDuplicate.Value;
            var documentStandardSelectedID = DocumentStandardSelector.Value;
            var documentTitle = TitleDuplicate.Text;

            if (documentStandardSelectedID != null
                && documentStandardSelectedID.HasValue
                && documentStandardSelectedID != Guid.Empty)
            {
                var entity = ServiceLocator.StandardSearch.GetStandard(documentStandardSelectedID.Value);
                if (entity == null || entity.StandardType != StandardType.Document || entity.OrganizationIdentifier != Organization.Identifier)
                    HttpResponseHelper.Redirect("/ui/admin/standards/documents/search", true);

                var content = ServiceLocator.ContentSearch.GetBlock(documentStandardSelectedID.Value);

                GetCopyInputValues(entity);

                StandardStore.Insert(entity, content);

                var data = StandardContainmentSearch.Bind(
                LinqExtensions1.Expr((StandardContainment x) =>
                Occupations.Utilities.Competencies.StandardInfo.Binder.Invoke(x.Child)).Expand(),
                x => x.ParentStandardIdentifier == documentStandardSelectedID.Value
                  && x.Child.StandardType == StandardType.Competency);

                var relationships = new List<StandardContainment>();

                foreach (var info in data)
                {
                    relationships.Add(new StandardContainment
                    {
                        ParentStandardIdentifier = entity.StandardIdentifier,
                        ChildStandardIdentifier = info.StandardIdentifier
                    });
                }

                if (relationships.Count > 0)
                    StandardContainmentStore.Insert(relationships);

                var outgoingRelationshipData = StandardContainmentSearch
                    .Bind(
                        x => new StandardInfo
                        {
                            Key = x.Child.StandardIdentifier,
                            Identifier = x.Child.StandardIdentifier,
                            Title = x.Child.ContentTitle ?? CoreFunctions.GetContentTextEn(x.Child.StandardIdentifier, ContentLabel.Title),
                            Sequence = x.ChildSequence,
                            Direction = ConnectionDirection.Outgoing
                        },
                        x => x.ParentStandardIdentifier == documentStandardSelectedID.Value
                          && x.Child.StandardType == StandardType.Document,
                        "Sequence,Title").Select(x => new StandardContainment
                        {
                            ParentStandardIdentifier = entity.StandardIdentifier,
                            ChildStandardIdentifier = x.Identifier
                        }).ToList();

                if (outgoingRelationshipData.Count > 0)
                    StandardContainmentStore.Insert(outgoingRelationshipData);

                HttpResponseHelper.Redirect($"/ui/admin/standards/documents/outline?asset={entity.StandardIdentifier}&status=saved", true);
            }
        }

        private void JsonFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null || e.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
        }

        private void JsonFileUpload_FileUploaded(object sender, EventArgs e)
        {
            string text;

            using (var stream = JsonFileUpload.OpenFile())
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    text = reader.ReadToEnd();
            }

            JsonInput.Text = text;
        }

        private void UploadSaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                var json = JsonInput.Text;
                var result = StandardHelper.DeserializeStandard(json);

                if (JsonInput.Text.IsEmpty() || result == null)
                {
                    CreatorStatus.AddMessage(AlertType.Error, $"Wrong JSON file uploaded");
                    return;
                }

                var standardIdentifier = UniqueIdentifier.Create();

                var document = new QStandard
                {
                    StandardIdentifier = standardIdentifier,
                    Sequence = result.Sequence,
                    Language = result.Language,
                    ContentDescription = result.ContentDescription,
                    AssetNumber = result.AssetNumber,
                    AuthorName = result.AuthorName,
                    Code = result.Code,
                    CompetencyScoreCalculationMethod = result.CompetencyScoreCalculationMethod,
                    CompetencyScoreSummarizationMethod = result.CompetencyScoreSummarizationMethod,
                    ContentName = result.ContentName,
                    ContentSettings = result.ContentSettings,
                    ContentSummary = result.ContentSummary,
                    ContentTitle = result.ContentTitle,
                    CreditIdentifier = result.CreditIdentifier,
                    DocumentType = result.DocumentType,
                    Icon = result.Icon,
                    LevelCode = result.LevelCode,
                    LevelType = result.LevelType,
                    SourceDescriptor = result.SourceDescriptor,
                    StandardLabel = result.StandardLabel,
                    StandardPrivacyScope = result.StandardPrivacyScope,
                    StandardTier = result.StandardTier,
                    StandardType = result.StandardType,
                    Tags = result.Tags,
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    DatePosted = DateTimeOffset.UtcNow
                };

                StandardStore.Insert(document, result.Content);

                if (result.Items.IsNotEmpty())
                {
                    List<StandardContainment> standardContainmentsToAdd = new List<StandardContainment>();
                    foreach (var item in result.Items)
                    {
                        var child = StandardSearch.SelectFirst(
                            x => x.ContentTitle == item.Title &&
                            x.Sequence == item.Sequence &&
                            x.AssetNumber == item.AssetNumber &&
                            x.Code == item.Code).StandardIdentifier;
                        standardContainmentsToAdd.Add(new StandardContainment()
                        { ChildStandardIdentifier = child, ParentStandardIdentifier = standardIdentifier });
                    }
                    StandardContainmentStore.Insert(standardContainmentsToAdd);
                }

                //if (result.OutgoingRef.IsNotEmpty())
                //    StandardContainmentStore.Insert(result.OutgoingRef);

                HttpResponseHelper.Redirect($"/ui/admin/standards/documents/outline?asset={standardIdentifier}&status=saved", true);

            }
            catch (JsonReaderException ex)
            {
                CreatorStatus.AddMessage(AlertType.Error, $"Your uploaded file has an unexpected format. {ex.Message}");
            }
            catch (Exception ex)
            {
                CreatorStatus.AddMessage(AlertType.Error, "Error during import: " + ex.Message);
            }
        }

        public void GetCopyInputValues(QStandard asset)
        {
            asset.ContentTitle = TitleDuplicate.Text;
            asset.ContentName = TitleDuplicate.Text;
            asset.StandardIdentifier = UniqueIdentifier.Create();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void Save()
        {
            var document = StandardFactory.Create(StandardType.Document);
            document.StandardIdentifier = UniqueIdentifier.Create();
            document.Language = "en";
            document.DocumentType = DocumentType.Value;

            var isCopDoc = document.DocumentType == Shift.Sdk.UI.DocumentType.CustomizedOccupationProfile;

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
            else
            {
                content = new ContentContainer();
            }

            content.Title.Text.Default = TitleInput.Text;

            if (isCopDoc)
            {
                var contentSettings = new ContentSettings();
                contentSettings.Locked.Add("Copyright");
                contentSettings.Locked.Add("StandardsDevelopmentProcess");
                contentSettings.Locked.Add("CertificationDevelopmentProcess");
                document.ContentSettings = contentSettings.Serialize();
            }

            StandardStore.Insert(document, content);

            if (hasBaseStandard && CopyCopemtencies)
            {
                var containments = StandardContainmentSearch.SelectCompetencyContainments(baseStandardIdentifier.Value);
                if (containments.Count > 0)
                {
                    foreach (var containment in containments)
                        containment.ParentStandardIdentifier = document.StandardIdentifier;

                    StandardContainmentStore.Insert(containments);
                }
            }

            var url = $"{EditUrl}?asset={document.StandardIdentifier}";

            HttpResponseHelper.Redirect(url);
        }
    }
}
