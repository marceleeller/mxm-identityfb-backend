using MxmIdentityfbBackend.Infra.Services;

namespace MxmIdentityfbBackend.Configuration;

public static class DependencyInjectionConfig
{

    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddScoped<TokenService>();

        return services;
    }
}
