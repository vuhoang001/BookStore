using System.Diagnostics.CodeAnalysis;
using BuildingBlocks.Chassis.Exceptions;

namespace BuildingBlocks.Chassis.Utils.Guards;

public static class GuardAgainstNotFoundException
{
    /// <summary>
    /// Validates that the provided value is not null.
    /// If null, throws a NotFoundException for the specified id.
    /// </summary>
    public static void NotFound<T>(
        this Guard guard,
        [NotNull] T? value,
        string id)
    {
        if (value is not null)
            return;

        throw NotFoundException.For<T>(id);
    }

    /// <summary>
    /// Validates that the provided value is not null.
    /// If null, throws a NotFoundException for the specified id.
    /// </summary>
    public static void NotFound<T>(
        this Guard guard,
        [NotNull] T? value,
        Guid id)
    {
        if (value is not null)
            return;

        throw NotFoundException.For<T>(id);
    } 
}
