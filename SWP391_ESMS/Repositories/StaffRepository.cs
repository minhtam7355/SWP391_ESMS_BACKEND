using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class StaffRepository : IStaffRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public StaffRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Boolean> AddStaffAsync(AddStaffModel model)
        {
            try
            {
                var newStaff = _mapper.Map<Staff>(model);
                await _dbContext.Staff.AddAsync(newStaff);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Boolean> DeleteStaffAsync(Guid id)
        {
            var deleteStaff = await _dbContext.Staff.FindAsync(id);
            if (deleteStaff != null)
            {
                _dbContext.Staff.Remove(deleteStaff);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<StaffModel>> GetAllStaffAsync()
        {
            var staffList = await _dbContext.Staff.ToListAsync();
            return _mapper.Map<List<StaffModel>>(staffList);
        }

        public async Task<StaffModel> GetStaffByIdAsync(Guid id)
        {
            var staff = await _dbContext.Staff.FindAsync(id);
            return _mapper.Map<StaffModel>(staff);
        }

        public async Task<Boolean> UpdateStaffAsync(Guid id, UpdateStaffModel model)
        {
            var existingStaff = await _dbContext.Staff.FindAsync(id);

            if (existingStaff != null)
            {
                _mapper.Map(model, existingStaff);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
