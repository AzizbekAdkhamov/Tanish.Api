using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using Tanish.Application.Common.Interfaces;
using Tanish.Infrastructure.AI;
using Tanish.Persistence.DbContexts;

namespace Tanish.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("Default"), o => o.UseVector()));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IEmbeddingService, EmbeddingService>();

        services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(sp =>
            new OpenAIClient(config["OpenAI:ApiKey"])
                .GetEmbeddingClient("text-embedding-3-small")
                .AsIEmbeddingGenerator());

        return services;
    }
}