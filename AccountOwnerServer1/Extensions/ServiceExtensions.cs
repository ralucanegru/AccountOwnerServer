using Contracts;
using Entities;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;
using System.Configuration;

namespace AccountOwnerServer1.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services) {
            
            services.AddCors( options =>
            {
                // AllowAnyOrigin() -> method which allows requests from any source
                // AllowAnyMethod() -> that allows all HTTP methods
                options.AddPolicy("CosPolicy", builder => builder.AllowAnyOrigin()
                                                    .AllowAnyMethod()
                                                    .AllowAnyHeader());
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(options =>
            {
            });
        }

        // create the service the first time you request it and then every subsequent request is calling the same instance of the service
        // all components are sharing the same service every time they need it
        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureMySqlContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config["SqlDatabase:ConnectionString"];

            /*
            services.AddDbContext<RepositoryContext>(options =>
            {
                options.UseSqlServer(config["SqlDatabase:ConnectionString"], opt =>
                {
                    opt.UseNetTopologySuite();
                    opt.EnableRetryOnFailure(
                        maxRetryCount: 6,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: new List<int> { 4060, 11001 });
                });

            });
            */
            services.AddDbContext<RepositoryContext>(o => o.UseSqlServer(connectionString));

        }

        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        }
    }
}
