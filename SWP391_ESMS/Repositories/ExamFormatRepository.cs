using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class ExamFormatRepository : IExamFormatRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExamFormatRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Task<bool> AddExamFormatAsync(ExamFormatModel model)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteExamFormatAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ExamFormatModel>> GetAllExamFormatsAsync()
        {
            var examFormats = await _dbContext.ExamFormats.ToListAsync();
            return _mapper.Map<List<ExamFormatModel>>(examFormats);
        }

        public Task<ExamFormatModel> GetExamFormatByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateExamFormatAsync(ExamFormatModel model)
        {
            throw new NotImplementedException();
        }
    }
}
