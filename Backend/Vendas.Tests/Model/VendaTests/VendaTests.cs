using FluentAssertions;
using Vendas.Model.Entities;

namespace Vendas.Tests.Model.VendaTests;

public class VendaTests
{
    [Fact]
    public void ValorTotal_ItemVenda_DeveMultiplicarQuantidadeEArredondar()
    {
        var item = new ItemVenda { Quantidade = 3, ValorUnitario = 54.90m };

        item.ValorTotal.Should().Be(164.70m);
    }

    [Fact]
    public void CalcularTotal_DeveSomarValorTotalDosItens()
    {
        var venda = new Venda
        {
            Itens =
            [
                new ItemVenda { Quantidade = 2, ValorUnitario = 15.75m },
                new ItemVenda { Quantidade = 1, ValorUnitario = 320.00m }
            ]
        };

        venda.CalcularTotal();

        venda.ValorTotal.Should().Be(351.50m);
    }

    [Fact]
    public void Normalizar_DataNaoInformada_DeveUsarDataDeHoje()
    {
        var venda = VendaValida();
        venda.DataVenda = default;

        venda.Normalizar();

        venda.DataVenda.Should().Be(DateTime.Today);
    }

    [Fact]
    public void Normalizar_DataComHora_DeveZerarHora()
    {
        var venda = VendaValida();
        venda.DataVenda = new DateTime(2025, 8, 7, 16, 45, 0);

        venda.Normalizar();

        venda.DataVenda.Should().Be(new DateTime(2025, 8, 7));
    }

    [Fact]
    public void ItemVenda_Normalizar_DeveArredondarValorUnitario()
    {
        var item = new ItemVenda { Quantidade = 1, ValorUnitario = 19.999m };

        item.Normalizar();

        item.ValorUnitario.Should().Be(20.00m);
    }

    [Fact]
    public void Validar_VendaValida_DeveRetornarListaVazia()
    {
        VendaValida().Validar().Should().BeEmpty();
    }

    [Fact]
    public void Validar_SemItens_DeveRetornarErro()
    {
        var venda = VendaValida();
        venda.Itens.Clear();

        venda.Validar().Should().ContainSingle()
            .Which.Should().Be("A venda deve ter ao menos um item.");
    }

    [Fact]
    public void Validar_DataFutura_DeveRetornarErro()
    {
        var venda = VendaValida();
        venda.DataVenda = DateTime.Today.AddDays(1);

        venda.Validar().Should().ContainSingle()
            .Which.Should().Be("A data da venda não pode ser futura.");
    }

    [Fact]
    public void Validar_ProdutoRepetido_DeveRetornarErroNoItemRepetido()
    {
        var venda = VendaValida();
        venda.Itens.Add(new ItemVenda { ProdutoId = 1, Quantidade = 1, ValorUnitario = 10m });

        venda.Validar().Should().ContainSingle()
            .Which.Should().Be("Item 3: produto repetido; some as quantidades.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100001)]
    public void ItemVenda_Validar_QuantidadeForaDoIntervalo_DeveRetornarErroComNumeroDoItem(int quantidade)
    {
        var item = new ItemVenda { ProdutoId = 1, Quantidade = quantidade, ValorUnitario = 10m };

        item.Validar(7).Should().ContainSingle()
            .Which.Should().Be("Item 7: quantidade deve estar entre 1 e 100000.");
    }

    [Fact]
    public void ItemVenda_Validar_ValorUnitarioNaoPositivo_DeveRetornarErro()
    {
        var item = new ItemVenda { ProdutoId = 1, Quantidade = 1, ValorUnitario = 0m };

        item.Validar(1).Should().ContainSingle()
            .Which.Should().Be("Item 1: valor unitário deve ser maior que zero.");
    }

    private static Venda VendaValida()
    {
        return new Venda
        {
            ClienteId = 1,
            DataVenda = DateTime.Today,
            Itens =
            [
                new ItemVenda { ProdutoId = 1, Quantidade = 2, ValorUnitario = 14.90m },
                new ItemVenda { ProdutoId = 2, Quantidade = 3, ValorUnitario = 31.20m }
            ]
        };
    }
}
