using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IStepService
    {
        IEnumerable<Step> GetSteps(Guid? recipeId, int page = 1, int pageSize = 25);
        Step? GetStepById(Guid id);
        Step CreateStep(StepCreateRequest request);
        Step? UpdateStep(Guid id, StepUpdateRequest request);
        bool DeleteStep(Guid id);
    }

    public class StepService : IStepService
    {
        private readonly IStepRepository _stepRepo;
        public StepService(IStepRepository stepRepo)
        {
            _stepRepo = stepRepo;
        }

        public IEnumerable<Step> GetSteps(Guid? recipeId, int page = 1, int pageSize = 25)
        {
            return _stepRepo.GetSteps(recipeId, page, pageSize);
        }

        public Step? GetStepById(Guid id) => _stepRepo.GetStepById(id);

        public Step CreateStep(StepCreateRequest request)
        {
            if (!request.recipeId.HasValue)
            {
                throw new ArgumentException("recipeId is required");
            }
            return _stepRepo.CreateStep(request.recipeId.Value, request.instruction, request.stepNumber);
        }

        public Step? UpdateStep(Guid id, StepUpdateRequest request)
        {
            return _stepRepo.UpdateStep(id, request.recipeId, request.instruction, request.stepNumber);
        }

        public bool DeleteStep(Guid id) => _stepRepo.DeleteStep(id);
    }
}
