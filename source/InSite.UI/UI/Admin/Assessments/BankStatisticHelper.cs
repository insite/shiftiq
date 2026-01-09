using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using InSite.Domain.Banks;
using InSite.Persistence;

namespace InSite.Admin.Assessments
{
    internal static class BankStatisticHelper
    {
        #region Classes

        public class AssetInfo
        {
            public Guid Thumbprint { get; set; }
            public string Code { get; set; }
        }

        #endregion

        #region Methods

        public static Dictionary<Guid, AssetInfo> GetBankAssets(BankState bank)
        {
            var includeSubCompetencies = CurrentSessionState.Identity.Organization.Toolkits.Assessments.EnableQuestionSubCompetencySelection;
            var key = typeof(BankStatisticHelper) + ".Assets." + bank.Identifier.ToString();
            var value = (Dictionary<Guid, AssetInfo>)HttpContext.Current.Items[key];

            if (value != null)
                return value;

            var filter = GetStandardFilter(bank, includeSubCompetencies);

            HttpContext.Current.Items[key] = value = StandardSearch
                .Bind(
                    x => new AssetInfo
                    {
                        Thumbprint = x.StandardIdentifier,
                        Code = x.Code
                    },
                    x => filter.Contains(x.StandardIdentifier))
                .ToDictionary(x => x.Thumbprint);

            return value;
        }

        private static HashSet<Guid> GetStandardFilter(BankState bank, bool includeSubCompetencies)
        {
            var filter = new HashSet<Guid>();

            foreach (var set in bank.Sets)
            {
                if (!filter.Contains(set.Standard))
                    filter.Add(set.Standard);

                foreach (var question in set.Questions)
                {
                    if (!filter.Contains(question.Standard))
                        filter.Add(question.Standard);

                    if (includeSubCompetencies && question.SubStandards != null)
                    {
                        foreach (var sub in question.SubStandards)
                            filter.Add(sub);
                    }

                }
            }

            return filter;
        }

        #endregion
    }
}