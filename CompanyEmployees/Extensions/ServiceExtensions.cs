using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Repository;
using CompanyEmployees.Formatter;

namespace CompanyEmployees.Extensions
{
    public static class ServiceExtensions
    {

        //Configure CORS - a mechanism to give or restrict access rights to applications from different domains
        public static void ConfigureCors(this IServiceCollection services) =>
            services.AddCors(options =>
            {
                //options.AddPolicy("CorsPolicy", builder =>
                //    builder.AllowAnyOrigin()
                //    .AllowAnyMethod()
                //    .AllowAnyHeader());

                options.AddPolicy("CorsPolicy", builder =>
                    builder.WithOrigins("https://google.com.ng")
                    .WithMethods("post", "get")
                    .WithHeaders("accept", "content-type"));
            });

        public static void ConfigureIISIntegration(this IServiceCollection services) =>
            services.Configure<IISOptions>(options => { });

        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddScoped<ILoggerManager, LoggerManager>();

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<RepositoryContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DbConnection"), 
                    b => b.MigrationsAssembly("CompanyEmployees")));

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder) =>
            builder.AddMvcOptions(config => 
                config.OutputFormatters.Add(new CsvOutputFormatter()));
    }
}
