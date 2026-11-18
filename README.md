# TemplateAPI

A minimal ASP.NET Core (.NET 8) Web API template with a `User` resource, full CRUD endpoints, and Entity Framework Core with a configurable database provider.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Getting started

```bash
dotnet run --project src/TemplateAPI
```

Swagger UI is available at `/swagger` when running in the Development environment. The database is created automatically on first run.

## Endpoints

| Method | Route              | Description       | Responses          |
|--------|--------------------|-------------------|--------------------|
| GET    | `/api/users`       | List all users    | 200                |
| GET    | `/api/users/{id}`  | Get a user by id  | 200, 404           |
| POST   | `/api/users`       | Create a user     | 201, 400, 409      |
| PUT    | `/api/users/{id}`  | Update a user     | 200, 400, 404, 409 |
| DELETE | `/api/users/{id}`  | Delete a user     | 204, 404           |

Example requests are in [`src/TemplateAPI/TemplateAPI.http`](src/TemplateAPI/TemplateAPI.http).

## Database configuration

The provider and connection string are set in the `Database` section of `appsettings.json`:

```json
"Database": {
  "Provider": "Sqlite",
  "ConnectionString": "Data Source=templateapi.db"
}
```

Supported providers: `Sqlite`, `SqlServer`, `PostgreSql`.

| Provider   | Example connection string |
|------------|---------------------------|
| Sqlite     | `Data Source=templateapi.db` |
| SqlServer  | `Server=localhost;Database=TemplateAPI;Trusted_Connection=True;TrustServerCertificate=True` |
| PostgreSql | `Host=localhost;Port=5432;Database=templateapi;Username=postgres;Password=postgres` |

Settings can be overridden per environment with `appsettings.{Environment}.json`, [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets), or environment variables:

```bash
Database__Provider=PostgreSql
Database__ConnectionString="Host=localhost;Database=templateapi;Username=postgres;Password=secret"
```

Keep real credentials out of `appsettings.json` — use user secrets locally and environment variables in deployed environments.

To add another provider, install its EF Core package, add a value to `DatabaseProvider` in `Data/DatabaseOptions.cs`, and add a case to the switch in `Data/DatabaseServiceExtensions.cs`.

## Project structure

```
src/TemplateAPI/
├── Controllers/   # HTTP endpoints
├── Data/          # DbContext and database provider configuration
├── DTOs/          # Request/response contracts with validation
├── Models/        # Entity models
└── Services/      # Business/data-access logic
```

## Notes

- The schema is created with `EnsureCreated()` at startup. Switch to EF Core migrations (`dotnet ef migrations add Initial`) once the schema starts evolving.
- No authentication is configured.
