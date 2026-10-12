using Vendas.Abstractions.Interfaces.DbSession;
using Vendas.Abstractions.Interfaces.Repositories;
using Vendas.Model.Entities;

namespace Vendas.DB.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly IDbSession _db;

    public ProdutoRepository(IDbSession db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Produto>> ListarAsync(string? busca)
    {
        var termo = string.IsNullOrWhiteSpace(busca) ? null : busca.Trim();
        return await _db.QueryAsync<Produto>("Scripts.Produto.Listar.sql", new { Busca = termo });
    }

    public Task<Produto?> ObterPorIdAsync(int id) =>
        _db.QueryFirstOrDefaultAsync<Produto>("Scripts.Produto.ObterPorId.sql", new { Id = id });

    public async Task<bool> ExisteNomeAsync(string nome, int? ignorarId) =>
        await _db.ExecuteScalarAsync<int>("Scripts.Produto.ExisteNome.sql", new { Nome = nome, IgnorarId = ignorarId }) == 1;

    public Task<int> CriarAsync(Produto produto) =>
        _db.ExecuteScalarAsync<int>("Scripts.Produto.Criar.sql", produto);

    public Task AtualizarAsync(Produto produto) =>
        _db.ExecuteAsync("Scripts.Produto.Atualizar.sql", produto);

    public Task ExcluirAsync(int id) =>
        _db.ExecuteAsync("Scripts.Produto.Excluir.sql", new { Id = id });

    public async Task<bool> PossuiVendasAsync(int id) =>
        await _db.ExecuteScalarAsync<int>("Scripts.Produto.PossuiVendas.sql", new { Id = id }) == 1;
}
