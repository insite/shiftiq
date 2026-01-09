using System;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Commands;

using Shift.Common;

namespace InSite.Persistence
{
    public class CommandStore : ICommandStore
    {
        public IJsonSerializer Serializer { get; private set; }

        public CommandStore(IJsonSerializer serializer)
        {
            Serializer = serializer;
        }

        public void Delete(Guid id)
        {
            using (var db = new InternalDbContext(true))
            {
                var command = db.Commands.FirstOrDefault(x => x.CommandIdentifier == id);
                if (command != null)
                {
                    db.Commands.Remove(command);
                    db.SaveChanges();
                }
            }
        }

        public bool Exists(Guid command)
        {
            using (var db = new InternalDbContext(false))
            {
                return db.Commands
                    .AsNoTracking()
                    .Any(x => x.CommandIdentifier == command);
            }
        }

        public SerializedCommand Get(Guid command)
        {
            using (var db = new InternalDbContext(false))
            {
                var entity = db.Commands
                    .AsNoTracking()
                    .Where(x => x.CommandIdentifier == command)
                    .FirstOrDefault();

                return entity ?? throw new CommandNotFoundException($"Command not found: {command}");
            }
        }

        public SerializedCommand[] GetExpired(DateTimeOffset at)
        {
            using (var db = new InternalDbContext(false))
            {
                return db.Commands
                    .AsNoTracking()
                    .Where(x => x.SendScheduled <= at && x.SendStatus == "Scheduled")
                    .OrderBy(x => x.SendScheduled).ThenBy(x => x.CommandIdentifier)
                    .ToArray();
            }
        }

        public void Insert(SerializedCommand c)
        {
            const string query = @"
INSERT INTO logs.Command
(
    AggregateIdentifier, ExpectedVersion,
    CommandIdentifier, CommandClass, CommandType, CommandData, CommandDescription,
    OriginOrganization, OriginUser,
    SendStatus, SendError,
    SendScheduled, SendStarted, SendCompleted, SendCancelled,
    BookmarkAdded, BookmarkExpired,
    RecurrenceInterval, RecurrenceUnit, RecurrenceWeekdays
)
VALUES
(
    @AggregateIdentifier, @ExpectedVersion,
    @CommandIdentifier, @CommandClass, @CommandType, @CommandData, @CommandDescription,
    @OriginOrganization, @OriginUser,
    @SendStatus, @SendError,
    @SendScheduled, @SendStarted, @SendCompleted, @SendCancelled,
    @BookmarkAdded, @BookmarkExpired,
    @RecurrenceInterval, @RecurrenceUnit, @RecurrenceWeekdays
);";

            using (var db = new InternalDbContext(false))
            using (var sqlConnection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                using (var sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    var parameters = sqlCommand.Parameters;

                    parameters.AddWithValue("AggregateIdentifier", c.AggregateIdentifier);
                    parameters.AddWithValue("ExpectedVersion", c.ExpectedVersion ?? (object)DBNull.Value);

                    parameters.AddWithValue("CommandIdentifier", c.CommandIdentifier);
                    parameters.AddWithValue("CommandClass", c.CommandClass);
                    parameters.AddWithValue("CommandType", c.CommandType);
                    parameters.AddWithValue("CommandData", c.CommandData);
                    parameters.AddWithValue("CommandDescription", c.CommandDescription ?? (object)DBNull.Value);

                    parameters.AddWithValue("OriginOrganization", c.OriginOrganization);
                    parameters.AddWithValue("OriginUser", c.OriginUser);

                    parameters.AddWithValue("SendStatus", (object)c.SendStatus ?? DBNull.Value);
                    parameters.AddWithValue("SendError", (object)c.SendError ?? DBNull.Value);

                    parameters.AddWithValue("SendScheduled", (object)c.SendScheduled ?? DBNull.Value);
                    parameters.AddWithValue("SendStarted", (object)c.SendStarted ?? DBNull.Value);
                    parameters.AddWithValue("SendCompleted", (object)c.SendCompleted ?? DBNull.Value);
                    parameters.AddWithValue("SendCancelled", (object)c.SendCancelled ?? DBNull.Value);

                    parameters.AddWithValue("BookmarkAdded", (object)c.BookmarkAdded ?? DBNull.Value);
                    parameters.AddWithValue("BookmarkExpired", (object)c.BookmarkExpired ?? DBNull.Value);

                    parameters.AddWithValue("RecurrenceInterval", (object)c.RecurrenceInterval ?? DBNull.Value);
                    parameters.AddWithValue("RecurrenceUnit", (object)c.RecurrenceUnit ?? DBNull.Value);
                    parameters.AddWithValue("RecurrenceWeekdays", (object)c.RecurrenceWeekdays ?? DBNull.Value);

                    try
                    {
                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (SqlException ex) { throw new SqlInsertException($"The command ({c.CommandType}) could not be saved.", ex); }
                }
            }
        }

        public void Update(SerializedCommand c)
        {
            const string query = @"
UPDATE logs.Command
SET CommandData = @CommandData, CommandDescription = @CommandDescription,
    SendScheduled = @SendScheduled, SendStarted = @SendStarted, SendCompleted = @SendCompleted, SendCancelled = @SendCancelled, 
    SendStatus = @SendStatus, SendError = @SendError,
    BookmarkAdded = @BookmarkAdded, BookmarkExpired = @BookmarkExpired,
    RecurrenceInterval = @RecurrenceInterval, RecurrenceUnit = @RecurrenceUnit, RecurrenceWeekdays = @RecurrenceWeekdays
WHERE CommandIdentifier = @CommandIdentifier;
";

            using (var db = new InternalDbContext(false))
            using (var sqlConnection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                using (var sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    var parameters = sqlCommand.Parameters;

                    parameters.AddWithValue("CommandIdentifier", c.CommandIdentifier);
                    parameters.AddWithValue("CommandData", c.CommandData);
                    parameters.AddWithValue("CommandDescription", c.CommandDescription ?? (object)DBNull.Value);

                    parameters.AddWithValue("SendScheduled", (object)c.SendScheduled ?? DBNull.Value);
                    parameters.AddWithValue("SendStarted", (object)c.SendStarted ?? DBNull.Value);
                    parameters.AddWithValue("SendCompleted", (object)c.SendCompleted ?? DBNull.Value);
                    parameters.AddWithValue("SendCancelled", (object)c.SendCancelled ?? DBNull.Value);

                    parameters.AddWithValue("SendStatus", (object)c.SendStatus ?? DBNull.Value);
                    parameters.AddWithValue("SendError", (object)c.SendError ?? DBNull.Value);

                    parameters.AddWithValue("BookmarkAdded", (object)c.BookmarkAdded ?? DBNull.Value);
                    parameters.AddWithValue("BookmarkExpired", (object)c.BookmarkExpired ?? DBNull.Value);

                    parameters.AddWithValue("RecurrenceInterval", (object)c.RecurrenceInterval ?? DBNull.Value);
                    parameters.AddWithValue("RecurrenceUnit", (object)c.RecurrenceUnit ?? DBNull.Value);
                    parameters.AddWithValue("RecurrenceWeekdays", (object)c.RecurrenceWeekdays ?? DBNull.Value);

                    try
                    {
                        sqlCommand.Connection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex) { throw new DbEntityException($"The command ({c.CommandType}) could not be saved.", ex); }
                }
            }
        }

        public SerializedCommand Serialize(ICommand command)
        {
            return command.Serialize(false);
        }
    }
}
