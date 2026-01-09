using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class OrganizationField
    {
        public string FieldName { get; set; }
        public bool IsRequired { get; set; }
        public bool IsVisible { get; set; }
        public bool IsMasked { get; set; }

        [DefaultValue(true)]
        public bool IsEditable { get; set; } = true;

        public bool IsEqual(OrganizationField other)
        {
            return FieldName.NullIfEmpty() == other.FieldName.NullIfEmpty()
                && IsRequired == other.IsRequired
                && IsVisible == other.IsVisible
                && IsMasked == other.IsMasked
                && IsEditable == other.IsEditable;
        }

        public static bool IsEqual(ICollection<OrganizationField> collection1, ICollection<OrganizationField> collection2)
        {
            return collection1.Count == collection2.Count
                && collection1.Zip(collection2, (a, b) => a.IsEqual(b)).All(x => x);
        }

        public OrganizationField Clone()
        {
            return new OrganizationField
            {
                FieldName = FieldName.NullIfEmpty(),
                IsRequired = IsRequired,
                IsVisible = IsVisible,
                IsMasked = IsMasked,
                IsEditable = IsEditable
            };
        }
    }
}