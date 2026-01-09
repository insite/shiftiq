using System;
using System.Web.UI;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Learning.Categories
{
    public partial class Create : AdminBasePage
    {
        public const string NavigateUrl = "/ui/admin/learning/categories/create";

        public static void Redirect() => HttpResponseHelper.Redirect(NavigateUrl);

        private Guid? CollectionIdentifier
        {
            get => (Guid?)ViewState[nameof(CollectionIdentifier)];
            set => ViewState[nameof(CollectionIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CancelButton.NavigateUrl = Search.NavigateUrl;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                Search.Redirect();

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One);

            CollectionIdentifier = Edit.GetCollectionId();
            if (!CollectionIdentifier.HasValue)
                Search.Redirect();

            CategoryDetail.SetDefaultInputValues(CollectionIdentifier.Value);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!CanEdit || !Page.IsValid)
                return;

            var item = new TCollectionItem();

            CategoryDetail.GetInputValues(item);

            item.CollectionIdentifier = CollectionIdentifier.Value;
            item.ItemIdentifier = UniqueIdentifier.Create();
            item.ItemSequence = TCollectionItemSearch.GetNextSequence(CollectionIdentifier.Value, item.OrganizationIdentifier.Value);

            TCollectionItemStore.Insert(item);

            TCollectionItemCache.Refresh();

            Edit.Redirect(item.ItemIdentifier);
        }
    }
}