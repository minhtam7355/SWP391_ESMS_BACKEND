using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IStaffRepository
    {
        public Task<List<StaffModel>> GetAllStaffAsync();

        public Task<StaffModel> GetStaffByIdAsync(Guid id);

        public Task<Boolean> AddStaffAsync(AddStaffModel model);

        public Task<Boolean> UpdateStaffAsync(Guid id, UpdateStaffModel model);

        public Task<Boolean> DeleteStaffAsync(Guid id);
    }
}
