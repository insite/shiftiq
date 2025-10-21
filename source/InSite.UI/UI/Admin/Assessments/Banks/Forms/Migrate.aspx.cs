using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

using CheckBoxList = System.Web.UI.WebControls.CheckBoxList;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class Migrate : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private class AssetInfo : SnippetBuilder.StandardModel
        {
            #region Methods (helpers)

            public static AssetInfo[] Select(Expression<Func<Persistence.Standard, bool>> filter)
            {
                return Persistence.StandardSearch.Bind(
                    x => new AssetInfo
                    {
                        Identifier = x.StandardIdentifier,
                        Label = x.StandardLabel,
                        Type = x.StandardType,
                        Name = x.ContentName,
                        Number = x.AssetNumber,
                        Title = x.ContentTitle,
                        Code = x.Code,

                        ParentIdentifier = x.Parent.StandardIdentifier,
                        ParentCode = x.Parent.Code
                    }, filter, null, "Sequence");
            }

            #endregion
        }

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        protected class DestinationSelectorItem
        {
            [JsonProperty(PropertyName = "id")]
            public Guid Identifier { get; set; }

            [JsonProperty(PropertyName = "text")]
            public string Text { get; set; }

            [JsonProperty(PropertyName = "html")]
            public string Html { get; set; }

            [JsonProperty(PropertyName = "children")]
            public List<DestinationSelectorItem> Children { get; } = new List<DestinationSelectorItem>();
        }

        [Serializable]
        protected class MappingCompetencyInfo
        {
            public Guid CompetencyIdentifier { get; }
            public Guid[] QuestionIdentifiers { get; }
            public MappingDestinationInfo Destination { get; set; }

            public MappingCompetencyInfo(Guid competency, IEnumerable<Guid> questions)
            {
                CompetencyIdentifier = competency;
                QuestionIdentifiers = questions.ToArray();
            }
        }

        [Serializable]
        protected class MappingDestinationInfo
        {
            public Guid BankIdentifier { get; set; }
            public Guid SetIdentifier { get; set; }
            public Guid CompetencyIdentifier { get; set; }
        }

        [Serializable]
        protected class MappingInfo
        {
            public Guid SetIdentifier { get; }
            public Guid GacIdentifier { get; }
            public MappingCompetencyInfo[] Competencies { get; set; }

            public MappingInfo(Guid set, Guid gac)
            {
                SetIdentifier = set;
                GacIdentifier = gac;
            }
        }

        private class MappingGacDataItem
        {
            public MappingInfo Info { get; set; }
            public MappingDataItemSequence Sequence { get; set; }
            public string HtmlTitle { get; set; }
            public MappingCompetencyDataItem[] Competencies { get; set; }
        }

        private class MappingCompetencyDataItem
        {
            public MappingCompetencyInfo Info { get; set; }
            public MappingDataItemSequence Sequence { get; set; }
            public string HtmlTitle { get; set; }
        }

        private class MappingDataItemSequence
        {
            public int Position { get; set; } = -1;
            public bool IsLetter { get; set; }
            public bool IsDigit { get; set; }
        }

        private class PreviewDataItem
        {
            public int QuestionSequence { get; internal set; }
            public string QuestionTitle { get; internal set; }
            public string DestinationBankName { get; internal set; }
            public string DestinationSetName { get; internal set; }
            public Guid DestinationGacId { get; internal set; }
            public string DestinationGacHtml { get; internal set; }
            public Guid DestinationCompetencyId { get; internal set; }
            public string DestinationCompetencyHtml { get; internal set; }
        }

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        protected IEnumerable<DestinationSelectorItem> DestinationSelectorItems
        {
            get => (IEnumerable<DestinationSelectorItem>)ViewState[nameof(DestinationSelectorItems)];
            set => ViewState[nameof(DestinationSelectorItems)] = value;
        }

        private MappingInfo[] MappingData
        {
            get => (MappingInfo[])ViewState[nameof(MappingData)];
            set => ViewState[nameof(MappingData)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DestinationTradeSelector.AutoPostBack = true;
            DestinationTradeSelector.ValueChanged += DestinationTradeSelector_ValueChanged;

            DestinationBanksValidator.ServerValidate += DestinationBanksValidator_ServerValidate;

            DestinationFrameworkRepeater.ItemDataBound += DestinationFrameworkRepeater_ItemDataBound;

            DestinationNextButton.Click += DestinationNextButton_Click;
            MappingNextButton.Click += MappingNextButton_Click;
            PreviewNextButton.Click += PreviewNextButton_Click;

            MappingGacRepeater.ItemDataBound += MappingGacRepeater_ItemDataBound;

            MappingSelectionValidator.ServerValidate += MappingSelectionValidator_ServerValidate;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (IsPostBack)
                return;

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
            {
                RedirectToSearch();
                return;
            }

            PageHelper.AutoBindHeader(this, null, $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>");
            BankDetails.BindBank(bank);

            DestinationTradeSelector.Filter.StandardTypes = new[] { StandardType.Profile };
            DestinationTradeSelector.Value = null;

            OnTradeSelected();

            DestinationCancelButton.NavigateUrl = GetReaderUrl();
            MappingCancelButton.NavigateUrl = GetReaderUrl();
            PreviewCancelButton.NavigateUrl = GetReaderUrl();

            MappingBankName.Text = bank.Name;
            MappingBankTitle.Text = bank.Content.Title.Default;
            MappingBankStandard.AssetID = bank.Standard;

            if (bank.Standard != Guid.Empty)
            {
                var standard = Persistence.StandardSearch.BindFirst(x => x, x => x.StandardIdentifier == bank.Standard);
                if (standard?.CompetencyScoreSummarizationMethod != null)
                    MappingBankStandardCalculationMethod.Text = $"<span class='badge bg-custom-default'>{OutlineHelper.DisplayCalculationMethod(standard.CompetencyScoreSummarizationMethod)}</span>";
            }

            {
                MappingData = BindMappingBankSetRepeater(bank);

                var hasData = MappingData.Length > 0;

                MappingGacRepeater.Visible = hasData;
                MappingNoSetMessage.Visible = !hasData;
            }

            MappingBankSetCount.Text = bank.Sets.Count.ToString("n0");
            MappingBankQuestionCount.Text = MappingData.Sum(x => x.Competencies.Sum(y => y.QuestionIdentifiers.Length)).ToString("n0");
        }

        #endregion

        #region Event handlers

        private void DestinationTradeSelector_ValueChanged(object sender, EventArgs e) =>
            OnTradeSelected();

        public void OnTradeSelected()
        {
            ClearMappingDestination();
            MappingSection.Visible = false;
            PreviewSection.Visible = false;

            if (!DestinationSection.IsSelected)
                DestinationSection.IsSelected = true;

            var hasSelection = DestinationTradeSelector.HasValue;

            BanksField.Visible = hasSelection;

            if (!hasSelection)
                return;

            var hasData = BindDestinationFrameworkRepeater();

            DestinationFrameworkRepeater.Visible = hasData;
            DestinationNoBanksMessage.Visible = !hasData;
        }

        private void DestinationFrameworkRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (IsContentItem(e))
            {
                var banks = (QBank[])DataBinder.Eval(e.Item.DataItem, "Banks");
                var bankList = (CheckBoxList)e.Item.FindControl("BankList");

                bankList.Items.Clear();
                foreach (var bank in banks)
                {
                    var item = new System.Web.UI.WebControls.ListItem
                    {
                        Text = (bank.BankName ?? bank.BankTitle) + $" <span class='form-text text-nowrap'>Asset #{bank.AssetNumber}</span>",
                        Value = bank.BankIdentifier.ToString()
                    };

                    bankList.Items.Add(item);
                }
            }
        }

        private void DestinationBanksValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = DestinationFrameworkRepeater.Items.Cast<RepeaterItem>().Any(item =>
            {
                var bankList = (CheckBoxList)item.FindControl("BankList");

                return bankList.Items.Cast<System.Web.UI.WebControls.ListItem>().Any(x => x.Selected);
            });
        }

        private void DestinationNextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var selectedBanks = DestinationFrameworkRepeater.Items.Cast<RepeaterItem>().SelectMany(item =>
            {
                var bankList = (CheckBoxList)item.FindControl("BankList");

                return bankList.Items.Cast<System.Web.UI.WebControls.ListItem>().Where(x => x.Selected).Select(x => Guid.Parse(x.Value));
            }).ToArray();

            BindMappingSelectItems(selectedBanks);

            ClearMappingDestination();
            MappingSection.Visible = true;
            MappingSection.IsSelected = true;

            PreviewSection.Visible = false;

            foreach (RepeaterItem gacItem in MappingGacRepeater.Items)
            {
                var competencyRepeater = (Repeater)gacItem.FindControl("CompetencyRepeater");

                foreach (RepeaterItem item in competencyRepeater.Items)
                {
                    var bankInput = (HiddenField)item.FindControl("DestinationBankIdentifier");
                    var setinput = (HiddenField)item.FindControl("DestinationSetIdentifier");
                    var competencyInput = (HiddenField)item.FindControl("DestinationCompetencyIdentifier");

                    bankInput.Value = string.Empty;
                    setinput.Value = string.Empty;
                    competencyInput.Value = string.Empty;
                }
            }
        }

        private void MappingSelectionValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = MappingGacRepeater.Items.Cast<RepeaterItem>().Any(gacItem =>
            {
                var competencyRepeater = (Repeater)gacItem.FindControl("CompetencyRepeater");

                return competencyRepeater.Items.Cast<RepeaterItem>().Any(item =>
                {
                    var bankInput = (HiddenField)item.FindControl("DestinationBankIdentifier");
                    var setinput = (HiddenField)item.FindControl("DestinationSetIdentifier");
                    var competencyInput = (HiddenField)item.FindControl("DestinationCompetencyIdentifier");

                    return !string.IsNullOrEmpty(bankInput.Value)
                        && !string.IsNullOrEmpty(setinput.Value)
                        && !string.IsNullOrEmpty(competencyInput.Value);
                });
            });
        }

        private void MappingNextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            foreach (RepeaterItem gacItem in MappingGacRepeater.Items)
            {
                var competencyRepeater = (Repeater)gacItem.FindControl("CompetencyRepeater");

                foreach (RepeaterItem item in competencyRepeater.Items)
                {
                    var bankInput = (HiddenField)item.FindControl("DestinationBankIdentifier");
                    var setinput = (HiddenField)item.FindControl("DestinationSetIdentifier");
                    var competencyInput = (HiddenField)item.FindControl("DestinationCompetencyIdentifier");

                    if (Guid.TryParse(bankInput.Value, out var bankId) && Guid.TryParse(setinput.Value, out var setId) && Guid.TryParse(competencyInput.Value, out var competencyId))
                        MappingData[gacItem.ItemIndex].Competencies[item.ItemIndex].Destination = new MappingDestinationInfo
                        {
                            BankIdentifier = bankId,
                            SetIdentifier = setId,
                            CompetencyIdentifier = competencyId
                        };
                }
            }

            PreviewSection.Visible = true;
            PreviewSection.IsSelected = true;

            BindPreviewQuestionRepeater();
        }

        private void MappingGacRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var repeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            repeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Competencies");
            repeater.DataBind();
        }

        private void PreviewNextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                Save();
            }
            catch (ApplicationError apperr)
            {
                WriteStatus.AddMessage(AlertType.Error, apperr.Message);
                return;
            }

            RedirectToReader();
        }

        #endregion

        #region Methods (data binding)

        private bool BindDestinationFrameworkRepeater()
        {
            var frameworks = AssetInfo.Select(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.ParentStandardIdentifier == DestinationTradeSelector.Value
                  && x.StandardType == StandardType.Framework);

            if (frameworks.Length == 0)
                return false;

            var banks = ServiceLocator.BankSearch.GetBanks(new QBankFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                FrameworkIdentifiers = frameworks.Select(x => x.Identifier).ToArray()
            }).Where(x => x.BankIdentifier != BankID).GroupBy(x => x.FrameworkIdentifier).ToDictionary(x => x.Key, x => x.OrderBy(y => y.BankName).ToArray());

            if (banks.Count == 0)
                return false;

            DestinationFrameworkRepeater.DataSource = frameworks.OrderBy(x => x.Title).Where(x => banks.ContainsKey(x.Identifier)).Select(x => new
            {
                HtmlTitle = SnippetBuilder.GetHtml(x),
                Banks = banks[x.Identifier]
            });
            DestinationFrameworkRepeater.DataBind();

            return true;
        }

        private MappingInfo[] BindMappingBankSetRepeater(BankState bank)
        {
            var standardsFilter = new HashSet<Guid>();

            var dataSource = bank.Sets.Select(set =>
            {
                standardsFilter.Add(set.Standard);

                return new MappingGacDataItem
                {
                    Info = new MappingInfo(set.Identifier, set.Standard),
                    Competencies = set.Questions.Where(q => q.Condition != "Purge").GroupBy(q => q.Standard).Select(group =>
                    {
                        standardsFilter.Add(group.Key);

                        return new MappingCompetencyDataItem
                        {
                            Info = new MappingCompetencyInfo(group.Key, group.Select(q => q.Identifier))
                        };
                    }).ToArray()
                };
            }).ToArray();

            var standards = AssetInfo
                .Select(x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && standardsFilter.Contains(x.StandardIdentifier) && x.Parent != null)
                .ToDictionary(x => x.Identifier);

            if (standards.Count == 0)
                return new MappingInfo[0];

            var applySequenceGac = true;

            foreach (var g in dataSource)
            {
                g.HtmlTitle = standards.TryGetValue(g.Info.GacIdentifier, out var gStandard)
                    ? SnippetBuilder.GetHtml(gStandard, false)
                    : "(Undefined)";
                g.Sequence = GetSequence(g.HtmlTitle);

                if (g.Sequence.Position == -1)
                    applySequenceGac = false;

                var applySequenceCompetency = true;

                foreach (var c in g.Competencies)
                {
                    c.HtmlTitle = standards.TryGetValue(c.Info.CompetencyIdentifier, out var cStandard)
                        ? SnippetBuilder.GetHtml(cStandard, false)
                        : "(Undefined)";
                    c.Sequence = GetSequence(c.HtmlTitle);

                    if (c.Sequence.Position == -1)
                        applySequenceCompetency = false;
                }

                if (applySequenceCompetency)
                    g.Competencies = g.Competencies.OrderBy(x => x.Sequence.Position).ThenBy(x => x.HtmlTitle).ToArray();
                else
                    g.Competencies = g.Competencies.OrderBy(x => x.HtmlTitle).ToArray();

                g.Info.Competencies = g.Competencies.Select(x => x.Info).ToArray();
            }

            if (applySequenceGac)
                dataSource = dataSource.OrderBy(x => x.Sequence.Position).ThenBy(x => x.HtmlTitle).ToArray();

            MappingGacRepeater.DataSource = dataSource;
            MappingGacRepeater.DataBind();

            return dataSource.Select(x => x.Info).ToArray();

            MappingDataItemSequence GetSequence(string title)
            {
                var result = new MappingDataItemSequence();

                if (title.IsEmpty())
                    return result;

                var index = title.IndexOf(".");
                if (index < 1 || index > 3)
                    return result;

                var value = title.Substring(0, index).ToUpper();

                for (var i = 0; i < value.Length; i++)
                {
                    var c = value[i];
                    if (c >= '0' && c <= '9')
                        result.IsDigit = true;
                    else if (c >= 'A' && c <= 'Z')
                        result.IsLetter = true;
                    else
                    {
                        result.IsLetter = false;
                        result.IsDigit = false;
                        break;
                    }
                }

                if (result.IsDigit && !result.IsLetter)
                    result.Position = int.Parse(value);
                else if (!result.IsDigit && result.IsLetter)
                    result.Position = Calculator.FromBase26(value);

                return result;
            }
        }

        private void BindMappingSelectItems(Guid[] selectedBanks)
        {
            var banks = ServiceLocator.BankSearch.GetBankStates(selectedBanks);

            var gacFilter = banks.SelectMany(x => x.Sets.Select(y => y.Standard)).Distinct().ToArray();
            var gacs = AssetInfo.Select(x => gacFilter.Contains(x.StandardIdentifier) && x.Parent != null).ToDictionary(x => x.Identifier);

            var competencies = AssetInfo.Select(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && gacFilter.Contains(x.Parent.StandardIdentifier)
                  && x.StandardType == StandardType.Competency
            ).GroupBy(x => x.ParentIdentifier).ToDictionary(x => x.Key, x => x.ToArray());

            var items = new List<DestinationSelectorItem>();

            foreach (var bank in banks)
            {
                var bankTitle = bank.Name.IfNullOrEmpty(() => bank.Content.Title?.Default);
                var bankItem = new DestinationSelectorItem
                {
                    Identifier = bank.Identifier,
                    Text = bankTitle,
                    Html = $"{HttpUtility.HtmlEncode(bankTitle)} <span class='form-text text-nowrap'>Asset #{bank.Asset}</span>",
                };

                foreach (var set in bank.Sets)
                {
                    var setTitle = set.Name;
                    var gacTitle = gacs.ContainsKey(set.Standard)
                        ? SnippetBuilder.GetHtml(gacs[set.Standard])
                        : "<strong>(Undefined)</strong>";

                    var setItem = new DestinationSelectorItem
                    {
                        Identifier = set.Identifier,
                        Text = setTitle,
                        Html = $"{HttpUtility.HtmlEncode(setTitle)} <div style='font-size:13px;'>{gacTitle}</div>"
                    };

                    if (competencies.ContainsKey(set.Standard))
                    {
                        var setCompetencies = competencies[set.Standard];

                        foreach (var competency in setCompetencies)
                        {
                            setItem.Children.Add(new DestinationSelectorItem
                            {
                                Identifier = competency.Identifier,
                                Text = string.IsNullOrEmpty(competency.Code)
                                    ? competency.Title
                                    : $"{competency.Code}. {competency.Title}",
                                Html = SnippetBuilder.GetHtml(competency)
                            });
                        }
                    }

                    bankItem.Children.Add(setItem);
                }

                items.Add(bankItem);
            }

            DestinationSelectorItems = items;
        }

        private void BindPreviewQuestionRepeater()
        {
            var bankFilter = MappingData
                .SelectMany(x => x.Competencies).Where(x => x.Destination != null)
                .Select(x => x.Destination.BankIdentifier).Concat(new Guid[] { BankID }).Distinct().ToArray();
            var allBanks = ServiceLocator.BankSearch.GetBankStates(bankFilter).ToDictionary(x => x.Identifier);

            var sourceBank = allBanks[BankID];
            var gacMapping = new Dictionary<Guid, List<PreviewDataItem>>();
            var competencyMapping = new Dictionary<Guid, List<PreviewDataItem>>();

            var dataSource = MappingData.SelectMany(x => x.Competencies).Where(x => x.Destination != null)
                .SelectMany(
                    mapItem => mapItem.QuestionIdentifiers.Select(
                        questionId =>
                        {
                            var question = sourceBank.FindQuestion(questionId);
                            if (question == null)
                                return null;

                            var destBank = allBanks[mapItem.Destination.BankIdentifier];
                            var destSet = destBank.FindSet(mapItem.Destination.SetIdentifier);

                            var result = new PreviewDataItem
                            {
                                QuestionSequence = question.BankIndex + 1,
                                QuestionTitle = question.Content.Title?.Default,
                                DestinationBankName = destBank.Name,
                                DestinationSetName = destSet.Name,
                                DestinationGacId = destSet.Standard,
                                DestinationCompetencyId = mapItem.Destination.CompetencyIdentifier
                            };

                            if (!gacMapping.ContainsKey(result.DestinationGacId))
                                gacMapping.Add(result.DestinationGacId, new List<PreviewDataItem>());

                            gacMapping[result.DestinationGacId].Add(result);

                            if (!competencyMapping.ContainsKey(result.DestinationCompetencyId))
                                competencyMapping.Add(result.DestinationCompetencyId, new List<PreviewDataItem>());

                            competencyMapping[result.DestinationCompetencyId].Add(result);

                            return result;
                        }))
                .Where(x => x != null)
                .OrderBy(x => x.QuestionSequence)
                .ToArray();

            var gacsInfo = AssetInfo.Select(x => gacMapping.Keys.Contains(x.StandardIdentifier) && x.Parent != null);
            foreach (var info in gacsInfo)
            {
                foreach (var item in gacMapping[info.Identifier])
                    item.DestinationGacHtml = SnippetBuilder.GetHtml(info);
            }

            var competencies = AssetInfo.Select(x => competencyMapping.Keys.Contains(x.StandardIdentifier) && x.Parent != null);
            foreach (var info in competencies)
            {
                foreach (var item in competencyMapping[info.Identifier])
                    item.DestinationCompetencyHtml = SnippetBuilder.GetHtml(info);
            }

            PreviewQuestionRepeater.DataSource = dataSource;
            PreviewQuestionRepeater.DataBind();
        }

        #endregion

        #region Methods (database operations)

        private void Save()
        {
            var sourceBank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (sourceBank == null)
                throw new ApplicationError("The source bank not found.");

            var commands = new List<AddQuestion>();

            foreach (var item in MappingData)
            {
                foreach (var mapping in item.Competencies)
                {
                    if (mapping.Destination == null)
                        continue;

                    foreach (var questionId in mapping.QuestionIdentifiers)
                    {
                        var sourceQuestion = sourceBank.FindQuestion(questionId)
                            ?? throw new ApplicationError($"The source question ({questionId}) not found.");

                        var destQuestion = sourceQuestion.Clone();
                        destQuestion.Identifier = UniqueIdentifier.Create();
                        destQuestion.Standard = mapping.Destination.CompetencyIdentifier;
                        destQuestion.SubStandards = null;
                        destQuestion.Condition = "Unassigned";
                        destQuestion.Asset = -1;
                        destQuestion.Source = sourceQuestion.Identifier;

                        commands.Add(new AddQuestion(mapping.Destination.BankIdentifier, mapping.Destination.SetIdentifier, destQuestion));
                    }
                }
            }

            if (commands.Count == 0)
                throw new ApplicationError("Nothing to migrate.");

            var numbers = Persistence.Sequence.IncrementMany(Organization.OrganizationIdentifier, SequenceType.Asset, commands.Count);
            for (var i = 0; i < numbers.Length; i++)
                commands[i].Question.Asset = numbers[i];

            foreach (var cmd in commands)
                ServiceLocator.SendCommand(cmd);
        }

        #endregion

        #region Methods (helpers)

        private void ClearMappingDestination()
        {
            if (MappingData == null)
                return;

            foreach (var item in MappingData)
                foreach (var mapping in item.Competencies)
                    mapping.Destination = null;
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader()
        {
            var url = GetReaderUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl()
        {
            return $"/ui/admin/assessments/banks/outline?bank={BankID}";
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}
