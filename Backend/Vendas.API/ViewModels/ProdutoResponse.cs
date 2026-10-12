using Vendas.Model.Entities;

namespace Vendas.API.ViewModels;

public class ProdutoResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal ValorUnitario { get; set; }
    public DateTime DataCadastro { get; set; }

    public static ProdutoResponse De(Produto produto) => new()
    {
        Id = produto.Id,
        Nome = produto.Nome,
        ValorUnitario = produto.ValorUnitario,
        DataCadastro = produto.DataCadastro
    };
}
