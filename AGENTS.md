# hoangphucspa-backend — Project Rules & Agent Guide

Repo: `hoangphucspa-backend` (ASP.NET Core 10 Minimal API + EF Core 10 + PostgreSQL (Neon)).
Solution: `HoangPhucSpa.slnx`.
Dev Port: `5080` (Scalar API Reference at `/scalar`, OpenAPI at `/openapi/v1.json`).

Rules are grouped by concern and numbered within their group (`A1`, `C3`, ...). Always reference a rule by its group-prefixed ID.

| Group | Concern |
| :--- | :--- |
| [A](#a--code-style-cleanliness--architecture) | Code Style, Cleanliness & Architecture |
| [B](#b--minimal-api-validation--error-handling) | Minimal API, Validation & Error Handling |
| [C](#c--ef-core-query-performance--postgresql) | EF Core Query, Performance & PostgreSQL |
| [D](#d--persistence-concurrency--side-effects) | Persistence, Concurrency & Side Effects |
| [E](#e--business-logic-consistency) | Business Logic Consistency |
| [F](#f--testing-migrations--git-workflow) | Testing, Migrations & Git Workflow |

---

## A — Code Style, Cleanliness & Architecture

**A1. Scope of Changes & Zero Dead Code**:
- Only modify files relevant to the current task. Do not refactor unrelated files without asking.
- Never leave dead code, unused parameters, commented-out code blocks, or unused `using` directives. Every line of code must have a clear purpose.

**A2. Language Policy (English Code & Routes / Vietnamese Messages & Comments)**:
- **100% English for Code & URLs**: All namespaces, class/method/property/parameter/variable names, Entities, DTOs, route URLs (`/services/{slug}`, `/news/{id}/{slug}`, `/library/{id}/{slug}` — never legacy Vietnamese URLs like `/dich-vu` or `/tin-tuc`), configuration keys, database table/column names, and Git commit messages must be written in modern English.
- **Vietnamese for Frontend Error Messages**: All user-facing error and validation messages returned to the Frontend (`ProblemDetails` `title` and `detail` via `Problems.*`, validation errors) must be written in natural, clear Vietnamese so the UI can display them directly.
- **Vietnamese for Code Comments**: Write XML documentation (`/// <summary>`) and inline comments (`//`) in clear Vietnamese to explain business logic and Backend architecture concepts.

**A3. Vertical Slice Architecture & One Responsibility Per File (< 300 Lines)**:
- Each file must have a single responsibility and stay under **300 lines**.
- **Entities**: One entity class per file in `src/HoangPhucSpa.Data/Entities/<Entity>.cs`.
- **Configurations**: One `IEntityTypeConfiguration<T>` per file in `src/HoangPhucSpa.Data/Configurations/<Entity>Configuration.cs`. Never bundle multiple entity configurations into a single file.
- **Features**: Each feature lives in `src/HoangPhucSpa.Api/Features/<Feature>/` with:
  - `<Feature>Endpoints.cs`: Route registration and `static` handler methods.
  - `<Feature>Dtos.cs`: Immutable `sealed record` request/response DTOs.

```text
src/
├── HoangPhucSpa.Data/
│   ├── Entities/                 # One file per entity (Service.cs, Post.cs, ...)
│   ├── Configurations/           # One file per configuration (ServiceConfiguration.cs, ...)
│   ├── Migrations/               # EF Core PostgreSQL Migrations
│   ├── Text/                     # SlugHelper.cs, ContentSanitizer.cs
│   ├── AppDbContext.cs           # Primary DbContext
│   ├── UtcDateTimeConverter.cs   # Ensures UTC DateTime kind on read/write
│   ├── AuditSaveChangesInterceptor.cs # Populates audit fields & RowVersion token
│   └── ICurrentUser.cs           # Current authenticated user abstraction
└── HoangPhucSpa.Api/
    ├── Infrastructure/           # Options, Problems, PagedResult, Paths, MediaUrlBuilder, RateLimiting, SecurityHeaders
    ├── Features/                 # Health, Site, Home, Services, Posts, Pages, Testimonials, Faqs, Search, Sitemap, Redirects, Auth, Admin
    │   └── <Feature>/
    │       ├── <Feature>Endpoints.cs
    │       └── <Feature>Dtos.cs
    └── Program.cs
```

**A4. Single Level of Abstraction Per Method (SLAP)**:
- Endpoint handlers should read like a clear sequence of high-level steps: validate input → query/load → apply domain changes → persist → trigger post-commit side effects → return `TypedResults`.
- If a block inside a handler requires a section comment to explain what it does (e.g., `// update slug redirects`), extract it into a descriptive `private static` helper method (`UpdateSlugRedirects(...)`).

**A5. Naming Conventions & No Magic Literals**:
- `PascalCase` for types, methods, properties, constants, and enum members.
- `camelCase` for local variables and parameters; `_camelCase` for private fields.
- `Async` suffix on async helper/service methods (endpoint handler methods registered in `MapGet`/`MapPost` may omit `Async` if concise, e.g., `GetServices`, `CreateService`).
- Boolean properties/variables must start with `Is`, `Has`, `Can`, or `Should` (`IsVisible`, `IsHot`, `IsSystem`, `MustChangePassword`).
- Extract numeric literals (other than `0`, `1`, `-1`) and repeated policy/key strings into named `const` or `static readonly` fields (e.g., `RateLimitPolicies.Public`, `PagedResultExtensions.MaxPageSize`).

**A6. Pattern Matching for Null & `sealed` Leaf Classes**:
- Always use `is null` and `is not null` (or pattern matching `is { Length: > 0 }`) instead of `== null` or `!= null`.
- Mark all DTO records, services, interceptors, options, and filters as `sealed` unless explicitly designed as a base class (`AuditableEntity`).

**A7. Shared Logic Threshold (Rule of Two)**:
- Keep feature-specific logic inside the feature folder.
- As soon as logic (validation, normalization, URL building, HTML extraction) is needed by **2 or more features**, extract it into a shared helper in `Infrastructure/` or `HoangPhucSpa.Data/Text/`. Never copy-paste regexes or mapping rules across features.

**A8. `TimeProvider` & `CancellationToken` Placement**:
- Always inject `TimeProvider` and call `clock.GetUtcNow().UtcDateTime` instead of `DateTime.UtcNow` so time-dependent logic is deterministic in tests.
- Every async endpoint handler and database helper must accept `CancellationToken ct` as its **final parameter** and pass it to all async EF Core / IO calls.

---

## B — Minimal API, Validation & Error Handling

**B1. Explicit `TypedResults` & OpenAPI Metadata**:
- Every endpoint handler must return an explicit `TypedResults` type (`Ok<T>`, `Results<Ok<T>, ProblemHttpResult>`, `Results<Created<T>, ProblemHttpResult, ValidationProblem>`, etc.) — never untyped `IResult` (`Results.Ok(...)`). This guarantees accurate OpenAPI schema generation for `hoangphucspa-web` and `hoangphucspa-admin`.
- Every endpoint group must declare `.WithTags(...)`, and every endpoint must declare `.WithName(...)` and `.WithSummary(...)`.

**B2. Never Expose Entities in API Contracts**:
- Never accept or return EF Core entity classes in endpoints.
- Always bind requests to dedicated request DTOs and project responses into `sealed record` DTOs.

**B3. Standardized RFC 9457 `ProblemDetails` (`Problems.*`)**:
- Never throw exceptions for expected business or validation failures (missing records, duplicate slugs, concurrency conflicts, invalid foreign keys).
- Always return structured errors using `Problems.NotFound(...)`, `Problems.Conflict(...)`, `Problems.BadRequest(...)`, `Problems.Forbidden(...)`, `Problems.Unauthorized(...)`, or `Problems.Invalid(field, error)`.

**B4. Mass-Assignment Protection & Server-Assigned Fields**:
- Request DTOs must only contain user-editable fields.
- Server-controlled properties (`Id`, `ViewCount`, `CreatedAt`, `UpdatedAt`, `CreatedById`, `SearchText`, `HitCount`) must never be present on Create/Update request DTOs.

**B5. Mandatory Foreign Key Existence Checks**:
- Whenever a request includes a foreign key ID (e.g., `ServiceId` on `SubService`, `CategoryId` on `Post`, `CoverImageId` on `Album`), verify its existence using `AnyAsync` (and verify matching `PostKind` for `CategoryId`) before saving. Return `Problems.Invalid(...)` or `Problems.NotFound(...)` if missing instead of letting PostgreSQL throw a foreign key constraint exception.

**B6. Nested Related Objects in Response DTOs**:
- Group fields belonging to a related entity into a nested DTO (e.g., `CategorySummaryDto? Category`, `ParentServiceDto Parent`, `SeoDto Seo`) rather than flattening `CategoryId`, `CategoryName`, `CategorySlug` at the top level of a detail response.

---

## C — EF Core Query, Performance & PostgreSQL

**C1. `AsNoTracking()` & SQL Projection (`.Select(...)`) for Read Queries**:
- Every read-only query (`GET` list, detail, home, site, search, sitemap) must start with `.AsNoTracking()` and project directly into the target DTO (or minimal anonymous type) via `.Select(...)` before calling `.ToListAsync(ct)` or `.FirstOrDefaultAsync(ct)`.
- Never load full entity graphs (especially large `ContentHtml` columns) into memory when only summary fields are needed.

**C2. Use `AnyAsync()` Instead of `CountAsync() > 0`**:
- Always use `await db.Set.AnyAsync(predicate, ct)` for existence and uniqueness checks so PostgreSQL executes a fast `EXISTS (...)` query instead of counting all matching rows.

**C3. Database-Level Pagination & Deterministic Ordering**:
- Always paginate at the database level using `.ToPagedAsync(page, pageSize, ct)`.
- **Every query that uses `Take` or `.ToPagedAsync(...)` MUST have a deterministic `OrderBy` ending with `.ThenBy(x => x.Id)`** (e.g., `.OrderBy(x => x.SortOrder).ThenBy(x => x.Id)` or `.OrderByDescending(x => x.PublishedAt).ThenByDescending(x => x.Id)`). Without a unique tie-breaker, PostgreSQL returns arbitrary row orders across pages when `SortOrder` or `PublishedAt` values are equal.

**C4. Never Run Concurrent Queries on the Same Scoped `AppDbContext`**:
- `AppDbContext` is not thread-safe. Always `await` each EF Core query sequentially.
- Never use `Task.WhenAll`, `Parallel.ForEachAsync`, or start a second query before awaiting the first on the same `AppDbContext`.

**C5. Soft Delete (`ISoftDeletable`) & Scoped `.IgnoreQueryFilters()`**:
- Soft-deletable entities (`Service`, `SubService`, `Post`) have a global query filter `HasQueryFilter(x => x.DeletedAt == null)`.
- Filtered indexes on soft-deletable entities must include `"DeletedAt" IS NULL` in `.HasFilter(...)`.
- Only use `.IgnoreQueryFilters()` in explicit Admin Trash / Restore / Permanent Delete endpoints. When using `.IgnoreQueryFilters()`, manually re-apply any other required predicates.

**C6. Write-Time Normalization & Case Sensitivity**:
- Normalize canonical fields on **write** before saving:
  - `Slug`: lowercase, trimmed, validated via `SlugHelper.IsValid(slug)`.
  - `Redirect.FromPath`: lowercase, leading `/`, no trailing `/` (except root).
  - Strings (`Name`, `Title`, `Email`): `.Trim()`.
- Never call `.ToLower()` or `.ToUpper()` on an indexed DB column inside a `.Where(...)` clause (which bypasses standard B-tree indexes); store the canonical lowercase value on write and compare with `==`.
- For in-memory C# string comparisons, always use `StringComparison.OrdinalIgnoreCase`.

---

## D — Persistence, Concurrency & Side Effects

**D1. Optimistic Concurrency via `RowVersion`**:
- All `AuditableEntity` tables include `byte[] RowVersion` configured with `.IsConcurrencyToken()`.
- `AuditSaveChangesInterceptor` automatically generates a new `RowVersion` (`Guid.NewGuid().ToByteArray()`) whenever an entity is `Added` or `Modified`.
- In Admin `PUT` handlers, decode the client's base64 `RowVersion`, assign it to `db.Entry(entity).Property(x => x.RowVersion).OriginalValue`, and catch `DbUpdateConcurrencyException` to return `Problems.Conflict("Dữ liệu đã bị thay đổi bởi người dùng khác. Vui lòng tải lại trang và thử lại.")`.

**D2. Narrow `DbUpdateException` Catching (PostgreSQL `23505`)**:
- Never write a bare `catch (DbUpdateException)` that swallows all database errors.
- Only catch unique constraint violations specifically using an exception filter:
  ```csharp
  catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
  {
      return Problems.Conflict("Dữ liệu bị trùng lặp (đường dẫn slug hoặc mã định danh đã tồn tại).");
  }
  ```

**D3. Execute Side Effects Only AFTER `SaveChangesAsync` Succeeds**:
- External side effects — enqueuing cache revalidation tags (`RevalidationQueue`), sending emails, or dispatching notifications — must execute **after** `await db.SaveChangesAsync(ct)` (and after `await transaction.CommitAsync(ct)` when using an explicit transaction). Never trigger side effects before the database write is committed.

**D4. Mandatory HTML Sanitization & `SearchText` Generation on Write**:
- Every write path that accepts HTML content (`ContentHtml`, `Summary`, `AnswerHtml`) must sanitize it using `sanitizer.Sanitize(html)` (`ContentSanitizer`) before saving.
- Every write path on searchable entities (`Service`, `SubService`, `Post`, `PostCategory`, `Page`, `Faq`, `Album`) must recompute `SearchText` using `SlugHelper.ToSearchText(...)`.

---

## E — Business Logic Consistency

**E1. Consistent Guards Across All Mutating Endpoints**:
- If an entity has a mutation rule (e.g., `Page` records cannot be deleted; `PostCategory` with `IsSystem == true` or existing posts cannot be deleted; `Service` with existing `SubServices` cannot be deleted; last active `Admin` user cannot be locked/demoted/deleted), enforce that guard consistently across every endpoint that mutates the entity.

**E2. Complete State Transitions in a Single Save**:
- When an operation changes state (e.g., changing a `Service` or `PostCategory` slug), update the entity, create/update the corresponding `Redirect` entry (preventing redirect chains), and write the `AuditLog` within the same unit of work.

**E3. Consistent Visibility Filtering Across All Public Endpoints**:
- Public endpoints (`List`, `Detail`, `Home`, `Site`, `Search`, `Sitemap`) must apply the exact same visibility rules:
  - Top-level entities must have `IsVisible == true` (and `DeletedAt == null` via global query filter).
  - Child entities (`SubService`, `Post` under a category) must verify both their own `IsVisible == true` and their parent's `IsVisible == true` where applicable. A hidden parentService must hide all its `SubServices` in detail, search, and sitemap endpoints.

**E4. Never Hard-Code Brand or Contact Information**:
- Site name, tagline, default SEO, hotline, Zalo, social links, and branches must always be read from `SiteSettings` and `Branches` in the database.

---

## F — Testing, Migrations & Git Workflow

**F1. Zero Warnings & Mandatory Verification Gate**:
- The solution enforces `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in `Directory.Build.props`.
- Every change must build and pass all tests before completion:
  ```bash
  dotnet build -c Release && dotnet test -c Release --no-build
  ```

**F2. EF Core Migrations**:
- Every change to an entity or `IEntityTypeConfiguration` must be accompanied by an updated EF Core migration in `src/HoangPhucSpa.Data`:
  ```bash
  dotnet ef migrations add <MigrationName> -p src/HoangPhucSpa.Data -s src/HoangPhucSpa.Api -o Migrations
  ```
- Keep one consolidated migration per task/feature rather than committing multiple broken intermediate migrations.

**F3. Testing Standards & Golden Tests**:
- Follow BDD test naming: `<MethodOrEndpoint>_Should_<ExpectedBehavior>_When_<Condition>()`.
- Cover both the happy path and rejection cases (`400`, `401`, `403`, `404`, `409`).
- Maintain mandatory test suites:
  - `SlugHelperTests`: Must match 100% of `tests/golden/slug-golden.json`.
  - `ContentSanitizerTests`: Verifies XSS stripping (`<script>`, `onerror`, `javascript:`, unauthorized `<iframe>`) and preserves allowed YouTube/Google Maps iframes.
  - `HealthEndpointTests`: Verifies liveness (`/api/v1/health/live`) and readiness (`/api/v1/health`).

**F4. Git & Delivery Rules**:
- **Never run `git commit`, `git push`, `git checkout`, or `git reset` automatically.** The user performs all Git state changes manually.
- When finishing a task, always propose a commit message and change overview in English in a single copyable block:
  ```text
  <type>(<scope>): <short imperative description in English>

  - <Bullet 1 starting with imperative verb: Add, Update, Remove, Fix, Refactor...>
  - <Bullet 2...>
  ```
