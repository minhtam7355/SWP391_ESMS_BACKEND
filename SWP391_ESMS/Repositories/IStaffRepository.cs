using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IStaffRepository
    {
        public Task<List<StaffModel>> GetAllStaffAsync();

        public Task<StaffModel> GetStaffByIdAsync(Guid id);

        public Task<Boolean> AddStaffAsync(StaffModel model);

        public Task<Boolean> UpdateStaffAsync(StaffModel model);

        public Task<Boolean> DeleteStaffAsync(StaffModel model);
    }
}
