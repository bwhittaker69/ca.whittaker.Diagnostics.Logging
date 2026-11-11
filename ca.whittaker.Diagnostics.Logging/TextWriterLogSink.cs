namespace ca.whittaker.Diagnostics.Logging;

/// <summary>Writes categorized lines and complete exception details to a caller-owned text writer.</summary>
/// <remarks>Use Console.Out, StringWriter, or a file StreamWriter. This sink neither flushes nor disposes the writer.</remarks>
public sealed class TextWriterLogSink : ILogSink
{
    private readonly TextWriter _writer;
    private readonly object _gate = new();

    /// <summary>Creates a text destination.</summary>
    /// <param name="writer">Writer whose lifetime and flushing remain the caller's responsibility.</param>
    /// <exception cref="ArgumentNullException">The writer is null.</exception>
    public TextWriterLogSink(TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(writer);
        _writer = writer;
    }

    /// <summary>Writes a categorized line followed by the exception, when supplied.</summary>
    /// <param name="category">Non-null category supplied by the logger.</param>
    /// <param name="message">Non-null message supplied by the logger.</param>
    /// <param name="exception">Exception rendered with ToString, including stack and inner exceptions.</param>
    /// <exception cref="ArgumentNullException">The category or message is null.</exception>
    /// <remarks>Writes through this sink are serialized; separate sinks sharing a writer must coordinate themselves.
    /// Writer errors, including a disposed writer or failed I/O, propagate without retry.</remarks>
    public void Write(string category, string message, Exception? exception)
    {
        ArgumentNullException.ThrowIfNull(category);
        ArgumentNullException.ThrowIfNull(message);
        lock (_gate)
        {
            _writer.WriteLine($"[{category}] {message}");
            if (exception is not null)
            {
                _writer.WriteLine(exception.ToString());
            }
        }
    }
}
