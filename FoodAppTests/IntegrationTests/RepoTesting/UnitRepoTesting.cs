using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Utilities;
using FluentAssertions;

namespace FoodAppTests.IntegrationTests.RepoTesting
{
    /// <summary>
    /// Integration tests for <see cref="UnitRepository"/>.
    /// They run against a real SQL Server container supplied by <see cref="DBMockup"/>.
    /// Rules for writing repository tests are described in Documentation/How_To_Make_Tests.md.
    /// </summary>
    [Collection("Database collection")]
    public class UnitRepoTesting
    {
        private readonly IUnitRepository _unitRepo;

        public UnitRepoTesting(DBMockup dBMockup)
        {
            DBConnector.SetConnectionString(dBMockup.ConnectionString);

            _unitRepo = new UnitRepository();
        }

        // =====================================================================
        // Arrange helpers - shared by many tests.
        // The database is shared by every test in the collection, so each seeded
        // value must be unique (a Guid suffix) to keep the tests independent.
        // =====================================================================

        /// <summary>Builds a unique name (prefix + guid) that always fits into nvarchar(50)/varchar(50).</summary>
        private static string UniqueName(string prefix = "Unit")
            => $"{prefix}_{Guid.NewGuid():N}";

        /// <summary>Creates a single unit and returns the persisted row (unique name by default).</summary>
        private Unit SeedUnit(decimal volumeEquivalent = 1m, string? desc = null, string? name = null)
            => _unitRepo.CreateUnit(name ?? UniqueName(), volumeEquivalent, desc);

        /// <summary>Creates one unit per suffix, all sharing the given unique prefix (for list / filter / paging).</summary>
        private List<Unit> SeedUnits(string namePrefix, params string[] suffixes)
        {
            var seeded = new List<Unit>();
            foreach (var suffix in suffixes)
            {
                seeded.Add(SeedUnit(name: $"{namePrefix}_{suffix}"));
            }
            return seeded;
        }

        // =====================================================================
        // CreateUnit
        // =====================================================================

        /// <summary>Happy path: a valid name, volume and description create a new unit row.</summary>
        [Fact]
        public void CreateUnit_NewUnit_ReturnsUnitObject()
        {
            //Arrange
            string unitName = UniqueName();

            //Act
            var result = _unitRepo.CreateUnit(unitName, 100m, "gram");

            //Assert
            result.Should().NotBeNull();
            result.name.Should().Be(unitName);
            result.volumeEquivalent.Should().Be(100m);
            result.desc.Should().Be("gram");
            result.id.Should().NotBe(Guid.Empty);
        }

