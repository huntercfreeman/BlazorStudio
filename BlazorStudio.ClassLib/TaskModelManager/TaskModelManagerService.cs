using System.Collections.Immutable;
using BlazorStudio.ClassLib.Errors;

namespace BlazorStudio.ClassLib.TaskModelManager;

public static class TaskModelManagerService
{
    private static readonly Dictionary<TaskModelKey, ITaskModel> ActiveTaskModelsDictionary = new();
    private static readonly Dictionary<TaskModelKey, ITaskModel> FailedTaskModelsDictionary = new();
    private static readonly Dictionary<TaskModelKey, ITaskModel> SuccessfulTaskModelsDictionary = new();
    private static readonly Dictionary<TaskModelKey, ITaskModel> SeeminglyUnresponsiveTaskModelsDictionary = new();

    private static readonly SemaphoreSlim ActiveTaskModelsDictionarySemaphoreSlim = new(1, 1);
    private static readonly SemaphoreSlim FailedTaskModelsDictionarySemaphoreSlim = new(1, 1);
    private static readonly SemaphoreSlim SuccessfulTaskModelsDictionarySemaphoreSlim = new(1, 1);
    private static readonly SemaphoreSlim SeeminglyUnresponsiveTaskModelsDictionarySemaphoreSlim = new(1, 1);

    public static ImmutableArray<ITaskModel> ActiveTaskModels { get; private set; } = ImmutableArray<ITaskModel>.Empty;
    public static ImmutableArray<ITaskModel> FailedTaskModels { get; private set; } = ImmutableArray<ITaskModel>.Empty;
    public static ImmutableArray<ITaskModel> SuccessfulTaskModels { get; private set; } =
        ImmutableArray<ITaskModel>.Empty;
    public static ImmutableArray<ITaskModel> SeeminglyUnresponsiveTaskModels { get; private set; } =
        ImmutableArray<ITaskModel>.Empty;

    public static event Action? OnTasksStateHasChangedEventHandler;
    public static event Action? OnTaskSeemsUnresponsiveEventHandler;

    public static async Task<ITaskModel> EnqueueTaskModelAsync(Func<CancellationToken, Task> taskFunc, string taskTitle,
        bool isBackgroundTask, TimeSpan lifetimeUntilSeemsUnresponsive,
        Func<Exception, RichErrorModel?>? richErrorModelOverrideFunc = null)
    {
        return await FireAndForgetAsync(taskFunc, taskTitle, isBackgroundTask, lifetimeUntilSeemsUnresponsive,
            richErrorModelOverrideFunc);
    }

    private static async Task<ITaskModel> FireAndForgetAsync(Func<CancellationToken, Task> taskFunc, string taskTitle,
        bool isBackgroundTask, TimeSpan lifetimeUntilSeemsUnresponsive,
        Func<Exception, RichErrorModel?>? richErrorModelOverrideFunc = null)
    {
        CancellationTokenSource cancellationTokenSource = new();

        var cancellationToken = cancellationTokenSource.Token;

        var taskModelKey = new TaskModelKey(Guid.NewGuid(), taskTitle);

        var taskModel = new TaskModel(taskModelKey,
            lifetimeUntilSeemsUnresponsive,
            cancellationTokenSource,
            isBackgroundTask
        );

        taskModel.Task = Task.Run(async () =>
        {
            try
            {
                var unresponsiveTask = Task.Delay(taskModel.LifetimeUntilSeemsUnresponsive,
                    cancellationToken);

                var task = taskFunc(cancellationToken);

                await Task.WhenAny(new[]
                {
                    task,
                    unresponsiveTask,
                });

                if (!task.IsCompleted)
                {
                    await AddToSeeminglyUnresponsiveTaskModelsDictionary(taskModel.Key, taskModel);

                    await RemoveFromActiveTaskModelsDictionary(taskModel.Key);

                    await task.ContinueWith(async previousTask =>
                    {
                        await RemoveFromSeeminglyUnresponsiveTaskModelsDictionary(taskModel.Key);
                    });
                }
                else if (task.Exception is not null)
                    await HandleException(task.Exception, taskModel, richErrorModelOverrideFunc);
                else
                {
                    if (SuccessfulTaskModelsDictionary.Count >= 99) await ClearSuccessfulTaskModels();

                    await AddToSuccessfulTaskModelsDictionary(taskModel.Key, taskModel);
                }
            }
            catch (Exception e)
            {
                await HandleException(e, taskModel, richErrorModelOverrideFunc);
            }
            finally
            {
                await RemoveFromActiveTaskModelsDictionary(taskModel.Key);
                cancellationTokenSource.Cancel();
            }
        }, cancellationToken);

        await AddToActiveTaskModelsDictionary(taskModel.Key, taskModel);

        // If in the instant between starting the task and Adding taskModel to
        // the dictionary the task completed and therefore was removed THEN
        // added therefore the taskModel must be removed
        if (taskModel.Task.IsCompleted) await RemoveFromActiveTaskModelsDictionary(taskModel.Key);

        return taskModel;
    }

