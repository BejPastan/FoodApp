using Azure;
using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface ITagService
    {
        IEnumerable<Tag> GetTags(string nameFilter="", int? recipeId=null, int page = 1, int perPage = 25);
        Tag? GetTagById(int id);
        Tag CreateTag(TagCreateRequest tagRequest);
        Tag? UpdateTag(int id, TagUpdateRequest request);
        bool DeleteTag(int id);
    }

    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;
        public TagService(ITagRepository repo) { _repo = repo; }

        /// <summary>
        /// Return tags using given filter
        /// </summary>
        /// <param name="nameFilter"></param>
        /// <param name="recipeId"></param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns></returns>
        public IEnumerable<Tag> GetTags(string nameFilter="", int? recipeId=null, int page = 1, int perPage = 25)
        {
            Console.WriteLine(recipeId);
            Console.WriteLine("This is in GetTags");
            return _repo.GetTags(nameFilter, recipeId, page, perPage);
        }
        public Tag? GetTagById(int id) => _repo.GetTagById(id);
        public Tag CreateTag(TagCreateRequest request)
        {
            return _repo.CreateTag(request.name);
        }
        public Tag? UpdateTag(int id, TagUpdateRequest request)
        {
            return _repo.UpdateTag(id, request.name);
        }
        public bool DeleteTag(int id) => _repo.DeleteTag(id);

        public IEnumerable<Tag> GetTagsByRecipe(int recipeId)
        {
            throw new NotImplementedException();
        }
    }
}
