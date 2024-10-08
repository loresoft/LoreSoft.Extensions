using System;

using BenchmarkDotNet.Attributes;

using LoreSoft.Extensions.Performance.Text;
using LoreSoft.Extensions.Text;

namespace LoreSoft.Extensions.Performance;

[BenchmarkCategory("Format")]
public class FormatPerformance
{
    private const string PostTemplate = "<h1>{Title}</h1><div>Author: {Author}</div><div>Updated: {Updated:d}</div>";
    private Post _post;

    [GlobalSetup]
    public void Setup()
    {
        _post = new Post("Bob", new DateTime(2024, 8, 1, 12, 30, 0), "This is a test post");
    }

    [Benchmark(Description = "Henri Format")]
    public string HenriFormatTest()
    {
        return HenriFormat.Format(PostTemplate, _post);
    }

    [Benchmark(Description = "Marisic Format")]
    public string MarisicFormatTest()
    {
        return MarisicFormat.Format(PostTemplate, _post);
    }

    [Benchmark(Description = "Name Formatter")]
    public string CustomFormatTest()
    {
        return NameFormatter.FormatName(PostTemplate, _post);
    }

}
