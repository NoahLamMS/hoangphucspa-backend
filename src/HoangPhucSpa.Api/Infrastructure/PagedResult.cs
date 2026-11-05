using Microsoft.EntityFrameworkCore;

namespace HoangPhucSpa.Api.Infrastructure;

/// <summary>
/// Khung dữ liệu phân trang chuẩn trả về cho danh sách API.
/// </summary>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages)
{
    public PagedResult<TOut> Map<TOut>(Func<T, TOut> map) =>
        new([.. Items.Select(map)], Page, PageSize, TotalItems, TotalPages);
}

public static class PagedResultExtensions
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 50;

    /// <summary>
    /// Thực thi phân trang trực tiếp ở tầng cơ sở dữ liệu (COUNT + OFFSET/LIMIT).
    /// Lưu ý: Truy vấn truyền vào bắt buộc phải có OrderBy(...).ThenBy(x => x.Id) trước khi gọi hàm này.
    /// </summary>
    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        this IQueryable<T> query,
        int page = DefaultPage,
        int pageSize = DefaultPageSize,
        CancellationToken ct = default)
    {
        page = Math.Max(DefaultPage, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var totalItems = await query.CountAsync(ct);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling((double)totalItems / pageSize);

        // Nếu không có bản ghi nào hoặc số trang vượt quá tổng số trang thì không cần truy vấn thêm lần 2.
        var items = totalItems == 0 || page > totalPages
            ? []
            : await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return new PagedResult<T>(items, page, pageSize, totalItems, totalPages);
    }
}
