using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Application.Standards.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Standards;
using InSite.Persistence;
using InSite.UI.Layout.Portal;

using Newtonsoft.Json.Linq;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Portal.Standards.Documents
{
    public partial class Outline : PortalBasePage, IHasParentLinkParameters
    {
        #region Classes

        private class StandardInfo
        {
            public Guid Key { get; set; }
            public Guid Identifier { get; set; }
            public string Title { get; set; }
            public int Sequence { get; set; }
            public ConnectionDirection Direction { get; set; }
        }

        #endregion

        #region Properties

        protected bool DocumentIsTemplate
        {
            get => (bool)ViewState[nameof(DocumentIsTemplate)];
            set => ViewState[nameof(DocumentIsTemplate)] = value;
        }

        private string PageAction => Request.QueryString["action"];

        private Guid? _standardId;
        protected Guid StandardIdentifier => _standardId
            ?? (_standardId = Guid.TryParse(Request["standard"], out var value) ? value : Guid.Empty).Value;

        private List<Tuple<bool, Guid>> RelatedStandardIdentifiers
        {
            get => (List<Tuple<bool, Guid>>)ViewState[nameof(RelatedStandardIdentifiers)];
            set => ViewState[nameof(RelatedStandardIdentifiers)] = value;
        }

        private Standard _document;
        private Standard Document => _document
            ?? (_document = StandardSearch.Select(StandardIdentifier));


        private ContentSettings _contentSettings;
        private ContentSettings ContentSettings => _contentSettings
            ?? (_contentSettings = ContentSettings.Deserialize(Document.ContentSettings));

        private bool? _allowManageFields;
        protected bool AllowManageFields
        {
            get
            {
                if (_allowManageFields == null)
                {
                    _allowManageFields = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Standards_Documents, PermissionOperation.Configure)
                        || Document.CreatedBy == User.UserIdentifier;
                }

                return _allowManageFields.Value;
            }
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RelatedNosAssignButton.Click += RelatedNosAssignButton_Click;
            RelatedOccProfAssignButton.Click += RelatedOccProfAssignButton_Click;

            RelatedStandardRepeater.DataBinding += RelatedStandardRepeater_DataBinding;
            RelatedStandardRepeater.ItemDataBound += RelatedStandardRepeater_ItemDataBound;
            RelatedStandardRepeater.ItemCommand += RelatedStandardRepeater_ItemCommand;
        }

        protected override void OnLoad(EventArgs e)
        {
            HandleAjaxRequest();

            base.OnLoad(e);

            if (IsPostBack)
                return;

            Open();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (AllowManageFields)
            {
                var inputs = SectionRepeater.Items.Cast<RepeaterItem>().Select(x => x.FindControl("IsLocked").ClientID).Append(IsTitleLocked.ClientID);
                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(Outline),
                    "register_inputs",
                    $"documentOutline.registerLocks({JsonHelper.SerializeJsObject(inputs)});",
                    true
                );
            }

            base.OnPreRender(e);
        }

        private void Open()
        {
            var entity = StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardIdentifier);
            if (entity == null
                || entity.StandardType != StandardType.Document
                || entity.OrganizationIdentifier != Organization.Identifier
                || entity.StandardPrivacyScope.IfNullOrEmpty("Tenant") == "User"
                    && entity.CreatedBy != User.UserIdentifier
                    && !CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Standards_Documents, PermissionOperation.Configure)
                )
            {
                HttpResponseHelper.Redirect("/ui/portal/standards/documents/search", true);
            }

            _document = entity;

            DocumentIsTemplate = entity.IsTemplate;
            GeneralSection.Title = entity.DocumentType;
            LockScript.Visible = AllowManageFields;

            SetInputValues();

            BuildBreadcrumbs();
        }

        private void BuildBreadcrumbs()
        {
            var title = ServiceLocator.ContentSearch.GetText(StandardIdentifier, ContentLabel.Title, CurrentLanguage) ?? Translate("Untitled");
            var subtitle = $"{Translate(Document.StandardType)} {Translate("Asset")} #{Document.AssetNumber}";

            var parentLinkTitle = Search.GetPageTitle(Document.DocumentType, Translator);
            AutoBindFolderHeader(null, null, title, new[] { parentLinkTitle });

            var master = (PortalMaster)Master;
            master.Breadcrumbs.BindTitleAndSubtitle(title, subtitle);
        }

        #endregion

        #region Methods (binding)

        private void SetInputValues()
        {
            DownloadButton.Visible = Document.DocumentType != DocumentType.NationalOccupationStandard;
            DownloadButton.NavigateUrl = new ReturnUrl("standard")
                .GetRedirectUrl($"/ui/portal/standards/documents/download?standard={StandardIdentifier}");

            TitleLink.NavigateUrl = PageAction.IsNotEmpty()
                ? $"/ui/portal/standards/documents/content?standard={StandardIdentifier}&action={PageAction}&tab=Title"
                : $"/ui/portal/standards/documents/content?standard={StandardIdentifier}&tab=Title";
            TitleLink.Visible = !Document.IsTemplate && AllowManageFields && !ContentSettings.Locked.Contains("Title");

            IsTitleLocked.Checked = ContentSettings.Locked.Contains("Title");

            BindContent();
            BindCompetencies();

            // Relationships

            StandardAdditionPanel.Visible = !Document.IsTemplate && AllowManageFields;
            RelatedOccProfPanel.Visible = Document.DocumentType == DocumentType.OccupationProfile
                || Document.DocumentType == DocumentType.JobDescription && Organization.OrganizationIdentifier == Shift.Constant.OrganizationIdentifiers.EHRC;

            SetupRelatedStandardSelectors();
            BindRelatedStandards();

            if (PageAction.IsNotEmpty() && PageAction.Equals("Job Comparison Tool"))
            {
                BackToAnalysis.Visible = true;
                BackToAnalysis.NavigateUrl = string.Format("/ui/portal/standards/documents/analysis?action={0}", PageAction);
                ScreenStatus.AddMessage(AlertType.Information, "New Competency Profile created. To return to 'Document Analysis' click back to 'Job Comparison Tool' button.");
            }

            ButtonPanel.Visible = DownloadButton.Visible || BackToAnalysis.Visible;
        }

        private void BindContent()
        {
            var content = ServiceLocator.ContentSearch.GetBlock(StandardIdentifier);

            TitleOutput.Text = content.Title.GetText(CurrentLanguage).IfNullOrEmpty(() => Translate("None"));

            var sections = SectionInfo.GetDocumentSections(Document.DocumentType).Select(x => new
            {
                x.ID,
                Title = x.GetTitle(),
                Tooltip = $"Change {x.GetTitle("en")}",
                Content = content.GetHtml(x.ID, CurrentLanguage).IfNullOrEmpty(() => Translate("None")),
                IsLocked = Document.IsTemplate || ContentSettings.Locked.Contains(x.ID)
            })
                .ToList();

            SectionRepeater.DataSource = sections;
            SectionRepeater.DataBind();
        }

        private void BindCompetencies()
        {
            // Knowledge

            KnowledgeSection.Visible = false;

            // Items

            ItemsCompetenciesPanel.SetInputValues(Document);
            ItemsCompetenciesPanel.SetAssetType(Document.DocumentType);
        }

        private void SetupRelatedStandardSelectors()
        {
            LoadRelationships(out var downstream, out var upstream);
            SetupRelatedNosStandardSelector(downstream, upstream);
            SetupRelatedOccProfStandardSelector(downstream, upstream);
        }

        private void SetupRelatedNosStandardSelector(StandardContainmentSearch.ContainmentTreeInfo[] downstream = null, StandardContainmentSearch.UpstreamRelationshipInfo[] upstream = null) =>
            SetupRelatedStandardSelector(RelatedNosStandardSelector, DocumentType.NationalOccupationStandard, downstream, upstream);

        private void SetupRelatedOccProfStandardSelector(StandardContainmentSearch.ContainmentTreeInfo[] downstream = null, StandardContainmentSearch.UpstreamRelationshipInfo[] upstream = null) =>
            SetupRelatedStandardSelector(RelatedOccProfStandardSelector, DocumentType.OccupationProfile, downstream, upstream);

        private void SetupRelatedStandardSelector(FindStandard comboBox, string docType, StandardContainmentSearch.ContainmentTreeInfo[] downstream = null, StandardContainmentSearch.UpstreamRelationshipInfo[] upstream = null)
        {
            if (downstream == null || upstream == null)
                LoadRelationships(out downstream, out upstream);

            comboBox.Filter.StandardTypes = new[] { StandardType.Document };
            comboBox.Filter.DocumentType = new[] { docType };
            comboBox.Filter.CreatedBy = User.UserIdentifier;
            comboBox.Filter.IsPortal = true;
            comboBox.Filter.Exclusions.StandardIdentifier = downstream
                .Select(x => x.ChildStandardIdentifier)
                .Concat(upstream.Select(x => x.ParentStandardIdentifier))
                .Append(StandardIdentifier)
                .Distinct()
                .ToList();
            comboBox.Value = null;
        }

        private void LoadRelationships(out StandardContainmentSearch.ContainmentTreeInfo[] downstream, out StandardContainmentSearch.UpstreamRelationshipInfo[] upstream)
        {
            downstream = StandardContainmentSearch.SelectTree(new[] { StandardIdentifier });
            upstream = StandardContainmentSearch.SelectUpstreamRelationships(new[] { StandardIdentifier });
        }

        private void BindRelatedStandards()
        {
            var incomingData = StandardContainmentSearch
                .Bind(
                    x => new StandardInfo
                    {
                        Key = x.Parent.StandardIdentifier,
                        Identifier = x.Parent.StandardIdentifier,
                        Title = CoreFunctions.GetContentText(x.Parent.StandardIdentifier, ContentLabel.Title, CurrentLanguage)
                                ?? CoreFunctions.GetContentTextEn(x.Parent.StandardIdentifier, ContentLabel.Title),
                        Sequence = 0,
                        Direction = ConnectionDirection.Incoming
                    },
                    x => x.ChildStandardIdentifier == StandardIdentifier
                      && x.Parent.StandardType == StandardType.Document,
                    "Title");

            var outgoingData = StandardContainmentSearch
                .Bind(
                    x => new StandardInfo
                    {
                        Key = x.Child.StandardIdentifier,
                        Identifier = x.Child.StandardIdentifier,
                        Title = CoreFunctions.GetContentText(x.Child.StandardIdentifier, ContentLabel.Title, CurrentLanguage)
                                ?? CoreFunctions.GetContentTextEn(x.Child.StandardIdentifier, ContentLabel.Title),
                        Sequence = x.ChildSequence,
                        Direction = ConnectionDirection.Outgoing
                    },
                    x => x.ParentStandardIdentifier == StandardIdentifier
                      && x.Child.StandardType == StandardType.Document,
                    "Sequence,Title");

            var data = incomingData.Concat(outgoingData).ToArray();

            RelatedStandardField.Visible = data.Length > 0;

            RelatedStandardRepeater.DataSource = data;
            RelatedStandardRepeater.DataBind();
        }

        protected string GetUrl(object item)
        {
            var url = (string)item;
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            if (!string.IsNullOrEmpty(PageAction))
                return url + $"&action={PageAction}";
            return url;
        }

        #endregion

        #region Event handlers

        private void RelatedNosAssignButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            OnRelatedAssign(RelatedNosStandardSelector.Value.Value);

            RelatedNosStandardSelector.Value = null;

            SetupRelatedNosStandardSelector();
        }

        private void RelatedOccProfAssignButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            OnRelatedAssign(RelatedOccProfStandardSelector.Value.Value);

            RelatedOccProfStandardSelector.Value = null;

            SetupRelatedOccProfStandardSelector();
        }

        private void OnRelatedAssign(Guid relatedKey)
        {
            if (relatedKey == StandardIdentifier)
                return;

            LoadRelationships(out var downstream, out var upstream);
            if (upstream.Any(x => x.ParentStandardIdentifier == relatedKey) || downstream.Any(x => x.ChildStandardIdentifier == relatedKey))
                return;

            SetUpJobDescription(relatedKey, StandardIdentifier);

            StandardContainmentStore.Insert(StandardIdentifier, relatedKey);
            StandardContainmentStore.CopyChildCompetencies(relatedKey, StandardIdentifier);

            BindContent();
            BindCompetencies();
            BindRelatedStandards();
        }

        private void SetUpJobDescription(Guid nocStandardIdentifier, Guid standardIdentifier)
        {
            var jobKey = Document.DocumentType.Equals("Job Description")
                ? "Purpose of Job"
                : "Job Definition";

            var baseStandardId = StandardSearch.BindFirst(x => x.StandardIdentifier, x => x.StandardIdentifier == nocStandardIdentifier);
            var baseContent = ServiceLocator.ContentSearch.GetBlock(baseStandardId);

            var content = new ContentContainer();
            content[jobKey] = baseContent["Job Definition"];

            ServiceLocator.SendCommand(new ModifyStandardContent(standardIdentifier, content));
        }

        private void RelatedStandardRepeater_DataBinding(object sender, EventArgs e)
        {
            RelatedStandardIdentifiers = new List<Tuple<bool, Guid>>();
        }

        private void RelatedStandardRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var info = (StandardInfo)e.Item.DataItem;

            RelatedStandardIdentifiers.Add(new Tuple<bool, Guid>(info.Direction == ConnectionDirection.Outgoing, info.Key));

            var button = (IButton)e.Item.FindControl("DeleteRelationshipButton");
            button.Visible = !DocumentIsTemplate && AllowManageFields;
        }

        private void RelatedStandardRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var relatedKey = RelatedStandardIdentifiers[e.Item.ItemIndex];

                if (relatedKey.Item1)
                    StandardContainmentStore.Delete(StandardIdentifier, relatedKey.Item2);
                else
                    StandardContainmentStore.Delete(relatedKey.Item2, StandardIdentifier);

                SetupRelatedStandardSelectors();
                BindRelatedStandards();
            }
        }

        private void HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest || !bool.TryParse(Page.Request.Form["isPageAjax"], out var isAjax) || !isAjax)
                return;

            var stateValue = Page.Request.Form["state"];
            var stateData = string.IsNullOrEmpty(stateValue)
                ? null
                : JObject.Parse(stateValue);

            if (stateData != null && stateData.Count > 0)
            {
                if (Document != null)
                {
                    var settings = ContentSettings.Deserialize(Document.ContentSettings);

                    foreach (var prop in stateData)
                    {
                        if (prop.Value.ToObject<bool>())
                            settings.Locked.Add(prop.Key);
                        else
                            settings.Locked.Remove(prop.Key);
                    }

                    ServiceLocator.SendCommand(new ModifyStandardFieldText(Document.StandardIdentifier, StandardField.ContentSettings, settings.Serialize()));
                }
            }

            Response.Clear();
            Response.Write("OK");
            Response.End();
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/search")
                ? $"type={HttpUtility.UrlEncode(StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardIdentifier)?.DocumentType)}"
                : null;
        }

        #endregion
    }
}