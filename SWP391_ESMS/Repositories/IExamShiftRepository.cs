using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IExamShiftRepository
    {
        public Task<List<ExamShiftModel>> GetAllExamShiftsAsync();

        public Task<ExamShiftModel> GetExamShiftByIdAsync(Guid id);

        public Task<Boolean> AddExamShiftAsync(AddExamShiftModel model);

        public Task<Boolean> UpdateExamShiftAsync(Guid id, UpdateExamShiftModel model);

        public Task<Boolean> DeleteExamShiftAsync(Guid id);

        public Task<Guid> GetExamShiftIdByName(string examShiftName);
    }
}
