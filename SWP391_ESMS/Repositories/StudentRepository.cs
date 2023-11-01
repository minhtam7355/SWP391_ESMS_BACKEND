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

        public async Task<Boolean> AddStudentAsync(AddStudentModel model)
        {
            try
            {
                var newStudent = _mapper.Map<Student>(model);
                await _dbContext.Students.AddAsync(newStudent);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Boolean> DeleteStudentAsync(Guid id)
        {
            var deleteStudent = await _dbContext.Students.FindAsync(id);
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

        public async Task<Boolean> UpdateStudentAsync(Guid id, UpdateStudentModel model)
        {
            var existingStudent = await _dbContext.Students.FindAsync(id);

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
