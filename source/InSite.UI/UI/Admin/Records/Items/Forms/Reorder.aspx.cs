using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Gradebooks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Records;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Items.Forms
{
    public partial class Reorder : AdminBasePage, IHasParentLinkParameters
    {
        private Guid GradebookIdentifier => Guid.TryParse(Request.QueryString["gradebook"], out var id) ? id : Guid.Empty;

        private Guid? ParentItemKey => Guid.TryParse(Request["parent"], out var key) ? key : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ReorderButton.Click += ReorderButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();

                CancelButton.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={GradebookIdentifier}&panel=config";
            }

            base.OnLoad(e);
        }

        private void ReorderButton_Click(object sender, EventArgs e)
        {
            var args = Request.Form["__EVENTARGUMENT"];

            if (!string.IsNullOrEmpty(args))
                Save(args);

            HttpResponseHelper.Redirect(CancelButton.NavigateUrl);
        }

        private void LoadData()
        {
            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            if (gradebook == null || gradebook.Tenant != Organization.Identifier)
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");

            PageHelper.AutoBindHeader(this, null, $"{gradebook.Name}");

            List<GradeItem> children;

            if (ParentItemKey.HasValue)
            {
                var parent = gradebook.FindItem(ParentItemKey.Value);
                if (parent == null)
                    HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");

                children = gradebook.FindItem(ParentItemKey.Value)?.Children;

                Instruction.Text = $"Reorder child items of <b>{parent.Name}</b>.";
            }
            else
            {
                children = gradebook.RootItems;

                Instruction.Text = "Reorder root items</b>.";
            }

            if (children == null || children.Count < 2)
                HttpResponseHelper.Redirect("/ui/admin/records/gradebooks/search");

            var items = children.Select(x => new { Name = x.Name });

            ItemRepeater.DataSource = items;
            ItemRepeater.DataBind();
        }

        private void Save(string args)
        {
            var indexes = args.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var itemIndexes = new List<int>();
            var changed = false;

            for (var i = 0; i < indexes.Length; i++)
            {
                int itemIndex;
                if (!int.TryParse(indexes[i], out itemIndex) || itemIndex < 0 || itemIndex >= indexes.Length)
                    return;

                itemIndexes.Add(itemIndex);

                if (itemIndex != i)
                    changed = true;
            }

            if (!changed)
                return;

            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(GradebookIdentifier);
            var children = ParentItemKey.HasValue ? gradebook.FindItem(ParentItemKey.Value)?.Children : gradebook.RootItems;

            if (children != null && children.Count == itemIndexes.Count)
            {
                var keys = new List<Guid>();
                foreach (var itemIndex in itemIndexes)
                    keys.Add(children[itemIndex].Identifier);

                var command = new ReorderGradeItem(GradebookIdentifier, ParentItemKey, keys.ToArray());
                ServiceLocator.SendCommand(command);

                Course2Store.ClearCache(Organization.Identifier);
            }
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={GradebookIdentifier}&panel=config"
                : null;
        }
    }
}
