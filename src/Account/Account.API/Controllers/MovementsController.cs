using Account.Application.Exceptions;
using Account.Application.Features.Movements.Commands.CreateMovement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/movements")]
public class MovementsController : ControllerBase
{
    private readonly ISender _mediator;

    public MovementsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateMovement(
        [FromHeader(Name = "X-Idempotency-Key")] Guid idRequisicao,
        [FromBody] CreateMovementRequest request)
    {
        if (idRequisicao == Guid.Empty)
        {
            return BadRequest(new { mensagem = "O cabeçalho X-Idempotency-Key é obrigatório.", tipoFalha = "MISSING_HEADER" });
        }

        var command = new CreateMovementCommand
        {
            IdRequisicao = idRequisicao,
            NumeroConta = request.NumeroConta,
            Valor = request.Valor,
            TipoMovimento = request.TipoMovimento
        };

        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InsufficientFundsException ex)
        {
            return BadRequest(new { mensagem = ex.Message, tipoFalha = ex.FailureType });
        }
        catch (Exception ex) when (ex is InvalidAccountException or InactiveAccountException or InvalidValueException or InvalidMovementTypeException)
        {           
            dynamic exception = ex;
            return BadRequest(new { mensagem = exception.Message, tipoFalha = exception.FailureType });
        }
        catch (ApplicationException ex) 
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }
}

public class CreateMovementRequest
{
    public int? NumeroConta { get; set; }
    public decimal Valor { get; set; }
    public char TipoMovimento { get; set; }
}