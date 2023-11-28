using Serilog;

namespace CronScheduledService;

public class CronDemoService : CronBackgroundService
{
    /// <inheritdoc />
    public CronDemoService(ILogger logger) : base(logger)
    {
    }

    /// <inheritdoc />
    protected override string ExpressionFactory() => "0-9,20-29,40-49 * * * * *";

    /// <inheritdoc />
    protected override Task ExecuteTaskAsync(CancellationToken stoppingToken)
    {
        Logger.Information("Executing at {Time}", DateTime.Now);
        return Task.CompletedTask;
    }
}
