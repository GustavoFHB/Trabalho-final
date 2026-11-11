using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SimuladorFormal.Api.Infra;

/// <summary>Converte erros de validação do domínio (ArgumentException) em HTTP 400 { "erro": "..." }.</summary>
public sealed class ErroValidacaoFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ArgumentException e) return;
        context.Result = new BadRequestObjectResult(new { erro = e.Message });
        context.ExceptionHandled = true;
    }
}
