using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IAccessRepository
    {
        public Task<UserInfo?> Login(LoginModel model);

        public Task<Boolean> Signup(SignupModel model);
    }
}
