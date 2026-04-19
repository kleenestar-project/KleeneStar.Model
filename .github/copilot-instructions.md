# Copilot Instructions — KleeneStar.Model

## Project Role

`KleeneStar.Model` is the **data model library** for the KleeneStar platform. It provides:
- Entity classes (EF Core entities)
- Enum types with associated extension methods
- REST value converters (`IRestValueConverter`)
- Entity Framework Core configurations (`IEntityTypeConfiguration<T>`)
- The `KleeneStarDbContext`, seeder, factory, and extensions
- The internal `ModelHub` for coordinating infrastructure access

It is consumed by `KleeneStar.Core` via `InternalsVisibleTo`.

## Target Framework

- .NET 10
- Class library — no entry point

---

## Project Structure

```
KleeneStar.Model/
└── src/
    └── KleeneStar.Model/
        ├── Config/               ← Application configuration (e.g. DbConfig)
        ├── Configure/            ← EF Core IEntityTypeConfiguration<T> classes
        ├── Converters/           ← IRestValueConverter implementations
        ├── Entities/             ← Entity classes and associated enums
        ├── AssemblyInfo.cs       ← InternalsVisibleTo declarations
        ├── KleeneStarDbContext.cs
        ├── KleeneStarDbContextExtensions.cs
        ├── KleeneStarDbContextFactory.cs
        ├── KleeneStarDbSeeder.cs                   ← partial class root
        ├── KleeneStarDbSeeder.<Domain>.cs           ← partial class per domain
        ├── ModelHub.cs                              ← partial class root
        └── ModelHub.<Domain>.cs                    ← partial class per domain
```

---

## Documentation Guidelines

All documentation must be placed inside the dedicated docs directory to maintain a consistent and discoverable structure across the project. Each document must be written in continuous English prose without the use of bullet points, dash based lists, or similar list formatting. Every section must begin with a short introductory paragraph that explains the purpose and context of the section before presenting any technical details or implementation specific information. Where illustrations or diagrams are required, they must be provided as ASCII based figures embedded directly into the text so that they remain readable in plain text environments and version control systems. This ensures that the documentation remains readable, coherent, and accessible for contributors, while also enforcing a uniform style that aligns with the overall conventions of the KleeneStar platform.

---

## Naming Conventions

| Artifact | Convention | Example |
|---|---|---|
| Entity class | `PascalCase` | `Class`, `Field`, `Workspace` |
| State enum | `<Entity>State` | `ClassState`, `FieldState` |
| AccessModifier enum | `<Entity>AccessModifier` or shared `AccessModifier` | `WorkspaceAccessModifier`, `AccessModifier` |
| State converter | `<Entity>StateConverter` | `ClassStateConverter`, `FieldStateConverter` |
| Other converters | `<Entity>Converter` or `<Type>Converter` | `FieldTypeConverter`, `FieldCardinalityConverter` |
| EF Core configuration | `<Entity>Configuration` | `ClassConfiguration` |
| Hub partial classes | `ModelHub.<Domain>.cs` | `ModelHub.Class.cs` |
| Seeder partial classes | `KleeneStarDbSeeder.<Domain>.cs` | `KleeneStarDbSeeder.Classes.cs` |

---

## Entity Design Pattern

Every entity implements `IEntity` and follows this structure:

```csharp
public class MyEntity : IEntity
{
    /// <summary>Gets or sets the database id.</summary>
    [IndexIgnore]
    [Key]
    public int RawId { get; set; }

    /// <summary>Gets or sets the unique identifier for the entity.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; }

    /// <summary>Gets or sets the description.</summary>
    public string Description { get; set; }

    /// <summary>Gets or sets the current state.</summary>
    [RestConverter<MyEntityStateConverter>]
    public MyEntityState State { get; set; }

    /// <summary>Gets or sets the icon.</summary>
    [RestConverter<RestValueConverterImageIcon>]
    public ImageIcon Icon { get; set; }

    /// <summary>Gets or sets the date and time when the entity was created.</summary>
    public DateTime Created { get; set; }

    /// <summary>Gets or sets the date and time when the entity was updated.</summary>
    public DateTime Updated { get; set; }

    /// <summary>Initializes a new instance of the class.</summary>
    public MyEntity()
    {
        Id = Guid.NewGuid();
    }
}
```

### Key Rules

- `RawId` is the **database primary key** (int, auto-incremented). Always decorated with `[IndexIgnore]` and `[Key]`.
- `Id` is the **logical GUID identifier** used in application and API logic.
- Navigation properties that must not appear in JSON serialization are decorated with `[JsonIgnore]`.
- Enum properties (state, access modifier, cardinality, type) always carry `[RestConverter<TConverter>]`.
- Collection navigation properties are initialized to empty lists: `= [];`.
- Constructors initialize `Id = Guid.NewGuid()`. An overload accepting a `Guid id` parameter exists where needed.

---

## Enum and Extension Method Pattern

State enums and other domain enums are defined **in the same file** as their extension class.

