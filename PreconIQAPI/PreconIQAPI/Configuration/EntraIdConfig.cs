namespace PreconIQAPI.Configuration;

/// <summary>
/// Represents the configuration settings required to integrate with Microsoft Entra ID (formerly Azure AD).
/// </summary>
/// <remarks>
/// This class provides properties for configuring authentication and authorization endpoints, as well
/// as client identifiers and scopes for API and UI integration. It is typically used to centralize and manage Entra
/// ID-related settings in an application.
/// </remarks>
public class EntraIdConfig
{
    /// <summary>
    /// The base login URL for Microsoft Entra ID (e.g., "https://login.microsoftonline.com/").
    /// </summary>
    public string LoginUrl { get; set; } = string.Empty;

    /// <summary>
    /// The tenant ID (GUID) for the Entra ID tenant.
    /// </summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>
    /// The client ID of the API application registration in Entra ID.
    /// </summary>
    public string ApiClientId { get; set; } = string.Empty;

    /// <summary>
    /// The client ID of the UI/SPA application registration in Entra ID.
    /// </summary>
    public string UiClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets the base authority URL for the tenant.
    /// </summary>
    public string AuthorityBase => $"{LoginUrl}{TenantId}";

    /// <summary>
    /// Gets the v2.0 authority URL for JWT token validation.
    /// </summary>
    public string Authority => $"{AuthorityBase}/v2.0";

    /// <summary>
    /// Gets the OAuth2 authorization endpoint URL.
    /// </summary>
    public Uri AuthorizationUrl => new($"{AuthorityBase}/oauth2/v2.0/authorize");

    /// <summary>
    /// Gets the OAuth2 token endpoint URL.
    /// </summary>
    public Uri TokenUrl => new($"{AuthorityBase}/oauth2/v2.0/token");

    /// <summary>
    /// Gets the full scope name for API access (api://{ApiClientId}/api.access).
    /// </summary>
    public string ScopeFullName => $"api://{ApiClientId}/api.access";
}