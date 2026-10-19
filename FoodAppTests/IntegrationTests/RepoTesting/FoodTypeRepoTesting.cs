using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;
using FluentAssertions;

namespace FoodAppTests.IntegrationTests.RepoTesting
{
    /// <summary>
    /// Integration tests for <see cref="FoodTypeRepository"/>.
    /// They run against a real SQL Server container supplied by <see cref="DBMockup"/>.
    /// General rules for writing repository tests are described in Documentation/How_To_Make_Tests.md.
    /// </summary>
    [Collection("Database collection")]
    public class FoodTypeRepoTesting
    {
        private readonly IFoodTypeRepository _foodTypeRepo;

        public FoodTypeRepoTesting(DBMockup dBMockup)
        {
            DBConnector.SetConnectionString(dBMockup.ConnectionString);

            _foodTypeRepo = new FoodTypeRepository();
        }

        // =====================================================================
        // Arrange helpers - shared by many tests.
        // The database is shared by every test in the collection, so each seeded
        // value must be unique to keep the tests independent from one another.
        // =====================================================================

        /// <summary>Builds a unique name (prefix + guid) that always fits into nvarchar(50).</summary>
        private static string UniqueName(string prefix = "FoodType")
            => $"{prefix}_{Guid.NewGuid():N}";

        /// <summary>Creates a single food type and returns the persisted row (unique name by default).</summary>
        private FoodType SeedFoodType(string? name = null)
            => _foodTypeRepo.CreateFoodType(name ?? UniqueName());

        /// <summary>Creates one food type per suffix; all of them share the given unique prefix.</summary>
        private List<FoodType> SeedFoodTypes(string namePrefix, params string[] suffixes)
        {
            var seeded = new List<FoodType>();
            foreach (var suffix in suffixes)
            {
                seeded.Add(_foodTypeRepo.CreateFoodType($"{namePrefix}_{suffix}"));
            }
            return seeded;
        }

        // =====================================================================
        // CreateFoodType
        // =====================================================================

        /// <summary>Happy path: inserting a new name returns the inserted row with a generated id.</summary>
        [Fact]
        public void CreateFoodType_NewFoodType_ReturnFoodTypeObject()
        {
            //Arrange
            string typeName = "Test_type";

            //Act
            var result = _foodTypeRepo.CreateFoodType(typeName);

            //Assert
            result.Should().NotBeNull();
            result.name.Should().Be(typeName);
            result.id.Should().NotBe(Guid.Empty);
        }

