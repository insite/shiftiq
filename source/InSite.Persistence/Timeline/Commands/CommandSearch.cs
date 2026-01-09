using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Logs.Read;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class CommandSearch : ICommandSearch
    {
        public IJsonSerializer _serializer;

        public CommandSearch(IJsonSerializer serializer)
        {
            _serializer = serializer;
        }

        public int Count(CommandFilter filter)
        {
            using (var db = new InternalDbContext(false))
            {
                return CreateQueryable(db, filter).Count();
            }
        }

        public ICommand GetCommand(Guid command)
        {
            var serialized = Get(command);
            if (serialized != null)
                return serialized.Deserialize(false);
            return null;
        }

        public SerializedCommand Get(Guid command)
        {
            using (var db = new InternalDbContext(false))
            {
                return db.Commands.FirstOrDefault(x => x.CommandIdentifier == command);
            }
        }

        public List<SerializedCommand> Get(CommandFilter filter)
        {
            using (var db = new InternalDbContext(false))
            {
                return CreateQueryable(db, filter)
                    .OrderByDescending(x => x.SendScheduled)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private IQueryable<SerializedCommand> CreateQueryable(InternalDbContext db, CommandFilter filter)
        {
            var q = db.Commands
                .AsNoTracking()
                .AsExpandable();

            if (filter.OriginOrganization.HasValue)
                q = q.Where(x => x.OriginOrganization == filter.OriginOrganization);

            if (filter.OriginUser.HasValue)
                q = q.Where(x => x.OriginUser == filter.OriginUser);

            if (!string.IsNullOrEmpty(filter.CommandClass))
                q = q.Where(x => x.CommandClass.Contains(filter.CommandClass));

            if (!string.IsNullOrEmpty(filter.CommandData))
                q = q.Where(x => x.CommandData.Contains(filter.CommandData));

            if (!string.IsNullOrEmpty(filter.CommandType))
                q = q.Where(x => x.CommandType.Contains(filter.CommandType));

            if (!string.IsNullOrEmpty(filter.SendError))
                q = q.Where(x => x.SendError.Contains(filter.SendError));

            if (filter.IsRecurring.HasValue)
            {
                if (filter.IsRecurring.Value)
                    q = q.Where(x => x.RecurrenceInterval.HasValue && x.RecurrenceUnit != null);
                else
                    q = q.Where(x => (!x.RecurrenceInterval.HasValue || x.RecurrenceUnit == null));
            }

            if (filter.CommandState.HasValue)
            {
                if (filter.CommandState == CommandState.Scheduled)
                    q = q.Where(x => x.SendScheduled != null && x.SendStatus == "Scheduled");
                else if (filter.CommandState == CommandState.Started)
                    q = q.Where(x => x.SendStarted != null && x.SendStatus == "Started");
                else if (filter.CommandState == CommandState.Completed)
                    q = q.Where(x => x.SendCompleted != null && x.SendStatus == "Completed");
                else if (filter.CommandState == CommandState.Cancelled)
                    q = q.Where(x => x.SendCancelled != null && x.SendStatus == "Cancelled");
                else if (filter.CommandState == CommandState.Bookmarked)
                    q = q.Where(x => x.BookmarkAdded != null);
            }

            if (filter.SendScheduled != null)
            {
                if (filter.SendScheduled.Since.HasValue)
                    q = q.Where(x => x.SendScheduled >= filter.SendScheduled.Since.Value);

                if (filter.SendScheduled.Before.HasValue)
                    q = q.Where(x => x.SendScheduled < filter.SendScheduled.Before.Value);
            }

            if (filter.SendStarted != null)
            {
                if (filter.SendStarted.Since.HasValue)
                    q = q.Where(x => x.SendStarted >= filter.SendStarted.Since.Value);

                if (filter.SendStarted.Before.HasValue)
                    q = q.Where(x => x.SendStarted < filter.SendStarted.Before.Value);
            }

            if (filter.SendCompleted != null)
            {
                if (filter.SendCompleted.Since.HasValue)
                    q = q.Where(x => x.SendCompleted >= filter.SendCompleted.Since.Value);

                if (filter.SendCompleted.Before.HasValue)
                    q = q.Where(x => x.SendCompleted < filter.SendCompleted.Before.Value);
            }

            if (filter.SendCancelled != null)
            {
                if (filter.SendCancelled.Since.HasValue)
                    q = q.Where(x => x.SendCancelled >= filter.SendCancelled.Since.Value);

                if (filter.SendCancelled.Before.HasValue)
                    q = q.Where(x => x.SendCancelled < filter.SendCancelled.Before.Value);
            }

            if (filter.BookmarkAdded != null)
            {
                if (filter.BookmarkAdded.Since.HasValue)
                    q = q.Where(x => x.BookmarkAdded >= filter.BookmarkAdded.Since.Value);

                if (filter.BookmarkAdded.Before.HasValue)
                    q = q.Where(x => x.BookmarkAdded < filter.BookmarkAdded.Before.Value);
            }

            if (filter.BookmarkExpired != null)
            {
                if (filter.BookmarkExpired.Since.HasValue)
                    q = q.Where(x => x.BookmarkExpired >= filter.BookmarkExpired.Since.Value);

                if (filter.BookmarkExpired.Before.HasValue)
                    q = q.Where(x => x.BookmarkExpired < filter.BookmarkExpired.Before.Value);
            }

            return q;
        }
    }
}
