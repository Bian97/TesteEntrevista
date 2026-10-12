namespace Vendas.Model.Entities;

public class ItemVenda
{
    public const int QuantidadeMaxima = 100000;

    public int Id { get; set; }
    public int VendaId { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal => Math.Round(Quantidade * ValorUnitario, 2, MidpointRounding.AwayFromZero);

    public void Normalizar() => ValorUnitario = Math.Round(ValorUnitario, 2, MidpointRounding.AwayFromZero);

    public IReadOnlyList<string> Validar(int numero)
    {
        var erros = new List<string>();

        if (Quantidade < 1 || Quantidade > QuantidadeMaxima)
        {
            erros.Add($"Item {numero}: quantidade deve estar entre 1 e {QuantidadeMaxima}.");
        }

        if (ValorUnitario <= 0)
        {
            erros.Add($"Item {numero}: valor unitário deve ser maior que zero.");
        }

        return erros;
    }
}
