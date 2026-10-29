namespace GameIt.BlazorUI.Core.Notifications;

public enum ToastLevel
{
    Success,
    Info,
    Warning,
    Error
}

public sealed record ToastMessage(Guid Id, string Title, string Message, ToastLevel Level, DateTime CreatedAt);

public class ToastService
{
    private readonly List<ToastMessage> _toasts = new();
    public event Action? OnChange;

    public IReadOnlyList<ToastMessage> Toasts => _toasts;

    public void ShowSuccess(string message, string title = "Success") => Enqueue(title, message, ToastLevel.Success);
    public void ShowInfo(string message, string title = "Notice") => Enqueue(title, message, ToastLevel.Info);
    public void ShowWarning(string message, string title = "Warning") => Enqueue(title, message, ToastLevel.Warning);
    public void ShowError(string message, string title = "Error") => Enqueue(title, message, ToastLevel.Error);

    public void Remove(Guid id)
    {
        var removed = _toasts.RemoveAll(t => t.Id == id);
        if (removed > 0)
        {
            OnChange?.Invoke();
        }
    }

    private void Enqueue(string title, string message, ToastLevel level)
    {
        var toast = new ToastMessage(Guid.NewGuid(), title, message, level, DateTime.UtcNow);
        _toasts.Add(toast);
        OnChange?.Invoke();

        _ = DismissAfterDelayAsync(toast.Id, level == ToastLevel.Error ? 6000 : 4000);
    }

    private async Task DismissAfterDelayAsync(Guid id, int delayMs)
    {
        await Task.Delay(delayMs);
        Remove(id);
    }
}
