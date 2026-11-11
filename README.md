# ca.whittaker.Diagnostics.Logging

Small, dependency-free diagnostic logging for .NET applications. Each logger has an explicit category, its own enablement setting, and a destination chosen by the caller. Useful for small tools, test harnesses, and adapters. It does not replace Microsoft.Extensions.Logging in applications that need levels, scopes, providers, filtering, or structured events.

## Requirements and installation

Targets .NET 9 and .NET 10, with no runtime NuGet dependencies. Building the repository requires the .NET 10 SDK and .NET 9 runtime to execute both test targets.

```powershell
dotnet add package ca.whittaker.Diagnostics.Logging --version 1.0.0-preview.1
```

This is a prerelease. Published versions are listed on [NuGet](https://www.nuget.org/packages/ca.whittaker.Diagnostics.Logging).

## Quick start

```csharp
using ca.whittaker.Diagnostics.Logging;

var logger = new Logger(
    category: "Example.Import",
    sink: new TextWriterLogSink(Console.Out),
    enabled: true);

logger.Log("Import started.");
logger.Log("Synthetic failure example.",
    new InvalidOperationException("Example only."));
```

Expected output starts with `[Example.Import] Import started.`. The next message includes the exception type and message. These examples use synthetic data.

## Configuration and defaults

| Member | Default / behaviour |
|---|---|
| Constructor category | Required, nonblank; passed exactly as supplied |
| Constructor sink | Required, caller-owned `ILogSink` |
| Constructor enabled | `false` |
| `IsEnabled` | Mutable per instance; affects subsequent calls |
| `Category` | Read-only after construction |
| `Log(message, exception)` | Exception defaults to null; disabled calls skip destination |
| `TextWriterLogSink` | Serializes writes through this sink; does not flush or dispose writer |

To capture output in memory:

```csharp
using var output = new StringWriter();
var logger = new Logger("Example.Capture", new TextWriterLogSink(output));
logger.Log("Not written.");
logger.IsEnabled = true;
logger.Log("Written.");
Console.WriteLine(output.ToString());
```

For console output, pass `Console.Out` or `Console.Error`. For a file, create and dispose a `StreamWriter` in your application and supply it to `TextWriterLogSink`. Decide flushing, rotation, access permissions, and retention in your application. Implement `ILogSink.Write` to forward to an existing logging provider.

Enablement works identically in Debug and Release. There is no `#if DEBUG` switch and no automatic debugger destination. Applications that require build-specific enablement must select it explicitly.

## Public API and failures

- `Logger(string category, ILogSink sink, bool enabled = false)` validates inputs.
- `Logger.Log(string message, Exception? exception = null)` validates a non-null message even when disabled, then checks instance enablement, then calls the sink.
- `ILogSink.Write(string category, string message, Exception? exception)` receives the original exception unchanged.
- `TextWriterLogSink(TextWriter writer)` writes `[category] message`, then `exception.ToString()` when provided.

Null required arguments throw `ArgumentNullException`; blank categories throw `ArgumentException`. Destination failures propagate without retry. Custom sinks define their own delivery guarantees and concurrency. The logger does not dispose sinks. Full exception details can include stack traces, paths, and inner exceptions.

## Limitations and troubleshooting

If output is missing, check `IsEnabled`, writer buffering, and destination lifetime. Use an explicitly enabled text writer for Release diagnostics. If a write fails, investigate the destination exception and permissions. Output may have been partially written before failure; do not assume retries are duplicate-free.

No automatic redaction, severity levels, retention, scope propagation, or asynchronous queue is provided. Do not log credentials, personal data, or secrets. Categories and messages are written verbatim, so validate untrusted multiline text before logging. Multiple logger instances can share one sink. Independent sinks sharing the same writer require external coordination.

## Compatibility and migration

This 1.0 prerelease changes the older static helper API. Replace global `EnableLogging` calls with an owned `Logger` instance. Supply a category rather than relying on stack inspection. Supply a destination instead of relying on `Debug.WriteLine`. Convert objects explicitly to strings. Release builds now write when enabled, so choose enablement deliberately.

See [migration guidance](https://github.com/bwhittaker69/ca.whittaker.Diagnostics.Logging/blob/main/docs/migration.md) and the [runnable sample](https://github.com/bwhittaker69/ca.whittaker.Diagnostics.Logging/tree/main/samples/Logging.Sample).

## Documentation and project links

- [Build, test, and contribute](https://github.com/bwhittaker69/ca.whittaker.Diagnostics.Logging/blob/main/CONTRIBUTING.md)
- [Source](https://github.com/bwhittaker69/ca.whittaker.Diagnostics.Logging)
- [Releases](https://github.com/bwhittaker69/ca.whittaker.Diagnostics.Logging/releases)
- [Support](https://github.com/bwhittaker69/ca.whittaker.Diagnostics.Logging/blob/main/SUPPORT.md)
- [Security reporting](https://github.com/bwhittaker69/ca.whittaker.Diagnostics.Logging/blob/main/SECURITY.md)
- [GPL version 3 licence](https://github.com/bwhittaker69/ca.whittaker.Diagnostics.Logging/blob/main/LICENSE.txt)

Copyright (c) 2026 Brett Whittaker.
