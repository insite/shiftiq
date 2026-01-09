using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FindEntity : BaseFindEntity<FindEntity.DataFilter>
    {
        #region Events

        public class CountArgs : EventArgs
        {
            public int? Count { get; set; }
        }

        public delegate void CountHandler(object sender, CountArgs args);

        public event CountHandler NeedDataCount;

        private CountArgs OnNeedDataCount()
        {
            var args = new CountArgs();

            NeedDataCount?.Invoke(this, args);

            return args;
        }

        public class DataArgs : EventArgs
        {
            public string Keyword { get; }

            public Paging Paging { get; set; }

            public DataItem[] Items { get; set; }

            public DataArgs(string keyword, Paging paging)
            {
                Keyword = keyword;
                Paging = paging;
            }
        }

        public delegate void DataHandler(object sender, DataArgs args);

        public event DataHandler NeedDataSource;

        private DataArgs OnNeedDataSource(string keyword, Paging paging)
        {
            var args = new DataArgs(keyword, paging);

            NeedDataSource?.Invoke(this, args);

            return args;
        }

        public class ItemsArgs : EventArgs
        {
            public Guid[] Identifiers { get; }

            public DataItem[] Items { get; set; }

            public ItemsArgs(Guid[] ids)
            {
                Identifiers = ids;
            }
        }

        public delegate void ItemsHandler(object sender, ItemsArgs args);

        public event ItemsHandler NeedSelectedItems;

        private ItemsArgs OnNeedSelectedItems(Guid[] ids)
        {
            var args = new ItemsArgs(ids);

            NeedSelectedItems?.Invoke(this, args);

            return args;
        }

        #endregion

        #region Classes

        public class DataFilter : Filter
        {
            public string Keyword { get; set; }
        }

        #endregion

        #region Properties

        public string EntityName
        {
            get => (string)(ViewState[nameof(EntityName)] ?? "Item");
            set => ViewState[nameof(EntityName)] = value.NullIfEmpty();
        }

        public string EditorUrl
        {
            get => (string)ViewState[nameof(EditorUrl)];
            set => ViewState[nameof(EditorUrl)] = value.NullIfEmpty();
        }

        #endregion

        protected override string GetEntityName() => EntityName;

        protected override string GetEditorUrl() => EditorUrl;

        protected override DataFilter GetFilter(string keyword)
        {
            return new DataFilter { Keyword = keyword };
        }

        protected override int Count(DataFilter filter)
        {
            var args = OnNeedDataCount();

            if (!args.Count.HasValue || args.Count.Value < 0)
                throw ApplicationError.Create("Can't get a count of items");

            return args.Count.Value;
        }

        protected override DataItem[] Select(DataFilter filter)
        {
            var args = OnNeedDataSource(filter.Keyword, filter.Paging);

            if (args.Items == null)
                throw ApplicationError.Create("Can't get a data source");

            return args.Items;
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            var args = OnNeedSelectedItems(ids);

            if (args.Items == null)
                throw ApplicationError.Create("Can't get the selected items");

            return args.Items;
        }
    }
}