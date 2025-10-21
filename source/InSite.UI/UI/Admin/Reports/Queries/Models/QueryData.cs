using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;

using Shift.Common;

namespace InSite.Admin.Reports.Queries.Models
{
    public class QueryData
    {
        #region Classes

        private class Column
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public Type Type { get; set; }
            public bool IsNullable { get; set; }

            internal void Write(BinaryWriter writer)
            {
                writer.Write(Name);
                writer.Write(Type.FullName);
                writer.Write(IsNullable);
            }

            internal static Column Read(BinaryReader reader, int index)
            {
                var result = new Column
                {
                    Index = index
                };

                result.Name = reader.ReadString();

                var typeName = reader.ReadString();
                result.Type = Type.GetType(typeName);

                result.IsNullable = reader.ReadBoolean();

                return result;
            }
        }

        public interface IRow
        {
            object this[int index] { get; set; }
            object this[string name] { get; set; }
        }

        private class InnerRow : IRow
        {
            public object this[int index]
            {
                get => _data[index];
                set => _data[index] = value;
            }

            public object this[string name]
            {
                get => _data[_columns[name].Index];
                set => _data[_columns[name].Index] = value;
            }

            private object[] _data;
            private Dictionary<string, Column> _columns;

            internal InnerRow(object[] data, Dictionary<string, Column> columns)
            {
                _data = data;
                _columns = columns;
            }
        }

        public interface IWriter : IDisposable
        {
            QueryData QueryData { get; }

            void AddColumn(string name, Type type, bool nullable);
            IRow AddRow();
        }

        private class Writer : IWriter
        {
            #region Constants

            private const int RowBufferSize = 1000;

            #endregion

            #region Properties

            public QueryData QueryData => _query;

            #endregion

            #region Fields

            private QueryData _query;
            private bool _isHeaderLocked = false;
            private bool _isBodyLocked = false;

            private List<long> _pagePositions;
            private List<object[]> _rowBuffer;
            private List<BinaryHelper.WriterMethod> _writerMethods;

            #endregion

            #region Construction

            public Writer(QueryData query)
            {
                _query = query;
            }

            #endregion

            #region Methods (data)

            public void AddColumn(string name, Type type, bool nullable)
            {
                if (_isHeaderLocked)
                    throw ApplicationError.Create("Can't add a column: header is locked");

                if (_query._columns.ContainsKey(name))
                    throw ApplicationError.Create("A column with the same name has already been added: " + name);

                _query._columns.Add(name, new Column
                {
                    Index = _query._columns.Count,
                    Name = name,
                    Type = type,
                    IsNullable = nullable,
                });
            }

            public IRow AddRow()
            {
                if (_isBodyLocked)
                    throw ApplicationError.Create("Can't add a row: body is locked");

                if (!_isHeaderLocked)
                    WriteFile(FileMode.Create, WriteHeader);

                if (_rowBuffer.Count > RowBufferSize)
                    WriteFile(FileMode.Append, WriteDataBuffer);

                var data = new object[_query._columns.Count];

                _rowBuffer.Add(data);

                return new InnerRow(data, _query._columns);
            }

            #endregion

            #region Methods (file)

            private void WriteHeader(BinaryWriter writer)
            {
                writer.Write(_query.QueryID);

                _writerMethods = new List<BinaryHelper.WriterMethod>(_query._columns.Count);

                writer.Write(_query._columns.Count);
                foreach (var c in _query._columns.Values.OrderBy(x => x.Index))
                {
                    c.Write(writer);

                    _writerMethods.Add(BinaryHelper.GetWriterMethod(c.Type, c.IsNullable));
                }

                _isHeaderLocked = true;
                _pagePositions = new List<long>();
                _rowBuffer = new List<object[]>(RowBufferSize);
            }

            private void WriteDataBuffer(BinaryWriter writer)
            {
                int rowIndex, cellIndex;

                for (rowIndex = 0; rowIndex < _rowBuffer.Count; rowIndex++)
                {
                    if (_query.RowsCount % PageSize == 0)
                        _pagePositions.Add(writer.BaseStream.Position);

                    var row = _rowBuffer[rowIndex];
                    for (cellIndex = 0; cellIndex < row.Length; cellIndex++)
                        _writerMethods[cellIndex](writer, row[cellIndex]);

                    _query.RowsCount++;
                }

                _rowBuffer.Clear();
            }

            private void WriteEnd(BinaryWriter writer)
            {
                WriteDataBuffer(writer);

                foreach (var p in _pagePositions)
                    writer.Write(p);

                writer.Write(_query.RowsCount);
                writer.Write(FileEnd);

                _isBodyLocked = true;
                _query._isLoaded = true;
            }

