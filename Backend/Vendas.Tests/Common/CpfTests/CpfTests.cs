using FluentAssertions;
using Vendas.Common.Utilitarios;

namespace Vendas.Tests.Common.CpfTests;

public class CpfTests
{
    [Theory]
    [InlineData("15783306290")]
    [InlineData("40952617307")]
    [InlineData("87134025950")]
    public void EhValido_CpfValido_DeveRetornarTrue(string cpf)
    {
        Cpf.EhValido(cpf).Should().BeTrue();
    }

    [Theory]
    [InlineData("15783306291")]
    [InlineData("12345678900")]
    [InlineData("1234567890")]
    [InlineData("")]
    [InlineData(null)]
    public void EhValido_CpfInvalido_DeveRetornarFalse(string? cpf)
    {
        Cpf.EhValido(cpf).Should().BeFalse();
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("99999999999")]
    public void EhValido_CpfComTodosDigitosIguais_DeveRetornarFalse(string cpf)
    {
        Cpf.EhValido(cpf).Should().BeFalse();
    }

    [Fact]
    public void EhValido_CpfComMascara_DeveRetornarTrue()
    {
        Cpf.EhValido("157.833.062-90").Should().BeTrue();
    }

    [Fact]
    public void Normalizar_ValorComMascara_DeveManterSomenteDigitos()
    {
        Cpf.Normalizar("157.833.062-90").Should().Be("15783306290");
    }

    [Fact]
    public void Normalizar_Nulo_DeveRetornarVazio()
    {
        Cpf.Normalizar(null).Should().BeEmpty();
    }

    [Fact]
    public void Formatar_CpfComDigitos_DeveRetornarFormatoComPontuacao()
    {
        Cpf.Formatar("15783306290").Should().Be("157.833.062-90");
    }
}
