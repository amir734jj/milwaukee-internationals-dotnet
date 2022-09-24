using System;
using System.Net;
using System.Reflection;
using System.Text;
using API.Extensions;
using API.Middlewares;
using API.Utilities;
using Microsoft.AspNetCore.SignalR;
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using DAL.Interfaces;
using DAL.ServiceApi;
using DAL.Utilities;
using EfCoreRepository.Extensions;
using Logic.Interfaces;
using Mailjet.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MlkPwgen;
using Models.Constants;
using Models.Entities;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;
using Scrutor;
using WebMarkupMin.AspNetCore6;
using static DAL.Utilities.ConnectionStringUtility;

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

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public void ConfigureServices(IServiceCollection services)
        {
            // https://stackoverflow.com/a/70304966/1834787
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var coldStartConfig = new BlobContainerClient(new Uri(Environment.GetEnvironmentVariable("AZURE_BLOB_CONFIG")!))
                .GetBlobClient("cold-start-config");

            services.AddDataProtection()
                .SetApplicationName("milwaukee-internationals-website-cold-start")
                .PersistKeysToAzureBlobStorage(coldStartConfig)
                .SetDefaultKeyLifetime(TimeSpan.FromDays(14));
            
            services.AddWebMarkupMin()
                .AddHtmlMinification()
                .AddXmlMinification()
                .AddHttpCompression();

            services.AddOptions();

            // Add our Config object so it can be injected
            services.Configure<SecureHeadersMiddlewareConfiguration>(
                _configuration.GetSection("SecureHeadersMiddlewareConfiguration"));

            services.AddLogging();
            
            services.Configure<JwtSettings>(_configuration.GetSection("JwtSettings"));

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

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

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

            services.Scan(scan => scan
                .FromAssemblies(Assembly.Load("API"), Assembly.Load("Logic"), Assembly.Load("DAL"))
                .AddClasses() //    to register
                .UsingRegistrationStrategy(RegistrationStrategy.Skip) // 2. Define how to handle duplicates
                .AsImplementedInterfaces() // 2. Specify which services they are registered as
                .WithTransientLifetime()); // 3. Set the lifetime for the services

            services.AddSingleton<CacheBustingUtility>();

            // If environment is localhost then use mock email service
            if (_env.IsDevelopment())
            {
                services.AddSingleton<IEmailServiceApi>(new EmailServiceApi());
            }
            else
            {
                /*
                var (accessKeyId, secretAccessKey, url) = (
                    _configuration.GetRequiredValue<string>("CLOUDCUBE_ACCESS_KEY_ID"),
                    _configuration.GetRequiredValue<string>("CLOUDCUBE_SECRET_ACCESS_KEY"),
                    _configuration.GetRequiredValue<string>("CLOUDCUBE_URL")
                );

                var prefix = new Uri(url).Segments[1];
                const string bucketName = "cloud-cube";

                // Generally bad practice
                var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);
                */

                // Create S3 client
                /*services.AddSingleton<IAmazonS3>(ctx => new AmazonS3Client(credentials, RegionEndpoint.USEast1));
                services.AddSingleton(new S3ServiceConfig(bucketName, prefix));

                services.AddTransient<IStorageService>(ctx => new S3StorageService(
                    ctx.GetRequiredService<ILogger<S3StorageService>>(),
                    ctx.GetRequiredService<IAmazonS3>(),
                    ctx.GetRequiredService<S3ServiceConfig>()
                ));*/
                
                services.AddSingleton(new BlobContainerClient(new Uri(Environment.GetEnvironmentVariable("AZURE_BLOB_CONFIG")!)));
                
                services.AddTransient<IStorageService, AzureBlobService>();
            }

            services.AddSingleton<GlobalConfigs>();

            // Initialize the email jet client
            services.AddTransient<IMailjetClient>(ctx => new MailjetClient(
                Environment.GetEnvironmentVariable("MAIL_JET_KEY"),
                Environment.GetEnvironmentVariable("MAIL_JET_SECRET"))
            );

            services.AddDbContext<EntityDbContext>(opt =>
            {
                if (_env.IsDevelopment())
                {
                    opt.EnableDetailedErrors();
                    opt.EnableSensitiveDataLogging();

                    opt.UseSqlite(_configuration.GetValue<string>("ConnectionStrings:Sqlite"));
                }
                else
                {
                    var postgresConnectionString =
                        ConnectionStringUrlToPgResource(_configuration.GetValue<string>("DATABASE_URL")
                                                        ?? throw new Exception("DATABASE_URL is null"));
                    opt.UseNpgsql(postgresConnectionString);
                }
            });

            services.AddIdentity<User, IdentityRole<int>>(x =>
                {
                    x.User.RequireUniqueEmail = true; 
                    x.Lockout.AllowedForNewUsers = true;
                    x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                    x.Lockout.MaxFailedAccessAttempts = 3;
                })
                .AddEntityFrameworkStores<EntityDbContext>()
                .AddRoles<IdentityRole<int>>()
                .AddDefaultTokenProviders();

            var jwtSetting = _configuration
                .GetSection("JwtSettings")
                .Get<JwtSettings>();

            // Random JWT key
            jwtSetting.Key = PasswordGenerator.Generate(length: 100, allowed: Sets.Alphanumerics);

            services.AddSingleton(jwtSetting);
            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
            {
                x.Cookie.MaxAge = TimeSpan.FromHours(3);
                x.LoginPath = new PathString("/Identity/login");
                x.LogoutPath = new PathString("/Identity/logout");
            }).AddJwtBearer(config =>
            {
                config.RequireHttpsMetadata = false;
                config.SaveToken = true;

                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSetting.Issuer,
                    ValidAudience = jwtSetting.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key))
                };
            });

            services.AddEfRepository<EntityDbContext>(opt => opt.Profile(Assembly.Load("Dal")));
            
            services.AddSingleton(new TableServiceClient(new Uri(Environment.GetEnvironmentVariable("AZURE_TABLE_EVENTS")!)));
            
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

            services.AddSignalR(config =>
            {
                config.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10 mega-bytes
                config.StreamBufferCapacity = 50;
                config.EnableDetailedErrors = true;
            }).AddNewtonsoftJsonProtocol();

            services.AddAutoMapper(Assembly.Load("Models"));
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configLogic"></param>
        /// <param name="apiEventService"></param>
        public void Configure(IApplicationBuilder app, IConfigLogic configLogic, IApiEventService apiEventService)
        {
            // Refresh global config
            configLogic.Refresh();

            // Add SecureHeadersMiddleware to the pipeline
            app.UseSecureHeadersMiddleware(_configuration.Get<SecureHeadersMiddlewareConfiguration>());

            app.UseCors("CorsPolicy");

            if (_env.IsDevelopment())
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
                    var exHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = exHandlerFeature?.Error;

                    var statusCodeEnum = (HttpStatusCode)context.Response.StatusCode;
                    await apiEventService.RecordEvent($"Failure with status code: {statusCodeEnum.ToString()} / {context.Response.StatusCode} route: [{context.Request.Method}] {context.Request.GetDisplayUrl()} => {exception?.Message}");
                    
                    context.Request.Path = $"/Error/{context.Response.StatusCode}";
                    await next();
                }
            });

            // Use wwwroot folder as default static path
            app.UseDefaultFiles()
                .UseHttpsRedirection()
                .UseEnableRequestRewind()
                .UseStaticFiles()
                .UseCookiePolicy()
                .UseSession()
                .UseRouting()
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHub<MessageHub>("/hub");
                    endpoints.MapHub<LogHub>("/log");
                });

            Console.WriteLine("Application Started!");
        }
    }
}
