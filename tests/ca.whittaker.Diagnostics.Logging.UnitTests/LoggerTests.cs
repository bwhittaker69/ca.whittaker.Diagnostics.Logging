using NUnit.Framework;

namespace ca.whittaker.Diagnostics.Logging.UnitTests;

public class LoggerTests
{
    [Test]
    public void DisabledByDefault_DoesNotCallSink()
    {
        var sink = new RecordingSink();
        var logger = new Logger("sample", sink);

        logger.Log("hidden", new Exception("hidden exception"));

        Assert.That(sink.Calls, Is.Empty);
    }

    [Test]
    public void Enabled_ForwardsCategoryMessageAndOriginalException()
    {
        var sink = new RecordingSink();
        var exception = new InvalidOperationException("synthetic", new Exception("inner"));
        var logger = new Logger("sample.category", sink, enabled: true);

        logger.Log("message", exception);

        Assert.That(sink.Calls, Has.Count.EqualTo(1));
        Assert.That(sink.Calls[0], Is.EqualTo(("sample.category", "message", exception)));
        Assert.That(sink.Calls[0].Exception, Is.SameAs(exception));
    }

    [Test]
    public void Enablement_IsIndependentAndCanBeChanged()
    {
        var sink = new RecordingSink();
        var first = new Logger("first", sink);
        var second = new Logger("second", sink, enabled: true);

        first.IsEnabled = true;
        second.IsEnabled = false;
        first.Log("written");
        second.Log("hidden");

        Assert.That(sink.Calls.Select(x => x.Category), Is.EqualTo(new[] { "first" }));
    }

    [TestCase("")]
    [TestCase(" ")]
    public void BlankCategory_IsRejected(string category)
    {
        Assert.Throws<ArgumentException>(() => new Logger(category, new RecordingSink()));
    }

    [Test]
    public void NullInputs_AreRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new Logger(null!, new RecordingSink()));
        Assert.Throws<ArgumentNullException>(() => new Logger("sample", null!));
        Assert.Throws<ArgumentNullException>(() => new Logger("sample", new RecordingSink()).Log(null!));
    }

    [Test]
    public void SinkFailure_PropagatesOriginalException()
    {
        var failure = new IOException("synthetic destination failure");
        var logger = new Logger("sample", new ThrowingSink(failure), enabled: true);

        var actual = Assert.Throws<IOException>(() => logger.Log("message"));

        Assert.That(actual, Is.SameAs(failure));
    }

    private sealed class RecordingSink : ILogSink
    {
        public List<(string Category, string Message, Exception? Exception)> Calls { get; } = new();
        public void Write(string category, string message, Exception? exception) =>
            Calls.Add((category, message, exception));
    }

    private sealed class ThrowingSink(Exception failure) : ILogSink
    {
        public void Write(string category, string message, Exception? exception) => throw failure;
    }
}
