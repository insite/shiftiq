using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Standards.Standards.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<StandardFilter>
    {
        private string DefaultStandardType => Request.QueryString["type"];

        public override StandardFilter Filter
        {
            get
            {
                if (!IsPostBack)
                    TrySelectStandardType(DefaultStandardType);

                var filter = new StandardFilter
                {
                    OrganizationIdentifier = Organization.Key,
                    DepartmentGroupIdentifier = DepartmentIdentifier.Value,

                    ParentTitle = ParentTitle.Text,
                    StandardTier = StandardTier.Text,
                    StandardLabel = StandardLabel.Text,
                    Code = Code.Text,
                    Title = Title.Text,
                    ContentName = ContentName.Text,
                    Keyword = Keyword.Text,
                    Number = ValueConverter.ToInt32Nullable(Number.Text),

                    Tags = TagSelector.ValuesArray,
                    Scope = Scope.Value.ToEnum(StandardTypeEnum.None),
                    Modified =
                    {
                        Since = UtcModifiedSince.Value?.UtcDateTime,
                        Before = UtcModifiedBefore.Value?.UtcDateTime
                    }
                };

                if (!string.IsNullOrEmpty(StandardType.Value))
                    filter.StandardTypes = new[] { StandardType.Value };

                // filter.Exclusions.StandardType.Add(Constant.Achievements.StandardType.Document);
                // filter.Exclusions.StandardType.Add(Constant.Achievements.StandardType.Collection);

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                DepartmentIdentifier.Value = value.DepartmentIdentifier;

                StandardType.Value = value.StandardTypes.IsEmpty() ? null : value.StandardTypes[0];
                ParentTitle.Text = value.ParentTitle;
                StandardTier.Text = value.StandardTier;
                StandardLabel.Text = value.StandardLabel;
                Code.Text = value.Code;
                Title.Text = value.Title;
                ContentName.Text = value.ContentName;
                Keyword.Text = value.Keyword;
                Number.Text = !value.Number.HasValue ? string.Empty : value.Number.ToString();

                TagSelector.Values = value.Tags;
                Scope.Value = value.Scope.GetName(StandardTypeEnum.None);

                UtcModifiedSince.Value = value.Modified.Since;
                UtcModifiedBefore.Value = value.Modified.Before;

                if (!IsPostBack)
                    TrySelectStandardType(DefaultStandardType);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            DepartmentIdentifier.OrganizationIdentifier = Organization.Identifier;

            TagSelector.LoadItems(OrganizationHelper.GetStandardOrganizationTags(Organization.Key));

            Scope.LoadItems(StandardTypeEnum.Framework, StandardTypeEnum.Cluster, StandardTypeEnum.Competency);
        }

        public override void Clear()
        {
            DepartmentIdentifier.Value = null;

            StandardType.ClearSelection();

            TrySelectStandardType(DefaultStandardType);

            ParentTitle.Text = null;
            StandardTier.Text = null;
            StandardLabel.Text = null;
            Code.Text = null;
            Title.Text = null;
            ContentName.Text = null;
            Keyword.Text = null;
            Number.Text = null;

            TagSelector.ClearSelection();
            Scope.ClearSelection();

            UtcModifiedSince.Value = null;
            UtcModifiedBefore.Value = null;
        }

        private void TrySelectStandardType(string value)
        {
            if (value.IsEmpty())
                return;

            StandardType.EnsureDataBound();

            var option = StandardType.FindOptionByValue(value, true);
            if (option != null)
                option.Selected = true;
        }
    }
}