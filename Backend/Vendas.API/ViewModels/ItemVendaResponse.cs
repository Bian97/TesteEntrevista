using Vendas.Model.Entities;

namespace Vendas.API.ViewModels;

public class ItemVendaResponse
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }

    public static ItemVendaResponse De(ItemVenda item) => new()
    {
        Id = item.Id,
        ProdutoId = item.ProdutoId,
        ProdutoNome = item.ProdutoNome,
        Quantidade = item.Quantidade,
        ValorUnitario = item.ValorUnitario,
        ValorTotal = item.ValorTotal
    };
}
