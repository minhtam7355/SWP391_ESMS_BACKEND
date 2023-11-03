using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IStudentRepository
    {
        public Task<List<StudentModel>> GetAllStudentsAsync();

        public Task<StudentModel> GetStudentByIdAsync(Guid id);

        public Task<Boolean> AddStudentAsync(StudentModel model);

        public Task<Boolean> UpdateStudentAsync(StudentModel model);

        public Task<Boolean> DeleteStudentAsync(StudentModel model);

    }
}
