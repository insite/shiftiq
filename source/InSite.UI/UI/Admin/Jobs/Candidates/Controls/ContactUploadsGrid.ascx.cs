using System;
using System.ComponentModel;
using System.Linq;

using InSite.Admin.Jobs.Candidates;
using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.UI.Admin.Jobs.Candidates.Controls
{
    public partial class ContactUploadsGrid : SearchResultsGridViewController<NullFilter>
    {
        protected override bool IsFinder => false;

        private Guid ContactId
        {
            get => (Guid)ViewState[nameof(ContactId)];
            set => ViewState[nameof(ContactId)] = value;
        }

        public void LoadData(Guid contactId)
        {
            ContactId = contactId;

            Search(new NullFilter());
        }

        protected override IListSource SelectData(NullFilter filter)
        {
            var data = TCandidateUploadSearch.SelectByContact(ContactId)
                .ApplyPaging(filter)
                .Select(x => new
                {
                    UploadIdentifier = x.UploadIdentifier,
                    UploadType = x.UploadType,
                    UploadSize = x.UploadSize,
                    UploadMime = x.UploadMime,
                    UploadUrl = UploadHelper.GetFileRelativePath(x.FileIdentifier)
                })
                .Where(x => !string.IsNullOrEmpty(x.UploadUrl))
                .ToList();

            return new SearchResultList(data);
        }

        protected override int SelectCount(NullFilter filter)
        {
            return TCandidateUploadSearch.Count(x => x.CandidateUserIdentifier == ContactId);
        }
    }
}