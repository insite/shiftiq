using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Standards.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Admin.Standards.Standards.Controls;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;

using CsvColumn = Shift.Common.CsvMapper.Column;
using CsvReader = Shift.Common.CsvMapper.Reader;

namespace InSite.Admin.Standards.Standards.Forms
{
    public partial class Upload : AdminBasePage
    {
        #region Constants

        private const int MaxErrorsCount = 25;
        private static readonly string CsvFieldsSessionKey = typeof(Upload).FullName + "." + nameof(CsvFields);

        #endregion

        #region Classes

        [Serializable]
        private class CsvColumnGroups
        {
            public CsvColumn[] Standard { get; set; }
            public CsvColumn[] CompetencyScore { get; set; }
            public CsvColumn[] Settings { get; set; }
            public CsvColumn[] ContentEn { get; set; }
            public CsvColumn[] ContentAlt { get; set; }
        }

        [Serializable]
        public class AssetNumberCsvColumn : CsvColumn
        {
            public AssetNumberCsvColumn(string name, string label, bool required)
                : base(name, label, required)
            {
            }

            public override void Read(CsvReader data)
            {
                var assetNumbers = new Dictionary<int, int>();

                while (data.NextRow())
                {
                    var value = data.GetValue().NullIfEmpty();

                    if (value == null)
                        continue;

                    if (!int.TryParse(value, out var number))
                    {
                        data.Messages.AddError("Row {0}: the value of column '{0}' ({1}) is not recognized as a valid asset number", data.RowIndex + 1, Label, data.ColIndex + 1);
                        continue;
                    }

                    if (assetNumbers.TryGetValue(number, out var count))
                        assetNumbers[number] = count + 1;
                    else
                        assetNumbers.Add(number, 1);

                    data.SetValue(number);
                }

                var duplicates = assetNumbers.Where(x => x.Value > 1).OrderByDescending(x => x.Value).ToArray();
                if (duplicates.Length > 0)
                {
                    var message = new StringBuilder();
                    message.Append("<p>Duplicate standard found:</p><ul>");

                    foreach (var kv in duplicates)
                        message.AppendFormat("<li>{0} <small>({1:n0} standards)</small></li>", kv.Key, kv.Value);

                    message.Append("</ul>");

                    data.Messages.AddImportantWarning(message.ToString());
                }
            }
        }

        [Serializable]
        public class ParentAssetNumberCsvColumn : CsvColumn
        {
            public ParentAssetNumberCsvColumn(string name, string label, bool required)
                : base(name, label, required)
            {
            }

            public override void Read(CsvReader data)
            {
                var assetNumbers = StandardSearch
                    .Bind(x => x.AssetNumber, x => x.OrganizationIdentifier == Organization.Identifier)
                    .ToHashSet();

                while (data.NextRow())
                {
                    var value = data.GetValue().NullIfEmpty();

                    if (value == null)
                        continue;

                    if (!int.TryParse(value, out var number))
                    {
                        data.Messages.AddError("Row {0}: the value of column '{1}' ({2}) is not recognized as a valid asset number", data.RowIndex + 1, Label, data.ColIndex + 1);
                        continue;
                    }

                    if (!assetNumbers.Contains(number))
                    {
                        data.Messages.AddImportantWarning("Row {0}: there is not exist Standard with asset number <strong>{1}</strong> to use it as Parent", data.RowIndex + 1, number);
                        continue;
                    }

                    data.SetValue(number);
                }
            }
        }

        [Serializable]
        private class CsvFieldsContainer
        {
            public Guid OrganizationId { get; }
            public CsvColumn[] Flattened { get; private set; }
            public CsvColumnGroups Groups { get; private set; }

            private CsvFieldsContainer(Guid organizationId)
            {
                OrganizationId = organizationId;
            }

            public static CsvFieldsContainer Create(Guid organizationId)
            {
                var result = new CsvFieldsContainer(organizationId)
                {
                    Groups = new CsvColumnGroups
                    {
                        Standard = CreateStandardColumns(organizationId),
                        CompetencyScore = CreateCompetencyScoreColumns(),
                        Settings = CreateSettingsColumns(),
                        ContentEn = CreateContentEnColumns(),
                        ContentAlt = CreateContentAltColumns()
                    }
                };

                result.Flattened = result.Groups.Standard
                    .Concat(result.Groups.CompetencyScore)
                    .Concat(result.Groups.Settings)
                    .Concat(result.Groups.ContentEn)
                    .Concat(result.Groups.ContentAlt)
                    .ToArray();

                return result;
            }

