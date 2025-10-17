using System;
using System.Collections.Specialized;

using Shift.Common;

namespace InSite.Domain.Messages
{
    public class Notification
    {
        public NotificationType Type
        {
            get => Slug.ToEnum<NotificationType>();
            set => Slug = value.ToString();
        }

        public string Slug { get; set; }
        public string Purpose { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Variables { get; set; }

        public string Courier { get; set; }
        public string Safety { get; set; }

        public bool IsObsolete { get; set; }

        public string[] Organizations { get; set; }

        public string Aggregate { get; set; }
        public string RecipientFunction { get; set; }
        public string RecipientVariables { get; set; }

        public Guid? OriginOrganization { get; set; }
        public Guid? OriginUser { get; set; }
        public Guid? MessageIdentifier { get; set; }

        public bool IsSafetyOff()
        {
            return Safety == "Off";
        }

        public Guid GetOriginOrganization()
        {
            return OriginOrganization ?? Guid.Empty;
        }

        public Guid GetOriginUser()
        {
            return OriginUser ?? Guid.Empty;
        }

        public virtual StringDictionary BuildVariableList()
        {
            var dict = new StringDictionary();
            var properties = GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(this);
                if (value != null)
                    dict.Add(property.Name, value.ToString());
            }
            return dict;
        }
    }
}