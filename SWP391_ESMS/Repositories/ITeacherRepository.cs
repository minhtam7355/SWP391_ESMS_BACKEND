using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface ITeacherRepository
    {
        public Task<List<TeacherModel>> GetAllTeachersAsync();

        public Task<TeacherModel> GetTeacherByIdAsync(Guid id);

        public Task<Boolean> AddTeacherAsync(TeacherModel model);

        public Task<Boolean> UpdateTeacherAsync(TeacherModel model);

        public Task<Boolean> DeleteTeacherAsync(Guid id);

        public Task<TeacherModel?> GetTeacherByExamSessionAsync(Guid examSessionId);
    }
}
