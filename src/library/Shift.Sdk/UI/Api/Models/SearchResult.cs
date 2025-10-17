using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SearchResult<TModel>
    {
        #region Properties

        [JsonProperty(PropertyName = "data")]
        public IReadOnlyList<TModel> Models { get; set; }

        [JsonProperty(PropertyName = "pageNumber")]
        public int PageNumber
        {
            get
            {
                var result = _pageNumber > PageCount ? PageCount : _pageNumber;

                return result <= 0 ? 1 : result;
            }
            set => _pageNumber = value;
        }

        public int RecordCount
        {
            get => _recordCount;
            set => _recordCount = value < 0 ? 0 : value;
        }

        public byte PageSize { get; set; } = 10;

        [JsonProperty(PropertyName = "pageCount")]
        public short PageCount => (short)Math.Ceiling((decimal)RecordCount / PageSize);

        #endregion

        #region Fields

        private int _pageNumber = 1;
        private int _recordCount = 0;

        #endregion

        #region Construction

        public SearchResult(int page)
        {
            PageNumber = page;
        }

        #endregion
    }
}