using Vendas.Model.Entities;

namespace Vendas.Abstractions.Interfaces.Services;

public interface IClienteService
{
    Task<IEnumerable<Cliente>> ListarAsync(string? busca);
    Task<Cliente> ObterAsync(int id);
    Task<Cliente> CriarAsync(Cliente cliente);
    Task<Cliente> AtualizarAsync(Cliente cliente);
    Task ExcluirAsync(int id);
}
