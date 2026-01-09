using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;

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

namespace InSite.Admin.Standards.Collections.Forms
{
    public partial class Create : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        protected string OutlinePath => "/ui/admin/standards/collections/outline";
        protected string SearchPath => "/ui/admin/standards/collections/search";
        private string Action => Request.QueryString["action"];
        private Guid? StandardIdentifier => Guid.TryParse(Request["asset"], out Guid result) ? result : (Guid?)null;
        public string DefaultLanguage => "en";

        #endregion

        #region Initialization, Loading and PreRender

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            StandardComboBox.AutoPostBack = true;
            StandardComboBox.ValueChanged += StandardComboBox_ValueChanged;

            SaveButton.Click += SaveButton_Click;
            UploadFileUploaded.Click += UploadFileUploaded_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            StandardComboBox.Filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;
            StandardComboBox.Filter.StandardTypes = new[] { StandardType.Collection };

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Upload);

            if (Action == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;

                if (StandardIdentifier.HasValue)
                {
                    var standardCollection = ServiceLocator.OldStandardSearch.GetStandard(StandardIdentifier.Value);

                    if (standardCollection != null)
                    {
                        StandardComboBox.Value = standardCollection.StandardIdentifier;

                        CopyTitle.Text = standardCollection.StandardTitle;
                        CopyLabel.Text = standardCollection.StandardLabel;
                    }
                }

            }

            OnCreationTypeSelected();

            CancelButton.NavigateUrl = GetBackUrl();
        }

        #endregion

        #region Event handling

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                SaveOne();
            if (value == CreationTypeEnum.Duplicate)
                SaveDuplicate();
            if (value == CreationTypeEnum.Upload)
                SaveOutline();
        }

        private void UploadFileUploaded_Click(object sender, EventArgs e)
        {
            if (!CreateUploadFile.HasFile)
            {
                CreatorStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                return;
            }

            else if (!CreateUploadFile.FilePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                CreatorStatus.AddMessage(AlertType.Error, "Invalid file type. File type supported .json");
                return;
            }

            using (var stream = CreateUploadFile.OpenFile())
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                    UploadJsonInput.Text = reader.ReadToEnd();
            }
        }

        private void StandardComboBox_ValueChanged(object sender, EventArgs e)
        {
            OnCopyMessageSelectorSelectedIndexChanged();
        }

        private void OnCopyMessageSelectorSelectedIndexChanged()
        {
            if (StandardComboBox.HasValue)
            {
                var standardCollection = ServiceLocator.OldStandardSearch.GetStandard(StandardComboBox.Value.Value);
                if (standardCollection != null)
                {
                    CopyTitle.Text = standardCollection.StandardTitle;
                    CopyLabel.Text = standardCollection.StandardLabel;
                }
            }
            else
            {
                CopyTitle.Text = null;
                CopyLabel.Text = null;
            }
        }
        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                MultiView.SetActiveView(OneView);
            else if (value == CreationTypeEnum.Duplicate)
                MultiView.SetActiveView(CopyView);
            else if (value == CreationTypeEnum.Upload)
                MultiView.SetActiveView(UploadView);
        }
        #endregion

        #region Database operations

        protected void SaveOne()
        {
            var asset = StandardFactory.Create("Collection");
            asset.StandardIdentifier = UniqueIdentifier.Create();
            asset.Language = DefaultLanguage;
            asset.ContentTitle = NewTitle.Text;
            asset.StandardLabel = NewLabel.Text;

            StandardStore.Insert(asset);

            HttpResponseHelper.Redirect($"{OutlinePath}?asset={asset.StandardIdentifier}&status=saved", true);
        }

        private void SaveDuplicate()
        {
            if (!Page.IsValid)
                return;

            var asset = StandardFactory.Create("Collection");
            asset.StandardIdentifier = UniqueIdentifier.Create();
            asset.Language = DefaultLanguage;
            asset.ContentTitle = CopyTitle.Text;
            asset.StandardLabel = CopyLabel.Text;

            StandardStore.Insert(asset);

            var data = StandardContainmentSearch.Bind(
                LinqExtensions1.Expr((StandardContainment x) =>
                    InSite.Admin.Standards.Occupations.Utilities.Competencies.StandardInfo.Binder.Invoke(x.Child)).Expand(),
                    x => x.ParentStandardIdentifier == StandardIdentifier.Value
                      && x.Child.StandardType == StandardType.Competency);

            var relationships = new List<StandardContainment>();

            foreach (var info in data)
            {
                relationships.Add(new StandardContainment
                {
                    ParentStandardIdentifier = asset.StandardIdentifier,
                    ChildStandardIdentifier = info.StandardIdentifier
                });
            }

            if (relationships.Count > 0)
                StandardContainmentStore.Insert(relationships);

            HttpResponseHelper.Redirect($"{OutlinePath}?asset={asset.StandardIdentifier}&status=saved", true);
        }

        private void SaveOutline()
        {
            if (!Page.IsValid)
                return;

            try
            {
                var json = UploadJsonInput.Text;
                var standard = StandardHelper.DeserializeStandard(json);

                if (UploadJsonInput.Text.IsEmpty() || standard == null)
                {
                    CreatorStatus.AddMessage(AlertType.Error, $"Wrong JSON file uploaded");
                    return;
                }

                var standardIdentifier = UniqueIdentifier.Create();

                StandardStore.Insert(
                    new QStandard
                    {
                        StandardIdentifier = standardIdentifier,
                        Sequence = standard.Sequence,
                        Language = standard.Language,
                        ContentDescription = standard.ContentDescription,
                        AssetNumber = standard.AssetNumber,
                        AuthorName = standard.AuthorName,
                        Code = standard.Code,
                        CompetencyScoreCalculationMethod = standard.CompetencyScoreCalculationMethod,
                        CompetencyScoreSummarizationMethod = standard.CompetencyScoreSummarizationMethod,
                        ContentName = standard.ContentName,
                        ContentSettings = standard.ContentSettings,
                        ContentSummary = standard.ContentSummary,
                        ContentTitle = standard.ContentTitle,
                        CreditIdentifier = standard.CreditIdentifier,
                        DocumentType = standard.DocumentType,
                        Icon = standard.Icon,
                        LevelCode = standard.LevelCode,
                        LevelType = standard.LevelType,
                        SourceDescriptor = standard.SourceDescriptor,
                        StandardLabel = standard.StandardLabel,
                        StandardPrivacyScope = standard.StandardPrivacyScope,
                        StandardTier = standard.StandardTier,
                        StandardType = standard.StandardType,
                        Tags = standard.Tags,
                        OrganizationIdentifier = Organization.OrganizationIdentifier
                    }
                );

                if (standard.Items.IsNotEmpty())
                {
                    List<StandardContainment> standardContainmentsToAdd = new List<StandardContainment>();
                    foreach (var item in standard.Items)
                    {
                        var child = StandardSearch.SelectFirst(
                            x => x.ContentTitle == item.Title &&
                            x.Sequence == item.Sequence &&
                            x.AssetNumber == item.AssetNumber &&
                            x.Code == item.Code).StandardIdentifier;
                        standardContainmentsToAdd.Add(new StandardContainment() { ChildStandardIdentifier = child, ParentStandardIdentifier = standardIdentifier });
                    }
                    StandardContainmentStore.Insert(standardContainmentsToAdd);
                }

                HttpResponseHelper.Redirect($"{OutlinePath}?asset={standardIdentifier}&status=saved", true);

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

        #endregion

        #region IHasParentLinkParameters

        private string GetBackUrl() =>
            new ReturnUrl().GetReturnUrl() ?? GetReaderUrl();

        private string GetReaderUrl()
        {
            return SearchPath;
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"asset={StandardIdentifier}&status=saved"
                : null;
        }

        #endregion
    }
}