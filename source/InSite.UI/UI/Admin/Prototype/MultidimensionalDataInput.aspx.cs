using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Prototypes
{
    public partial class MultidimensionalDataInput : AdminBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            AddDimensionButton.Click += (x, y) => AddDimension();
            ApplyDimensionsButton.Click += (x, y) => ApplyDimensions();
            DimensionRepeater.DataBinding += DimensionRepeater_DataBinding;
            DimensionRepeater.ItemCommand += DimensionRepeater_ItemCommand;

            MappingRepeater.DataBinding += MappingRepeater_DataBinding;
            MappingRepeater.ItemCreated += MappingRepeater_ItemCreated;
            MappingRepeater.ItemDataBound += MappingRepeater_ItemDataBound;
            ApplyMappingButton.Click += (x, y) => ApplyMapping();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            if (Organization.Code != "demo")
                HttpResponseHelper.SendHttp404();

            PageHelper.AutoBindHeader(this);

            InitDimensions();
            InitMapping();
            ApplyMapping();
        }

        #region Dimensions

        [Serializable]
        private class DimensionItem
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public string[] Values { get; set; }
        }

        private List<DimensionItem> Dimensions
        {
            get => (List<DimensionItem>)ViewState[nameof(Dimensions)];
            set => ViewState[nameof(Dimensions)] = value;
        }

        private void InitDimensions()
        {
            Dimensions = new List<DimensionItem>
            {
                new DimensionItem
                {
                    Index = 0,
                    Name = "Lifespan",
                    Values = new[] { "Children 0-14", "Youth 15-24", "Adult 25-64", "Seniors 65-84", "Older seniors 85+" }
                },
                new DimensionItem
                {
                    Index = 1,
                    Name = "Occupation",
                    Values = new[] { "HCA", "LPN", "RN" }
                },
                new DimensionItem
                {
                    Index = 2,
                    Name = "Type of Care",
                    Values = new[] { "Medical care", "Post-op care", "Public health", "Surgical Care" }
                },
                new DimensionItem
                {
                    Index = 3,
                    Name = "Topic",
                    Values = new[] { "Arthritis", "Cancer", "Cerebrovascular accident", "Cerebral palsy", "COPD", "Depression", "Diabetes", "Osteoporosis", "Anxiety", "Hysterectomy" }
                }
            };

            DimensionRepeater.DataBind();
        }

        private void ReadDimensions()
        {
            Dimensions = new List<DimensionItem>();

            foreach (RepeaterItem item in DimensionRepeater.Items)
            {
                if (!IsContentItem(item))
                    continue;

                var name = (ITextControl)item.FindControl("Name");
                var values = (ITextControl)item.FindControl("Values");

                Dimensions.Add(new DimensionItem
                {
                    Index = Dimensions.Count,
                    Name = name.Text.Trim(),
                    Values = values.Text.Split('\r', '\n').Select(x => x.Trim()).Where(x => x.IsNotEmpty()).ToArray()
                });
            }
        }

        private void AddDimension()
        {
            ReadDimensions();
            Dimensions.Add(new DimensionItem());
            DimensionRepeater.DataBind();
        }

        private void ApplyDimensions()
        {
            ReadDimensions();

            if (Dimensions.Count == 0)
            {
                StatusAlert.AddMessage(AlertType.Error, "You must add at least one dimension.");
                return;
            }

            if (Dimensions.Any(x => x.Name.IsEmpty()))
            {
                StatusAlert.AddMessage(AlertType.Error, "Dimension Name is required field.");
                return;
            }

            if (Dimensions.Any(x => x.Values.IsEmpty()))
            {
                StatusAlert.AddMessage(AlertType.Error, "Dimension Values is required field.");
                return;
            }

            MappingRepeater.DataBind();
        }

        private void DimensionRepeater_DataBinding(object sender, EventArgs e)
        {
            DimensionsHeader.InnerHtml = $"Dimensions <span class='text-body-secondary'>({Dimensions.Count})</span>";
            DimensionRepeater.DataSource = Dimensions;
        }

        private void DimensionRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            ReadDimensions();
            Dimensions.RemoveAt(e.Item.ItemIndex);
            DimensionRepeater.DataBind();
        }

        protected string GetDimensionValues()
        {
            var dataItem = (DimensionItem)Page.GetDataItem();
            return dataItem.Values.IsEmpty() ? string.Empty : string.Join("\r\n", dataItem.Values);
        }

        #endregion

        #region Mapping

        private enum MappingItemType
        {
            Tab,
            Column,
            Row,
            Value
        }

        [Serializable]
        private class MappingItem
        {
            public MappingItemType Type { get; set; }
            public DimensionItem Dimension { get; set; }
        }

        private bool IsMappingChanged
        {
            get => (bool)(ViewState[nameof(IsMappingChanged)] ?? false);
            set => ViewState[nameof(IsMappingChanged)] = value;
        }

        private List<MappingItem> Mapping
        {
            get => (List<MappingItem>)ViewState[nameof(Mapping)];
            set => ViewState[nameof(Mapping)] = value;
        }

        private void InitMapping()
        {
            MappingRepeater.DataBind();
            SetMappingDefaultValues();
        }

        private void SetMappingDefaultValues()
        {
            var value = 0;

            foreach (RepeaterItem item in MappingRepeater.Items)
            {
                if (!IsContentItem(item))
                    continue;

                var dimension = (ComboBox)item.FindControl("Dimension");
                dimension.ValueAsInt = value++;
                OnDimensionValueChanged(dimension, false, null);
            }
        }

        private void ReadMapping()
        {
            Mapping = new List<MappingItem>();

            foreach (RepeaterItem item in MappingRepeater.Items)
            {
                if (!IsContentItem(item))
                    continue;

                var type = (ITextControl)item.FindControl("Type");
                var dimension = (ComboBox)item.FindControl("Dimension");

                Mapping.Add(new MappingItem
                {
                    Type = type.Text.ToEnum<MappingItemType>(),
                    Dimension = dimension.ValueAsInt.HasValue ? Dimensions[dimension.ValueAsInt.Value] : null,
                });
            }
        }

        private void MappingRepeater_DataBinding(object sender, EventArgs e)
        {
            IsMappingChanged = true;

            var comboBoxItems = Dimensions
                .Select((x, i) => new Shift.Common.ListItem
                {
                    Text = x.Name,
                    Value = i.ToString()
                })
                .ToArray();

            MappingRepeater.DataSource = Enumerable.Range(0, Dimensions.Count)
                .Select((x, i) =>
                {
                    var type = MappingItemType.Tab.GetName();
                    switch (i)
                    {
                        case 0: type = MappingItemType.Value.GetName(); break;
                        case 1: type = MappingItemType.Row.GetName(); break;
                        case 2: type = MappingItemType.Column.GetName(); break;
                    }

                    return new
                    {
                        Type = type,
                        Items = comboBoxItems
                    };
                })
                .Reverse();
        }

        private void MappingRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dimension = (ComboBox)e.Item.FindControl("Dimension");
            dimension.AutoPostBack = true;
            dimension.ValueChanged += (x, y) => OnDimensionValueChanged((ComboBox)x, false, y.OldValue);
        }

        private void MappingRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dimension = (ComboBox)e.Item.FindControl("Dimension");
            dimension.LoadItems((IEnumerable<Shift.Common.ListItem>)DataBinder.Eval(e.Item.DataItem, "Items"));
        }

        private void OnDimensionValueChanged(ComboBox comboBox, bool disableOption, string beforeValue)
        {
            var currentItem = (RepeaterItem)comboBox.NamingContainer;
            var currentValue = comboBox.Value;
            var hasCurrentValue = currentValue.IsNotEmpty();

            foreach (RepeaterItem item in MappingRepeater.Items)
            {
                if (!IsContentItem(item) || item == currentItem)
                    continue;

                var dimension = (ComboBox)item.FindControl("Dimension");
                if (hasCurrentValue && dimension.Value == currentValue)
                    dimension.Value = null;

                if (disableOption)
                {
                    var searchValue = hasCurrentValue ? currentValue : beforeValue;
                    var toggleValue = !hasCurrentValue;

                    foreach (var option in dimension.FlattenOptions())
                    {
                        if (option.Value == searchValue)
                            option.Enabled = toggleValue;
                    }
                }
            }
        }

        private void ApplyMapping()
        {
            ReadMapping();

            if (Mapping.Any(x => x.Dimension == null))
            {
                StatusAlert.AddMessage(AlertType.Error, "For all mapping items, the dimension must be selected.");
                return;
            }

            MappingInput.Value = JsonConvert.SerializeObject(Mapping);

            if (IsMappingChanged)
            {
                DataInput.Value = null;
                IsMappingChanged = false;
            }
        }

        #endregion
    }
}