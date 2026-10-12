using FluentAssertions;
using Moq;
using Vendas.Abstractions.Interfaces.Repositories;
using Vendas.Business.Services;
using Vendas.Common.Utilitarios;
using Vendas.Model.Entities;

namespace Vendas.Tests.Business.ProdutoServiceTests;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _repositorio = new();
    private readonly ProdutoService _servico;

    public ProdutoServiceTests()
    {
        _servico = new ProdutoService(_repositorio.Object);
    }

    [Fact]
    public async Task Criar_ProdutoValido_DeveGravarEDevolverComId()
    {
        _repositorio.Setup(r => r.CriarAsync(It.IsAny<Produto>())).ReturnsAsync(5);

        var resultado = await _servico.CriarAsync(ProdutoValido());

        resultado.Id.Should().Be(5);
    }

    [Fact]
    public async Task Criar_EntidadeInvalida_DeveLancarValidacaoSemGravar()
    {
        var produto = ProdutoValido();
        produto.Nome = "   ";

        Func<Task> acao = () => _servico.CriarAsync(produto);

        await acao.Should().ThrowAsync<ValidacaoException>();
        _repositorio.Verify(r => r.CriarAsync(It.IsAny<Produto>()), Times.Never);
    }

    [Fact]
    public async Task Criar_NomeDuplicado_DeveLancarConflito()
    {
        _repositorio.Setup(r => r.ExisteNomeAsync("Fone Bluetooth", null)).ReturnsAsync(true);

        Func<Task> acao = () => _servico.CriarAsync(ProdutoValido());

        await acao.Should().ThrowAsync<ConflitoException>()
            .WithMessage("Já existe um produto cadastrado com este nome.");
    }

    [Fact]
    public async Task Operacoes_ProdutoInexistente_DeveLancarNaoEncontrado()
    {
        var produto = ProdutoValido();
        produto.Id = 99;

        Func<Task> obter = () => _servico.ObterAsync(99);
        Func<Task> atualizar = () => _servico.AtualizarAsync(produto);
        Func<Task> excluir = () => _servico.ExcluirAsync(99);

        await obter.Should().ThrowAsync<NaoEncontradoException>().WithMessage("Produto não encontrado.");
        await atualizar.Should().ThrowAsync<NaoEncontradoException>().WithMessage("Produto não encontrado.");
        await excluir.Should().ThrowAsync<NaoEncontradoException>().WithMessage("Produto não encontrado.");
    }

    [Fact]
    public async Task Excluir_ProdutoComVendas_DeveLancarConflito()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(ProdutoValido());
        _repositorio.Setup(r => r.PossuiVendasAsync(2)).ReturnsAsync(true);

        Func<Task> acao = () => _servico.ExcluirAsync(2);

        await acao.Should().ThrowAsync<ConflitoException>()
            .WithMessage("Não é possível excluir um produto que possui vendas.");
        _repositorio.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Excluir_ProdutoValido_DeveChamarRepositorio()
    {
        _repositorio.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(ProdutoValido());

        await _servico.ExcluirAsync(2);

        _repositorio.Verify(r => r.ExcluirAsync(2), Times.Once);
    }

    private static Produto ProdutoValido() => new()
    {
        Nome = "Fone Bluetooth",
        ValorUnitario = 129.50m
    };
}
