using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using ZonefyDotnet.Database;
using ZonefyDotnet.Services.Interfaces;
using ZonefyDotnet.Services.Implementations;
using ZonefyDotnet.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ZonefyDotnet.Configurations;
using ZonefyDotnet.Common;
using Google.Apis.Drive.v3;

namespace ZonefyDotnet.DI
{
    public static class ServiceCollectionExtension
    {
        private static readonly ILoggerFactory ContextLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });


        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(opts =>
            {
                opts.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
        }

        public static void ConfigureRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {/*
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetConcurrencyLimiter(
                        partitionKey: context.Request.Headers.Host.ToString(),
                        factory: partition => new ConcurrencyLimiterOptions
                        {
                            PermitLimit = 1,
                        })                        
                    );*/
                /*options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Request.Headers.Host.ToString(),
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 5,
                            QueueLimit = 10,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            Window = TimeSpan.FromSeconds(10)
                        })
                );*/
                /*options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.Request.Headers.Host.ToString(),
                        factory: partition => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            SegmentsPerWindow = 2,
                            Window = TimeSpan.FromSeconds(1),
                        })
                );*/

                /*options.AddFixedWindowLimiter("getAllCoursePolicy", opt =>
                {
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.PermitLimit = 1;
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });*/

                //sliding window
                /*options.AddSlidingWindowLimiter("getAllCoursePolicy", opt =>
                {
                    opt.Window = TimeSpan.FromSeconds(1);
                    opt.SegmentsPerWindow = 2;
                    opt.PermitLimit = 2;
                    opt.QueueLimit = 3;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });*/
                options.AddTokenBucketLimiter("getAllCoursePolicy", opt =>
                {
                    opt.TokenLimit = 4;
                    opt.QueueLimit = 2;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                    opt.TokensPerPeriod = 1;
                    opt.AutoReplenishment = true;
                });//.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = (c, t) =>
                {
                    Console.WriteLine(c.HttpContext.Request.Path);
                    var exception = new RestException(HttpStatusCode.TooManyRequests, ResponseMessages.TooManyRequest);
                    return ValueTask.FromException(exception);
                    //return ValueTask.CompletedTask;
                };
            });
        }

        [Obsolete]
        public static void ConfigureMvc(this IServiceCollection services)
        {
            /*services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Latest)
                .ConfigureApiBehaviorOptions(o =>
                {
                    o.InvalidModelStateResponseFactory = context => new ValidationFailedResult(context.ModelState);
                }).AddFluentValidation(x =>
                    x.RegisterValidatorsFromAssemblyContaining<UserValidator>());*/
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            //services.AddAutoMapper(typeof(UserMapper));
        }

        //add database
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            var constring = configuration.GetConnectionString("DefaultSQLConnection");
            services.AddDbContext<AppDbContext>(opts =>
                opts
                    .UseLoggerFactory(ContextLoggerFactory)
                    // .UseSqlServer(constring));
                    .UseNpgsql(constring));
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddScoped<ILoggerManager, LoggerManager>();
        }

        //JWT TOKEN
        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtParameters>();
            //var jwtUserSecret = jwtSettings.GetSection("Secret").Value;

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new
                        SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero,
                };
            });
        }

        public static void ConfigureApiVersioning(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApiVersioning(opt =>
            {
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ReportApiVersions = true;
            });
            services.AddVersionedApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddMvcCore().AddApiExplorer();
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<RemoveVersionFromParameter>();
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });

                // TODO: Fix the Docker error on this
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        //public static void ConfigureGoogleDriveService(this IServiceCollection services)
        //{
        //    // Register the GoogleDriveAuthConfig as a singleton
        //    var googleDriveAuthConfig = new GoogleDriveAuthConfig();
        //    services.AddSingleton(googleDriveAuthConfig);

        //    // Register the DriveService as a singleton using the authenticated instance
        //    services.AddSingleton(googleDriveAuthConfig.CreateDriveService());

        //    // Register GoogleDriveService and its interface
        //    services.AddTransient<IGoogleDriveService, GoogleDriveService>();
        //    //services.AddSingleton<DriveService>(provider =>
        //    //{
        //    //    var googleDriveAuthConfig = provider.GetRequiredService<GoogleDriveAuthConfig>();
        //    //    return googleDriveAuthConfig.CreateDriveService();
        //    //});
        //}
        public static void ConfigureGoogleDriveService(this IServiceCollection services)
        {
            // Register the GoogleDriveAuthConfig as a singleton, letting DI resolve its dependencies
            services.AddSingleton<GoogleDriveAuthConfig>();

            // Register the DriveService as a singleton using the authenticated instance
            services.AddSingleton(provider =>
            {
                var googleDriveAuthConfig = provider.GetRequiredService<GoogleDriveAuthConfig>();
                return googleDriveAuthConfig.DriveService; // Use the initialized DriveService
            });

            // Register GoogleDriveService and its interface
            services.AddTransient<IGoogleDriveService, GoogleDriveService>();
        }

        public static void ConfigureRedisServer(this IServiceCollection services, IConfiguration configuration)
        {
            var constring = configuration.GetValue<string>("Redis:ConnectionString");
            Console.WriteLine("redis con string di: " + constring);
            services.AddSingleton(opt =>
            {
                return new RedisService(constring);
            });
        }
    }
}
