using System;
using System.Reflection;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using API.Extensions;
using API.Middlewares;
using Autofac;
using DAL.Configs;
using DAL.Interfaces;
using DAL.ServiceApi;
using DAL.Utilities;
using EasyCaching.Core.Configurations;
using EfCoreRepository.Extensions;
using EFCoreSecondLevelCacheInterceptor;
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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models.Constants;
using Models.Entities;
using Models.Utilities;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;
using reCAPTCHA.AspNetCore;
using WebMarkupMin.AspNetCore3;
using static Dal.Utilities.ConnectionStringUtility;

namespace API
{
    public class Startup
    {
        private readonly IConfigurationRoot _configuration;

        private readonly IWebHostEnvironment _env;

        public Startup(IWebHostEnvironment env)
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

        public void ConfigureContainer(ContainerBuilder builder)
        {
            
            var (accessKeyId, secretAccessKey, url) = (
                _configuration.GetRequiredValue<string>("CLOUDCUBE_ACCESS_KEY_ID"),
                _configuration.GetRequiredValue<string>("CLOUDCUBE_SECRET_ACCESS_KEY"),
                _configuration.GetRequiredValue<string>("CLOUDCUBE_URL")
            );

            var prefix = new Uri(url).Segments[1];
            const string bucketName = "cloud-cube";

            // Generally bad practice
            var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);

            // Create S3 client
            builder.Register(ctx => new AmazonS3Client(credentials, RegionEndpoint.USEast1)).As<IAmazonS3>();
            builder.RegisterInstance(new S3ServiceConfig(bucketName, prefix)).As<S3ServiceConfig>();

            builder.RegisterAssemblyTypes(Assembly.Load("API"), Assembly.Load("Logic"), Assembly.Load("DAL"))
                .AsImplementedInterfaces();
            
            // If environment is localhost then use mock email service
            if (_env.IsDevelopment())
            {
                builder.RegisterInstance(new EmailServiceApi()).As<IEmailServiceApi>();
                builder.RegisterInstance(new S3Service()).As<IS3Service>();
            }
            else
            {
                builder.Register(ctx => new S3Service(
                    ctx.Resolve<ILogger<S3Service>>(),
                    ctx.Resolve<IAmazonS3>(),
                    ctx.Resolve<S3ServiceConfig>()
                )).As<IS3Service>();
            }

            builder.RegisterType<GlobalConfigs>().SingleInstance();

            // Initialize the email jet client
            builder.Register(ctx => new MailjetClient(
                Environment.GetEnvironmentVariable("MAIL_JET_KEY"),
                Environment.GetEnvironmentVariable("MAIL_JET_SECRET"))
            ).As<IMailjetClient>();
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public void ConfigureServices(IServiceCollection services)
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Milwaukee-Internationals-API", Version = "v1" });
            });

            services.AddMvc(x =>
                {
                    x.ModelValidatorProviders.Clear();

                    // Not need to have https
                    x.RequireHttpsPermanent = false;

                    // Allow anonymous for localhost
                    if (_env.IsDevelopment())
                    {
                        x.Filters.Add<AllowAnonymousFilter>();
                    }

                    x.Filters.Add<PreventAuthenticatedActionFilter>();
                })
                .AddViewOptions(x => { x.HtmlHelperOptions.ClientValidationEnabled = true; })
                .AddNewtonsoftJson(x =>
                {
                    x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    x.SerializerSettings.Converters.Add(new StringEnumConverter());
                }).AddRazorPagesOptions(x => { x.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()); });

            services.AddWebMarkupMin(opt =>
                {
                    opt.AllowMinificationInDevelopmentEnvironment = true;
                    opt.AllowCompressionInDevelopmentEnvironment = true;
                })
                .AddHtmlMinification()
                .AddHttpCompression();

            services.AddDbContext<EntityDbContext>(opt =>
            {
                if (_env.IsDevelopment())
                {
                    opt.UseSqlite(_configuration.GetValue<string>("ConnectionStrings:Sqlite"));
                }
                else
                {
                    var postgresConnectionString =
                        ConnectionStringUrlToPgResource(_configuration.GetValue<string>("DATABASE_URL")
                                                        ?? throw new Exception("DATABASE_URL is null"));
                    opt.UseNpgsql(postgresConnectionString);
                }
            }, ServiceLifetime.Transient);

            services.AddIdentity<User, IdentityRole<int>>(x => { x.User.RequireUniqueEmail = true; })
                .AddEntityFrameworkStores<EntityDbContext>()
                .AddRoles<IdentityRole<int>>()
                .AddDefaultTokenProviders();

            // L2 EF cache
            if (_env.IsDevelopment())
            {
                services.AddEFSecondLevelCache(options =>
                    options.UseEasyCachingCoreProvider("memory").DisableLogging(true)
                );

                services.AddEasyCaching(options => options.UseInMemory("memory"));
            }
            else
            {
                services.AddEFSecondLevelCache(options =>
                    options.UseEasyCachingCoreProvider("redis").DisableLogging(true));

                services.AddEasyCaching(options =>
                {
                    var (_, dictionary) = UrlUtility.UrlToResource(_configuration.GetValue<string>("REDISTOGO_URL"));

                    // use memory cache with your own configuration
                    options.UseRedis(x =>
                    {
                        x.DBConfig.Endpoints.Add(new ServerEndPoint(dictionary["Host"], int.Parse(dictionary["Port"])));
                        x.DBConfig.Username = dictionary["Username"];
                        x.DBConfig.Password = dictionary["Password"];
                        x.DBConfig.AbortOnConnectFail = false;
                    }, "redis");
                });
            }

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
            {
                x.Cookie.MaxAge = TimeSpan.FromMinutes(60);
                x.LoginPath = new PathString("/Identity/login");
                x.LogoutPath = new PathString("/Identity/logout");
            });

            // Re-Captcha config
            services.Configure<RecaptchaSettings>(_configuration.GetSection("RecaptchaSettings"));
            services.AddTransient<IRecaptchaService, RecaptchaService>();

            services.AddEfRepository<EntityDbContext>(c => c.Profile(Assembly.Load("DAL")));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configLogic"></param>
        public void Configure(IApplicationBuilder app, IConfigLogic configLogic)
        {
            // Refresh global config
            configLogic.Refresh();

            // Add SecureHeadersMiddleware to the pipeline
            app.UseSecureHeadersMiddleware(_configuration.Get<SecureHeadersMiddlewareConfiguration>());

            app.UseCors("CorsPolicy");

            if (_env.IsDevelopment())
            {
                app.UseDatabaseErrorPage();

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

            // Not necessary for this workshop but useful when running behind kubernetes
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                // Read and use headers coming from reverse proxy: X-Forwarded-For X-Forwarded-Proto
                // This is particularly important so that HttpContent.Request.Scheme will be correct behind a SSL terminating proxy
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
            });

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.IsFailure())
                {
                    context.Request.Path = $"/Error/{context.Response.StatusCode}";
                    await next();
                }
            });


            // Use wwwroot folder as default static path
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseCookiePolicy()
                .UseSession()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => endpoints.MapControllers());

            Console.WriteLine("Application Started!");
        }
    }
}