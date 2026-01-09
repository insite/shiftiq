using System;
using System.Web;

using InSite.Application.Quizzes.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Contract;

namespace InSite.UI.Admin.Assessments.Quizzes
{
    public partial class Search : SearchPage<TQuizFilter>
    {
        public const string NavigateUrl = "/ui/admin/assessment/quizzes/search";

        public static string GetNavigateUrl(string type = null)
        {
            var url = NavigateUrl;

            if (type.IsNotEmpty())
                type += "?type=" + HttpUtility.UrlEncode(type);

            return url;
        }

        public static void Redirect(string type = null) => HttpResponseHelper.Redirect(GetNavigateUrl(type));

        public override string EntityName => "Quiz";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageHelper.AutoBindHeader(this, new BreadcrumbItem(
                "Add New Quiz",
                Create.GetNavigateUrl(Request.QueryString["type"]),
                null, null));
        }
    }
}