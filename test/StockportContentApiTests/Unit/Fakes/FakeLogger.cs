namespace StockportContentApiTests.Unit.Fakes
{
    public class FakeLogger<T> : ILogger<T>
    {
        public string ErrorMessage;
        public string InfoMessage;
        public string DebugMessage;
        public Exception Exception;

        public void Error(string message)
        {
            ErrorMessage = message;
        }

        public void Error(string message, Exception exception)
        {
            ErrorMessage = message;
            Exception = exception;
        }

        public void Debug(string message)
        {
            DebugMessage = message;
        }

        public void Info(string message)
        {
            InfoMessage = message;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Debug(formatter.Invoke(state, exception));
                    break;
                case LogLevel.Information:
                    Info(formatter.Invoke(state, exception));
                    break;
                case LogLevel.Error:
                    if (exception == null)
                        Error(formatter.Invoke(state, null));
                    else
                        Error(formatter.Invoke(state, exception), exception);
                    break;
                default:
                    // Don't do anything
                    Console.WriteLine("Fake logger recieved unexpected input.");
                    break;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new MemoryStream();
        }
    }
}