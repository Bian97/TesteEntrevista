using Vendas.Model.Entities;

namespace Vendas.Abstractions.Interfaces.Repositories;

public interface IProdutoRepository
{
    Task<IEnumerable<Produto>> ListarAsync(string? busca);
    Task<Produto?> ObterPorIdAsync(int id);
    Task<bool> ExisteNomeAsync(string nome, int? ignorarId);
    Task<int> CriarAsync(Produto produto);
    Task AtualizarAsync(Produto produto);
    Task ExcluirAsync(int id);
    Task<bool> PossuiVendasAsync(int id);
}
