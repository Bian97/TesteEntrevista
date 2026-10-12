using Microsoft.AspNetCore.Mvc;
using Vendas.Abstractions.Interfaces.Services;
using Vendas.API.ViewModels;

namespace Vendas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? busca)
    {
        var produtos = await _produtoService.ListarAsync(busca);
        return Ok(produtos.Select(ProdutoResponse.De));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var produto = await _produtoService.ObterAsync(id);
        return Ok(ProdutoResponse.De(produto));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ProdutoRequest request)
    {
        var produto = await _produtoService.CriarAsync(request.ParaEntidade());
        return Ok(ProdutoResponse.De(produto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ProdutoRequest request)
    {
        var entidade = request.ParaEntidade();
        entidade.Id = id;
        var produto = await _produtoService.AtualizarAsync(entidade);
        return Ok(ProdutoResponse.De(produto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _produtoService.ExcluirAsync(id);
        return NoContent();
    }
}
