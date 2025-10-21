using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application;
using InSite.Application.Contacts.Read;
using InSite.Application.Users.Write;

namespace InSite.Persistence
{
    public static class UserConnectionStore
    {
        private static ICommander _commander;

        public static void Initialize(ICommander commander)
        {
            _commander = commander;
        }

        public static void Save(QUserConnection c)
        {
            _commander.Send(new ConnectUser(c.FromUserIdentifier, c.ToUserIdentifier, c.IsLeader, c.IsManager, c.IsSupervisor, c.IsValidator, c.Connected));
        }

        public static void Delete(UserConnection c)
        {
            _commander.Send(new DisconnectUser(c.FromUserIdentifier, c.ToUserIdentifier));
        }

        public static void Delete(Guid from, Guid to)
        {
            _commander.Send(new DisconnectUser(from, to));
        }

        public static void DeleteDownstream(Guid from)
        {
            List<QUserConnection> connections;

            using (var db = new InternalDbContext())
            {
                connections = db.QUserConnections
                    .Where(x => x.FromUserIdentifier == from)
                    .ToList();
            }

            foreach (var c in connections)
                _commander.Send(new DisconnectUser(c.FromUserIdentifier, c.ToUserIdentifier));
        }

        public static void DeleteUpstream(Guid to)
        {
            List<QUserConnection> connections;

            using (var db = new InternalDbContext())
            {
                connections = db.QUserConnections
                    .Where(x => x.ToUserIdentifier == to)
                    .ToList();
            }

            foreach (var c in connections)
                _commander.Send(new DisconnectUser(c.FromUserIdentifier, c.ToUserIdentifier));
        }
    }
}
