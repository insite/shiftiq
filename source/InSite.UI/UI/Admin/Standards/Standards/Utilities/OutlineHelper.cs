using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Standards.Standards.Utilities
{
    public static class OutlineHelper
    {
        #region Classes

        public class ReorderItem
        {
            public Guid Key { get; set; }
            public Guid Identifier { get; set; }
            public int Number { get; set; }
            public bool IsEdge { get; set; }
            public int SuperSequence { get; set; }
            public int Sequence { get; set; }
            public string Type { get; set; }
            public string Icon { get; set; }
            public string Title { get; set; }
        }

        #endregion

        private static OrganizationState Organization => CurrentSessionState.Identity.Organization;

        public static bool Indent(Guid standardKey, Guid parentKey)
        {
            var parent = StandardSearch.BindFirst(
                x => new { x.StandardIdentifier, MaxSequence = (int?)x.Children.Max(y => y.Sequence) },
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && x.StandardIdentifier == parentKey
                     && x.Parent.Children.Any(y => y.StandardIdentifier == standardKey));

            if (parent == null)
                return false;

            StandardStore.Update(standardKey, entity =>
            {
                entity.ParentStandardIdentifier = parent.StandardIdentifier;
                entity.Sequence = (parent.MaxSequence ?? 0) + 1;
            });

            return true;
        }

        public static bool Outdent(Guid standardKey, Guid parentKey)
        {
            var parent = StandardSearch.BindFirst(
                x => new { x.ParentStandardIdentifier, MaxSequence = (int?)x.Parent.Children.Max(y => y.Sequence) },
                x => x.OrganizationIdentifier == Organization.OrganizationIdentifier && x.StandardIdentifier == parentKey &&
                     x.Children.Any(y => y.StandardIdentifier == standardKey));

            if (parent == null)
                return false;

            StandardStore.Update(standardKey, entity =>
            {
                entity.ParentStandardIdentifier = parent.ParentStandardIdentifier;
                entity.Sequence = parent.ParentStandardIdentifier.HasValue ? (parent.MaxSequence ?? 0) + 1 : 0;
            });

            return true;
        }

        public static ReorderItem[] LoadReorderItems(Guid key)
        {
            var data = StandardContainmentSummarySearch
                .Bind(
                    x => new ReorderItem
                    {
                        Key = x.ChildStandardIdentifier,
                        Identifier = x.ChildStandardIdentifier,
                        Number = x.ChildAssetNumber,
                        IsEdge = !x.ParentIsPrimaryContainer,
                        SuperSequence = x.ParentSequence,
                        Sequence = x.ChildSequence,
                        Type = x.ChildStandardType,
                        Icon = x.ChildIcon,
                        Title = CoreFunctions.GetContentTextEn(x.ChildStandardIdentifier, ContentLabel.Title)
                    },
                    x => x.ParentStandardIdentifier == key,
                    null, "ParentSequence,ChildSequence");

            return data;
        }

        public static bool SaveReorder(Guid key, IEnumerable<int> itemsOrder)
        {
            if (itemsOrder == null)
                return false;

            var updatedCount = 0;
            var superSequence = 0;
            var sequence = 0;

            var itemsByNumber = LoadReorderItems(key).ToDictionary(x => x.Number, x => x);

            foreach (var asset in itemsOrder)
            {
                if (!itemsByNumber.ContainsKey(asset))
                {
                    updatedCount = 0;
                    break;
                }

                var item = itemsByNumber[asset];

                if (item.IsEdge)
                {
                    item.SuperSequence = superSequence;
                    item.Sequence = ++sequence;
                }
                else
                {
                    item.SuperSequence = ++superSequence;
                    sequence = 0;
                }

                updatedCount++;
            }

            if (itemsByNumber.Count != updatedCount)
                return false;

            var itemsByKey = itemsByNumber.Values.ToDictionary(x => x.Key);

            var assetFilter = itemsByNumber.Values.Where(x => !x.IsEdge).Select(x => x.Key).ToArray();
            if (assetFilter.Length > 0)
                StandardStore.Update(
                    x => assetFilter.Contains(x.StandardIdentifier),
                    asset => asset.Sequence = itemsByKey[asset.StandardIdentifier].SuperSequence);

            var edgeFilter = itemsByNumber.Values.Where(x => x.IsEdge).Select(x => x.Key).ToArray();
            if (edgeFilter.Length > 0)
            {
                var relationships = StandardContainmentSearch.Select(x => x.ParentStandardIdentifier == key && edgeFilter.Contains(x.ChildStandardIdentifier));
                foreach (var relationship in relationships)
                {
                    var assetItem = itemsByKey[relationship.ChildStandardIdentifier];
                    relationship.ChildSequence = assetItem.Sequence;
                }

                StandardContainmentStore.Update(relationships);
            }

            return true;
        }

        private static readonly string StatusCacheKey = typeof(OutlineHelper).FullName + ".StatusCache";

        public static string GetStatusBadgeHtml(string value, string cssClass)
        {
            if (value.IsEmpty())
                return string.Empty;

            var data = (List<TCollectionItem>)HttpContext.Current.Items[StatusCacheKey];
            if (data == null)
            {
                HttpContext.Current.Items[StatusCacheKey] = data = TCollectionItemCache.Select(new TCollectionItemFilter
                {
                    OrganizationIdentifier = Organization.Identifier,
                    CollectionName = CollectionName.Standards_Classification_Status
                });
            }

            var item = data.Where(x => x.ItemName == value).FirstOrDefault();
            if (item == null)
                return string.Empty;

            var colorClass = "text-body";

            if (item.ItemColor.IsNotEmpty())
            {
                var indicator = item.ItemColor.ToEnumNullable<Indicator>();
                colorClass = "bg-" + indicator.GetContextualClass();
            }

            var badgeClass = "badge " + colorClass;
            if (cssClass.IsNotEmpty())
                badgeClass += " " + cssClass;

            return $"<span class='{badgeClass}'>{item.ItemName}</span>";
        }
    }
}