using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IUnitService
    {
        IEnumerable<Unit> GetUnits(string nameFilter, int page = 1, int pageSize = 25);
        Unit? GetUnitById(int id);
        Unit CreateUnit(Unit request);
        Unit? UpdateUnit(Unit request);
        bool DeleteUnit(int id);
        UnitConvertResp ConvertUnit(int oldUnitId, int newUnitId, float originalAmount);
    }

    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _repo;
        public UnitService(IUnitRepository repo) { _repo = repo; }

        public IEnumerable<Unit> GetUnits(string nameFilter, int page = 1, int pageSize = 25) => _repo.GetUnits(nameFilter, page, pageSize);
        public Unit? GetUnitById(int id) => _repo.GetUnitById(id);
        public Unit CreateUnit(string name, decimal volumeEquivalent) => _repo.CreateUnit(name, volumeEquivalent);
        public Unit? UpdateUnit(Unit reques)
        {
            return _repo.UpdateUnit(reques.id, reques.name, reques.volumeEquivalent);
        }
        public bool DeleteUnit(int id) => _repo.DeleteUnit(id);

        public Unit CreateUnit(Unit request)
        {
            return _repo.CreateUnit(request.name, request.volumeEquivalent);
        }

        public UnitConvertResp ConvertUnit(int oldUnitId, int newUnitId, float originalAmount)
        {
            return _repo.ConvertUnit(oldUnitId, newUnitId, originalAmount);
        }
    }
}
