using Vendas.Model.Entities;

namespace Vendas.API.ViewModels;

public class VendaRequest
{
    public int ClienteId { get; set; }
    public DateTime? DataVenda { get; set; }

    public List<ItemVendaRequest> Itens { get; set; } = new();

    public Venda ParaEntidade() => new()
    {
        ClienteId = ClienteId,
        DataVenda = DataVenda ?? default,
        Itens = (Itens ?? new()).Select(i => i.ParaEntidade()).ToList()
    };
}
