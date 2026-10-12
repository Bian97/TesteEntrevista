using Dapper;
using Microsoft.Data.SqlClient;
using Vendas.Abstractions.Interfaces.DbSession;
using Vendas.Model.Configs;

namespace Vendas.DB.Sessions;

public class DbSession : IDbSession
{
    private readonly BancoConfig _config;
    private readonly SqlConnection _conexao;
    private SqlTransaction? _transacao;

    public DbSession(BancoConfig config)
    {
        _config = config;
        _conexao = new SqlConnection(config.ConnectionString);
    }

    public void BeginTransaction()
    {
        if (_transacao == null)
        {
            _conexao.Open();
            _transacao = _conexao.BeginTransaction();
        }
    }

    public void Commit()
    {
        _transacao?.Commit();
        FinalizarTransacao();
    }

    public void Rollback()
    {
        _transacao?.Rollback();
        FinalizarTransacao();
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string script, object? parameters = null)
    {
        return await _conexao.QueryAsync<T>(PegarQuery(script), parameters, _transacao, _config.TimeOut);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string script, object? parameters = null)
    {
        return await _conexao.QueryFirstOrDefaultAsync<T>(PegarQuery(script), parameters, _transacao, _config.TimeOut);
    }

    public async Task<int> ExecuteAsync(string script, object? parameters = null)
    {
        return await _conexao.ExecuteAsync(PegarQuery(script), parameters, _transacao, _config.TimeOut);
    }

    public async Task<T> ExecuteScalarAsync<T>(string script, object? parameters = null)
    {
        return (await _conexao.ExecuteScalarAsync<T>(PegarQuery(script), parameters, _transacao, _config.TimeOut))!;
    }

    public void Dispose()
    {
        _transacao?.Dispose();
        _conexao.Dispose();
    }

    private void FinalizarTransacao()
    {
        _transacao?.Dispose();
        _transacao = null;
        _conexao.Close();
    }

    private static string PegarQuery(string script)
    {
        var nome = "Vendas.DB." + script;
        using var stream = typeof(DbSession).Assembly.GetManifestResourceStream(nome)
            ?? throw new InvalidOperationException($"Script SQL não encontrado: {nome}");
        using var leitor = new StreamReader(stream);
        return leitor.ReadToEnd();
    }
}
