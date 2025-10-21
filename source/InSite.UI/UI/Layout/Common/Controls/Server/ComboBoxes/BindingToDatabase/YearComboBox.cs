using System;
using System.Collections.Generic;

using Shift.Common;


namespace InSite.Common.Web.UI
{
    public class YearComboBox : ComboBox
    {
        public int? StartYear
        {
            get => (int?)ViewState[nameof(StartYear)];
            set => ViewState[nameof(StartYear)] = value;
        }

        public List<int> YearList
        {
            get => (List<int>)ViewState[nameof(YearList)];
            set => ViewState[nameof(YearList)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            if (YearList != null && YearList.Count > 0)
            {
                foreach(var year in YearList)
                {
                    list.Add(year.ToString());
                }
            }
            else
            {
                if (!StartYear.HasValue)
                    StartYear = 2005;

                for(int i = StartYear.Value; i <= DateTime.Now.Year; i++)
                {
                    list.Add(i.ToString());
                }
            }
            return list;
        }
    }
}