using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Messages
{
    public class Notifications
    {
        public static Notification[] All { get; set; }

        public static NotificationType[] Types
            => All.Select(x => x.Slug)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => x.ToEnum<NotificationType>())
                .ToArray();

        public static bool Contains(string name)
        {
            return All.Any(x => StringHelper.Equals(x.Slug, name));
        }

        public static Notification Select(string name)
        {
            var type = name.ToEnum<NotificationType>();

            return Select(type);
        }

        public static Notification Select(NotificationType type, string organization = null)
        {
            Notification notification = null;
            var slug = type.GetName();

            if (organization != null)
                notification = All.FirstOrDefault(x => x.Slug == slug
                    && x.Organizations != null
                    && x.Organizations.Length > 0
                    && x.Organizations.Any(o => o == organization));

            if (notification == null)
                notification = All.FirstOrDefault(x => x.Slug == slug);

            return notification;
        }

        static Notifications()
        {
            LoadTypeDefinitions();
            ValidateTypeDefinitions();
        }

        /// <remarks>
        /// Load notification type definitions from the JSON embedded resource.
        /// </remarks>
        private static void LoadTypeDefinitions()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var name = assembly.GetManifestResourceNames()
                   .FirstOrDefault(n => n.EndsWith(".NotificationTypes.json"));

            if (name == null)
                throw new InvalidOperationException("Resource not found.");

            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    All = JsonConvert.DeserializeObject<List<Notification>>(json).ToArray();
                }
            }
        }

        private static void ValidateTypeDefinitions()
        {
            foreach (NotificationType item in Enum.GetValues(typeof(NotificationType)))
                if (Select(item) == null)
                    throw new Exception($"The definition for notification type {item} is missing from NotificationTypes.json");

            foreach (var item in All)
                if (!Enum.IsDefined(typeof(NotificationType), item.Slug))
                    throw new Exception($"The notification {item.Slug} is missing from the enumeration type.");
        }
    }
}