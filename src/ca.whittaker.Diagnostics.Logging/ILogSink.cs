namespace ca.whittaker.Diagnostics.Logging;

/// <summary>Represents an explicitly selected diagnostic destination.</summary>
/// <remarks>Implementations define concurrency and delivery behaviour. Failures are not swallowed by Logger.</remarks>
public interface ILogSink
{
    /// <summary>Writes one diagnostic message.</summary>
    /// <param name="category">The logger's explicit category.</param>
    /// <param name="message">The validated, non-null message.</param>
    /// <param name="exception">Original exception, or null when absent.</param>
    /// <remarks>The caller must avoid sensitive values. This interface provides no redaction.</remarks>
    void Write(string category, string message, Exception? exception);
}
