using FluentAssertions;
using Vendas.Model.Entities;

namespace Vendas.Tests.Model.ClienteTests;

public class ClienteTests
{
    [Fact]
    public void Normalizar_CpfETelefoneComMascara_DeveGravarSomenteDigitos()
    {
        var cliente = ClienteValido();
        cliente.Cpf = "284.715.396-91";
        cliente.Telefone = "(31) 98765-4321";

        cliente.Normalizar();

        cliente.Cpf.Should().Be("28471539691");
        cliente.Telefone.Should().Be("31987654321");
    }

    [Fact]
    public void Normalizar_NomeComEspacos_DeveRemoverEspacosNasPontas()
    {
        var cliente = ClienteValido();
        cliente.Nome = "  Carlos Eduardo Lima  ";

        cliente.Normalizar();

        cliente.Nome.Should().Be("Carlos Eduardo Lima");
    }

    [Fact]
    public void Normalizar_EmailComMaiusculasEEspacos_DeveGravarMinusculoSemEspacos()
    {
        var cliente = ClienteValido();
        cliente.Email = "  Carlos.Lima@Mail.COM ";

        cliente.Normalizar();

        cliente.Email.Should().Be("carlos.lima@mail.com");
    }

    [Fact]
    public void Normalizar_EmailETelefoneVazios_DeveGravarNull()
    {
        var cliente = ClienteValido();
        cliente.Email = "   ";
        cliente.Telefone = "";

        cliente.Normalizar();

        cliente.Email.Should().BeNull();
        cliente.Telefone.Should().BeNull();
    }

    [Fact]
    public void Validar_ClienteValido_DeveRetornarListaVazia()
    {
        var cliente = ClienteValido();

        cliente.Validar().Should().BeEmpty();
    }

    [Fact]
    public void Validar_NomeVazio_DeveRetornarNomeObrigatorio()
    {
        var cliente = ClienteValido();
        cliente.Nome = "";

        cliente.Validar().Should().ContainSingle().Which.Should().Be("Nome é obrigatório.");
    }

    [Fact]
    public void Validar_NomeComMaisDe150Caracteres_DeveRetornarErro()
    {
        var cliente = ClienteValido();
        cliente.Nome = new string('A', 151);

        cliente.Validar().Should().ContainSingle()
            .Which.Should().Be("Nome deve ter no máximo 150 caracteres.");
    }

    [Fact]
    public void Validar_CpfVazio_DeveRetornarCpfObrigatorio()
    {
        var cliente = ClienteValido();
        cliente.Cpf = "";

        cliente.Validar().Should().ContainSingle().Which.Should().Be("CPF é obrigatório.");
    }

    [Fact]
    public void Validar_CpfInvalido_DeveRetornarCpfInvalido()
    {
        var cliente = ClienteValido();
        cliente.Cpf = "12345678900";

        cliente.Validar().Should().ContainSingle().Which.Should().Be("CPF inválido.");
    }

    [Fact]
    public void Validar_EmailComMaisDe200Caracteres_DeveRetornarErro()
    {
        var cliente = ClienteValido();
        cliente.Email = new string('a', 195) + "@mail.com";

        cliente.Validar().Should().ContainSingle()
            .Which.Should().Be("E-mail deve ter no máximo 200 caracteres.");
    }

    [Theory]
    [InlineData("maria@")]
    [InlineData("sem-arroba.com")]
    [InlineData("Carlos <carlos.lima@mail.com>")]
    public void Validar_EmailInvalido_DeveRetornarEmailInvalido(string email)
    {
        var cliente = ClienteValido();
        cliente.Email = email;

        cliente.Validar().Should().ContainSingle().Which.Should().Be("E-mail inválido.");
    }

    [Theory]
    [InlineData("319876543")]
    [InlineData("319876543210")]
    public void Validar_TelefoneComQuantidadeInvalidaDeDigitos_DeveRetornarErro(string telefone)
    {
        var cliente = ClienteValido();
        cliente.Telefone = telefone;

        cliente.Validar().Should().ContainSingle()
            .Which.Should().Be("Telefone deve ter 10 ou 11 dígitos.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void Validar_DataNascimentoHojeOuFutura_DeveRetornarErro(int diasAFrente)
    {
        var cliente = ClienteValido();
        cliente.DataNascimento = DateTime.Today.AddDays(diasAFrente);

        cliente.Validar().Should().ContainSingle()
            .Which.Should().Be("A data de nascimento deve ser anterior à data de hoje.");
    }

    [Fact]
    public void Validar_DataNascimentoAnterior1900_DeveRetornarErro()
    {
        var cliente = ClienteValido();
        cliente.DataNascimento = new DateTime(1899, 12, 31);

        cliente.Validar().Should().ContainSingle()
            .Which.Should().Be("Data de nascimento deve ser maior ou igual a 01/01/1900.");
    }

    private static Cliente ClienteValido() => new()
    {
        Cpf = "28471539691",
        Nome = "Carlos Eduardo Lima",
        Email = "carlos.lima@mail.com",
        Telefone = "31987654321",
        DataNascimento = new DateTime(1987, 11, 23)
    };
}
