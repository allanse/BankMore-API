using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Transfers.Application.Exceptions;
using Transfers.Application.Features.Transfers.Commands.InitiateTransfer;

namespace Transfers.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/transfers")]
public class TransfersController : ControllerBase
{
    private readonly ISender _mediator;

    public TransfersController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InitiateTransfer(
        [FromHeader(Name = "X-Idempotency-Key")] Guid idRequisicao,
        [FromBody] InitiateTransferRequest request)
    {
        var command = new InitiateTransferCommand
        {
            IdRequisicao = idRequisicao,
            NumeroContaDestino = request.NumeroContaDestino,
            Valor = request.Valor
        };

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (Exception ex) when (ex is InvalidValueException or InvalidAccountException or InactiveAccountException)
        {
            dynamic exception = ex;
            return BadRequest(new { mensagem = exception.Message, tipoFalha = exception.FailureType });
        }
    }
}

public class InitiateTransferRequest
{
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
}