using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Domain.Contacts;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    public class QPersonSecretStore : IPersonSecretStore
    {
        public void Insert(PersonSecretAdded e)
        {
            var entity = new QPersonSecret
            {
                SecretIdentifier = e.AggregateIdentifier,
                PersonIdentifier = e.PersonId,
                SecretExpiry = e.Expiry,
                SecretLifetimeLimit = e.Lifetime,
                SecretName = e.Name,
                SecretType = e.Type,
                SecretValue = e.Value,
            };

            using (var db = new InternalDbContext())
            {
                db.QPersonSecrets.Add(entity);
                db.SaveChanges();
            }
        }

        public void Delete(PersonSecretRemoved e)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QPersonSecrets
                    .Where(x => x.SecretIdentifier == e.AggregateIdentifier)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                db.QPersonSecrets.Remove(entity);
                db.SaveChanges();
            }
        }
    }
}
