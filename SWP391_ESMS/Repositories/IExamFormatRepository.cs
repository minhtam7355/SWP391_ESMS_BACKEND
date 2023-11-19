using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IExamFormatRepository
    {
        public Task<List<ExamFormatModel>> GetAllExamFormatsAsync();

        public Task<ExamFormatModel> GetExamFormatByIdAsync(Guid id);

        public Task<Boolean> AddExamFormatAsync(ExamFormatModel model);

        public Task<Boolean> UpdateExamFormatAsync(ExamFormatModel model);

        public Task<Boolean> DeleteExamFormatAsync(Guid id);
    }
}
