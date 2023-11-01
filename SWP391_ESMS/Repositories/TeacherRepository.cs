using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public TeacherRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Boolean> AddTeacherAsync(AddTeacherModel model)
        {
            try
            {
                var newTeacher = _mapper.Map<Teacher>(model);
                await _dbContext.Teachers.AddAsync(newTeacher);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Boolean> DeleteTeacherAsync(Guid id)
        {
            var deleteTeacher = await _dbContext.Teachers.FindAsync(id);
            if (deleteTeacher != null)
            {
                _dbContext.Teachers.Remove(deleteTeacher);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<TeacherModel>> GetAllTeachersAsync()
        {
            var teachers = await _dbContext.Teachers.ToListAsync();
            return _mapper.Map<List<TeacherModel>>(teachers);
        }

        public async Task<TeacherModel> GetTeacherByIdAsync(Guid id)
        {
            var teacher = await _dbContext.Teachers.FindAsync(id);
            return _mapper.Map<TeacherModel>(teacher);
        }

        public async Task<Boolean> UpdateTeacherAsync(Guid id, UpdateTeacherModel model)
        {
            var existingTeacher = await _dbContext.Teachers.FindAsync(id);

            if (existingTeacher != null)
            {
                _mapper.Map(model, existingTeacher);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
