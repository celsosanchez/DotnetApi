using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.EventSource;
using Microsoft.Extensions.Options;

namespace RestApi.Controllers;

public class LoggerOptions
{
    public virtual string FilePath { get; set; } = "log_{date}.log";
    public virtual string FolderPath { get; set; } = ".";

}
[ProviderAlias("DataLoggerFile")]
public class DataLoggerProvider : ILoggerProvider
{
    public readonly LoggerOptions Options;
    public DataLoggerProvider(IOptions<LoggerOptions> _options)
    {
        Options = _options.Value;
        if (!Directory.Exists(Options.FolderPath))
        {
            Directory.CreateDirectory(Options.FolderPath);
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new DataLogger(this);
    }

    public void Dispose()
    {
    }
}

public class DataLogger : ILogger
{
    protected readonly DataLoggerProvider _dataLoggerProvider;

    public DataLogger([NotNull] DataLoggerProvider dataLoggerProvider)
    {
        _dataLoggerProvider = dataLoggerProvider;
    }

    public IDisposable? BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var fullFilePath = _dataLoggerProvider.Options.FolderPath + "/" + _dataLoggerProvider.Options.FilePath.Replace("{date}", DateTimeOffset.UtcNow.ToString("yyyyMMdd"));
        var logRecord = string.Format("{0} [{1}] {2} {3}", "[" + DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss+00:00") + "]", logLevel.ToString(), formatter(state, exception), exception != null ? exception.StackTrace : "");

        using (var streamWriter = new StreamWriter(fullFilePath, true))
        {
            streamWriter.WriteLine(logRecord);
        }
    }
}
public static class DataLoggerExtensions
{
    public static ILoggingBuilder AddDataFileLogger(this ILoggingBuilder builder, Action<LoggerOptions> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, DataLoggerProvider>();
        builder.Services.Configure(configure);
        return builder;
    }
}