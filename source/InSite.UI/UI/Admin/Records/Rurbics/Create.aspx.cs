using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

using Shift.Common.Timeline.Commands;

using InSite.Application.Records.Read;
using InSite.Application.Rubrics.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Admin.Records.Rurbics.Controls;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Rurbics
{
    public partial class Create : AdminBasePage, IOverrideWebRouteParent, IHasParentLinkParameters
    {
        public const string NavigateUrl = "/ui/admin/records/rubrics/create";

        public static string GetNavigateUrl(Guid? bankId = null, Guid? rubricId = null, string action = null, int? type = null)
        {
            var url = new WebUrl(NavigateUrl);

            if (bankId.HasValue)
                url.QueryString.Add("bank", bankId.Value.ToString());

            if (rubricId.HasValue)
                url.QueryString.Add("rubric", rubricId.Value.ToString());

            if (action.IsNotEmpty())
                url.QueryString.Add("action", action);

            if (type.HasValue)
                url.QueryString.Add("type", type.Value.ToString());

            return url.ToString();
        }

        public static void Redirect(Guid? bankId = null, Guid? rubricId = null, string action = null, int? type = null) =>
            HttpResponseHelper.Redirect(GetNavigateUrl(bankId, rubricId, action, type));

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            NextButton.Click += NextButton_Click;
            SaveButton.Click += SaveButton_Click;

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += (s, a) => OnCreationTypeSelected();

            CopyRubricComboBox.AutoPostBack = true;
            CopyRubricComboBox.ValueChanged += (s, a) => OnCopyRubricComboBoxSelectedIndexChanged();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            Open();
        }

        private void Open()
        {
            Detail.ShowPoints = false;
            CriteriaList.LoadData();

            var cancelUrl = AddUrlParameters(GetParentUrl(null), null);

            CancelButton1.NavigateUrl = cancelUrl;
            CancelButton2.NavigateUrl = cancelUrl;

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate);

            if (Request.QueryString["action"] == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;

                if (Guid.TryParse(Request.QueryString["rubric"], out var rubricId))
                {
                    CopyRubricComboBox.ValueAsGuid = rubricId;
                    OnCopyRubricComboBoxSelectedIndexChanged(rubricId);
                }
            }

            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
            {
                MultiViewRubricDetail.SetActiveView(OneViewRubricDetail);
                MultiViewCriteriaList.SetActiveView(OneViewCriteriaList);
            }
            if (value == CreationTypeEnum.Duplicate)
            {
                MultiViewRubricDetail.SetActiveView(CopyViewRubricDetail);
                MultiViewCriteriaList.SetActiveView(CopyViewCriteriaList);
            }
        }


        private void OnCopyRubricComboBoxSelectedIndexChanged(Guid? rubricId = null)
        {
            if (CopyRubricComboBox.ValueAsGuid.HasValue)
                SetCopyRubric(CopyRubricComboBox.ValueAsGuid.Value);
            else if (rubricId.HasValue)
                SetCopyRubric(rubricId.Value);
            else
            {
                CopyDetail.ResetValues();
                CopyCriteriaList.ResetData();
            }
        }

        private void SetCopyRubric(Guid rubricId)
        {
            var rubric = ServiceLocator.RubricSearch.GetRubric(rubricId);
            if (rubric == null || rubric.OrganizationIdentifier != Organization.Identifier)
                Redirect();

            rubric.RubricTitle += " - copy";

            CopyDetail.SetInputValues(rubric);
            CopyCriteriaList.LoadData(rubric);
            CriteriaTab.Visible = true;
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            CriteriaTab.Visible = Page.IsValid;

            if (!Page.IsValid)
                return;

            Detail.ShowPoints = true;

            CriteriaTab.IsSelected = true;
            CriteriaTab.Visible = true;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                SaveOne();
            if (value == CreationTypeEnum.Duplicate)
                SaveCopy();
        }

        private void SaveOne()
        {
            var criteria = CriteriaList.GetCriteria();
            if (criteria.Count == 0)
            {
                AlertStatus.AddMessage(AlertType.Error, "At least one criterion entry must be added.");
                return;
            }

            var rubric = new QRubric
            {
                OrganizationIdentifier = Organization.Identifier,
                RubricIdentifier = UniqueIdentifier.Create(),
                RubricPoints = criteria.Sum(x => x.Points)
            };

            if (rubric.RubricPoints <= 0)
            {
                AlertStatus.AddMessage(AlertType.Error, "The rubric has no points.");
                return;
            }

            Detail.GetInputValues(rubric);

            var commands = new List<ICommand>
            {
                new CreateRubric(rubric.RubricIdentifier, rubric.RubricTitle),
                new DescribeRubric(rubric.RubricIdentifier, rubric.RubricDescription)
            };

            RubricCriteriaList.Save(rubric.RubricIdentifier, criteria, commands);

            ServiceLocator.SendCommand(new RunCommands(rubric.RubricIdentifier, commands.ToArray()));

            var returnUrl = GetReturnUrl();

            var url = !string.IsNullOrEmpty(returnUrl)
                ? AddUrlParameters(returnUrl, rubric.RubricIdentifier)
                : Outline.GetNavigateUrl(rubric.RubricIdentifier);

            HttpResponseHelper.Redirect(url);
        }

        private void SaveCopy()
        {
            var criteria = CopyCriteriaList.GetCriteria();
            if (criteria.Count == 0)
            {
                AlertStatus.AddMessage(AlertType.Error, "At least one criterion entry must be added.");
                return;
            }

            var rubric = new QRubric
            {
                OrganizationIdentifier = Organization.Identifier,
                RubricIdentifier = UniqueIdentifier.Create(),
                RubricPoints = criteria.Sum(x => x.Points)
            };

            if (rubric.RubricPoints <= 0)
            {
                AlertStatus.AddMessage(AlertType.Error, "The rubric has no points.");
                return;
            }

            CopyDetail.GetInputValues(rubric);

            var commands = new List<ICommand>
            {
                new CreateRubric(rubric.RubricIdentifier, rubric.RubricTitle),
                new DescribeRubric(rubric.RubricIdentifier, rubric.RubricDescription)
            };

            RubricCriteriaList.Save(rubric.RubricIdentifier, criteria, commands);

            ServiceLocator.SendCommand(new RunCommands(rubric.RubricIdentifier, commands.ToArray()));

            var returnUrl = GetReturnUrl();

            var url = !string.IsNullOrEmpty(returnUrl)
                ? AddUrlParameters(returnUrl, rubric.RubricIdentifier)
                : Outline.GetNavigateUrl(rubric.RubricIdentifier);

            HttpResponseHelper.Redirect(url);
        }

        private string AddUrlParameters(string url, Guid? rubric)
        {
            var result = string.Empty;

            if (rubric.HasValue)
                result += $"&rubric={rubric}";

            var type = Request.QueryString["type"];
            if (type.IsNotEmpty())
                result += $"&type={type}";

            result += "&returnFromRubric=1";

            if (!url.Contains("?"))
                result = "?" + result.Substring(1);

            return url + result;
        }

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/banks/outline")
                ? $"bank={Request["bank"]}"
                : GetParentLinkParameters(parent, null);
        }

        IWebRoute IOverrideWebRouteParent.GetParent()
            => GetParent();
    }
}