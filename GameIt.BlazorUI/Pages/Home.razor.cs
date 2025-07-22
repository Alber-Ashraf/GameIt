using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Models.Game;
using GameIt.BlazorUI.Services;
using Microsoft.AspNetCore.Components;

namespace GameIt.BlazorUI.Pages
{
    public partial class Home
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        public IGameService GameService { get; set; } = default!;

        private CancellationToken token = default;

        //private List<CategoryDto> Categories = new();
        private List<GamesListVM> FeaturedGames = new();
        private Random random = new();

        protected override async Task OnInitializedAsync()
        {
            //Categories = await CategoryService.GetAllCategoriesAsync();
            FeaturedGames = (await GameService.GetFeaturedGamesAsync(6, token)).ToList();
        }

        protected string[] Categories = new string[]
        {
            "Action", "RPG", "Comedy", "Strategy", "Sports", "Horror"
        };

        protected void ViewGame(Guid id) => NavigationManager.NavigateTo($"/games/details/{id}");

        private string GetRandomShape()
        {
            var shapes = new[] { "rounded", "hexagon", "diamond", "blob" };
            return shapes[random.Next(shapes.Length)];
        }
    }
}
