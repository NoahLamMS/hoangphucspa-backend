# hoangphucspa-backend

API của dự án HoangPhucSpa: ASP.NET Core 10 Minimal API + EF Core + SQL Server. Đây là nơi duy nhất chạm database; web công khai (`hoangphucspa-web`) và trang quản trị (`hoangphucspa-admin`) đều gọi API này.

Trạng thái hiện tại: **skeleton**. Chỉ có health check, OpenAPI + Scalar (môi trường Development), cấu hình có kiểm tra khi khởi động, log Serilog. Chưa có entity, migration, auth hay nghiệp vụ.

## Cấu trúc

```
hoangphucspa-backend/
├── HoangPhucSpa.slnx
├── Directory.Build.props        # Nullable, ImplicitUsings, InvariantGlobalization=false
├── Directory.Packages.props     # version NuGet tập trung, cố định
├── global.json                  # SDK 10.0.301
├── src/
│   ├── HoangPhucSpa.Api/        # Minimal API (Program.cs, Features/, Infrastructure/)
│   └── HoangPhucSpa.Data/       # AppDbContext (chưa có bảng)
├── tests/HoangPhucSpa.Tests/    # xUnit 2.x + WebApplicationFactory
└── infra/                       # Docker Compose cho dev: SQL Server + Mailpit
```

## Cổng

| Dịch vụ | Địa chỉ |
|---|---|
| API | http://localhost:5080 |
| Scalar (chỉ Development) | http://localhost:5080/scalar |
| OpenAPI JSON (chỉ Development) | http://localhost:5080/openapi/v1.json |
| SQL Server (`hps-sql`) | 127.0.0.1:1433 |
| Mailpit web (`hps-mailpit`) | http://localhost:8025 |
| Mailpit SMTP | 127.0.0.1:1025 |

## Chạy lần đầu

```bash
# 1. Docker: SQL Server + Mailpit
cd infra
cp .env.example .env            # điền MSSQL_SA_PASSWORD (chỉ lần đầu tạo volume)
docker compose -f docker-compose.dev.yml up -d
docker inspect -f '{{.State.Health.Status}}' hps-sql   # đợi "healthy"
./create-dev-db.sh              # tạo database rỗng HoangPhucSpa
cd ..

# 2. Secret dev (user-secrets, xem mục dưới)

# 3. Chạy API
dotnet watch --project src/HoangPhucSpa.Api
curl http://localhost:5080/api/v1/health/live
curl http://localhost:5080/api/v1/health
```

Dừng container nhưng giữ dữ liệu: `docker compose -f infra/docker-compose.dev.yml stop`. Chỉ dùng `down -v` khi chắc chắn muốn xoá sạch dữ liệu SQL.

## Endpoint

| Method | Đường dẫn | Ý nghĩa |
|---|---|---|
| GET | `/api/v1/health/live` | Liveness, không đụng DB. Luôn `{"status":"Healthy"}` 200. |
| GET | `/api/v1/health` | Readiness, kiểm tra kết nối DB. `Healthy` 200, `Unhealthy` 503. |

## Cấu hình và secret

Giá trị không nhạy cảm nằm ở `appsettings*.json`. Giá trị nhạy cảm nằm ở **user-secrets** (không nằm trong repo). Các khóa cần có khi chạy Development:

- `ConnectionStrings:Default`
- `Media:RootPath`
- `DataProtection:KeysPath`
- `Web:RevalidateSecret` (tối thiểu 32 ký tự; web dùng cùng giá trị này ở biến `REVALIDATE_SECRET`)

Nạp qua stdin để giá trị không lộ trong lịch sử shell (làm trong cùng một phiên shell có sẵn các biến, không in ra):

```bash
dotnet user-secrets init --project src/HoangPhucSpa.Api
printf '{"ConnectionStrings:Default":"...","Media:RootPath":"...","DataProtection:KeysPath":"...","Web:RevalidateSecret":"..."}' \
  | dotnet user-secrets set --project src/HoangPhucSpa.Api
```

`App:*` và `Web:*` trong `appsettings.json` để rỗng. Ngoài Development, thiếu cấu hình thì API dừng ngay lúc khởi động (`ValidateOnStart`).

## Kiểm thử

```bash
dotnet build -c Release
dotnet test -c Release                       # test cần DB sẽ bị bỏ qua (3 pass, 1 skipped)

# Chạy cả test cần SQL Server thật (cần container hps-sql đang chạy):
( set -a; . infra/.env; set +a; dotnet test -c Release )
```

Test tiêm cấu hình bằng `UseSetting` trong `ApiFactory` (user-secrets không nạp trong test host). Giá trị trong test là chuỗi giả, chỉ dùng cho test.

## Ghi chú

- `dotnet-ef` cài bằng Homebrew cần `export DOTNET_ROOT=/opt/homebrew/opt/dotnet/libexec` mới tìm thấy runtime.
- Database `HoangPhucSpa` hiện rỗng: migration đầu tiên làm ở M2 (BE-2.2).
- `System.Security.Cryptography.Pkcs` được ghim lên bản đã vá trong `Directory.Packages.props` (package gián tiếp). Bỏ ghim khi bản EF Core/SqlClient kéo vào đã vá.