            private static CsvColumn[] CreateStandardColumns(Guid organizationId)
            {
                return new CsvColumn[]
                {
                    new AssetNumberCsvColumn("AssetNumber", "Asset Number", false),
                    new CsvColumn.StringEnum("Type", "Type", true, false, StandardSearch.GetAllTypeNames(organizationId).Select(x => x)),
                    new CsvColumn.String("Tier", "Tier", false, true, 30),
                    new CsvColumn.String("Label", "Tag", false, true, 30),
                    new CsvColumn.String("Name", "Name", false, true, 512),
                    new CsvColumn.String("Code", "Code", false, true, 40),
                    new ParentAssetNumberCsvColumn("Parent", "Parent", false),
                    new CsvColumn.String("SourceReference", "Source/Reference", false, true),
                    new CsvColumn.String("AuthorName", "Author Name", false, true, 100),
                    new CsvColumn.DateTimeOffset("DateAuthored", "Date Authored", false, true),
                    new CsvColumn.DateTimeOffset("DatePosted", "Date Posted", false, true),
                    new CsvColumn.Boolean("ConfigurationPractical", "Configuration Practical", false, false),
                    new CsvColumn.Boolean("ConfigurationTheory", "Configuration Theory", false, false)
                };
            }

            private static CsvColumn[] CreateCompetencyScoreColumns()
            {
                return new CsvColumn[]
                {
                    new CsvColumn.StringEnum("CompetencyScoreSummarizationMethod", "Summarization Method", false, true, new[] { "sum_tier1", "sum_tier0" }),
                    new CsvColumn.StringEnum("CompetencyScoreCalculationMethod", "Calculation Method", false, true, new[] { "DecayingAverage", "NumberOfTimes", "MostRecentScore", "HighestScore" }),
                    new CsvColumn.Integer("CalculationArgument", "Calculation Argument", false, true),
                    new CsvColumn.Decimal("MaxPoints", "Max Points", false, true, 0, 999.9m),
                    new CsvColumn.Decimal("MasteryPoints", "Mastery Points", false, true, 0, 999.9m)
                };
            }

            private static CsvColumn[] CreateSettingsColumns()
            {
                return new CsvColumn[]
                {
                    new CsvColumn.Integer("Sequence", "Sequence", false, false, 0),
                    new CsvColumn.String("LevelType", "Level Type", false, true, 32),
                    new CsvColumn.String("LevelCode", "Level Code", false, true, 1),
                    new CsvColumn.String("VersionMajor", "Version Major", false, true, 8),
                    new CsvColumn.String("VersionMinor", "Version Minor", false, true, 8),
                    new CsvColumn.String("Requirements", "Requirements", false, true),
                    new CsvColumn.String("CreditIdentifier", "Credit Identifier", false, true, 32),
                    new CsvColumn.Decimal("CreditHours", "Credit Hours", false, true, 0, 999.9m),
                    new CsvColumn.String("Tags", "Tags", false, true, 128),
                };
            }

            private static CsvColumn[] CreateContentEnColumns()
            {
                return new CsvColumn[]
                {
                    new CsvColumn.String("ENTitle", "EN Title", false, true),
                    new CsvColumn.String("ENSummary", "EN Summary", false, true),
                    new CsvColumn.String("ENBody", "EN Body", false, true),
                    new CsvColumn.String("ENStatements", "EN Statements", false, true)
                };
            }

            private static CsvColumn[] CreateContentAltColumns()
            {
                return new CsvColumn[]
                {
                    new CsvColumn.LanguageCode("Language", "Language", false),
                    new CsvColumn.String("ALTTitle", "ALT Title", false, true),
                    new CsvColumn.String("ALTSummary", "ALT Summary", false, true),
                    new CsvColumn.String("ALTBody", "ALT Body", false, true),
                    new CsvColumn.String("ALTStatements", "ALT Statements", false, true)
                };
            }
        }

        #endregion

        #region Properties