        /// <summary>
        /// Error case: name is NOT NULL in the database, so inserting null fails.
        /// DBConnector swallows the SqlException, leaving an empty result, which makes
        /// the repository throw "Insert failed for food_type".
        /// </summary>
        [Fact]
        public void CreateFoodType_NullName_ThrowsException()
        {
            //Arrange
            string? nullName = null;

            //Act
            Action act = () => _foodTypeRepo.CreateFoodType(nullName!);

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>
        /// Error case: an empty name is not a valid food type name.
        /// NOTE: the repository has no validation yet and SQL Server accepts '',
        /// so this test is EXPECTED TO FAIL until validation is added to FoodTypeRepository.
        /// </summary>
        [Fact]
        public void CreateFoodType_EmptyName_ThrowsException()
        {
            //Arrange
            string emptyName = string.Empty;

            //Act
            Action act = () => _foodTypeRepo.CreateFoodType(emptyName);

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Error case: name exceeds nvarchar(50); the truncation error is swallowed and the insert throws.</summary>
        [Fact]
        public void CreateFoodType_NameLongerThan50Chars_ThrowsException()
        {
            //Arrange
            string tooLongName = new string('a', 51);

            //Act
            Action act = () => _foodTypeRepo.CreateFoodType(tooLongName);

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Boundary case: a name of exactly 50 characters is still accepted.</summary>
        [Fact]
        public void CreateFoodType_NameAtMaxLength_ReturnsFoodType()
        {
            //Arrange
            string maxLengthName = new string('b', 50);

            //Act
            var result = _foodTypeRepo.CreateFoodType(maxLengthName);

            //Assert
            result.name.Should().Be(maxLengthName);
            result.id.Should().NotBe(Guid.Empty);
        }

        /// <summary>Boundary case: name has no unique constraint, so a duplicate is allowed and gets its own id.</summary>
        [Fact]
        public void CreateFoodType_DuplicateName_ReturnsFoodTypeWithDifferentId()
        {
            //Arrange
            string duplicatedName = UniqueName();

            //Act
            var first = _foodTypeRepo.CreateFoodType(duplicatedName);
            var second = _foodTypeRepo.CreateFoodType(duplicatedName);

            //Assert
            second.name.Should().Be(duplicatedName);
            second.id.Should().NotBe(first.id);
        }

        // =====================================================================
        // GetFoodTypeById
        // =====================================================================

        /// <summary>Happy path: an existing id returns the matching row.</summary>
        [Fact]
        public void GetFoodTypeById_ExistingId_ReturnsFoodType()
        {
            //Arrange
            var seeded = SeedFoodType();

            //Act
            var result = _foodTypeRepo.GetFoodTypeById(seeded.id);

            //Assert
            result.Should().NotBeNull();
            result!.id.Should().Be(seeded.id);
            result.name.Should().Be(seeded.name);
        }

        /// <summary>Error case: an unknown id returns null instead of throwing.</summary>
        [Fact]
        public void GetFoodTypeById_NonExistingId_ReturnsNull()
        {
            //Arrange
            Guid unknownId = Guid.NewGuid();

            //Act
            var result = _foodTypeRepo.GetFoodTypeById(unknownId);

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: Guid.Empty is never a valid row id and returns null.</summary>
        [Fact]
        public void GetFoodTypeById_EmptyGuid_ReturnsNull()
        {
            //Arrange
            Guid emptyId = Guid.Empty;

            //Act
            var result = _foodTypeRepo.GetFoodTypeById(emptyId);

            //Assert
            result.Should().BeNull();
        }

        // =====================================================================
        // GetFoodTypes
        // =====================================================================

        /// <summary>Happy path: the filter returns only the rows whose name contains it.</summary>
        [Fact]
        public void GetFoodTypes_MatchingFilter_ReturnsOnlyMatchingItems()
        {
            //Arrange
            string prefix = UniqueName("Filter");
            SeedFoodTypes(prefix, "A", "B");

            //Act
            var result = _foodTypeRepo.GetFoodTypes(prefix).ToList();

            //Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(foodType => foodType.name.StartsWith(prefix));
        }

        /// <summary>Error case: a filter that matches nothing returns an empty collection.</summary>
        [Fact]
        public void GetFoodTypes_NonMatchingFilter_ReturnsEmpty()
        {
            //Arrange
            string missingPrefix = UniqueName("NoMatch");

            //Act
            var result = _foodTypeRepo.GetFoodTypes(missingPrefix);

            //Assert
            result.Should().BeEmpty();
        }

        /// <summary>Error case: a null filter must not break the query (it is treated as empty).</summary>
        [Fact]
        public void GetFoodTypes_NullFilter_DoesNotThrow()
        {
            //Act
            Action act = () => _foodTypeRepo.GetFoodTypes(null!).ToList();

            //Assert
            act.Should().NotThrow();
        }

        /// <summary>Boundary case: paging returns the requested slice and an empty last page.</summary>
        [Fact]
        public void GetFoodTypes_Paging_ReturnsExpectedPageSizes()
        {
            //Arrange
            string prefix = UniqueName("Paging");
            SeedFoodTypes(prefix, "1", "2", "3", "4", "5");

            //Act
            var page1 = _foodTypeRepo.GetFoodTypes(prefix, page: 1, pageSize: 2).ToList();
            var page2 = _foodTypeRepo.GetFoodTypes(prefix, page: 2, pageSize: 2).ToList();
            var page3 = _foodTypeRepo.GetFoodTypes(prefix, page: 3, pageSize: 2).ToList();
            var page4 = _foodTypeRepo.GetFoodTypes(prefix, page: 4, pageSize: 2).ToList();

            //Assert
            page1.Should().HaveCount(2);
            page2.Should().HaveCount(2);
            page3.Should().HaveCount(1);
            page4.Should().BeEmpty();
            page1.Select(foodType => foodType.id).Should().NotIntersectWith(page2.Select(foodType => foodType.id));
        }

        /// <summary>Happy path: results come back ordered by name ascending.</summary>
        [Fact]
        public void GetFoodTypes_ReturnsOrderedByNameAscending()
        {
            //Arrange
            string prefix = UniqueName("Order");
            SeedFoodTypes(prefix, "C", "A", "B");

            //Act
            var result = _foodTypeRepo.GetFoodTypes(prefix).ToList();

            //Assert
            result.Select(foodType => foodType.name).Should().BeInAscendingOrder();
        }

        // =====================================================================
        // UpdateFoodType
        // =====================================================================

        /// <summary>Happy path: an existing id gets its name changed and the change is persisted.</summary>
        [Fact]
        public void UpdateFoodType_ExistingId_ReturnsUpdatedFoodType()
        {
            //Arrange
            var seeded = SeedFoodType();
            string newName = UniqueName("Updated");

            //Act
            var result = _foodTypeRepo.UpdateFoodType(seeded.id, newName);

            //Assert
            result.Should().NotBeNull();
            result!.id.Should().Be(seeded.id);
            result.name.Should().Be(newName);

            var persisted = _foodTypeRepo.GetFoodTypeById(seeded.id);
            persisted.Should().NotBeNull();
            persisted!.name.Should().Be(newName);
        }

        /// <summary>Error case: updating an unknown id affects no rows and returns null.</summary>
        [Fact]
        public void UpdateFoodType_NonExistingId_ReturnsNull()
        {
            //Arrange
            Guid unknownId = Guid.NewGuid();

            //Act
            var result = _foodTypeRepo.UpdateFoodType(unknownId, UniqueName("Updated"));

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: a null name is rejected by the guard clause before touching the database.</summary>
        [Fact]
        public void UpdateFoodType_NullName_ReturnsNull()
        {
            //Arrange
            var seeded = SeedFoodType();

            //Act
            var result = _foodTypeRepo.UpdateFoodType(seeded.id, null);

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: a name longer than nvarchar(50) fails silently and returns null.</summary>
        [Fact]
        public void UpdateFoodType_NameLongerThan50Chars_ReturnsNull()
        {
            //Arrange
            var seeded = SeedFoodType();
            string tooLongName = new string('a', 51);

            //Act
            var result = _foodTypeRepo.UpdateFoodType(seeded.id, tooLongName);

            //Assert
            result.Should().BeNull();
        }

        // =====================================================================
        // DeleteFoodType
        // =====================================================================

        /// <summary>Happy path: deleting an existing id removes the row.</summary>
        [Fact]
        public void DeleteFoodType_ExistingId_RemovesRow()
        {
            //Arrange
            var seeded = SeedFoodType();

            //Act
            bool result = _foodTypeRepo.DeleteFoodType(seeded.id);

            //Assert
            result.Should().BeTrue();
            _foodTypeRepo.GetFoodTypeById(seeded.id).Should().BeNull();
        }

        /// <summary>Error case: deleting an unknown id is not an error, it simply deletes nothing (current behaviour).</summary>
        [Fact]
        public void DeleteFoodType_NonExistingId_ReturnsTrue()
        {
            //Arrange
            Guid unknownId = Guid.NewGuid();

            //Act
            bool result = _foodTypeRepo.DeleteFoodType(unknownId);

            //Assert
            result.Should().BeTrue();
        }

        /// <summary>Error case: Guid.Empty is not an error either and returns true (current behaviour).</summary>
        [Fact]
        public void DeleteFoodType_EmptyGuid_ReturnsTrue()
        {
            //Arrange
            Guid emptyId = Guid.Empty;

            //Act
            bool result = _foodTypeRepo.DeleteFoodType(emptyId);

            //Assert
            result.Should().BeTrue();
        }
    }
}