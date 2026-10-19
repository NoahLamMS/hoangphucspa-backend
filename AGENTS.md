# hoangphucspa-backend — Agent Guide (Quy tắc phát triển toàn diện)

Repo: `hoangphucspa-backend` (ASP.NET Core 10 Minimal API + EF Core + SQL Server).
Solution: `HoangPhucSpa.slnx`.
Cổng dev: 5080 (Scalar API Reference tại `/scalar`, OpenAPI tại `/openapi/v1.json`).

---

## I. NGUYÊN TẮC LÀM VIỆC CỐT LÕI (BẮT BUỘC VỚI MỌI AGENT)

1. **Tuyệt đối KHÔNG tự ý chạy lệnh Git:** Cấm chạy `git commit`, `git push`, `git checkout`, `git reset`. Người dùng tự thực hiện mọi thao tác Git. Hoàn thành việc chỉ đưa ra **Gợi ý lệnh commit**.
2. **Verify trước khi bàn giao:** Mọi thay đổi bắt buộc phải build và test thành công:
   ```bash
   dotnet build -c Release && dotnet test -c Release --no-build
   ```
3. **Phạm vi thay đổi (Change Scope):**
   - Thay đổi đúng trọng tâm, tối thiểu số dòng code cần thiết. Không tiện tay refactor code ngoài vùng yêu cầu.
   - Thấy lỗi khác ngoài yêu cầu: Báo lại cho người dùng, không tự ý sửa kèm.
4. **Bảo mật:**
   - Tuyệt đối không commit chuỗi kết nối, secret hay mật khẩu vào git. Cấu hình nhạy cảm đặt trong User Secrets (`dotnet user-secrets`).
   - Luôn sanitize mã HTML nhận từ CMS qua `ContentSanitizer.Create().Sanitize(...)` trước khi lưu vào database.
5. **Không hard-code dữ liệu thương hiệu:** Tên site, hotline, liên hệ, chi nhánh lưu trong database (`SiteSettings`, `Branches`).

---

## II. KIẾN TRÚC & CẤU TRÚC CODE (VERTICAL SLICE)

Mỗi tính năng nằm trọn trong một thư mục feature:
- `XxxEndpoints.cs`: Đăng ký route và handler static.
- `XxxDtos.cs`: Chứa các `record` DTO.
- Logic chỉ tách thành service riêng khi được tái sử dụng ở từ 2 nơi trở lên.

```text
src/
├── HoangPhucSpa.Data/
│   ├── Entities/                 # Service, SubService, Post, Page, Slide, AppUser...
│   ├── Configurations/           # IEntityTypeConfiguration<T> cho từng entity
│   ├── Migrations/               # EF Core Migrations
│   ├── Text/                     # SlugHelper, ContentSanitizer
│   ├── AppDbContext.cs           # DbContext chính
│   ├── UtcDateTimeConverter.cs   # Gắn UTC cho mọi DateTime
│   ├── AuditSaveChangesInterceptor.cs # Tự điền CreatedAt/UpdatedAt
│   └── ICurrentUser.cs           # Lấy user hiện tại
└── HoangPhucSpa.Api/
    ├── Infrastructure/           # Options, Problems, PagedResult, Paths, RateLimiting, SecurityHeaders
    ├── Features/                 # Health, Site, Home, Services, Posts, Pages, Admin, Auth...
    │   └── <Feature>/
    │       ├── <Feature>Endpoints.cs
    │       └── <Feature>Dtos.cs
    └── Program.cs
```

---

## III. QUY CHUẨN API & MINIMAL API

1. **Handler trả `TypedResults` tường minh:**
   - Dùng `Results<Ok<T>, ProblemHttpResult, ...>` đầy đủ để OpenAPI mô tả chính xác kiểu dữ liệu và web/admin sinh type đúng (`schema.d.ts`).
2. **Không trả Entity thô ra ngoài API:**
   - Mọi truy vấn đọc phải `Select` vào DTO.
   - Luôn thêm `AsNoTracking()` cho truy vấn đọc để tối ưu hiệu năng và giải phóng bộ nhớ.
3. **Xử lý lỗi chuẩn RFC 7807 (ProblemDetails):**
   - Dùng helper `Problems.*` (`NotFound`, `Conflict`, `BadRequest`, `Invalid`).
   - Thông báo lỗi bằng tiếng Việt, rõ ràng.
   - Không dùng `throw Exception` cho luồng nghiệp vụ thông thường.
