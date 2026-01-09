using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Admin.Standards.Standards.Utilities;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Web.Data;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class OutlineEdit : BaseUserControl
    {
        #region Classes

        [Serializable]
        private class ControlData
        {
            public Guid StandardIdentifier { get; set; }
            public Guid? ParentStandardIdentifier { get; set; }
            public Guid? PrevSiblingStandardIdentifier { get; set; }
            public Guid? NextSiblingStandardIdentifier { get; set; }
        }

        #endregion

        #region Events

        public event EventHandler Updated;
        private void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        private ControlData CurrentData
        {
            get => (ControlData)ViewState[nameof(CurrentData)];
            set => ViewState[nameof(CurrentData)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            IndentButton.Click += IndentButton_Click;
            OutdentButton.Click += OutdentButton_Click;
            ReorderStartButton.Click += ReorderStartButton_Click;
            ReorderCancelButton.Click += ReorderCancelButton_Click;
            ReorderSaveButton.Click += ReorderSaveButton_Click;
            SaveButton.Click += SaveButton_Click;
            CreateButton.Click += CreateButton_Click;
        }

        #endregion

        #region Event handlers

        private void IndentButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentData?.PrevSiblingStandardIdentifier != null)
                    OutlineHelper.Indent(CurrentData.StandardIdentifier, CurrentData.PrevSiblingStandardIdentifier.Value);

                OnUpdated();
            }
            catch (ApplicationError err)
            {
                if (StandardStore.IsDepthLimitException(err))
                {
                    EditorStatus.AddMessage(AlertType.Error, StandardStore.DepthLimitErrorText);
                    return;
                }

                throw;
            }
        }

        private void OutdentButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (CurrentData?.ParentStandardIdentifier != null)
                    OutlineHelper.Outdent(CurrentData.StandardIdentifier, CurrentData.ParentStandardIdentifier.Value);

                OnUpdated();
            }
            catch (ApplicationError err)
            {
                if (StandardStore.IsDepthLimitException(err))
                {
                    EditorStatus.AddMessage(AlertType.Error, StandardStore.DepthLimitErrorText);
                    return;
                }

                throw;
            }
        }

        private void ReorderStartButton_Click(object sender, EventArgs e)
        {
            var data = OutlineHelper.LoadReorderItems(CurrentData.StandardIdentifier);
            if (data.Length == 0)
                return;

            EditorContainer.Visible = false;
            ReorderContainer.Visible = true;
            ReorderState.Value = string.Empty;

            ReorderRepeater.DataSource = data.Select(x => new
            {
                x.Number,
                x.Type,
                Title = HttpUtility.HtmlEncode(x.Title.IfNullOrEmpty("(Untitled)")),
                Icon = x.Icon ?? StandardSearch.GetStandardTypeIcon(x.Type)
            });
            ReorderRepeater.DataBind();
        }

        private void ReorderCancelButton_Click(object sender, EventArgs e)
        {
            EditorContainer.Visible = true;
            ReorderContainer.Visible = false;
            ReorderState.Value = string.Empty;

            ReorderRepeater.DataSource = null;
            ReorderRepeater.DataBind();
        }

        private void ReorderSaveButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ReorderState.Value))
            {
                var reorderState = JsonConvert.DeserializeObject<int[]>(ReorderState.Value);

                OutlineHelper.SaveReorder(CurrentData.StandardIdentifier, reorderState);
            }

            EditorContainer.Visible = true;
            ReorderContainer.Visible = false;
            ReorderState.Value = string.Empty;

            ReorderRepeater.DataSource = null;
            ReorderRepeater.DataBind();

            OnUpdated();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (CurrentData != null)
            {
                StandardStore.Update(x => x.OrganizationIdentifier == Organization.Key && x.StandardIdentifier == CurrentData.StandardIdentifier, entity =>
                {
                    entity.StandardType = StandardType.Value;
                    entity.ContentTitle = Title.Text;
                    entity.ContentSummary = Summary.Text;
                    entity.Code = Code.Text;
                    entity.IsTheory = IsTheory.Checked;
                    entity.IsPractical = IsPractical.Checked;
                    entity.CreditIdentifier = CreditIdentifier.Text;
                    entity.LevelType = LevelType.Text;
                    entity.LevelCode = LevelCode.Text;
                    entity.MajorVersion = MajorVersion.Text;
                    entity.MinorVersion = MinorVersion.Text;
                });
            }

            OnUpdated();
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (CurrentData != null)
            {
                var type = CreatorTypeRepeater.Items
                    .Cast<RepeaterItem>()
                    .Select(x => (IRadioButton)x.FindControl("RadioButton"))
                    .Where(x => x.Checked)
                    .Select(x => x.Text)
                    .Single();
                var titles = CreatorTitles.Text
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrEmpty(x));

                try
                {
                    foreach (var title in titles)
                    {
                        var asset = StandardFactory.Create(type);
                        asset.StandardIdentifier = UniqueIdentifier.Create();
                        asset.ParentStandardIdentifier = CurrentData.StandardIdentifier;
                        asset.Language = "en";
                        asset.ContentTitle = title;

                        StandardStore.Insert(asset);
                    }
                }
                catch (ApplicationError err)
                {
                    if (StandardStore.IsDepthLimitException(err))
                    {
                        CreatorStatus.AddMessage(AlertType.Error, StandardStore.DepthLimitErrorText);
                        return;
                    }

                    throw;
                }
            }

            OnUpdated();
        }

        #endregion

        public bool LoadData(int asset, bool canDelete)
        {
            CurrentData = null;

            EditorContainer.Visible = false;
            ReorderContainer.Visible = false;

            var standard = StandardSearch.BindFirst(x => new
            {
                x.StandardIdentifier,
                x.ParentStandardIdentifier,
                x.StandardType,
                x.AssetNumber,
                Code = x.Code,
                IsPractical = x.IsPractical,
                IsTheory = x.IsTheory,
                CreditIdentifier = x.CreditIdentifier,
                LevelType = x.LevelType,
                LevelCode = x.LevelCode,
                MajorVersion = x.MajorVersion,
                MinorVersion = x.MinorVersion,

                AllowReorder =
                    x.Children.Count() + x.ParentContainments.Count() > 1,
                Siblings = x.Parent.Children.OrderBy(y => y.Sequence).Select(y => y.StandardIdentifier)
            }, x => x.OrganizationIdentifier == Organization.Key && x.AssetNumber == asset);

            if (standard == null)
                return false;

            EditorContainer.Visible = true;

            CurrentData = new ControlData
            {
                StandardIdentifier = standard.StandardIdentifier,
                ParentStandardIdentifier = standard.ParentStandardIdentifier
            };

            if (standard.Siblings.Any())
            {
                var siblings = standard.Siblings.ToArray();
                var currentIndex = Array.FindIndex(siblings, x => x == CurrentData.StandardIdentifier);

                CurrentData.PrevSiblingStandardIdentifier = currentIndex > 0 ? siblings[currentIndex - 1] : (Guid?)null;
                CurrentData.NextSiblingStandardIdentifier = currentIndex < siblings.Length - 1 ? siblings[currentIndex + 1] : (Guid?)null;
            }

            var content = ServiceLocator.ContentSearch.GetBlock(
                CurrentData.StandardIdentifier,
                ContentContainer.DefaultLanguage,
                new[] { ContentLabel.Title, ContentLabel.Summary });

            Header.InnerHtml = $"{HttpUtility.HtmlEncode(content.Title.GetText())} " +
                $"<small class='text-body-secondary'>{standard.StandardType} Asset #{standard.AssetNumber}</small>";

            StandardType.Value = standard.StandardType;
            Title.Text = content.Title.Text.Default;
            Summary.Text = content.Summary.Text.Default;
            Code.Text = standard.Code;
            IsTheory.Checked = standard.IsTheory;
            IsPractical.Checked = standard.IsPractical;
            CreditIdentifier.Text = standard.CreditIdentifier;
            LevelType.Text = standard.LevelType;
            LevelCode.Text = standard.LevelCode;
            MajorVersion.Text = standard.MajorVersion;
            MinorVersion.Text = standard.MinorVersion;

            IndentButton.Enabled = CurrentData.PrevSiblingStandardIdentifier.HasValue;
            OutdentButton.Enabled = CurrentData.ParentStandardIdentifier.HasValue;
            ReorderStartButton.Enabled = standard.AllowReorder;

            CreatorTypeRepeater.DataSource = StandardSearch.GetAllTypeNames(Identity.Organization.Identifier);
            CreatorTypeRepeater.DataBind();

            var returnUrl = new ReturnUrl("asset");
            DeleteButton.NavigateUrl = returnUrl.GetRedirectUrl($"/admin/standards/delete?asset={CurrentData.StandardIdentifier}");
            DeleteButton.Visible = canDelete;

            return true;
        }
    }
}