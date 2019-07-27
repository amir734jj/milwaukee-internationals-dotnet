using System;
using System.Reflection;
using API.Attributes;
using API.Extensions;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using DAL.Interfaces;
using DAL.ServiceApi;
using DAL.Utilities;
using Logic.Interfaces;
using Mailjet.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;
using static API.Utilities.ConnectionStringUtility;

namespace API
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        private readonly IHostingEnvironment _env;

        private Container _container;

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

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //Add MailKit
            services.AddMailKit(optionBuilder =>
            {
                var emailSection = _configuration.GetSection("Email");

                var mailKitOptions = new MailKitOptions
                {
                    // Get options from secrets.json
                    Server = emailSection.GetValue<string>("Server"),
                    Port = emailSection.GetValue<int>("Port"),
                    SenderName = emailSection.GetValue<string>("SenderName"),
                    SenderEmail = emailSection.GetValue<string>("SenderEmail"),

                    // Can be optional with no authentication 
                    Account = emailSection.GetValue<string>("Account"),
                    Password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD"),

                    // Enable ssl or tls
                    Security = true
                };

                optionBuilder.UseMailKit(mailKitOptions);
            });

            services.AddRouting(options => { options.LowercaseUrls = true; });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(50);
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = ApiConstants.AuthenticationSessionCookieName;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {Title = "Milwaukee-Internationals-API", Version = "v1"});
            });

            services.AddMvc(x =>
            {
                // Authorize
                x.Filters.Add<AuthorizeActionFilter>();

                // Role
                x.Filters.Add<UserRoleActionFilter>();

                x.ModelValidatorProviders.Clear();

                // Not need to have https
                x.RequireHttpsPermanent = false;
            }).AddJsonOptions(x =>
            {
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                x.SerializerSettings.Converters.Add(new StringEnumConverter());
            }).AddRazorPagesOptions(x => { x.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()); });

            _container = new Container();

            _container.Configure(config =>
            {
                // Register stuff in container, using the StructureMap APIs...
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.Assembly("Logic");
                    _.Assembly("DAL");
                    _.WithDefaultConventions();
                });

                var entityDbContext = new EntityDbContext(builder =>
                {
                    if (_env.IsLocalhost())
                    {
                        builder.UseSqlite(_configuration.GetValue<string>("ConnectionStrings:Sqlite"));
                    }
                    else
                    {
                        builder.UseNpgsql(
                            ConnectionStringUrlToResource(Environment.GetEnvironmentVariable("DATABASE_URL"))
                            ?? throw new Exception("DATABASE_URL is null"));
                    }
                });
                
                config.For<EntityDbContext>().Use(entityDbContext).Transient();
                
                services.AddSingleton(entityDbContext);

                // All the other service configuration.
                services.AddAutoMapper(x =>
                {
                    x.AddMaps(Assembly.Load("Models"));
                    x.AddCollectionMappers();
                    x.UseEntityFrameworkCoreModel<EntityDbContext>(services);
                });

                // If environment is localhost then use mock email service
                if (_env.IsLocalhost())
                {
                    config.For<IEmailServiceApi>().Use(new EmailServiceApi()).Singleton();
                }
                
                // It has to be a singleton
                config.For<IIdentityDictionary>().Singleton();

                // Singleton to handle identities
                config.For<IIdentityLogic>().Singleton();

                // Initialize the email jet client
                config.For<IMailjetClient>().Use(new MailjetClient(
                    Environment.GetEnvironmentVariable("MAIL_JET_KEY"),
                    Environment.GetEnvironmentVariable("MAIL_JET_SECRET"))
                    ).Singleton();
                
                // Populate the container using the service collection
                config.Populate(services);
            });
            
            _container.AssertConfigurationIsValid();

            return _container.GetInstance<IServiceProvider>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMapper mapper)
        {
            if (_env.IsLocalhost())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseDeveloperExceptionPage();
            app.UseCookiePolicy();
            app.UseSession();
            app.UseStaticFiles();

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Home}/{action=Index}"); });

            // Just to make sure everything is running fine
            _container.GetInstance<EntityDbContext>();

            Console.WriteLine("Application Started!");
        }
    }
}