        private string[][] Values
        {
            get => (string[][])ViewState[nameof(Values)];
            set => ViewState[nameof(Values)] = value;
        }

        protected string FirstRecordsTitle { get; set; }

        private Dictionary<string, List<Tuple<AlertType, string, string>>> ProgressStatus
        {
            get
            {
                var key = typeof(Upload) + nameof(ProgressStatus);

                return (Dictionary<string, List<Tuple<AlertType, string, string>>>)(Session[key]
                    ?? (Session[key] = new Dictionary<string, List<Tuple<AlertType, string, string>>>()));
            }
        }

        private CsvFieldsContainer CsvFields => (CsvFieldsContainer)Session[CsvFieldsSessionKey];

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnsureCsvFieldsInited();

            ImportFile.FileUploaded += ImportFile_FileUploaded;

            UploadNextButton.Click += UploadNextButton_Click;
            FieldsNextButton.Click += FieldsNextButton_Click;
            ValidationNextButton.Click += ValidationNextButton_Click;
            ValidationBackButton.Click += ValidationBackButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (!Identity.IsGranted(Route.ToolkitNumber, PermissionOperation.Write))
                HttpResponseHelper.Redirect("/ui/admin/standards/home");

            PageHelper.AutoBindHeader(this, null, "Upload Standards");

            ShowCancelMessage();

            FieldsSection.Visible = false;
            ValidationSection.Visible = false;
            CompleteSection.Visible = false;
        }

        private void ShowCancelMessage()
        {
            var pContextId = Page.Request.QueryString["progressContext"];
            if (pContextId.IsEmpty() || !ProgressStatus.ContainsKey(pContextId))
                return;

            var status = ProgressStatus[pContextId];

            foreach (var info in status)
                StatusTop.AddMessage(info.Item1, info.Item2);

            ProgressStatus.Remove(pContextId);
        }

        private void EnsureCsvFieldsInited()
        {
            var obj = (CsvFieldsContainer)Session[CsvFieldsSessionKey];
            if (obj == null || obj.OrganizationId != Organization.Identifier)
                Session[CsvFieldsSessionKey] = CsvFieldsContainer.Create(Organization.Identifier);
        }

        #endregion

        #region Event handlers

        private void ImportFile_FileUploaded(object sender, EventArgs e)
        {
            ValidationSection.Visible = false;
            CompleteSection.Visible = false;

            FieldsSection.Visible = false;
            NavPanel.SelectedIndex = 0;

            UploadNextButton.Visible = LoadFile(ImportFile.OpenFile());
        }

        private void UploadNextButton_Click(object sender, EventArgs e)
        {
            ValidationSection.Visible = false;
            CompleteSection.Visible = false;

            FieldsSection.Visible = true;
            NavPanel.SelectedIndex = 1;
        }

        private void FieldsNextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var columns = GetReaderColumns();
            var data = CsvReader.Read(Values, CsvFields.Flattened.Select(x => x.Name), columns);

            if (!data.Messages.IsEmpty)
            {
                CompleteSection.Visible = false;
                ValidationSection.Visible = true;
                NavPanel.SelectedIndex = 2;
                ValidationNextButton.Visible = !data.Messages.HasImportantError && !data.Messages.HasError;

                if (data.Messages.HasWarning)
                {
                    var message = new StringBuilder();

                    message.AppendFormat("<p>");

                    WriteValidationHeader(message, "warning", data.Messages.WarningCount);

                    message.Append(":</p>");

                    data.Messages.WriteWarnings(message, MaxErrorsCount);

                    ValidationStatus.AddMessage(AlertType.Warning, message.ToString());
                }

                if (data.Messages.HasImportantWarning)
                    foreach (var message in data.Messages.ImportantWarnings)
                        ValidationStatus.AddMessage(AlertType.Warning, message);

                if (data.Messages.HasError)
                {
                    var message = new StringBuilder();

                    message.AppendFormat("<p>");

                    WriteValidationHeader(message, "error", data.Messages.ErrorCount);

                    message.Append(":</p>");

                    data.Messages.WriteErrors(message, MaxErrorsCount);

                    ValidationStatus.AddMessage(AlertType.Error, message.ToString());
                }

                if (data.Messages.HasImportantError)
                    foreach (var message in data.Messages.ImportantErrors)
                        ValidationStatus.AddMessage(AlertType.Error, message);

                return;
            }

