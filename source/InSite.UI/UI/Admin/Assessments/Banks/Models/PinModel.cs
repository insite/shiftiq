using System;
using System.Collections.Generic;
using System.Web;

using Shift.Common;

namespace InSite.Admin.Assessments.Banks
{
    [Serializable]
    public class PinModel
    {
        private static readonly string Key = typeof(PinModel).FullName;

        public HashSet<int> FieldAssetNumbers { get; set; }

        public static PinModel GetModel(Guid bank)
        {
            var container = GetContainer();

            return container.GetOrAdd(bank, () => new PinModel
            {
                FieldAssetNumbers = new HashSet<int>()
            });
        }

        public static bool RemoveModel(Guid bank)
        {
            var container = GetContainer();

            return container.Remove(bank);
        }

        private static Dictionary<Guid, PinModel> GetContainer()
        {
            var session = HttpContext.Current.Session;

            return (Dictionary<Guid, PinModel>)(session[Key]
                ?? (session[Key] = new Dictionary<Guid, PinModel>()));
        }
    }
}