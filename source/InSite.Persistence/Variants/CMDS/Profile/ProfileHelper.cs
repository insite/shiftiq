using System;
using System.Collections.Generic;

using InSite.Application.Standards.Read;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence.Plugin.CMDS
{
    public static class ProfileHelper
    {
        #region Public methods

        public static string InitNumber(QStandard asset, string user)
        {
            var organizationId = asset.OrganizationIdentifier != OrganizationIdentifiers.CMDS
                ? asset.OrganizationIdentifier
                : (Guid?)null;

            return organizationId == null ? GenerateNumber(user) : GenerateNumber(organizationId.Value, user);
        }

        #endregion

        #region Private methods

        private static string GenerateNumber(string user)
        {
            string numberStr = ProfileRepository.SelectMaxNumber();
            int number = string.IsNullOrEmpty(numberStr) ? 0 : int.Parse(numberStr, Cultures.Default);
            IReadOnlyList<Standard> duplicates;

            do
            {
                number++;
                numberStr = number.ToString(Cultures.Default).PadLeft(4, '0');
                duplicates = StandardSearch.Select(x => x.StandardType == "Profile" && x.Code == numberStr);
            }
            while (duplicates.IsNotEmpty());

            return numberStr;
        }

        private static string GenerateNumber(Guid organizationId, string user)
        {
            var organization = OrganizationSearch.Select(organizationId);

            if (string.IsNullOrEmpty(organization.CompanyName))
                return GenerateNumber(user);

            string numberStr = ProfileRepository.SelectMaxNumber(organization.CompanyName);

            int number;

            if (string.IsNullOrEmpty(numberStr))
                number = 0;
            else
            {
                int index = numberStr.LastIndexOf('-');
                numberStr = numberStr.Substring(index + 1);

                number = int.Parse(numberStr, Cultures.Default);
            }

            do
            {
                number++;

                numberStr = string.Format("{0}-{1}",
                                            organization.CompanyName,
                                            number.ToString(Cultures.Default).PadLeft(4, '0')
                    );
            }
            while (StandardSearch.SelectFirst(x => x.Code == numberStr) != null);

            return numberStr;
        }

        #endregion
    }
}