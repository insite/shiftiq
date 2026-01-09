using System;
using System.Linq;

using InSite.Application.Surveys.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Messages;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QSurveyFormFilter>
    {
        protected override bool EnableSearchValidation => true;

        public int? DefaultStatusID => int.TryParse(Request["statusId"], out int value) ? value : (int?)null;

        public override QSurveyFormFilter Filter
        {
            get
            {
                var filter = new QSurveyFormFilter
                {
                    OrganizationIdentifier = Organization.OrganizationIdentifier,
                    Name = Name.Text,
                    Title = Title.Text,
                    LastModifiedSince = LastModifiedSince.Value,
                    LastModifiedBefore = LastModifiedBefore.Value,
                    CurrentStatus = CurrentStatus.ValuesArray,
                    IsLocked = LockStatus.ValueAsBoolean,
                    MessageIdentifier = MessageIdentifier.Value

                };

                GetCheckedShowColumns(filter);

                filter.SortByColumn = SortColumns.Value;

                return filter;
            }
            set
            {
                Name.Text = value.Name;

                LastModifiedSince.Value = value.LastModifiedSince;
                LastModifiedBefore.Value = value.LastModifiedBefore;
                MessageIdentifier.Value = value.MessageIdentifier;
                CurrentStatus.Values = value.CurrentStatus;

                LockStatus.ValueAsBoolean = value.IsLocked;

                SortColumns.Value = value.SortByColumn;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MessageIdentifier.AutoPostBack = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            MessageIdentifier.Filter.IncludeTypes = new string[] { MessageTypeName.Notification, MessageTypeName .Invitation};
        }

        public override void Clear()
        {
            Name.Text = null;
            Title.Text = null;
            LastModifiedSince.Value = null;
            LastModifiedBefore.Value = null;
            MessageIdentifier.Value = null;
            CurrentStatus.ClearSelection();
            LockStatus.ClearSelection();
        }
    }
}