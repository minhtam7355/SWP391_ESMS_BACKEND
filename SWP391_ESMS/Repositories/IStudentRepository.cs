using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IStudentRepository
    {
        public Task<List<StudentModel>> GetAllStudentsAsync();

        public Task<StudentModel> GetStudentByIdAsync(Guid id);

        public Task<Boolean> AddStudentAsync(AddStudentModel model);

        public Task<Boolean> UpdateStudentAsync(Guid id, UpdateStudentModel model);

        public Task<Boolean> DeleteStudentAsync(Guid id);

    }
}
