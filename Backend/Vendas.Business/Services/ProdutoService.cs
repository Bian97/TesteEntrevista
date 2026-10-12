using Vendas.Abstractions.Interfaces.Repositories;
using Vendas.Abstractions.Interfaces.Services;
using Vendas.Common.Utilitarios;
using Vendas.Model.Entities;

namespace Vendas.Business.Services;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtos;

    public ProdutoService(IProdutoRepository produtos)
    {
        _produtos = produtos;
    }

    public Task<IEnumerable<Produto>> ListarAsync(string? busca) => _produtos.ListarAsync(busca);

    public async Task<Produto> ObterAsync(int id)
    {
        return await _produtos.ObterPorIdAsync(id) ?? throw new NaoEncontradoException("Produto não encontrado.");
    }

    public async Task<Produto> CriarAsync(Produto produto)
    {
        produto.Normalizar();
        var erros = produto.Validar();
        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }

        if (await _produtos.ExisteNomeAsync(produto.Nome, null))
        {
            throw new ConflitoException("Já existe um produto cadastrado com este nome.");
        }

        produto.Id = await _produtos.CriarAsync(produto);
        return produto;
    }

    public async Task<Produto> AtualizarAsync(Produto produto)
    {
        if (await _produtos.ObterPorIdAsync(produto.Id) is null)
        {
            throw new NaoEncontradoException("Produto não encontrado.");
        }

        produto.Normalizar();
        var erros = produto.Validar();
        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }

        if (await _produtos.ExisteNomeAsync(produto.Nome, produto.Id))
        {
            throw new ConflitoException("Já existe um produto cadastrado com este nome.");
        }

        await _produtos.AtualizarAsync(produto);
        return produto;
    }

    public async Task ExcluirAsync(int id)
    {
        if (await _produtos.ObterPorIdAsync(id) is null)
        {
            throw new NaoEncontradoException("Produto não encontrado.");
        }

        if (await _produtos.PossuiVendasAsync(id))
        {
            throw new ConflitoException("Não é possível excluir um produto que possui vendas.");
        }

        await _produtos.ExcluirAsync(id);
    }
}
