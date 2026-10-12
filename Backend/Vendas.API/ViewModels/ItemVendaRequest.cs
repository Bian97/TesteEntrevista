using Vendas.Model.Entities;

namespace Vendas.API.ViewModels;

public class ItemVendaRequest
{
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }

    public ItemVenda ParaEntidade() => new()
    {
        ProdutoId = ProdutoId,
        Quantidade = Quantidade,
        ValorUnitario = ValorUnitario
    };
}
