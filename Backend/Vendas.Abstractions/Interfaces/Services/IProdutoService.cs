using Vendas.Model.Entities;

namespace Vendas.Abstractions.Interfaces.Services;

public interface IProdutoService
{
    Task<IEnumerable<Produto>> ListarAsync(string? busca);
    Task<Produto> ObterAsync(int id);
    Task<Produto> CriarAsync(Produto produto);
    Task<Produto> AtualizarAsync(Produto produto);
    Task ExcluirAsync(int id);
}