            private void WriteFile(FileMode mode, Action<BinaryWriter> action)
            {
                var isCreate = mode == FileMode.Create
                    || mode == FileMode.OpenOrCreate
                    || mode == FileMode.CreateNew;

                var fLock = _fileLocks.GetOrAdd(_query.QueryID, CreateFileLock);
                lock (fLock)
                {
                    var path = GetFilePath(_query.QueryID);
                    using (var fs = File.Open(path, mode, FileAccess.Write, FileShare.None))
                    {
                        using (var writer = new BinaryWriter(fs))
                        {
                            action(writer);
                        }
                    }

                    File.SetLastAccessTime(path, DateTime.Now);

                    if (isCreate)
                        fLock.Exists = true;
                }

                if (isCreate)
                    EnsureTimerExists();
            }

            #endregion

            public void Dispose()
            {
                if (!_isHeaderLocked)
                    WriteFile(FileMode.Create, WriteHeader);

                WriteFile(FileMode.Append, WriteEnd);
            }
        }

        private class FileLock
        {
            public bool Exists { get; set; } = true;
        }

        #endregion

        #region Properties

        public Guid QueryID { get; private set; }

        public int RowsCount { get; private set; }

        #endregion 

        #region Fields

        private Dictionary<string, Column> _columns = new Dictionary<string, Column>();
        private bool _isLoaded = false;

        #endregion

        #region Fields (static)

        private const int TimerDueTime = 1 * 60 * 1000;
        private const int PageSize = 50;
        private static readonly Guid FileEnd = Guid.NewGuid();

        private static readonly object _syncRoot = new object();
        private static readonly ConcurrentDictionary<Guid, FileLock> _fileLocks = new ConcurrentDictionary<Guid, FileLock>();

        private static readonly string _storagePath;
        private static Timer _timer;

        #endregion

        #region Construction

        static QueryData()
        {
            _storagePath = Path.Combine(ServiceLocator.FilePaths.TempFolderPath, "AdminQueries");

            var dir = new DirectoryInfo(_storagePath);
            if (dir.Exists)
            {
                foreach (var file in dir.GetFiles())
                    file.Delete();

                foreach (var subDir in dir.GetDirectories())
                    subDir.Delete(true);
            }
            else
            {
                dir.Create();
            }
        }

        private QueryData()
        {

        }

        #endregion

        #region Methods (data)

        public static IWriter Create()
        {
            var query = new QueryData
            {
                QueryID = Guid.NewGuid()
            };

            return new Writer(query);
        }

        public DataTable ToDataTable(Paging paging)
        {
            if (!_isLoaded)
                throw ApplicationError.Create("QueryData is not loaded");

            var result = new DataTable();

            if (_columns.Count == 0)
                return result;

            foreach (var column in _columns.Values.OrderBy(x => x.Index))
            {
                result.Columns.Add(new DataColumn(column.Name, column.Type)
                {
                    AllowDBNull = column.IsNullable
                });
            }

            if (RowsCount == 0)
                return result;

            var start = 1;
            var end = RowsCount;

            if (paging != null)
            {
                (start, end) = paging.ToStartEnd();

                if (start < 1)
                    start = 1;

                if (end > RowsCount)
                    end = RowsCount;
            }

            var rows = ReadRows(this, start, end);
            for (var i = 0; i < rows.Length; i++)
                AddRow(result, rows[i]);

            return result;
        }

        #endregion

        #region Methods (file)

        public static QueryData Load(Guid queryId)
        {
            QueryData result = null;

            var fLock = _fileLocks.GetOrAdd(queryId, CreateFileLock);
            lock (fLock)
            {
                if (fLock.Exists)
                {
                    try
                    {
                        var path = GetFilePath(queryId);
                        using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (var reader = new BinaryReader(fs))
                            {
                                result = new QueryData();
                                result.QueryID = reader.ReadGuid();

                                var colCount = reader.ReadInt32();
                                for (var colIndex = 0; colIndex < colCount; colIndex++)
                                {
                                    var column = Column.Read(reader, colIndex);

                                    result._columns.Add(column.Name, column);
                                }

                                result._isLoaded = true;

                                fs.Seek(-20, SeekOrigin.End);

                                result.RowsCount = reader.ReadInt32();

                                var fileEnd = reader.ReadGuid();
                                if (fileEnd != FileEnd)
                                    result = null;
                            }
                        }

                        if (result != null)
                            File.SetLastAccessTime(path, DateTime.Now);
                    }
                    catch
                    {

                    }
                }
            }

