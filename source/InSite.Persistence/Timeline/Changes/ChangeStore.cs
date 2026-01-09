using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

using Shift.Common.Timeline.Changes;

using Shift.Common;

using Shift.Toolbox;

namespace InSite.Persistence
{
    public class ChangeStore : IChangeStore
    {
        private string DatabaseConnectionString { get; set; }

        private string OfflineStorageFolder { get; set; }

        public IJsonSerializer Serializer { get; private set; }

        private readonly HashSet<string> _obsoleteChangeTypes;

        public ChangeStore(IJsonSerializer serializer, string databaseConnectionString, string offlineStorageFolder)
        {
            Serializer = serializer;
            DatabaseConnectionString = databaseConnectionString;
            OfflineStorageFolder = offlineStorageFolder;

            _obsoleteChangeTypes = new HashSet<string>();
        }

        public void Box(Guid aggregate, bool archive = true)
        {
            GetClassAndOrganization(aggregate, out string aggregateClass, out Guid aggregateOrganization);

            IEnumerable<SerializedChange> changes = null;

            if (archive)
                changes = GetSerializedChanges(aggregate, -1);

            // Delete the aggregate and the changes from the online logs.
            var deleted = Delete(aggregate);

            if (archive)
            {
                // Create a new directory using the aggregate identifier as the folder name.
                var path = Path.Combine(OfflineStorageFolder, aggregate.ToString());
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                // Serialize the change stream and write it to an external file.
                var json = Serializer.Serialize(changes);
                var file = Path.Combine(path, "Changes.json");
                File.WriteAllText(file, json, Encoding.Unicode);
                var info = new FileInfo(file);

                // Create a metadata file to describe the boxed aggregated.
                var meta = new StringBuilder();
                meta.AppendLine($"Aggregate Identifier : {aggregate}");
                meta.AppendLine($"     Aggregate Class : {aggregateClass}");
                meta.AppendLine($"    Aggregate Tenant : {aggregateOrganization}");
                meta.AppendLine($"  Serialized Changes : {changes.Count():n0}");
                meta.AppendLine($"Deleted Log Entries  : {deleted:n0}");
                meta.AppendLine($"    Date/Time Boxed  : {DateTime.Now:dddd, MMMM d, yyyy HH:mm} Local Time");
                meta.AppendLine($"                     : {DateTimeOffset.UtcNow:dddd, MMMM d, yyyy HH:mm} UTC");
                file = Path.Combine(path, "Metadata.txt");
                File.WriteAllText(file, meta.ToString());

                // Write an index entry for the boxed aggregate.
                var index = Path.Combine(OfflineStorageFolder, "Boxes.csv");
                File.AppendAllText(index, $"{DateTime.Now:yyyy/MM/dd-HH:mm},{aggregate},{aggregateClass},{info.Length / 1024} KB,{aggregateOrganization}\n");
            }
        }

        public void Unbox(Guid aggregate, Func<Guid, AggregateRoot> creator)
        {
            GetClassAndOrganization(aggregate, out string aggregateClass, out Guid aggregateOrganization);

            // Create a new directory using the aggregate identifier as the folder name.
            var path = Path.Combine(OfflineStorageFolder, aggregate.ToString());

            // Deserialize the change stream and write it to an external file.
            var file = Path.Combine(path, "Changes.json");
            var json = File.ReadAllText(file, Encoding.Unicode);
            var changes = Serializer.Deserialize<List<SerializedChange>>(json)
                .Select(x => x.Deserialize())
                .ToList();

            // Save the changes to the database.
            Save(creator(aggregate), changes);
        }

