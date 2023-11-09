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

        public async Task<Boolean> AddTeacherAsync(TeacherModel model)
        {
            try
            {
                var newTeacher = _mapper.Map<Teacher>(model);
                newTeacher.TeacherId = Guid.NewGuid();
                newTeacher.PasswordHash = BC.EnhancedHashPassword(model.Password, 13);
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

        public async Task<TeacherModel?> GetTeacherByExamSessionAsync(Guid examSessionId)
        {
            var examSession = await _dbContext.ExamSessions.FindAsync(examSessionId);
            if (examSession == null || examSession.Teacher == null) return null;
            return _mapper.Map<TeacherModel>(examSession.Teacher);
        }

        public async Task<TeacherModel> GetTeacherByIdAsync(Guid id)
        {
            var teacher = await _dbContext.Teachers.FindAsync(id);
            return _mapper.Map<TeacherModel>(teacher);
        }

        public async Task<Boolean> UpdateTeacherAsync(TeacherModel model)
        {
            var existingTeacher = await _dbContext.Teachers.FindAsync(model.TeacherId);

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
