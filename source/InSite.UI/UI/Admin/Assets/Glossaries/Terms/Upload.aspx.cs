using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Glossaries.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assets.Glossaries.Terms.Forms
{
    public partial class Upload : AdminBasePage
    {
        #region Constants

        private const string Col_Name = "Name";
        private const string Col_Title = "Title";
        private const string Col_Definition = "Definition";

        #endregion

        #region Classes

        [Serializable]
        private class TermInfo
        {
            public string Name { get; set; }
            public Shift.Common.ContentContainer Content { get; } = new Shift.Common.ContentContainer();
        }

        private class TermMappingInfo
        {
            public int? NameIndex { get; set; }
            public List<TermLangMappingInfo> Languages { get; } = new List<TermLangMappingInfo>();
        }

        private class TermLangMappingInfo
        {
            public Language.LanguageInfo Language { get; }
            public List<TermContentMappingInfo> Content { get; } = new List<TermContentMappingInfo>();

            public TermLangMappingInfo(Language.LanguageInfo lang)
            {
                Language = lang;
            }
        }

        private class TermContentMappingInfo
        {
            public int Index { get; }
            public string Title { get; }
            public string Label { get; }

            public TermContentMappingInfo(int index, string title)
                : this(index, title, title)
            {
            }

            public TermContentMappingInfo(int index, string title, string label)
            {
                Index = index;
                Title = title;
                Label = label;
            }
        }

        #endregion

        #region Properties

        private TermInfo[] ImportData
        {
            get => (TermInfo[])ViewState[nameof(ImportData)];
            set => ViewState[nameof(ImportData)] = value;
        }

        #endregion

        #region Initialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ImportFile.FileUploaded += ImportFile_FileUploaded;
            NextButton.Click += NextButton_Click;
            CancelButton2.Click += CancelButton2_Click;
            ImportButton.Click += ImportButton_Click;

            ImportDataRepeater.ItemDataBound += ImportDataRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);
        }

        #endregion

        #region Event handlers

        private void ImportFile_FileUploaded(object sender, EventArgs e)
        {
            bool success;

            using (var stream = ImportFile.OpenFile())
                success = LoadFile(stream);

            ImportSection.Visible = false;
            NextButton.Visible = success;

            if (!success)
                return;

            ScreenStatus.AddMessage(AlertType.Success, "File was successfully parsed, click <b>Next</b> to continue.");
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            FileSection.Visible = false;
            ImportSection.Visible = true;

            ImportDataRepeater.DataSource = ImportData;
            ImportDataRepeater.DataBind();
        }

        private void CancelButton2_Click(object sender, EventArgs e)
        {
            FileSection.Visible = true;
            ImportSection.Visible = false;
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            foreach (var item in ImportData)
            {
                var term = ServiceLocator.GlossarySearch.GetTerm(GlossaryHelper.GlossaryIdentifier, item.Name);
                if (term != null)
                {
                    var content = ServiceLocator.ContentSearch.GetBlock(term.TermIdentifier);

                    content.Set(item.Content, ContentContainer.SetNullAction.None);

                    ServiceLocator.SendCommand(new ReviseGlossaryTerm(
                        GlossaryHelper.GlossaryIdentifier,
                        term.TermIdentifier,
                        item.Name,
                        content));
                }
                else
                {
                    ServiceLocator.SendCommand(new ProposeGlossaryTerm(
                        GlossaryHelper.GlossaryIdentifier,
                        UniqueIdentifier.Create(),
                        item.Name, item.Content
                    ));
                }
            }

            HttpResponseHelper.Redirect("/ui/admin/assets/glossaries/search");
        }

        private void ImportDataRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (TermInfo)e.Item.DataItem;

            var languageRepeater = (Repeater)e.Item.FindControl("LanguageRepeater");
            languageRepeater.DataSource = dataItem.Content.Languages.OrderBy(l => l).Select(lang =>
            {
                var title = dataItem.Content.Title.GetText(lang);
                var description = dataItem.Content.Description.GetHtml(lang);

                return new
                {
                    Lang = lang.ToUpper(),
                    Html = title.IsEmpty()
                        ? description
                        : $"<h4>{title}</h4><div>{description}</div>",

                };
            });
            languageRepeater.DataBind();
        }

        #endregion

        #region Methods (load file)

        private bool LoadFile(Stream file)
        {
            ImportData = null;

            var encoding = Encoding.GetEncoding(FileEncoding.Value);

            string[][] csvArray;
            {
                try
                {
                    csvArray = CsvImportHelper.GetValues(file, null, false, encoding);
                }
                catch (ApplicationError ex)
                {
                    ScreenStatus.AddMessage(AlertType.Error, ex.Message);

                    return false;
                }
            }

            if (csvArray.Length < 2)
            {
                ScreenStatus.AddMessage(AlertType.Error, "The file has no data to import.");
                return false;
            }

            var mapping = ReadFileHeader(csvArray[0]);
            if (mapping == null)
                return false;

            var result = ReadFileData(mapping, csvArray);
            if (result == null)
                return false;

            ImportData = result.ToArray();

            return true;
        }

        private TermMappingInfo ReadFileHeader(string[] csvHeader)
        {
            var mapping = new TermMappingInfo();

            for (var i = 0; i < csvHeader.Length; i++)
            {
                var csvValue = StringHelper.RemoveNonAlphanumericCharacters(csvHeader[i]);
                if (csvValue.IsEmpty())
                    continue;

                if (csvValue.Equals(Col_Name, StringComparison.OrdinalIgnoreCase))
                {
                    if (mapping.NameIndex.HasValue)
                    {
                        Error_MoreThanOnce(csvValue);
                        return null;
                    }

                    mapping.NameIndex = i;

                    continue;
                }

                TermContentMappingInfo colInfo = null;

                if (csvValue.StartsWith(Col_Title, StringComparison.OrdinalIgnoreCase))
                    colInfo = new TermContentMappingInfo(i, Col_Title);
                else if (csvValue.StartsWith(Col_Definition, StringComparison.OrdinalIgnoreCase))
                    colInfo = new TermContentMappingInfo(i, Col_Definition, ContentLabel.Description);
                else
                    continue;

                Language.LanguageInfo lang;

                if (csvValue.Length > colInfo.Title.Length)
                {
                    var code = csvValue.Substring(colInfo.Title.Length);
                    code = Language.CodeExists(code) ? code : Language.GetCode(code);
                    lang = Language.GetInfo(code);
                }
                else
                {
                    lang = Language.GetInfo(Shift.Common.ContentContainer.DefaultLanguage);
                }

                if (lang == null)
                {
                    Error_LangNotDefined(csvValue);
                    return null;
                }

                if (!Organization.Languages.Any(x => x.TwoLetterISOLanguageName == lang.Code))
                {
                    Error_LangNotValid(lang.Name);
                    return null;
                }

                var mappingInfo = mapping.Languages
                    .FirstOrDefault(x => x.Language == lang);

                if (mappingInfo == null)
                    mapping.Languages.Add(mappingInfo = new TermLangMappingInfo(lang));
                else if (mappingInfo.Content.Any(x => x.Title == colInfo.Title))
                {
                    Error_MoreThanOnce($"{colInfo.Title} ({lang.Name})");
                    return null;
                }

                mappingInfo.Content.Add(colInfo);
            }

            if (!mapping.NameIndex.HasValue)
            {
                Error_RequiredNotFound(Col_Name);
                return null;
            }

            mapping.Languages.Sort((a, b) => a.Language.Name.CompareTo(b.Language.Name));

            var definitionNotFound = mapping.Languages
                .Where(l => !l.Content.Any(c => c.Title == Col_Definition))
                .Select(l => $"{Col_Definition} ({l.Language.Name})")
                .ToArray();
            if (definitionNotFound.Length > 0)
            {
                Error_RequiredNotFound(string.Join(", ", definitionNotFound));
                return null;
            }

            if (!mapping.Languages.Any(x => x.Language.Code.Equals(Shift.Common.ContentContainer.DefaultLanguage)))
            {
                Error_RequiredLanguage(Language.GetName(Shift.Common.ContentContainer.DefaultLanguage));
                return null;
            }

            return mapping;
        }

        private List<TermInfo> ReadFileData(TermMappingInfo mapping, string[][] csvArray)
        {
            var result = new List<TermInfo>();

            for (var i = 1; i < csvArray.Length; i++)
            {
                var csvRow = csvArray[i];

                var info = new TermInfo();
                info.Name = GetValue(mapping.NameIndex.Value);

                if (info.Name == null)
                {
                    Error_RequiredValue(i, Col_Name);
                    return null;
                }

                if (result.Any(x => x.Name.Equals(info.Name)))
                {
                    Error_TermDuplicate(info.Name);
                    return null;
                }

                foreach (var lang in mapping.Languages)
                {
                    foreach (var content in lang.Content)
                    {
                        var value = GetValue(content.Index);

                        if (value.IsEmpty() && content.Title == Col_Definition)
                        {
                            Error_RequiredValue(i, $"{Col_Definition} ({lang.Language.Name})");
                            return null;
                        }

                        info.Content[content.Label].Text[lang.Language.Code] = value;
                    }
                }

                result.Add(info);

                string GetValue(int index) =>
                    index < csvRow.Length ? csvRow[index].Trim().NullIfEmpty() : null;
            }

            return result;
        }

        #endregion

        #region Methods (helpers)

        protected string GetStatusHtml()
        {
            var dataItem = (TermInfo)Page.GetDataItem();
            var term = ServiceLocator.GlossarySearch.GetTerm(GlossaryHelper.GlossaryIdentifier, dataItem.Name);

            return term == null
                ? "<span class='badge bg-success'>New</span>"
                : $"<a target='_blank' href='/ui/admin/assets/glossaries/terms/revise?term={term.TermIdentifier}' class='badge bg-warning'>Exists</a>";
        }

        private void Error_MoreThanOnce(string colName) =>
            ScreenStatus.AddMessage(AlertType.Error, "Column can't be defined more than once: " + colName);

        private void Error_LangNotDefined(string colName) =>
            ScreenStatus.AddMessage(AlertType.Error, "Column language is not defined: " + colName);

        private void Error_LangNotValid(string lang) =>
            ScreenStatus.AddMessage(AlertType.Error, $"{lang} is not supported by {Organization.CompanyName} organization account.");

        private void Error_RequiredNotFound(string colName) =>
            ScreenStatus.AddMessage(AlertType.Error, "Required column not found: " + colName);

        private void Error_RequiredValue(int rowIndex, string colName) =>
            ScreenStatus.AddMessage(AlertType.Error, $"Row #{rowIndex + 1:n0} is missing the '{colName}' column value. The '{colName}' is required column.");

        private void Error_RequiredLanguage(string lang) =>
            ScreenStatus.AddMessage(AlertType.Error, $"{lang} is required language.");

        private void Error_TermDuplicate(string term) =>
            ScreenStatus.AddMessage(AlertType.Error, $"The name of term must be unique: " + term);

        #endregion
    }
}