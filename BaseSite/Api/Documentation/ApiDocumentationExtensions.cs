using BaseSite.Api.Authentication;
using Microsoft.OpenApi;

namespace BaseSite.Api.Documentation;

internal static class ApiDocumentationExtensions
{
    public static IServiceCollection AddApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info.Title = "BaseSite API";
                document.Info.Version = "v1";
                document.Info.Description = "Call POST /api/auth/login, then paste the accessToken from the response into Bearer authentication to try protected endpoints.";
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes[AccessTokenAuthenticationHandler.SchemeName] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Description = "Enter the accessToken returned by POST /api/auth/login, without the Bearer prefix."
                };
                return Task.CompletedTask;
            });

            options.AddOperationTransformer((operation, context, cancellationToken) =>
            {
                var metadata = context.Description.ActionDescriptor.EndpointMetadata;
                if (metadata.OfType<IAllowAnonymous>().Any() || !metadata.OfType<IAuthorizeData>().Any())
                    return Task.CompletedTask;

                operation.Security ??= [];
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(AccessTokenAuthenticationHandler.SchemeName, context.Document)] = []
                });
                return Task.CompletedTask;
            });
        });

        return services;
    }
}
