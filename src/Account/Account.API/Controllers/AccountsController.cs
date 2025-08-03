using Account.Application.Exceptions;
using Account.Application.Features.Accounts.Commands.CreateAccount;
using Account.Application.Features.Accounts.Commands.InactivateAccount;
using Account.Application.Features.Accounts.Queries.GetAccountBalance;
using Account.Application.Features.Accounts.Queries.GetAccountById;
using Account.Application.Features.Accounts.Queries.GetAccountByNumber;
using Account.Application.Features.Accounts.Queries.Login;
using Account.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Account.API.Controllers;

[ApiController]
[Route("api/v1/accounts")]
public class AccountsController : ControllerBase
{
    private readonly ISender _mediator;

    public AccountsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAccount([FromBody] CreateAccountCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return Created(string.Empty, response);
        }        
        catch (InvalidDocumentException ex)
        {            
            return BadRequest(new { mensagem = ex.Message, tipoFalha = ex.FailureType });
        }        
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginQuery query)
    {
        try
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {            
            return Unauthorized(new
            {
                mensagem = ex.Message,
                FailureType = "USER_UNAUTHORIZED"
            });
        }
    }

    [HttpDelete("current")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> InactivateCurrentAccount([FromBody] InactivateCurrentAccountCommand command)
    {
        try
        {
            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidAccountException ex)
        {
            return BadRequest(new { mensagem = ex.Message, tipoFalha = ex.FailureType });
        }
        catch (UnauthorizedAccessException ex)
        {            
            return Unauthorized(new { mensagem = ex.Message });
        }
        catch (DomainException ex)
        {            
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("current/balance")]
    [Authorize]
    [ProducesResponseType(typeof(GetAccountBalanceQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAccountBalance()
    {
        try
        {
            var query = new GetAccountBalanceQuery();
            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (Exception ex) when (ex is InvalidAccountException or InactiveAccountException)
        {            
            dynamic exception = ex;
            return BadRequest(new { mensagem = exception.Message, tipoFalha = exception.FailureType });
        }
    }

    [HttpGet("{accountNumber:int}", Name = "GetAccountByNumber")]
    [Authorize]
    [ProducesResponseType(typeof(AccountDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountByNumber(int accountNumber)
    {
        var query = new GetAccountByNumberQuery { AccountNumber = accountNumber };
        var response = await _mediator.Send(query);

        return response is not null ? Ok(response) : NotFound();
    }

    [HttpGet("by-id/{accountId:guid}", Name = "GetAccountById")]
    [Authorize]
    [ProducesResponseType(typeof(AccountDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountById(Guid accountId)
    {
        var query = new GetAccountByIdQuery { AccountId = accountId };
        var response = await _mediator.Send(query);

        return response is not null ? Ok(response) : NotFound();
    }
}