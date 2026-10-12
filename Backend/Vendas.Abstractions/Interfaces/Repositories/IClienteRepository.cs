using Vendas.Model.Entities;

namespace Vendas.Abstractions.Interfaces.Repositories;

public interface IClienteRepository
{
    Task<IEnumerable<Cliente>> ListarAsync(string? busca);
    Task<Cliente?> ObterPorIdAsync(int id);
    Task<bool> ExisteCpfAsync(string cpf, int? ignorarId);
    Task<int> CriarAsync(Cliente cliente);
    Task AtualizarAsync(Cliente cliente);
    Task ExcluirAsync(int id);
    Task<bool> PossuiVendasAsync(int id);
}
