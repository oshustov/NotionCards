using Microsoft.Extensions.DependencyInjection;
using NotionCards.Core.Data.Entities;

namespace NotionCards.Storage;

public static class DependencyInjection
{
  public static IServiceCollection AddStorage(this IServiceCollection services)
  {
    services.AddDbContext<AppDbContext>();
    services.AddScoped<ICardsStorage, CardsStorage>();
    return services;
  }
}