using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IStudentRepository
    {
        public Task<List<StudentModel>> GetAllStudentsAsync();

        public Task<StudentModel> GetStudentByIdAsync(Guid id);

        public Task<Boolean> AddStudentAsync(StudentModel model);

        public Task<Boolean> UpdateStudentAsync(StudentModel model);

        public Task<Boolean> DeleteStudentAsync(Guid id);

        public Task<List<StudentModel>?> GetStudentsByCourseAsync(Guid courseId);

        public Task<List<StudentModel>?> GetStudentsByExamSessionAsync(Guid examSessionId);

        public Task<List<StudentModel>?> GetUnassignedStudentsAsync(Guid courseId);

        public Task<Dictionary<string, double>> GetMajorDistributionAsync();
    }
}
