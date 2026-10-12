using FluentAssertions;
using Moq;
using Vendas.Abstractions.Interfaces.Repositories;
using Vendas.Business.Services;
using Vendas.Common.Utilitarios;
using Vendas.Model.Entities;

namespace Vendas.Tests.Business.VendaServiceTests;

public class VendaServiceTests
{
    private readonly Mock<IVendaRepository> _vendas = new();
    private readonly Mock<IClienteRepository> _clientes = new();
    private readonly Mock<IProdutoRepository> _produtos = new();
    private readonly VendaService _servico;

    public VendaServiceTests()
    {
        _servico = new VendaService(_vendas.Object, _clientes.Object, _produtos.Object);
        _clientes.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Cliente { Id = 1 });
        _produtos.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(new Produto { Id = 1, ValorUnitario = 12.50m });
        _produtos.Setup(r => r.ObterPorIdAsync(2)).ReturnsAsync(new Produto { Id = 2, ValorUnitario = 8.75m });
    }

    [Fact]
    public async Task Criar_VendaValida_DeveDevolverVendaCompleta()
    {
        _vendas.Setup(r => r.CriarAsync(It.IsAny<Venda>())).ReturnsAsync(10);
        _vendas.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(new Venda { Id = 10 });

        var resultado = await _servico.CriarAsync(VendaValida());

        resultado.Id.Should().Be(10);
    }

    [Fact]
    public async Task Criar_EntidadeInvalida_DeveLancarValidacaoSemGravar()
    {
        var venda = VendaValida();
        venda.Itens.Clear();

        Func<Task> acao = () => _servico.CriarAsync(venda);

        await acao.Should().ThrowAsync<ValidacaoException>();
        _vendas.Verify(r => r.CriarAsync(It.IsAny<Venda>()), Times.Never);
    }

    [Fact]
    public async Task Criar_ClienteInexistente_DeveLancarValidacao()
    {
        var venda = VendaValida();
        venda.ClienteId = 99;

        Func<Task> acao = () => _servico.CriarAsync(venda);

        await acao.Should().ThrowAsync<ValidacaoException>()
            .WithMessage("*Cliente não informado ou inexistente.*");
    }

    [Fact]
    public async Task Criar_ProdutoInexistente_DeveLancarValidacao()
    {
        var venda = VendaValida();
        venda.Itens[0].ProdutoId = 99;

        Func<Task> acao = () => _servico.CriarAsync(venda);

        await acao.Should().ThrowAsync<ValidacaoException>()
            .WithMessage("Item 1: produto inexistente.*");
    }

    [Fact]
    public async Task Criar_TotalEnviadoPeloCliente_DeveSerRecalculadoNoServidor()
    {
        var venda = VendaValida();
        venda.ValorTotal = 1m;
        Venda? gravada = null;
        _vendas.Setup(r => r.CriarAsync(It.IsAny<Venda>()))
            .Callback<Venda>(v => gravada = v)
            .ReturnsAsync(10);
        _vendas.Setup(r => r.ObterPorIdAsync(10)).ReturnsAsync(new Venda { Id = 10 });

        await _servico.CriarAsync(venda);

        gravada.Should().NotBeNull();
        gravada!.ValorTotal.Should().Be(2 * 12.50m + 3 * 8.75m);
    }

    [Fact]
    public async Task Obter_VendaInexistente_DeveLancarNaoEncontrado()
    {
        Func<Task> acao = () => _servico.ObterAsync(99);

        await acao.Should().ThrowAsync<NaoEncontradoException>()
            .WithMessage("Venda não encontrada.");
    }

    private static Venda VendaValida()
    {
        return new Venda
        {
            ClienteId = 1,
            DataVenda = DateTime.Today,
            Itens =
            [
                new ItemVenda { ProdutoId = 1, Quantidade = 2, ValorUnitario = 12.50m },
                new ItemVenda { ProdutoId = 2, Quantidade = 3, ValorUnitario = 8.75m }
            ]
        };
    }
}
