using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class AddressFilter : Filter
    {
        #region Properties

        public string ProvinceName { get; set; }
        public string CityName { get; set; }

        #endregion

        #region Public methods

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(ProvinceName)
                && string.IsNullOrEmpty(CityName)
                ;
        }

        #endregion
    }
}
