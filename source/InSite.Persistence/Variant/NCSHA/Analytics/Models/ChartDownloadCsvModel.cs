namespace InSite.Persistence.Plugin.NCSHA
{
    public class ChartDownloadCsvModel
    {
        #region Classes

        public class Dataset
        {
            public string Title { get; set; }
            public DatasetItem[] Items { get; set; }
        }

        public class DatasetItem
        {
            public int Year { get; set; }
            public decimal? Value { get; set; }
        }

        #endregion

        public Dataset[] Datasets { get; set; }
    }
}