using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class MajorRepository : IMajorRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public MajorRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Boolean> AddMajorAsync(MajorModel model)
        {
            try
            {
                var newMajor = _mapper.Map<Major>(model);
                newMajor.MajorId = Guid.NewGuid();
                await _dbContext.Majors.AddAsync(newMajor);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<Boolean> DeleteMajorAsync(MajorModel model)
        {
            var deleteMajor = await _dbContext.Majors.FindAsync(model.MajorId);
            if (deleteMajor != null)
            {
                _dbContext.Majors.Remove(deleteMajor);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<MajorModel>> GetAllMajorsAsync()
        {
            var majors = await _dbContext.Majors.ToListAsync();
            return _mapper.Map<List<MajorModel>>(majors);
        }

        public async Task<MajorModel> GetMajorByIdAsync(Guid id)
        {
            var major = await _dbContext.Majors.FindAsync(id);
            return _mapper.Map<MajorModel>(major);
        }

        public async Task<Guid> GetMajorIdByName(string majorName)
        {
            var major = await _dbContext.Majors.FirstOrDefaultAsync(m => m.MajorName == majorName);
            return major!.MajorId;
        }

        public async Task<Boolean> UpdateMajorAsync(MajorModel model)
        {
            var existingMajor = await _dbContext.Majors.FindAsync(model.MajorId);

            if (existingMajor != null)
            {
                _mapper.Map(model, existingMajor);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
