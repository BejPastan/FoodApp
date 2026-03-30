using Azure;
using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface ITagService
    {
        IEnumerable<Tag> GetTags(string nameFilter, int page = 1, int perPage = 25);
        Tag? GetTagById(int id);
        Tag CreateTag(TagCreateRequest tagRequest);
        Tag? UpdateTag(int id, TagUpdateRequest request);
        bool DeleteTag(int id);
    }

    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;
        public TagService(ITagRepository repo) { _repo = repo; }

        public IEnumerable<Tag> GetTags(string nameFilter, int page = 1, int perPage = 25)
        {
            return _repo.GetTags(nameFilter, page, perPage);
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
    }
}
