namespace PreconIQAPI.Configuration;

/// <summary>
/// Represents Swagger/OpenAPI documentation configuration settings.
/// </summary>
/// <remarks>
/// This class centralizes all Swagger-related configuration including API metadata,
/// OAuth2 settings, and UI customization options.
/// </remarks>
public class SwaggerConfig
{
    /// <summary>
    /// The API title displayed in Swagger UI.
    /// </summary>
    public string Title { get; set; } = "PreconIQ API";

    /// <summary>
    /// The API version displayed in Swagger UI.
    /// </summary>
    public string Version { get; set; } = "v1";

    /// <summary>
    /// The OAuth2 redirect URL for Swagger UI authentication.
    /// </summary>
    public string OAuth2RedirectUrl { get; set; } = string.Empty;

    /// <summary>
    /// The security scheme identifier used in OpenAPI specification.
    /// </summary>
    public string SecuritySchemeId { get; set; } = "oauth2";

    /// <summary>
    /// The description for the API access scope.
    /// </summary>
    public string ScopeDescription { get; set; } = "API Access";

    /// <summary>
    /// The Swagger endpoint path.
    /// </summary>
    public string SwaggerEndpoint { get; set; } = "/swagger/v1/swagger.json";

    /// <summary>
    /// Additional OAuth2 scopes required for Swagger UI authentication.
    /// </summary>
    public string[] AdditionalScopes { get; set; } = { "openid", "profile" };
}