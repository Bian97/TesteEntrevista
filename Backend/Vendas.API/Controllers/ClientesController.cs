using Microsoft.AspNetCore.Mvc;
using Vendas.Abstractions.Interfaces.Services;
using Vendas.API.ViewModels;

namespace Vendas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? busca)
    {
        var clientes = await _clienteService.ListarAsync(busca);
        return Ok(clientes.Select(ClienteResponse.De));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var cliente = await _clienteService.ObterAsync(id);
        return Ok(ClienteResponse.De(cliente));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ClienteRequest request)
    {
        var cliente = await _clienteService.CriarAsync(request.ParaEntidade());
        return Ok(ClienteResponse.De(cliente));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] ClienteRequest request)
    {
        var entidade = request.ParaEntidade();
        entidade.Id = id;
        var cliente = await _clienteService.AtualizarAsync(entidade);
        return Ok(ClienteResponse.De(cliente));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Excluir(int id)
    {
        await _clienteService.ExcluirAsync(id);
        return NoContent();
    }
}