            ValidationSection.Visible = false;
            CompleteSection.Visible = true;
            NavPanel.SelectedIndex = 3;

            ImportData(data);

            void WriteValidationHeader(StringBuilder message, string entityName, int count)
            {
                message.Append(entityName.ToQuantity(count, "N0"))
                    .Append(" ")
                    .Append(count > 1 ? "have" : "has")
                    .Append(" been found");

                if (count > MaxErrorsCount)
                    message.Append(". Here are first ").Append(entityName.ToQuantity(MaxErrorsCount, "N0"));
            }
        }

        private void ValidationNextButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            ValidationSection.Visible = false;
            CompleteSection.Visible = true;
            NavPanel.SelectedIndex = 3;

            var columns = GetReaderColumns();
            var data = CsvReader.Read(Values, CsvFields.Flattened.Select(x => x.Name), columns);

            ImportData(data);
        }

        private void ValidationBackButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            NavPanel.SelectedIndex = 1;

            ValidationSection.Visible = false;
            CompleteSection.Visible = false;
        }

        #endregion

        #region Methods (load file)

        private bool LoadFile(Stream file)
        {
            var encoding = Encoding.GetEncoding(FileEncoding.Value);

            try
            {
                Values = CsvImportHelper.GetValues(file, null, false, encoding);
            }
            catch (ApplicationError ex)
            {
                StatusTop.Indicator = AlertType.Error;
                StatusTop.Text = ex.Message;

                return false;
            }

            if (!ValidateFileErrors())
                return false;

            var selectorItems = Values[0].Select((x, i) => new Shift.Common.ListItem
            {
                Text = x,
                Value = i.ToString()
            }).ToArray();
            var selectorValues = selectorItems.ToDictionary(
                x => StringHelper.RemoveNonAlphanumericCharacters(x.Text),
                x => x.Value,
                StringComparer.OrdinalIgnoreCase);

            FieldsStandard.LoadData(GetFields(CsvFields.Groups.Standard));
            FieldsCompetencyScore.LoadData(GetFields(CsvFields.Groups.CompetencyScore));
            FieldsSettings.LoadData(GetFields(CsvFields.Groups.Settings));
            FieldsContentEn.LoadData(GetFields(CsvFields.Groups.ContentEn));
            FieldsContentAlt.LoadData(GetFields(CsvFields.Groups.ContentAlt));

            return true;

            IEnumerable<UploadFieldRepeater.FieldInfo> GetFields(IEnumerable<CsvColumn> columns)
            {
                return columns.Select(x => new UploadFieldRepeater.FieldInfo
                {
                    Name = x.Name,
                    Required = x.Required,
                    Title = x.Label,
                    Value = selectorValues.TryGetValue(x.Name, out var value) ? value : null,
                    DataSource = selectorItems
                });
            }
        }

        private bool ValidateFileErrors()
        {
            if (Values.Length < 2)
            {
                StatusTop.Indicator = AlertType.Error;
                StatusTop.Text = "The file has no data to import.";
                return false;
            }

            var firstRow = Values[0];
            var colNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var errorMessage = new MessageBuilder();

            for (var colIndex = 0; colIndex < firstRow.Length; colIndex++)
            {
                var colName = StringHelper.RemoveNonAlphanumericCharacters(firstRow[colIndex]);

                if (colName.IsEmpty())
                    errorMessage.AddError("Row {0} is missing the name of column {1}", 1, colIndex + 1);
                else if (!colNames.Add(colName))
                    errorMessage.AddError("Row {0} contains a duplicate value in column {1} ({2})", 1, colIndex + 1, firstRow[colIndex]);
            }

            var columnsCount = firstRow.Length;

            for (var rowIndex = 1; rowIndex < Values.Length; rowIndex++)
            {
                var columns = Values[rowIndex];
                if (columns.Length != columnsCount)
                    errorMessage.AddError("Row {0} has invalid columns count", rowIndex + 1);
            }

            if (!errorMessage.HasError)
                return true;

            var message = new StringBuilder();

            message.AppendFormat("<p>No standards have been uploaded because the file contains {0:n0} error(s)", errorMessage.ErrorCount);

            if (errorMessage.ErrorCount > MaxErrorsCount)
                message.AppendFormat(". Here are first {0:n0} errors", MaxErrorsCount);

            message.Append(":</p>");

            errorMessage.WriteErrors(message, MaxErrorsCount);

            StatusTop.Indicator = AlertType.Error;
            StatusTop.Text = message.ToString();

            return false;
        }

        #endregion

        #region Methods (import data)

        private CsvReader.ColumnData[] GetReaderColumns()
        {
            var result = new List<CsvReader.ColumnData>();

            result.AddRange(GetReaders(CsvFields.Groups.Standard, FieldsStandard));
            result.AddRange(GetReaders(CsvFields.Groups.CompetencyScore, FieldsCompetencyScore));
            result.AddRange(GetReaders(CsvFields.Groups.Settings, FieldsSettings));
            result.AddRange(GetReaders(CsvFields.Groups.ContentEn, FieldsContentEn));
            result.AddRange(GetReaders(CsvFields.Groups.ContentAlt, FieldsContentAlt));

            return result.ToArray();

            IEnumerable<CsvReader.ColumnData> GetReaders(IEnumerable<CsvColumn> columns, UploadFieldRepeater repeater)
            {
                foreach (var c in columns)
                {
                    if (int.TryParse(repeater.GetSelectorValue(c.Name), out var index) && index >= 0)
                        yield return new CsvReader.ColumnData(c, index);
                }
            }
        }

        private bool ImportData(CsvMapper.ICsvReader data)
        {
            try
            {
                var rowCount = data.RowCount;

                SetProgressStatus(0, rowCount);

                var entitiesByNumberRowIndex = -1;
                var entitiesByNumber = StandardSearch.Bind(x => x.AssetNumber, x => x.OrganizationIdentifier == Organization.Identifier).ToDictionary(x => x, x => (QStandard)null);
                var uniqueIds = new HashSet<Guid>();
                var isFirstRecordsFull = false;
                var isLastRecordsFull = false;
                var firstRecords = new List<QStandard>(5);
                var lastRecords = new LinkedList<QStandard>();
                var rowIndex = 0;

                for (; rowIndex < rowCount; rowIndex++)
                {
                    if (!Page.Response.IsClientConnected)
                        break;

                    var entity = FindStandardByNumber(data.GetValue<int?>(rowIndex, "AssetNumber"));
                    var isNew = entity == null;
                    var standardType = data.GetValue<string>(rowIndex, "Type");

                    if (isNew)
                        entity = StandardFactory.Create(standardType);
                    else
                        entity.StandardType = standardType;

                    entity.ParentStandardIdentifier = FindStandardByNumber(data.GetValue<int?>(rowIndex, "Parent"))?.StandardIdentifier;

                    MapStandardData(data, rowIndex, entity);

                    if (isNew)
                    {
                        entity.StandardIdentifier = UniqueIdentifier.Create();
                        entity.OrganizationIdentifier = Organization.Identifier;

                        StandardStore.Insert(entity);
                    }
                    else
                        StandardStore.Update(entity);

                    if (uniqueIds.Add(entity.StandardIdentifier))
                    {
                        if (!isFirstRecordsFull)
                        {
                            firstRecords.Add(entity);
                            isFirstRecordsFull = firstRecords.Count == 5;
                        }
                        else if (isLastRecordsFull)
                        {
                            lastRecords.RemoveFirst();
                            lastRecords.AddLast(entity);
                        }
                        else
                        {
                            lastRecords.AddLast(entity);
                            isLastRecordsFull = lastRecords.Count == 7;
                        }
                    }

                    SaveContentData(data, rowIndex, entity);

                    SetProgressStatus(rowIndex, rowCount);

                    if (!Page.Response.IsClientConnected)
                        break;
                }

                if (uniqueIds.Count > 0)
                {
                    var message = $"{"standard".ToQuantity(uniqueIds.Count, "N0")} successfully uploaded.";

                    StatusComplete.AddMessage(AlertType.Success, message);
                }
                else
                {
                    StatusComplete.AddMessage(AlertType.Warning, "No standards uploaded to the database.");
                }

                LoadPreviewData(firstRecords.ToArray(), lastRecords.ToArray());

                if (!Page.Response.IsClientConnected)
                {
                    var status = ProgressStatus.ContainsKey(SaveProgress.ContextID)
                        ? ProgressStatus[SaveProgress.ContextID]
                        : null;

                    if (status == null)
                        ProgressStatus.Add(SaveProgress.ContextID, status = new List<Tuple<AlertType, string, string>>());

                    foreach (var message in StatusComplete.GetMessages())
                        status.Add(message);

                    status.Add(new Tuple<AlertType, string, string>(AlertType.Error, null, "Cancelled by user."));
                }

                return true;

                QStandard FindStandardByNumber(int? number)
                {
                    const int bunchSize = 500;

                    if (entitiesByNumberRowIndex < rowIndex)
                    {
                        var index = entitiesByNumberRowIndex + 1;

                        entitiesByNumberRowIndex += bunchSize;
                        if (entitiesByNumberRowIndex >= rowCount)
                            entitiesByNumberRowIndex = rowCount - 1;

                        var numbers = new HashSet<int>();

                        for (; index <= entitiesByNumberRowIndex; index++)
                        {
                            var assetNumber = data.GetValue<int?>(index, "AssetNumber");
                            if (assetNumber.HasValue && entitiesByNumber.ContainsKey(assetNumber.Value))
                                numbers.Add(assetNumber.Value);

                            var parentAssetNumber = data.GetValue<int?>(index, "Parent");
                            if (parentAssetNumber.HasValue && entitiesByNumber.ContainsKey(assetNumber.Value))
                                numbers.Add(parentAssetNumber.Value);
                        }

                        if (numbers.Count > 0)
                        {
                            var entities = ServiceLocator.StandardSearch.GetStandards(new QStandardFilter
                            {
                                OrganizationIdentifier = Organization.Identifier,
                                AssetNumbers = numbers.ToArray()
                            });
                            for (var i = 0; i < entities.Count; i++)
                            {
                                var entity = entities[i];
                                entitiesByNumber[entity.AssetNumber] = entity;
                            }
                        }
                    }

                    return number.HasValue && entitiesByNumber.TryGetValue(number.Value, out var result) ? result : null;
                }
            }
            finally
            {
                SaveProgress.RemoveContext();
            }
        }

        private void MapStandardData(CsvMapper.ICsvReader data, int rowIndex, QStandard entity)
        {
            entity.StandardTier = data.GetValue<string>(rowIndex, "Tier");
            entity.StandardLabel = data.GetValue<string>(rowIndex, "Label");
            entity.ContentTitle = data.GetValue<string>(rowIndex, "ENTitle");
            entity.ContentName = data.GetValue<string>(rowIndex, "Name");
            entity.Code = data.GetValue<string>(rowIndex, "Code");
            entity.SourceDescriptor = data.GetValue<string>(rowIndex, "SourceReference");
            entity.AuthorName = data.GetValue<string>(rowIndex, "AuthorName");
            entity.AuthorDate = data.GetValue<DateTimeOffset?>(rowIndex, "DateAuthored");
            entity.DatePosted = data.GetValue<DateTimeOffset?>(rowIndex, "DatePosted");
            entity.IsPractical = data.GetValue<bool?>(rowIndex, "ConfigurationPractical") ?? false;
            entity.IsTheory = data.GetValue<bool?>(rowIndex, "ConfigurationTheory") ?? false;
            entity.CompetencyScoreSummarizationMethod = data.GetValue<string>(rowIndex, "CompetencyScoreSummarizationMethod");
            entity.CompetencyScoreCalculationMethod = data.GetValue<string>(rowIndex, "CompetencyScoreCalculationMethod");
            entity.CalculationArgument = data.GetValue<int?>(rowIndex, "CalculationArgument");
            entity.PointsPossible = data.GetValue<decimal?>(rowIndex, "MaxPoints");
            entity.MasteryPoints = data.GetValue<decimal?>(rowIndex, "MasteryPoints");
            entity.Sequence = data.GetValue<int?>(rowIndex, "Sequence") ?? 0;
            entity.LevelType = data.GetValue<string>(rowIndex, "LevelType");
            entity.LevelCode = data.GetValue<string>(rowIndex, "LevelCode");
            entity.MajorVersion = data.GetValue<string>(rowIndex, "VersionMajor");
            entity.MinorVersion = data.GetValue<string>(rowIndex, "VersionMinor");
            entity.ContentDescription = data.GetValue<string>(rowIndex, "Requirements");
            entity.CreditIdentifier = data.GetValue<string>(rowIndex, "CreditIdentifier");
            entity.CreditHours = data.GetValue<decimal?>(rowIndex, "CreditHours");
            entity.Tags = data.GetValue<string>(rowIndex, "Tags");
        }

        private void SaveContentData(CsvMapper.ICsvReader data, int rowIndex, QStandard entity)
        {
            var enTitle = data.GetValue<string>(rowIndex, "ENTitle");
            var enSummary = data.GetValue<string>(rowIndex, "ENSummary");
            var enBody = data.GetValue<string>(rowIndex, "ENBody");
            var enStatements = data.GetValue<string>(rowIndex, "ENStatements");

            var altLanguage = data.GetValue<string>(rowIndex, "Language").IfNullOrEmpty("fr");
            var altTitle = data.GetValue<string>(rowIndex, "ALTTitle");
            var altSummary = data.GetValue<string>(rowIndex, "ALTSummary");
            var altBody = data.GetValue<string>(rowIndex, "ALTBody");
            var altStatements = data.GetValue<string>(rowIndex, "ALTStatements");

            if (enTitle.IsNotEmpty())
                ServiceLocator.ContentStore.Save("Standard", entity.StandardIdentifier, "Title", enTitle, Shift.Common.ContentContainer.DefaultLanguage, entity.OrganizationIdentifier);

            if (enSummary.IsNotEmpty())
                ServiceLocator.ContentStore.Save("Standard", entity.StandardIdentifier, "Summary", enSummary, Shift.Common.ContentContainer.DefaultLanguage, entity.OrganizationIdentifier);

            if (enBody.IsNotEmpty())
                ServiceLocator.ContentStore.Save("Standard", entity.StandardIdentifier, "Body", enBody, Shift.Common.ContentContainer.DefaultLanguage, entity.OrganizationIdentifier);

            if (enStatements.IsNotEmpty())
                ServiceLocator.ContentStore.Save("Standard", entity.StandardIdentifier, "Statements", enStatements, Shift.Common.ContentContainer.DefaultLanguage, entity.OrganizationIdentifier);

            if (altTitle.IsNotEmpty())
                ServiceLocator.ContentStore.Save("Standard", entity.StandardIdentifier, "Title", altTitle, altLanguage, entity.OrganizationIdentifier);

            if (altSummary.IsNotEmpty())
                ServiceLocator.ContentStore.Save("Standard", entity.StandardIdentifier, "Summary", altSummary, altLanguage, entity.OrganizationIdentifier);

            if (altBody.IsNotEmpty())
                ServiceLocator.ContentStore.Save("Standard", entity.StandardIdentifier, "Body", altBody, altLanguage, entity.OrganizationIdentifier);

            if (altStatements.IsNotEmpty())
                ServiceLocator.ContentStore.Save("Standard", entity.StandardIdentifier, "Statements", altStatements, altLanguage, entity.OrganizationIdentifier);
        }

        private void LoadPreviewData(QStandard[] firstRecords, QStandard[] lastRecords)
        {
            if (firstRecords.Length == 0)
                return;

            FirstRecordsRepeater.Visible = true;

            if (firstRecords.Length + lastRecords.Length <= 10)
            {
                FirstRecordsTitle = string.Empty;

                FirstRecordsRepeater.DataSource = firstRecords.Concat(lastRecords);
                FirstRecordsRepeater.DataBind();

                LastRecordsRepeater.Visible = false;

                return;
            }

            FirstRecordsTitle = "First 5 standards:";

            FirstRecordsRepeater.DataSource = firstRecords;
            FirstRecordsRepeater.DataBind();

            LastRecordsRepeater.Visible = true;

            LastRecordsRepeater.DataSource = lastRecords.Skip(1).Take(5);
            LastRecordsRepeater.DataBind();
        }

        private void SetProgressStatus(int currentPosition, int positionMax)
        {
            SaveProgress.UpdateContext(context =>
            {
                var progressBar = (ProgressIndicator.ContextData)context.Items["Progress"];
                progressBar.Total = positionMax;
                progressBar.Value = currentPosition;
            });
        }

        #endregion
    }
}
