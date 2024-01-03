using Microsoft.Extensions.DependencyInjection;

namespace NotionCards.Storage;

public static class DependencyInjection
{
  public static IServiceCollection AddStorage(this IServiceCollection services)
  {
    services.AddDbContext<AppDbContext>();
    return services;
  }
}