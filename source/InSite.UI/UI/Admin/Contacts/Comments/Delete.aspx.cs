using System;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Application.People.Write;
using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Domain.Contacts;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Contacts.Comments.Forms
{
    public partial class Delete : AdminBasePage, IHasParentLinkParameters
    {
        #region Properties

        private Guid ContactIdentifier => Guid.TryParse(Request["contact"], out var value) ? value : Guid.Empty;

        private Guid CommentKey => Guid.Parse(Request["comment"]);

        private bool _isPersonLoaded;

        private QPerson _person;

        private QPerson Person
        {
            get
            {
                if (!_isPersonLoaded)
                {
                    _person = ServiceLocator.PersonSearch.GetPerson(ContactIdentifier, Organization.Key, x => x.User);
                    _isPersonLoaded = true;
                }

                return _person;
            }
        }

        #endregion

        #region Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RemoveButton.Click += RemoveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        #endregion

        #region Event handlers

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            RemoveComment();

            RedirectToReader();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            if (Person == null)
                RedirectToSearch();

            var comment = PersonCommentSummarySearch.SelectFirst(x => x.CommentIdentifier == CommentKey);
            if (comment == null)
                RedirectToReader();

            SetInputValues(comment);
        }

        private void RemoveComment()
        {
            var comment = QCommentSearch.SelectFirst(CommentKey);
            if (comment == null)
                return;

            var person = ServiceLocator.PersonSearch.GetPerson(ContactIdentifier, Organization.Identifier);
            if (person == null)
                RedirectToReader();

            ServiceLocator.SendCommand(new ModifyPersonComment(person.PersonIdentifier, CommentActionType.Delete, Get(comment)));
        }

        #endregion

        #region Settings/getting input values

        private void SetInputValues(VComment comment)
        {
            PageHelper.AutoBindHeader(this, null, Person.User.FullName);

            PersonDetail.BindPerson(Person, User.TimeZone);

            CommentText.Text = AssetCommentHelper.GetDescription(comment.CommentText, comment.UploadName, comment.UploadUrl, comment.UploadSize);
            CommentAuthor.Text = comment.AuthorUserName;
            CommentPosted.Text = GetLocalTime(comment.CommentPosted, User.TimeZone);

            CancelButton.NavigateUrl = GetParentUrl();
        }

        private string GetLocalTime(DateTimeOffset? item, TimeZoneInfo tz)
        {
            return item.Format(tz, nullValue: "None");
        }

        private PersonComment Get(QComment comment)
        {
            return new PersonComment()
            {
                Author = comment.AuthorUserIdentifier,
                AuthorName = comment.AuthorUserName,
                Comment = comment.CommentIdentifier,
                Container = comment.ContainerIdentifier,
                ContainerType = comment.ContainerType,
                Flag = comment.CommentFlag,
                IsPrivate = comment.CommentIsPrivate,
                ModifiedBy = comment.TimestampModifiedBy,
                Organization = comment.OrganizationIdentifier,
                Posted = comment.CommentPosted,
                Resolved = comment.CommentResolved,
                Revised = comment.CommentRevised,
                SubjectType = comment.ContainerSubtype,
                Text = comment.CommentText,
                Topic = comment.TopicUserIdentifier
            };
        }
        #endregion

        #region Methods (redirect)

        private static void RedirectToSearch() => HttpResponseHelper.Redirect($"/ui/admin/contacts/people/search", true);

        private void RedirectToReader()
        {
            var url = GetParentUrl();

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetParentUrl()
        {
            var url = new ReturnUrl().GetReturnUrl();

            if (url == null)
                url = $"/ui/admin/contacts/people/edit?contact={ContactIdentifier}&panel=comments";

            return url;
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/edit")
                ? $"contact={ContactIdentifier}"
                : null;
        }

        #endregion
    }
}