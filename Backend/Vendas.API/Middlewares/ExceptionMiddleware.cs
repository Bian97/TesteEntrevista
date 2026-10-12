using Microsoft.Data.SqlClient;
using Vendas.Common.Utilitarios;

namespace Vendas.API.Middlewares;

public class ExceptionMiddleware
{
    private const int ErroIndiceUnico = 2601;
    private const int ErroViolacaoUnique = 2627;
    private const int ErroChaveEstrangeira = 547;

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidacaoException ex)
        {
            await Responder(context, StatusCodes.Status400BadRequest, new { mensagem = "Dados inválidos.", erros = ex.Erros });
        }
        catch (ConflitoException ex)
        {
            await Responder(context, StatusCodes.Status409Conflict, new { mensagem = ex.Message });
        }
        catch (NaoEncontradoException ex)
        {
            await Responder(context, StatusCodes.Status404NotFound, new { mensagem = ex.Message });
        }
        catch (SqlException ex) when (ex.Number == ErroIndiceUnico || ex.Number == ErroViolacaoUnique)
        {
            await Responder(context, StatusCodes.Status409Conflict, new { mensagem = "Registro duplicado." });
        }
        catch (SqlException ex) when (ex.Number == ErroChaveEstrangeira)
        {
            await Responder(context, StatusCodes.Status409Conflict, new { mensagem = "Operação inválida: o registro está em uso." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado na requisição {Metodo} {Caminho}", context.Request.Method, context.Request.Path);
            await Responder(context, StatusCodes.Status500InternalServerError, new { mensagem = "Ocorreu um erro interno no servidor." });
        }
    }

    private static Task Responder(HttpContext context, int status, object corpo)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(corpo);
    }
}
