using System;
using System.Web;

using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Messages.Messages.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<MessageFilter>
    {
        private string DefaultType
        {
            get
            {
                var type = Request.QueryString["type"];
                if (!string.IsNullOrEmpty(type))
                    return HttpUtility.UrlDecode(type);
                return null;
            }
        }

        public bool HasDefaultCriteria => !string.IsNullOrEmpty(DefaultType);

        public override MessageFilter Filter
        {
            get
            {
                var filter = new MessageFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    Type = !IsPostBack && !string.IsNullOrEmpty(DefaultType) ? DefaultType : MessageType.Value,
                    Name = MessageName.Text,
                    Title = Subject.Text,
                    Modified = new DateTimeRange(ModifiedSince.Value?.UtcDateTime, ModifiedBefore.Value?.UtcDateTime),
                    SenderNickname = SenderNickname.Text,
                    SenderName = SenderName.Text,
                    SenderEmail = SenderEmail.Text,
                    IsDisabled = MessageDisabled.ValueAsBoolean,
                    SystemMailbox = SystemMailbox.Text,
                    HasSender = null
                };

                if (OrganizationIdentifierCriterion.Visible)
                    filter.OrganizationIdentifier = OrganizationIdentifier.Value;

                GetCheckedShowColumns(filter);

                filter.OrderBy = SortColumns.Value;

                filter.TimeZone = User.TimeZone.Id;

                return filter;
            }
            set
            {
                if (OrganizationIdentifierCriterion.Visible)
                    OrganizationIdentifier.Value = value.OrganizationIdentifier;

                MessageType.Value = !IsPostBack && !string.IsNullOrEmpty(DefaultType) ? DefaultType : value.Type;
                MessageName.Text = value.Name;
                Subject.Text = value.Title;
                MessageDisabled.ValueAsBoolean = value.IsDisabled;

                ModifiedSince.Value = value.Modified.Since;
                ModifiedBefore.Value = value.Modified.Before;

                SenderNickname.Text = value.SenderNickname;
                SenderName.Text = value.SenderName;
                SenderEmail.Text = value.SenderEmail;
                SystemMailbox.Text = value.SystemMailbox;

                SortColumns.Value = value.OrderBy;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            OrganizationIdentifierCriterion.Visible = Organization.OrganizationIdentifier == OrganizationIdentifiers.Global;
        }

        public override void Clear()
        {
            if (string.IsNullOrEmpty(DefaultType))
                MessageType.ClearSelection();
            else
                MessageType.Value = DefaultType;

            MessageName.Text = null;
            Subject.Text = null;
            MessageDisabled.ValueAsBoolean = null;

            ModifiedSince.Value = null;
            ModifiedBefore.Value = null;

            SenderNickname.Text = null;
            SenderName.Text = null;
            SenderEmail.Text = null;
            SystemMailbox.Text = null;
        }
    }
}