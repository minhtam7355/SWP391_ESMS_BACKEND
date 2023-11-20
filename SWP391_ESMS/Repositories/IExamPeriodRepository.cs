using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IExamPeriodRepository
    {
        public Task<List<ExamPeriodModel>> GetAllExamPeriodsAsync();

        public Task<Boolean> AddExamPeriodAsync(ExamPeriodModel model);

        public Task<Boolean> UpdateExamPeriodAsync(ExamPeriodModel model);

        public Task<Boolean> DeleteExamPeriodAsync(Guid id);
    }
}
