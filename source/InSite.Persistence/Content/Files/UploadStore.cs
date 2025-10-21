using System;
using System.Collections.Generic;
using System.Data.Entity;

using Shift.Common;

using Shift.Constant;

namespace InSite.Persistence
{
    public static class UploadStore
    {
        public static void Commit(List<Upload> insert, List<Upload> update, List<Upload> delete)
        {
            using (var db = new InternalDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var entity in delete)
                        {
                            db.Uploads.Attach(entity);
                            db.Entry(entity).State = EntityState.Deleted;
                        }

                        db.SaveChanges();

                        foreach (var entity in insert)
                            db.Uploads.Add(entity);

                        foreach (var entity in update)
                        {
                            db.Uploads.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                        }

                        db.SaveChanges();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();

                        throw;
                    }
                }
            }
        }

        public static void DeleteLink(Guid uploadId)
        {
            using (var db = new InternalDbContext())
            {
                var entity = new Upload { UploadIdentifier = uploadId };
                db.Entry(entity).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }

        public static Upload InsertLink(Upload upload)
        {
            using (var db = new InternalDbContext())
            {
                upload.UploadType = UploadType.Link;
                upload.UploadPrivacyScope = "Platform";

                if (upload.UploadIdentifier == Guid.Empty)
                    upload.UploadIdentifier = UniqueIdentifier.Create();

                db.Uploads.Add(upload);
                db.SaveChanges();
                return upload;
            }
        }

        public static Upload UpdateLink(Upload entity)
        {
            using (var context = new InternalDbContext())
            {
                context.Entry(entity).State = EntityState.Modified;
                context.SaveChanges();
                return entity;
            }
        }
    }
}
