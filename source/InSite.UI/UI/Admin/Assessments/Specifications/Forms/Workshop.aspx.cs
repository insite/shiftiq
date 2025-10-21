using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Specifications.Forms
{
    public partial class Workshop : AdminBasePage, IHasParentLinkParameters
    {
        #region Classes

        private interface IJsonResult
        {
            string Type { get; }
        }

        private class UploadSuccessResult : IJsonResult
        {
            public UploadSuccessResult(string path, string name, bool isImage)
            {
                Type = "OK";
                Path = path;
                Name = name;
                IsImage = isImage;
            }

            public string Path { get; }
            public string Name { get; }
            public bool IsImage { get; }
            public string Type { get; }
        }

        private class UploadErrorResult : IJsonResult
        {
            public UploadErrorResult()
            {
                Type = "ERROR";
                Messages = new List<string>();
            }

            public List<string> Messages { get; }
            public string Type { get; }
        }

        [Serializable]
        private class FormDataInfo
        {
            public int FormLimit { get; set; }
            public int QuestionLimit { get; set; }

            public List<CriterionInfo> Criteria { get; } = new List<CriterionInfo>();
        }

        [Serializable]
        private class CriterionInfo
        {
            public Guid Identifier { get; internal set; }
            public decimal SetWeight { get; set; }

            public List<CriterionFilterInfo> Filter { get; } = new List<CriterionFilterInfo>();
        }

        [Serializable]
        private class CriterionFilterInfo
        {
            public int? Competency { get; set; }

            public int Tax1 { get; set; }
            public int Tax2 { get; set; }
            public int Tax3 { get; set; }
        }

        [Serializable]
        private class QuestionFilterInternal : QuestionFilter
        {
            public DateRangeShortcut? DateRangeShortcut { get; set; }

            public DateTimeRange DateRange { get; set; }

            #region Initialization

            static QuestionFilterInternal()
            {
                QuestionFilterSerializer.Register<QuestionFilterInternal>(31, ReadInternal, WriteInternal);
            }

            #endregion

            #region Methods (serialization)

            private static void WriteInternal(QuestionFilter filter, BinaryWriter writer)
            {
                var @internal = (QuestionFilterInternal)filter;

                Write(filter, writer);

                writer.Write(@internal.DateRangeShortcut.HasValue ? (byte)@internal.DateRangeShortcut.Value : byte.MaxValue);
                writer.WriteNullable(@internal.DateRange?.Since);
                writer.WriteNullable(@internal.DateRange?.Before);
            }

            private static QuestionFilter ReadInternal(BinaryReader reader)
            {
                var filter = new QuestionFilterInternal();

                Read(filter, reader);

                var shortcut = reader.ReadByte();
                var since = reader.ReadDateTimeNullable();
                var before = reader.ReadDateTimeNullable();

                filter.DateRangeShortcut = shortcut == byte.MaxValue ? null : (DateRangeShortcut?)shortcut;
                filter.DateRange = since.HasValue || before.HasValue
                    ? new DateTimeRange { Since = since, Before = before }
                    : null;

                return filter;
            }

            #endregion
        }

        #endregion

        #region Constants

        private const long QuestionsChangesDatesPrecision = TimeSpan.TicksPerDay;

        #endregion

        #region Properties

        private Guid BankID => ParseGuid(Request.QueryString["bank"], Guid.Empty).Value;

        private Guid SpecID => ParseGuid(Request.QueryString["spec"], Guid.Empty).Value;

        private Dictionary<Guid, Guid> SetGacMapping
        {
            get => (Dictionary<Guid, Guid>)ViewState[nameof(SetGacMapping)];
            set => ViewState[nameof(SetGacMapping)] = value;
        }

        private FormDataInfo FormData
        {
            get => (FormDataInfo)ViewState[nameof(FormData)];
            set => ViewState[nameof(FormData)] = value;
        }

        private QuestionFilter CurrentQuestionFilter
        {
            get => (QuestionFilter)ViewState[nameof(CurrentQuestionFilter)];
            set => ViewState[nameof(CurrentQuestionFilter)] = value;
        }

        private Tuple<DateTime, Guid[]>[] QuestionsChangesDates
        {
            get
            {
                var result = (Tuple<DateTime, Guid[]>[])ViewState[nameof(QuestionsChangesDates)];

                if (result == null)
                {
                    ViewState[nameof(QuestionsChangesDates)] = result = ServiceLocator.BankSearch
                        .GetQuestions(new QBankQuestionFilter { OrganizationIdentifier = Organization.Identifier, BankIdentifier = BankID })
                        .Where(q => q.LastChangeTime.HasValue)
                        .GroupBy(q =>
                        {
                            var userDate = TimeZoneInfo.ConvertTime(q.LastChangeTime.Value, User.TimeZone).DateTime;
                            return Clock.Trim(userDate, QuestionsChangesDatesPrecision);
                        })
                        .Select(
                            group => new Tuple<DateTime, Guid[]>(
                                group.Key,
                                group
                                    .Select(q => q.QuestionIdentifier)
                                    .Distinct()
                                    .ToArray()
                            )
                        )
                        .ToArray();
                }

                return result;
            }
        }

        #endregion

        #region Fields

        private BankState _bank;
        private ReturnUrl _returnUrl;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriterionTableRepeater.ItemDataBound += CriterionTableRepeater_ItemDataBound;
            CompetencyTableRepeater.ItemDataBound += CompetencyTableRepeater_ItemDataBound;
            BankViewRepeater.ItemDataBound += BankViewRepeater_ItemDataBound;

            SaveButton.Click += SaveButton_Click;

            SetSelector.AutoPostBack = true;
            SetSelector.ValueChanged += SetSelector_ValueChanged;

            CompetencySelector.AutoPostBack = true;
            CompetencySelector.ValueChanged += CompetencySelector_ValueChanged;

            QuestionRepeater.NeedRefresh += QuestionRepeater_NeedRefresh;

            AddQuestionButton1.Click += AddQuestionButton_Click;
            AddQuestionButton2.Click += AddQuestionButton_Click;

            MoveQuestionsButton.Click += MoveQuestionsButton_Click;

            ApplyFilterButton.Click += ApplyFilterButton_Click;
            ClearFilterButton.Click += ClearFilterButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            HandleAjaxRequest();

            base.OnLoad(e);

            if (!CanEdit)
                RedirectToSearch();

            if (IsPostBack)
                return;

            if (Request.QueryString["panel"] == "questions")
                QuestionsTab.IsSelected = true;
            else if (Request.QueryString["panel"] == "comments")
                CommentsTab.IsSelected = true;

            Open();

            var hasSet = SetSelector.Items.Count > 0;

            SetSelector.Enabled = SetSelector.Items.Count > 1;
            AddQuestionButton1.Visible = hasSet;
            AddQuestionButton2.Visible = hasSet;

            Guid? competencyId = null;

            if (hasSet)
            {
                var setId = ParseGuid(Request.QueryString["set"]);
                var questionId = ParseGuid(Request.QueryString["question"]);

                if (questionId.HasValue)
                {
                    var bank = GetBankData();
                    var question = bank.FindQuestion(questionId.Value);

                    if (question != null)
                    {
                        setId = question.Set.Identifier;
                        competencyId = question.Standard;

                        ScriptManager.RegisterStartupScript(
                            Page,
                            GetType(),
                            "scrollto_question",
                            $"$(document).ready(function() {{ workshopQuestionRepeater.scrollToQuestion('{question.Identifier}'); }});",
                            true);
                    }
                }

                if (setId.HasValue)
                {
                    var setItem = SetSelector.FindOptionByValue(setId.Value.ToString(), true);
                    if (setItem != null)
                        setItem.Selected = true;
                }
            }

            OnSetSelected(competencyId);

            var filter = QuestionsTab.IsSelected
                ? QuestionFilterSerializer.Deserialize(Request.QueryString["filter"])
                : null;

            if (filter != null)
            {
                if (competencyId.HasValue)
                    filter.StandardIdentifier = competencyId.Value;

                SetQuestionFilter(filter);
            }

            LoadQuestionRepeater();

            AddQuestionButton2.CopyFrom(AddQuestionButton1);
        }

        #endregion

        #region Event handlers

        private void ApplyFilterButton_Click(object sender, EventArgs e) => LoadQuestionRepeater();

        private void ClearFilterButton_Click(object sender, EventArgs e)
        {
            ClearQuestionFilter();
            LoadQuestionRepeater();
        }

        private void QuestionRepeater_NeedRefresh(object sender, EventArgs e) => LoadQuestionRepeater();

        private void HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest || !bool.TryParse(Page.Request.Form["isPageAjax"], out var isAjax) || !isAjax)
                return;

            var questionId = Guid.Parse(Page.Request.Form["questionId"]);
            var authorType = Page.Request.Form["authorType"];
            var flag = Page.Request.Form["flag"].ToEnum<FlagType>();
            var text = Page.Request.Form["text"];

            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(authorType) && CommentAuthorTypeComboBox.IsValidValue(authorType))
            {
                ServiceLocator.SendCommand(new PostComment(
                    BankID,
                    UniqueIdentifier.Create(),
                    flag,
                    CommentType.Question,
                    questionId,
                    User.UserIdentifier,
                    authorType,
                    null,
                    text,
                    null, null, null,
                    DateTimeOffset.UtcNow));
            }

            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var question = bank.FindQuestion(questionId);

            var returnQuery = "bank&spec&panel=questions";
            if (CurrentQuestionFilter?.IsEmpty == false)
                returnQuery += "&filter=" + QuestionFilterSerializer.Serialize(CurrentQuestionFilter);

            var commentRepeater = (WorkshopQuestionCommentRepeater)LoadControl("~/UI/Admin/Assessments/Questions/Controls/WorkshopQuestionCommentRepeater.ascx");
            commentRepeater.LoadSpecificationData(question, new ReturnUrl(returnQuery));

            var result = new StringBuilder();
            using (var writer = new StringWriter(result))
            {
                var htmlWriter = new HtmlTextWriter(writer);
                commentRepeater.RenderControl(htmlWriter);
            }

            Response.Clear();
            Response.Write(result.ToString());
            Response.End();
        }

        private void CriterionTableRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var standardRepeater = (Repeater)e.Item.FindControl("SetStandardRepeater");
            standardRepeater.DataSource = (IEnumerable<Guid>)DataBinder.Eval(e.Item.DataItem, "SetStandards");
            standardRepeater.DataBind();
        }

        private void CompetencyTableRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Competencies");
            competencyRepeater.DataBind();
        }

        private void BankViewRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!RepeaterHelper.IsContentItem(e))
                return;

            var competencyRepeater = (Repeater)e.Item.FindControl("CompetencyRepeater");
            competencyRepeater.DataSource = DataBinder.Eval(e.Item.DataItem, "Competencies");
            competencyRepeater.DataBind();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!Save())
                return;

            _bank = null;

            var selectedSetId = SetSelector.Value;
            var selectedCompetencyId = CompetencySelector.ValueAsGuid;

            Open();

            var setItem = SetSelector.FindOptionByValue(selectedSetId, true);
            if (setItem != null)
                setItem.Selected = true;

            OnSetSelected(null);

            CompetencySelector.ValueAsGuid = selectedCompetencyId;
            LoadQuestionRepeater();
        }

        private void SetSelector_ValueChanged(object sender, EventArgs e)
        {
            OnSetSelected(null);
            LoadQuestionRepeater();
        }

        private void OnSetSelected(Guid? competency)
        {
            var setId = SetSelector.ValueAsGuid;
            var gacId = setId.HasValue && SetGacMapping.ContainsKey(setId.Value) ? SetGacMapping[setId.Value] : (Guid?)null;

            CompetencySelector.ValueAsGuid = null;
            CompetencySelector.ListFilter.ParentStandardIdentifiers = new[] { gacId ?? Guid.Empty };
            CompetencySelector.Enabled = gacId.HasValue;

            if (gacId.HasValue)
            {
                CompetencySelector.RefreshData();

                if (CompetencySelector.Items.Count > 1)
                {
                    if (competency.HasValue)
                    {
                        CompetencySelector.ValueAsGuid = competency.Value;
                    }
                    else
                    {
                        ((IComboBoxOption)CompetencySelector.Items[1]).Selected = true;

                        var bank = GetBankData();
                        var set = bank.FindSet(setId.Value);

                        foreach (var option in CompetencySelector.FlattenOptions())
                        {
                            if (option.Value.IsEmpty())
                                continue;

                            var competencyId = Guid.Parse(option.Value);

                            if (set.Questions.Any(x => x.Standard == competencyId))
                            {
                                option.Selected = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void CompetencySelector_ValueChanged(object sender, EventArgs e) => LoadQuestionRepeater();

        private void AddQuestionButton_Click(object sender, CommandEventArgs e)
        {
            var setId = SetSelector.ValueAsGuid;
            if (!setId.HasValue)
                return;

            var competencyId = CompetencySelector.ValueAsGuid ?? Guid.Empty;

            if (e.CommandName == "Creator")
            {
                var url = $"/ui/admin/assessments/questions/add?bank={BankID}&set={setId}";

                if (competencyId != Guid.Empty)
                    url += $"&competency={competencyId}";

                var returnQuery = "question&panel=questions";

                if (CurrentQuestionFilter?.IsEmpty == false)
                    returnQuery += "&filter=" + QuestionFilterSerializer.Serialize(CurrentQuestionFilter);

                url = GetReturnUrlInternal().GetRedirectUrl(url, returnQuery);

                HttpResponseHelper.Redirect(url);
            }

            var question = new Question();

            if (e.CommandName == "default" || e.CommandName == "QuickMultipleChoice")
            {
                question.Type = QuestionItemType.SingleCorrect;
                question.CalculationMethod = QuestionCalculationMethod.Default;

                question.Options.Add(new Option());
                question.Options.Add(new Option());
                question.Options.Add(new Option());
                question.Options.Add(new Option());
            }
            else if (e.CommandName == "QuickComposedEssay")
            {
                question.Type = QuestionItemType.ComposedEssay;
                question.CalculationMethod = QuestionCalculationMethod.Default;
            }
            else if (e.CommandName == "QuickComposedVoice")
            {
                question.Type = QuestionItemType.ComposedVoice;
                question.CalculationMethod = QuestionCalculationMethod.Default;
            }
            else if (e.CommandName == "QuickMultipleCorrect")
            {
                question.Type = QuestionItemType.MultipleCorrect;
                question.CalculationMethod = QuestionCalculationMethod.AllOrNothing;
                question.Points = 1;

                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
            }
            else if (e.CommandName == "QuickBooleanTable")
            {
                question.Type = QuestionItemType.BooleanTable;
                question.CalculationMethod = QuestionCalculationMethod.AllOrNothing;
                question.Points = 1;

                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
                question.Options.Add(new Option { IsTrue = false });
            }
            else if (e.CommandName == "QuickMatching")
            {
                question.Type = QuestionItemType.Matching;
                question.CalculationMethod = QuestionCalculationMethod.AllOrNothing;
                question.Points = 1;
            }
            else
                throw new NotImplementedException("Unexpected command name: " + e.CommandName);

            if (competencyId != Guid.Empty)
            {
                question.Standard = competencyId;
                question.SubStandards = null;
            }

            question.Identifier = UniqueIdentifier.Create();
            question.Asset = Sequence.Increment(Organization.OrganizationIdentifier, SequenceType.Asset);
            question.Condition = "Unassigned";

            ServiceLocator.SendCommand(new AddQuestion(BankID, setId.Value, question));

            LoadQuestionRepeater();

            ScriptManager.RegisterStartupScript(
                Page,
                GetType(),
                "scrollto_question",
                $"$(document).ready(function() {{ workshopQuestionRepeater.scrollToQuestion('{question.Identifier}'); }});",
                true);
        }

        private void MoveQuestionsButton_Click(object sender, EventArgs e)
        {
            var setId = SetSelector.ValueAsGuid;
            if (!setId.HasValue || CurrentQuestionFilter?.IsEmpty != false)
                return;

            if (setId == Guid.Empty)
                throw new ApplicationError("Invalid SetIdentifier value");

            if (CurrentQuestionFilter.StandardIdentifier == Guid.Empty)
                throw new ApplicationError("Invalid StandardIdentifier value");

            if (CurrentQuestionFilter.Taxonomy == byte.MaxValue)
                throw new ApplicationError("Invalid Taxonomy value");

            var redirectUrl = GetReturnUrlInternal().GetRedirectUrl(
                $"/ui/admin/assessments/questions/move?bank={BankID}&spec={SpecID}&set={setId}&filter={QuestionFilterSerializer.Serialize(CurrentQuestionFilter)}",
                "filter&panel=questions");

            HttpResponseHelper.Redirect(redirectUrl);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = GetBankData();
            if (bank == null)
                RedirectToSearch();

            var spec = bank.FindSpecification(SpecID);
            if (spec == null)
                RedirectToReader();

            SetInputValues(spec);

            ProblemRepeater.BindModelToControls(spec);
        }

        private bool Save()
        {
            if (FormData.FormLimit != SpecificationFormLimit.ValueAsInt.Value || FormData.QuestionLimit != SpecificationQuestionLimit.ValueAsInt.Value)
                ServiceLocator.SendCommand(new ReconfigureSpecification(BankID, SpecID, ConsequenceType.Low, SpecificationFormLimit.ValueAsInt.Value, SpecificationQuestionLimit.ValueAsInt.Value));

            for (var i = 0; i < CriterionTableRepeater.Items.Count; i++)
            {
                var setWeight = (NumericBox)CriterionTableRepeater.Items[i].FindControl("SetWeight");
                var competencyRepeater = (Repeater)CompetencyTableRepeater.Items[i].FindControl("CompetencyRepeater");

                var initialData = FormData.Criteria[i];
                var requestData = new CriterionInfo { SetWeight = (setWeight.ValueAsDecimal ?? 0) / 100 };
                var hasChanges = initialData.SetWeight != requestData.SetWeight;

                for (var j = 0; j < competencyRepeater.Items.Count; j++)
                {
                    var initialFilterInfo = initialData.Filter[j];
                    if (!initialFilterInfo.Competency.HasValue)
                        continue;

                    var repeaterItem = competencyRepeater.Items[j];
                    var tax1Count = (NumericBox)repeaterItem.FindControl("Tax1Count");
                    var tax2Count = (NumericBox)repeaterItem.FindControl("Tax2Count");
                    var tax3Count = (NumericBox)repeaterItem.FindControl("Tax3Count");

                    var requestFilterInfo = new CriterionFilterInfo
                    {
                        Competency = initialFilterInfo.Competency,
                        Tax1 = tax1Count.ValueAsInt ?? 0,
                        Tax2 = tax2Count.ValueAsInt ?? 0,
                        Tax3 = tax3Count.ValueAsInt ?? 0,
                    };

                    requestData.Filter.Add(requestFilterInfo);

                    if (requestFilterInfo.Tax1 != initialFilterInfo.Tax1 || requestFilterInfo.Tax2 != initialFilterInfo.Tax2 || requestFilterInfo.Tax3 != initialFilterInfo.Tax3)
                        hasChanges = true;
                }

                if (!hasChanges)
                    continue;

                var pivotTable = new PivotTable();

                pivotTable.AddRow("Competency", requestData.Filter.Select(x => x.Competency.Value.ToString()).ToArray());
                pivotTable.AddColumn("Taxonomy", new[] { "1", "2", "3" });

                var tax1Key = new MultiKey<string>("1");
                var tax2Key = new MultiKey<string>("2");
                var tax3Key = new MultiKey<string>("3");

                foreach (var filterRow in requestData.Filter)
                {
                    var rowKey = new MultiKey<string>(filterRow.Competency.Value.ToString());

                    pivotTable.SetCellValue(rowKey, tax1Key, Number.NullIfOutOfRange(filterRow.Tax1, 0));
                    pivotTable.SetCellValue(rowKey, tax2Key, Number.NullIfOutOfRange(filterRow.Tax2, 0));
                    pivotTable.SetCellValue(rowKey, tax3Key, Number.NullIfOutOfRange(filterRow.Tax3, 0));
                }

                ServiceLocator.SendCommand(new ChangeCriterionFilter(BankID, initialData.Identifier, requestData.SetWeight, null, null, pivotTable));
            }

            return true;
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(Specification spec)
        {
            var title = spec.Name;
            if (string.IsNullOrEmpty(title))
                title = spec.Bank.Content.Title?.Default;

            PageHelper.AutoBindHeader(
                this,
                qualifier: title + $" <span class='form-text'>Specification Asset #{spec.Asset}</span>");

            WorkshopScript.LoadData(spec.Bank);

            FormData = new FormDataInfo
            {
                FormLimit = spec.FormLimit,
                QuestionLimit = spec.QuestionLimit
            };

            CloseButton.NavigateUrl = GetReaderUrl(spec.Identifier);

            QuestionDateRangeSelector.LoadItems(
                DateRangeShortcut.Today,
                DateRangeShortcut.Yesterday,
                DateRangeShortcut.ThisWeek,
                DateRangeShortcut.LastWeek,
                DateRangeShortcut.ThisMonth,
                DateRangeShortcut.LastMonth,
                DateRangeShortcut.ThisYear,
                DateRangeShortcut.LastYear);
            QuestionDateRangeSelector.Items.Add(new ComboBoxOption("Custom Dates", "Custom"));

            BankStandard.AssetID = spec.Bank.Standard;
            SpecificationFormLimit.ValueAsInt = spec.FormLimit;
            SpecificationQuestionLimit.ValueAsInt = spec.QuestionLimit;

            var criterionSetWeightTotal = 0M;

            var specGacs = spec.Criteria.SelectMany(x => x.Sets).Select(x => x.Standard).Distinct().Where(x => x != Guid.Empty).ToArray();

            var assets = StandardSearch
                .Bind(x => new SnippetBuilder.StandardModel
                {
                    Label = x.StandardLabel,
                    Type = x.StandardType,
                    Name = x.ContentName,
                    Number = x.AssetNumber,
                    Title = x.ContentTitle,
                    Code = x.Code,
                    ParentCode = x.Parent.Code,

                    Identifier = x.StandardIdentifier,
                    ParentIdentifier = x.Parent.StandardIdentifier,
                    Sequence = x.Sequence,
                    ParentSequence = x.Parent.Sequence,
                }, x => specGacs.Contains(x.Parent.StandardIdentifier));

            var gacCompetencyMapping = assets.GroupBy(x => x.ParentIdentifier).ToDictionary(x => x.Key, x => x.OrderBy(y => y.ParentSequence).ThenBy(y => y.Sequence).ThenBy(y => y.Name).ToArray());
            var competencyMapping = assets.ToDictionary(x => x.Identifier);

            CriterionTableRepeater.DataSource = spec.Criteria.Select(x =>
            {
                var setWeight = Math.Round(x.SetWeight * 100m);

                criterionSetWeightTotal += x.SetWeight;

                return new
                {
                    CriterionID = x.Identifier,
                    CriterionTitle = x.Title,
                    SetWeight = setWeight,
                    SetStandards = x.Sets.Select(y => y.Standard)
                };
            });
            CriterionTableRepeater.DataBind();

            if (criterionSetWeightTotal > 1)
                ScreenStatus.AddMessage(AlertType.Warning, "Please ensure your question set weights sum to 100%.");

            CompetencyTableRepeater.DataSource = spec.Criteria.Select(criterion =>
            {
                Func<int, int, Tuple<MultiKey<string>, MultiKey<string>>> getFilterKeys;

                var hasFilterDimensions = criterion.PivotFilter?.IsEmpty == false;

                if (hasFilterDimensions && criterion.PivotFilter.RowDimensions[0].Name == "Competency" && criterion.PivotFilter.ColumnDimensions[0].Name == "Taxonomy")
                {
                    getFilterKeys = (int c, int t) => new Tuple<MultiKey<string>, MultiKey<string>>(
                        new MultiKey<string>(c.ToString()),
                        new MultiKey<string>(t.ToString()));
                }
                else if (hasFilterDimensions && criterion.PivotFilter.RowDimensions[0].Name == "Taxonomy" && criterion.PivotFilter.ColumnDimensions[0].Name == "Competency")
                {
                    getFilterKeys = (int c, int t) => new Tuple<MultiKey<string>, MultiKey<string>>(
                        new MultiKey<string>(t.ToString()),
                        new MultiKey<string>(c.ToString()));
                }
                else
                {
                    getFilterKeys = (int c, int t) => new Tuple<MultiKey<string>, MultiKey<string>>(null, null);
                }

                var criterionInfo = new CriterionInfo
                {
                    Identifier = criterion.Identifier,
                    SetWeight = criterion.SetWeight
                };
                FormData.Criteria.Add(criterionInfo);

                return new
                {
                    CriterionID = criterion.Identifier,
                    CriterionTitle = criterion.Title,

                    Competencies = criterion.Sets.Where(x => gacCompetencyMapping.ContainsKey(x.Standard)).SelectMany(x => gacCompetencyMapping[x.Standard]).Select(competency =>
                    {
                        var result = new
                        {
                            CompetencyStandardIdentifier = competency.Identifier,
                            CompetencyName = SnippetBuilder.GetHtml(competency),
                            QuestionsCount = (int?)null,
                            Tax1Count = GetPivotTableValue(competency.Number, 1),
                            Tax2Count = GetPivotTableValue(competency.Number, 2),
                            Tax3Count = GetPivotTableValue(competency.Number, 3),
                        };

                        criterionInfo.Filter.Add(new CriterionFilterInfo
                        {
                            Competency = competency.Number,
                            Tax1 = result.Tax1Count ?? 0,
                            Tax2 = result.Tax2Count ?? 0,
                            Tax3 = result.Tax3Count ?? 0,
                        });

                        return result;
                    })
                };

                int? GetPivotTableValue(int? competencyNumber, int taxonomyNumber)
                {
                    if (competencyNumber == null || criterion.PivotFilter == null || criterion.PivotFilter.IsEmpty)
                        return null;

                    var keys = getFilterKeys(competencyNumber.Value, taxonomyNumber);
                    if (!criterion.PivotFilter.RowDimensions.IsValidKey(keys.Item1) || !criterion.PivotFilter.ColumnDimensions.IsValidKey(keys.Item2))
                        return null;

                    var value = criterion.PivotFilter.GetCellValue(keys.Item1, keys.Item2);

                    return value == 0 ? null : value;
                }
            });
            CompetencyTableRepeater.DataBind();

            BankViewRepeater.DataSource = spec.Criteria.Select(criterion =>
            {
                return new
                {
                    CriterionID = criterion.Identifier,
                    CriterionTitle = criterion.Title,
                    Competencies = criterion.Sets
                        .Where(x => gacCompetencyMapping.ContainsKey(x.Standard))
                        .SelectMany(s => gacCompetencyMapping[s.Standard].Select(c => new { Set = s, Competency = c }))
                        .Select(x =>
                        {
                            int total = 0, tax1 = 0, tax2 = 0, tax3 = 0, unassigned = 0;

                            foreach (var q in x.Set.Questions.Where(y => y.Standard == x.Competency.Identifier))
                            {
                                if (q.Condition == "New" || q.Condition == "Edit" || q.Condition == "Copy")
                                {
                                    total++;

                                    if (q.Classification != null)
                                    {
                                        if (q.Classification.Taxonomy == 1)
                                            tax1++;

                                        if (q.Classification.Taxonomy == 2)
                                            tax2++;

                                        if (q.Classification.Taxonomy == 3)
                                            tax3++;
                                    }
                                }
                                else if (q.Condition == "Unassigned")
                                {
                                    unassigned++;
                                }
                            }

                            return new
                            {
                                CompetencyStandardIdentifier = x.Competency.Identifier,
                                CompetencyName = SnippetBuilder.GetHtml(x.Competency),
                                CompetencySequence = x.Competency.Sequence,
                                QuestionsCount = total == 0 ? (int?)null : total,
                                Tax1Count = tax1,
                                Tax2Count = tax2,
                                Tax3Count = tax3,
                                UnassignedCount = unassigned == 0 ? (int?)null : unassigned
                            };
                        }).OrderBy(x => x.CompetencySequence).ThenBy(x => x.CompetencyName)
                };
            });
            BankViewRepeater.DataBind();

            {
                SetGacMapping = new Dictionary<Guid, Guid>();

                SetSelector.Items.Clear();

                foreach (var set in spec.Criteria.SelectMany(x => x.Sets).Distinct().OrderBy(x => x.Sequence))
                {
                    SetSelector.Items.Add(new ComboBoxOption(set.Name, set.Identifier.ToString()));
                    SetGacMapping.Add(set.Identifier, set.Standard);
                }
            }

            {
                var returnUrl = new ReturnUrl("bank&spec&panel=comments");
                var comments = CommentRepeater.LoadData(spec.Bank, new[] { spec.Bank.Identifier, spec.Identifier }, returnUrl);

                CommentsTab.SetTitle("Comments", comments);

                AddCommentLink.NavigateUrl = returnUrl.GetRedirectUrl($"/ui/admin/assessments/comments/author?bank={BankID}&spec={spec.Identifier}");
            }

            {
                var hasData = spec.Bank.Attachments.Count > 0;
                var returnUrl = new ReturnUrl("bank&spec&attachment&panel=attachments");

                FilterAttachmentsTextBox.Visible = hasData;

                AttachmentsNav.Visible = hasData;
                AttachmentsNav.LoadData(spec.Bank.Identifier, spec.Bank.Attachments, returnUrl, true);
                AttachmentsTab.SetTitle("Attachments", spec.Bank.Attachments.Count);

                if (!IsPostBack)
                {
                    var isSelected = false;

                    if (Guid.TryParse(Request.QueryString["attachment"], out var attachmentId))
                    {
                        var attachment = spec.Bank.FindAttachment(attachmentId);
                        if (attachment != null)
                        {
                            if (attachment.Type == AttachmentType.Image)
                                isSelected = AttachmentsNav.SelectTab(TabType.Image);
                            else if (attachment.Type == AttachmentType.Document)
                                isSelected = AttachmentsNav.SelectTab(TabType.Document);
                            else
                                isSelected = AttachmentsNav.SelectTab(TabType.Other);
                        }
                    }

                    if (!isSelected && Request.QueryString["panel"] == "attachments")
                    {
                        isSelected = true;

                        var tab = Request.QueryString["tab"];
                        if (!string.IsNullOrEmpty(tab))
                        {
                            if (tab == "images")
                                AttachmentsNav.SelectTab(TabType.Image);
                            else if (tab == "documents")
                                AttachmentsNav.SelectTab(TabType.Document);
                            else if (tab == "other")
                                AttachmentsNav.SelectTab(TabType.Other);
                        }
                    }

                    if (isSelected)
                        AttachmentsTab.IsSelected = true;
                }
            }
        }

        private void LoadQuestionRepeater()
        {
            var hasData = false;
            var hasQuestions = false;
            var setId = SetSelector.ValueAsGuid;

            if (setId.HasValue)
            {
                var bank = GetBankData();
                var spec = bank.FindSpecification(SpecID);

                CurrentQuestionFilter = GetQuestionFilter();

                var set = spec.Criteria.SelectMany(x => x.Sets).FirstOrDefault(x => x.Identifier == setId);
                var questions = set.Questions.Filter(CurrentQuestionFilter).AsQueryable();

                if (CurrentQuestionFilter is QuestionFilterInternal intFilter)
                {
                    var range = intFilter.DateRangeShortcut.HasValue
                        ? Shift.Common.Calendar.GetDateTimeRange(intFilter.DateRangeShortcut.Value)
                        : new DateTimeRange(intFilter.DateRange?.Since, intFilter.DateRange?.Before);

                    if (!range.IsEmpty)
                    {
                        var query = QuestionsChangesDates.AsQueryable();

                        if (range.Since.HasValue)
                        {
                            var sinceDate = Clock.Trim(range.Since.Value, QuestionsChangesDatesPrecision);
                            query = query.Where(x => x.Item1 >= sinceDate);
                        }

                        if (range.Before.HasValue)
                        {
                            var beforeDate = Clock.Trim(range.Before.Value, QuestionsChangesDatesPrecision);
                            query = query.Where(x => x.Item1 < beforeDate);
                        }

                        var ids = query.SelectMany(x => x.Item2).ToHashSet();

                        questions = questions.Where(x => ids.Contains(x.Identifier));
                    }
                }

                var dataItems = questions.Select(x => new WorkshopQuestionRepeater.DataItem(x)).ToArray();

                hasData = dataItems.Length > 0;
                hasQuestions = set.Questions.Count > 0;

                QuestionCount.Text = dataItems.Length.ToString("n0");

                var returnQuery = "bank&spec&panel=questions";

                if (CurrentQuestionFilter?.IsEmpty == false)
                    returnQuery += "&filter=" + QuestionFilterSerializer.Serialize(CurrentQuestionFilter);

                QuestionRepeater.LoadData(spec, dataItems, new ReturnUrl(returnQuery));
            }
            else
            {
                CurrentQuestionFilter = null;
            }

            QuestionsHeader.Visible = hasData;
            QuestionFilter.Visible = hasQuestions;
            AddQuestionButton2.Visible = hasData;
            MoveQuestionsButton.Visible = CompetencySelector.ValueAsGuid.HasValue && hasData;
        }

        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() =>
            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? spec = null)
        {
            var url = GetReaderUrl(spec);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? spec = null)
        {
            var url = $"/ui/admin/assessments/banks/outline?bank={BankID}";

            if (spec.HasValue)
                url += $"&spec={spec.Value}";

            return url;
        }

        #endregion

        #region Methods (question filter)

        private void SetQuestionFilter(QuestionFilter filter)
        {
            CompetencySelector.ValueAsGuid = filter.StandardIdentifier == Guid.Empty ? null : filter.StandardIdentifier;
            QuestionTaxonomy.ValueAsInt = filter.Taxonomy;
            IsQuestionHasLig.ValueAsBoolean = filter.HasLig;
            IsQuestionHasReference.ValueAsBoolean = filter.HasReference;

            QuestionFlag.EnsureDataBound();
            QuestionFlag.EnumValues = filter.Flag;

            QuestionCondition.EnsureDataBound();
            QuestionCondition.Values = filter.Condition;

            if (filter is QuestionFilterInternal intFilter)
            {
                QuestionDateRangeSince.Value = null;
                QuestionDateRangeBefore.Value = null;

                if (intFilter.DateRangeShortcut.HasValue)
                {
                    QuestionDateRangeSelector.Value = intFilter.DateRangeShortcut.Value.GetName();
                }
                else if (intFilter.DateRange?.IsEmpty == false)
                {
                    QuestionDateRangeSelector.Value = "Custom";
                    QuestionDateRangeSince.Value = intFilter.DateRange.Since;
                    QuestionDateRangeBefore.Value = intFilter.DateRange.Before;
                }
            }
        }

        private QuestionFilter GetQuestionFilter()
        {
            var flag = QuestionFlag.EnumValues.ToHashSet();
            var condition = QuestionCondition.Values.ToHashSet();

            var result = new QuestionFilterInternal
            {
                StandardIdentifier = CompetencySelector.ValueAsGuid ?? Guid.Empty,
                Flag = flag.Count > 0 ? flag : null,
                Condition = condition.Count > 0 ? condition : null,
                Taxonomy = QuestionTaxonomy.ValueAsInt,
                HasLig = IsQuestionHasLig.ValueAsBoolean,
                HasReference = IsQuestionHasReference.ValueAsBoolean,
            };

            var questionDateRange = QuestionDateRangeSelector.Value;
            if (questionDateRange.IsNotEmpty())
            {
                if (questionDateRange == "Custom")
                    result.DateRange = new DateTimeRange
                    {
                        Since = QuestionDateRangeSince.Value,
                        Before = QuestionDateRangeBefore.Value,
                    };
                else
                    result.DateRangeShortcut = questionDateRange.ToEnum<DateRangeShortcut>();
            }

            return result;
        }

        private void ClearQuestionFilter()
        {
            QuestionFlag.ClearSelection();
            QuestionCondition.ClearSelection();
            QuestionTaxonomy.ClearSelection();
            IsQuestionHasLig.ClearSelection();
            IsQuestionHasReference.ClearSelection();
            QuestionDateRangeSelector.ClearSelection();
            QuestionDateRangeSince.Value = null;
            QuestionDateRangeBefore.Value = null;
        }

        #endregion

        #region Methods (helper)

        private BankState GetBankData() => _bank ?? (_bank = ServiceLocator.BankSearch.GetBankState(BankID));

        private Guid? ParseGuid(string value, Guid? defaultValue = null) =>
            Guid.TryParse(value, out var result) && result != Guid.Empty ? result : defaultValue;

        private ReturnUrl GetReturnUrlInternal() => _returnUrl ?? (_returnUrl = new ReturnUrl("bank&spec"));

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