using System;
using System.Linq;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Outlines.Controls
{
    public partial class BankSection : BaseUserControl
    {
        #region Properties

        private const string DeleteUrl = "/admin/assessments/banks/delete";

        protected Guid BankID
        {
            get => (Guid)ViewState[nameof(BankID)];
            set => ViewState[nameof(BankID)] = value;
        }

        public Func<BankState> LoadBank { get; internal set; }

        public bool HasSummaries => StatisticsTab.Visible;

        #endregion

        #region Data binding

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DisableBankButton.Click += (x, y) => ChangeBankStatus(false);
            EnableBankButton.Click += (x, y) => ChangeBankStatus(true);
        }

        public byte[] GetSummariesXlsx(BankState bank)
            => QuestionStatisticsPanel.GetXlsx(bank);

        public void LoadData(BankState bank, QBank bankQuery, bool canWrite, bool canDelete, out bool isSelected)
        {
            BankID = bank.Identifier;

            isSelected = false;

            { // Identification

                DeleteLink.NavigateUrl = DeleteUrl + $"?bank={bank.Identifier}";
                DeleteLink.Visible = canDelete;

                AssetNumber.Text = bank.Asset.ToString();
                BankLevel.Text = bank.Level.ToString();
                BankLevelField.Visible = bank.IsAdvanced;
                BankName.Text = bank.Name;
                BankNameField.Visible = bank.IsAdvanced;

                SetBankStatus(bank.IsActive);

                BankStandardField.Visible = bank.IsAdvanced;
                ChangeBankStandard.NavigateUrl = $"/ui/admin/assessments/banks/change-standard?bank={bank.Identifier}";

                BankTitle.Text = bank.Content.Title.Default;
                BankEdition.Text = bank.Edition.ToString();
                BankEditionField.Visible = bank.IsAdvanced;

                LockedLabel.Visible = bank.IsLocked;
                UnlockQuestionBank.Visible = bank.IsLocked && canWrite;
                UnlockedLabel.Visible = !bank.IsLocked;
                LockQuestionBank.Visible = !bank.IsLocked && canWrite;

                if (LockedLabel.Visible)
                    UnlockQuestionBank.NavigateUrl = $"/ui/admin/assessments/banks/lock?command=unlock&bank={bank.Identifier}";

                if (UnlockedLabel.Visible)
                    LockQuestionBank.NavigateUrl = $"/ui/admin/assessments/banks/lock?command=lock&bank={bank.Identifier}";

                if (bank.Standard != Guid.Empty)
                {
                    var standard = StandardSearch.BindFirst(x => x, x => x.StandardIdentifier == bank.Standard);
                    if (standard != null)
                    {
                        if (standard.CompetencyScoreSummarizationMethod != null)
                            BankStandardCalculationMethod.Text = $"<span class='badge bg-custom-default'>{OutlineHelper.DisplayCalculationMethod(standard.CompetencyScoreSummarizationMethod)}</span>";

                        BankStandard.Text = $"<a href=\"/ui/admin/standards/edit?id={standard.StandardIdentifier}\">{standard.ContentTitle ?? standard.ContentName ?? standard.Code}</a>";
                    }
                }
                else
                    BankStandard.Text = "None";

                ChangeBankLevel.NavigateUrl = $"/ui/admin/assessments/banks/change-level?bank={bank.Identifier}";
                RenameBankLink2.NavigateUrl = $"/ui/admin/assessments/banks/rename?bank={bank.Identifier}";
                ChangeBankEdition.NavigateUrl = $"/ui/admin/assessments/banks/change-level?bank={bank.Identifier}";
            }

            { // Content
                var content = bank.Content ?? new ContentExamBank();

                Summary.Text = content.Summary.Default.IfNullOrEmpty("None");
                ExamMaterialsForDistribution.Text = Markdown.ToHtml(content.MaterialsForDistribution.Default.IfNullOrEmpty("None"));
                ExamMaterialsForParticipation.Text = Markdown.ToHtml(content.MaterialsForParticipation.Default.IfNullOrEmpty("None"));

                EditBankTitleLink.NavigateUrl = $"/ui/admin/assessments/banks/content?bank={bank.Identifier}&tab=title";
                EditBankSummaryLink.NavigateUrl = $"/ui/admin/assessments/banks/content?bank={bank.Identifier}&tab=summary";
                EditExamMaterialsLink1.NavigateUrl = EditExamMaterialsLink2.NavigateUrl = $"/ui/admin/assessments/banks/content?bank={bank.Identifier}&tab=materials";

                StatisticsTab.Visible = bank.IsAdvanced && StatisticsPanel.LoadData(bank);

                TranslationsTab.Visible = BankTranslationPanel.LoadData(bank.Tenant);
            }

            { // Measurements
                SetCount.Text = bankQuery.SetCount.ToString("n0");
                QuestionCount.Text = bankQuery.QuestionCount.ToString("n0");
                QuestionPointsSum.Text = bank.Sets.SelectMany(x => x.Questions).Sum(x => x.Points ?? 0).ToString("n0");
                OptionCount.Text = bankQuery.OptionCount.ToString("n0");
                SpecificationCount.Text = bankQuery.SpecCount.ToString("n0");
                FormCount.Text = bankQuery.FormCount.ToString("n0");

                BankSize.Text = $"{GetBankSize(bank):n0} KB";

                var attemptCount = ServiceLocator.AttemptSearch.CountAttempts(new QAttemptFilter { BankIdentifier = bank.Identifier });
                AttemptCount.Text = attemptCount.ToString("n0");
            }

            ProblemRepeater.BindModelToControls(bank);
            ProblemsTab.Visible = ProblemRepeater.ItemCount > 0;

            if (!IsPostBack)
            {
                var panel = Request.QueryString["panel"];
                if (panel == "bank.problems")
                {
                    isSelected = true;

                    if (ProblemsTab.Visible)
                        ProblemsTab.IsSelected = true;
                }
            }

            SetPermissions(canWrite);
        }

        private void SetPermissions(bool canWrite)
        {
            ChangeBankStandard.Visible = canWrite;
            RenameBankLink2.Visible = canWrite;
            ChangeBankLevel.Visible = canWrite;
            ChangeBankEdition.Visible = canWrite;
            EditBankTitleLink.Visible = canWrite;
            EditBankSummaryLink.Visible = canWrite;
            EditExamMaterialsLink1.Visible = canWrite;
            EditExamMaterialsLink2.Visible = canWrite;
        }

        private static readonly MemoryCache<int> _cache = new MemoryCache<int>();

        private int GetBankSize(BankState bank)
        {
            var key = $"Admin/Assessments/Banks/{bank.Identifier}/Size";
            if (!_cache.Exists(key))
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };
                var json = JsonConvert.SerializeObject(bank, Formatting.Indented, settings);
                _cache.Add(key, json.Length / 1024, 3600, true); // 60 minutes
            }
            return _cache[key];
        }

        private void ChangeBankStatus(bool disabled)
        {
            ServiceLocator.SendCommand(new ChangeBankStatus(BankID, disabled));

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank != null)
                SetBankStatus(bank.IsActive);
            else
                HttpResponseHelper.Redirect(Request.RawUrl);
        }

        private void SetBankStatus(bool isActive)
        {
            DisableBankButton.Visible = isActive;
            EnableBankButton.Visible = !isActive;
            BankStatusHelp.InnerText = isActive ? "Bank status is Active" : "Bank status is Inactive";
        }

        #endregion
    }
}