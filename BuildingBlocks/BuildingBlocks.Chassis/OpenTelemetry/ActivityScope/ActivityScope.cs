using System.Diagnostics;

namespace BuildingBlocks.Chassis.OpenTelemetry.ActivityScope;

public class ActivityScope : IActivityScope
{
    public static readonly IActivityScope Instance = new ActivityScope();


    public Activity? Start(string name, StartActivityOptions options)
    {
        return options.Parent.HasValue
            ? ActivitySourceProvider.Instance.CreateActivity(
                $"{ActivitySourceProvider.DefaultSourceName}.{name}",
                StartActivityOptions.Kind,
                options.Parent.Value,
                idFormat: ActivityIdFormat.W3C,
                tags: options.Tags)?.Start()
            : ActivitySourceProvider
                .Instance.CreateActivity(
                    $"{ActivitySourceProvider.DefaultSourceName}.{name}",
                    StartActivityOptions.Kind,
                    options.ParentId ?? Activity.Current?.ParentId,
                    idFormat: ActivityIdFormat.W3C,
                    tags: options.Tags
                )
                ?.Start();
    }

    public async Task Run(
        string name,
        Func<Activity?, CancellationToken, Task> run,
        StartActivityOptions options,
        CancellationToken ct
    )
    {
        using var activity = Start(name, options) ?? Activity.Current;

        try
        {
            await run(activity, ct).ConfigureAwait(false);

            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.SetTag("exception.type", ex.GetType().FullName);
            activity?.SetTag("exception.message", ex.Message);
            activity?.SetTag("exception.stacktrace", ex.StackTrace);
            throw;
        }
    }

    public async Task<TResult> Run<TResult>(
        string name,
        Func<Activity?, CancellationToken, Task<TResult>> run,
        StartActivityOptions options,
        CancellationToken ct
    )
    {
        using var activity = Start(name, options) ?? Activity.Current;

        try
        {
            var result = await run(activity, ct).ConfigureAwait(false);

            activity?.SetStatus(ActivityStatusCode.Ok);

            return result;
        }
        catch (Exception ex)
        {
            activity?.SetTag("exception.type", ex.GetType().FullName);
            activity?.SetTag("exception.message", ex.Message);
            activity?.SetTag("exception.stacktrace", ex.StackTrace);
            activity?.SetStatus(ActivityStatusCode.Error);
            throw;
        }
    }
}
