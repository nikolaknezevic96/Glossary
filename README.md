# Glossary Management API

## Overview

This project implements a backend glossary management system using
.NET 8, ASP.NET Core Web API, Entity Framework Core, and SQL Server.

The solution follows a layered architecture and enforces domain rules
for glossary term lifecycle management (Draft => Published => Archived).
Authentication is implemented using JWT.

------------------------------------------------------------------------

## Architecture

The solution is structured into the following layers:

-   Glossary.Api -- Web layer (controllers, authentication,
    configuration)
-   Glossary.Application -- Use cases, DTOs, service layer
-   Glossary.Domain -- Core business rules and domain model
-   Glossary.Infrastructure -- EF Core persistence and repository
    implementation
-   Glossary.Tests -- Unit tests: xUnit and Moq

------------------------------------------------------------------------

## Authentication

JWT Bearer authentication is used.

Seeded user for demonstration:

Username: author
Password: password

Protected endpoints require a valid JWT token.

------------------------------------------------------------------------

## Database

The database used is SQL Server (LocalDB recommended). Migrations are applied automatically at startup.
Initial data is seeded.

------------------------------------------------------------------------

### Configuration

Update appsettings.json if necessary:

"ConnectionStrings": {
  "Default": "Server=localhost\\SQLEXPRESS;Database=GlossaryDb;Trusted_Connection=True;TrustServerCertificate=True"
}

"Jwt": {
  "Issuer": "Glossary.Api",
  "Audience": "Glossary.Api",
  "Key": "asdasdasdasdasdasdasdasdasdasdasdasdasdasd"
}

### Apply Migrations (if required)

dotnet ef database update --project Glossary.Infrastructure
--startup-project Glossary.Api

### Run the API

dotnet run --project Glossary.Api

Access Swagger at:

https://localhost:{port}/swagger

------------------------------------------------------------------------

## Testing

Run unit tests from the solution root:

dotnet test

Tests cover domain validation rules, lifecycle transitions,
authorization rules, and repository interaction verification.



