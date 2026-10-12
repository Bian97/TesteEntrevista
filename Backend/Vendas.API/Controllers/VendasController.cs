using Microsoft.AspNetCore.Mvc;
using Vendas.Abstractions.Interfaces.Services;
using Vendas.API.ViewModels;

namespace Vendas.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var vendas = await _vendaService.ListarAsync();
        return Ok(vendas.Select(VendaResponse.De));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(int id)
    {
        var venda = await _vendaService.ObterAsync(id);
        return Ok(VendaResponse.De(venda));
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] VendaRequest request)
    {
        var venda = await _vendaService.CriarAsync(request.ParaEntidade());
        return Ok(VendaResponse.De(venda));
    }
}
