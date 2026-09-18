using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IFoodService
    {
        IEnumerable<Food> GetFoods(Guid? typeId, string nameFilter, int page = 1, int pageSize = 25);
        Food? GetFoodById(Guid id);
        Food CreateFood(CreateFoodRequest request);
        Food? UpdateFood(Guid id, UpdateFoodRequest request);
        bool DeleteFood(Guid id);
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

        /// <inheritdoc/>
        public IEnumerable<Food> GetFoods(Guid? typeId, string nameFilter, int page = 1, int pageSize = 25)
        {
            return _repo.GetFoods(typeId, nameFilter, page, pageSize);
        }

        /// <inheritdoc/>
        public Food? GetFoodById(Guid id)
        {
            var food = _repo.GetFoodById(id);
            var foodType = food != null ? _foodTypeRepo.GetFoodTypeById(food.foodTypeId) : null;
            food!.foodType = foodType;
            return food;
        }

        /// <inheritdoc/>
        public Food CreateFood(CreateFoodRequest request)
        {
            Guid? foodTypeId = request.foodTypeId;

            if (foodTypeId.HasValue && foodTypeId.Value != Guid.Empty && request.foodType != null)
            {
                throw new ArgumentException("you could specify onle foodTypeId or crete object");
            }

            if ((!foodTypeId.HasValue || foodTypeId == Guid.Empty) && request.foodType != null)
            {
                var createdType = _foodTypeRepo.CreateFoodType(request.foodType.name);
                foodTypeId = createdType.id;
            }

                var result = _repo.CreateFood(request.name, foodTypeId ?? Guid.Empty);
            return result;
        }

        public Food? UpdateFood(Guid id, UpdateFoodRequest request)
        {
            Guid? foodTypeId = request.foodTypeId;
            Console.WriteLine(foodTypeId);
            if ((!foodTypeId.HasValue || foodTypeId.Value == Guid.Empty) && request.foodType != null)
            {
                var createdType = _foodTypeRepo.CreateFoodType(request.foodType.name);
                foodTypeId = createdType.id;
            }

            if(foodTypeId== Guid.Empty)
            {
                foodTypeId = null;
            }

            var updated = _repo.UpdateFood(id, request.name, foodTypeId);
            return updated;
        }

        public bool DeleteFood(Guid id) => _repo.DeleteFood(id);
    }
}
