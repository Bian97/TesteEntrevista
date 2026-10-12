namespace Vendas.Abstractions.Interfaces.DbSession;

public interface IDbSession : IDisposable
{
    void BeginTransaction();
    void Commit();
    void Rollback();
    Task<IEnumerable<T>> QueryAsync<T>(string script, object? parameters = null);
    Task<T?> QueryFirstOrDefaultAsync<T>(string script, object? parameters = null);
    Task<int> ExecuteAsync(string script, object? parameters = null);
    Task<T> ExecuteScalarAsync<T>(string script, object? parameters = null);
}
