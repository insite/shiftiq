namespace Engine.Api.Internal
{
    public interface ISqlDatabase
    {
        Task<int> CountAsync(string query, Dictionary<string, object>? parameters = null);

        Task<List<T>> SelectAsync<T>(string query, Dictionary<string, object>? parameters = null, QueryPagination? pagination = null);

        Task ExecuteQueryAsync(string query, object? o);
    }
}