using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IFoodService
    {
        IEnumerable<Food> GetFoods(int? typeId, string nameFilter, int page = 1, int pageSize = 25);
        Food? GetFoodById(int id);
        Food CreateFood(CreteFoodRequest request);
        Food? UpdateFood(int id, UpdateFoodRequest request);
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
        public Food? GetFoodById(int id)
        {
            var food = _repo.GetFoodById(id);
            var foodType = food != null ? _foodTypeRepo.GetFoodTypeById(food.foodTypeId) : null;
            food!.foodType = foodType;
            return food;
        }

        public Food CreateFood(CreteFoodRequest request)
        {
            int? foodTypeId = request.foodTypeId;

            if (foodTypeId.HasValue && foodTypeId.Value > 0 && request.foodType != null)
            {
                throw new ArgumentException("you could specify onle foodTypeId or crete object");
            }

            if ((!foodTypeId.HasValue || foodTypeId <=0) && request.foodType != null)
            {
                var createdType = _foodTypeRepo.CreateFoodType(request.foodType.name);
                foodTypeId = createdType.id;
            }

                var result = _repo.CreateFood(request.name, foodTypeId ?? 0);
            return result;
        }

        public Food? UpdateFood(int id, UpdateFoodRequest request)
        {
            int? foodTypeId = request.foodTypeId;
            Console.WriteLine(foodTypeId);
            if ((!foodTypeId.HasValue || foodTypeId.Value <=0) && request.foodType != null)
            {
                var createdType = _foodTypeRepo.CreateFoodType(request.foodType.name);
                foodTypeId = createdType.id;
            }

            if(foodTypeId==0)
            {
                foodTypeId = null;
            }

            var updated = _repo.UpdateFood(id, request.name, foodTypeId);
            return updated;
        }

        public bool DeleteFood(int id) => _repo.DeleteFood(id);
    }
}
