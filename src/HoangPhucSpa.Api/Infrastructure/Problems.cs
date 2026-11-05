using Microsoft.AspNetCore.Http.HttpResults;

namespace HoangPhucSpa.Api.Infrastructure;

/// <summary>
/// Tạo các phản hồi lỗi chuẩn RFC 9457 (ProblemDetails) với thông báo tiếng Việt trả về cho Frontend.
/// </summary>
public static class Problems
{
    public static ProblemHttpResult NotFound(string detail = "Không tìm thấy dữ liệu yêu cầu.") =>
        TypedResults.Problem(detail: detail, statusCode: StatusCodes.Status404NotFound, title: "Không tìm thấy");

    public static ProblemHttpResult Conflict(string detail = "Dữ liệu bị trùng hoặc đã thay đổi.") =>
        TypedResults.Problem(detail: detail, statusCode: StatusCodes.Status409Conflict, title: "Xung đột dữ liệu");

    public static ProblemHttpResult BadRequest(string detail) =>
        TypedResults.Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest, title: "Yêu cầu không hợp lệ");

    public static ProblemHttpResult Forbidden(string detail = "Bạn không có quyền thực hiện thao tác này.") =>
        TypedResults.Problem(detail: detail, statusCode: StatusCodes.Status403Forbidden, title: "Không đủ quyền");

    public static ProblemHttpResult Unauthorized(string detail = "Vui lòng đăng nhập để tiếp tục.") =>
        TypedResults.Problem(detail: detail, statusCode: StatusCodes.Status401Unauthorized, title: "Chưa xác thực");

    public static ValidationProblem Invalid(string field, string error) =>
        TypedResults.ValidationProblem(
            new Dictionary<string, string[]> { [field] = [error] },
            title: "Dữ liệu không hợp lệ");
}
