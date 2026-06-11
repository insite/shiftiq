using System.Text.RegularExpressions;

namespace Shift.Hub
{
    public class DatabaseMigrator
    {
        private readonly ISqlDatabase _database;

        public DatabaseMigrator(EngineDbContext context, ISqlDatabase database)
        {
            _database = database;
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
                    await _database.ExecuteQueryAsync(statement, null);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred executing this script: " + script, ex);
            }
        }

        private static IEnumerable<string> SplitSqlStatements(string sqlScript)
        {
            var statements = Regex.Split(
                sqlScript,
                @"^\s*GO\s* ($ | \-\- .*$)",
                RegexOptions.Multiline |
                RegexOptions.IgnorePatternWhitespace |
                RegexOptions.IgnoreCase);

            return statements
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim(' ', '\r', '\n'))
                .ToList();
        }

        public async Task MigrateAsync()
        {
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly == null)
                return;

            var path = Path.GetDirectoryName(assembly.Location);
            if (path == null)
                return;

            var scripts = Directory.GetFiles(Path.Combine(path, "Schema", "Migrations"), "*.sql");
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

                var calls = await _database.SelectAsync<Upgrade>("SELECT * FROM [database].TUpgrade WHERE ScriptName = @ScriptName", parameters);

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

            await _database.ExecuteQueryAsync("INSERT INTO [database].TUpgrade (ScriptName, ScriptExecuted, ScriptData) VALUES (@ScriptName, @ScriptExecuted, @ScriptData)", upgrade);
        }
    }
}
