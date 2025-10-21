using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public abstract class BaseToggleList<T> : ListControl, IRepeatInfoUser, INamingContainer, IPostBackDataHandler where T : BaseToggle
    {
        #region Classes

        public class ChangedArgs : EventArgs
        {
            public int ChangedItemIndex { get; set; }
        }

        #endregion

        #region Properties

        public virtual int RepeatColumns
        {
            get => (int)(ViewState[nameof(RepeatColumns)] ?? 0);
            set => ViewState[nameof(RepeatColumns)] = Number.CheckRange(value, 0);
        }

        public virtual RepeatDirection RepeatDirection
        {
            get => (RepeatDirection)(ViewState[nameof(RepeatDirection)] ?? RepeatDirection.Vertical);
            set => ViewState[nameof(RepeatDirection)] = value;
        }

        public virtual RepeatLayout RepeatLayout
        {
            get => (RepeatLayout)(ViewState[nameof(RepeatLayout)] ?? RepeatLayout.Table);
            set => ViewState[nameof(RepeatLayout)] = value;
        }

        #endregion

        #region Fields

        private bool _isEnabled;
        private int _changedItemIndex;

        #endregion

        #region ListControl

        protected override Style CreateControlStyle() => new TableStyle(ViewState);

        protected override Control FindControl(string id, int pathOffset) => this;

        protected virtual Style GetItemStyle(ListItemType itemType, int repeatIndex) => null;

        #endregion

        #region IPostBackDataHandler

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) =>
            LoadPostData(postDataKey, postCollection);

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (!IsEnabled)
                return false;

            var value = postCollection[postDataKey];
            var selectedIndex = SelectedIndex;

            EnsureDataBound();

            var changedItemIndex = LoadPostData(postDataKey, value, selectedIndex);
            if (changedItemIndex == null)
                return false;

            _changedItemIndex = changedItemIndex.Value;

            return true;
        }

        protected abstract int? LoadPostData(string key, string value, int selectedIndex);

        void IPostBackDataHandler.RaisePostDataChangedEvent() => RaisePostDataChangedEvent();

        protected virtual void RaisePostDataChangedEvent()
        {
            if (AutoPostBack && !Page.IsPostBackEventControlRegistered)
            {
                Page.AutoPostBackControl = this;

                if (CausesValidation)
                    Page.Validate(ValidationGroup);
            }

            OnSelectedIndexChanged(new ChangedArgs { ChangedItemIndex = _changedItemIndex });
        }

        #endregion

        #region Render

        protected override void OnPreRender(EventArgs e)
        {
            Page.ClientScript.RegisterForEventValidation(UniqueID);

            base.OnPreRender(e);
        }

        protected abstract T GetToggleToRepeat();

        protected abstract void SetupToggleToRepeat(T toggle, System.Web.UI.WebControls.ListItem item, int index);

        protected override void Render(HtmlTextWriter writer)
        {
            if (Items.Count == 0)
                return;

            var repeatInfo = new RepeatInfo
            {
                RepeatColumns = RepeatColumns,
                RepeatDirection = RepeatDirection,
                RepeatLayout = RepeatLayout
            };
            var style = base.ControlStyleCreated ? base.ControlStyle : null;

            repeatInfo.RenderRepeater(writer, this, style, this);
        }

        protected virtual void RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer)
        {
            if (repeatIndex == 0)
                _isEnabled = base.IsEnabled;

            var item = Items[repeatIndex];

            var toggle = GetToggleToRepeat();
            toggle.Attributes.Clear();
            toggle.CssClass = string.Empty;

            var attributes = item.Attributes;
            foreach (string key in attributes.Keys)
            {
                if (key.Equals("class", StringComparison.OrdinalIgnoreCase))
                    toggle.CssClass = key;
                else
                    toggle.Attributes[key] = attributes[key];
            }

            toggle.ID = repeatIndex.ToString();
            toggle.Text = item.Text;
            toggle.Checked = item.Selected;
            toggle.Enabled = _isEnabled && item.Enabled;

            SetupToggleToRepeat(toggle, item, repeatIndex);

            toggle.RenderControl(writer);
        }

        #endregion

        #region IRepeatInfoUser

        protected virtual bool HasFooter => false;

        bool IRepeatInfoUser.HasFooter => HasFooter;

        protected virtual bool HasHeader => false;

        bool IRepeatInfoUser.HasHeader => HasHeader;

        protected virtual bool HasSeparators => false;

        bool IRepeatInfoUser.HasSeparators => HasSeparators;

        protected virtual int RepeatedItemCount => Items?.Count ?? 0;

        int IRepeatInfoUser.RepeatedItemCount => RepeatedItemCount;

        Style IRepeatInfoUser.GetItemStyle(ListItemType itemType, int repeatIndex) =>
            GetItemStyle(itemType, repeatIndex);

        void IRepeatInfoUser.RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer) =>
            RenderItem(itemType, repeatIndex, repeatInfo, writer);

        #endregion
    }
}