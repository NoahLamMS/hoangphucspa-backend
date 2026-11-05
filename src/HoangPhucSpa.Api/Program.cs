using System.Text.Json;
using System.Text.Json.Serialization;
using HoangPhucSpa.Api.Features.Health;
using HoangPhucSpa.Api.Infrastructure;
using HoangPhucSpa.Data;
using HoangPhucSpa.Data.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

const long MaxRequestBodyBytes = 12 * 1024 * 1024;
const string DefaultTrustedNetwork = "172.30.0.0/24";

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.AddServerHeader = false;
    kestrel.Limits.MaxRequestBodySize = MaxRequestBodyBytes;
});

builder.Services.AddSerilog((_, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddOptions<AppOptions>().BindConfiguration("App").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<WebOptions>().BindConfiguration("Web").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<MediaOptions>().BindConfiguration("Media");

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<AuditSaveChangesInterceptor>();
builder.Services.AddSingleton<MediaUrlBuilder>();
builder.Services.AddSingleton(_ => ContentSanitizer.Create());

builder.Services.AddDbContext<AppDbContext>((sp, options) => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Default"))
    .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

builder.Services.AddProblemDetails();
builder.Services.AddValidation();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
builder.Services.AddAppRateLimiting();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(builder.Configuration["Proxy:TrustedNetwork"] ?? DefaultTrustedNetwork));
});

var dataProtection = builder.Services.AddDataProtection().SetApplicationName("HoangPhucSpa");
if (builder.Configuration["DataProtection:KeysPath"] is { Length: > 0 } keysPath)
{
    dataProtection.PersistKeysToFileSystem(new DirectoryInfo(keysPath));
}

builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseSerilogRequestLogging();
app.UseSecurityHeaders();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseDevMediaFiles();
}

var api = app.MapGroup("/api/v1");
api.MapHealthEndpoints();

app.Run();

public partial class Program;
