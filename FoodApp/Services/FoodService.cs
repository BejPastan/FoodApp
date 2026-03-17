using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IFoodService
    {
        IEnumerable<Food> GetFoods(int? typeId, string nameFilter, int page = 1, int pageSize = 25);
        Food? GetFoodById(int id);
        Food CreateFood(Food request);
        Food? UpdateFood(Food request);
        bool DeleteFood(int id);
    }

    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _repo;
        private readonly IFoodTypeRepository _foodTypeRepo;
        public FoodService(IFoodRepository repo, IFoodTypeRepository foodTypeRepo)
        { 
            _repo = repo;
            _foodTypeRepo = foodTypeRepo;
        }
        public IEnumerable<Food> GetFoods(int? typeId, string nameFilter, int page = 1, int pageSize = 25)
        {
            return _repo.GetFoods(typeId, nameFilter, page, pageSize);
        }
        public Food? GetFoodById(int id) => _repo.GetFoodById(id);

        public Food CreateFood(Food request)
        {
            int? foodTypeId = request.foodTypeId;

            if ((foodTypeId == null || foodTypeId <=0) && request.foodType != null)
            {
                var createdType = _foodTypeRepo.CreateFoodType(request.foodType.name);
                foodTypeId = createdType.id;
            }

            var result = _repo.CreateFood(request.name, foodTypeId ??0);
            return result;
        }

        public Food? UpdateFood(Food request)
        {
            int? foodTypeId = request.foodTypeId;

            if ((foodTypeId == null || foodTypeId <=0) && request.foodType != null)
            {
                var createdType = _foodTypeRepo.CreateFoodType(request.foodType.name);
                foodTypeId = createdType.id;
            }

            var updated = _repo.UpdateFood(request.id, request.name, foodTypeId);
            return updated;
        }

        public bool DeleteFood(int id) => _repo.DeleteFood(id);
    }
}
