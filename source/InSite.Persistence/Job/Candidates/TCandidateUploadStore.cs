using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TCandidateUploadStore
    {
        public static void Insert(TCandidateUpload entity)
        {
            using (var db = new InternalDbContext())
            {
                if (entity.UploadIdentifier == Guid.Empty)
                    entity.UploadIdentifier = Guid.NewGuid();

                db.TCandidateUploads.Add(entity);
                db.SaveChanges();
            }
        }

        public static void Update(Guid contactId, string type, TCandidateUpload newValue)
        {
            var newValues = newValue != null ? new[] { newValue } : null;

            Update(contactId, type, newValues);
        }

        public static void Update(Guid contactId, string type, IList<TCandidateUpload> newValues)
        {
            using (var db = new InternalDbContext())
            {
                var entities = db.TCandidateUploads
                    .Where(x => x.CandidateUserIdentifier == contactId && x.UploadType == type)
                    .ToList();

                db.TCandidateUploads.RemoveRange(entities);
                db.SaveChanges();

                if (newValues != null)
                {
                    foreach (var value in newValues)
                    {
                        if (value.UploadIdentifier == Guid.Empty)
                            value.UploadIdentifier = UniqueIdentifier.Create();

                        value.CandidateUserIdentifier = contactId;
                        value.UploadType = type;

                        db.TCandidateUploads.Add(value);
                        db.SaveChanges();
                    }
                }
            }
        }

        public static void Delete(Guid key)
        {
            using (var db = new InternalDbContext())
            {
                // TODO: JobConnect
                // var entity = db.ContactUploads.FirstOrDefault(x => x.ContactUploadIdentifier == key);
                // db.ContactUploads.Remove(entity);
                // db.SaveChanges();
            }
        }

        public static void DeleteByUser(Guid user)
        {
            using (var db = new InternalDbContext())
            {
                // TODO: JobConnect
                // var entities = db.ContactUploads.Where(x => x.CandidateIdentifier == user).ToList();
                // db.ContactUploads.RemoveRange(entities);
                // db.SaveChanges();
            }
        }
    }
}
