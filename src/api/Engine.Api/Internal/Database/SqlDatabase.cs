using System.Data.Common;
using System.Text.RegularExpressions;

using Dapper;

using Engine.Api.Metadata;
using Engine.Api.Orchestration;

using Microsoft.EntityFrameworkCore;

namespace Engine.Api.Internal
{
    public class SqlDatabase : ISqlDatabase
    {
        private readonly DbConnection _connection;

        public SqlDatabase(EngineDbContext context)
        {
            _connection = context.Database.GetDbConnection();
        }

        public async Task<int> CountAsync(string select, Dictionary<string, object>? parameters = null)
        {
            return await _connection.ExecuteScalarAsync<int>(select, parameters);
        }

        public async Task ExecuteQueryAsync(string query, object? o)
        {
            await _connection.ExecuteAsync(query, o);
        }

        public async Task<List<T>> SelectAsync<T>(string select, Dictionary<string, object>? parameters = null, QueryPagination? pagination = null)
        {
            if (pagination != null)
            {
                if (pagination.PageSize < 1)
                    throw new ArgumentNullException("pagination.PageSize");

                if (pagination.Page < 1)
                    throw new ArgumentNullException("pagination.PageNumber");

                var offset = (pagination.Page - 1) * pagination.PageSize;
                select += $" OFFSET @PageOffset ROWS FETCH NEXT @PageSize ROWS ONLY";

                parameters ??= new();
                parameters.Add("PageOffset", offset);
                parameters.Add("PageSize", pagination.PageSize);
            }

            return (await _connection.QueryAsync<T>(select, parameters)).ToList();
        }

        public async Task ExecuteScriptAsync(string script)
        {
            try
            {
                var query = await File.ReadAllTextAsync(script);
                if (string.IsNullOrEmpty(query))
                    return;

                var statements = SplitSqlStatements(query);
                foreach (var statement in statements)
                    await ExecuteQueryAsync(statement, null);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred executing this script: " + script, ex);
            }
        }

        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            // Split by "GO" statements
            var statements = Regex.Split(
                sqlScript,
                @"^\s*GO\s* ($ | \-\- .*$)",
                RegexOptions.Multiline |
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.IgnoreCase);

            // Remove empties, trim, and return
            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'))
                .ToList();
        }

        public async Task UpgradeAsync()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly == null)
                return;

            var path = Path.GetDirectoryName(assembly.Location);
            if (path == null)
                return;

            var scripts = Directory.GetFiles(Path.Combine(path, "Metadata", "Scripts"), "*.sql");
            await ExecuteScriptsOnceAsync(scripts);
        }

        private async Task ExecuteScriptsOnceAsync(string[] scripts)
        {
            foreach (var script in scripts)
                await ExecuteScriptOnceAsync(script);
        }

        private async Task ExecuteScriptOnceAsync(string script)
        {
            if (await ScriptIsExecutedAsync(script))
                return;

            await ExecuteScriptAsync(script);
            await ScriptExecutedAsync(script);
        }

        private async Task<bool> ScriptIsExecutedAsync(string script)
        {
            try
            {
                var filename = Path.GetFileName(script);

                var parameters = new Dictionary<string, object>
                {
                    { "@ScriptName", filename }
                };

                var calls = await SelectAsync<Upgrade>("SELECT * FROM [database].TUpgrade WHERE ScriptName = @ScriptName", parameters);

                return calls.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private async Task ScriptExecutedAsync(string script)
        {
            var fileName = Path.GetFileName(script);

            var file = await File.ReadAllTextAsync(script);

            var upgrade = new Upgrade(fileName, DateTimeOffset.Now, file);

            await ExecuteQueryAsync("INSERT INTO [database].TUpgrade (ScriptName, ScriptExecuted, ScriptData) VALUES (@ScriptName, @ScriptExecuted, @ScriptData)", upgrade);
        }
    }
}