using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IStepService
    {
        IEnumerable<Step> GetSteps(int? recipeId, int page = 1, int pageSize = 25);
        Step? GetStepById(int id);
        Step CreateStep(Step request);
        Step? UpdateStep(Step request);
        bool DeleteStep(int id);
    }

    public class StepService : IStepService
    {
        private readonly IStepRepository _stepRepo;
        public StepService(IStepRepository stepRepo)
        {
            _stepRepo = stepRepo;
        }

        public IEnumerable<Step> GetSteps(int? recipeId, int page = 1, int pageSize = 25)
        {
            return _stepRepo.GetSteps(recipeId, page, pageSize);
        }

        public Step? GetStepById(int id) => _stepRepo.GetStepById(id);

        public Step CreateStep(Step request)
        {
            return _stepRepo.CreateStep(request.recipeId, request.instruction, request.stepNumber);
        }

        public Step? UpdateStep(Step request)
        {
            return _stepRepo.UpdateStep(request.id, request.recipeId, request.instruction, request.stepNumber);
        }

        public bool DeleteStep(int id) => _stepRepo.DeleteStep(id);
    }
}
