namespace Vendas.Model.Entities;

public class Venda
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteCpf { get; set; } = string.Empty;
    public string ClienteNome { get; set; } = string.Empty;
    public DateTime DataVenda { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataCadastro { get; set; }
    public List<ItemVenda> Itens { get; set; } = new();

    public void CalcularTotal() => ValorTotal = Itens.Sum(i => i.ValorTotal);

    public void Normalizar()
    {
        if (DataVenda == default)
        {
            DataVenda = DateTime.Today;
        }
        DataVenda = DataVenda.Date;

        foreach (var item in Itens)
        {
            item.Normalizar();
        }
    }

    public IReadOnlyList<string> Validar()
    {
        var erros = new List<string>();

        if (DataVenda > DateTime.Today)
        {
            erros.Add("A data da venda não pode ser futura.");
        }

        if (Itens.Count == 0)
        {
            erros.Add("A venda deve ter ao menos um item.");
            return erros;
        }

        var produtosInformados = new HashSet<int>();
        for (var i = 0; i < Itens.Count; i++)
        {
            var item = Itens[i];
            var numero = i + 1;

            erros.AddRange(item.Validar(numero));

            if (!produtosInformados.Add(item.ProdutoId))
            {
                erros.Add($"Item {numero}: produto repetido; some as quantidades.");
            }
        }

        return erros;
    }
}
