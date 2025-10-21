using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Shift.Common.Linq;

namespace InSite.Admin.Contacts.People.Models
{
    public class PersonSearchResultsModel
    {
        public Dictionary<Guid, bool> Documents { get; set; }
        public Dictionary<Guid, string[]> Statuses { get; set; }
        public Dictionary<Guid, string[]> Cases { get; set; }

        public Guid[] Users { get; set; }

        public bool HasDocuments(Guid user)
        {
            if (Documents == null || !Documents.ContainsKey(user))
                return false;

            return Documents[user];
        }

        public List<string> GetStatuses(Guid user)
        {
            if (Statuses == null || !Statuses.ContainsKey(user))
                return new List<string>();

            return Statuses[user].OrderBy(x => x).ToList();
        }

        public List<string> GetCases(Guid user)
        {
            if (Cases == null || !Cases.ContainsKey(user))
                return new List<string>();

            return Cases[user].OrderBy(x => x).ToList();
        }
    }
}