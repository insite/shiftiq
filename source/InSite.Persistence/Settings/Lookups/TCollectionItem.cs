using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class TCollectionItem
    {
        public Guid CollectionIdentifier { get; set; }
        public Guid? GroupIdentifier { get; set; }
        public Guid ItemIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string ItemColor { get; set; }
        public string ItemDescription { get; set; }
        public string ItemFolder { get; set; }
        public string ItemIcon { get; set; }
        public string ItemName { get; set; }
        public string ItemNameTranslation { get; set; }

        public bool ItemIsDisabled { get; set; }

        public int ItemNumber { get; set; }
        public int ItemSequence { get; set; }

        public decimal? ItemHours { get; set; }

        public virtual VOrganization Organization { get; set; }
        public virtual TCollection Collection { get; set; }

        public virtual ICollection<TAchievementCategory> Achievements { get; set; }
        public virtual ICollection<TCourseCategory> Courses { get; set; }
        public virtual ICollection<TProgramCategory> Programs { get; set; }
        public virtual ICollection<Person> Persons { get; set; }
    }
}