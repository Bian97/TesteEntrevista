using Vendas.Model.Entities;

namespace Vendas.API.ViewModels;

public class VendaResponse
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteCpf { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public List<ItemVendaResponse> Itens { get; set; } = new();

    public static VendaResponse De(Venda venda) => new()
    {
        Id = venda.Id,
        ClienteId = venda.ClienteId,
        ClienteCpf = venda.ClienteCpf,
        ClienteNome = venda.ClienteNome,
        DataVenda = venda.DataVenda,
        ValorTotal = venda.ValorTotal,
        Itens = venda.Itens.Select(ItemVendaResponse.De).ToList()
    };
}
