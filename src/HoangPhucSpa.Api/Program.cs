using System.Text.Json;
using System.Text.Json.Serialization;
using HoangPhucSpa.Api.Features.Health;
using HoangPhucSpa.Api.Infrastructure;
using HoangPhucSpa.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(k =>
{
    k.AddServerHeader = false;                              // không lộ "Server: Kestrel"
    k.Limits.MaxRequestBodySize = 12 * 1024 * 1024;         // file tối đa 10 MB + dư cho multipart
});

builder.Services.AddSerilog((sp, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Cấu hình có kiểu; thiếu giá trị bắt buộc thì dừng ngay khi khởi động
builder.Services.AddOptions<AppOptions>().BindConfiguration("App").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<WebOptions>().BindConfiguration("Web").ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<MediaOptions>().BindConfiguration("Media");

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<AuditSaveChangesInterceptor>();
builder.Services.AddSingleton<MediaUrlBuilder>();
builder.Services.AddSingleton(_ => HoangPhucSpa.Data.Text.ContentSanitizer.Create());

// Đọc connection string trong lambda (lazy) để WebApplicationFactory.UseSetting ghi đè được
builder.Services.AddDbContext<AppDbContext>((sp, o) => o
    .UseSqlServer(builder.Configuration.GetConnectionString("Default"))
    .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

builder.Services.AddProblemDetails();
builder.Services.AddValidation();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
builder.Services.AddAppRateLimiting();
builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));

// Reverse proxy đứng trước API: chỉ tin X-Forwarded-* từ mạng này
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownIPNetworks.Add(System.Net.IPNetwork.Parse(builder.Configuration["Proxy:TrustedNetwork"] ?? "172.30.0.0/24"));
});

// Khoá mã hoá cookie: lưu ra thư mục để restart không làm mất khoá
var dataProtection = builder.Services.AddDataProtection().SetApplicationName("HoangPhucSpa");
if (builder.Configuration["DataProtection:KeysPath"] is { Length: > 0 } keysPath)
    dataProtection.PersistKeysToFileSystem(new DirectoryInfo(keysPath));

builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

app.UseForwardedHeaders();           // phải đứng đầu để IP/scheme đúng cho mọi middleware sau
app.UseExceptionHandler();           // lỗi không bắt → 500 ProblemDetails, không lộ stack trace
app.UseStatusCodePages();            // 404/405 không có body → ProblemDetails
app.UseSerilogRequestLogging();
app.UseSecurityHeaders();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                // /openapi/v1.json (web và admin sinh type từ đây)
    app.MapScalarApiReference();     // /scalar
    app.UseDevMediaFiles();          // phục vụ static files từ media root
}

var api = app.MapGroup("/api/v1");
api.MapHealthEndpoints();

app.Run();

public partial class Program;
