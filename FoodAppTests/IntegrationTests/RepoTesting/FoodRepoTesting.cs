using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;
using FluentAssertions;

namespace FoodAppTests.IntegrationTests.RepoTesting
{
    /// <summary>
    /// Integration tests for <see cref="FoodRepository"/>.
    /// They run against a real SQL Server container supplied by <see cref="DBMockup"/>.
    /// Rules for writing repository tests are described in Documentation/How_To_Make_Tests.md.
    /// </summary>
    [Collection("Database collection")]
    public class FoodRepoTesting
    {
        private readonly IFoodRepository _foodRepo;
        private readonly IFoodTypeRepository _foodTypeRepo;

        public FoodRepoTesting(DBMockup dBMockup)
        {
            DBConnector.SetConnectionString(dBMockup.ConnectionString);

            _foodRepo = new FoodRepository();
            _foodTypeRepo = new FoodTypeRepository();
        }

        // =====================================================================
        // Arrange helpers - shared by many tests.
        // The database is shared by every test in the collection, so each seeded
        // value must be unique (a Guid suffix) to keep the tests independent.
        // =====================================================================

        /// <summary>Builds a unique name (prefix + guid) that always fits into nvarchar(100).</summary>
        private static string UniqueName(string prefix = "Food")
            => $"{prefix}_{Guid.NewGuid():N}";

        /// <summary>Creates a food type and returns its id - needed because food.foodTypeId is a foreign key.</summary>
        private Guid SeedFoodType()
            => _foodTypeRepo.CreateFoodType(UniqueName("Type")).id;

        /// <summary>Creates a single food of the given type and returns the persisted row.</summary>
        private Food SeedFood(Guid foodTypeId, string? name = null)
            => _foodRepo.CreateFood(name ?? UniqueName(), foodTypeId);

        /// <summary>Creates one food per suffix, all sharing the given unique prefix (for list / filter / paging).</summary>
        private List<Food> SeedFoods(Guid foodTypeId, string namePrefix, params string[] suffixes)
        {
            var seeded = new List<Food>();
            foreach (var suffix in suffixes)
            {
                seeded.Add(_foodRepo.CreateFood($"{namePrefix}_{suffix}", foodTypeId));
            }
            return seeded;
        }

        // =====================================================================
        // CreateFood
        // =====================================================================

        /// <summary>Happy path: a valid name and a real food type create a new food row.</summary>
        [Fact]
        public void CreateFood_NewFood_ReturnsFoodObject()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            string foodName = UniqueName();

            //Act
            var result = _foodRepo.CreateFood(foodName, foodTypeId);

            //Assert
            result.Should().NotBeNull();
            result.name.Should().Be(foodName);
            result.foodTypeId.Should().Be(foodTypeId);
            result.id.Should().NotBe(Guid.Empty);
        }

        /// <summary>Error case: foodTypeId does not exist, the foreign key is violated and the insert throws.</summary>
        [Fact]
        public void CreateFood_NonExistingFoodType_ThrowsException()
        {
            //Arrange
            Guid unknownTypeId = Guid.NewGuid();

            //Act
            Action act = () => _foodRepo.CreateFood(UniqueName(), unknownTypeId);

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Error case: Guid.Empty is not a valid food type and violates the foreign key.</summary>
        [Fact]
        public void CreateFood_EmptyFoodTypeId_ThrowsException()
        {
            //Arrange
            Guid emptyTypeId = Guid.Empty;

            //Act
            Action act = () => _foodRepo.CreateFood(UniqueName(), emptyTypeId);

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Error case: name is required, a null name must be rejected.</summary>
        [Fact]
        public void CreateFood_NullName_ThrowsException()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();

            //Act
            Action act = () => _foodRepo.CreateFood(null!, foodTypeId);

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Error case: name is required, an empty name must be rejected.</summary>
        [Fact]
        public void CreateFood_EmptyName_ThrowsException()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();

            //Act
            Action act = () => _foodRepo.CreateFood(string.Empty, foodTypeId);

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Error case: name exceeds nvarchar(100); the truncation error is swallowed and the insert throws.</summary>
        [Fact]
        public void CreateFood_NameLongerThan100Chars_ThrowsException()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            string tooLongName = new string('a', 101);

            //Act
            Action act = () => _foodRepo.CreateFood(tooLongName, foodTypeId);

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Boundary case: a name of exactly 100 characters is still accepted.</summary>
        [Fact]
        public void CreateFood_NameAtMaxLength_ReturnsFoodObject()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            string maxLengthName = new string('b', 100);

            //Act
            var result = _foodRepo.CreateFood(maxLengthName, foodTypeId);

            //Assert
            result.name.Should().Be(maxLengthName);
        }

        // =====================================================================
        // GetFoodById
        // =====================================================================

        /// <summary>Happy path: an existing id returns the matching food row.</summary>
        [Fact]
        public void GetFoodById_ExistingId_ReturnsFoodObject()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            var seeded = SeedFood(foodTypeId);

            //Act
            var result = _foodRepo.GetFoodById(seeded.id);

            //Assert
            result.Should().NotBeNull();
            result!.id.Should().Be(seeded.id);
            result.name.Should().Be(seeded.name);
            result.foodTypeId.Should().Be(foodTypeId);
        }

