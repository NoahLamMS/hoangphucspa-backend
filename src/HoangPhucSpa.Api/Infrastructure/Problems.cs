using Microsoft.AspNetCore.Http.HttpResults;

namespace HoangPhucSpa.Api.Infrastructure;

public static class Problems
{
    public static ProblemHttpResult NotFound(string detail = "Không tìm thấy dữ liệu.") =>
        TypedResults.Problem(detail: detail, statusCode: StatusCodes.Status404NotFound, title: "Không tìm thấy");

    public static ProblemHttpResult Conflict(string detail = "Dữ liệu bị xung đột.") =>
        TypedResults.Problem(detail: detail, statusCode: StatusCodes.Status409Conflict, title: "Xung đột dữ liệu");

    public static ProblemHttpResult BadRequest(string detail) =>
        TypedResults.Problem(detail: detail, statusCode: StatusCodes.Status400BadRequest, title: "Yêu cầu không hợp lệ");

    public static ValidationProblem Invalid(string field, string error) =>
        TypedResults.ValidationProblem(new Dictionary<string, string[]> { [field] = [error] });
}
