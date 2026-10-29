using GameIt.BlazorUI.Core.Http;
using GameIt.Contracts.Categories;
using System.Net.Http.Json;

namespace GameIt.BlazorUI.Features.Categories.Services;

public interface ICategoriesApiClient
{
    Task<ApiResult<List<CategorySummaryDto>>> GetAllCategoriesAsync(CancellationToken ct = default);
    Task<ApiResult<CategoryDetailDto>> GetCategoryByIdAsync(Guid id, CancellationToken ct = default);
    Task<ApiResult<CategoryDetailDto>> GetCategoryByNameAsync(string name, CancellationToken ct = default);
    Task<ApiResult<Guid>> CreateCategoryAsync(UpsertCategoryRequestDto request, CancellationToken ct = default);
    Task<ApiResult> UpdateCategoryAsync(Guid id, UpsertCategoryRequestDto request, CancellationToken ct = default);
    Task<ApiResult> DeleteCategoryAsync(Guid id, CancellationToken ct = default);
}

public class CategoriesApiClient : ICategoriesApiClient
{
    private readonly HttpClient _http;

    public CategoriesApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ApiResult<List<CategorySummaryDto>>> GetAllCategoriesAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.Categories, ct);
        return await ProblemDetailsParser.ToApiResultAsync<List<CategorySummaryDto>>(response);
    }

    public async Task<ApiResult<CategoryDetailDto>> GetCategoryByIdAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.CategoryById(id), ct);
        return await ProblemDetailsParser.ToApiResultAsync<CategoryDetailDto>(response);
    }

    public async Task<ApiResult<CategoryDetailDto>> GetCategoryByNameAsync(string name, CancellationToken ct = default)
    {
        var response = await _http.GetAsync(ApiEndpoints.CategoryByName(name), ct);
        return await ProblemDetailsParser.ToApiResultAsync<CategoryDetailDto>(response);
    }

    public async Task<ApiResult<Guid>> CreateCategoryAsync(UpsertCategoryRequestDto request, CancellationToken ct = default)
    {
        var response = await _http.PostAsJsonAsync(ApiEndpoints.Categories, request, ct);
        return await ProblemDetailsParser.ToApiResultAsync<Guid>(response);
    }

    public async Task<ApiResult> UpdateCategoryAsync(Guid id, UpsertCategoryRequestDto request, CancellationToken ct = default)
    {
        request.Id = id;
        var response = await _http.PutAsJsonAsync(ApiEndpoints.CategoryById(id), request, ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }

    public async Task<ApiResult> DeleteCategoryAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _http.DeleteAsync(ApiEndpoints.CategoryById(id), ct);
        return await ProblemDetailsParser.ToApiResultAsync(response);
    }
}
