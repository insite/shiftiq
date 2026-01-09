using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Assessments.Options.Controls;
using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

using CheckBoxList = System.Web.UI.WebControls.CheckBoxList;

namespace InSite.Admin.Assessments.Questions.Forms
{
    public partial class Move : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private class AssetInfo : SnippetBuilder.StandardModel
        {
            #region Methods (helpers)

            public static AssetInfo[] Select(Expression<Func<Standard, bool>> filter)
            {
                return StandardSearch.Bind(
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

        private class FilterDataItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
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

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Guid SpecificationID => Guid.TryParse(Request.QueryString["spec"], out var value) ? value : Guid.Empty;

        private Guid SetID => Guid.TryParse(Request.QueryString["set"], out var value) ? value : Guid.Empty;

        protected IEnumerable<DestinationSelectorItem> DestinationSelectorItems
        {
            get => (IEnumerable<DestinationSelectorItem>)ViewState[nameof(DestinationSelectorItems)];
            set => ViewState[nameof(DestinationSelectorItems)] = value;
        }

        #endregion

        #region Fields


        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DestinationTradeSelector.AutoPostBack = true;
            DestinationTradeSelector.ValueChanged += DestinationTradeSelector_ValueChanged;

            DestinationBanksValidator.ServerValidate += DestinationBanksValidator_ServerValidate;

            DestinationFrameworkRepeater.ItemDataBound += DestinationFrameworkRepeater_ItemDataBound;

            MappingQuestionRepeater.ItemDataBound += MappingQuestionRepeater_ItemDataBound;

            NextButton.Click += DestinationNextButton_Click;
            SaveButton.Click += MappingSaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (IsPostBack)
                return;

            var filter = GetQuestionFilter();
            if (filter == null)
                RedirectToSearch();

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToSearch();

            PageHelper.AutoBindHeader(this, null, bank.Name);

            var readerUrl = GetReaderUrl();

            { // Destination
                DestinationTradeSelector.Filter.StandardTypes = new[] { Shift.Constant.StandardType.Profile };
                DestinationTradeSelector.Value = null;

                OnTradeSelected();
            }

            { // Mapping
                MappingBankName.Text = bank.Name;
                MappingBankTitle.Text = bank.Content.Title.Default;

                if (SpecificationID != Guid.Empty)
                {
                    var spec = bank.FindSpecification(SpecificationID);

                    MappingSpecificationField.Visible = true;
                    MappingSpecificationName.Text = spec.Name;
                }
                else
                {
                    MappingSpecificationField.Visible = false;
                }

                if (SetID != Guid.Empty)
                {
                    var set = bank.FindSet(SetID);

                    MappingSetField.Visible = true;
                    MappingSetName.Text = set.Name;
                }
                else
                {
                    MappingSetField.Visible = false;
                }

                if (filter.StandardIdentifier.HasValue)
                {
                    MappingCompetencyField.Visible = true;
                    MappingCompetency.AssetID = filter.StandardIdentifier.Value;
                }
                else
                {
                    MappingCompetencyField.Visible = false;
                }

                var filterItems = new List<FilterDataItem>();

                if (filter.Flag.IsNotEmpty())
                    filterItems.Add(new FilterDataItem
                    {
                        Name = "Question Flag",
                        Value = "<ul><li>" + string.Join("</li><li>", filter.Flag.Select(x => x.GetName())) + "</li></ul>"
                    });

                if (filter.Condition.IsNotEmpty())
                    filterItems.Add(new FilterDataItem
                    {
                        Name = "Condition",
                        Value = "<ul><li>" + string.Join("</li><li>", filter.Condition) + "</li></ul>"
                    });

                if (filter.Taxonomy.HasValue)
                {
                    var entity = TCollectionItemCache.SelectFirst(new TCollectionItemFilter
                    {
                        OrganizationIdentifier = Identity.Organization.Identifier,
                        CollectionName = CollectionName.Assessments_Questions_Classification_Taxonomy,
                        ItemSequence = filter.Taxonomy.Value
                    });

                    filterItems.Add(new FilterDataItem
                    {
                        Name = "Taxonomy",
                        Value = $"{entity.ItemSequence}. {entity.ItemName}"
                    });
                }

                if (filter.HasLig.HasValue)
                    filterItems.Add(new FilterDataItem
                    {
                        Name = "LIG Settings",
                        Value = filter.HasLig.Value ? "LIG" : "No LIG"
                    });

                if (filter.HasReference.HasValue)
                    filterItems.Add(new FilterDataItem
                    {
                        Name = "Reference Settings",
                        Value = filter.HasReference.Value ? "Reference" : "No Reference"
                    });

                MappingFilterColumn.Visible = filterItems.Count > 0;
                MappingFilterRepeater.DataSource = filterItems;
                MappingFilterRepeater.DataBind();

                BindMappingQuestionRepeater(bank);

                CancelButton.NavigateUrl = readerUrl;
            }

            SuccessCloseButton.NavigateUrl = readerUrl;
        }

        #endregion

        #region Methods (event handling)

        private void DestinationTradeSelector_ValueChanged(object sender, EventArgs e) =>
            OnTradeSelected();

        public void OnTradeSelected()
        {
            MappingSection.Visible = false;

            var hasSelection = DestinationTradeSelector.HasValue;

            BanksField.Visible = hasSelection;

            if (!hasSelection)
                return;

            var hasData = BindDestinationFrameworkRepeater();

            DestinationFrameworkRepeater.Visible = hasData;
            DestinationNoBanksMessage.Visible = !hasData;
        }

        private void DestinationBanksValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = DestinationFrameworkRepeater.Items.Cast<RepeaterItem>().Any(item =>
            {
                var bankList = (CheckBoxList)item.FindControl("BankList");

                return bankList.Items.Cast<System.Web.UI.WebControls.ListItem>().Any(x => x.Selected);
            });
        }

