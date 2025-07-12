using Aplicacion.Services.whatsapp;
using Aplicacion.Services;
using Infraestructura.Context;
using Infraestructura.Core.Jwtoken;
using Infraestructura.Core.RestClient;
using Microsoft.EntityFrameworkCore;
using WebServices.Middleware;

namespace WebServices.RegisterContainer
{
    public static class Container
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            string conectionString = configuration.GetConnectionString("conectionDataBase");

            services.AddDbContext<AppDbContext>(
                dbContextOption => dbContextOption.UseSqlServer(conectionString),
                ServiceLifetime.Transient
            );

            services.AddTransient<IDataContext, AppDbContext>();
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // JWT
            services.AddTransient<ITokenService, JwtTokenService>();

            // RestClient
            RestClientFactory.SetCurrent(new HttpRestClientFactory());
            //services.AddTransient<IRestClient, HttpRestClient>();
            //services.AddTransient<IRestClientFactory, HttpRestClientFactory>();

            // Application services
            services.AddScoped<SecurityApplicationService>();
            services.AddScoped<WhatsappAppService>();
            services.AddScoped<SesionLogApplicationService>();

            // Middleware
            services.AddTransient<GlobalExceptionHandlingMiddleware>();

            return services;
        }
    }
}
