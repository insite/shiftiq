using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class Grid : GridView
    {
        [Flags]
        public enum TranslationFlag
        {
            None = 0,
            Header = 1
        }

        private IHasTranslator _translator = null;

        public bool EnablePaging
        {
            get => AllowCustomPaging && AllowPaging;
            set
            {
                AllowCustomPaging = value;
                AllowPaging = value;
            }
        }

        public bool EnableSorting
        {
            get => AllowSorting;
            set => AllowSorting = value;
        }

        public bool AutoBinding
        {
            get => (bool)(ViewState[nameof(AutoBinding)] ?? true);
            set => ViewState[nameof(AutoBinding)] = value;
        }

        private bool IsDataBound
        {
            get => (bool)(ViewState[nameof(IsDataBound)] ?? false);
            set => ViewState[nameof(IsDataBound)] = value;
        }

        public TranslationFlag Translation
        {
            get => (TranslationFlag)(ViewState[nameof(Translation)] ?? TranslationFlag.None);
            set => ViewState[nameof(Translation)] = value;
        }

        public Paging Paging => VirtualItemCount == 0 ? Paging.SetSkipTake(0, 0) : Paging.SetPage(PageIndex + 1, PageSize);

        public Grid()
        {
            AutoGenerateColumns = false;
            EnablePaging = true;
            EnableSorting = false;
            GridLines = GridLines.None;
            CellSpacing = -1;
            CssClass = "table table-striped";

            PagerTemplate = new GridPagination.Template();
            PagerSettings.Mode = PagerButtons.Numeric;
            PagerSettings.Position = PagerPosition.Bottom;
            PageSize = 20;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // If the GridView fires the RowDeleting event, we get an unhandled System.Web.HttpException when it is not explicitly handled. By
            // default, we can ignore this and allow grid containers to decide if/how to handle RowDeleting events raised by child controls.

            RowDeleting += (x, y) => { };
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (AutoBinding && !IsDataBound)
                DataBind();

            if (HeaderRow != null)
                HeaderRow.TableSection = TableRowSection.TableHeader;

            if (FooterRow != null)
                FooterRow.TableSection = TableRowSection.TableFooter;

            if (TopPagerRow != null)
                TopPagerRow.TableSection = TableRowSection.TableHeader;

            if (BottomPagerRow != null)
                BottomPagerRow.TableSection = TableRowSection.TableFooter;

            base.OnPreRender(e);
        }

        protected override void PrepareControlHierarchy()
        {
            base.PrepareControlHierarchy();

            if (Controls.Count == 0)
                return;

            var table = Controls[0] as Table;
            if (table == null)
                return;

            foreach (GridViewRow row in table.Rows)
            {
                if (row.RowType != DataControlRowType.Header)
                    continue;

                foreach (TableCell cell in row.Cells)
                {
                    string align;
                    switch (cell.HorizontalAlign)
                    {
                        case HorizontalAlign.Left:
                            align = "start";
                            break;
                        case HorizontalAlign.Right:
                            align = "end";
                            break;
                        case HorizontalAlign.Center:
                            align = "center";
                            break;
                        case HorizontalAlign.Justify:
                            align = "justify";
                            break;
                        default:
                            continue;
                    }

                    cell.Style[HtmlTextWriterStyle.TextAlign] = align;
                }
            }
        }

        protected override void OnPageIndexChanging(GridViewPageEventArgs e)
        {
            if (e.Cancel || !AutoBinding)
            {
                base.OnPageIndexChanging(e);
                return;
            }

            var prevPageIndex = PageIndex;

            PageIndex = e.NewPageIndex;

            DataBind();

            if (DataSource != null)
                return;

            PageIndex = prevPageIndex;

            e.Cancel = true;
        }

        protected override void OnDataBinding(EventArgs e)
        {
            _translator = Translation != TranslationFlag.None ? Page as IHasTranslator : null;

            base.OnDataBinding(e);

            IsDataBound = true;
        }

        protected override void OnRowDataBound(GridViewRowEventArgs e)
        {
            if (_translator != null)
            {
                if (Translation.HasFlag(TranslationFlag.Header) && e.Row.RowType == DataControlRowType.Header)
                    foreach (TableCell cell in e.Row.Cells)
                        cell.Text = _translator.Translator.Translate(cell.Text);
            }

            base.OnRowDataBound(e);
        }
    }
}