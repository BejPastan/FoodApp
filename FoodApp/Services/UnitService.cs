using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IUnitService
    {
        IEnumerable<Unit> GetUnits(string nameFilter, Guid[] ids, int page = 1, int pageSize = 25);
        Unit? GetUnitById(Guid id);
        Unit CreateUnit(UnitCreateRequest request);
        Unit? UpdateUnit(Guid id, UnitUpdateRequest request);
        bool DeleteUnit(Guid id);
        UnitConvertResp ConvertUnit(Guid oldUnitId, Guid newUnitId, float originalAmount);
    }

    /// <summary>
    /// Serviceto manage units, andconvert them
    /// </summary>
    /// <param name="repo"></param>
    public class UnitService(IUnitRepository repo) : IUnitService
    {
        private readonly IUnitRepository _repo = repo;

        public IEnumerable<Unit> GetUnits(string nameFilter, Guid[] ids, int page = 1, int pageSize = 25)
        {
            Console.WriteLine(ids.Length);
            return _repo.GetUnits(nameFilter, ids, page, pageSize);
        }
        public Unit? GetUnitById(Guid id) => _repo.GetUnitById(id);
        
        public Unit? UpdateUnit(Guid id, UnitUpdateRequest request)
        {
            return _repo.UpdateUnit(id, request.name, request.volumeEquivalent, request.desc);
        }
        public bool DeleteUnit(Guid id) => _repo.DeleteUnit(id);
        public Unit CreateUnit(UnitCreateRequest request)
        {
            return _repo.CreateUnit(request.name, request.volumeEquivalent, request.desc);
        }

        public UnitConvertResp ConvertUnit(Guid oldUnitId, Guid newUnitId, float originalAmount)
        {
            return _repo.ConvertUnit(oldUnitId, newUnitId, originalAmount);
        }
    }
}
