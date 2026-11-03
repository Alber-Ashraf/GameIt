using Blazored.LocalStorage;
using GameIt.BlazorUI;
using GameIt.BlazorUI.Core.Auth;
using GameIt.BlazorUI.Core.Http;
using GameIt.BlazorUI.Core.Notifications;
using GameIt.BlazorUI.Features.Auth.Services;
using GameIt.BlazorUI.Features.Catalog.Services;
using GameIt.BlazorUI.Features.Categories.Services;
using GameIt.BlazorUI.Features.Discounts.Services;
using GameIt.BlazorUI.Features.Library.Services;
using GameIt.BlazorUI.Features.Purchases.Services;
using GameIt.BlazorUI.Features.Reviews.Services;
using GameIt.BlazorUI.Features.Wishlist.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var hostBase = builder.HostEnvironment.BaseAddress;
var configuredApiUrl = builder.Configuration["ApiBaseUrl"];
string apiBaseUrl;
if (!string.IsNullOrWhiteSpace(configuredApiUrl))
{
    apiBaseUrl = configuredApiUrl.EndsWith('/') ? configuredApiUrl : $"{configuredApiUrl}/";
}
else if (hostBase.Contains("localhost:7294") || hostBase.Contains("localhost:5036"))
{
    apiBaseUrl = "https://localhost:7040/";
}
else
{
    apiBaseUrl = hostBase;
}

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<ToastService>();
builder.Services.AddTransient<JwtAuthorizationMessageHandler>();

builder.Services.AddHttpClient("GameItApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<JwtAuthorizationMessageHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("GameItApi"));

builder.Services.AddScoped<IAuthApiClient, AuthApiClient>();
builder.Services.AddScoped<IGamesApiClient, GamesApiClient>();
builder.Services.AddScoped<ICategoriesApiClient, CategoriesApiClient>();
builder.Services.AddScoped<IDiscountsApiClient, DiscountsApiClient>();
builder.Services.AddScoped<IReviewsApiClient, ReviewsApiClient>();
builder.Services.AddScoped<IWishlistApiClient, WishlistApiClient>();
builder.Services.AddScoped<ILibraryApiClient, LibraryApiClient>();
builder.Services.AddScoped<IPurchasesApiClient, PurchasesApiClient>();

await builder.Build().RunAsync();
