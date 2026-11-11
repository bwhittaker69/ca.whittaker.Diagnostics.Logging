using ca.whittaker.Diagnostics.Logging;

var logger = new Logger("Example.Import", new TextWriterLogSink(Console.Out), enabled: true);
logger.Log("Import started.");
logger.Log("Synthetic failure example.", new InvalidOperationException("Example only."));

using var writer = new StringWriter();
var captured = new Logger("Example.Capture", new TextWriterLogSink(writer));
captured.Log("Hidden by default.");
if (writer.ToString().Length != 0) throw new Exception("Disabled logger wrote output.");
captured.IsEnabled = true;
captured.Log("Captured.");
if (!writer.ToString().Contains("[Example.Capture] Captured.")) throw new Exception("Capture failed.");
Console.WriteLine("Sample checks passed.");
