using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contents.Read;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class LabelSearch
    {
        public static readonly Guid ContainerIdentifier = new Guid("5d3b4c71-f3f7-4e41-9945-0f83cf922e2f");

        private static List<TContent> _labels;

        public class LanguageItem
        {
            public string Tag { get; set; }
            public string Text { get; set; }
            public int? Count { get; set; }
        }

        public class LabelItem
        {
            public string ContentLabel { get; set; }
            public List<LanguageItem> Languages { get; set; }
            public int? ReferenceCount { get; set; }
        }

        static LabelSearch()
        {
            Refresh();
        }

        public static void Refresh()
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                _labels = db.TContents
                    .AsNoTracking()
                    .Where(x => x.ContainerType == ContentContainerType.Application && x.ContainerIdentifier == ContainerIdentifier)
                    .ToList();
            }
        }

        public static List<LabelItem> Search(LabelFilter filter)
        {
            var query = _labels.AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.LabelName.HasValue())
                query = query.Where(x => Contains(x.ContentLabel, filter.LabelName));

            if (filter.LabelTranslation.HasValue())
                query = query.Where(x => Contains(x.ContentText, filter.LabelTranslation));

            var list = query.ToList();

            return list
                .GroupBy(x => x.ContentLabel)
                .Select(x => new LabelItem
                {
                    ContentLabel = x.Key,
                    Languages = x.Select(y =>
                    {
                        string tag;

                        if (y.OrganizationIdentifier == OrganizationIdentifiers.Global)
                        {
                            tag = y.ContentLanguage;
                        }
                        else
                        {
                            var organization = OrganizationSearch.Select(y.OrganizationIdentifier);
                            tag = organization != null
                                ? $"{y.ContentLanguage}:{organization.Code}"
                                : y.ContentLanguage;
                        }

                        return new LanguageItem
                        {
                            Tag = tag,
                            Text = y.ContentText,
                            Count = y.ReferenceCount
                        };
                    })
                    .OrderBy(z => z.Tag)
                    .ToList()
                })
                .OrderBy(x => x.ContentLabel)
                .ToList();
        }

        private static bool Contains(string a, string b)
        {
            if (a == null)
                return false;
            return a.IndexOf(b, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string GetTranslation(string name, string language, Guid organization)
            => GetTranslation(name, language, organization, false, true, null);

        public static string GetTranslation(string name, string language, Guid organization, bool allowNull)
            => GetTranslation(name, language, organization, allowNull, true, null);

        public static string GetTranslation(string name, string language, Guid organization, bool allowNull, bool allowDefault, string @default = null)
        {
            var contents = _labels
                .Where(x => string.Equals(x.ContentLabel, name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var translation = GetTranslation(language, organization, contents);

            if (!string.IsNullOrEmpty(translation))
                return translation;

            if (allowDefault)
            {
                translation = GetTranslation("en", organization, contents);

                if (!string.IsNullOrEmpty(translation))
                    return translation;
            }

            return allowNull ? null : @default ?? name;
        }

        private static string GetTranslation(string language, Guid? organizationIdentifier, List<TContent> contents)
        {
            var content = contents.FirstOrDefault(x =>
                string.Equals(x.ContentLanguage, language, StringComparison.OrdinalIgnoreCase)
                && (organizationIdentifier == null && x.OrganizationIdentifier == OrganizationIdentifiers.Global || x.OrganizationIdentifier == organizationIdentifier)
            );

            if (content == null && organizationIdentifier.HasValue && organizationIdentifier != OrganizationIdentifiers.Global)
            {
                content = contents.FirstOrDefault(x =>
                    string.Equals(x.ContentLanguage, language, StringComparison.OrdinalIgnoreCase)
                    && x.OrganizationIdentifier == OrganizationIdentifiers.Global
                );
            }

            return content?.ContentText;
        }
    }
}