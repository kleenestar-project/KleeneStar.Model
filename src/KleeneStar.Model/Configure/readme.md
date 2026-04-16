# Entity Configuration

This document explains how entity configurations in the `KleeneStar.Model` project are structured and how they interact with the multi-provider architecture of **KleeneStar**.
All configurations implement `IEntityTypeConfiguration<T>` and are intentionally designed to be provider-neutral, ensuring compatibility with SQLite, PostgreSQL, and SQL Server.

All entity configurations are located in:

```
KleeneStar.Model/Configure/
```

Each entity has its own configuration class:

```
WorkspaceConfiguration.cs
CategoryConfiguration.cs
...
```

## Purpose of the Configuration Layer

The configuration classes define:
- Table names
- Primary keys
- Property rules (required, max length, column names)
- Navigation properties
- Relationships (one-to-many, many-to-many, etc.)
- Value conversions

- They do not contain any provider-specific SQL types or database-specific logic.
This ensures that the model layer remains clean, portable, and independent of the underlying database engine.

