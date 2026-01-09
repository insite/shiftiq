using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Pages.Write;
using InSite.Application.Sites.Read;
using InSite.Common.Web.UI;

using Shift.Common.Events;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Sites.Pages.Controls
{
    public partial class RelationshipList : BaseUserControl
    {
        #region Classes

        protected class DataItem
        {
            public Guid Identifier { get; set; }
            public int Sequence { get; set; }
            public string Title { get; set; }
            public string Slug { get; set; }
            public string Type { get; set; }
            public string Control { get; set; }
            public Guid OrganizationIdentifier { get; set; }
            public string EditUrl { get; set; }

            public static readonly Expression<Func<QPage, DataItem>> PageBinder = LinqExtensions1.Expr((QPage x) => new DataItem
            {
                Identifier = x.PageIdentifier,
                Sequence = x.Sequence,
                Title = x.Title,
                Slug = x.PageSlug,
                Type = x.PageType,
                Control = x.ContentControl,
                OrganizationIdentifier = x.OrganizationIdentifier,
                EditUrl = "/ui/admin/sites/pages/outline?id="
            });

            public static readonly Expression<Func<QSite, DataItem>> SiteBinder = LinqExtensions1.Expr((QSite x) => new DataItem
            {
                Identifier = x.SiteIdentifier,
                Title = x.SiteTitle,
                OrganizationIdentifier = x.OrganizationIdentifier,
                EditUrl = "/ui/admin/sites/outline?id="
            });
        }

        #endregion

        #region Events

        public event EventHandler Refreshed;

        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        private Guid? SiteId
        {
            get => (Guid?)ViewState[nameof(SiteId)];
            set => ViewState[nameof(SiteId)] = value;
        }

        private Guid? ParentPageId
        {
            get => (Guid?)ViewState[nameof(ParentPageId)];
            set => ViewState[nameof(ParentPageId)] = value;
        }

        private Guid? PageId
        {
            get => (Guid?)ViewState[nameof(PageId)];
            set => ViewState[nameof(PageId)] = value;
        }

        public int ItemCount
        {
            get => (int)(ViewState[nameof(ItemCount)] ?? 0);
            set => ViewState[nameof(ItemCount)] = value;
        }

        protected ConnectionDirection Direction
        {
            get => (ConnectionDirection)ViewState[nameof(Direction)];
            set => ViewState[nameof(Direction)] = value;
        }

        public bool EnableAJAX
        {
            get => (bool)(ViewState[nameof(EnableAJAX)] ?? true);
            set => ViewState[nameof(EnableAJAX)] = value;
        }

        public bool EnableSort
        {
            get => (bool)ViewState[nameof(EnableSort)];
            set => ViewState[nameof(EnableSort)] = value;
        }

        private List<Guid> DataIds
        {
            get => (List<Guid>)ViewState[nameof(DataIds)];
            set => ViewState[nameof(DataIds)] = value;
        }

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RefreshButton.Click += RefreshButton_Click;

            DataRepeater.ItemDataBound += DataRepeater_ItemDataBound;

            UpdatePanel.Request += UpdatePanel_Request;

            ReorderButton.Click += ReorderButton_Click;

            HeaderContent.ContentKey = typeof(RelationshipList).FullName;
        }

        #endregion

        #region Loading

        public void LoadData(QSite entity, ConnectionDirection direction)
        {
            SiteId = entity.SiteIdentifier;
            PageId = null;
            ParentPageId = null;

            AddResourceLink.NavigateUrl = $"/ui/admin/sites/pages/create?site={entity.SiteIdentifier}";

            LoadData(direction);
        }

        public void LoadData(QPage entity, ConnectionDirection direction)
        {
            SiteId = entity.SiteIdentifier;
            PageId = entity.PageIdentifier;
            ParentPageId = entity.ParentPageIdentifier;

            AddResourceLink.NavigateUrl = $"/ui/admin/sites/pages/create?parent={entity.PageIdentifier}";

            LoadData(direction);
        }

        private void LoadData(ConnectionDirection direction)
        {
            Direction = direction;

            BindRepeater();

            CommandButtons2.Visible = direction == ConnectionDirection.Outgoing;
            ReorderCommandButtons.Visible = direction == ConnectionDirection.Outgoing;

            ReorderButton.OnClientClick = $"inSite.common.gridReorderHelper.startReorder('{UniqueID}'); return false;";
        }

        protected override void OnPreRender(EventArgs e)
        {
            UpdatePanel.ChildrenAsTriggers = EnableAJAX;

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(RelationshipList),
                $"register_reorder_{UniqueID}",
                $@"
                inSite.common.gridReorderHelper.registerReorder({{
                    id:'{UniqueID}',
                    selector:'#{SectionControl.ClientID} div.res-cont-list > table:first',
                    items:'tbody > tr',
                    {(EnableAJAX ? $"updatePanelId:'{UpdatePanel.ClientID}'" : $"callbackControlId:'{ReorderButton.UniqueID}'")},
                }});", true);

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void DataRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            DataIds.Add((Guid)DataBinder.Eval(e.Item.DataItem, "Identifier"));
        }

        private void ReorderButton_Click(object sender, EventArgs e)
        {
            // EnableSort = true;
            OnReorderCommand(Request.Form["__EVENTARGUMENT"]);
        }

        private void UpdatePanel_Request(object sender, StringValueArgs e)
        {
            OnReorderCommand(e.Value);
        }

        public void OnReorderCommand(string argument)
        {
            if (string.IsNullOrEmpty(argument) || Direction != ConnectionDirection.Outgoing)
                return;

            var args = argument.Split('&');
            var command = args[0];
            var value = args.Length > 1 ? args[1] : null;

            if (command == "cancel-reorder")
            {
                //EnableSort = false;
            }
            else if (command == "save-reorder")
            {
                var changes = JavaScriptHelper.GridReorder.Parse(value);
                var reorder = new Guid[DataIds.Count];

                for (int i = 0; i < DataIds.Count; i++)
                    reorder[i] = DataIds[i];

                foreach (var c in changes)
                    reorder[c.Destination.ItemIndex] = DataIds[c.Source.ItemIndex];

                if (PageId.HasValue)
                {
                    var toReorder = ServiceLocator.PageSearch.GetReorderByResourceId(PageId.Value, reorder);
                    Reorder(toReorder);
                }
                else if (SiteId.HasValue)
                {
                    var toReorder = ServiceLocator.PageSearch.GetReorderBySiteId(SiteId.Value, reorder);
                    Reorder(toReorder);
                }

                BindRepeater();
                //EnableSort = false;
            }
            OnRefreshed();
        }

        private static void Reorder(List<QPage> toReorder)
        {
            var script = new List<ICommand>();
            foreach (var item in toReorder)
                script.Add(new ChangePageSequence(item.PageIdentifier, item.Sequence));

            foreach (var command in script)
                ServiceLocator.SendCommand(command);
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            BindRepeater();
            OnRefreshed();
        }

        #endregion

        #region Helper methods

        private void BindRepeater()
        {
            DataIds = new List<Guid>();

            var containments = GetContainments();

            ItemCount = containments.Length;
            DataRepeater.Visible = containments.Length > 0;
            DataRepeater.DataSource = containments;
            DataRepeater.DataBind();
        }

        private DataItem[] GetContainments()
        {
            if (Direction == ConnectionDirection.Incoming)
            {
                var items = ServiceLocator.PageSearch.Bind(
                        DataItem.PageBinder,
                        x => x.PageIdentifier == ParentPageId.Value,
                        "Sequence");

                return PageId.HasValue
                    ? items
                    : !ParentPageId.HasValue && SiteId.HasValue
                        ? ServiceLocator.SiteSearch.Bind(DataItem.SiteBinder, x => x.SiteIdentifier == SiteId.Value)
                        : new DataItem[0];
            }
            else if (Direction == ConnectionDirection.Outgoing)
            {
                return PageId.HasValue
                    ? ServiceLocator.PageSearch.Bind(DataItem.PageBinder, x => x.ParentPageIdentifier == PageId.Value, "Sequence")
                    : ServiceLocator.PageSearch.Bind(DataItem.PageBinder, x => x.SiteIdentifier == SiteId && !x.ParentPageIdentifier.HasValue, "Sequence");
            }
            else
                throw new NotImplementedException();
        }

        #endregion
    }
}