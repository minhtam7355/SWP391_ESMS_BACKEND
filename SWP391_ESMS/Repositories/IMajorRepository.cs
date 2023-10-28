using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IMajorRepository
    {
        public Task<List<MajorModel>> GetAllMajorsAsync();

        public Task<MajorModel> GetMajorByIdAsync(Guid id);

        public Task<Boolean> AddMajorAsync(AddMajorModel model);

        public Task<Boolean> UpdateMajorAsync(Guid id, UpdateMajorModel model);

        public Task<Boolean> DeleteMajorAsync(Guid id);

        public Task<Guid> GetMajorIdByName(string majorName);
    }
}
