using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public StudentRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Boolean> AddStudentAsync(StudentModel model)
        {
            try
            {
                var newStudent = _mapper.Map<Student>(model);
                newStudent.StudentId = Guid.NewGuid();
                newStudent.PasswordHash = BC.EnhancedHashPassword(model.Password, 13);
                await _dbContext.Students.AddAsync(newStudent);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Boolean> DeleteStudentAsync(StudentModel model)
        {
            var deleteStudent = await _dbContext.Students.FindAsync(model.StudentId);
            if (deleteStudent != null)
            {
                _dbContext.Students.Remove(deleteStudent);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<StudentModel>> GetAllStudentsAsync()
        {
            var students = await _dbContext.Students.ToListAsync();
            return _mapper.Map<List<StudentModel>>(students);
        }

        public async Task<StudentModel> GetStudentByIdAsync(Guid id)
        {
            var student = await _dbContext.Students.FindAsync(id);
            return _mapper.Map<StudentModel>(student);
        }

        public async Task<Boolean> UpdateStudentAsync(StudentModel model)
        {
            var existingStudent = await _dbContext.Students.FindAsync(model.StudentId);

            if (existingStudent != null)
            {
                _mapper.Map(model, existingStudent);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
