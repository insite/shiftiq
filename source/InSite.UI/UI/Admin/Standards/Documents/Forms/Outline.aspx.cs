using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Standards.Documents.Utilities;
using InSite.Application.Standards.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Standards;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json.Linq;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Standards.Documents.Forms
{
    public partial class Outline : AdminBasePage
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

        protected Guid? StandardIdentifier
        {
            get
            {
                if (!_standardId.HasValue && Guid.TryParse(Request["asset"], out var value))
                    _standardId = value;
                return _standardId;
            }
        }

        private List<Tuple<bool, Guid>> RelatedStandardIdentifiers
        {
            get => (List<Tuple<bool, Guid>>)ViewState[nameof(RelatedStandardIdentifiers)];
            set => ViewState[nameof(RelatedStandardIdentifiers)] = value;
        }

        protected bool AllowManageFields => _allowManageFields
            ?? (_allowManageFields = CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Standards_Documents, PermissionOperation.Configure)).Value;

        protected bool CanWrite { get; set; }

        #endregion

        #region Fields

        private Guid? _standardId;
        private bool? _allowManageFields;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RelatedAssignButton.Click += RelatedAssignButton_Click;

            RelatedStandardRepeater.DataBinding += RelatedStandardRepeater_DataBinding;
            RelatedStandardRepeater.ItemDataBound += RelatedStandardRepeater_ItemDataBound;
            RelatedStandardRepeater.ItemCommand += RelatedStandardRepeater_ItemCommand;

            IsTemplate.AutoPostBack = true;
            IsTemplate.CheckedChanged += IsTemplate_CheckedChanged;

            StandardPrivacyScope.AutoPostBack = true;
            StandardPrivacyScope.ValueChanged += StandardPrivacyScope_ValueChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            HandleAjaxRequest();

            base.OnLoad(e);

            if (!IsPostBack)
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
            var entity = StandardIdentifier.HasValue ? StandardSearch.SelectFirst(x => x.StandardIdentifier == StandardIdentifier.Value) : null;
            if (entity == null || entity.StandardType != Shift.Constant.StandardType.Document || entity.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect("/ui/admin/standards/documents/search", true);

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{entity.ContentTitle ?? entity.ContentName ?? "Untitled"} <span class='form-text'>{entity.StandardType} Asset #{entity.AssetNumber}</span>");

            DownloadLink.NavigateUrl = $"/ui/admin/standards/documents/download?asset={StandardIdentifier.Value}";
            DuplicateLink.NavigateUrl = $"/ui/admin/standards/documents/create?action=duplicate&asset={StandardIdentifier.Value}";
            DeleteLink.NavigateUrl = $"/admin/standards/documents/delete?asset={StandardIdentifier.Value}";

            GeneralSection.Title = entity.DocumentType;
            LockScript.Visible = AllowManageFields;

            SetInputValues(entity);
        }

        #endregion

        #region Methods (binding)

        private void SetInputValues(Standard document)
        {
            var content = ServiceLocator.ContentSearch.GetBlock(document.StandardIdentifier);
            var settings = ContentSettings.Deserialize(document.ContentSettings);

            DownloadButton.NavigateUrl = new ReturnUrl("asset")
                .GetRedirectUrl($"/ui/admin/standards/download?asset={StandardIdentifier}");

            {
                TitleOutput.Text = content.Title.Text.Default.IfNullOrEmpty("None");
                TitleLink.NavigateUrl = $"/ui/admin/standards/documents/content?asset={StandardIdentifier}&tab=Title";
                TitleLink.Visible = AllowManageFields || !settings.Locked.Contains("Title");

                IsTitleLocked.Checked = settings.Locked.Contains("Title");
                //IsTitleLocked.Visible = AllowManageFields;

                if (document.DocumentType != null)
                {
                    var sections = SectionInfo.GetDocumentSections(document.DocumentType).Select(x => new
                    {
                        x.ID,
                        Title = x.GetTitle(),
                        Content = content.GetHtml(x.ID).IfNullOrEmpty("None"),
                        IsLocked = settings.Locked.Contains(x.ID)
                    });

                    SectionRepeater.DataSource = sections;
                    SectionRepeater.DataBind();
                }
                CanWrite = Identity.IsGranted(PermissionIdentifiers.Admin_Standards);
            }

            IsTemplate.Checked = document.IsTemplate;
            StandardPrivacyScope.Value = document.StandardPrivacyScope.IfNullOrEmpty("Tenant");
            StandardPrivacyScope.FindOptionByValue("User").Enabled = document.CreatedBy == User.UserIdentifier;

            BindCompetencies(document);

            // Relationships

            SetupRelatedStandardSelector();
            BindRelatedStandards();
        }

        private void BindCompetencies(Standard document)
        {
            // Knowledge

            KnowledgeSection.Visible = false;

            // Items

            ItemsCompetenciesPanel.SetInputValues(document);
            ItemsCompetenciesPanel.SetAssetType(document.DocumentType);
        }

        private void SetupRelatedStandardSelector()
        {
            var downstream = StandardContainmentSearch.SelectTree(new[] { StandardIdentifier.Value });
            var upstream = StandardContainmentSearch.SelectUpstreamRelationships(new[] { StandardIdentifier.Value });

            RelatedStandardSelector.Filter.StandardTypes = new[] { Shift.Constant.StandardType.Document };
            RelatedStandardSelector.Filter.DocumentType = new[] { DocumentType.NationalOccupationStandard };
            RelatedStandardSelector.Filter.Exclusions.StandardIdentifier = downstream
                .Select(x => x.ChildStandardIdentifier)
                .Concat(upstream.Select(x => x.ParentStandardIdentifier))
                .Append(StandardIdentifier.Value)
                .Distinct()
                .ToList();
        }

        private void BindRelatedStandards()
        {
            var incomingData = StandardContainmentSearch
                .Bind(
                    x => new StandardInfo
                    {
                        Key = x.Parent.StandardIdentifier,
                        Identifier = x.Parent.StandardIdentifier,
                        Title = x.Parent.ContentTitle ?? CoreFunctions.GetContentTextEn(x.Parent.StandardIdentifier, ContentLabel.Title),
                        Sequence = 0,
                        Direction = ConnectionDirection.Incoming
                    },
                    x => x.ChildStandardIdentifier == StandardIdentifier.Value
                      && x.Parent.StandardType == Shift.Constant.StandardType.Document,
                    "Title");

            var outgoingData = StandardContainmentSearch
                .Bind(
                    x => new StandardInfo
                    {
                        Key = x.Child.StandardIdentifier,
                        Identifier = x.Child.StandardIdentifier,
                        Title = x.Child.ContentTitle ?? CoreFunctions.GetContentTextEn(x.Child.StandardIdentifier, ContentLabel.Title),
                        Sequence = x.ChildSequence,
                        Direction = ConnectionDirection.Outgoing
                    },
                    x => x.ParentStandardIdentifier == StandardIdentifier.Value
                      && x.Child.StandardType == StandardType.Document,
                    "Sequence,Title");

            var data = incomingData.Concat(outgoingData).ToArray();

            RelatedStandardField.Visible = data.Length > 0;

            RelatedStandardRepeater.DataSource = data;
            RelatedStandardRepeater.DataBind();
        }

        #endregion

        #region Event handlers

        private void RelatedAssignButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                return;

            var relatedKey = RelatedStandardSelector.Value.Value;

            RelatedStandardSelector.Value = null;

            SetupRelatedStandardSelector();

            if (relatedKey == StandardIdentifier.Value)
                return;

            var upstream = StandardContainmentSearch.SelectUpstreamRelationships(new[] { StandardIdentifier.Value });
            if (upstream.Any(x => x.ParentStandardIdentifier == relatedKey))
                return;

            var downstream = StandardContainmentSearch.SelectTree(new[] { StandardIdentifier.Value });
            if (downstream.Any(x => x.ChildStandardIdentifier == relatedKey))
                return;

            StandardContainmentStore.Insert(StandardIdentifier.Value, relatedKey);
            StandardContainmentStore.CopyChildCompetencies(relatedKey, StandardIdentifier.Value);

            var document = StandardSearch.Select(StandardIdentifier.Value);
            BindCompetencies(document);

            BindRelatedStandards();
        }

        private void RelatedStandardRepeater_DataBinding(object sender, EventArgs e)
        {
            RelatedStandardIdentifiers = new List<Tuple<bool, Guid>>();
        }

        private void RelatedStandardRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var info = (StandardInfo)e.Item.DataItem;

            RelatedStandardIdentifiers.Add(new Tuple<bool, Guid>(info.Direction == ConnectionDirection.Outgoing, info.Key));
        }

        private void RelatedStandardRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var relatedKey = RelatedStandardIdentifiers[e.Item.ItemIndex];

                if (relatedKey.Item1)
                    StandardContainmentStore.Delete(StandardIdentifier.Value, relatedKey.Item2);
                else
                    StandardContainmentStore.Delete(relatedKey.Item2, StandardIdentifier.Value);

                SetupRelatedStandardSelector();
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
                var entity = ServiceLocator.StandardSearch.GetStandard(StandardIdentifier.Value);
                if (entity != null)
                {
                    var settings = ContentSettings.Deserialize(entity.ContentSettings);

                    foreach (var prop in stateData)
                    {
                        if (prop.Value.ToObject<bool>())
                            settings.Locked.Add(prop.Key);
                        else
                            settings.Locked.Remove(prop.Key);
                    }

                    ServiceLocator.SendCommand(new ModifyStandardFieldText(entity.StandardIdentifier, StandardField.ContentSettings, settings.Serialize()));
                }
            }

            Response.Clear();
            Response.Write("OK");
            Response.End();
        }

        private void IsTemplate_CheckedChanged(object sender, EventArgs e)
        {
            StandardStore.Update(StandardIdentifier.Value, x => x.IsTemplate = IsTemplate.Checked);
        }

        private void StandardPrivacyScope_ValueChanged(object sender, EventArgs e)
        {
            StandardStore.Update(StandardIdentifier.Value, x => x.StandardPrivacyScope = StandardPrivacyScope.Value);
        }

        #endregion
    }
}