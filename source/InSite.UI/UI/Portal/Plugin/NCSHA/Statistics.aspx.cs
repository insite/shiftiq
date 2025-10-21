using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.Persistence.Plugin.NCSHA;
using InSite.UI.Layout.Lobby;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using Field = InSite.Persistence.Plugin.NCSHA.Field;
using Filter = InSite.Persistence.Plugin.NCSHA.Filter;
using StringFormat = System.Drawing.StringFormat;

namespace InSite.Custom.NCSHA.Analytics.Chart
{
    public partial class View : LobbyBasePage
    {
        #region Classes

        [JsonObject(MemberSerialization.OptIn)]
        private abstract class JsonResult
        {
            public JsonResult(string type)
            {
                Type = type;
            }

            [JsonProperty(PropertyName = "type")]
            public string Type { get; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class JsonErrorResult : JsonResult
        {
            public JsonErrorResult(string message)
                : base("ERROR")
            {
                Message = message;
            }

            [JsonProperty(PropertyName = "message")]
            public string Message { get; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class SaveFilterResult : JsonResult
        {
            [JsonProperty(PropertyName = "code")]
            public string Code { get; }

            public SaveFilterResult(string code)
                : base("FILTER")
            {
                Code = code;
            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class LoadFilterResult : JsonResult
        {
            [JsonProperty(PropertyName = "data")]
            public IEnumerable<FilterGroupModel> Data { get; set; }

            public LoadFilterResult()
                : base("FILTER")
            {

            }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class FilterGroupModel
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "allowDelete")]
            public bool AllowDelete { get; set; }

            [JsonProperty(PropertyName = "items")]
            public IEnumerable<FilterItemModel> Items { get; set; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        private class FilterItemModel
        {
            [JsonProperty(PropertyName = "id")]
            public Guid ID { get; set; }

            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "date")]
            public string Date { get; set; }
        }

        private class DownloadCsvData
        {
            public ChartDownloadCsvModel Model { get; set; }
            public ChartDataGetModel[] CriteriaJson { get; set; }
        }

        private class CsvItem
        {
            public string Name { get; private set; }
            public int Year { get; private set; }
            public decimal Value { get; private set; }

            public CsvItem(string name, int year, decimal value)
            {
                Name = name;
                Year = year;
                Value = value;
            }
        }

        private class DownloadPngData
        {
            public string Image { get; set; }
            public ChartDataGetModel[] CriteriaJson { get; set; }
        }

        private class PngTitle
        {
            public IReadOnlyList<PngTitleSurvey> Surveys => _surveys;

            private List<PngTitleSurvey> _surveys;

            private PngTitle()
            {

            }

            #region Methods

            public static PngTitle Create(ChartDataGetModel[] getModels)
            {
                var surveys = GetSurveys(getModels, out Dictionary<string, List<PngTitleField>> fields);
                AppendFieldsYears(fields);
                GroupCategories(surveys);

                return new PngTitle
                {
                    _surveys = surveys
                };
            }

            public void Draw(Graphics gfx, RectangleF layoutRect, Font normalFont, Font boldFont, Brush brush)
            {
                var cursor = new PointF(0, 0);

                Draw(gfx, layoutRect, normalFont, boldFont, brush, ref cursor, false);
            }

            public float GetHeight(float layoutWidth, Font normalFont, Font boldFont)
            {
                var cursor = new PointF(0, 0);

                using (var bmp = new Bitmap(1, 1))
                {
                    using (var gfx = Graphics.FromImage(bmp))
                    {
                        var layoutRect = new RectangleF(0, 0, layoutWidth, 0);
                        Draw(gfx, layoutRect, normalFont, boldFont, null, ref cursor, true);
                    }
                }

                return cursor.Y + normalFont.GetHeight() * 1.5f;
            }

            #endregion

            #region Methods (initialization)

            private static List<PngTitleSurvey> GetSurveys(ChartDataGetModel[] getModels, out Dictionary<string, List<PngTitleField>> fields)
            {
                fields = new Dictionary<string, List<PngTitleField>>();

                var programs = ChartModel.GetPrograms();
                var surveys = new List<PngTitleSurvey>();

                PngTitleSurvey survey = null;
                PngTitleCategory category = null;

                foreach (var getModel in getModels)
                {
                    foreach (var code in getModel.Code)
                    {
                        var field = programs.GetField(code);

                        if (survey == null || survey.Title != field.Category.Program.Title)
                        {
                            survey = new PngTitleSurvey(field.Category.Program.Title);

                            surveys.Add(survey);
                        }

                        if (category == null || category.Title != field.Category.Title)
                            category = survey.AddCategory(field.Category.Title);

                        var titleField = category.AddField(field.Code, field.Title, getModel.FromYear, getModel.ToYear);

                        if (!fields.ContainsKey(field.Code))
                            fields.Add(field.Code, new List<PngTitleField>());

                        fields[field.Code].Add(titleField);
                    }

                    category = null;
                }

                return surveys;
            }

            private static void AppendFieldsYears(Dictionary<string, List<PngTitleField>> fields)
            {
                var codeFilter = fields.Values
                    .Where(x => x.Any(y => !y.FromYear.HasValue || !y.ThruYear.HasValue))
                    .Select(x => x[0].Code)
                    .ToArray();

                if (codeFilter.Length == 0)
                    return;

                var data = CounterRepository
                    .Distinct(x => new { x.Code, x.Year }, x => codeFilter.Contains(x.Code), "Code")
                    .GroupBy(x => x.Code)
                    .Select(x => new { Code = x.Key, MinYear = x.Min(y => y.Year), MaxYear = x.Max(y => y.Year) })
                    .ToArray();

                foreach (var item in data)
                {
                    foreach (var field in fields[item.Code])
                    {
                        if (!field.FromYear.HasValue)
                            field.FromYear = item.MinYear;

                        if (!field.ThruYear.HasValue)
                            field.ThruYear = item.MaxYear;
                    }
                }
            }

            private static void GroupCategories(List<PngTitleSurvey> surveys)
            {
                PngTitleSurvey prevSurvey = null;
                PngTitleCategory prevCategory = null;

                for (var x = 0; x < surveys.Count; x++)
                {
                    var survey = surveys[x];

                    for (var y = 0; y < survey.Categories.Count; y++)
                    {
                        var category = survey.Categories[y];

                        if (
                                prevCategory != null
                            && (prevSurvey == null || prevSurvey.Title == survey.Title)
                            && category.Title == prevCategory.Title
                            && category.FromYear == prevCategory.FromYear
                            && category.ThruYear == prevCategory.ThruYear
                           )
                        {
                            prevCategory.AddFields(category.Fields);
                            survey.RemoveCategory(category);
                            category = null;
                            y--;
                        }

                        if (category != null)
                            prevCategory = category;
                    }

                    if (survey.Categories.Count == 0)
                    {
                        surveys.Remove(survey);
                        survey = null;
                        x--;
                    }

                    if (survey != null)
                        prevSurvey = survey;
                }
            }

            #endregion

            #region Methods (drawing)

            private void Draw(Graphics gfx, RectangleF layoutRect, Font normalFont, Font boldFont, Brush brush, ref PointF cursor, bool isMeasure)
            {
                var isFirstSurvey = true;

                foreach (var survey in _surveys)
                {
                    if (!isFirstSurvey)
                    {
                        cursor.X = 0;
                        cursor.Y += normalFont.GetHeight() * 1.5f;
                    }
                    else
                        isFirstSurvey = false;

                    DrawText(gfx, boldFont, brush, layoutRect, ref cursor, survey.Title, isMeasure);

                    var isFirstField = true;
                    var sb = new StringBuilder(": ");

                    foreach (var category in survey.Categories)
                    {
                        foreach (var field in category.Fields)
                        {
                            if (isFirstField)
                                isFirstField = false;
                            else
                                sb.Append(", ");

                            sb.AppendFormat("{0} – {1} ({2}-{3})", category.Title, field.Title, field.FromYear, field.ThruYear);
                        }
                    }

                    sb.Append(";");

                    DrawText(gfx, normalFont, brush, layoutRect, ref cursor, sb.ToString(), isMeasure);
                }
            }

            private static void DrawText(Graphics gfx, Font font, Brush brush, RectangleF layoutRect, ref PointF cursor, string value, bool isMeasure)
            {
                if (string.IsNullOrEmpty(value))
                    return;

                using (var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near, Trimming = StringTrimming.None, FormatFlags = StringFormatFlags.NoClip | StringFormatFlags.NoWrap })
                {
                    var layoutWidth = layoutRect.Width - layoutRect.X;
                    var buffer = new StringBuilder();

                    foreach (var ch in value)
                    {
                        if (ch == ' ' && buffer.Length > 0)
                        {
                            DrawWord(gfx, font, brush, layoutRect, ref cursor, format, layoutWidth, buffer.ToString(), isMeasure);
                            buffer.Clear();
                        }

                        buffer.Append(ch);
                    }

                    if (buffer.Length > 0)
                        DrawWord(gfx, font, brush, layoutRect, ref cursor, format, layoutWidth, buffer.ToString(), isMeasure);
                }
            }

            private static void DrawWord(Graphics gfx, Font font, Brush brush, RectangleF layoutRect, ref PointF cursor, StringFormat format, float layoutWidth, string word, bool isMeasure)
            {
                var wordWidth = GetWordWidth(word);

                if (wordWidth + cursor.X > layoutWidth)
                {
                    cursor.X = 0;
                    cursor.Y += font.GetHeight();

                    word = word.TrimStart();
                    wordWidth = GetWordWidth(word);
                }

                if (!isMeasure)
                    gfx.DrawString(word, font, brush, layoutRect.X + cursor.X, layoutRect.Y + cursor.Y, format);

                cursor.X += wordWidth;

                float GetWordWidth(string value)
                {
                    var wordSize = gfx.MeasureString(value, font, int.MaxValue, format);
                    var spaceSize = gfx.MeasureString(" ", font, int.MaxValue, format);

                    return wordSize.Width - spaceSize.Width;
                }
            }

            #endregion

            #region Overriden methods

            public override string ToString()
            {
                var sb = new StringBuilder();

                var isFirstSurvey = true;

                foreach (var survey in _surveys)
                {
                    if (isFirstSurvey)
                        isFirstSurvey = false;
                    else
                        sb.Append(';').AppendLine();

                    sb.Append('*').Append(survey.Title).Append("*: ");

                    var isFirstField = true;

                    foreach (var category in survey.Categories)
                    {
                        foreach (var field in category.Fields)
                        {
                            if (isFirstField)
                                isFirstField = false;
                            else
                                sb.Append(", ");

                            sb.AppendFormat("{0} – {1} ({2}-{3})", category.Title, field.Title, field.FromYear, field.ThruYear);
                        }
                    }
                }

                return sb.ToString();
            }

            #endregion
        }

        private class PngTitleSurvey
        {
            #region Proprties

            public string Title { get; set; }

            public IReadOnlyList<PngTitleCategory> Categories => _categories;

            #endregion

            #region Fields

            private List<PngTitleCategory> _categories;

            #endregion

            #region Construction

            public PngTitleSurvey(string title)
            {
                Title = title;
                _categories = new List<PngTitleCategory>();
            }

            #endregion

            #region Methods

            public PngTitleCategory AddCategory(string title)
            {
                var item = new PngTitleCategory(title);

                _categories.Add(item);

                return item;
            }

            public void RemoveCategory(PngTitleCategory category)
            {
                _categories.Remove(category);
            }

            #endregion
        }

        private class PngTitleCategory
        {
            #region Properties

            public string Title { get; private set; }

            public int FromYear => _fromYear ?? (_fromYear = _fields.Min(x => x.FromYear ?? 0)).Value;

            public int ThruYear => _thruYear ?? (_thruYear = _fields.Max(x => x.ThruYear ?? 0)).Value;

            public IReadOnlyList<PngTitleField> Fields => _fields;

            #endregion

            #region Fields

            private int? _fromYear;
            private int? _thruYear;
            private List<PngTitleField> _fields;

            #endregion

            #region Construction

            public PngTitleCategory(string title)
            {
                Title = title;
                _fields = new List<PngTitleField>();
            }

            #endregion

            #region Methods

            public PngTitleField AddField(string code, string title, int? fromYear, int? thruYear)
            {
                var item = new PngTitleField(code, title, fromYear, thruYear);
                item.Updated += Item_Updated;

                _fields.Add(item);

                return item;
            }

            public void AddFields(IEnumerable<PngTitleField> fields)
            {
                foreach (var field in fields)
                {
                    field.Updated += Item_Updated;
                    _fields.Add(field);
                }
            }

            private void Item_Updated(object sender, EventArgs e)
            {
                _fromYear = null;
                _thruYear = null;
            }

            #endregion
        }

        private class PngTitleField
        {
            #region Events

            public event EventHandler Updated;

            public void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

            #endregion

            #region Properties

            public string Title { get; private set; }

            public string Code { get; private set; }

            public int? FromYear
            {
                get { return _fromYear; }
                set { _fromYear = value; OnUpdated(); }
            }

            public int? ThruYear
            {
                get { return _thruYear; }
                set { _thruYear = value; OnUpdated(); }
            }

            #endregion

            #region Fields

            public int? _fromYear;
            public int? _thruYear;

            #endregion

            #region Construction

            public PngTitleField(string code, string title, int? fromYear, int? thruYear)
            {
                Code = code;
                Title = title;
                FromYear = fromYear;
                ThruYear = thruYear;
            }

            #endregion
        }

        #endregion

        #region Properties

        private Guid? FilterId => Guid.TryParse(Request["filter"], out var filterId) ? filterId : (Guid?)null;

        private bool IsDisplayWelcomeWindow
        {
            get => Session[typeof(View) + "." + nameof(IsDisplayWelcomeWindow)] as bool? ?? true;
            set => Session[typeof(View) + "." + nameof(IsDisplayWelcomeWindow)] = value;
        }

        private ChartModel _model;
        private ChartModel Model => _model ?? (_model = ChartModel.Create(FilterId));

        protected string JsonPrograms => JsonHelper.SerializeJsObject(Model.Programs);
        protected string DefaultChartData => Model.Data != null ? JsonHelper.SerializeJsObject(Model.Data) : "null";

        Guid? UserIdentifier => CurrentSessionState.Identity?.User?.UserIdentifier;

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadCsvButton.Click += DownloadCsvButton_Click;
            DownloadPngButton.Click += DownloadPngButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (Organization.Identifier != OrganizationIdentifiers.NCSHA
             || UserIdentifier == null
             || !CurrentSessionState.Identity.IsGranted("Custom/NCSHA/Analytics", PermissionOperation.Read))
                HttpResponseHelper.Redirect("/");

            HandleAjaxRequest();

            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            if (IsDisplayWelcomeWindow)
            {
                var resource = ServiceLocator.PageSearch.BindFirst(
                    x => new
                    {
                        x.PageIdentifier,
                        x.Title
                    },
                    x => x.OrganizationIdentifier == Organization.OrganizationIdentifier
                      && x.PageSlug == "Analytics/Welcome");
                var welcomeModel = resource == null
                    ? null
                    : new ChartWelcomeModel
                    {
                        Title = resource.Title,
                        BodyHtml = ServiceLocator.ContentSearch.GetHtml(resource.PageIdentifier, ContentLabel.Body)
                    };

                if (welcomeModel != null && !string.IsNullOrEmpty(welcomeModel.BodyHtml))
                {
                    if (string.IsNullOrEmpty(welcomeModel.Title))
                        welcomeModel.Title = "Welcome!";

                    WelcomeModelTitle.Text = welcomeModel.Title;
                    WelcomeModelBodyHtml.Text = welcomeModel.BodyHtml;
                }

                IsDisplayWelcomeWindow = false;
            }

            if (Model.FilterId.HasValue)
                OnHistoryEventOccurred(new ChartFilterLoadedEvent(Model.FilterId.Value, Model.FilterName), Model.FilterData);

            RefreshPanel.Visible = CurrentSessionState.Identity.IsGranted("Custom/NCSHA/Analytics", PermissionOperation.Configure);
            WelcomeWindowPanel.Visible = Model.WelcomeModel != null;
        }

        private void OnHistoryEventOccurred(ChartHistoryEvent @event, params ChartDataGetModel[] chartCriteria)
        {
            var codeFilter = chartCriteria.SelectMany(x => x.Code);
            var fields = FieldRepository.Bind(x => x, x => codeFilter.Contains(x.Code)).ToDictionary(x => x.Code, x => x);

            foreach (var c in chartCriteria)
            {
                var eventCriteria = @event.AddCriteria(c.Region, c.FromYear, c.ToYear, c.Func, c.DatasetType, c.AxisName, c.AxisUnit);

                foreach (var code in c.Code)
                {
                    if (!fields.TryGetValue(code, out var field))
                        field = new Field
                        {
                            Code = code,
                            IsNumeric = false,
                            Category = "N/A",
                            Name = "N/A",
                            Unit = "N/A"
                        };

                    eventCriteria.AddField(field.Category, field.Name, field.Code);
                }
            }

            var u = CurrentSessionState.Identity.User;
            HistoryRepository.Insert(u.UserIdentifier, u.FullName, u.Email, @event);
        }

        #endregion

        #region Download

        private void DownloadCsvButton_Click(object sender, EventArgs e)
        {
            var data = JsonConvert.DeserializeObject<DownloadCsvData>(DownloadData.Value);

            if (data?.Model?.Datasets == null || data.CriteriaJson.IsEmpty())
                HttpResponseHelper.SendHttp404();

            var items = new List<CsvItem>();

            foreach (var dataset in data.Model.Datasets)
            {
                for (var i = 0; i < dataset.Items.Length; i++)
                {
                    var item = dataset.Items[i];
                    if (!item.Value.HasValue)
                        continue;

                    items.Add(new CsvItem(dataset.Title, item.Year, item.Value.Value));
                }
            }

            var helper = new CsvExportHelper(items.ToSearchResult());

            helper.AddMapping("Name", "Name");
            helper.AddMapping("Year", "Year");
            helper.AddMapping("Value", "Value");

            var outputData = helper.GetBytes(Encoding.UTF8);

            OnHistoryEventOccurred(new ChartDownloadedEvent("Comma Separated Values", "csv"), data.CriteriaJson);

            Response.SendFile($"chart-{DateTime.UtcNow:yyyy-MM-dd}", "csv", outputData);
        }

        private void DownloadPngButton_Click(object sender, EventArgs e)
        {
            const string imgPrefix = "data:image/png;base64,";

            var data = JsonConvert.DeserializeObject<DownloadPngData>(DownloadData.Value);
            var image = data.Image;
            var chartGetModels = data.CriteriaJson;

            if (image.IsEmpty() || !image.StartsWith(imgPrefix) || chartGetModels.IsEmpty())
                HttpResponseHelper.SendHttp404();

            var inputBase64ImageData = image.Substring(imgPrefix.Length);
            if (string.IsNullOrEmpty(inputBase64ImageData))
                HttpResponseHelper.SendHttp404();

            var pngTitle = PngTitle.Create(chartGetModels);

            var inputImageData = Convert.FromBase64String(inputBase64ImageData);
            byte[] outputImageData = null;

            using (var fontFamily = new FontFamily("Helvetica"))
            {
                Brush fontBrush = null;
                Font normalFont = null;
                Font boldFont = null;

                try
                {
                    using (var inputMs = new MemoryStream(inputImageData))
                    {
                        using (var inputImage = new Bitmap(inputMs))
                        {
                            var fontSize = inputImage.Width * 0.015f;

                            fontBrush = new SolidBrush(Color.FromArgb(255, 83, 101, 112));
                            normalFont = new Font(fontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
                            boldFont = new Font(fontFamily, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);

                            var copyrightRectangleHeight = normalFont.GetHeight() * 2.5f;

                            var titleYPadding = normalFont.GetHeight() * 1.5f;
                            var titleXPadding = inputImage.Width * 0.05f;
                            var titleRectangleHeight = pngTitle.GetHeight(inputImage.Width - titleXPadding * 2, normalFont, boldFont) + titleYPadding * 2;

                            using (var outputImage = new Bitmap(inputImage.Width, (int)(titleRectangleHeight + inputImage.Height + copyrightRectangleHeight)))
                            {
                                outputImage.SetResolution(inputImage.HorizontalResolution, inputImage.VerticalResolution);

                                using (var g = Graphics.FromImage(outputImage))
                                {
                                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                                    g.Clear(Color.White);
                                    g.DrawImageUnscaled(inputImage, 0, (int)titleRectangleHeight);

                                    pngTitle.Draw(g, new RectangleF(titleXPadding, titleYPadding, inputImage.Width - titleXPadding * 2, inputImage.Height), normalFont, boldFont, fontBrush);

                                    using (var copyrightFormat = new StringFormat { LineAlignment = StringAlignment.Center, Alignment = StringAlignment.Center })
                                    {
                                        var text = string.Format("Copyright © {0} National Council of State Housing Agencies", DateTime.Today.Year);

                                        g.DrawString(text, normalFont, fontBrush, new RectangleF(0, titleRectangleHeight + inputImage.Height, outputImage.Width, copyrightRectangleHeight), copyrightFormat);
                                    }
                                }

                                using (var outputMs = new MemoryStream())
                                {
                                    outputImage.Save(outputMs, ImageFormat.Png);

                                    outputImageData = outputMs.ToArray();
                                }
                            }
                        }
                    }
                }
                finally
                {
                    if (fontBrush != null)
                    {
                        fontBrush.Dispose();
                        fontBrush = null;
                    }
                    if (normalFont != null)
                    {
                        normalFont.Dispose();
                        normalFont = null;
                    }

                    if (boldFont != null)
                    {
                        boldFont.Dispose();
                        boldFont = null;
                    }
                }
            }

            OnHistoryEventOccurred(new ChartDownloadedEvent("PNG Image", "png"), chartGetModels);

            Response.SendFile($"chart-{DateTime.UtcNow:yyyy-MM-dd}", "png", outputImageData);
        }

        #endregion

        #region Handle actions

        private void HandleAjaxRequest()
        {
            if (!HttpRequestHelper.IsAjaxRequest)
                return;

            var action = Page.Request.Form["action"];
            string json;

            if (action == "criteria")
                json = DoActionCriteria();
            else if (action == "filter-get")
                json = DoActionFilterGet();
            else if (action == "filter-post")
                json = DoActionFilterPost();
            else if (action == "filter-delete")
                json = DoActionFilterDelete();
            else if (action == "data")
                json = DoActionData();
            else
                return;

            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(json);
            Response.End();
        }

        private string DoActionCriteria()
        {
            var code = Page.Request.Form["code"].Split(new[] { ',' });
            var model = ChartCriteriaModel.Create(code);

            return JsonConvert.SerializeObject(model);
        }

        private string DoActionFilterGet()
        {
            var data = FilterRepository
                .Bind(x => new { x.FilterId, x.AuthorUserIdentifier, x.FilterName, x.DateSaved }, x => true)
                .GroupBy(x => x.AuthorUserIdentifier);

            var contacts = UserSearch
                .Bind(
                    x => new { x.UserIdentifier, x.FullName },
                    new UserFilter { IncludeUserIdentifiers = data.Select(x => x.Key).ToArray() })
                .ToDictionary(x => x.UserIdentifier, x => x);

            var groups = new List<FilterGroupModel>();

            foreach (var dataGroup in data)
            {
                var contact = contacts.GetOrDefault(dataGroup.Key);

                groups.Add(new FilterGroupModel
                {
                    Name = contact == null ? "(Unknown)" : contact.FullName,
                    AllowDelete = contact != null && contact.UserIdentifier == UserIdentifier,
                    Items = dataGroup
                        .Select(x => new FilterItemModel { ID = x.FilterId, Name = x.FilterName, Date = x.DateSaved.ToString("MMM d, yyyy") })
                        .OrderBy(x => x.Name)
                });
            }

            var result = new LoadFilterResult { Data = groups.OrderBy(x => x.Name) };

            return JsonConvert.SerializeObject(result);
        }

        private string DoActionFilterPost()
        {
            var name = Page.Request.Form["name"];
            var data = Page.Request.Form["data"];
            var overwrite = bool.TryParse(Page.Request.Form["overwrite"], out var boolValue) ? boolValue : (bool?)null;

            if (name != null)
                name = name.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(data))
                return JsonTextError("Invalid request.");

            try
            {
                JToken.Parse(data);
            }
            catch (Exception)
            {
                return JsonTextError("Invalid request.");
            }

            var authorId = UserIdentifier ?? Shift.Constant.UserIdentifiers.Someone;
            if (overwrite != true && FilterRepository.Exists(x => x.AuthorUserIdentifier == authorId && x.FilterName == name))
                return FilterResult("EXISTS");

            var filter = new Filter
            {
                AuthorUserIdentifier = authorId,
                FilterName = name,
                DateSaved = DateTimeOffset.UtcNow,
                FilterData = data
            };

            FilterRepository.Save(filter);

            return FilterResult("OK");

            string FilterResult(string code)
            {
                var model = new SaveFilterResult(code);
                return JsonConvert.SerializeObject(model);
            }
        }

        private string DoActionFilterDelete()
        {
            var id = Guid.Parse(Page.Request["id"]);

            var entity = FilterRepository.BindFirst(x => new { x.AuthorUserIdentifier }, x => x.FilterId == id);

            if (entity == null)
                return JsonTextError("The record not found.");

            if (entity.AuthorUserIdentifier != UserIdentifier)
                return JsonTextError("Access denied.");

            FilterRepository.Delete(id);

            return "{\"type\":\"OK\"}";
        }

        private string DoActionData()
        {
            var model = GetChartDataGetModel();

            if (model.Code.IsEmpty())
                return JsonTextError("Invalid request: field code is null");

            var fieldsUnits = CounterRepository.Distinct(x => x.Unit, x => model.Code.Contains(x.Code));
            if (fieldsUnits.Length != 1)
                return JsonTextError("All selected fields must have the same measurement unit.");

            if (!string.IsNullOrEmpty(model.AxisUnit) && !string.Equals(model.AxisUnit, fieldsUnits[0], StringComparison.OrdinalIgnoreCase))
                return JsonTextError($"Your {(model.AxisName ?? "(unknown)").ToLower()} Y axis is displaying {model.AxisUnit}. If you want to add multiple series to the same Y axis then they must have the same measurement unit. If you are not already using the secondary Y axis then select it for this series.");

            var filter = new ChartDataModel.Filter(model);
            var result = ChartDataModel.Create(filter);

            OnHistoryEventOccurred(new ChartViewedEvent(), model);

            return JsonConvert.SerializeObject(result);
        }

        private ChartDataGetModel GetChartDataGetModel()
        {
            return new ChartDataGetModel
            {
                Code = !string.IsNullOrEmpty(Page.Request.Form["code"]) ? Page.Request.Form["code"].Split(new[] { ',' }) : null,
                Region = !string.IsNullOrEmpty(Page.Request.Form["region"]) ? Page.Request.Form["region"].Split(new[] { ',' }) : null,
                FromYear = int.TryParse(Page.Request.Form["fromYear"], out var fromYear) ? fromYear : (int?)null,
                ToYear = int.TryParse(Page.Request.Form["toYear"], out var toYear) ? toYear : (int?)null,
                Func = Page.Request.Form["func"],
                AxisName = Page.Request.Form["axisName"],
                AxisUnit = Page.Request.Form["axisUnit"],
                DatasetType = Page.Request.Form["datasetType"]
            };
        }

        private string JsonTextError(string textMessage)
        {
            var html = string.IsNullOrEmpty(textMessage)
                ? string.Empty
                : textMessage.Replace("\r", string.Empty).Replace("\n", "<br/>");

            var data = new JsonErrorResult(html);

            return JsonConvert.SerializeObject(data);
        }

        #endregion
    }
}