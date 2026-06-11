namespace Shift.Hub
{
    public interface ISqlDatabase
    {
        Task<int> CountAsync(string query, Dictionary<string, object>? parameters = null);

        Task<List<T>> SelectAsync<T>(string query, Dictionary<string, object>? parameters = null, QueryPagination? pagination = null);

        Task ExecuteQueryAsync(string query, object? o);

        // Runs the statements in order within a single transaction, rolling back on failure.
        Task ExecuteInTransactionAsync(IEnumerable<(string Query, object? Parameters)> statements);
    }
}
