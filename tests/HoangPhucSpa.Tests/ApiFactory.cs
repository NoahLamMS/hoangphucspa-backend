using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HoangPhucSpa.Tests;

/// <summary>
/// Khởi tạo host kiểm thử tích hợp (Integration Test) cho API.
/// Dùng UseSetting để bơm cấu hình mẫu vì user-secrets không tự nạp trong môi trường test.
/// </summary>
public sealed class ApiFactory(string? connectionString = null) : WebApplicationFactory<Program>
{
    // Chuỗi khoá giả chỉ dùng cho môi trường test (đủ 32 ký tự để vượt qua bước kiểm tra cấu hình).
    private const string TestOnlyRevalidateSecret = "test-only-not-a-real-secret-0123456789abcdef";

    // Chuỗi kết nối trỏ tới cổng không hoạt động để giả lập tình huống mất kết nối DB.
    private const string UnreachableConnectionString =
        "Host=localhost;Port=1;Database=HoangPhucSpa;Username=postgres;Password=fake;Timeout=2;Command Timeout=2;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("Web:RevalidateSecret", TestOnlyRevalidateSecret);
        builder.UseSetting("ConnectionStrings:Default", connectionString ?? UnreachableConnectionString);
    }
}
