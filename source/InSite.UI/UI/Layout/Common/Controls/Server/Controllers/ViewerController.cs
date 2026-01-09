using System;

using InSite.UI.Layout.Admin;

namespace InSite.Common.Web.UI
{
    public abstract class ViewerController : AdminBasePage
    {
        protected virtual string GetString(Guid? val)
        {
            return val.HasValue ? val.Value.ToString() : "-";
        }

        protected virtual string GetString(int? val)
        {
            return val.HasValue ? val.Value.ToString() : "-";
        }

        protected virtual string GetString(bool val)
        {
            return val ? "yes" : "no";
        }

        protected virtual string GetString(string str)
        {
            if (!string.IsNullOrEmpty(str)) return str;

            return "-";
        }

        protected virtual string GetDateString(DateTime date)
        {
            return date.ToString("MMM d, yyyy");
        }

        protected virtual string GetDateString(DateTime? date)
        {
            if (date.HasValue) return GetDateString(date.Value);

            return "-";
        }
    }
}