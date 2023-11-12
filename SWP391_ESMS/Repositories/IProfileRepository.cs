using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IProfileRepository
    {
        public Task<UserInfo?> GetUserProfileAsync(Guid id, string role);

        public Task<Boolean> UpdateUserProfileAsync(UserInfo model);

        public Task<Boolean> ChangePasswordAsync(ChangePasswordModel model, Guid id, string role);

        public Task<Boolean> SaveProfilePictureAsync(Guid id, string role, string base64Image);

        public Task<bool> IsUsernameAvailableAsync(string username);

        public Task<bool> IsEmailAvailableAsync(string email);
    }
}
