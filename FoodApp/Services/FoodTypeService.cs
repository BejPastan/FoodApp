using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IFoodTypeService
    {
        IEnumerable<FoodType> GetFoodTypes(string nameFilter, int page = 1, int pageSize = 25);
        FoodType? GetFoodTypeById(int id);
        FoodType CreateFoodType(FoodType request);

        /// <summary>
        /// Patch food type
        /// </summary>
        /// <param name="id">id of food type to patch</param>
        /// <param name="request">Food type object with data to patch</param>
        /// <returns></returns>
        FoodType? UpdateFoodType(int id, FoodTypeUpdateRequest request);
        bool DeleteFoodType(int id);
    }

    public class FoodTypeService(IFoodTypeRepository repo) : IFoodTypeService
    {
        private readonly IFoodTypeRepository _repo = repo;

        public IEnumerable<FoodType> GetFoodTypes(string nameFilter, int page = 1, int pageSize = 25) => _repo.GetFoodTypes(nameFilter, page, pageSize);
        public FoodType? GetFoodTypeById(int id) => _repo.GetFoodTypeById(id);
        public FoodType CreateFoodType(FoodType request)
        {
            return _repo.CreateFoodType(request.name);
        }

        /// <summary>
        /// Patch food type
        /// </summary>
        /// <param name="id">id of food type to patch</param>
        /// <param name="request">Food type object with data to patch</param>
        /// <returns></returns>
        public FoodType? UpdateFoodType(int id, FoodTypeUpdateRequest request)
        {
            return _repo.UpdateFoodType(id, request.name);
        }
        public bool DeleteFoodType(int id) => _repo.DeleteFoodType(id);
    }
}
