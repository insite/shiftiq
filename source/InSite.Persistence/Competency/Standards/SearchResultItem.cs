using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    public class SearchResultItem
    {
        public Guid Identifier { get; }
        public string Name { get; }
        public string Icon { get; set; }
        public Guid ModifiedBy { get; }
        public int Number { get; }
        public string SubType { get; }
        public Guid OrganizationIdentifier { get; }
        public string OrganizationName { get; }
        public string Title { get; }
        public DateTimeOffset Modified { get; }

        public SearchResultItem(Guid identifier, string name, string icon, Guid modifiedBy, int number, string subType, Guid organizationIdentifier, string organizationName, string title, DateTimeOffset modified)
        {
            Identifier = identifier;
            Name = name;
            Icon = icon;
            ModifiedBy = modifiedBy;
            Number = number;
            SubType = subType;
            OrganizationIdentifier = organizationIdentifier;
            OrganizationName = organizationName;
            Title = title;
            Modified = modified;
        }

        public override bool Equals(object obj)
        {
            return obj is SearchResultItem other &&
                   Identifier.Equals(other.Identifier) &&
                   Name == other.Name &&
                   Icon == other.Icon &&
                   ModifiedBy.Equals(other.ModifiedBy) &&
                   Number == other.Number &&
                   SubType == other.SubType &&
                   OrganizationIdentifier.Equals(other.OrganizationIdentifier) &&
                   OrganizationName == other.OrganizationName &&
                   Title == other.Title &&
                   Modified.Equals(other.Modified);
        }

        public override int GetHashCode()
        {
            int hashCode = 1701767238;
            hashCode = hashCode * -1521134295 + Identifier.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Icon);
            hashCode = hashCode * -1521134295 + ModifiedBy.GetHashCode();
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SubType);
            hashCode = hashCode * -1521134295 + OrganizationIdentifier.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OrganizationName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Title);
            hashCode = hashCode * -1521134295 + Modified.GetHashCode();
            return hashCode;
        }
    }
}