4. **Thời gian UTC:**
   - Toàn bộ thời gian lưu trong database là `datetime2` chuẩn UTC (đã cấu hình qua `UtcDateTimeConverter`).
   - Lấy thời gian từ `TimeProvider` được inject, không gọi `DateTime.UtcNow` trực tiếp để phục vụ kiểm thử.
5. **CancellationToken:**
   - Mọi endpoint handler phải nhận `CancellationToken ct` và truyền xuống tất cả lệnh EF Core bất đồng bộ.
6. **Đường dẫn chuẩn:**
   - Đường dẫn công khai (`path`, `canonicalPath`) chỉ được dựng ở class `Paths` (`Infrastructure/Paths.cs`), dùng chung cho mọi feature.

---

## IV. DATABASE & EF CORE RULES

1. **Code-First Migration:** Thay đổi entity phải đi kèm migration:
   ```bash
   dotnet ef migrations add <TenMigration> -p src/HoangPhucSpa.Data -s src/HoangPhucSpa.Api -o Migrations
   ```
2. **Xóa mềm (Soft Delete):**
   - Các bảng nội dung kế thừa `ISoftDeletable` (`DeletedAt`).
   - Bắt buộc có Global Query Filter `b.HasQueryFilter(x => x.DeletedAt == null)`.
3. **Chống ghi đè đồng thời (Concurrency):**
   - Các bảng quản trị có cột `RowVersion rowversion`.
   - Khi cập nhật qua admin, kiểm tra version và bắt `DbUpdateConcurrencyException` trả về 409 Conflict.
4. **Tìm kiếm (SearchText):**
   - Dùng `SlugHelper.ToSearchText()` để sinh chuỗi tìm kiếm không dấu, chữ thường lưu vào cột `SearchText`.

---

## V. TESTING & GOLDEN TESTS

1. **Cảnh báo xem như lỗi:** Solution bật `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`. Build không được có bất kỳ warning nào.
2. **Nguồn chân lý Golden Test (Slug):**
   - File `hoangphucspa-backend/tests/golden/slug-golden.json` là nguồn duy nhất cho cả C# và TypeScript. Thay đổi thuật ngữ hoặc slug phải cập nhật file này trước rồi đồng bộ sang `hoangphucspa-admin`.
3. **Bộ test bắt buộc:**
   - `SlugHelperTests`: Khớp 100% với dữ liệu `tests/golden/slug-golden.json`.
   - `ContentSanitizerTests`: Kiểm tra chặn XSS (chặn script, thẻ độc, cho phép iframe YouTube/Maps).
   - `HealthEndpointTests`: Kiểm tra liveness và readiness (kiểm tra DB).

---

## VI. FEATURE WORKFLOW PLAYBOOK

### Luồng I: Xây dựng Endpoint / Feature mới
1. **I-0. Schema:** Định nghĩa Entity trong `HoangPhucSpa.Data/Entities/` và cấu hình Fluent API trong `Configurations/`.
2. **I-1. Migration:** Tạo EF Migration (nếu có thay đổi bảng).
3. **I-2. DTOs:** Khai báo các `record` DTO trong `Features/<Feature>/<Feature>Dtos.cs`.
4. **I-3. Endpoints:** Viết handler static trả về `TypedResults` trong `Features/<Feature>/<Feature>Endpoints.cs`.
5. **I-4. Test:** Viết integration test hoặc unit test cho logic mới.
6. **I-5. Verify:** Chạy `dotnet build -c Release && dotnet test -c Release --no-build`.

### Luồng J: Sửa Endpoint hiện có
1. **J-0. Định vị:** Tìm đúng class endpoint và DTO liên quan.
2. **J-1. Sửa phạm vi hẹp:** Tránh thay đổi hợp đồng API nếu không bắt buộc (để tránh làm vỡ giao diện Web và Admin).
3. **J-2. Verify:** Chạy lại toàn bộ bộ test.

---

## VII. GỢI Ý COMMIT MESSAGE

Khi hoàn thành task, luôn gợi ý commit message theo định dạng:
```text
<type>(<module>): <mô tả ngắn>

- Gạch đầu dòng thay đổi Schema / Database (nếu có)
- Gạch đầu dòng thay đổi logic API / Endpoint
```
