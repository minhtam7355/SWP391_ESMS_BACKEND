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

        public async Task<Dictionary<string, double>> GetMajorDistributionAsync()
        {
            try
            {
                var totalStudents = await _dbContext.Students.CountAsync();

                var majorDistributionTasks = _dbContext.Majors
                    .ToDictionary(
                        major => major.MajorName!,
                        major => _dbContext.Students
                            .Where(student => student.MajorId == major.MajorId)
                            .CountAsync()
                            .ContinueWith(task => totalStudents > 0
                                ? Math.Round((double)task.Result / totalStudents * 100, 2)
                                : 0.0)
                    );

                // Await all tasks before creating the final dictionary
                var majorDistributionResults = await Task.WhenAll(majorDistributionTasks.Values);

                // Combine keys and results into the final dictionary
                var result = majorDistributionTasks.Keys
                    .Zip(majorDistributionResults, (key, value) => new { key, value })
                    .ToDictionary(pair => pair.key, pair => pair.value);

                return result;
            }
            catch (Exception)
            {
                return new Dictionary<string, double>(); // Handle the error appropriately.
            }
        }

        public async Task<StudentModel> GetStudentByIdAsync(Guid id)
        {
            var student = await _dbContext.Students.FindAsync(id);
            return _mapper.Map<StudentModel>(student);
        }

        public async Task<List<StudentModel>?> GetStudentsByExamSessionAsync(Guid examSessionId)
        {
            var examSession = await _dbContext.ExamSessions.FindAsync(examSessionId);
            if (examSession == null) return null;
            return _mapper.Map<List<StudentModel>>(examSession.Students);
        }

        public async Task<List<StudentModel>?> GetUnassignedStudentsAsync(Guid courseId)
        {
            try
            {
                // Query to retrieve students who are enrolled in the course but not assigned to any exam session.
                var unassignedStudents = await _dbContext.Students
                    .Where(s => s.Courses.Any(c => c.CourseId == courseId) && !s.ExamSessions.Any(es => es.CourseId == courseId))
                    .ToListAsync();

                return _mapper.Map<List<StudentModel>>(unassignedStudents);
            }
            catch (Exception)
            {
                return null;
            }
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
