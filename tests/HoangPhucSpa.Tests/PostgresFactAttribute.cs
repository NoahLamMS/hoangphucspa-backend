namespace HoangPhucSpa.Tests;

/// <summary>
/// Đánh dấu các test cần kết nối PostgreSQL thật. Tự động bỏ qua (Skip) nếu chưa cấu hình biến môi trường POSTGRES_CONNECTION_STRING.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class PostgresFactAttribute : FactAttribute
{
    public const string ConnectionStringVariable = "POSTGRES_CONNECTION_STRING";

    public PostgresFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(ConnectionStringVariable)))
        {
            Skip = $"Thiếu biến môi trường {ConnectionStringVariable} để chạy test kết nối PostgreSQL thật.";
        }
    }
}
