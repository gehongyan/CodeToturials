using System.Text.RegularExpressions;
using Xunit.Abstractions;

// ReSharper disable InconsistentNaming

namespace RegexTutorial;

public partial class RegexTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public RegexTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void IsMatch()
    {
        bool isMatch = Regex.IsMatch("123", @"\d+", RegexOptions.Compiled);
        Assert.True(isMatch);
        isMatch = Regex.IsMatch("abc", @"\d+", RegexOptions.Compiled);
        Assert.False(isMatch);

        isMatch = Regex.IsMatch("abc", @"ABC", RegexOptions.Compiled);
        Assert.False(isMatch);
        isMatch = Regex.IsMatch("abc", @"ABC", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Assert.True(isMatch);
    }

    [Fact]
    public void Match()
    {
        Match match = Regex.Match("abcdefg123456", @"\d+", RegexOptions.Compiled);
        Assert.True(match.Success);
        Assert.Equal("123456", match.Value);
        Assert.Equal(7, match.Index);
        Assert.Equal(6, match.Length);
    }

    [Fact]
    public void Matches()
    {
        const string input = """
            1、THE WIND FOREST（风之甬道）---选自电影《龙猫》久石让/曲
            2、哈尔的移动城堡主题曲---选自电影《哈尔的移动城堡》木村弓/久石让/曲
            3、崖の上のポニョ---选自电影《悬崖上的金鱼姬》久石让/曲
            4、The Rain---选自电影《菊次郎的夏天》久石让/曲
            5、Princess Mononoke---选自电影《幽灵公主》久石让/曲
            6、Ashitaka and San---选自电影《幽灵公主》久石让/曲
            7、风之谷主题曲---选自电影《风之谷》久石让/曲
            8、HANA-BI ---选自电影《花火》久石让/曲
            9、君だけを见ていた---选自《月光星愿演奏会》久石让/曲
            10、いつも何度でも---选自电影《千与千寻》久石让/曲
            11、千尋のワルツ---选自电影《千与千寻》久石让/曲
            12、总是一次又一次---选自电影《千与千寻》久石让/曲
            13、幻化成风---选自电影《猫之报恩》久石让/曲
            14、となりのトトロ---选自电影《龙猫》久石让/曲
            15、天空之城主题曲---选自电影《天空之城》久石让/曲
            16、SUMMER---选自电影《菊次郎的夏天》久石让/曲
            """;
        MatchCollection matches = Regex.Matches(
            input,
            @"\d+、(.+?)---选自(电影)?《(.+?)》(.*?)/曲",
            RegexOptions.Multiline | RegexOptions.Compiled);
        Assert.Equal(16, matches.Count);
        _testOutputHelper.WriteLine("曲名");
        foreach (Match match in matches)
            _testOutputHelper.WriteLine(match.Groups[1].Value);

        _testOutputHelper.WriteLine(string.Empty);

        matches = Regex.Matches(
            input,
            @"\d+、(?<title>.+?)---选自(电影)?《(?<from>.+?)》(?<composer>.*?)/曲",
            RegexOptions.Multiline | RegexOptions.Compiled);
        Assert.Equal(16, matches.Count);
        _testOutputHelper.WriteLine("电影名");
        foreach (Match match in matches)
            _testOutputHelper.WriteLine(match.Groups["from"].Value);
    }

    [Fact]
    public void Replace()
    {
        const string input = """
            1、THE WIND FOREST（风之甬道）---选自电影《龙猫》久石让/曲
            2、哈尔的移动城堡主题曲---选自电影《哈尔的移动城堡》木村弓/久石让/曲
            3、崖の上のポニョ---选自电影《悬崖上的金鱼姬》久石让/曲
            4、The Rain---选自电影《菊次郎的夏天》久石让/曲
            5、Princess Mononoke---选自电影《幽灵公主》久石让/曲
            6、Ashitaka and San---选自电影《幽灵公主》久石让/曲
            7、风之谷主题曲---选自电影《风之谷》久石让/曲
            8、HANA-BI ---选自电影《花火》久石让/曲
            9、君だけを见ていた---选自《月光星愿演奏会》久石让/曲
            10、いつも何度でも---选自电影《千与千寻》久石让/曲
            11、千尋のワルツ---选自电影《千与千寻》久石让/曲
            12、总是一次又一次---选自电影《千与千寻》久石让/曲
            13、幻化成风---选自电影《猫之报恩》久石让/曲
            14、となりのトトロ---选自电影《龙猫》久石让/曲
            15、天空之城主题曲---选自电影《天空之城》久石让/曲
            16、SUMMER---选自电影《菊次郎的夏天》久石让/曲
            """;

        // 使用编号组
        string result1 = Regex.Replace(
            input,
            @"\d+、(.+?)---选自(电影)?《(.+?)》(.*?)/曲",
            @"《$1》（$3）——$4",
            RegexOptions.Compiled);
        _testOutputHelper.WriteLine(result1);
        Assert.NotEqual(input, result1);

        _testOutputHelper.WriteLine(string.Empty);

        // 使用命名组
        string result2 = Regex.Replace(
            input,
            @"\d+、(?<title>.+?)---选自(电影)?《(?<from>.+?)》(?<composer>.*?)/曲",
            @"《${title}》（${from}）——${composer}",
            RegexOptions.Compiled);
        _testOutputHelper.WriteLine(result2);
        Assert.NotEqual(input, result2);

        Assert.Equal(result1, result2);

        // 使用 MatchEvaluator
        string result3 = Regex.Replace(
            input,
            @"\d+、(?<title>.+?)---选自(电影)?《(?<from>.+?)》(?<composer>.*?)/曲",
            match => $"《{match.Groups["title"].Value}》（{match.Groups["from"].Value}）——{match.Groups["composer"].Value}",
            RegexOptions.Compiled);
        _testOutputHelper.WriteLine(result3);
        Assert.NotEqual(input, result3);

        Assert.Equal(result1, result3);
    }

    [Fact]
    public void Split()
    {
        string[] splitResults = Regex.Split(
            "1Alpha 2Bravo 3Charlie 4Delta 5Echo",
            @"\s?\d", RegexOptions.Compiled);
        Assert.Equal(6, splitResults.Length);
        foreach (string result in splitResults)
            _testOutputHelper.WriteLine(result);
    }

    [Fact]
    public void RegexSourceGenerator()
    {
        bool isMatch = TagsRegex().IsMatch("<p>text</p>");
        Assert.True(isMatch);
    }

    [GeneratedRegex(@"<(.*?)>(.+?)</\1>", RegexOptions.Compiled)]
    private static partial Regex TagsRegex();
}