    private static async Task HandleException(Exception e,
        TaskModel taskModel,
        Func<Exception, RichErrorModel?>? richErrorModelOverrideFunc)
    {
        RichErrorModel? richErrorModel = null;

        if (richErrorModelOverrideFunc is not null) richErrorModel = richErrorModelOverrideFunc(e);

        richErrorModel ??= new RichErrorModel(
            e.Message,
            $"{nameof(taskModel.Key.Title)} threw an exception",
            () => true,
            _ => Console.WriteLine(string.Empty)
        );

        taskModel.RichErrorModel = richErrorModel;

        await AddToFailedTaskModelsDictionary(taskModel.Key, taskModel);
    }

    private static async Task AddToActiveTaskModelsDictionary(TaskModelKey taskModelKey, TaskModel taskModel)
    {
        try
        {
            await ActiveTaskModelsDictionarySemaphoreSlim.WaitAsync();

            ActiveTaskModelsDictionary.Add(taskModelKey, taskModel);

            ActiveTaskModels = ActiveTaskModelsDictionary.Values
                .ToImmutableArray();

            OnTasksStateHasChangedEventHandler?.Invoke();
        }
        finally
        {
            ActiveTaskModelsDictionarySemaphoreSlim.Release();
        }
    }

    private static async Task AddToSeeminglyUnresponsiveTaskModelsDictionary(TaskModelKey taskModelKey,
        TaskModel taskModel)
    {
        try
        {
            await SeeminglyUnresponsiveTaskModelsDictionarySemaphoreSlim.WaitAsync();

            SeeminglyUnresponsiveTaskModelsDictionary.Add(taskModelKey, taskModel);

            SeeminglyUnresponsiveTaskModels = SeeminglyUnresponsiveTaskModelsDictionary.Values
                .ToImmutableArray();

            OnTasksStateHasChangedEventHandler?.Invoke();
        }
        finally
        {
            SeeminglyUnresponsiveTaskModelsDictionarySemaphoreSlim.Release();
        }
    }

    private static async Task AddToFailedTaskModelsDictionary(TaskModelKey taskModelKey, TaskModel taskModel)
    {
        try
        {
            await FailedTaskModelsDictionarySemaphoreSlim.WaitAsync();

            FailedTaskModelsDictionary.Add(taskModelKey, taskModel);

            FailedTaskModels = FailedTaskModelsDictionary.Values
                .ToImmutableArray();

            OnTasksStateHasChangedEventHandler?.Invoke();
        }
        finally
        {
            FailedTaskModelsDictionarySemaphoreSlim.Release();
        }
    }

    private static async Task AddToSuccessfulTaskModelsDictionary(TaskModelKey taskModelKey, TaskModel taskModel)
    {
        try
        {
            await SuccessfulTaskModelsDictionarySemaphoreSlim.WaitAsync();

            SuccessfulTaskModelsDictionary.Add(taskModelKey, taskModel);

            SuccessfulTaskModels = SuccessfulTaskModelsDictionary.Values
                .ToImmutableArray();

            OnTasksStateHasChangedEventHandler?.Invoke();
        }
        finally
        {
            SuccessfulTaskModelsDictionarySemaphoreSlim.Release();
        }
    }

    private static async Task RemoveFromActiveTaskModelsDictionary(TaskModelKey taskModelKey)
    {
        try
        {
            await ActiveTaskModelsDictionarySemaphoreSlim.WaitAsync();

            ActiveTaskModelsDictionary.Remove(taskModelKey);

            ActiveTaskModels = ActiveTaskModelsDictionary.Values
                .ToImmutableArray();

            OnTasksStateHasChangedEventHandler?.Invoke();
        }
        finally
        {
            ActiveTaskModelsDictionarySemaphoreSlim.Release();
        }
    }

