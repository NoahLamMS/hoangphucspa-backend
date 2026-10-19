namespace HoangPhucSpa.Tests;

/// <summary>
/// Test cần SQL Server thật. Bỏ qua khi thiếu biến môi trường MSSQL_SA_PASSWORD
/// (xunit 2.x không có skip lúc chạy, nên đặt Skip ngay ở attribute).
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class SqlServerFactAttribute : FactAttribute
{
    public const string PasswordVariable = "MSSQL_SA_PASSWORD";

    public SqlServerFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(PasswordVariable)))
            Skip = $"Thiếu biến môi trường {PasswordVariable} (nạp từ infra/.env để chạy test này).";
    }
}
