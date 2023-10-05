using Microsoft.Extensions.Logging;

namespace MusicStore.Services.Utils;

public static class Utilities
{
    public static int GetTotalPages(int total, int rows)
    {
        var totalPages = total / rows;
        if (total % rows > 0)
        {
            totalPages++;
        }

        return totalPages;
    }

    public static string LogMessage(this ILogger logger, Exception exception, string methodCaller, bool error = true)
    {
        var friendlyMessage = $"Error en {methodCaller}";
        if (error)
            logger.LogCritical(exception, "{friendlyMessage} {Message}", friendlyMessage, exception.Message);
        else
            logger.LogWarning(exception, "{friendlyMessage} {Message}", friendlyMessage, exception.Message);
        return friendlyMessage;
        
    }
    
}