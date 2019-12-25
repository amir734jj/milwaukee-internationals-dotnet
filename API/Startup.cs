using System;
using API.Extensions;
using DAL.Interfaces;
using DAL.ServiceApi;
using DAL.Utilities;
using Logic.Interfaces;
using Mailjet.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models.Constants;
using Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;
using StructureMap;
using Swashbuckle.AspNetCore.Swagger;
using WebMarkupMin.AspNetCore2;
using static API.Utilities.ConnectionStringUtility;

namespace API
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        private readonly IHostingEnvironment _env;

        private IContainer _container;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddJsonFile("secureHeaderSettings.json", true, true)
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
            services.AddOptions();

            // Add our Config object so it can be injected
            services.Configure<SecureHeadersMiddlewareConfiguration>(
                _configuration.GetSection("SecureHeadersMiddlewareConfiguration"));

            services.AddLogging();

            // Add MailKit
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
                x.ModelValidatorProviders.Clear();

                // Not need to have https
                x.RequireHttpsPermanent = false;

                // Allow anonymous for localhost
                if (_env.IsLocalhost())
                {
                    x.Filters.Add<AllowAnonymousFilter>();
                }
            }).AddJsonOptions(x =>
            {
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                x.SerializerSettings.Converters.Add(new StringEnumConverter());
            }).AddRazorPagesOptions(x =>
            {
                x.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            });
            
            services.AddWebMarkupMin(opt =>
                {
                    opt.AllowMinificationInDevelopmentEnvironment = true;
                    opt.AllowCompressionInDevelopmentEnvironment = true;
                })
                .AddHtmlMinification()
                .AddHttpCompression();
            
            services.AddDbContext<EntityDbContext>(opt => ResolveEntityDbContext(_env, _configuration)(opt));

            services.AddIdentity<User, IdentityRole<int>>(x => { x.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<EntityDbContext>()
                .AddRoles<IdentityRole<int>>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
            {
                x.Cookie.MaxAge = TimeSpan.FromMinutes(60);
            });
            
            _container = new Container(config =>
            {
                // Register stuff in container, using the StructureMap APIs...
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.Assembly("API");
                    _.Assembly("Logic");
                    _.Assembly("DAL");
                    _.WithDefaultConventions();
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
        public void Configure(IApplicationBuilder app)
        {
            // Add SecureHeadersMiddleware to the pipeline
            app.UseSecureHeadersMiddleware(_configuration.Get<SecureHeadersMiddlewareConfiguration>());

            app.UseEnableRequestRewind();

            app.UseDatabaseErrorPage();

            app.UseDeveloperExceptionPage();

            app.UseAuthentication();

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
                app.UseWebMarkupMin();
            }

            // Use wwwroot folder as default static path
            app.UseDefaultFiles();

            // Serve static files
            app.UseStaticFiles();

            // Not necessary for this workshop but useful when running behind kubernetes
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                // Read and use headers coming from reverse proxy: X-Forwarded-For X-Forwarded-Proto
                // This is particularly important so that HttpContet.Request.Scheme will be correct behind a SSL terminating proxy
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
            });

            app.UseCookiePolicy();

            app.UseSession();

            app.UseMvc(routes => { routes.MapRoute("default", "{controller=Home}/{action=Index}"); });

            Console.WriteLine("Application Started!");
        }

        /// <summary>
        /// Resolve EntityDbContext
        /// </summary>
        /// <param name="env"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static Action<DbContextOptionsBuilder> ResolveEntityDbContext(IHostingEnvironment env,
            IConfiguration configuration)
        {
            return builder =>
            {
                if (env.IsLocalhost())
                {
                    builder.UseSqlite(configuration.GetValue<string>("ConnectionStrings:Sqlite"));
                }
                else
                {
                    builder.UseNpgsql(
                        ConnectionStringUrlToResource(Environment.GetEnvironmentVariable("DATABASE_URL"))
                        ?? throw new Exception("DATABASE_URL is null"));
                }
            };
        }
    }
}
