using MusicStore.Dto.Request;
using MusicStore.Dto.Validations;

namespace MusicStore.UnitTests;

public class FluentValidationTests
{
    [Fact]
    public void SaleDtoRequestValidator_ShouldHaveError_WhenQuantityIsZero()
    {
        // Arrange
        var validator = new SaleDtoRequestValidator();
        var request = new SaleDtoRequest(1, 0);

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TicketQuantity");
    }

    [Fact]
    public void SaleDtoRequestValidator_ShouldPass_WhenQuantityIsValid()
    {
        // Arrange
        var validator = new SaleDtoRequestValidator();
        var request = new SaleDtoRequest(1, 2);

        // Act
        var result = validator.Validate(request);

        // Assert
        Assert.True(result.IsValid);
    }
}
