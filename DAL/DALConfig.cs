using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace DAL
{
    public static class DALConfig
    {
        public static IServiceCollection ConfigureDAL(this IServiceCollection services, IConfiguration config)
        {
            var cString = config.GetConnectionString("Default");
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cString));
            services.AddTransient<IDbContext, AppDbContext>();
            services.AddScoped<IStore, EntityStore>();
            return services;
        }
    }
}
