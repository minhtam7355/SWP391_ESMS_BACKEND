using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IExamSessionRepository
    {
        public Task<List<ExamSessionModel>> GetAllExamSessionsAsync();

        public Task<ExamSessionModel> GetExamSessionByIdAsync(Guid id);

        public Task<Boolean> AddExamSessionAsync(ExamSessionModel model);

        public Task<Boolean> UpdateExamSessionAsync(ExamSessionModel model);

        public Task<Boolean> DeleteExamSessionAsync(ExamSessionModel model);

        public Task<List<ExamSessionModel>?> GetExamSessionsByStudentAsync(Guid studentId);

        public Task<Boolean> RemoveStudentFromExamSessionAsync(Guid examSessionId, Guid studentId);

        public Task<Boolean> AddStudentToExamSessionAsync(Guid examSessionId, Guid studentId);

        public Task<Boolean> AddTeacherToExamSessionAsync(Guid examSessionId, Guid teacherId);

        public Task<Boolean> RemoveTeacherFromExamSessionAsync(Guid examSessionId);
    }
}
