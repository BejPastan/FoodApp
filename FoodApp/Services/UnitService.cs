using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IUnitService
    {
        IEnumerable<Unit> GetUnits(string nameFilter, int[] ids, int page = 1, int pageSize = 25);
        Unit? GetUnitById(int id);
        Unit CreateUnit(UnitCreateRequest request);
        Unit? UpdateUnit(int id, UnitUpdateRequest request);
        bool DeleteUnit(int id);
        UnitConvertResp ConvertUnit(int oldUnitId, int newUnitId, float originalAmount);
    }

    /// <summary>
    /// Serviceto manage units, andconvert them
    /// </summary>
    /// <param name="repo"></param>
    public class UnitService(IUnitRepository repo) : IUnitService
    {
        private readonly IUnitRepository _repo = repo;

        public IEnumerable<Unit> GetUnits(string nameFilter, int[] ids, int page = 1, int pageSize = 25)
        {
            Console.WriteLine(ids.Length);
            return _repo.GetUnits(nameFilter, ids, page, pageSize);
        }
        public Unit? GetUnitById(int id) => _repo.GetUnitById(id);
        
        public Unit? UpdateUnit(int id, UnitUpdateRequest request)
        {
            return _repo.UpdateUnit(id, request.name, request.volumeEquivalent, request.desc);
        }
        public bool DeleteUnit(int id) => _repo.DeleteUnit(id);

        public Unit CreateUnit(UnitCreateRequest request)
        {
            return _repo.CreateUnit(request.name, request.volumeEquivalent, request.desc);
        }

        public UnitConvertResp ConvertUnit(int oldUnitId, int newUnitId, float originalAmount)
        {
            return _repo.ConvertUnit(oldUnitId, newUnitId, originalAmount);
        }
    }
}
