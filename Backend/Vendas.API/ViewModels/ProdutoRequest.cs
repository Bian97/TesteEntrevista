using Vendas.Model.Entities;

namespace Vendas.API.ViewModels;

public class ProdutoRequest
{
    public string Nome { get; set; } = string.Empty;

    public decimal ValorUnitario { get; set; }

    public Produto ParaEntidade() => new()
    {
        Nome = Nome,
        ValorUnitario = ValorUnitario
    };
}