        /// <summary>Error case: an unknown id returns null instead of throwing.</summary>
        [Fact]
        public void GetFoodById_NonExistingId_ReturnsNull()
        {
            //Arrange
            Guid unknownId = Guid.NewGuid();

            //Act
            var result = _foodRepo.GetFoodById(unknownId);

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: Guid.Empty is never a valid row id and returns null.</summary>
        [Fact]
        public void GetFoodById_EmptyGuid_ReturnsNull()
        {
            //Act
            var result = _foodRepo.GetFoodById(Guid.Empty);

            //Assert
            result.Should().BeNull();
        }

        // =====================================================================
        // GetFoods
        // =====================================================================

        /// <summary>Happy path: the type filter returns only the foods of that type.</summary>
        [Fact]
        public void GetFoods_ByFoodType_ReturnsOnlyThatType()
        {
            //Arrange
            Guid wantedTypeId = SeedFoodType();
            Guid otherTypeId = SeedFoodType();
            SeedFoods(wantedTypeId, UniqueName("Wanted"), "A", "B");
            SeedFood(otherTypeId);

            //Act
            var result = _foodRepo.GetFoods(wantedTypeId, string.Empty).ToList();

            //Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(food => food.foodTypeId == wantedTypeId);
        }

        /// <summary>Happy path: the name filter returns only the foods whose name contains it.</summary>
        [Fact]
        public void GetFoods_ByNameFilter_ReturnsOnlyMatchingItems()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            string prefix = UniqueName("Filtered");
            SeedFoods(foodTypeId, prefix, "A", "B");
            SeedFood(foodTypeId);

            //Act
            var result = _foodRepo.GetFoods(null, prefix).ToList();

            //Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(food => food.name.StartsWith(prefix));
        }

        /// <summary>Happy path: type + name filters are combined.</summary>
        [Fact]
        public void GetFoods_ByFoodTypeAndNameFilter_ReturnsMatchingItem()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            string prefix = UniqueName("Both");
            var seeded = SeedFoods(foodTypeId, prefix, "A", "B");

            //Act
            var result = _foodRepo.GetFoods(foodTypeId, $"{prefix}_A").ToList();

