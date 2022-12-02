using Moq;

namespace StockportContentApiTests
{
    public class LogTesting
    {
        public static void Assert<T>(Mock<ILogger<T>> loggerMock, LogLevel logLevel, string logMessage)
        {
            Func<object, Type, bool> state = (v, t) => v.ToString().Contains(logMessage);

            loggerMock.Verify(_ => _.Log(logLevel, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => state(v, t)), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }
    }
}