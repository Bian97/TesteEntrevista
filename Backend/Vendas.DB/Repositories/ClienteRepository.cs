using Vendas.Abstractions.Interfaces.DbSession;
using Vendas.Abstractions.Interfaces.Repositories;
using Vendas.Model.Entities;

namespace Vendas.DB.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly IDbSession _db;

    public ClienteRepository(IDbSession db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Cliente>> ListarAsync(string? busca)
    {
        var termo = string.IsNullOrWhiteSpace(busca) ? null : busca.Trim();
        var buscaCpf = termo is null ? string.Empty : new string(termo.Where(char.IsAsciiDigit).ToArray());
        return await _db.QueryAsync<Cliente>("Scripts.Cliente.Listar.sql", new { Busca = termo, BuscaCpf = buscaCpf });
    }

    public Task<Cliente?> ObterPorIdAsync(int id) =>
        _db.QueryFirstOrDefaultAsync<Cliente>("Scripts.Cliente.ObterPorId.sql", new { Id = id });

    public async Task<bool> ExisteCpfAsync(string cpf, int? ignorarId) =>
        await _db.ExecuteScalarAsync<int>("Scripts.Cliente.ExisteCpf.sql", new { Cpf = cpf, IgnorarId = ignorarId }) == 1;

    public Task<int> CriarAsync(Cliente cliente) =>
        _db.ExecuteScalarAsync<int>("Scripts.Cliente.Criar.sql", cliente);

    public Task AtualizarAsync(Cliente cliente) =>
        _db.ExecuteAsync("Scripts.Cliente.Atualizar.sql", cliente);

    public Task ExcluirAsync(int id) =>
        _db.ExecuteAsync("Scripts.Cliente.Excluir.sql", new { Id = id });

    public async Task<bool> PossuiVendasAsync(int id) =>
        await _db.ExecuteScalarAsync<int>("Scripts.Cliente.PossuiVendas.sql", new { Id = id }) == 1;
}
