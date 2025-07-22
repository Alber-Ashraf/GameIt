using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Models.Game;
using GameIt.BlazorUI.Services.Base;
using Microsoft.AspNetCore.Components;

namespace GameIt.BlazorUI.Pages.Game;

public partial class Index : IDisposable
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    public IGameService GameService { get; set; } = default!;

    [Inject]
    public ILogger<Index> Logger { get; set; } = default!;

    private CancellationTokenSource _cts = new();

    public List<GamesListVM> Games { get; private set; }
    public string ErrorMessage { get; set; } = string.Empty;

    protected void CreateGame() => NavigationManager.NavigateTo("/games/create/");

    protected void EditGame(Guid id) => NavigationManager.NavigateTo($"/games/edit/{id}");

    protected void ViewGame(Guid id) => NavigationManager.NavigateTo($"/games/details/{id}");

    protected async Task DeleteGame(Guid id)
    {
        try
        {
            var response = await GameService.Delete(id);

            if (response.Success)
            {
                Games = await GameService.GetAllWithCategoryAsync(_cts.Token);
                ErrorMessage = string.Empty;
            }
            else
            {
                ErrorMessage = response.Message;
            }
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Delete operation was canceled");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            Logger.LogError(ex, "Failed to delete game");
        }
        catch (Exception ex)
        {
            ErrorMessage = "An unexpected error occurred";
            Logger.LogError(ex, "Unexpected error deleting game");
        }

        StateHasChanged();
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Games = await GameService.GetAllWithCategoryAsync(_cts.Token);
        }
        catch (OperationCanceledException)
        {
            Logger.LogInformation("Game loading was canceled");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
            Logger.LogError(ex, "API error loading games");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load games";
            Logger.LogError(ex, "Error loading games");
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        GC.SuppressFinalize(this);
    }
}