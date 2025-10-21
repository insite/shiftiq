using System;

namespace InSite.Persistence
{
    public class TCandidateUpload
    {
        public Guid? CandidateUserIdentifier { get; set; }
        public Guid UploadIdentifier { get; set; }
        public Guid FileIdentifier { get; set; }

        public string UploadMime { get; set; }
        public string UploadType { get; set; }
        public string UploadName { get; set; }

        public int UploadSize { get; set; }

        public virtual User Candidate { get; set; }
    }
}
