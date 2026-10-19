namespace HoangPhucSpa.Tests;

/// <summary>
/// Test cần PostgreSQL / Neon thật. Bỏ qua khi thiếu biến môi trường POSTGRES_CONNECTION_STRING.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class PostgresFactAttribute : FactAttribute
{
    public const string ConnectionStringVariable = "POSTGRES_CONNECTION_STRING";

    public PostgresFactAttribute()
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(ConnectionStringVariable)))
            Skip = $"Thiếu biến môi trường {ConnectionStringVariable} để chạy test DB PostgreSQL thật.";
    }
}
