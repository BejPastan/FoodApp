# How To Make Tests

Guidelines for writing **integration tests for a single repository** in `FoodAppTests`.
Use `IntegrationTests/RepoTesting/FoodTypeRepoTesting.cs` as a living example.

## 1. What kind of test is it?

Repository tests are **integration tests**: they talk to a real SQL Server instance, not to mocks.
The database is provided by `DBMockup` (Testcontainers + DbUp migrations), so a test verifies the
real SQL, the real schema and the real Dapper mapping.

## 2. Where does it live?

- Folder: `FoodAppTests/IntegrationTests/RepoTesting/`
- File name: `<RepositoryName>Testing.cs` (e.g. `FoodTypeRepoTesting.cs` for `FoodTypeRepository`)
- Namespace: `FoodAppTests.IntegrationTests.RepoTesting`
- One **new class per repository**.

## 3. Initialization

`DBMockup`:
- starts an `mssql/server:2022` container,
- creates the database,
- runs every embedded script from `FoodAppTests/Scripts/db_vXXXX.sql` (via DbUp).

`DBMockup` is registered as an `ICollectionFixture`, so the container is started **once** and
**shared by every test in the collection**. Therefore:
- each test class must be decorated with `[Collection("Database collection")]`,
- the constructor must take `DBMockup` as a parameter and point `DBConnector` at its connection
  string *before* creating the repository.

```csharp
[Collection("Database collection")]
public class XRepoTesting
{
    private readonly IXRepository _repo;

    public XRepoTesting(DBMockup dBMockup)
    {
        DBConnector.SetConnectionString(dBMockup.ConnectionString);

        _repo = new XRepository();
    }
}
```

## 4. Isolation (very important)

The database is shared and **not reset between tests**, so all data a test creates must be
**unique**. Never assert on "the whole table" - always query the data the test created itself.
The easiest way is to append a `Guid` to every seeded value.

## 5. Arrange helpers

Extract every "set up data" step into a `private` helper and reuse it. Typical set of helpers:

```csharp
private static string UniqueName(string prefix = "FoodType")
    => $"{prefix}_{Guid.NewGuid():N}";

private FoodType SeedFoodType(string? name = null)
    => _repo.CreateFoodType(name ?? UniqueName());

private List<FoodType> SeedFoodTypes(string namePrefix, params string[] suffixes)
{
    var seeded = new List<FoodType>();
    foreach (var suffix in suffixes)
    {
        seeded.Add(_repo.CreateFoodType($"{namePrefix}_{suffix}"));
    }
    return seeded;
}
```

Rules of thumb:
- one helper that builds **unique values**,
- one helper that creates a **single row**,
- one helper that creates **many rows** for list / filter / paging tests.

## 6. Test structure

Every test follows Arrange / Act / Assert:

```csharp
[Fact]
public void Method_Scenario_ExpectedResult()
{
    //Arrange
    ...

    //Act
    var result = _repo.Method(...);

    //Assert
    result.Should()...;
}
```

Conventions:
- one behaviour per test,
- name: `MethodName_Scenario_ExpectedResult`,
- assert with **FluentAssertions** (`result.Should().BeNull()`, `.HaveCount(n)`, `.BeEmpty()` ...),
- document every non obvious expectation directly above the test as an XML `/// <summary>` comment.

## 7. What to cover for every repository method

For **each** public method write tests for:

1. **Happy path** - valid input returns the expected value.
2. **Not found** - an unknown `Guid` (`Guid.NewGuid()`, `Guid.Empty`) returns `null` / empty / `false`.
3. **Invalid input** - `null`, empty string, wrong id, duplicated value, value over the DB limit.
4. **Boundary values** - exactly the max column length, first/last page, empty page.
5. **Paging & ordering** - correct page size, empty last page, the ordering the repository promises.

## 8. How errors surface (read the repository first!)

`DBConnector.QueryDatabase<T>` **catches `SqlException` and returns an empty sequence** - it never
lets the SQL error escape. As a result a database error turns into:

- `Create...` -> empty result -> the repository `throw`s,
- `Update...` / `Get...` -> `FirstOrDefault()` -> returns `null`, if update failed
- `Delete...` -> should reurn false if deletation failed

Repositiries should react in this manor, until there is writen different in comments of spcific method.
Test could failed it there are errors in repos