# Database Migrations Guide

This document explains how database migrations are organized in **KleeneStar** and how to create, update, and manage migrations for each supported database provider.
**KleeneStar** uses a provider-neutral model assembly and provider-specific migration assemblies, allowing full flexibility while keeping the core model clean and independent.

## Project Structure

```
KleeneStar.Model/          → Provider-neutral model (DbContext, entities, configuration)
KleeneStar.Model.Sqlite/   → Sqlite migrations + Sqlite DbContext factory
```

Each provider assembly contains:
- its own migration history
- its own model snapshot
- its own  factory method
- its own provider configuration

- The model assembly contains no provider-specific logic.

## Creating and Managing Migrations

Because KleeneStar supports multiple database engines, each provider requires its own migration set.
Migrations must always be created inside the provider-specific project, not in the model project.

EF Core uses:
- the model from KleeneStar.Model
- the provider configuration from the provider assembly
- the startup project to bootstrap the process

### Creating a Migration

To create a new migration for a specific database provider, navigate to the root directory of the corresponding provider‑specific project (e.g., `KleeneStar.Model.Sqlite`) and run the following command:

```
cd ..\..\KleeneStar.Model\src\KleeneStar.Model.Sqlite\
dotnet ef migrations add InitialCreate --context KleeneStar.Model.KleeneStarDbContext
```

The value `InitialCreate` is simply the name of the migration you want to generate.
Choose a descriptive name that reflects the change (e.g., `AddUserTable`, `RenameColumnX`, `AddIndexes`).


### Updating an Existing Migration Set

To add a new migration to an existing provider-specific migration set, use the same command pattern as above, replacing `InitialCreate` with the desired migration name:

```
dotnet ef migrations add MyMigration --context KleeneStar.Model.KleeneStarDbContext
```

This command:
- writes the migration files into the provider‑specific project
- updates the provider‑specific model snapshot
