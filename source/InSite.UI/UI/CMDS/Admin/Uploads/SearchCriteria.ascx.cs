using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Admin.Uploads.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<UploadFilter>
    {
        private static bool CanModifyContainerType => Identity.IsInRole(CmdsRole.Programmers);

        public override UploadFilter Filter
        {
            get
            {
                var filter = new UploadFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    ContainerType = CanModifyContainerType ? ContainerType.Value : UploadContainerType.Oganization,
                    Keyword = Keyword.Text,
                    PostedSince = PostedSince.Value.ToDateTimeOffset(TimeSpan.Zero),
                    PostedBefore = PostedBefore.Value.ToDateTimeOffset(TimeSpan.Zero)
                };

                filter.UploadType = string.IsNullOrEmpty(UploadType.Value)
                    ? new[] { Shift.Constant.UploadType.CmdsFile, Shift.Constant.UploadType.Link }
                    : new[] { UploadType.Value };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                ContainerType.Value = CanModifyContainerType ? value.ContainerType : UploadContainerType.Oganization;
                UploadType.Value = value.UploadType != null && value.UploadType.Length == 1 ? value.UploadType[0] : null;
                Keyword.Text = value.Keyword;
                PostedSince.Value = value.PostedSince?.UtcDateTime;
                PostedBefore.Value = value.PostedBefore?.UtcDateTime;
            }
        }

        public override void Clear()
        {
            ContainerType.Value = UploadContainerType.Oganization;
            UploadType.ClearSelection();
            Keyword.Text = null;
            PostedSince.Value = null;
            PostedBefore.Value = null;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            ContainerType.Enabled = CanModifyContainerType;
        }
    }
}