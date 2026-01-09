using System;
using System.Collections.Generic;
using System.ComponentModel;

using InSite.Application.Contacts.Read;
using InSite.Application.Courses.Read;

namespace InSite.Application.Invoices.Read
{
    public class TProduct
    {
        public Guid ProductIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? ObjectIdentifier { get; set; }
        public Guid? IndustryItemIdentifier { get; set; }
        public Guid? OccupationItemIdentifier { get; set; }
        public Guid? LevelItemIdentifier { get; set; }
        public string ProductCurrency { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductType { get; set; }
        public string ProductImageUrl { get; set; }
        public string ObjectType { get; set; }
        public string ProductUrl { get; set; }
        public string ProductSummary { get; set; }
        public decimal? ProductPrice { get; set; }
        public DateTimeOffset? Published { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? PublishedBy { get; set; }
        public Guid ModifiedBy { get; set; }

        [DefaultValue(false)]
        public bool IsFeatured { get; set; }

        [DefaultValue(false)]
        public bool IsTaxable { get; set; }

        [DefaultValue(0)]
        public int ProductQuantity { get; set; }

        public virtual ICollection<QInvoiceItem> InvoiceItems { get; set; } = new HashSet<QInvoiceItem>();
        public virtual ICollection<QGroup> MembershipGroups { get; set; } = new HashSet<QGroup>();
        public virtual ICollection<TOrderItem> OrderItems { get; set; } = new HashSet<TOrderItem>();
        public virtual ICollection<TCourseDistribution> CourseDistributions { get; set; } = new HashSet<TCourseDistribution>();
    }

    public class QueryProduct
    {
        public Guid ProductIdentifier { get; set; }
        public string ProductCurrency { get; set; }
        public string ProductDescription { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public decimal? ProductPrice { get; set; }
        public string ProductImageUrl { get; set; }

        public Guid? ObjectIdentifier { get; set; }
        public string ObjectType { get; set; }
        public string ObjectName { get; set; }
    }
}
