using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Domain.Contacts;

using Shift.Common;

namespace InSite.Persistence
{
    public class QUserStore : IUserStore
    {
        public void Delete(UserDeleted e)
        {
            using (var db = new InternalDbContext())
            {
                var user = db.QUsers
                    .Where(x => x.UserIdentifier == e.AggregateIdentifier)
                    .FirstOrDefault();

                if (user == null)
                    return;

                var connections = db.QUserConnections
                    .Where(x => x.FromUserIdentifier == e.AggregateIdentifier)
                    .ToList();

                var authFactors = db.TUserAuthenticationFactors.Where(x => x.UserIdentifier == e.AggregateIdentifier).ToList();

                db.TUserAuthenticationFactors.RemoveRange(authFactors);
                db.QUserConnections.RemoveRange(connections);
                db.QUsers.Remove(user);

                db.SaveChanges();
            }
        }

        public void DeleteAll()
        {
            const string sql = @"
TRUNCATE TABLE identities.QUser
TRUNCATE TABLE identities.QUserConnection
";

            using (var db = new InternalDbContext())
                db.Database.ExecuteSqlCommand(sql);
        }

        public void DeleteAll(Guid id)
        {
            const string sql = @"
delete from identities.QUser where UserIdentifier = @User
delete from identities.QUserConnection where FromUserIdentifier = @User or ToUserIdentifier = @User
";

            using (var db = new InternalDbContext())
                db.Database.ExecuteSqlCommand(sql, new[]
                {
                    new SqlParameter("User", id)
                });
        }

        public void Insert(UserCreated e)
        {
            var entity = new QUser
            {
                UserIdentifier = e.AggregateIdentifier,
                Email = e.Email,
                FirstName = e.FirstName,
                LastName = e.LastName,
                MiddleName = e.MiddleName,
                FullName = e.FullName,
                SoundexFirstName = Pronunciation.Soundex(e.FirstName, 4, 0),
                SoundexLastName = Pronunciation.Soundex(e.LastName, 4, 0),
                TimeZone = e.TimeZone,
                UserPasswordHash = UserState.Defaults.UserPasswordHash,
                UserPasswordExpired = UserState.Defaults.UserPasswordExpired,
                AccessGrantedToCmds = UserState.Defaults.AccessGrantedToCmds,
                MultiFactorAuthentication = UserState.Defaults.MultiFactorAuthentication
            };

            using (var db = new InternalDbContext())
            {
                db.QUsers.Add(entity);
                db.SaveChanges();
            }
        }

        public void Update(UserConnected e)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QUserConnections
                    .Where(x => x.FromUserIdentifier == e.AggregateIdentifier && x.ToUserIdentifier == e.ToUserId)
                    .FirstOrDefault();

                if (entity == null)
                {
                    entity = new QUserConnection
                    {
                        FromUserIdentifier = e.AggregateIdentifier,
                        ToUserIdentifier = e.ToUserId
                    };
                    db.QUserConnections.Add(entity);
                }

                entity.IsLeader = e.IsLeader;
                entity.IsManager = e.IsManager;
                entity.IsSupervisor = e.IsSupervisor;
                entity.IsValidator = e.IsValidator;
                entity.Connected = e.Connected;

                db.SaveChanges();
            }
        }

        public void Update(UserDisconnected e)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QUserConnections
                    .Where(x => x.FromUserIdentifier == e.AggregateIdentifier && x.ToUserIdentifier == e.ToUserId)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                db.QUserConnections.Remove(entity);
                db.SaveChanges();
            }
        }

        public void Update(Change e, Action<QUser> action)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QUsers.Where(x => x.UserIdentifier == e.AggregateIdentifier).FirstOrDefault();
                if (entity == null)
                    return;

                action(entity);

                db.SaveChanges();
            }
        }
    }
}
