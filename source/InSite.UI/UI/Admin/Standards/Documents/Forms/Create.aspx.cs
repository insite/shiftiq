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

            var isEhrcOrganization = Organization.OrganizationIdentifier == Shift.Constant.OrganizationIdentifiers.EHRC;
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

                    if (isEhrcOrganization)
                    {
                        if (content[key1].IsEmpty)
                            content[key1].Html.Default = "<p>All rights reserved, Tourism HR Canada, 2021.</p>" +
                                "<p>None of the contents of this document may be reproduced except when explicitly allowed in writing by the rights holder. Fair use of contents is allowed for non-commercial applications.</p>";

                        if (content[key2].IsEmpty)
                            content[key2].Html.Default = "<h2>Developed for Industry by Industry</h2>" +
                                "<p>Industry expertise and experience are the key ingredients to emerit&reg; National Occupational Standards. Extensive participation by a representative group of tourism sector professionals plays a critical role in the development and validation process. The CTHRC collaborates with those who work in and are affected by an occupation to produce realistic and comprehensive occupational standards. Job incumbents, supervisors, educators and other industry representatives from participating provinces and territories participate in a variety of activities to develop and ratify the final standards. These activities include surveys, interviews, focus group sessions and standards review and validation activities. Job incumbents are extremely valuable sources of information for defining the scope, tasks, knowledge, skills and attitudes required. By obtaining information directly from the most knowledgeable individuals, the CTHRC ensures that the standards contain accurate, relevant and practical information.</p>" +
                                "<h2>Steps in the Development Process</h2>" +
                                "<p>There are three steps to the standards development process:</p>" +
                                "<ol>" +
                                "<li>Exploring the Occupational Profile: The scope of the occupation and the types of activities and tasks performed in an occupation are determined through a variety of research methods. A qualified person, often referred to as a \"job analyst,\" develops draft standards by analyzing existing written information on the occupation and by conducting surveys, interviews and focus groups. A common activity is to bring a representative group of stakeholders together for a face-to-face meeting or series of virtual meetings to define the occupation.</li>" +
                                "<li>Reviewing/Validating the Draft Standards: A larger group of subject matter experts is then asked to provide feedback on the draft standards. This allows the job analyst to obtain impartial feedback from beyond the original stakeholder group, thus helping to ensure that sufficient scope of the occupational domain is captured and that content is relevant.</li>" +
                                "<li>Finding Consensus and Ratifying the Standards: When a stakeholder ratifies the document, he or she accepts the standards as valid. If, during the ratification stage, a stakeholder identifies an issue and suggests a specific change, then a formal process is undertaken to resolve the issue and finalize the content of the standards. Once pan-Canadian consensus on the scope, tasks and competencies is achieved, standards are published and made readily available to the industry.</li>" +
                                "</ol>" +
                                "<p>emerit&reg; National Occupational Standards are recognized in the Canadian tourism sector and internationally because they are valid, relevant, practical and-most of all-developed by industry for industry.</p>" +
                                "<p>Do you have any feedback regarding these National Occupational Standards? If so, please email <a href='mailto:standards@cthrc.ca'>standards@cthrc.ca</a> and we will review your input for future updates.</p>";

                        if (content[key3].IsEmpty)
                            content[key3].Html.Default = "<h2>Recognizing Competence</h2>" +
                                "<p>Industry-defined credentials play an important role in increasing and recognizing professionalism and competence in tourism. emerit&reg; Professional Certification is competency-based, which means:</p>" +
                                "<ul>" +
                                "<li>it is based on industry-defined National Occupational Standards</li>" +
                                "<li>it defines a level of performance that industry expects</li>" +
                                "</ul>" +
                                "<p>Professional certification assesses both the knowledge and performance required to be considered competent in an occupation. emerit&reg; Professional Certification benefits workers, operators and the tourism sector as a whole. Workers receive recognition for good performance, increase their opportunities for advancement and may be able to receive credit for entry into formal training programs. Operators benefit from having qualified staff whose work results in increased productivity and guest satisfaction. In addition, the industry's and the general public's image of the tourism sector is enhanced.</p>" +
                                "<h2>Steps in the Development Process</h2>" +
                                "<p>The certification development process has three steps:</p>" +
                                "<ol>" +
                                "<li>Developing the Assessment Tools: A qualified team of people, often referred to as \"assessment specialists,\" creates assessment instruments that measure the knowledge and skills at the level defined in the National Occupational Standards. Usually there are two types of assessments: one that measures application of knowledge and one that measures performance of skills and demonstration of attitudes.</li>" +
                                "<li>Pilot Testing of Instruments: Assessment specialists collaborate with industry professionals to develop valid and reliable assessments. To ensure the assessments meet international standards of testing science, the assessments are administered to a representative group of industry professionals. The pilot participants complete each assessment as if they were certification candidates and provide feedback. The assessment specialists then analyze the results and consider the feedback to refine the assessments.</li>" +
                                "<li>Reviewing and Finalizing the Assessment Tools: A final round of revisions is performed before assessments are final. A group of industry professionals come together, virtually or face-to-face, to discuss final revisions to the assessments. An assessment specialist guides the group in the review process. This ensures the assessments are at the right level of difficulty and measure what they are supposed to measure. The group reviews every exam question and sets the percentage required to pass the performance assessment. At this time, the industry professionals also define the amount and type of work experience required for a candidate to become certified. Once group consensus has been reached on all details, the assessment instruments are published and candidates can register to complete the assessments and become certified.</li>" +
                                "</ol>" +
                                "<p>As workers complete assessments to become certified, a third-party testing provider collects statistics to monitor the quality of the assessments. Revisions are made if necessary.</p>" +
                                "<p>emerit&reg; Professional Certification is available for front-line, supervisory and management occupations. emerit&reg; certification recognizes workers as leading professionals in the industry who earn the right to display their professional designation on business cards and resumes.</p>";
                    }
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

            if (isEhrcOrganization && isCopDoc)
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
