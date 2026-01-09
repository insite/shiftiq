using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Persistence.Plugin.NCSHA
{
    public class ChartModel
    {
        #region Classes

        public class FieldEventArgs : EventArgs
        {
            public FieldInfo Field { get; private set; }

            public FieldEventArgs(FieldInfo field)
            {
                Field = field ?? throw new ArgumentNullException(nameof(field));
            }
        }

        public delegate void FieldEventHandler(object sender, FieldEventArgs args);

        public class ProgramCollection : IReadOnlyList<ProgramInfo>
        {
            #region Properties

            public ProgramInfo this[int index] => _items[index];

            public int Count => _items.Count;

            #endregion

            #region Fields

            private List<ProgramInfo> _items = new List<ProgramInfo>();
            private Dictionary<string, ProgramInfo> _codes = new Dictionary<string, ProgramInfo>(StringComparer.OrdinalIgnoreCase);
            private Dictionary<string, FieldInfo> _fields = new Dictionary<string, FieldInfo>(StringComparer.OrdinalIgnoreCase);

            #endregion

            #region Methods

            public ProgramInfo Add(string code, string title)
            {
                if (string.IsNullOrEmpty(code))
                    throw new ArgumentNullException(nameof(code));

                if (string.IsNullOrEmpty(title))
                    throw new ArgumentNullException(nameof(title));

                if (_codes.ContainsKey(code))
                    throw new ApplicationError($"The collection already contains this program code: {code}.");

                if (code.Length != 2)
                    throw new ApplicationError($"Invalid program code: {code}.");

                var program = new ProgramInfo(this, code, title);
                program.FieldAdded += Program_FieldAdded;
                _items.Add(program);
                _codes.Add(program.Code, program);

                return program;
            }

            public ProgramInfo Get(string code)
            {
                if (code.Length > 2)
                    code = code.Substring(0, 2);

                return _codes.ContainsKey(code)
                    ? _codes[code]
                    : null;
            }

            public FieldInfo GetField(string code)
            {
                return _fields.ContainsKey(code) ? _fields[code] : null;
            }

            #endregion

            #region Event handlers

            private void Program_FieldAdded(object sender, FieldEventArgs args)
            {
                _fields.Add(args.Field.Code, args.Field);
            }

            #endregion

            #region IEnumerable

            public IEnumerator<ProgramInfo> GetEnumerator() => _items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class ProgramInfo
        {
            #region Events

            public event FieldEventHandler FieldAdded;

            private void OnFieldAdded(FieldInfo field)
            {
                var args = new FieldEventArgs(field);

                FieldAdded?.Invoke(this, args);
            }

            #endregion

            #region Properties

            [JsonProperty(PropertyName = "code")]
            public string Code { get; private set; }

            [JsonProperty(PropertyName = "title")]
            public string Title { get; private set; }

            [JsonProperty(PropertyName = "categories")]
            public IReadOnlyList<CategoryInfo> Categories => _list;

            public ProgramCollection Root { get; private set; }

            #endregion

            #region Fields

            private List<CategoryInfo> _list = new List<CategoryInfo>();
            private Dictionary<string, CategoryInfo> _dict = new Dictionary<string, CategoryInfo>();

            #endregion

            #region Construction

            public ProgramInfo(ProgramCollection root, string code, string title)
            {
                Code = code;
                Title = title;
                Root = root;
            }

            #endregion

            #region Methods

            public CategoryInfo GetCategory(string title)
            {
                if (_dict.ContainsKey(title))
                    return _dict[title];

                var category = new CategoryInfo(this, title);
                category.FieldAdded += Category_FieldAdded;

                _list.AddSorted(category);
                _dict.Add(category.Title, category);

                return category;
            }

            #endregion

            #region Event handlers

            private void Category_FieldAdded(object sender, FieldEventArgs args) =>
                FieldAdded?.Invoke(this, args);

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class CategoryInfo : IComparable<CategoryInfo>
        {
            #region Events

            public event FieldEventHandler FieldAdded;

            private void OnFieldAdded(FieldInfo field)
            {
                var args = new FieldEventArgs(field);

                FieldAdded?.Invoke(this, args);
            }

            #endregion

            #region Properties

            [JsonProperty(PropertyName = "title")]
            public string Title { get; private set; }

            [JsonProperty(PropertyName = "fields")]
            public IReadOnlyList<FieldInfo> Fields => _items;

            public ProgramInfo Program { get; private set; }

            #endregion

            #region Fields

            private List<FieldInfo> _items = new List<FieldInfo>();

            #endregion

            #region Construction

            public CategoryInfo(ProgramInfo program, string title)
            {
                Title = title;
                Program = program;
            }

            #endregion

            #region Methods

            public void AddField(string code, string title)
            {
                var field = new FieldInfo(this, code, title);

                _items.AddSorted(field);

                OnFieldAdded(field);
            }

            public int CompareTo(CategoryInfo other) => this.Title.CompareTo(other.Title);

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class FieldInfo : IComparable<FieldInfo>
        {
            #region Properties

            [JsonProperty(PropertyName = "code")]
            public string Code { get; private set; }

            [JsonProperty(PropertyName = "title")]
            public string Title { get; private set; }

            public CategoryInfo Category { get; private set; }

            #endregion

            #region Construction

            public FieldInfo(CategoryInfo category, string code, string title)
            {
                Code = code;
                Title = title;
                Category = category;
            }

            #endregion

            #region Methods

            public int CompareTo(FieldInfo other)
            {
                var result = string.Compare(Title, other.Title, StringComparison.OrdinalIgnoreCase);

                if (result == 0)
                    result = string.Compare(Code, other.Code, StringComparison.OrdinalIgnoreCase);

                return result;
            }

            #endregion
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public class ChartData
        {
            [JsonProperty(PropertyName = "filter")]
            public string FilterJson { get; set; }

            [JsonProperty(PropertyName = "datasets")]
            public List<IEnumerable<ChartDataModel>> Datasets { get; } = new List<IEnumerable<ChartDataModel>>();
        }

        #endregion

        #region Properties

        public Guid? FilterId { get; private set; }

        public string FilterName { get; private set; }

        public ChartDataGetModel[] FilterData { get; private set; }

        public ChartData Data { get; private set; }

        public ChartWelcomeModel WelcomeModel { get; set; } // To remove

        public IEnumerable<ProgramInfo> Programs { get; private set; }

        #endregion

        #region Construction

        private ChartModel()
        {

        }

        #endregion

        #region Initialization

        public static ChartModel Create(Guid? filterId)
        {
            var model = new ChartModel
            {
                Programs = GetPrograms().Where(x => x.Categories.Count > 0),
            };

            var filterEntity = filterId.HasValue
                ? FilterRepository.BindFirst(x => new { x.FilterId, x.FilterName, x.FilterData }, x => x.FilterId == filterId.Value)
                : null;

            if (filterEntity != null)
            {
                var chartData = new ChartData
                {
                    FilterJson = filterEntity.FilterData
                };

                if (!string.IsNullOrEmpty(chartData.FilterJson))
                {
                    var inputs = JsonConvert.DeserializeObject<ChartDataGetModel[]>(chartData.FilterJson);
                    foreach (var input in inputs)
                    {
                        var filter = new ChartDataModel.Filter(input);
                        var datasets = ChartDataModel.Create(filter);
                        chartData.Datasets.Add(datasets);
                    }

                    if (chartData.Datasets.Count > 0)
                    {
                        model.FilterId = filterEntity.FilterId;
                        model.FilterName = filterEntity.FilterName;
                        model.FilterData = inputs;
                        model.Data = chartData;
                    }
                }
            }

            return model;
        }

        public static ProgramCollection GetPrograms(bool loadFields = true)
        {
            var programs = new ProgramCollection
            {
                { "AB", "Administration and Budget" },
                { "HI", "HOME Investment Partnerships" },
                { "HC", "Low Income Housing Tax Credits" },
                { "MF", "Multifamily Bonds" },
                { "MR", "Mortgage Revenue Bonds" },
                { "PA", "Private Activity Bonds" }
            };

            if (!loadFields)
                return programs;

            var excludedCodes = new[] { "HC084", "HC329", "HC343", "HC344", "HC345", "MF224", "MF225", "MF226", "MR186" };

            var data = CounterRepository.Distinct(x => new { x.Code, x.Category, x.Name }, null, "Code");

            ProgramInfo program = null;

            foreach (var item in data)
            {
                if (StringHelper.EqualsAny(item.Code, excludedCodes))
                    continue;

                if (program == null || !item.Code.StartsWith(program.Code, StringComparison.OrdinalIgnoreCase))
                    program = programs.Get(item.Code);

                var category = program.GetCategory(item.Category);
                category.AddField(item.Code, item.Name);
            }

            return programs;
        }

        #endregion
    }
}