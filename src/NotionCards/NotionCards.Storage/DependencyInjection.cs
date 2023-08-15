using Marten;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NotionCards.Storage
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddStorage(this IServiceCollection services, IHostEnvironment env)
    {
      services.AddMarten(x =>
      {
        x.Connection("Host=localhost;Port=5432;Database=postgres;Username=postgres;password=pgpassword");

        if (env.IsDevelopment())
        {
          x.AutoCreateSchemaObjects = Weasel.Core.AutoCreate.All;
        }
      });

      return services;
    }
  }
}
