using Vendas.Abstractions.Interfaces.Repositories;
using Vendas.Abstractions.Interfaces.Services;
using Vendas.Common.Utilitarios;
using Vendas.Model.Entities;

namespace Vendas.Business.Services;

public class VendaService : IVendaService
{
    private readonly IVendaRepository _vendas;
    private readonly IClienteRepository _clientes;
    private readonly IProdutoRepository _produtos;

    public VendaService(IVendaRepository vendas, IClienteRepository clientes, IProdutoRepository produtos)
    {
        _vendas = vendas;
        _clientes = clientes;
        _produtos = produtos;
    }

    public async Task<IEnumerable<Venda>> ListarAsync() => await _vendas.ListarAsync();

    public async Task<Venda> ObterAsync(int id)
    {
        return await _vendas.ObterPorIdAsync(id) ?? throw new NaoEncontradoException("Venda não encontrada.");
    }

    public async Task<Venda> CriarAsync(Venda venda)
    {
        venda.Normalizar();
        var erros = venda.Validar().ToList();

        if (venda.ClienteId <= 0 || await _clientes.ObterPorIdAsync(venda.ClienteId) is null)
        {
            erros.Add("Cliente não informado ou inexistente.");
        }

        for (var i = 0; i < venda.Itens.Count; i++)
        {
            var item = venda.Itens[i];
            if (item.ProdutoId <= 0 || await _produtos.ObterPorIdAsync(item.ProdutoId) is null)
            {
                erros.Add($"Item {i + 1}: produto inexistente.");
            }
        }

        if (erros.Count > 0)
        {
            throw new ValidacaoException(erros);
        }

        venda.CalcularTotal();
        venda.Id = await _vendas.CriarAsync(venda);
        return await ObterAsync(venda.Id);
    }
}
