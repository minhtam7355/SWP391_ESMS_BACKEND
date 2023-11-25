using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
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

        public async Task<bool> AddExamFormatAsync(ExamFormatModel model)
        {
            try
            {
                var newFormat = _mapper.Map<ExamFormat>(model);
                newFormat.ExamFormatId = Guid.NewGuid();
                await _dbContext.ExamFormats.AddAsync(newFormat);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteExamFormatAsync(Guid id)
        {
            var deleteFormat = await _dbContext.ExamFormats.FindAsync(id);
            if (deleteFormat != null)
            {
                _dbContext.ExamFormats.Remove(deleteFormat);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ExamFormatModel>> GetAllExamFormatsAsync()
        {
            var examFormats = await _dbContext.ExamFormats.ToListAsync();
            return _mapper.Map<List<ExamFormatModel>>(examFormats);
        }

        public async Task<ExamFormatModel> GetExamFormatByIdAsync(Guid id)
        {
            var examFormat = await _dbContext.ExamFormats.FindAsync(id);
            return _mapper.Map<ExamFormatModel>(examFormat);
        }

        public async Task<bool> IsExamFormatUniqueAsync(string examFormatCode, string examFormatName)
        {
            var existingFormat = await _dbContext.ExamFormats
                .FirstOrDefaultAsync(ef => ef.ExamFormatCode == examFormatCode || ef.ExamFormatName == examFormatName);

            return existingFormat == null;
        }

        public async Task<bool> UpdateExamFormatAsync(ExamFormatModel model)
        {
            var existingFormat = await _dbContext.ExamFormats.FindAsync(model.ExamFormatId);

            if (existingFormat != null)
            {
                _mapper.Map(model, existingFormat);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
