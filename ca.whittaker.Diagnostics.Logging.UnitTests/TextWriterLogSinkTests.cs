using NUnit.Framework;

namespace ca.whittaker.Diagnostics.Logging.UnitTests;

public class TextWriterLogSinkTests
{
    [Test]
    public void Write_ContainsCategoryMessageAndCompleteException()
    {
        using var writer = new StringWriter();
        var sink = new TextWriterLogSink(writer);
        var exception = new InvalidOperationException("outer", new Exception("inner"));

        sink.Write("sample", "message", exception);

        Assert.That(writer.ToString(), Does.StartWith("[sample] message" + Environment.NewLine));
        Assert.That(writer.ToString(), Does.Contain(exception.ToString()));
        writer.Write("still owned by caller");
        Assert.That(writer.ToString(), Does.EndWith("still owned by caller"));
    }

    [Test]
    public void DisposedDestination_FailsVisibly()
    {
        var writer = new StringWriter();
        writer.Dispose();
        var sink = new TextWriterLogSink(writer);

        Assert.Throws<ObjectDisposedException>(() => sink.Write("sample", "message", null));
    }

    [Test]
    public void NullDestination_IsRejected() =>
        Assert.Throws<ArgumentNullException>(() => new TextWriterLogSink(null!));

    [Test]
    public void SharedSink_SerializesCompleteEntries()
    {
        using var writer = new StringWriter();
        var sink = new TextWriterLogSink(writer);

        Parallel.For(0, 100, i => sink.Write("parallel", i.ToString(), null));

        var lines = writer.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.That(lines, Has.Length.EqualTo(100));
        Assert.That(lines.Distinct().Count(), Is.EqualTo(100));
        Assert.That(lines.All(x => x.StartsWith("[parallel] ")), Is.True);
    }
}
