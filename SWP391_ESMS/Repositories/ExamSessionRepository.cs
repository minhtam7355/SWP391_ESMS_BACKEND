using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Helpers;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class ExamSessionRepository : IExamSessionRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExamSessionRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Boolean> AddExamSessionAsync(ExamSessionModel model)
        {
            try
            {
                var newExamSession = _mapper.Map<ExamSession>(model);
                await _dbContext.ExamSessions.AddAsync(newExamSession);
                await _dbContext.SaveChangesAsync();

                return true;
            }catch (Exception ex)
            {
                return false;
            }
            
        }

        public async Task<Boolean> DeleteExamSessionAsync(Guid id)
        {
            var deleteExamSession = await _dbContext.ExamSessions.FindAsync(id);
            if (deleteExamSession != null)
            {
                _dbContext.ExamSessions.Remove(deleteExamSession);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ExamSessionModel>> GetAllExamSessionsAsync()
        {
            var examSessions = await _dbContext.ExamSessions.ToListAsync();
            return _mapper.Map<List<ExamSessionModel>>(examSessions);

        }

        public async Task<ExamSessionModel> GetExamSessionByIdAsync(Guid id)
        {
            var examSession = await _dbContext.ExamSessions.FindAsync(id);
            return _mapper.Map<ExamSessionModel>(examSession);

        }

        public async Task<Boolean> UpdateExamSessionAsync(Guid id, ExamSessionModel model)
        {
            var existingExamSession = await _dbContext.ExamSessions.FindAsync(id);

            if (existingExamSession != null)
            {
                _mapper.Map(model, existingExamSession);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
