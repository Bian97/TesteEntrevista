using Vendas.Abstractions.Interfaces.DbSession;
using Vendas.Abstractions.Interfaces.Repositories;
using Vendas.Model.Entities;

namespace Vendas.DB.Repositories;

public class VendaRepository : IVendaRepository
{
    private readonly IDbSession _db;

    public VendaRepository(IDbSession db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Venda>> ListarAsync() =>
        await _db.QueryAsync<Venda>("Scripts.Venda.Listar.sql");

    public async Task<Venda?> ObterPorIdAsync(int id)
    {
        var venda = await _db.QueryFirstOrDefaultAsync<Venda>("Scripts.Venda.ObterPorId.sql", new { Id = id });
        if (venda is null)
        {
            return null;
        }

        var itens = await _db.QueryAsync<ItemVenda>("Scripts.Venda.ObterItens.sql", new { VendaId = id });
        venda.Itens = itens.ToList();
        return venda;
    }

    public async Task<int> CriarAsync(Venda venda)
    {
        _db.BeginTransaction();
        try
        {
            var vendaId = await _db.ExecuteScalarAsync<int>("Scripts.Venda.CriarVenda.sql", venda);
            foreach (var item in venda.Itens)
            {
                await _db.ExecuteAsync("Scripts.Venda.CriarItem.sql", new
                {
                    VendaId = vendaId,
                    item.ProdutoId,
                    item.Quantidade,
                    item.ValorUnitario
                });
            }

            _db.Commit();
            return vendaId;
        }
        catch
        {
            _db.Rollback();
            throw;
        }
    }
}
