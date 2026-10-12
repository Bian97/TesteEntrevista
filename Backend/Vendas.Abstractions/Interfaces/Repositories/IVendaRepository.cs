using Vendas.Model.Entities;

namespace Vendas.Abstractions.Interfaces.Repositories;

public interface IVendaRepository
{
    Task<IEnumerable<Venda>> ListarAsync();
    Task<Venda?> ObterPorIdAsync(int id);
    Task<int> CriarAsync(Venda venda);
}
