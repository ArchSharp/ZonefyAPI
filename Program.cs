using Asp.Versioning.ApiExplorer;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Domain.Entities.Configurations;
using ZonefyDotnet.DI;
using ZonefyDotnet.DTOs;
using ZonefyDotnet.Helpers;
using ZonefyDotnet.Middlewares;
using ZonefyDotnet.Services.HostedService;
using ZonefyDotnet.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtParameters>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSender>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<EmailVerificationUrls>(builder.Configuration.GetSection("EmailVerificationUrls"));
//builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
//builder.Services.Configure<PayStackSettings>(builder.Configuration.GetSection("PayStackSettings"));
builder.Services.Configure<GoogleTwoFactorAuthSettings>(builder.Configuration.GetSection("GoogleTwoFactorAuthSettings"));
builder.Services.Configure<TwilioFnParameters>(builder.Configuration.GetSection("TwilioFnParameters"));
//builder.Services.Configure<SendGridEmailSettings>(builder.Configuration.GetSection("SendGridEmailSettings"));
builder.Services.Configure<RabbitMQMessageBroker>(builder.Configuration.GetSection("RabbitMQMessageBroker"));
builder.Services.Configure<DriveAccess>(builder.Configuration.GetSection("DriveAccess"));
//builder.Services.Configure<string>(builder.Configuration.GetSection("Redis"));

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacContainerModule()));

builder.Services.ConfigureCors();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureSqlContext(builder.Configuration);//add database
builder.Services.ConfigureSwagger();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<RabbitMqConsumerHostedService>();
builder.Services.AddControllers();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.ConfigureJwt(builder.Configuration);
builder.Services.ConfigureApiVersioning(builder.Configuration);
builder.Services.ConfigureMvc();//register automapper
builder.Services.ConfigureRateLimiting();
builder.Services.ConfigureGoogleDriveService();
builder.Services.ConfigureRedisServer(builder.Configuration);
builder.Services.AddSingleton<S3Service>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                       description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseErrorHandler();

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.Run();
