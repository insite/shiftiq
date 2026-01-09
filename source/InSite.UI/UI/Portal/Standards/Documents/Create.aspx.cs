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

                if (isOccupationProfile || isJobDescription && Organization.OrganizationIdentifier == OrganizationIdentifiers.EHRC)
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

            var isEhrcOrganization = Organization.OrganizationIdentifier == OrganizationIdentifiers.EHRC;
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

            if (isEhrcOrganization && isCopDoc)
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