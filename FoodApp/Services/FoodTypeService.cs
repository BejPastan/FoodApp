using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IFoodTypeService
    {
        IEnumerable<FoodType> GetFoodTypes(string nameFilter, int page = 1, int pageSize = 25);
        FoodType? GetFoodTypeById(int id);
        FoodType CreateFoodType(FoodType request);
        FoodType? UpdateFoodType(FoodType request);
        bool DeleteFoodType(int id);
    }

    public class FoodTypeService : IFoodTypeService
    {
        private readonly IFoodTypeRepository _repo;
        public FoodTypeService(IFoodTypeRepository repo) { _repo = repo; }

        public IEnumerable<FoodType> GetFoodTypes(string nameFilter, int page = 1, int pageSize = 25) => _repo.GetFoodTypes(nameFilter, page, pageSize);
        public FoodType? GetFoodTypeById(int id) => _repo.GetFoodTypeById(id);
        public FoodType CreateFoodType(FoodType request)
        {
            return _repo.CreateFoodType(request.name);
        }
        public FoodType? UpdateFoodType(FoodType request)
        {
            return _repo.UpdateFoodType(request.id, request.name);
        }
        public bool DeleteFoodType(int id) => _repo.DeleteFoodType(id);
    }
}
