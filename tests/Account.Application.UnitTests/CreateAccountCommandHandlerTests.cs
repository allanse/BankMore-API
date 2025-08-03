using Account.Application.Contracts;
using Account.Application.Exceptions;
using Account.Application.Features.Accounts.Commands.CreateAccount;
using Account.Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Account.Application.UnitTests;

public class CreateAccountCommandHandlerTests
{
    private readonly Mock<ICurrentAccountRepository> _repositoryMock;
    private readonly Mock<IPasswordHasher<CurrentAccount>> _passwordHasherMock;

    public CreateAccountCommandHandlerTests()
    {        
        _repositoryMock = new Mock<ICurrentAccountRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher<CurrentAccount>>();
    }

    [Fact]
    public async Task Handle_ShouldThrowInvalidOperationException_WhenCpfAlreadyExists()
    {
        // Arrange        
        var command = new CreateAccountCommand
        {
            Nome = "Nico Bico",
            Cpf = "12345678901",
            Password = "NaoUso123!"
        };
        
        _repositoryMock.Setup(r => r.CpfExistsAsync(It.IsAny<string>()))
                       .ReturnsAsync(true);
        
        var handler = new CreateAccountCommandHandler(_repositoryMock.Object, _passwordHasherMock.Object);

        // Act       
        Func<Task> action = async () => await handler.Handle(command, CancellationToken.None);

        // Assert        
        await action.Should().ThrowAsync<InvalidDocumentException>()
                    .WithMessage("O CPF informado já foi cadastrado!");
    }
}