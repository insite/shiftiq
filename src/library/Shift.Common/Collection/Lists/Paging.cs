using System;

using Newtonsoft.Json;

namespace Shift.Common
{
    [Serializable]
    public class Paging
    {
        public int Skip { get; private set; }
        public int? Take { get; private set; }

        [JsonConstructor]
        private Paging(int skip, int? take)
        {
            Skip = skip;
            Take = take;
        }

        public (int StartRow, int EndRow) ToStartEnd()
        {
            return (Skip + 1, Skip + (Take ?? 0));
        }

        public (int Skip, int Take) ToSkipTake()
        {
            return (Skip, Take ?? 0);
        }

        public static Paging SetSkipTake(int skip, int take)
            => new Paging(skip, take);

        public static Paging SetPage(int? page, int pageSize)
        {
            if (page == null)
                return null;

            if (page <= 0)
                return new Paging(0, 0);

            var skip = (page.Value - 1) * pageSize;

            return new Paging(skip, pageSize);
        }

        public static Paging SetStartEnd(int startRow, int endRow)
        {
            if (startRow == 0 && endRow == 0)
                return new Paging(0, 0);

            var skip = startRow >= 1 ? startRow - 1 : 0;
            var take = endRow != int.MaxValue && startRow < endRow ? endRow - startRow + 1 : (int?)null;

            return new Paging(skip, take);
        }
    }
}