    private static async Task RemoveFromSeeminglyUnresponsiveTaskModelsDictionary(TaskModelKey taskModelKey)
    {
        try
        {
            await SeeminglyUnresponsiveTaskModelsDictionarySemaphoreSlim.WaitAsync();

            SeeminglyUnresponsiveTaskModelsDictionary.Remove(taskModelKey);

            SeeminglyUnresponsiveTaskModels = SeeminglyUnresponsiveTaskModelsDictionary.Values
                .ToImmutableArray();

            OnTasksStateHasChangedEventHandler?.Invoke();
        }
        finally
        {
            SeeminglyUnresponsiveTaskModelsDictionarySemaphoreSlim.Release();
        }
    }

    public static async Task ClearActiveTaskModels()
    {
        try
        {
            await ActiveTaskModelsDictionarySemaphoreSlim.WaitAsync();

            ActiveTaskModelsDictionary.Clear();

            FailedTaskModelsDictionary.Clear();

            FailedTaskModels = FailedTaskModelsDictionary.Values
                .ToImmutableArray();

            OnTasksStateHasChangedEventHandler?.Invoke();
        }
        finally
        {
            ActiveTaskModelsDictionarySemaphoreSlim.Release();
        }
    }

    public static async Task ClearFailedTaskModels()
    {
        try
        {
            await FailedTaskModelsDictionarySemaphoreSlim.WaitAsync();

            FailedTaskModelsDictionary.Clear();

            FailedTaskModels = FailedTaskModelsDictionary.Values
                .ToImmutableArray();

            OnTasksStateHasChangedEventHandler?.Invoke();
        }
        finally
        {
            FailedTaskModelsDictionarySemaphoreSlim.Release();
        }
    }

    public static async Task ClearSuccessfulTaskModels()
    {
        try
        {
            await SuccessfulTaskModelsDictionarySemaphoreSlim.WaitAsync();

            SuccessfulTaskModelsDictionary.Clear();

            SuccessfulTaskModels = SuccessfulTaskModelsDictionary.Values
                .ToImmutableArray();

            OnTasksStateHasChangedEventHandler?.Invoke();
        }
        finally
        {
            SuccessfulTaskModelsDictionarySemaphoreSlim.Release();
        }
    }

    public static async Task ClearSeeminglyUnresponsiveTaskModels()
    {
        try
        {
            await SeeminglyUnresponsiveTaskModelsDictionarySemaphoreSlim.WaitAsync();

            SeeminglyUnresponsiveTaskModelsDictionary.Clear();

            SeeminglyUnresponsiveTaskModels = SeeminglyUnresponsiveTaskModelsDictionary.Values
                .ToImmutableArray();

            OnTasksStateHasChangedEventHandler?.Invoke();
        }
        finally
        {
            SeeminglyUnresponsiveTaskModelsDictionarySemaphoreSlim.Release();
        }
    }

    private class TaskModel : ITaskModel
    {
        public TaskModel(TaskModelKey key, TimeSpan lifetimeUntilSeemsUnresponsive,
            CancellationTokenSource cancellationTokenSource, bool isBackgroundTask)
        {
            Key = key;
            LifetimeUntilSeemsUnresponsive = lifetimeUntilSeemsUnresponsive;
            CancellationTokenSource = cancellationTokenSource;
            IsBackgroundTask = isBackgroundTask;
        }

        public TaskModelKey Key { get; }
        public Task Task { get; set; } = Task.CompletedTask;
        public TimeSpan LifetimeUntilSeemsUnresponsive { get; }
        public RichErrorModel? RichErrorModel { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; }
        public bool IsBackgroundTask { get; }
    }

    // ReSharper disable once UnusedMember.Local
    //
    // Need disable UnusedMember.Local because
    // I want this method to be here as indication of purpose
    // of the encompassing class even if it is isn't
    // currently being used.
    private static void OnOnTaskSeemsUnresponsiveEventHandler()
    {
        OnTaskSeemsUnresponsiveEventHandler?.Invoke();
    }
}