            //Assert
            result.Should().ContainSingle();
            result[0].id.Should().Be(seeded[0].id);
        }

        /// <summary>Error case: a filter that matches nothing returns an empty collection.</summary>
        [Fact]
        public void GetFoods_NonMatchingFilter_ReturnsEmpty()
        {
            //Arrange
            string missingPrefix = UniqueName("NoMatch");

            //Act
            var result = _foodRepo.GetFoods(null, missingPrefix);

            //Assert
            result.Should().BeEmpty();
        }

        /// <summary>Boundary case: an empty name filter is ignored and does not break the query.</summary>
        [Fact]
        public void GetFoods_EmptyNameFilter_IsIgnored()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            SeedFoods(foodTypeId, UniqueName("EmptyFilter"), "A", "B");

            //Act
            var result = _foodRepo.GetFoods(foodTypeId, string.Empty).ToList();

            //Assert
            result.Should().HaveCount(2);
        }

        /// <summary>Boundary case: a null name filter is ignored (treated the same as empty).</summary>
        [Fact]
        public void GetFoods_NullNameFilter_IsIgnored()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            SeedFoods(foodTypeId, UniqueName("NullFilter"), "A", "B");

            //Act
            var result = _foodRepo.GetFoods(foodTypeId, null!).ToList();

            //Assert
            result.Should().HaveCount(2);
        }

        /// <summary>Boundary case: paging returns the requested slice and an empty last page.</summary>
        [Fact]
        public void GetFoods_Paging_ReturnsExpectedPageSizes()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            SeedFoods(foodTypeId, UniqueName("Paging"), "1", "2", "3", "4", "5");

            //Act
            var page1 = _foodRepo.GetFoods(foodTypeId, string.Empty, page: 1, pageSize: 2).ToList();
            var page2 = _foodRepo.GetFoods(foodTypeId, string.Empty, page: 2, pageSize: 2).ToList();
            var page3 = _foodRepo.GetFoods(foodTypeId, string.Empty, page: 3, pageSize: 2).ToList();
            var page4 = _foodRepo.GetFoods(foodTypeId, string.Empty, page: 4, pageSize: 2).ToList();

            //Assert
            page1.Should().HaveCount(2);
            page2.Should().HaveCount(2);
            page3.Should().HaveCount(1);
            page4.Should().BeEmpty();
            page1.Select(food => food.id).Should().NotIntersectWith(page2.Select(food => food.id));
        }

        /// <summary>Happy path: results come back ordered by name ascending.</summary>
        [Fact]
        public void GetFoods_ReturnsOrderedByNameAscending()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            SeedFoods(foodTypeId, UniqueName("Order"), "C", "A", "B");

            //Act
            var result = _foodRepo.GetFoods(foodTypeId, string.Empty).ToList();

            //Assert
            result.Select(food => food.name).Should().BeInAscendingOrder();
        }

        // =====================================================================
        // UpdateFood
        // =====================================================================

        /// <summary>Happy path: a new name is applied and the current food type is kept.</summary>
        [Fact]
        public void UpdateFood_NameOnly_ReturnsUpdatedFood()
        {
            //Arrange
            Guid foodTypeId = SeedFoodType();
            var seeded = SeedFood(foodTypeId);
            string newName = UniqueName("Updated");

            //Act
            var result = _foodRepo.UpdateFood(seeded.id, newName, null);

            //Assert
            result.Should().NotBeNull();
            result!.name.Should().Be(newName);
            result.foodTypeId.Should().Be(foodTypeId);

            _foodRepo.GetFoodById(seeded.id)!.name.Should().Be(newName);
        }

        /// <summary>Happy path: a new food type is applied and the current name is kept.</summary>
        [Fact]
        public void UpdateFood_FoodTypeOnly_ReturnsUpdatedFood()
        {
            //Arrange
            Guid oldTypeId = SeedFoodType();
            Guid newTypeId = SeedFoodType();
            var seeded = SeedFood(oldTypeId);

            //Act
            var result = _foodRepo.UpdateFood(seeded.id, null, newTypeId);

            //Assert
            result.Should().NotBeNull();
            result!.foodTypeId.Should().Be(newTypeId);
            result.name.Should().Be(seeded.name);
        }

        /// <summary>Happy path: both name and food type are updated together.</summary>
        [Fact]
        public void UpdateFood_NameAndFoodType_ReturnsUpdatedFood()
        {
            //Arrange
            Guid newTypeId = SeedFoodType();
            var seeded = SeedFood(SeedFoodType());
            string newName = UniqueName("Updated");

            //Act
            var result = _foodRepo.UpdateFood(seeded.id, newName, newTypeId);

            //Assert
            result.Should().NotBeNull();
            result!.name.Should().Be(newName);
            result.foodTypeId.Should().Be(newTypeId);
        }

        /// <summary>Error case: no field is provided, so nothing is updated and null is returned.</summary>
        [Fact]
        public void UpdateFood_NoFieldsProvided_ReturnsNull()
        {
            //Arrange
            var seeded = SeedFood(SeedFoodType());

            //Act
            var result = _foodRepo.UpdateFood(seeded.id, null, null);

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: updating an unknown id affects no rows and returns null.</summary>
        [Fact]
        public void UpdateFood_NonExistingId_ReturnsNull()
        {
            //Act
            var result = _foodRepo.UpdateFood(Guid.NewGuid(), UniqueName("Updated"), null);

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: a name longer than nvarchar(100) fails silently and returns null.</summary>
        [Fact]
        public void UpdateFood_NameLongerThan100Chars_ReturnsNull()
        {
            //Arrange
            var seeded = SeedFood(SeedFoodType());

            //Act
            var result = _foodRepo.UpdateFood(seeded.id, new string('a', 101), null);

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: a food type that does not exist violates the foreign key and returns null.</summary>
        [Fact]
        public void UpdateFood_NonExistingFoodType_ReturnsNull()
        {
            //Arrange
            var seeded = SeedFood(SeedFoodType());

            //Act
            var result = _foodRepo.UpdateFood(seeded.id, null, Guid.NewGuid());

            //Assert
            result.Should().BeNull();
        }

        // =====================================================================
        // DeleteFood
        // =====================================================================

        /// <summary>Happy path: deleting an existing id removes the row and returns true.</summary>
        [Fact]
        public void DeleteFood_ExistingId_RemovesRow()
        {
            //Arrange
            var seeded = SeedFood(SeedFoodType());

            //Act
            bool result = _foodRepo.DeleteFood(seeded.id);

            //Assert
            result.Should().BeTrue();
            _foodRepo.GetFoodById(seeded.id).Should().BeNull();
        }

        /// <summary>Error case: deleting an unknown id deletes nothing, so it must report failure (false).</summary>
        [Fact]
        public void DeleteFood_NonExistingId_ReturnsFalse()
        {
            //Act
            bool result = _foodRepo.DeleteFood(Guid.NewGuid());

            //Assert
            result.Should().BeFalse();
        }

        /// <summary>Error case: Guid.Empty is not a valid id, so the deletion must report failure (false).</summary>
        [Fact]
        public void DeleteFood_EmptyGuid_ReturnsFalse()
        {
            //Act
            bool result = _foodRepo.DeleteFood(Guid.Empty);

            //Assert
            result.Should().BeFalse();
        }
    }
}