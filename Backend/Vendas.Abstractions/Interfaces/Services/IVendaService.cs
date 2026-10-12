using Vendas.Model.Entities;

namespace Vendas.Abstractions.Interfaces.Services;

public interface IVendaService
{
    Task<IEnumerable<Venda>> ListarAsync();
    Task<Venda> ObterAsync(int id);
    Task<Venda> CriarAsync(Venda venda);
}
