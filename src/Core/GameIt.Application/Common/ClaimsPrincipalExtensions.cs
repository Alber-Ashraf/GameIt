namespace GameIt.Application.Common;

public static class ClaimsPrincipalExtensions
{
    public static string? FindFirstValue(this System.Security.Claims.ClaimsPrincipal? principal, string claimType)
        => principal?.FindFirst(claimType)?.Value;
}
