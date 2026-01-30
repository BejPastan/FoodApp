using FoodApp.Models;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface ITagService
    {
        IEnumerable<Tag> GetTags(string nameFilter);
        Tag? GetTagById(int id);
        Tag CreateTag(Tag tagRequest);
        Tag? UpdateTag(Tag request);
        bool DeleteTag(int id);
    }

    public class TagService : ITagService
    {
        private readonly ITagRepository _repo;
        public TagService(ITagRepository repo) { _repo = repo; }

        public IEnumerable<Tag> GetTags(string nameFilter) => _repo.GetTags(nameFilter);
        public Tag? GetTagById(int id) => _repo.GetTagById(id);
        public Tag CreateTag(Tag name)
        {
            return _repo.CreateTag(name.name);
        }
        public Tag? UpdateTag(Tag request)
        {
            return _repo.UpdateTag(request.id, request.name);
        }
        public bool DeleteTag(int id) => _repo.DeleteTag(id);
    }
}
