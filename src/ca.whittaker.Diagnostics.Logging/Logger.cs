namespace ca.whittaker.Diagnostics.Logging;

/// <summary>Writes diagnostic messages to an explicit destination and category.</summary>
/// <remarks>Enablement belongs to this instance. Messages are not automatically redacted.</remarks>
public sealed class Logger
{
    private readonly ILogSink _sink;

    /// <summary>Creates a logger, disabled unless explicitly enabled.</summary>
    /// <param name="category">Nonblank category written with each message.</param>
    /// <param name="sink">Destination, owned by the caller.</param>
    /// <param name="enabled">Whether this instance initially writes messages. Defaults to false.</param>
    /// <exception cref="ArgumentException">The category is blank.</exception>
    /// <exception cref="ArgumentNullException">The category or sink is null.</exception>
    public Logger(string category, ILogSink sink, bool enabled = false)
    {
        ArgumentNullException.ThrowIfNull(category);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentNullException.ThrowIfNull(sink);
        Category = category;
        _sink = sink;
        IsEnabled = enabled;
    }

    /// <summary>Gets the explicit category shared by this instance's messages.</summary>
    public string Category { get; }

    /// <summary>Gets or sets whether this instance writes messages. Defaults to false.</summary>
    /// <remarks>Other logger instances are unaffected. Enabled logging works in Debug and Release.</remarks>
    public bool IsEnabled { get; set; }

    // #############################################
    // DIAGNOSTIC MESSAGE PROCESS
    // #############################################

    /// <summary>Writes a message and optional original exception when enabled.</summary>
    /// <param name="message">Message to write without automatic redaction.</param>
    /// <param name="exception">Optional exception passed unchanged to the destination.</param>
    /// <exception cref="ArgumentNullException">The message is null, even when disabled.</exception>
    /// <remarks>Disabled calls do not invoke the sink. Sink failures propagate to the caller.</remarks>
    public void Log(string message, Exception? exception = null)
    {
        // -----------------------------------------
        // STEP 1: VALIDATE THE MESSAGE
        // -----------------------------------------
        ArgumentNullException.ThrowIfNull(message);

        // -----------------------------------------
        // STEP 2: CHECK THIS INSTANCE'S ENABLEMENT
        // -----------------------------------------
        if (!IsEnabled)
        {
            return;
        }

        // -----------------------------------------
        // STEP 3: WRITE TO THE SELECTED DESTINATION
        // -----------------------------------------
        _sink.Write(Category, message, exception);
    }
}
