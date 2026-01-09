using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class Pagination : BasePagination
    {
        #region Constants

        public const string DefaultDescription = "Page <strong>{0:n0}</strong> of <strong>{1:n0}</strong> - Items <strong>{2:n0}</strong> to <strong>{3:n0}</strong> of <strong>{4:n0}</strong>";

        #endregion

        #region Events

        public class PageChangedEventArgs : EventArgs
        {
            public int OldPage { get; }
            public int NewPage { get; }

            public PageChangedEventArgs(int oldPage, int newPage)
            {
                OldPage = oldPage;
                NewPage = newPage;
            }
        }

        public delegate void PageChangedEventHandler(object sender, PageChangedEventArgs e);

        public event PageChangedEventHandler PageChanged;

        #endregion

        #region Properties

        public int PageSize
        {
            get
            {
                return (int)(ViewState[nameof(PageSize)] ?? 20);
            }
            set
            {
                ViewState[nameof(PageSize)] = Number.CheckRange(value, 1);
                Recalculate();
            }
        }

        public int PageGroupSize
        {
            get
            {
                return (int)(ViewState[nameof(PageGroupSize)] ?? 10);
            }
            set
            {
                ViewState[nameof(PageGroupSize)] = Number.CheckRange(value, 2);
                Recalculate();
            }
        }

        public int PageIndex
        {
            get
            {
                return (int)(ViewState[nameof(PageIndex)] ?? -1);
            }
            set
            {
                ViewState[nameof(PageIndex)] = Number.CheckRange(value, -1);
                Recalculate();
            }
        }

        public int ItemsCount
        {
            get
            {
                return (int)(ViewState[nameof(ItemsCount)] ?? 0);
            }
            set
            {
                ViewState[nameof(ItemsCount)] = Number.CheckRange(value, 0);
                Recalculate();
            }
        }

        public string Description
        {
            get => (string)ViewState[nameof(Description)];
            set => ViewState[nameof(Description)] = value;
        }

        public int PageCount
        {
            get => (int)ViewState[nameof(PageCount)];
            private set => ViewState[nameof(PageCount)] = Number.CheckRange(value, 0);
        }

        public int StartItem => PageIndex == -1 ? 0 : PageIndex * PageSize + 1;

        public int EndItem => PageIndex == -1 ? 0 : Number.CheckRange(PageIndex * PageSize + PageSize, 1, ItemsCount);

        public int ItemsSkip => PageIndex == -1 ? 0 : PageIndex * PageSize;

        public int ItemsTake => PageIndex == -1 ? 0 : PageIndex < PageCount - 1 ? PageSize : ItemsCount - PageIndex * PageSize;

        #endregion

        #region Fields

        private bool _isRecalculate = false;

        #endregion

        #region Construction

        public Pagination()
        {
            Description = DefaultDescription;
        }

        #endregion

        #region Methods

        protected override void OnPageChanged(int page)
        {
            var oldPage = PageIndex;
            var newPage = PageIndex = page - 1;

            if (oldPage != newPage)
                PageChanged?.Invoke(this, new PageChangedEventArgs(oldPage, newPage));
        }

        private void Recalculate()
        {
            if (_isRecalculate)
                return;

            _isRecalculate = true;

            PageCount = (int)Math.Ceiling((decimal)ItemsCount / PageSize);

            if (PageIndex >= PageCount)
                PageIndex = PageCount - 1;

            if (PageCount > 0)
            {
                if (PageIndex == -1)
                    PageIndex = 0;

                ActivePage = PageIndex + 1;
                StartPage = (int)Math.Floor((decimal)PageIndex / PageGroupSize) * PageGroupSize + 1;
                EndPage = Number.CheckRange(StartPage + PageGroupSize - 1, 0, PageCount);

                if (Description.IsNotEmpty())
                {
                    DescriptionHtml = string.Format(
                        Global.Translate(Description),
                        PageIndex + 1,
                        PageCount,
                        StartItem,
                        EndItem,
                        ItemsCount
                    );
                }
            }
            else
            {
                ActivePage = 0;
                StartPage = 0;
                EndPage = 0;
                DescriptionHtml = null;
            }

            ShowPrev = StartPage > PageGroupSize;
            ShowNext = PageCount > EndPage;

            _isRecalculate = false;
        }

        #endregion
    }

    public class GridPagination : BasePagination
    {
        #region Constants

        private const int PageGroupSize = 10;

        #endregion

        #region Classes

        public class Template : ITemplate
        {
            private Page _page = null;

            public void InstantiateIn(Control container)
            {
                if (_page == null)
                    _page = ((Control)HttpContext.Current.CurrentHandler).Page;

                var pagination = new GridPagination();
                pagination.ID = "Pagination";

                container.Controls.Add(pagination);
                container.DataBinding += Container_DataBinding;
            }

            private static void Container_DataBinding(object sender, EventArgs e)
            {
                var container = (Control)sender;
                var grid = (GridView)container.NamingContainer.NamingContainer;
                var pagination = (GridPagination)container.FindControl("Pagination");

                pagination.LoadData(grid);
            }
        }

        #endregion

        #region Methods

        protected override void OnPageChanged(int page)
        {
            RaiseBubbleEvent(
                this,
                new GridViewCommandEventArgs(this, new CommandEventArgs("Page", page.ToString())));
        }

        private void LoadData(GridView grid)
        {
            ActivePage = grid.PageIndex + 1;
            StartPage = (int)Math.Floor((decimal)grid.PageIndex / PageGroupSize) * PageGroupSize + 1;
            EndPage = Number.CheckRange(StartPage + PageGroupSize - 1, 0, grid.PageCount);
            ShowPrev = StartPage > PageGroupSize;
            ShowNext = grid.PageCount > EndPage;

            var startIndex = grid.PageIndex * grid.PageSize;
            var itemsCount = grid.AllowCustomPaging ? grid.VirtualItemCount : grid.Rows.Count;

            DescriptionHtml = string.Format(
                Global.Translate(Pagination.DefaultDescription),
                grid.PageIndex + 1,
                grid.PageCount,
                startIndex + 1,
                Number.CheckRange(startIndex + grid.PageSize, 1, itemsCount),
                itemsCount
            );
        }

        #endregion
    }

    public abstract class BasePagination : BaseControl, IPostBackDataHandler
    {
        #region Events

        protected abstract void OnPageChanged(int page);

        #endregion

        #region Properties

        protected int StartPage
        {
            get => (int)(ViewState[nameof(StartPage)] ?? 0);
            set => ViewState[nameof(StartPage)] = value;
        }

        protected int EndPage
        {
            get => (int)(ViewState[nameof(EndPage)] ?? 0);
            set => ViewState[nameof(EndPage)] = value;
        }

        protected int ActivePage
        {
            get => (int)(ViewState[nameof(ActivePage)] ?? 0);
            set => ViewState[nameof(ActivePage)] = value;
        }

        protected bool ShowPrev
        {
            get => (bool)(ViewState[nameof(ShowPrev)] ?? false);
            set => ViewState[nameof(ShowPrev)] = value;
        }

        protected bool ShowNext
        {
            get => (bool)(ViewState[nameof(ShowNext)] ?? false);
            set => ViewState[nameof(ShowNext)] = value;
        }

        protected string DescriptionHtml
        {
            get => (string)ViewState[nameof(DescriptionHtml)];
            set => ViewState[nameof(DescriptionHtml)] = value;
        }

        #endregion

        #region Fields

        private int? _requestPage;

        #endregion

        #region Loading

        protected override void OnPreRender(EventArgs e)
        {
            var postBackCode = Page.ClientScript.GetPostBackEventReference(
                new PostBackOptions(this) { AutoPostBack = true }, true);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Pagination),
                "init_" + ClientID,
                $@"
if ($('#{ClientID}').data('inited') !== true) {{
    $('#{ClientID} a[data-page]').on('click', function (e) {{
        e.preventDefault();
        document.getElementsByName('{UniqueID}')[0].value = this.dataset.page;
        {postBackCode};
    }});
    $('#{ClientID}').data('inited', true);
}}",
                true);

            base.OnPreRender(e);
        }

        #endregion

        #region IPostBackEventHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) =>
            LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent() => RaisePostDataChangedEvent();

        private bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            Page.ClientScript.ValidateEvent(postDataKey);

            var value = postCollection[postDataKey];
            if (value != null && int.TryParse(value, out var page))
            {
                _requestPage = page;

                return true;
            }

            return false;
        }

        private void RaisePostDataChangedEvent()
        {
            if (!Page.IsPostBackEventControlRegistered)
                Page.AutoPostBackControl = this;

            OnPageChanged(_requestPage.Value);
        }

        #endregion

        #region Rendering

        protected override void Render(HtmlTextWriter writer)
        {
            #region nav.insite-pager

            AddAttributesToRender(writer);

            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses("insite-pager", CssClass));
            writer.AddAttribute("aria-label", "Grid Navigation");
            writer.RenderBeginTag("nav");

            #region ul.pagination

            if (EndPage > 1)
                RenderPagination(writer);
            else
                writer.Write("&nbsp;");

            #endregion

            #region div.insite-pager-info

            if (DescriptionHtml.IsNotEmpty())
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "insite-pager-info");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write(DescriptionHtml);
                writer.RenderEndTag();
            }

            #endregion

            #region div.clearfix

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "clearfix");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderEndTag();

            #endregion

            #region input[type="hidden"]

            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();

            #endregion

            writer.RenderEndTag();

            #endregion
        }

        private void RenderPagination(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "pagination");
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            if (ShowPrev)
                RenderLi(true, StartPage - 1, "...", "Previous Pages", null);

            if (StartPage > 0)
            {
                var start = StartPage;
                var end = EndPage;
                var active = ActivePage;
                for (var n = start; n <= end; n++)
                {
                    if (n != active)
                    {
                        RenderLi(
                            true, n, "{0:n0}", "Go To Page #{0:n0}",
                            n != active - 1 && n != active + 1
                                ? "d-none d-sm-block"
                                : null);
                    }
                    else
                    {
                        RenderLi(false, n, "{0:n0}<span class=\"visually-hidden\">(current)</span>", "Current Page", "active");
                    }
                }
            }

            if (ShowNext)
                RenderLi(true, EndPage + 1, "...", "Next Pages", null);

            writer.RenderEndTag();

            void RenderLi(bool anchor, int page, string text, string tooltip, string @class)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, ControlHelper.MergeCssClasses("page-item", @class));
                writer.RenderBeginTag(HtmlTextWriterTag.Li);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "page-link");

                if (tooltip.IsNotEmpty())
                    writer.AddAttribute(HtmlTextWriterAttribute.Title, string.Format(tooltip, page));

                var value = string.Format(text, page);

                if (anchor)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, "#page_" + page);
                    writer.AddAttribute("data-page", page.ToString());
                    writer.RenderBeginTag(HtmlTextWriterTag.A);
                    writer.Write(value);
                    writer.RenderEndTag();
                }
                else
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Span);
                    writer.Write(value);
                    writer.RenderEndTag();
                }

                writer.RenderEndTag();
            }
        }

        #endregion
    }
}