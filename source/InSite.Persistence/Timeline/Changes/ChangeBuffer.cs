using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Persistence
{
    public class ChangeBuffer : IChangeBuffer
    {
        private string _connectionString { get; set; }
        private SqlConnection _connection;
        private DataTable _changes;
        private IJsonSerializer _serializer { get; set; }

        public ChangeBuffer(IJsonSerializer serializer, string connectionString)
        {
            _serializer = serializer;
            _connectionString = connectionString;
        }

        public void Open()
        {
            _connection = new SqlConnection(_connectionString);
            _connection.Open();

            using (var tx = _connection.BeginTransaction(IsolationLevel.Serializable))
            {
                Initialize(tx);
                tx.Commit();
            }
        }

        private void Initialize(SqlTransaction tx)
        {
            _changes = new DataTable();

            _changes.Columns.Add("AggregateIdentifier", typeof(Guid));
            _changes.Columns.Add("AggregateVersion", typeof(int));

            _changes.Columns.Add("OriginOrganization", typeof(Guid));
            _changes.Columns.Add("OriginUser", typeof(Guid));

            _changes.Columns.Add("ChangeTime", typeof(DateTimeOffset));
            _changes.Columns.Add("ChangeType", typeof(string));
            _changes.Columns.Add("ChangeData", typeof(string));

            const string query = @"
TRUNCATE TABLE logs.AggregateBuffer;
TRUNCATE TABLE logs.ChangeBuffer;
";
            using (var truncate = new SqlCommand(query, _connection, tx))
                truncate.ExecuteNonQuery();
        }

        public void Save(AggregateRoot aggregate, IEnumerable<IChange> changes)
        {
            var list = new List<SerializedChange>();
            foreach (var c in changes)
                list.Add(c.Serialize(aggregate.AggregateIdentifier, c.AggregateVersion));

            InsertAggregate(changes.First().OriginOrganization, aggregate.AggregateIdentifier, ChangeStore.GetAggregateType(aggregate), _serializer.GetClassName(aggregate.GetType()));
            InsertChanges(list);
        }

        private void InsertAggregate(Guid organization, Guid aggregate, string name, string type)
        {
            const string query = @"
INSERT INTO logs.AggregateBuffer (OriginOrganization, AggregateIdentifier, AggregateType, AggregateClass) VALUES (@OriginOrganization, @AggregateIdentifier, @AggregateType, @AggregateClass);
";
            using (var insert = new SqlCommand(query, _connection))
            {
                insert.Parameters.AddWithValue("OriginOrganization", organization);
                insert.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                insert.Parameters.AddWithValue("AggregateType", name);
                insert.Parameters.AddWithValue("AggregateClass", type);
                insert.ExecuteNonQuery();
            }
        }

        private void InsertChanges(List<SerializedChange> changes)
        {
            foreach (var e in changes)
            {
                var row = _changes.NewRow();
                row["AggregateIdentifier"] = e.AggregateIdentifier;
                row["AggregateVersion"] = e.AggregateVersion;
                row["OriginOrganization"] = e.OriginOrganization;
                row["OriginUser"] = e.OriginUser;
                row["ChangeTime"] = e.ChangeTime;
                row["ChangeType"] = e.ChangeType;
                row["ChangeData"] = e.ChangeData;
                _changes.Rows.Add(row);
            }
        }

        public void Flush()
        {
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_connection))
            {
                bulkCopy.BatchSize = 5000;
                bulkCopy.DestinationTableName = "logs.ChangeBuffer";
                bulkCopy.WriteToServer(_changes);
            }

            const string query = @"
INSERT INTO logs.[Aggregate]
(
      AggregateIdentifier
    , AggregateType
    , AggregateClass
    , AggregateExpires
    , AchievementUpgraded
    , OriginOrganization
)
    SELECT
          AggregateIdentifier
        , AggregateType
        , AggregateClass
        , AggregateExpires
        , SYSDATETIMEOFFSET()
        , OriginOrganization
    FROM
        logs.AggregateBuffer;

INSERT INTO logs.[Change]
(
      AggregateIdentifier
    , AggregateVersion
    , OriginOrganization
    , OriginUser
    , ChangeTime
    , ChangeType
    , ChangeData
)
    SELECT 
          AggregateIdentifier
        , AggregateVersion
        , OriginOrganization
        , OriginUser
        , ChangeTime
        , ChangeType
        , ChangeData 
    FROM 
        logs.ChangeBuffer;

";
            using (var tx = _connection.BeginTransaction(IsolationLevel.Serializable))
            {
                using (var command = new SqlCommand(query, _connection, tx))
                {
                    command.CommandTimeout = 5 * 60;
                    command.ExecuteNonQuery();
                }
                Initialize(tx);
                tx.Commit();
            }
        }

        public void Close()
        {
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
            if (_connection != null)
                _connection.Dispose();
            _connection = null;
        }
    }
}
