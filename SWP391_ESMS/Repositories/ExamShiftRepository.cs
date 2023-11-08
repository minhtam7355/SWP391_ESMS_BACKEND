using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class ExamShiftRepository : IExamShiftRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExamShiftRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Boolean> AddExamShiftAsync(ExamShiftModel model)
        {
            try
            {
                var newExamShift = _mapper.Map<ExamShift>(model);
                newExamShift.ShiftId = Guid.NewGuid();
                await _dbContext.ExamShifts.AddAsync(newExamShift);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Boolean> DeleteExamShiftAsync(Guid id)
        {
            var deleteExamShift = await _dbContext.ExamShifts.FindAsync(id);
            if (deleteExamShift != null)
            {
                _dbContext.ExamShifts.Remove(deleteExamShift);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ExamShiftModel>> GetAllExamShiftsAsync()
        {
            var examShifts = await _dbContext.ExamShifts.ToListAsync();
            return _mapper.Map<List<ExamShiftModel>>(examShifts);
        }

        public async Task<ExamShiftModel> GetExamShiftByIdAsync(Guid id)
        {
            var examShift = await _dbContext.ExamShifts.FindAsync(id);
            return _mapper.Map<ExamShiftModel>(examShift);
        }

        public async Task<Guid> GetExamShiftIdByName(string examShiftName)
        {
            var examShift = await _dbContext.ExamShifts.FirstOrDefaultAsync(s => s.ShiftName == examShiftName);
            return examShift!.ShiftId;
        }

        public async Task<Boolean> UpdateExamShiftAsync(ExamShiftModel model)
        {
            var existingExamShift = await _dbContext.ExamShifts.FindAsync(model.ShiftId);

            if (existingExamShift != null)
            {
                _mapper.Map(model, existingExamShift);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
