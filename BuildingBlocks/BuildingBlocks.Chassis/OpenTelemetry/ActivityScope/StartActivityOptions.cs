using System.Diagnostics;

namespace BuildingBlocks.Chassis.OpenTelemetry.ActivityScope;

public sealed record StartActivityOptions
{
    public const ActivityKind Kind = ActivityKind.Internal;

    public Dictionary<string, object?> Tags { get; set; } = [];

    public string? ParentId { get; set; }

    public ActivityContext? Parent { get; set; }
}
