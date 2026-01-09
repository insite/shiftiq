using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Admin.Assessments.Forms.Utilities;

using Shift.Common;

namespace InSite.Admin.Assessments.Forms.Controls
{
    public partial class FormAddendumDetails : BaseUserControl
    {
        private class GroupInfo
        {
            public string GroupTitle { get; set; }
            public AddendumHelper.RepeaterDataItem[] Items { get; set; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FooterLiteral.ContentKey = GetType().ToString();

            GroupRepeater.ItemDataBound += GroupRepeater_ItemDataBound;
        }

        public void SetInputValues(Form form, bool canWrite)
        {
            SetupEditLinks(form, canWrite);

            var attachmentMapping = form.Specification.Bank
                .EnumerateAllAttachments()
                .ToDictionary(x => (x.Asset, x.AssetVersion), x => x);
            var addendum = form.Addendum;
            var dataSource = AddendumHelper.GetRepeaterDataSource(
                GetAttachments(addendum.Acronyms),
                GetAttachments(addendum.Formulas),
                GetAttachments(addendum.Figures));
            var groups = new List<GroupInfo>();

            if (dataSource[0].Length > 0)
                groups.Add(new GroupInfo
                {
                    GroupTitle = "Acronyms",
                    Items = dataSource[0]
                });

            if (dataSource[1].Length > 0)
                groups.Add(new GroupInfo
                {
                    GroupTitle = "Formulas",
                    Items = dataSource[1]
                });

            if (dataSource[2].Length > 0)
                groups.Add(new GroupInfo
                {
                    GroupTitle = "Figures",
                    Items = dataSource[2]
                });

            RepeaterPanel.Visible = groups.Count > 0;

            GroupRepeater.DataSource = groups;
            GroupRepeater.DataBind();

            Attachment[] GetAttachments(IEnumerable<FormAddendumItem> value) => value
                .Select(x => (x.Asset, x.Version))
                .Distinct()
                .Where(x => attachmentMapping.ContainsKey(x))
                .Select(x => attachmentMapping[x])
                .ToArray();
        }

        private void GroupRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var groupInfo = (GroupInfo)e.Item.DataItem;

            var itemRepeater = (Repeater)e.Item.FindControl("ItemRepeater");
            itemRepeater.DataSource = groupInfo.Items;
            itemRepeater.DataBind();
        }

        private void SetupEditLinks(Form form, bool canWrite)
        {
            var queryString = $"bank={form.Specification.Bank.Identifier}&form={form.Identifier}";

            EditLink.NavigateUrl = $"/ui/admin/assessments/forms/change-addendum?{queryString}";
            EditLink.Visible = canWrite && form.Specification.Bank.Attachments.IsNotEmpty();
        }
    }
}