using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HoangPhucSpa.Data;

/// <summary>
/// Đảm bảo mọi giá trị DateTime khi ghi xuống và đọc lên từ PostgreSQL đều mang DateTimeKind.Utc.
/// </summary>
public sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
    v => v.Kind == DateTimeKind.Local ? v.ToUniversalTime() : v,
    v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
