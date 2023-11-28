using System.Diagnostics;
using Cronos;

namespace CronScheduledService;

/// <summary>
///     周期性计时器。
/// </summary>
public sealed class CronosPeriodicTimer : IDisposable
{
    private PeriodicTimer? _activeTimer;

    private static readonly TimeSpan MinDelay = TimeSpan.FromMilliseconds(500);

    /// <summary>
    ///     初始化一个 <see cref="CronosPeriodicTimer"/> 类的新实例。
    /// </summary>
    /// <param name="expression"> 表达式 </param>
    /// <param name="format"> 格式 </param>
    public CronosPeriodicTimer(string expression, CronFormat format)
    {
        Expression = CronExpression.Parse(expression, format);
    }

    /// <summary>
    ///     是否已释放。
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    ///     表达式。
    /// </summary>
    public CronExpression Expression { get; }

    /// <summary>
    ///     等待下一个周期。
    /// </summary>
    /// <param name="cancellationToken"> 取消令牌 </param>
    /// <returns> 是否到达下一个周期 </returns>
    /// <exception cref="InvalidOperationException"> 一次只能有一个消费者。 </exception>
    public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PeriodicTimer timer;
        lock (Expression)
        {
            if (IsDisposed) return false;
            if (_activeTimer is not null)
                throw new InvalidOperationException("One consumer at a time.");
            DateTime utcNow = DateTime.UtcNow;

            DateTime? utcNext = Expression.GetNextOccurrence(utcNow + MinDelay, TimeZoneInfo.Local);
            if (utcNext is null)
                throw new InvalidOperationException("Unreachable date.");
            TimeSpan delay = utcNext.Value - utcNow;
            Debug.Assert(delay > MinDelay);
            timer = _activeTimer = new PeriodicTimer(delay);
        }

        try
        {
            // Dispose the timer after the first tick.
            using (timer)
            {
                return await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            Volatile.Write(ref _activeTimer, null);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        PeriodicTimer? activeTimer;
        lock (Expression)
        {
            if (IsDisposed) return;
            IsDisposed = true;
            activeTimer = _activeTimer;
        }

        activeTimer?.Dispose();
    }
}
