using FluentAssertions;
using Moq;
using Vendas.Abstractions.Interfaces.Repositories;
using Vendas.Business.Services;
using Vendas.Common.Utilitarios;
using Vendas.Model.Entities;

namespace Vendas.Tests.Business.ClienteServiceTests;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _repositorio = new();
    private readonly ClienteService _servico;

    public ClienteServiceTests()
    {
        _servico = new ClienteService(_repositorio.Object);
    }

    [Fact]
    public async Task Criar_ClienteValido_DeveGravarEDevolverComId()
    {
        _repositorio.Setup(r => r.CriarAsync(It.IsAny<Cliente>())).ReturnsAsync(7);

        var resultado = await _servico.CriarAsync(ClienteValido());

        resultado.Id.Should().Be(7);
    }

    [Fact]
    public async Task Criar_EntidadeInvalida_DeveLancarValidacaoSemGravar()
    {
        var cliente = ClienteValido();
        cliente.Nome = "";

        Func<Task> acao = () => _servico.CriarAsync(cliente);

        await acao.Should().ThrowAsync<ValidacaoException>();
        _repositorio.Verify(r => r.CriarAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public async Task Criar_CpfDuplicado_DeveLancarConflito()
    {
        _repositorio.Setup(r => r.ExisteCpfAsync("63029481778", null)).ReturnsAsync(true);

        Func<Task> acao = () => _servico.CriarAsync(ClienteValido());

        await acao.Should().ThrowAsync<ConflitoException>()
            .WithMessage("Já existe um cliente cadastrado com este CPF.");
    }

    [Fact]
    public async Task Operacoes_ClienteInexistente_DeveLancarNaoEncontrado()
    {
        var cliente = ClienteValido();
        cliente.Id = 99;

        Func<Task> obter = () => _servico.ObterAsync(99);
        Func<Task> atualizar = () => _servico.AtualizarAsync(cliente);
        Func<Task> excluir = () => _servico.ExcluirAsync(99);

        await obter.Should().ThrowAsync<NaoEncontradoException>().WithMessage("Cliente não encontrado.");
        await atualizar.Should().ThrowAsync<NaoEncontradoException>().WithMessage("Cliente não encontrado.");
        await excluir.Should().ThrowAsync<NaoEncontradoException>().WithMessage("Cliente não encontrado.");
    }

    [Fact]
    public async Task Excluir_ClienteComVendas_DeveLancarConflito()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(3)).ReturnsAsync(ClienteValido());
        _repositorio.Setup(r => r.PossuiVendasAsync(3)).ReturnsAsync(true);

        Func<Task> acao = () => _servico.ExcluirAsync(3);

        await acao.Should().ThrowAsync<ConflitoException>()
            .WithMessage("Não é possível excluir um cliente que possui vendas.");
        _repositorio.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Excluir_ClienteValido_DeveChamarRepositorio()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(3)).ReturnsAsync(ClienteValido());

        await _servico.ExcluirAsync(3);

        _repositorio.Verify(r => r.ExcluirAsync(3), Times.Once);
    }

    private static Cliente ClienteValido() => new()
    {
        Cpf = "63029481778",
        Nome = "Fernanda Ribeiro",
        Email = "fernanda.ribeiro@provedor.com.br",
        Telefone = "21976543210",
        DataNascimento = new DateTime(1994, 2, 17)
    };
}
