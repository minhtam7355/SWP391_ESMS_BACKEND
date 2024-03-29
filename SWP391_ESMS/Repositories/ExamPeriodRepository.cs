﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class ExamPeriodRepository : IExamPeriodRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExamPeriodRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> AddExamPeriodAsync(ExamPeriodModel model)
        {
            try
            {
                var newExamPeriod = _mapper.Map<ExamPeriod>(model);
                newExamPeriod.ExamPeriodId = Guid.NewGuid();
                await _dbContext.ExamPeriods.AddAsync(newExamPeriod);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteExamPeriodAsync(Guid id)
        {
            var deleteExamPeriod = await _dbContext.ExamPeriods.FindAsync(id);
            if (deleteExamPeriod != null)
            {
                _dbContext.ExamPeriods.Remove(deleteExamPeriod);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ExamPeriodModel>> GetAllExamPeriodsAsync()
        {
            var examPeriods = await _dbContext.ExamPeriods.ToListAsync();
            return _mapper.Map<List<ExamPeriodModel>>(examPeriods);
        }

        public async Task<ExamPeriodModel> GetExamPeriodByIdAsync(Guid id)
        {
            var examPeriod = await _dbContext.ExamPeriods.FindAsync(id);
            return _mapper.Map<ExamPeriodModel>(examPeriod);
        }

        public async Task<bool> IsExamPeriodNameUniqueAsync(string examPeriodName)
        {
            // Check if there is any existing exam period with the same name
            var existingExamPeriod = await _dbContext.ExamPeriods
                .FirstOrDefaultAsync(ep => ep.ExamPeriodName == examPeriodName);

            return existingExamPeriod == null;
        }

        public async Task<bool> UpdateExamPeriodAsync(ExamPeriodModel model)
        {
            var existingExamPeriod = await _dbContext.ExamPeriods.FindAsync(model.ExamPeriodId);

            if (existingExamPeriod != null)
            {               
                _mapper.Map(model, existingExamPeriod);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
