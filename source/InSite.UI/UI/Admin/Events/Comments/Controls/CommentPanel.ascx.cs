using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using InSite.Application.Contents.Read;
using InSite.Application.Events.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Events.Comments.Controls
{
    public partial class CommentPanel : BaseUserControl
    {
        #region Properties

        public int Count => Repeater.Items.Count;

        private Guid EventIdentifier
        {
            get => (Guid)ViewState[nameof(EventIdentifier)];
            set => ViewState[nameof(EventIdentifier)] = value;
        }

        private EventType EventType;

        public bool IsReadOnly
        {
            get => (bool?)ViewState[nameof(IsReadOnly)] ?? false;
            set => ViewState[nameof(IsReadOnly)] = value;
        }


        #endregion

        #region Fields

        private Dictionary<Guid, string> _authorNameCache;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.DataBinding += (s, a) => _authorNameCache = new Dictionary<Guid, string>();
            Repeater.ItemDataBound += Repeater_ItemDataBound;
        }

        #endregion

        #region Load data

        public int LoadData(Guid @event, EventType typeOfEvent)
        {
            EventIdentifier = @event;
            EventType = typeOfEvent;

            SetInputValues();

            AddCommentButton.Visible = !IsReadOnly;

            if (!IsReadOnly)
            {
                if (typeOfEvent == EventType.Exam)
                    AddCommentButton.NavigateUrl = $"/ui/admin/events/comments/post?event={EventIdentifier}";

                if (typeOfEvent == EventType.Class)
                    AddCommentButton.NavigateUrl = $"/ui/admin/events/classes/comments/post?event={EventIdentifier}";
            }

            return Repeater.Items.Count;
        }

        #endregion

        #region Event handling

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var comment = (QComment)e.Item.DataItem;

            var commandsBlock = e.Item.FindControl("CommandsBlock");
            if (commandsBlock != null)
                commandsBlock.Visible = !IsReadOnly;

            var queryString = $"?event={comment.EventIdentifier}&id={comment.CommentIdentifier}";

            var editCommentButton = (IconLink)e.Item.FindControl("EditCommentButton");
            editCommentButton.NavigateUrl = EventType == EventType.Exam
                ? "/ui/admin/events/comments/post" + queryString
                : "/ui/admin/events/classes/comments/post" + queryString;

            var deleteCommentButton = (IconLink)e.Item.FindControl("DeleteCommentButton");
            deleteCommentButton.NavigateUrl = EventType == EventType.Exam
                ? "/admin/events/comments/delete" + queryString
                : "/ui/admin/events/classes/comments/delete" + queryString;
        }

        #endregion

        #region Getting and setting input values

        private void SetInputValues()
        {
            Repeater.DataSource = ServiceLocator.EventSearch.GetComments(new QEventCommentFilter
            {
                EventIdentifier = EventIdentifier
            });
            Repeater.DataBind();
        }

        public void SetEventIdentifier(Guid eventIdentifier)
        {
            EventIdentifier = eventIdentifier;
        }
        #endregion

        #region Helper methods

        protected string GetAuthorName()
        {
            var dataItem = (QComment)Page.GetDataItem();

            if (dataItem.AuthorUserIdentifier == Guid.Empty)
                return UserNames.Someone;

            if (!_authorNameCache.TryGetValue(dataItem.AuthorUserIdentifier, out var name))
            {
                name = UserSearch.Bind(dataItem.AuthorUserIdentifier, u => u.FullName).IfNullOrEmpty(UserNames.Someone);
                _authorNameCache.Add(dataItem.AuthorUserIdentifier, name);
            }

            return name;
        }

        protected string GetTimestamp()
        {
            var dataItem = (QComment)Page.GetDataItem();
            return $"<span title='{LocalizeTime(dataItem.CommentPosted, null, false)}'>commented " + TimeZones.Format(dataItem.CommentPosted, CurrentSessionState.Identity.User.TimeZone) + "</span>";
        }

        #endregion
    }
}