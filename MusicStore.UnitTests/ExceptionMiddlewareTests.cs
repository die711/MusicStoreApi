using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using MusicStore.Api.Middlewares;

namespace MusicStore.UnitTests;

public class ExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ShouldCatchExceptionAndReturn500()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<ExceptionMiddleware>>();

        // Simular un request delegate que lanza excepcion
        RequestDelegate next = (HttpContext ctx) => throw new InvalidOperationException("Test exception");

        var middleware = new ExceptionMiddleware(next, mockLogger.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream(); // To prevent writing to a null stream

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);

        // Verify the logger was called
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!),
            Times.Once);
    }
}
