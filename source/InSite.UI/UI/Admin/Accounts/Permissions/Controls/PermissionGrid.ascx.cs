using System;
using System.ComponentModel;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Accounts.Permissions.Controls
{
    public partial class PermissionGrid : SearchResultsGridViewController<TGroupActionFilter>
    {
        #region Events

        public event EventHandler Refreshed;

        private void OnRefreshed() => Refreshed?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        protected override bool IsFinder => false;

        protected bool CanWrite => CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Accounts, PermissionOperation.Write);

        protected bool CanDelete => CurrentSessionState.Identity.IsGranted(PermissionIdentifiers.Admin_Accounts, PermissionOperation.Write);

        protected string JsManagerName => $"{ClientID}_manager";

        private string ReturnParams
        {
            get => (string)ViewState[nameof(ReturnParams)];
            set => ViewState[nameof(ReturnParams)] = value;
        }

        #endregion

        #region Fields

        private ReturnUrl _returnUrl;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RefreshButton.Click += RefreshButton_Click;
            DeletePermissionButton.Click += DeletePermissionButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            AddButton.Visible = CanWrite;
        }

        #endregion

        #region Event handlers

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            SearchWithCurrentPageIndex(Filter);

            OnRefreshed();
        }

        private void DeletePermissionButton_Click(object sender, EventArgs e)
        {
            if (!CanDelete)
                return;

            var actionId = Guid.Parse(DeleteIdentifier.Value);
            var groupId = Filter.GroupIdentifier.Value;

            TGroupPermissionStore.Delete(groupId, actionId);

            SearchWithCurrentPageIndex(Filter);

            OnRefreshed();
        }

        #endregion

        #region Public methods

        public void LoadData(Guid group, string returnParams)
        {
            ReturnParams = returnParams;

            AddButton.NavigateUrl = GetRedirectUrl("/ui/admin/accounts/permissions/create?group={0}", group);

            var filter = new TGroupActionFilter
            {
                GroupIdentifier = group
            };

            Search(filter);
        }

        #endregion

        #region Search results

        protected override int SelectCount(TGroupActionFilter filter)
        {
            return TGroupPermissionSearch.Count(filter);
        }

        protected override IListSource SelectData(TGroupActionFilter filter)
        {
            return TGroupPermissionSearch.Select(filter);
        }

        #endregion

        #region Helpers

        protected string GetRedirectUrl(string format, params object[] args)
        {
            var url = string.Format(format, args);

            if (ReturnParams == null)
                return url;

            if (_returnUrl == null)
                _returnUrl = new ReturnUrl(ReturnParams);

            return _returnUrl.GetRedirectUrl(url);
        }

        #endregion
    }
}