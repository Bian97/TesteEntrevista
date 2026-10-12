namespace Vendas.Model.Entities;

public class Produto
{
    public const int NomeTamanhoMaximo = 150;
    public const decimal ValorMaximo = 9999999.99m;

    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal ValorUnitario { get; set; }
    public DateTime DataCadastro { get; set; }

    public void Normalizar()
    {
        Nome = (Nome ?? string.Empty).Trim();
        ValorUnitario = Math.Round(ValorUnitario, 2, MidpointRounding.AwayFromZero);
    }

    public IReadOnlyList<string> Validar()
    {
        var erros = new List<string>();

        if (string.IsNullOrEmpty(Nome))
        {
            erros.Add("Nome é obrigatório.");
        }
        else if (Nome.Length > NomeTamanhoMaximo)
        {
            erros.Add($"Nome deve ter no máximo {NomeTamanhoMaximo} caracteres.");
        }

        if (ValorUnitario <= 0 || ValorUnitario > ValorMaximo)
        {
            erros.Add("Valor unitário deve ser maior que zero e no máximo 9999999,99.");
        }

        return erros;
    }
}