            EnsureTimerExists();

            return result;
        }

        private static object[][] ReadRows(QueryData query, int start, int end)
        {
            List<object[]> result = null;

            var fLock = _fileLocks.GetOrAdd(query.QueryID, CreateFileLock);
            lock (fLock)
            {
                if (fLock.Exists)
                {
                    var readers = new List<BinaryHelper.ReaderMethod>(query._columns.Count);
                    foreach (var c in query._columns.Values.OrderBy(x => x.Index))
                        readers.Add(BinaryHelper.GetReaderMethod(c.Type, c.IsNullable));

                    var path = GetFilePath(query.QueryID);
                    using (var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (var reader = new BinaryReader(fs))
                        {
                            var pageIndex = (int)Math.Floor((double)start / PageSize);
                            var pageCount = (int)Math.Ceiling((double)query.RowsCount / PageSize);

                            if (pageIndex < pageCount)
                            {
                                {
                                    fs.Seek(-20 - (pageCount - pageIndex) * 8, SeekOrigin.End); // Go to the start position of page indexes

                                    var pageStart = reader.ReadInt64();
                                    fs.Seek(pageStart, SeekOrigin.Begin); // Go to the start position of page data
                                }

                                int cellIndex;

                                for (var skip = (start - 1) % PageSize; skip > 0; skip--)
                                {
                                    for (cellIndex = 0; cellIndex < readers.Count; cellIndex++)
                                        readers[cellIndex](reader);
                                }

                                result = new List<object[]>(end - start + 1);

                                for (var take = result.Capacity; take > 0; take--)
                                {
                                    var data = new object[query._columns.Count];
                                    for (cellIndex = 0; cellIndex < readers.Count; cellIndex++)
                                        data[cellIndex] = readers[cellIndex](reader);

                                    result.Add(data);
                                }
                            }
                        }
                    }

                    File.SetLastAccessTime(path, DateTime.Now);
                }
            }

            EnsureTimerExists();

            return result.ToArray();
        }

        public static bool CleanUp()
        {
            var isClean = true;

            lock (_syncRoot)
            {
                var dir = new DirectoryInfo(_storagePath);

                foreach (var file in dir.GetFiles())
                {
                    if (file.Extension == ".bin" && Guid.TryParse(Path.GetFileNameWithoutExtension(file.Name), out var queryId))
                    {
                        var fLock = _fileLocks.GetOrAdd(queryId, CreateFileLock);
                        lock (fLock)
                        {
                            var fileDate = file.LastAccessTimeUtc;

                            if (fileDate < file.LastWriteTimeUtc)
                                fileDate = file.LastWriteTimeUtc;

                            fLock.Exists = (DateTime.UtcNow - fileDate).TotalMinutes <= 5;
                        }

                        if (!fLock.Exists)
                        {
                            file.Delete();
                            _fileLocks.TryRemove(queryId, out _);
                        }
                        else
                        {
                            isClean = false;
                        }
                    }
                    else
                    {
                        file.Delete();
                    }
                }

                foreach (var kv in _fileLocks)
                {
                    if (!kv.Value.Exists)
                        _fileLocks.TryRemove(kv.Key, out _);
                }
            }

            return isClean;
        }

        private static void EnsureTimerExists()
        {
            lock (_syncRoot)
            {
                if (_timer == null)
                    _timer = new Timer(OnTimer, null, TimerDueTime, Timeout.Infinite);
            }
        }

        private static void OnTimer(object state)
        {
            lock (_syncRoot)
            {
                if (CleanUp())
                {
                    try
                    {
                        _timer.Dispose();
                        _timer = null;
                    }
                    catch
                    {
                    }
                }
                else
                {
                    _timer.Change(TimerDueTime, Timeout.Infinite);
                }
            }
        }

        #endregion

        #region Methods (helpers)

        private static string GetFilePath(Guid key) =>
            Path.Combine(_storagePath, key + ".bin");

        private static FileLock CreateFileLock(Guid key)
        {
            var path = GetFilePath(key);

            return new FileLock
            {
                Exists = File.Exists(path)
            };
        }

        private static DataRow AddRow(DataTable t, object[] data)
        {
            var dbRow = t.NewRow();

            for (var j = 0; j < data.Length; j++)
                dbRow[j] = data[j] ?? DBNull.Value;

            t.Rows.Add(dbRow);

            return dbRow;
        }

        #endregion
    }
}