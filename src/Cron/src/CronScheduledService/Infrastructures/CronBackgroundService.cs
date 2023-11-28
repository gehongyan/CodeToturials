using Cronos;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CronScheduledService;

/// <summary>
///     基于 Cron 表达式的后台服务。
/// </summary>
public abstract class CronBackgroundService : BackgroundService
{
    private CronosPeriodicTimer? _timer;
    private readonly string? _serviceName;

    /// <summary>
    ///     初始化一个 <see cref="CronBackgroundService"/> 类的新实例。
    /// </summary>
    /// <param name="logger"> 日志。 </param>
    protected CronBackgroundService(ILogger logger)
    {
        Logger = logger;
        _serviceName = GetType().Name;
    }

    /// <summary>
    ///     日志
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    ///     获取 Cron 表达式工厂方法。
    /// </summary>
    protected abstract string ExpressionFactory();

    /// <summary>
    ///     重新创建定时器。
    /// </summary>
    protected void RecreateTimer()
    {
        _timer?.Dispose();
        string expression = ExpressionFactory();
        _timer = CreateTimer(expression);
    }

    private CronosPeriodicTimer? CreateTimer(string expression)
    {
        Logger.Information("{TaskName} is creating timer with cron expression {CronExpression}",
            _serviceName, expression);
        try
        {
            return new CronosPeriodicTimer(expression, CronFormat.IncludeSeconds);
        }
        catch (CronFormatException e)
        {
            Logger.Error(e, "{TaskName} failed to create timer with cron expression {CronExpression}",
                _serviceName, expression);
        }

        return null;
    }

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        string currentCron = ExpressionFactory();
        _timer = CreateTimer(currentCron);
        await base.StartAsync(cancellationToken);
    }

    /// <summary>
    ///     执行任务。
    /// </summary>
    /// <param name="stoppingToken"> 取消令牌。 </param>
    /// <returns> 表示异步操作的任务。 </returns>
    protected abstract Task ExecuteTaskAsync(CancellationToken stoppingToken);

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_timer is null or { IsDisposed: true })
                {
                    Logger.Verbose("{TaskName} timer is not created or disposed",
                        _serviceName);
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                    continue;
                }

                DateTime? nextOccurrence = _timer.Expression.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local);
                Logger.Information("{TaskName} timer next occurrence is {NextOccurrence:yyyy'-'MM'-'dd HH':'mm':'ss zzzz} (in {Span})",
                    _serviceName,
                    nextOccurrence?.ToLocalTime(), nextOccurrence - DateTime.UtcNow);

                if (!await _timer.WaitForNextTickAsync(stoppingToken))
                {
                    Logger.Verbose("{TaskName} timer is signaled to stop",
                        _serviceName);
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                    continue;
                }

                Logger.Information("{TaskName} is about to execute...",
                    _serviceName);
                await ExecuteTaskAsync(stoppingToken);
            }
            catch (Exception e)
            {
                Logger.Error(e, "{TaskName} task failed",
                    _serviceName);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        await base.StopAsync(cancellationToken);
    }

}
