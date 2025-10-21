using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Persistence;

using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public static class FindStandardHelper
    {
        #region Classes

        public enum TextType { TypeNumberTitle, CodeTitle, Title, ContentName }

        #endregion

        #region Methods

        public static IEnumerable<T> Select<T>(StandardFilter filter, TextType textType, Func<Guid, string, T> getItem, string language)
        {
            EvalTextType(textType, out var orderBy, out var getText);

            filter.OrderBy = orderBy;

            return StandardSearch
                .SelectFinderEntity(filter, language)
                .Select(x => getItem(x.StandardIdentifier, getText(x)));
        }

        public static IEnumerable<T> Select<T>(Guid[] ids, TextType textType, Func<Guid, string, T> getItem, string language)
        {
            EvalTextType(textType, out var orderBy, out var getText);

            var filter = new StandardFilter
            {
                Inclusions = ids,
                OrderBy = orderBy
            };

            return StandardSearch
                .SelectFinderEntity(filter, language)
                .Select(x => getItem(x.StandardIdentifier, getText(x)));
        }

        private static void EvalTextType(TextType type, out string orderBy, out Func<StandardSearch.FinderEntity, string> getText)
        {
            orderBy = null;
            getText = null;

            switch (type)
            {
                case TextType.TypeNumberTitle:
                    orderBy = $"{nameof(StandardSearch.FinderEntity.Type)},{nameof(StandardSearch.FinderEntity.Number)},{nameof(StandardSearch.FinderEntity.Title)}";
                    getText = x => $"{x.Type} {x.Number}. {x.Title}";
                    break;
                case TextType.CodeTitle:
                    orderBy = $"{nameof(StandardSearch.FinderEntity.Code)},{nameof(StandardSearch.FinderEntity.Title)}";
                    getText = x => $"{x.Code}: {x.Title}"; ;
                    break;
                case TextType.Title:
                    orderBy = $"{nameof(StandardSearch.FinderEntity.Title)}";
                    getText = x => x.Title;
                    break;
                default:
                    throw new NotImplementedException("Unexpected text type: " + type.GetName());
            }
        }

        #endregion
    }
}