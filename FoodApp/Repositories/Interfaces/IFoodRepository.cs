using FoodApp.Models;

public interface IFoodRepository
    {
        IEnumerable<Food> GetFoods(Guid? typeId, string nameFilter, int page = 1, int pageSize = 25);
        Food? GetFoodById(Guid id);
        Food CreateFood(string name, Guid foodTypeId);
        Food? UpdateFood(Guid id, string? name, Guid? foodTypeId);
        bool DeleteFood(Guid id);
    }