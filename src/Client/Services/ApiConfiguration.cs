using System.Reflection;

namespace Client.Services;

internal static class ApiConfiguration
{
    public static string BaseUrl { get; } =
        typeof(ApiConfiguration).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .SingleOrDefault(attribute => attribute.Key == "StylesChatApiBaseUrl")
            ?.Value
        ?? throw new InvalidOperationException("The API base URL is not configured.");
}
