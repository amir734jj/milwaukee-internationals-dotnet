using System;
using System.Net;
using System.Reflection;
using API.Attributes;
using API.Extensions;
using AutoMapper;
using DAL.Utilities;
using Logic;
using Logic.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.Constants;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;
using static API.Utilities.ConnectionStringUtility;

namespace API
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        private IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options => { options.LowercaseUrls = true; });

            services.AddMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = ApiConstants.AuthenticationSessionCookieName;
            });
            
            // All the other service configuration.
            services.AddAutoMapper(x => { x.AddProfiles(Assembly.Load("Models")); });

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info {Title = "Milwaukee-Internationals-API", Version = "v1"}); });

            services.AddMvc(x => { x.Filters.Add<AuthorizeActionFilter>(); });

            var container = new Container();

            container.Configure(config =>
            {
                // Register stuff in container, using the StructureMap APIs...
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.Assembly("Logic");
                    _.Assembly("DAL");
                    _.WithDefaultConventions();
                });

                //Populate the container using the service collection
                config.Populate(services);

                config.For<EntityDbContext>().Use(new EntityDbContext(builder =>
                {
                    if (_env.IsLocalhost())
                    {
                        builder.UseSqlite(_configuration.GetValue<string>("ConnectionStrings:Sqlite"));    
                    }
                    else
                    {
                        builder.UseNpgsql(ConnectionStringUrlToResource(Environment.GetEnvironmentVariable("DATABASE_URL"))
                                          ?? throw new Exception("DATABASE_URL is null"));
                    }
                }));

                // It has to be a singleton
                config.For<ISigninLogic>().Singleton();
            });
            
            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {   
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            app.UseDeveloperExceptionPage();

            app.UseCookiePolicy();

            app.UseSession();
            
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}");
            });

            app.UseStaticFiles();
        }
    }
}