        public bool Exists<T>(Guid aggregate)
        {
            const string query = @"SELECT TOP 1 1 FROM logs.[Aggregate] WHERE AggregateIdentifier = @AggregateIdentifier AND AggregateClass = @AggregateClass";

            var aggClass = Serializer.GetClassName(typeof(T));

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var select = new SqlCommand(query, connection))
                {
                    select.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    select.Parameters.AddWithValue("AggregateClass", aggClass);

                    object o = select.ExecuteScalar();
                    return o != null && o != DBNull.Value;
                }
            }
        }

        public bool Exists(Guid aggregate, int version)
        {
            const string query = @"SELECT TOP 1 1 FROM logs.Aggregate WHERE AggregateIdentifier = @AggregateIdentifier AND AggregateVersion = @AggregateVersion";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();
                using (var select = new SqlCommand(query, connection))
                {
                    select.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    select.Parameters.AddWithValue("AggregateVersion", version);
                    object o = select.ExecuteScalar();
                    return o != null && o != DBNull.Value;
                }
            }
        }

        public int Count(Guid aggregate, int fromVersion = -1)
        {
            const string text = @"
SELECT 
    COUNT(*)
FROM 
    logs.Change 
WHERE
    AggregateIdentifier = @AggregateIdentifier AND AggregateVersion > @AggregateVersion;";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(text, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    command.Parameters.AddWithValue("AggregateVersion", fromVersion);

                    return (int)command.ExecuteScalar();
                }
            }
        }

        public int Count(Guid aggregate, string keyword, bool includeChildren)
        {
            var where = includeChildren
                ? "a.RootAggregateIdentifier = @AggregateIdentifier"
                : "e.AggregateIdentifier = @AggregateIdentifier";

            if (!string.IsNullOrEmpty(keyword))
                where += " AND (u.FullName LIKE @Keyword OR e.ChangeType LIKE @Keyword OR e.ChangeData LIKE @Keyword)";

            var joinAggregate = includeChildren ? "INNER JOIN logs.Aggregate AS a ON a.AggregateIdentifier = e.AggregateIdentifier" : "";

            var query = $@"
SELECT 
    COUNT(*)
FROM 
    logs.Change AS e
    LEFT JOIN identities.[User] AS u ON u.UserIdentifier = e.OriginUser
    {joinAggregate}
WHERE
    {where}
";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);

                    if (!string.IsNullOrEmpty(keyword))
                        command.Parameters.AddWithValue("Keyword", $"%{keyword}%");

                    return (int)command.ExecuteScalar();
                }
            }
        }

        public int Count(string aggregateType, IEnumerable<Guid> aggregateIdentifiers)
        {
            var sql = $@"
SELECT 
    COUNT(*)
FROM 
    logs.Change 
WHERE
    AggregateIdentifier IN ('{string.Join("','", aggregateIdentifiers)}')
    AND AggregateIdentifier IN (SELECT AggregateIdentifier FROM logs.[Aggregate] WHERE AggregateType = @AggregateType);";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("AggregateType", aggregateType);

                    return (int)command.ExecuteScalar();
                }
            }
        }

        public List<Guid> GetAggregates(string aggregateType)
        {
            string text = @"
SELECT 
    AggregateIdentifier
FROM 
    logs.Aggregate 
WHERE
    AggregateType = @AggregateType
";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(text, connection))
                {
                    command.CommandTimeout = 60 * 5;
                    command.Parameters.AddWithValue("AggregateType", aggregateType);

                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<Guid>();

                        while (reader.Read())
                            list.Add(reader.GetGuid(0));

                        return list;
                    }
                }
            }
        }

        public IChange[] GetChanges(Guid aggregate, int fromVersion)
        {
            return GetSerializedChanges(aggregate, fromVersion)
                .Select(x => Deserialize(x))
                .ToArray();
        }

        public IChange[] GetChanges(Guid aggregate, int fromVersion, int toVersion)
        {
            return GetSerializedChanges(aggregate, fromVersion, toVersion)
                .Select(x => Deserialize(x))
                .ToArray();
        }

        public IChange[] GetChanges(string aggregateType, Guid? aggregateIdentifier, IEnumerable<string> changeTypes)
        {
            return GetSerializedChanges(aggregateType, aggregateIdentifier, changeTypes)
                .Select(x => Deserialize(x))
                .ToArray();
        }

        public List<IChange> GetChanges(string aggregateType, IEnumerable<Guid> aggregateIdentifiers)
        {
            return EnumerateSerializedChanges(aggregateType, aggregateIdentifiers)
                .Select(x => Deserialize(x))
                .ToList();
        }

        public IChange[] GetChangesPaged(Guid aggregate, string keyword, bool includeChildren, int skip, int pageSize)
        {
            return GetSerializedChangesPaged(aggregate, keyword, includeChildren, skip, pageSize)
                .Select(x => Deserialize(x))
                .ToArray();
        }

        public IChange GetChange(Guid aggregate, int version)
        {
            var e = GetSerializedChange(aggregate, version);

            return e == null ? null : Deserialize(e);
        }

        public Guid[] GetExpired(DateTimeOffset at)
        {
            using (var db = new InternalDbContext(false))
            {
                return db.Aggregates
                    .AsNoTracking()
                    .Where(x => x.AggregateExpires != null && x.AggregateExpires <= at)
                    .Select(x => x.AggregateIdentifier)
                    .ToArray();
            }
        }

        private IChange Deserialize(SerializedChange change)
        {
            return !_obsoleteChangeTypes.Contains(change.ChangeType)
                ? change.Deserialize()
                : change;
        }

        private List<SerializedChange> GetSerializedChanges(Guid aggregate, int fromVersion)
        {
            string query = string.Empty;

            query = BuildChangeQuery("AggregateIdentifier = @AggregateIdentifier AND AggregateVersion > @AggregateVersion", "AggregateVersion");

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    command.Parameters.AddWithValue("AggregateVersion", fromVersion);
                    command.CommandTimeout = 3 * 60;
                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<SerializedChange>();

                        while (reader.Read())
                        {
                            var item = ReadSerializedChange(reader);
                            list.Add(item);
                        }

                        return list;
                    }
                }
            }
        }

        public List<SerializedChange> GetSerializedChangesPaged(Guid aggregate, string keyword, bool includeChildren, int skip, int pageSize)
        {
            var where = includeChildren
                ? "a.RootAggregateIdentifier = @AggregateIdentifier"
                : "e.AggregateIdentifier = @AggregateIdentifier";

            var orderBy = includeChildren ? "ChangeTime DESC, AggregateVersion DESC" : "AggregateVersion DESC";

            var query = BuildChangeQuery(where, orderBy, null, includeChildren, keyword, Paging.SetSkipTake(skip, pageSize));

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);

                    if (!string.IsNullOrEmpty(keyword))
                        command.Parameters.AddWithValue("Keyword", $"%{keyword}%");

                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<SerializedChange>();

                        while (reader.Read())
                        {
                            var item = ReadSerializedChange(reader);
                            list.Add(item);
                        }

                        return list;
                    }
                }
            }
        }

        private List<SerializedChange> GetSerializedChanges(Guid aggregate, int fromVersion, int toVersion)
        {
            var query = BuildChangeQuery("AggregateIdentifier = @AggregateIdentifier AND AggregateVersion BETWEEN @FromAggregateVersion AND @ToAggregateVersion", "AggregateVersion");

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    command.Parameters.AddWithValue("FromAggregateVersion", fromVersion);
                    command.Parameters.AddWithValue("ToAggregateVersion", toVersion);

                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<SerializedChange>();

                        while (reader.Read())
                        {
                            var item = ReadSerializedChange(reader);
                            list.Add(item);
                        }

                        return list;
                    }
                }
            }
        }

        private List<SerializedChange> GetSerializedChanges(string aggregateType, Guid? aggregateIdentifier, IEnumerable<string> changeTypes)
        {
            var where = @"
(@AggregateIdentifier IS NULL OR AggregateIdentifier = @AggregateIdentifier)
AND AggregateIdentifier IN (SELECT AggregateIdentifier FROM logs.[Aggregate] WHERE AggregateType = @AggregateType)";

            if (changeTypes != null && changeTypes.Any())
                where += $" AND ChangeType IN ('{string.Join("','", changeTypes)}')";

            var query = BuildChangeQuery(where, "AggregateVersion");

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandTimeout = 60 * 5;
                    command.Parameters.AddWithValue("AggregateType", aggregateType);
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregateIdentifier ?? (object)DBNull.Value);

                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<SerializedChange>();

                        while (reader.Read())
                        {
                            var item = ReadSerializedChange(reader);
                            list.Add(item);
                        }

                        return list;
                    }
                }
            }
        }

        private IEnumerable<SerializedChange> EnumerateSerializedChanges(string aggregateType, IEnumerable<Guid> aggregateIdentifiers)
        {
            var where = $@"
AggregateIdentifier IN ('{string.Join("','", aggregateIdentifiers)}')
AND AggregateIdentifier IN (SELECT AggregateIdentifier FROM logs.[Aggregate] WHERE AggregateType = @AggregateType)";

            var query = BuildChangeQuery(where, "ChangeTime,AggregateVersion");

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandTimeout = 60 * 5;
                    command.Parameters.AddWithValue("AggregateType", aggregateType);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                            yield return ReadSerializedChange(reader);
                    }
                }
            }
        }

        private SerializedChange GetSerializedChange(Guid aggregate, int version)
        {
            var query = BuildChangeQuery("AggregateIdentifier = @AggregateIdentifier AND AggregateVersion = @AggregateVersion");

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    command.Parameters.AddWithValue("AggregateVersion", version);

                    using (var reader = command.ExecuteReader())
                        return reader.Read() ? ReadSerializedChange(reader) : null;
                }
            }
        }

        public void Save(AggregateRoot aggregate, IEnumerable<IChange> changes)
        {
            var list = new List<SerializedChange>();

            foreach (var e in changes)
            {
                var item = e.Serialize(aggregate.AggregateIdentifier, e.AggregateVersion);

                list.Add(item);
            }

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    EnsureAggregateExists(
                        changes.First().OriginOrganization,
                        aggregate.AggregateIdentifier,
                        aggregate.RootAggregateIdentifier,
                        GetAggregateType(aggregate),
                        Serializer.GetClassName(aggregate.GetType()),
                        connection,
                        transaction
                    );

                    if (list.Count > 1)
                        InsertChanges(list, connection, transaction);
                    else
                        InsertChange(list[0], connection, transaction);

                    transaction.Commit();
                }
            }
        }

        public static string GetAggregateType(AggregateRoot aggregate)
        {
            return GetAggregateType(aggregate.GetType());
        }

        public static string GetAggregateType(Type aggregateType)
        {
            return aggregateType.Name.Replace("Aggregate", string.Empty);
        }

        public void Save(IEnumerable<AggregateImport> imports)
        {
            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var list = new List<SerializedChange>();

                    foreach (var import in imports)
                    {
                        var aggregate = import.Aggregate;
                        var changes = import.Changes;

                        foreach (var e in changes)
                            list.Add(e.Serialize(aggregate.AggregateIdentifier, e.AggregateVersion));

                        EnsureAggregateExists(
                            changes.First().OriginOrganization,
                            aggregate.AggregateIdentifier,
                            aggregate.RootAggregateIdentifier,
                            GetAggregateType(aggregate),
                            Serializer.GetClassName(aggregate.GetType()),
                            connection,
                            transaction
                        );
                    }

                    if (list.Count > 1)
                        InsertChanges(list, connection, transaction);
                    else
                        InsertChange(list[0], connection, transaction);

                    transaction.Commit();
                }
            }
        }

        public void Save(IChange change)
        {
            var item = change.Serialize(change.AggregateIdentifier, change.AggregateVersion);

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    UpdateChange(item, connection, transaction);

                    transaction.Commit();
                }
            }
        }

        public void Insert(IChange change, int index)
        {
            var item = change.Serialize(change.AggregateIdentifier, change.AggregateVersion);

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    // Before we insert the new change we need to increment the version number for all subsequent changes.

                    const string query = @"
update logs.Change 
set AggregateVersion = AggregateVersion + 1
where AggregateIdentifier = @AggregateIdentifier and AggregateVersion >= @InsertIndex;
";

                    using (var command = new SqlCommand(query, connection, transaction))
                    {
                        var parameters = command.Parameters;

                        parameters.AddWithValue("AggregateIdentifier", change.AggregateIdentifier);
                        parameters.AddWithValue("@InsertIndex", index);

                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch (SqlException ex) { throw new SqlInsertException($"The change ({change.GetType().Name}) could not be inserted.", ex); }
                    }

                    // Now we can insert the new change without getting a primary key violation.

                    InsertChange(item, connection, transaction);

                    transaction.Commit();
                }
            }
        }

        #region Methods (read helpers)

        private static string BuildChangeQuery(string where, string orderBy = null, string selectClause = null, bool joinAggregate = false, string keyword = null, Paging paging = null)
        {
            var query = new StringBuilder();

            query.Append("SELECT ");

            if (!string.IsNullOrEmpty(selectClause))
                query.Append(selectClause);

            query.Append(@"
    e.AggregateIdentifier
   ,e.AggregateVersion
   ,e.ChangeTime
   ,e.ChangeType
   ,e.ChangeData
   ,e.OriginOrganization
   ,e.OriginUser
FROM 
    logs.Change AS e
");

            if (joinAggregate)
            {
                query.Append(" INNER JOIN logs.Aggregate AS a ON a.AggregateIdentifier = e.AggregateIdentifier");
            }

            if (!string.IsNullOrEmpty(keyword))
                query.Append(" LEFT JOIN identities.[User] AS u ON u.UserIdentifier = e.OriginUser");

            if (!string.IsNullOrEmpty(where) || !string.IsNullOrEmpty(keyword))
            {
                query.Append(" WHERE ");

                if (!string.IsNullOrEmpty(where))
                    query.Append($"({where})");

                if (!string.IsNullOrEmpty(keyword))
                    query.Append(" AND (u.FullName LIKE @Keyword OR e.ChangeType LIKE @Keyword OR e.ChangeData LIKE @Keyword)");
            }

            if (!string.IsNullOrEmpty(orderBy))
                query.Append(" ORDER BY ").Append(orderBy);

            if (paging != null)
                query.Append($" OFFSET {paging.Skip} ROWS FETCH NEXT {paging.Take} ROWS ONLY");

            query.Append(";");

            return query.ToString();
        }

        private static SerializedChange ReadSerializedChange(SqlDataReader reader)
        {
            return new SerializedChange
            {
                AggregateIdentifier = reader.GetGuid(0),
                AggregateVersion = reader.GetInt32(1),
                ChangeTime = reader.GetDateTimeOffset(2),
                ChangeType = reader.GetString(3),
                ChangeData = reader.GetString(4),
                OriginOrganization = reader.GetGuid(5),
                OriginUser = reader.GetGuid(6)
            };
        }

        #endregion

        #region Methods (insert, update, delete)

        private int Delete(Guid aggregate)
        {
            const string query = @"
DELETE FROM logs.Aggregate WHERE AggregateIdentifier = @AggregateIdentifier;
DELETE FROM logs.Change WHERE AggregateIdentifier = @AggregateIdentifier;
";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandTimeout = 3 * 60;
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public int Rollback(Guid aggregate, int version)
        {
            const string query = @"
DELETE FROM logs.Change WHERE AggregateIdentifier = @AggregateIdentifier AND AggregateVersion >= @AggregateVersion;
";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.CommandTimeout = 3 * 60;
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    command.Parameters.AddWithValue("AggregateVersion", version);
                    return command.ExecuteNonQuery();
                }
            }
        }

        public void RegisterObsoleteChangeTypes(IEnumerable<string> changeTypes)
        {
            foreach (var changeType in changeTypes)
                _obsoleteChangeTypes.Add(changeType);
        }

        private void InsertChange(SerializedChange e, SqlConnection connection, SqlTransaction transaction)
        {
            const string query = @"
INSERT INTO logs.Change
(
    AggregateIdentifier, AggregateVersion,
    ChangeType, ChangeData,
    OriginOrganization, OriginUser,
    ChangeTime
)
VALUES
( @AggregateIdentifier, @AggregateVersion, @ChangeType, @ChangeData, @OriginOrganization, @OriginUser, @ChangeTime )";

            using (var command = new SqlCommand(query, connection, transaction))
            {
                var parameters = command.Parameters;

                parameters.AddWithValue("AggregateIdentifier", e.AggregateIdentifier);
                parameters.AddWithValue("AggregateVersion", e.AggregateVersion);

                parameters.AddWithValue("ChangeType", e.ChangeType);
                parameters.AddWithValue("ChangeData", e.ChangeData);

                parameters.AddWithValue("OriginOrganization", e.OriginOrganization);
                parameters.AddWithValue("OriginUser", e.OriginUser);

                parameters.AddWithValue("ChangeTime", e.ChangeTime);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex) { throw new SqlInsertException($"The change ({e.ChangeType}) could not be saved.", ex); }
            }
        }

        private void InsertChanges(List<SerializedChange> changes, SqlConnection connection, SqlTransaction transaction)
        {
            var table = new DataTable();

            table.Columns.Add("AggregateIdentifier", typeof(Guid));
            table.Columns.Add("AggregateVersion", typeof(int));

            table.Columns.Add("OriginOrganization", typeof(Guid));
            table.Columns.Add("OriginUser", typeof(Guid));

            table.Columns.Add("ChangeTime", typeof(DateTimeOffset));
            table.Columns.Add("ChangeType", typeof(string));
            table.Columns.Add("ChangeData", typeof(string));

            foreach (var e in changes)
            {
                var row = table.NewRow();
                row["AggregateIdentifier"] = e.AggregateIdentifier;
                row["AggregateVersion"] = e.AggregateVersion;
                row["OriginOrganization"] = e.OriginOrganization;
                row["OriginUser"] = e.OriginUser;
                row["ChangeTime"] = e.ChangeTime;
                row["ChangeType"] = e.ChangeType;
                row["ChangeData"] = e.ChangeData;
                table.Rows.Add(row);
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.BatchSize = 5000;
                bulkCopy.DestinationTableName = "logs.Change";
                bulkCopy.WriteToServer(table);
            }
        }

        private void UpdateChange(SerializedChange e, SqlConnection connection, SqlTransaction transaction)
        {
            const string query = @"
UPDATE logs.Change SET 
ChangeType = @ChangeType, ChangeData = @ChangeData,
OriginOrganization = @OriginOrganization, OriginUser = @OriginUser
WHERE AggregateIdentifier = @AggregateIdentifier AND AggregateVersion = @AggregateVersion";

            using (var command = new SqlCommand(query, connection, transaction))
            {
                var parameters = command.Parameters;

                parameters.AddWithValue("AggregateIdentifier", e.AggregateIdentifier);
                parameters.AddWithValue("AggregateVersion", e.AggregateVersion);

                parameters.AddWithValue("ChangeType", e.ChangeType);
                parameters.AddWithValue("ChangeData", e.ChangeData);

                parameters.AddWithValue("OriginOrganization", e.OriginOrganization);
                parameters.AddWithValue("OriginUser", e.OriginUser);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex) { throw new SqlInsertException($"The change ({e.ChangeType}) could not be saved.", ex); }
            }
        }

        #endregion

        #region Methods (lookup)

        private void EnsureAggregateExists(Guid organization, Guid aggregate, Guid rootAggregate, string name, string type, SqlConnection connection, SqlTransaction transaction)
        {
            const string query = @"
IF NOT EXISTS(SELECT TOP 1 1 FROM logs.Aggregate WHERE AggregateIdentifier = @AggregateIdentifier)
  BEGIN
    INSERT INTO logs.Aggregate (OriginOrganization, AggregateIdentifier, RootAggregateIdentifier, AggregateType, AggregateClass) VALUES (@OriginOrganization, @AggregateIdentifier, @RootAggregateIdentifier, @AggregateType, @AggregateClass);
  END";

            if (rootAggregate == Guid.Empty)
                rootAggregate = aggregate;

            using (var insert = new SqlCommand(query, connection, transaction))
            {
                insert.Parameters.AddWithValue("OriginOrganization", organization);
                insert.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                insert.Parameters.AddWithValue("RootAggregateIdentifier", rootAggregate);
                insert.Parameters.AddWithValue("AggregateType", name);
                insert.Parameters.AddWithValue("AggregateClass", type);

                insert.ExecuteNonQuery();
            }
        }

        public void GetClassAndOrganization(Guid aggregate, out string @class, out Guid organization)
        {
            @class = null;
            organization = Guid.Empty;

            const string text = @"select AggregateClass, OriginOrganization from logs.Aggregate where AggregateIdentifier = @AggregateIdentifier";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(text, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            @class = reader.GetString(0);
                            organization = reader.GetGuid(1);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