        private void DestinationFrameworkRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var banks = (QBank[])DataBinder.Eval(e.Item.DataItem, "Banks");
            var bankList = (CheckBoxList)e.Item.FindControl("BankList");

            bankList.Items.Clear();

            foreach (var bank in banks)
                bankList.Items.Add(new System.Web.UI.WebControls.ListItem
                {
                    Text = (bank.BankName ?? bank.BankTitle) + $" <span class='form-text text-nowrap'>Asset #{bank.AssetNumber}</span>",
                    Value = bank.BankIdentifier.ToString()
                });
        }

        private void MappingQuestionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var question = (Question)DataBinder.Eval(e.Item.DataItem, "Question");

            var optionRepeater = (OptionReadRepeater)e.Item.FindControl("OptionRepeater");
            optionRepeater.LoadData(question);
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

            MappingSection.Visible = true;
            NextButton.Visible = false;
            SaveButton.Visible = true;

            foreach (RepeaterItem rItem in MappingQuestionRepeater.Items)
            {
                var bankInput = (HiddenField)rItem.FindControl("DestinationBankIdentifier");
                var setinput = (HiddenField)rItem.FindControl("DestinationSetIdentifier");
                var competencyInput = (HiddenField)rItem.FindControl("DestinationCompetencyIdentifier");

                bankInput.Value = string.Empty;
                setinput.Value = string.Empty;
                competencyInput.Value = string.Empty;
            }
        }

        private void MappingSaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            MoveContainer.Visible = false;

            var ids = new List<Guid>();
            var commands = new List<MoveQuestion>();

            foreach (RepeaterItem rItem in MappingQuestionRepeater.Items)
            {
                var questionInput = (ITextControl)rItem.FindControl("QuestionId");
                var bankInput = (HiddenField)rItem.FindControl("DestinationBankIdentifier");
                var setinput = (HiddenField)rItem.FindControl("DestinationSetIdentifier");
                var competencyInput = (HiddenField)rItem.FindControl("DestinationCompetencyIdentifier");

                if (!Guid.TryParse(bankInput.Value, out var destBankId) || !Guid.TryParse(setinput.Value, out var destSetId) || !Guid.TryParse(competencyInput.Value, out var destCompetencyId))
                    continue;

                var questionId = Guid.Parse(questionInput.Text);

                ids.Add(questionId);
                commands.Add(new MoveQuestion(BankID, destBankId, destSetId, destCompetencyId, 0, questionId));
            }

            var hasAttempts = GetHasAttemptsFilter(ids);
            var commandsCount = 0;

            foreach (var command in commands)
            {
                if (hasAttempts.Contains(command.Question))
                    continue;

                command.Asset = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);

                ServiceLocator.SendCommand(command);

                commandsCount++;
            }

            SuccessMessage.Visible = true;

            SuccessStatus.AddMessage(AlertType.Success, $"{"question".ToQuantity(commandsCount)} have been successfully moved.");
        }

        #endregion

        #region Methods (data binding)

        private bool BindDestinationFrameworkRepeater()
        {
            var frameworks = AssetInfo.Select(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && x.ParentStandardIdentifier == DestinationTradeSelector.Value
                  && x.StandardType == Shift.Constant.StandardType.Framework);

            if (frameworks.Length == 0)
                return false;

            var banks = ServiceLocator.BankSearch.GetBanks(new QBankFilter
            {
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                FrameworkIdentifiers = frameworks.Select(x => x.Identifier).ToArray()
            }).GroupBy(x => x.FrameworkIdentifier).ToDictionary(x => x.Key, x => x.OrderBy(y => y.BankName).ToArray());

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

        private void BindMappingQuestionRepeater(BankState bank)
        {
            IEnumerable<Question> questions;

            if (SpecificationID != Guid.Empty)
            {
                var spec = bank.FindSpecification(SpecificationID);
                var sets = spec.Criteria.AsQueryable().SelectMany(x => x.Sets);

                if (SetID != Guid.Empty)
                    sets = sets.Where(x => x.Identifier == SetID);

                questions = sets.SelectMany(x => x.Questions);
            }
            else
            {
                throw new NotImplementedException();
            }

            var filter = GetQuestionFilter();

            questions = questions.Filter(filter).ToArray();

            var hasAttempts = GetHasAttemptsFilter(questions.Select(x => x.Identifier));

            MappingQuestionRepeater.DataSource = questions.Select(x => new
            {
                Question = x,
                AllowMove = !hasAttempts.Contains(x.Identifier) && x.IsLastVersion() && x.Fields.Count == 0
            });
            MappingQuestionRepeater.DataBind();
        }

        private void BindMappingSelectItems(Guid[] bankId)
        {
            var banks = ServiceLocator.BankSearch.GetBankStates(bankId);

            var gacFilter = banks.SelectMany(x => x.Sets.Select(y => y.Standard)).Distinct().ToArray();
            var gacs = AssetInfo.Select(x => gacFilter.Contains(x.StandardIdentifier) && x.Parent != null).ToDictionary(x => x.Identifier);

            var competencies = AssetInfo.Select(
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                  && gacFilter.Contains(x.Parent.StandardIdentifier)
                  && x.StandardType == Shift.Constant.StandardType.Competency
            ).GroupBy(x => x.ParentIdentifier).ToDictionary(x => x.Key, x => x.ToArray());

            var items = new List<DestinationSelectorItem>();

            foreach (var bank in banks)
            {
                var bankTitle = HttpUtility.HtmlEncode(bank.Name.IfNullOrEmpty(bank.Content.Title?.Default));
                var bankItem = new DestinationSelectorItem
                {
                    Identifier = bank.Identifier,
                    Text = bankTitle,
                    Html = $"{bankTitle} <span class='form-text text-nowrap'>Asset #{bank.Asset}</span>",
                };

                foreach (var set in bank.Sets)
                {
                    var setTitle = HttpUtility.HtmlEncode(set.Name);
                    var gacTitle = gacs.ContainsKey(set.Standard)
                        ? SnippetBuilder.GetHtml(gacs[set.Standard])
                        : "<strong>(Undefined)</strong>";

                    var setItem = new DestinationSelectorItem
                    {
                        Identifier = set.Identifier,
                        Text = setTitle,
                        Html = $"{setTitle} <div style='font-size:13px;'>{gacTitle}</div>"
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
                                    ? HttpUtility.HtmlEncode(competency.Title)
                                    : $"{HttpUtility.HtmlEncode(competency.Code)}. {HttpUtility.HtmlEncode(competency.Title)}",
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
        #endregion

        #region Methods (helpers)

        private static HashSet<Guid> GetHasAttemptsFilter(IEnumerable<Guid> input) =>
            ServiceLocator.AttemptSearch.GetExistsQuestionIdentifiers(input).ToHashSet();

        private QuestionFilter GetQuestionFilter() => QuestionFilterSerializer.Deserialize(Request.QueryString["filter"]);

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
            return new ReturnUrl().GetReturnUrl()
                ?? $"/ui/admin/assessments/banks/outline?bank={BankID}";
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