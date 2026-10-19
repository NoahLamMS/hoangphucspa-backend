using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HoangPhucSpa.Tests;

/// <summary>
/// Host test của API. Cấu hình được tiêm bằng UseSetting vì user-secrets không nạp trong test host.
/// </summary>
public sealed class ApiFactory(string? connectionString = null) : WebApplicationFactory<Program>
{
    // Chuỗi GIẢ, chỉ dùng cho test (không phải secret thật): đủ 32 ký tự để qua validation.
    private const string TestOnlyRevalidateSecret = "test-only-not-a-real-secret-0123456789abcdef";

    // Chuỗi kết nối GIẢ trỏ cổng không có dịch vụ nào: dùng để kiểm tra trường hợp DB không truy cập được.
    private const string UnreachableConnectionString =
        "Host=localhost;Port=1;Database=HoangPhucSpa;Username=postgres;Password=fake;Timeout=2;Command Timeout=2;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("Web:RevalidateSecret", TestOnlyRevalidateSecret);
        builder.UseSetting("ConnectionStrings:Default", connectionString ?? UnreachableConnectionString);
    }
}
