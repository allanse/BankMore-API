using Account.Domain;
using FluentAssertions;

namespace Account.Application.UnitTests;

public class MovimentoTests
{
    [Fact]
    public void Create_ShouldThrowDomainException_WhenValueIsNegative()
    {
        // Arrange
        var idContaCorrente = Guid.NewGuid();
        var tipoMovimento = 'C';
        var valorInvalido = -100.0m;

        // Act        
        Action action = () => Movimento.Create(idContaCorrente, tipoMovimento, valorInvalido);

        // Assert        
        action.Should().Throw<DomainException>()
              .WithMessage("O valor da movimentação deve ser positivo.");
    }

    [Fact]
    public void Create_ShouldCreateMovement_WhenDataIsValid()
    {
        // Arrange
        var idContaCorrente = Guid.NewGuid();
        var tipoMovimento = 'D';
        var valorValido = 250.75m;

        // Act
        var movimento = Movimento.Create(idContaCorrente, tipoMovimento, valorValido);

        // Assert
        movimento.Should().NotBeNull();
        movimento.IdContaCorrente.Should().Be(idContaCorrente);
        movimento.Valor.Should().Be(valorValido);
        movimento.TipoMovimento.Should().Be(tipoMovimento);
    }
}