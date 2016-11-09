using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Moq;

namespace StockportContentApiTests
{
    public class LogTesting
    {
        public static void Assert<T>(Mock<ILogger<T>> loggerMock, LogLevel logLevel, string logMessage)
        {
            loggerMock.Verify(
                x => x.Log(
                        logLevel, 
                        0, 
                        new FormattedLogValues(logMessage), 
                        It.IsAny<Exception>(),
                        It.IsAny<Func<object, Exception, string>>()
                    ), 
                Times.AtLeastOnce);
        }
    }
}