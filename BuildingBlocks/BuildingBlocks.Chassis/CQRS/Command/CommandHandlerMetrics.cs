using System.Diagnostics.Metrics;

namespace BuildingBlocks.Chassis.CQRS.Command;

public class CommandHandlerMetrics : IDisposable
{
    private readonly UpDownCounter<long> _activeEventHandlingCounter;
    private readonly Histogram<double> _eventHandlingDuration;
    private readonly Meter _meter;
    private readonly TimeProvider _timeProvider;
    private readonly Counter<long> _totalCommandsNumber;

    public CommandHandlerMetrics(IMeterFactory meterFactory, TimeProvider timeProvider)
    {
        // _timeProvider = timeProvider;
        // _meter        = meterFactory.Create(ActivitySourceProvider.DefaultSourceName);
        //
        // _totalCommandsNumber = _meter.CreateCounter<long>(
        //     TelemetryTags.Commands.TotalCommandsNumber,
        //     "{command}",
        //     "Total number of commands send to command handlers"
        // );
        //
        // _activeEventHandlingCounter = _meter.CreateUpDownCounter<long>(
        //     TelemetryTags.Commands.ActiveCommandsNumber,
        //     "{command}",
        //     "Number of commands currently being handled"
        // );
        //
        // _eventHandlingDuration = _meter.CreateHistogram<double>(
        //     TelemetryTags.Commands.CommandHandlingDuration, "s",
        //     "Measures the duration of inbound commands"
        // );
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}