        /// <summary>Error case: name is NOT NULL, a null name must be rejected.</summary>
        [Fact]
        public void CreateUnit_NullName_ThrowsException()
        {
            //Act
            Action act = () => _unitRepo.CreateUnit(null!, 1m, "desc");

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Error case: name is required, an empty name must be rejected.</summary>
        [Fact]
        public void CreateUnit_EmptyName_ThrowsException()
        {
            //Act
            Action act = () => _unitRepo.CreateUnit(string.Empty, 1m, "desc");

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Error case: name exceeds nvarchar(50); the truncation error is swallowed and the insert throws.</summary>
        [Fact]
        public void CreateUnit_NameLongerThan50Chars_ThrowsException()
        {
            //Act
            Action act = () => _unitRepo.CreateUnit(new string('a', 51), 1m, "desc");

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Error case: desc exceeds varchar(50); the truncation error is swallowed and the insert throws.</summary>
        [Fact]
        public void CreateUnit_DescLongerThan50Chars_ThrowsException()
        {
            //Act
            Action act = () => _unitRepo.CreateUnit(UniqueName(), 1m, new string('a', 51));

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Boundary case: desc is nullable, so a null description is accepted.</summary>
        [Fact]
        public void CreateUnit_NullDesc_ReturnsUnitObject()
        {
            //Arrange
            string unitName = UniqueName();

            //Act
            var result = _unitRepo.CreateUnit(unitName, 1m, null);

            //Assert
            result.name.Should().Be(unitName);
            result.desc.Should().BeNullOrEmpty();
        }

        /// <summary>Error case: volume exceeds decimal(10,4); the overflow error is swallowed and the insert throws.</summary>
        [Fact]
        public void CreateUnit_VolumeTooLarge_ThrowsException()
        {
            //Act
            Action act = () => _unitRepo.CreateUnit(UniqueName(), 10_000_000m, "desc");

            //Assert
            act.Should().Throw<Exception>();
        }

        /// <summary>Boundary case: a 50 char name and the largest decimal(10,4) volume are accepted.</summary>
        [Fact]
        public void CreateUnit_MaxLengthAndMaxVolume_ReturnsUnitObject()
        {
            //Arrange
            string maxName = new string('c', 50);

            //Act
            var result = _unitRepo.CreateUnit(maxName, 999999.9999m, "desc");

            //Assert
            result.name.Should().Be(maxName);
            result.volumeEquivalent.Should().Be(999999.9999m);
        }

        // =====================================================================
        // GetUnitById
        // =====================================================================

        /// <summary>Happy path: an existing id returns the matching unit row.</summary>
        [Fact]
        public void GetUnitById_ExistingId_ReturnsUnitObject()
        {
            //Arrange
            var seeded = SeedUnit(volumeEquivalent: 250m, desc: "millilitre");

            //Act
            var result = _unitRepo.GetUnitById(seeded.id);

            //Assert
            result.Should().NotBeNull();
            result!.id.Should().Be(seeded.id);
            result.name.Should().Be(seeded.name);
            result.volumeEquivalent.Should().Be(250m);
        }

        /// <summary>Error case: an unknown id returns null instead of throwing.</summary>
        [Fact]
        public void GetUnitById_NonExistingId_ReturnsNull()
        {
            //Act
            var result = _unitRepo.GetUnitById(Guid.NewGuid());

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: Guid.Empty is never a valid row id and returns null.</summary>
        [Fact]
        public void GetUnitById_EmptyGuid_ReturnsNull()
        {
            //Act
            var result = _unitRepo.GetUnitById(Guid.Empty);

            //Assert
            result.Should().BeNull();
        }

        // =====================================================================
        // GetUnits
        // =====================================================================

        /// <summary>Happy path: the search filter matches the name and returns only those units.</summary>
        [Fact]
        public void GetUnits_ByNameFilter_ReturnsOnlyMatchingItems()
        {
            //Arrange
            string prefix = UniqueName("Filtered");
            SeedUnits(prefix, "A", "B");
            SeedUnit();

            //Act
            var result = _unitRepo.GetUnits(prefix, Array.Empty<Guid>()).ToList();

            //Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(unit => unit.name.StartsWith(prefix));
        }

        /// <summary>Happy path: the search filter also matches the description.</summary>
        [Fact]
        public void GetUnits_ByDescFilter_ReturnsMatchingItem()
        {
            //Arrange
            string descMarker = UniqueName("Desc");
            var seeded = SeedUnit(desc: descMarker, name: UniqueName("Named"));

            //Act
            var result = _unitRepo.GetUnits(descMarker, Array.Empty<Guid>()).ToList();

            //Assert
            result.Should().ContainSingle();
            result[0].id.Should().Be(seeded.id);
        }

        /// <summary>Happy path: when unit ids are provided, only those units are returned.</summary>
        [Fact]
        public void GetUnits_ByUnitIds_ReturnsOnlyThoseUnits()
        {
            //Arrange
            var seeded = SeedUnits(UniqueName("ById"), "A", "B", "C");
            Guid[] wantedIds = [seeded[0].id, seeded[2].id];

            //Act
            var result = _unitRepo.GetUnits(null, wantedIds).ToList();

            //Assert
            result.Should().HaveCount(2);
            result.Select(unit => unit.id).Should().BeEquivalentTo(wantedIds);
        }

        /// <summary>
        /// Boundary case: a null unitIds array means "skip this parameter" (see the repository docs)
        /// and must not throw.
        /// </summary>
        [Fact]
        public void GetUnits_NullUnitIds_DoesNotThrow()
        {
            //Act
            Action act = () => _unitRepo.GetUnits(null, null).ToList();

            //Assert
            act.Should().NotThrow();
        }

        /// <summary>Boundary case: an empty unitIds array means "skip this parameter".</summary>
        [Fact]
        public void GetUnits_EmptyUnitIds_IsIgnored()
        {
            //Arrange
            string prefix = UniqueName("EmptyIds");
            SeedUnits(prefix, "A", "B");

            //Act
            var result = _unitRepo.GetUnits(prefix, Array.Empty<Guid>()).ToList();

            //Assert
            result.Should().HaveCount(2);
        }

        /// <summary>Error case: a filter that matches nothing returns an empty collection.</summary>
        [Fact]
        public void GetUnits_NonMatchingFilter_ReturnsEmpty()
        {
            //Arrange
            string missingPrefix = UniqueName("NoMatch");

            //Act
            var result = _unitRepo.GetUnits(missingPrefix, Array.Empty<Guid>());

            //Assert
            result.Should().BeEmpty();
        }

        /// <summary>Boundary case: paging returns the requested slice and an empty last page.</summary>
        [Fact]
        public void GetUnits_Paging_ReturnsExpectedPageSizes()
        {
            //Arrange
            string prefix = UniqueName("Paging");
            SeedUnits(prefix, "1", "2", "3", "4", "5");

            //Act
            var page1 = _unitRepo.GetUnits(prefix, Array.Empty<Guid>(), page: 1, pageSize: 2).ToList();
            var page2 = _unitRepo.GetUnits(prefix, Array.Empty<Guid>(), page: 2, pageSize: 2).ToList();
            var page3 = _unitRepo.GetUnits(prefix, Array.Empty<Guid>(), page: 3, pageSize: 2).ToList();
            var page4 = _unitRepo.GetUnits(prefix, Array.Empty<Guid>(), page: 4, pageSize: 2).ToList();

            //Assert
            page1.Should().HaveCount(2);
            page2.Should().HaveCount(2);
            page3.Should().HaveCount(1);
            page4.Should().BeEmpty();
            page1.Select(unit => unit.id).Should().NotIntersectWith(page2.Select(unit => unit.id));
        }

        /// <summary>Happy path: results come back ordered by name ascending.</summary>
        [Fact]
        public void GetUnits_ReturnsOrderedByNameAscending()
        {
            //Arrange
            string prefix = UniqueName("Order");
            SeedUnits(prefix, "C", "A", "B");

            //Act
            var result = _unitRepo.GetUnits(prefix, Array.Empty<Guid>()).ToList();

            //Assert
            result.Select(unit => unit.name).Should().BeInAscendingOrder();
        }

        // =====================================================================
        // UpdateUnit
        // =====================================================================

        /// <summary>Happy path: a new name is applied and the other fields are kept.</summary>
        [Fact]
        public void UpdateUnit_NameOnly_ReturnsUpdatedUnit()
        {
            //Arrange
            var seeded = SeedUnit(volumeEquivalent: 5m, desc: "desc");
            string newName = UniqueName("Updated");

            //Act
            var result = _unitRepo.UpdateUnit(seeded.id, newName, null, null);

            //Assert
            result.Should().NotBeNull();
            result!.name.Should().Be(newName);
            result.volumeEquivalent.Should().Be(5m);
            result.desc.Should().Be("desc");
        }

        /// <summary>Happy path: a new volume is applied and the other fields are kept.</summary>
        [Fact]
        public void UpdateUnit_VolumeOnly_ReturnsUpdatedUnit()
        {
            //Arrange
            var seeded = SeedUnit(volumeEquivalent: 5m, desc: "desc");

            //Act
            var result = _unitRepo.UpdateUnit(seeded.id, null, 42m, null);

            //Assert
            result.Should().NotBeNull();
            result!.volumeEquivalent.Should().Be(42m);
            result.name.Should().Be(seeded.name);
        }

        /// <summary>Happy path: a new description is applied and the other fields are kept.</summary>
        [Fact]
        public void UpdateUnit_DescOnly_ReturnsUpdatedUnit()
        {
            //Arrange
            var seeded = SeedUnit(volumeEquivalent: 5m, desc: "old");

            //Act
            var result = _unitRepo.UpdateUnit(seeded.id, null, null, "new");

            //Assert
            result.Should().NotBeNull();
            result!.desc.Should().Be("new");
            result.name.Should().Be(seeded.name);
        }

        /// <summary>Happy path: name, volume and description are updated together.</summary>
        [Fact]
        public void UpdateUnit_AllFields_ReturnsUpdatedUnit()
        {
            //Arrange
            var seeded = SeedUnit();
            string newName = UniqueName("Updated");

            //Act
            var result = _unitRepo.UpdateUnit(seeded.id, newName, 3.5m, "all");

            //Assert
            result.Should().NotBeNull();
            result!.name.Should().Be(newName);
            result.volumeEquivalent.Should().Be(3.5m);
            result.desc.Should().Be("all");

            _unitRepo.GetUnitById(seeded.id)!.name.Should().Be(newName);
        }

        /// <summary>Error case: no field is provided, so nothing is updated and null is returned.</summary>
        [Fact]
        public void UpdateUnit_NoFieldsProvided_ReturnsNull()
        {
            //Arrange
            var seeded = SeedUnit();

            //Act
            var result = _unitRepo.UpdateUnit(seeded.id, null, null, null);

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: updating an unknown id affects no rows and returns null.</summary>
        [Fact]
        public void UpdateUnit_NonExistingId_ReturnsNull()
        {
            //Act
            var result = _unitRepo.UpdateUnit(Guid.NewGuid(), UniqueName("Updated"), 1m, null);

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: a name longer than nvarchar(50) fails silently and returns null.</summary>
        [Fact]
        public void UpdateUnit_NameLongerThan50Chars_ReturnsNull()
        {
            //Arrange
            var seeded = SeedUnit();

            //Act
            var result = _unitRepo.UpdateUnit(seeded.id, new string('a', 51), null, null);

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: a description longer than varchar(50) fails silently and returns null.</summary>
        [Fact]
        public void UpdateUnit_DescLongerThan50Chars_ReturnsNull()
        {
            //Arrange
            var seeded = SeedUnit();

            //Act
            var result = _unitRepo.UpdateUnit(seeded.id, null, null, new string('a', 51));

            //Assert
            result.Should().BeNull();
        }

        /// <summary>Error case: a volume larger than decimal(10,4) fails silently and returns null.</summary>
        [Fact]
        public void UpdateUnit_VolumeTooLarge_ReturnsNull()
        {
            //Arrange
            var seeded = SeedUnit();

            //Act
            var result = _unitRepo.UpdateUnit(seeded.id, null, 10_000_000m, null);

            //Assert
            result.Should().BeNull();
        }

        // =====================================================================
        // DeleteUnit
        // =====================================================================

        /// <summary>Happy path: deleting an existing id removes the row and returns true.</summary>
        [Fact]
        public void DeleteUnit_ExistingId_RemovesRow()
        {
            //Arrange
            var seeded = SeedUnit();

            //Act
            bool result = _unitRepo.DeleteUnit(seeded.id);

            //Assert
            result.Should().BeTrue();
            _unitRepo.GetUnitById(seeded.id).Should().BeNull();
        }

        /// <summary>Error case: deleting an unknown id deletes nothing, so it must report failure (false).</summary>
        [Fact]
        public void DeleteUnit_NonExistingId_ReturnsFalse()
        {
            //Act
            bool result = _unitRepo.DeleteUnit(Guid.NewGuid());

            //Assert
            result.Should().BeFalse();
        }

        /// <summary>Error case: Guid.Empty is not a valid id, so the deletion must report failure (false).</summary>
        [Fact]
        public void DeleteUnit_EmptyGuid_ReturnsFalse()
        {
            //Act
            bool result = _unitRepo.DeleteUnit(Guid.Empty);

            //Assert
            result.Should().BeFalse();
        }

        // =====================================================================
        // ConvertUnit
        // =====================================================================

        /// <summary>Happy path: the amount is scaled by the ratio of the two volumes.</summary>
        [Fact]
        public void ConvertUnit_ValidUnits_ReturnsConvertedAmount()
        {
            //Arrange
            var gram = SeedUnit(volumeEquivalent: 1m);
            var kilogram = SeedUnit(volumeEquivalent: 1000m);

            //Act
            var result = _unitRepo.ConvertUnit(gram.id, kilogram.id, 500f);

            //Assert
            result.Should().NotBeNull();
            result.newUnitAmount.Should().BeApproximately(0.5f, 0.0001f);
        }

        /// <summary>Boundary case: converting a unit to itself keeps the amount unchanged.</summary>
        [Fact]
        public void ConvertUnit_SameUnit_ReturnsSameAmount()
        {
            //Arrange
            var unit = SeedUnit(volumeEquivalent: 250m);

            //Act
            var result = _unitRepo.ConvertUnit(unit.id, unit.id, 3f);

            //Assert
            result.newUnitAmount.Should().BeApproximately(3f, 0.0001f);
        }

        /// <summary>Error case: an unknown source or target id fails the conversion and throws.</summary>
        [Fact]
        public void ConvertUnit_NonExistingUnit_ThrowsInvalidOperationException()
        {
            //Arrange
            var unit = SeedUnit(volumeEquivalent: 1m);

            //Act
            Action act = () => _unitRepo.ConvertUnit(unit.id, Guid.NewGuid(), 1f);

            //Assert
            act.Should().Throw<InvalidOperationException>();
        }

        /// <summary>Error case: both ids unknown also fails the conversion and throws.</summary>
        [Fact]
        public void ConvertUnit_BothUnitsNonExisting_ThrowsInvalidOperationException()
        {
            //Act
            Action act = () => _unitRepo.ConvertUnit(Guid.NewGuid(), Guid.NewGuid(), 1f);

            //Assert
            act.Should().Throw<InvalidOperationException>();
        }
    }
}