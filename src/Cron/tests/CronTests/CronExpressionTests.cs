namespace CronTests;

/// <summary>
///     Cron 表达式测试。
/// </summary>
public class CronExpressionTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    ///     初始化一个 <see cref="CronExpressionTests"/> 类的新实例。
    /// </summary>
    /// <param name="testOutputHelper"> 测试输出辅助类 </param>
    public CronExpressionTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    /// <summary>
    ///     Cron 表达式基本语法
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         本项目使用 Cronos 类库作为 Cron 表达式解析库。参见：https://github.com/HangfireIO/Cronos
    ///     </para>
    ///     <para>
    ///         Cron 表达式可以用于定时周期性任务的触发时间规则，标准 Cron 表达式可以有 5 个字段，分别为
    ///         分钟、小时、日期、月份、星期。部分应用场景下可能也会有扩展的字段支持，例如 秒、年。
    ///         Cronos 类库支持 5 字段标准 Cron 表达式与额外支持指定秒的 6 字段扩展 Cron 表达式，不支持指定年。
    ///         因此，Cron 表达式最小粒度为秒，小于秒的粒度，如毫秒、微秒等，都不支持。
    ///         字段内不允许存在空格，字段与字段之间应有至少一个空格。每个字段的允许值如下方代码注释。
    ///         除了允许值之外，还可以使用 <c>*</c> 表示任意值，<c>?</c> 表示不指定值，<c>-</c> 表示范围，<c>/</c> 表示步长。
    ///         除此之外，部分字段还支持 <c>L</c>、<c>W</c>、<c>#</c> 等特殊字符，具体含义请参考后文。
    ///         部分字段还支持别名，例如月份字段支持使用英文缩写 JAN-DEC 表示 1-12 月，星期字段支持使用 SUN-SAT 表示 0-6。
    ///         另外，星期字段的 0 和 7 都表示星期天。
    ///     </para>
    /// </remarks>
    [Fact]
    public void Syntax()
    {
        const string
            //                     Allowed values          Allowed special characters           Comment
            //
            //      ┌───────────── second (optional)       0-59              * , - /
            //      │ ┌───────────── minute                0-59              * , - /
            //      │ │ ┌───────────── hour                0-23              * , - /
            //      │ │ │ ┌───────────── day of month      1-31              * , - / L W ?
            //      │ │ │ │ ┌───────────── month           1-12 or JAN-DEC   * , - /
            //      │ │ │ │ │ ┌───────────── day of week   0-6  or SUN-SAT   * , - / # L ?      Both 0 and 7 means SUN
            //      │ │ │ │ │ │
            cron = "* * * * * *";
        CronExpression expression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
        DateTime now = DateTime.UtcNow;
        DateTime from = new(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, DateTimeKind.Utc);
        DateTime? nextOccurrence = expression.GetNextOccurrence(from, inclusive: true);
        Assert.Equal(from, nextOccurrence);
    }

    /// <summary>
    ///     指定数值。
    /// </summary>
    /// <remarks>
    ///     所有的字段都支持指定数值。例如：
    ///     <list type="bullet">
    ///         <item><term> <c>* * * * * ?</c> 表示每秒时执行。这是不指定任何值时的触发周期粒度。 </term></item>
    ///         <item><term> <c>0 * * * * ?</c> 表示每分钟的 0 秒时执行。 </term></item>
    ///         <item><term> <c>0 30 9 * * ?</c> 表示每天 9:30:00 执行。 </term></item>
    ///         <item><term> <c>0 30 9 * * 1</c> 表示每周一 9:30:00 执行。 </term></item>
    ///         <item><term> <c>0 30 9 1 * ?</c> 表示每月 1 日 9:30:00 执行。 </term></item>
    ///         <item><term> <c>0 30 9 1 10 ?</c> 表示每年 10 月 1 日 9:30:00 执行。 </term></item>
    ///         <item>
    ///             <term>
    ///                 <c>0 30 9 1 10 1</c> 表示每年 10 月 1 日为星期一的 9:30:00 执行。
    ///                 需要注意的是，标准的 Cron 表达式不支持同时指定日期和星期，因为一个月的第几天和星期几是互斥的。
    ///                 因此，标准的 Cron 表达式中，日期和星期只能指定一个，另一个必须使用 <c>?</c> 表示不指定。
    ///                 <c>?</c> 表示的是不指定值，而 <c>*</c> 表示的是任意值。
    ///                 但某些框架可能在此基础上扩展了同时支持指定这两个字段的功能。
    ///             </term>
    ///         </item>
    ///     </list>
    /// </remarks>
    [Theory]
    [InlineData("* * * * * ?", "每秒")]
    [InlineData("0 * * * * ?", "每分钟的 0 秒")]
    [InlineData("0 30 9 * * ?", "每天 9:30:00")]
    [InlineData("0 30 9 ? * 1", "每周一 9:30:00")]
    [InlineData("0 30 9 1 * ?", "每月 1 日 9:30:00")]
    [InlineData("0 30 9 1 10 ?", "每年 10 月 1 日 9:30:00")]
    [InlineData("0 30 9 1 10 1", "每年 10 月 1 日为星期一的 9:30:00")]
    public void SingleValue(string cron, string description)
    {
        _testOutputHelper.WriteLine("Cron 表达式单一数值示例");
        _testOutputHelper.WriteLine($"表达式 {cron}");
        _testOutputHelper.WriteLine(description);
        CronExpression cronExpression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
        PrintOccurrences(cronExpression);
        Assert.True(true);
    }

    /// <summary>
    ///     指定范围数值。
    /// </summary>
    /// <remarks>
    ///     所有的字段都支持指定范围数值。例如：
    ///     <list type="bullet">
    ///         <item><term> <c>0-3 * * * * ?</c> 表示每分钟的 0-3 秒时执行。 </term></item>
    ///         <item><term> <c>0 0 9-12 * * ?</c> 表示每天 9-12 点的 0 分 0 秒执行。 </term></item>
    ///         <item><term> <c>0 0 9-12 ? * 1-5</c> 表示每周一至周五 9-12 点的 0 分 0 秒执行。 </term></item>
    ///         <item><term> <c>0 30 22-2 * * ?</c> 表示每天 22 点至次日 2 点的 30 分 0 秒执行。 </term></item>
    ///     </list>
    /// </remarks>
    [Theory]
    [InlineData("0-3 0 * * * ?", "每小时 0 分的 0-3 秒")]
    [InlineData("0 0 9-12 * * ?", "每天 9-12 点的 0 分 0 秒")]
    [InlineData("0 0 9-12 ? * 1-5", "每周一至周五 9-12 点的 0 分 0 秒")]
    [InlineData("0 30 22-2 * * ?", "每天 22 点至次日 2 点的 30 分 0 秒")]
    public void RangeValue(string cron, string description)
    {
        _testOutputHelper.WriteLine("Cron 表达式范围数值示例");
        _testOutputHelper.WriteLine($"表达式 {cron}");
        _testOutputHelper.WriteLine(description);
        CronExpression cronExpression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
        PrintOccurrences(cronExpression);
        Assert.True(true);
    }

    /// <summary>
    ///     指定步长数值。
    /// </summary>
    /// <remarks>
    ///     所有的字段都支持指定步长数值。例如：
    ///         <list type="bullet">
    ///             <item><term> <c>0/3 * * * * ?</c> 表示每分钟的 0 秒开始，每 3 秒执行。 </term></item>
    ///             <item><term> <c>0 0/5 * * * ?</c> 表示每小时的 0 分开始，每 5 分执行。 </term></item>
    ///             <item><term> <c>0 0 0/2 * * ?</c> 表示每天的 0 点开始，每 2 小时执行。 </term></item>
    ///             <item><term> <c>0 0 9 1/3 * ?</c> 表示每月 1 日的 9 点开始，每 3 天执行。 </term></item>
    ///         </list>
    /// </remarks>
    [Theory]
    [InlineData("0/3 * * * * ?", "每分钟的 0 秒开始，每 3 秒")]
    [InlineData("0 0/5 * * * ?", "每小时的 0 分开始，每 5 分")]
    [InlineData("0 0 0/2 * * ?", "每天的 0 点开始，每 2 小时")]
    [InlineData("0 0 9 1/3 * ?", "每月 1 日的 9 点开始，每 3 天")]
    public void StepValue(string cron, string description)
    {
        _testOutputHelper.WriteLine("Cron 表达式步长数值示例");
        _testOutputHelper.WriteLine($"表达式 {cron}");
        _testOutputHelper.WriteLine(description);
        CronExpression cronExpression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
        PrintOccurrences(cronExpression);
        Assert.True(true);
    }

    /// <summary>
    ///     指定多个数值。
    /// </summary>
    /// <remarks>
    ///     所有的字段都支持指定多个数值。例如：
    ///     <list type="bullet">
    ///         <item><term> <c>0,30 * * * * ?</c> 表示每分钟的 0 秒和 30 秒执行。 </term></item>
    ///         <item><term> <c>0 0,30 * * * ?</c> 表示每小时的 0 分和 30 分执行。 </term></item>
    ///         <item><term> <c>0 0 0,12 * * ?</c> 表示每天的 0 点和 12 点执行。 </term></item>
    ///         <item><term> <c>0 0 9 1,10 * ?</c> 表示每月 1 日和 10 日的 9 点执行。 </term></item>
    ///         <item><term> <c>0 0 9 1 3-6,9-12 ?</c> 表示每年 3-6 月和 9-12 月的 1 日的 9 点执行。 </term></item>
    ///         <item><term> <c>0 0 9 ? * 2,4,6</c> 表示每周二、周四、周六的 9 点执行。 </term></item>
    ///     </list>
    /// </remarks>
    [Theory]
    [InlineData("0,30 * * * * ?", "每分钟的 0 秒和 30 秒")]
    [InlineData("0 0,30 * * * ?", "每小时的 0 分和 30 分")]
    [InlineData("0 0 0,12 * * ?", "每天的 0 点和 12 点")]
    [InlineData("0 0 9 1,10 * ?", "每月 1 日和 10 日的 9 点")]
    [InlineData("0 0 9 1 3-6,9-12 ?", "每年 3-6 月和 9-12 月的 1 日的 9 点")]
    [InlineData("0 0 9 ? * 2,4,6", "每周二、周四、周六的 9 点")]
    public void MultipleValue(string cron, string description)
    {
        _testOutputHelper.WriteLine("Cron 表达式多个数值示例");
        _testOutputHelper.WriteLine($"表达式 {cron}");
        _testOutputHelper.WriteLine(description);
        CronExpression cronExpression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
        PrintOccurrences(cronExpression);
        Assert.True(true);
    }

    /// <summary>
    ///     指定别名。
    /// </summary>
    /// <remarks>
    ///     部分字段支持指定别名。例如：
    ///     <list type="bullet">
    ///         <item><term> <c>0 9 1 JAN-MAR ?</c> 表示每年 1-3 月的 1 日的 9 点执行。 </term></item>
    ///         <item><term> <c>0 9 ? * MON-FRI</c> 表示每周一至周五的 9 点执行。 </term></item>
    ///     </list>
    ///     <para>
    ///         月份字段支持的别名有：JAN、FEB、MAR、APR、MAY、JUN、JUL、AUG、SEP、OCT、NOV、DEC
    ///     </para>
    ///     <para>
    ///         星期字段支持的别名有：SUN、MON、TUE、WED、THU、FRI、SAT
    ///     </para>
    /// </remarks>
    [Theory]
    [InlineData("0 9 1 JAN-MAR ?", "每年 1-3 月的 1 日的 9 点")]
    [InlineData("0 9 ? * MON-FRI", "每周一至周五的 9 点")]
    public void AliasNames(string cron, string description)
    {
        _testOutputHelper.WriteLine("Cron 表达式中的别名");
        _testOutputHelper.WriteLine($"表达式 {cron}");
        _testOutputHelper.WriteLine(description);
        CronExpression cronExpression = CronExpression.Parse(cron);
        PrintOccurrences(cronExpression);
        Assert.True(true);
    }

    /// <summary>
    ///     特殊字符。
    /// </summary>
    /// <remarks>
    ///     部分字段支持特殊字符。包括：
    ///     <list type="bullet">
    ///         <item><term> <c>L</c> 表示最后一个值。可出现在日期和星期字段。 </term></item>
    ///         <item><term> <c>W</c> 表示工作日。可出现在日期字段。 </term></item>
    ///         <item><term> <c>#</c> 表示第几个。可出现在星期字段。 </term></item>
    ///     </list>
    ///     例如：
    ///     <list type="bullet">
    ///         <item><term> <c>0 9 L * ?</c> 表示每月最后一天的 9 点执行。 </term></item>
    ///         <item><term> <c>0 9 L-2 * ?</c> 表示每月倒数第三天的 9 点执行。 </term></item>
    ///         <item><term> <c>0 9 3W * ?</c> 表示每月第三个工作日的 9 点执行。 </term></item>
    ///         <item><term> <c>0 9 LW * ?</c> 表示每月最后一个工作日的 9 点执行。 </term></item>
    ///         <item><term> <c>0 9 * * 6#2</c> 表示每第二个星期六的 9 点执行。 </term></item>
    ///         <item><term> <c>0 9 * 3 1#2</c> 表示每年 3 月的第二个星期一的 9 点执行。 </term></item>
    ///     </list>
    /// </remarks>
    [Theory]
    [InlineData("0 9 L * ?", "每月最后一天的 9 点")]
    [InlineData("0 9 L-2 * ?", "每月倒数第三天的 9 点")]
    [InlineData("0 9 3W * ?", "每月第三个工作日的 9 点")]
    [InlineData("0 9 LW * ?", "每月最后一个工作日的 9 点")]
    [InlineData("0 9 * * 6#2", "每第二个星期六的 9 点")]
    [InlineData("0 9 * 3 1#2", "每年 3 月的第二个星期一的 9 点")]
    public void SpecialChars(string cron, string description)
    {
        _testOutputHelper.WriteLine("Cron 表达式中的特殊字符");
        _testOutputHelper.WriteLine($"表达式 {cron}");
        _testOutputHelper.WriteLine(description);
        CronExpression cronExpression = CronExpression.Parse(cron);
        PrintOccurrences(cronExpression);
        Assert.True(true);
    }

    /// <summary>
    ///     同时指定日期和星期。
    /// </summary>
    /// <remarks>
    ///     在标准 Cron 表达式中，日期和星期是互斥的，因此不能同时指定。当其中一个字段指定了值，另一个字段必须使用 <c>?</c> 表示不指定。
    ///     但某些框架可能在此基础上扩展了同时支持指定这两个字段的功能。Cronos 框架支持同时指定日期和星期。
    ///     此时，日期和星期的值都会被考虑，即同时满足日期和星期的值时，才会触发任务。
    ///     例如：<c>0 8 9 * 1</c> 表示每个为周一的 9 日的 8 点执行。
    ///     在某些其它框架中，可能会有不同的解释，例如：Vixie Cron 会将该表达式解析为每个星期一及每个月的 9 日的 8 点都执行。
    /// </remarks>
    [Theory]
    [InlineData("0 8 9 * 1", "每个为周一的 9 日的 8 点")]
    public void SpecifyDayAndWeek(string cron, string description)
    {
        _testOutputHelper.WriteLine("Cronos 框架中同时指定日期和星期");
        _testOutputHelper.WriteLine($"表达式 {cron}");
        _testOutputHelper.WriteLine(description);
        CronExpression cronExpression = CronExpression.Parse(cron);
        PrintOccurrences(cronExpression);
        Assert.True(true);
    }

    /// <summary>
    ///     预定义表达式。
    /// </summary>
    /// <remarks>
    ///     Cronos 框架还支持一些预定义的表达式，例如：
    ///     <list type="bullet">
    ///         <item><term> <c>@every_second</c> 表示每秒。 </term></item>
    ///         <item><term> <c>@every_minute</c> 表示每分钟。 </term></item>
    ///         <item><term> <c>@hourly</c> 表示每小时。 </term></item>
    ///         <item><term> <c>@daily</c> 与 <c>@midnight</c> 表示每天。 </term></item>
    ///         <item><term> <c>@weekly</c> 表示每周。 </term></item>
    ///         <item><term> <c>@monthly</c> 表示每月。 </term></item>
    ///         <item><term> <c>@yearly</c> 与 <c>@annually</c> 表示每年。 </term></item>
    ///     </list>
    /// </remarks>
    [Theory]
    [InlineData("@every_second", "每秒")]
    [InlineData("@every_minute", "每分钟")]
    [InlineData("@hourly", "每小时")]
    [InlineData("@daily", "每天")]
    [InlineData("@weekly", "每周")]
    [InlineData("@monthly", "每月")]
    [InlineData("@yearly", "每年")]
    public void Macro(string cron, string description)
    {
        _testOutputHelper.WriteLine("Cronos 框架中的预定义表达式");
        _testOutputHelper.WriteLine($"表达式 {cron}");
        _testOutputHelper.WriteLine(description);
        CronExpression cronExpression = CronExpression.Parse(cron);
        _testOutputHelper.WriteLine($"等效表达式 {cronExpression}");
        PrintOccurrences(cronExpression);
        Assert.True(true);
    }

    /// <summary>
    ///     获取指定时间范围内的 Cron 表达式触发时间。
    /// </summary>
    /// <param name="cronExpression"> Cron 表达式 </param>
    /// <param name="count"> 获取的触发时间数量 </param>
    /// <returns> 指定时间范围内的 Cron 表达式触发时间 </returns>
    /// <remarks>
    ///     调用 <see cref="CronExpression.GetOccurrences(DateTime,DateTime,TimeZoneInfo,bool,bool)"/>
    ///     方法可以获取指定时间范围内的 Cron 表达式触发时间。需要注意，该方法所传入的 <see cref="DateTime"/>
    ///     其 <see cref="DateTime.Kind"/> 属性必须为 <see cref="DateTimeKind.Utc"/>，否则会抛出异常。
    ///     因此如果要获取本地时间范围内的触发时间，可以先将本地时间转换为 UTC 时间，再调用该方法，再将返回值转换为本地时间。也可以使用
    ///     <see cref="CronExpression.GetOccurrences(DateTimeOffset,DateTimeOffset,TimeZoneInfo,bool,bool)"/>
    ///     重载，<see cref="DateTimeOffset"/> 中包含了时区信息，因此不会产生歧义。
    /// </remarks>
    private static IEnumerable<DateTimeOffset> GetOccurrences(CronExpression cronExpression, int count = 10) =>
        cronExpression.GetOccurrences(DateTimeOffset.Now, DateTimeOffset.MaxValue, TimeZoneInfo.Local)
            .Take(count)
            .Select(x => x.ToLocalTime());

    /// <summary>
    ///     将 <see cref="DateTimeOffset"/> 转换为字符串。
    /// </summary>
    /// <param name="dateTime"> <see cref="DateTimeOffset"/> 实例 </param>
    /// <returns> <see cref="DateTimeOffset"/> 的字符串表示 </returns>
    private static string DateTimeToString(DateTimeOffset dateTime) =>
        dateTime.ToString("yyyy-MM-dd ddd HH:mm:ss zzz");

    /// <summary>
    ///     打印指定时间范围内的 Cron 表达式触发时间。
    /// </summary>
    /// <param name="cronExpression"> Cron 表达式 </param>
    /// <param name="count"> 获取的触发时间数量 </param>
    private void PrintOccurrences(CronExpression cronExpression, int count = 10)
    {
        IEnumerable<string> occurrences = GetOccurrences(cronExpression, count).Select(DateTimeToString);
        foreach (string occurrence in occurrences)
            _testOutputHelper.WriteLine($"|> {occurrence}");
    }

    /*
     * Cron 表达式在线工具：https://cron.qqe2.com/
     */
}
