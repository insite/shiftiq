using System;

using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class SectorComboBox : ComboBox
    {
        public Guid OrganizationIdentifier
        {
            get { return (Guid)ViewState[nameof(OrganizationIdentifier)]; }
            set { ViewState[nameof(Guid)] = value; }
        }

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            var data = TCollectionItemCache.Select(new TCollectionItemFilter
            {
                OrganizationIdentifier = OrganizationIdentifier,
                CollectionName = CollectionName.Contacts_Settings_Industries_Name
            });

            foreach (var item in data)
            {
                list.Add(item.ItemName);

                if (item.ItemDescription.IsNotEmpty())
                {
                    var subSectors = item.ItemDescription.Split(new string[] { "; " }, StringSplitOptions.None);
                    foreach (var subSector in subSectors)
                        list.Add(subSector);
                }
            }

            return list;
        }
    }
}