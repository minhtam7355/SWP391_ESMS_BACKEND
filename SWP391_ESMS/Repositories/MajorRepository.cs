﻿using AutoMapper;
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

        public async Task<Boolean> AddMajorAsync(AddMajorModel model)
        {
            try
            {
                var newMajor = _mapper.Map<Major>(model);
                await _dbContext.Majors.AddAsync(newMajor);
                await _dbContext.SaveChangesAsync();

                return true;
            }catch (Exception ex) 
            {
                return false;
            }
            
        }

        public async Task<Boolean> DeleteMajorAsync(Guid id)
        {
            var deleteMajor = await _dbContext.Majors.FindAsync(id);
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

        public async Task<Boolean> UpdateMajorAsync(Guid id, UpdateMajorModel model)
        {
            var existingMajor = await _dbContext.Majors.FindAsync(id);

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