```csharp
public enum ClassState
{
    /// <summary>Indicates that the class is fully configured and usable.</summary>
    Active,

    /// <summary>Indicates that the item is archived and no longer active.</summary>
    Archived
}

public static class ClassStateExtensions
{
    public static bool IsActive(this ClassState state) => state == ClassState.Active;

    public static Guid Id(this ClassState state) => state switch
    {
        ClassState.Active   => Guid.Parse("F732D35B-AF61-4E46-ABAE-26868A518367"),
        ClassState.Archived => Guid.Parse("0302997E-78CB-4CC5-8A1B-BD77B1227F07"),
        _                   => Guid.Empty
    };

    public static string Text(this ClassState state) => state switch
    {
        ClassState.Active   => "kleenestar.core:state.active.label",
        ClassState.Archived => "kleenestar.core:state.archived.label",
        _                   => null
    };

    public static string Color(this ClassState state) => state switch
    {
        ClassState.Active   => TypeColorSelection.Success.ToClass(),
        ClassState.Archived => TypeColorSelection.Danger.ToClass(),
        _                   => TypeColorSelection.Default.ToClass()
    };
}
```

### Rules for Enums

- GUIDs in `Id()` are **fixed constants** — never generated at runtime.
- `Text()` returns an **i18n resource key** prefixed with `kleenestar.core:`.
- `Color()` returns a CSS class string using `TypeColorSelection.<Value>.ToClass()`.
- State enums always have `IsActive()`.
- Shared state values (`Active`, `Archived`, `Locked`, `Disabled`, `Deleted`) use the shared i18n keys `kleenestar.core:state.<value>.label`.
- Type/modifier enums use entity-scoped keys: `kleenestar.core:<enumtype>.<value>.label`.

---

## REST Value Converter Pattern

Converters translate between GUID strings (from REST payloads) and strongly typed enum values.

```csharp
public class ClassStateConverter : IRestValueConverter
{
    public object FromRaw(object rawValue, Type targetType)
    {
        if (rawValue is null) return null;

        if (rawValue is string s)
        {
            var id = s.Split(";", StringSplitOptions.RemoveEmptyEntries)
                       .Select(x => x.Trim())
                       .Where(x => x.Length > 0)
                       .Select(x => Guid.TryParse(x, out var g) ? (Guid?)g : null)
                       .Where(g => g.HasValue)
                       .Select(g => g.Value)
                       .FirstOrDefault();

            if (id == ClassState.Archived.Id()) return ClassState.Archived;
            return ClassState.Active;
        }

        return rawValue;
    }

    public object ToRaw(object value, Type sourceType) => value switch
    {
        ClassState.Archived => ClassState.Archived.Id(),
        _                   => ClassState.Active.Id()
    };
}
```

- For enums with many values, use `Enum.GetValues(typeof(TEnum))` in `FromRaw` and iterate via `value.Id() == id`.
- Always provide a safe default return value.

---

## EF Core Configuration Pattern

```csharp
internal class ClassConfiguration : IEntityTypeConfiguration<Class>
{
    public void Configure(EntityTypeBuilder<Class> builder)
    {
        builder.ToTable("Class");
        builder.HasKey(x => x.RawId);

        builder.Property(x => x.RawId)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasColumnName("Name")
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(x => x.State)
            .HasColumnName("State");
    }
}
```

- Configuration classes are `internal`.
- Column names match property names.
- All configurations are registered in `KleeneStarDbContext.OnModelCreating`.

---

## ModelHub Pattern

```csharp
internal static partial class ModelHub
{
    public static IComponentHub ComponentHub { get; set; }
    public static IApplicationContext ApplicationContext { get; set; }
    public static IHttpServerContext HttpServerContext { get; set; }
    public static DbConfig DatabaseConfig { get; set; }

    public static KleeneStarDbContext CreateDbContext() { ... }
}
```

- `internal static partial` — split across `ModelHub.<Domain>.cs` files.
- Exposes infrastructure access points (component hub, db context factory).

---

## Seeder Pattern

The `KleeneStarDbSeeder` is a `public static partial class` split per domain:

```csharp
// KleeneStarDbSeeder.cs
public static partial class KleeneStarDbSeeder
{
    public static void Seed(KleeneStarDbContext db) { ... }
}

// KleeneStarDbSeeder.Classes.cs
public static partial class KleeneStarDbSeeder
{
    private static void SeedClasses(KleeneStarDbContext db) { ... }
}
```

- Each domain gets its own partial file.
- Seeder methods are `private static`.
- Enum properties use enum values (not string literals): `FieldType = FieldType.Text`.

---

## Commenting Style

Every `public` and `internal` member has an XML doc comment. Pattern:

```csharp
/// <summary>
/// Gets or sets the current state of the class.
/// </summary>
[RestConverter<ClassStateConverter>]
public ClassState State { get; set; }
```

Multi-sentence summaries use full sentences ending with periods. Parameter docs use `<param name="x">...</param>`. Return value docs use `<returns>...</returns>`.

---

## Assembly Visibility

```csharp
[assembly: InternalsVisibleTo("KleeneStar.Core")]
[assembly: InternalsVisibleTo("Kleenestar.Model.Test")]
[assembly: InternalsVisibleTo("KleeneStar.Core.Test")]
```

Internal types are accessible to `KleeneStar.Core` and both test projects.

---

## Dependencies

| Package | Role |
|---|---|
| `Microsoft.EntityFrameworkCore` | ORM |
| `WebExpress.WebApp` | `IRestValueConverter`, `[RestConverter<T>]` |
| `WebExpress.WebIndex` | `[IndexIgnore]` attribute |
| `WebExpress.WebUI` | `ImageIcon`, `TypeColorSelection` |
