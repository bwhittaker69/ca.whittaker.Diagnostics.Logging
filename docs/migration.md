# Migration and integration

The package targets .NET 9 and .NET 10. Use the matching SDK/runtime for your application. Each package version is independent of the publisher's other packages.

The old static helper used a global switch, stack-trace category discovery, and Debug-only output. The new instance API is enabled explicitly in both build configurations. Construct and own one logger per required category, or inject one from your application's composition root.

A sink is an external capability. The logger's execution order stays visible: validate, check enablement, write. A private application can implement ILogSink to bridge to Microsoft.Extensions.Logging. Keep authentication rules and provider setup in that application.

Exception arguments are forwarded unchanged. TextWriterLogSink uses ToString to preserve inner exception and stack information. Nothing is automatically redacted. Handle destination failures at the calling process boundary if continued operation is required; the package intentionally does not hide failed writes.

Logger does not own destination resources. Dispose and flush file writers in your application. Writes using the same TextWriterLogSink are serialized. Do not use separate sinks on the same non-thread-safe writer without coordination.
