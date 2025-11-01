using UnityEngine;

namespace KarenKrill.UniCore.Logging
{
    public static class LoggerExtensions
    {
        public static void LogWarning(this ILogger logger, object message)
        {
            logger.LogWarning(string.Empty, message);
        }
    }
}
