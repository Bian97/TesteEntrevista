using FluentAssertions;
using Vendas.Model.Entities;

namespace Vendas.Tests.Model.ProdutoTests;

public class ProdutoTests
{
    [Fact]
    public void Normalizar_NomeComEspacos_DeveRemoverEspacosNasPontas()
    {
        var produto = ProdutoValido();
        produto.Nome = "  Cadeira Ergonômica  ";

        produto.Normalizar();

        produto.Nome.Should().Be("Cadeira Ergonômica");
    }

    [Fact]
    public void Normalizar_ValorNoPontoMedio_DeveArredondarParaCima()
    {
        var produto = ProdutoValido();
        produto.ValorUnitario = 10.005m;

        produto.Normalizar();

        produto.ValorUnitario.Should().Be(10.01m);
    }

    [Fact]
    public void Validar_NomeVazio_DeveRetornarNomeObrigatorio()
    {
        var produto = ProdutoValido();
        produto.Nome = "";

        produto.Validar().Should().ContainSingle().Which.Should().Be("Nome é obrigatório.");
    }

    [Fact]
    public void Validar_NomeComMaisDe150Caracteres_DeveRetornarErro()
    {
        var produto = ProdutoValido();
        produto.Nome = new string('A', 151);

        produto.Validar().Should().ContainSingle()
            .Which.Should().Be("Nome deve ter no máximo 150 caracteres.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10000000)]
    public void Validar_ValorForaDoIntervalo_DeveRetornarErroDeValor(double valor)
    {
        var produto = ProdutoValido();
        produto.ValorUnitario = (decimal)valor;

        produto.Validar().Should().ContainSingle()
            .Which.Should().Be("Valor unitário deve ser maior que zero e no máximo 9999999,99.");
    }

    private static Produto ProdutoValido() => new()
    {
        Nome = "Cadeira Ergonômica",
        ValorUnitario = 349.00m
    };
}
