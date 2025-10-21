using System;

using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Constant;
using Shift.Contract;

namespace InSite.UI.Admin.Messages.Emails
{
    public partial class Search : SearchPage<EmailFilter>
    {
        public override string EntityName => "Email";

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!IsQueryStringValid(Request.QueryString, null, null, SearchAlert))
                DisableForm();

            else if (!IsPostBack)
                LoadSearchedResults();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchResults.OnError += error => SearchAlert.AddMessage(AlertType.Error, error);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.BindHeader(this, new BreadcrumbItem[] {
                    new BreadcrumbItem("Messages", "/ui/admin/messages/home", null, null),
                    new BreadcrumbItem("Emails", null, null, null),
                    new BreadcrumbItem("Search", null, null, null)
                });
            }
        }
    }
}