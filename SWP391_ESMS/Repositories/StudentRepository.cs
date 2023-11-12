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

                // Materialize the majors and students in each major
                var majors = await _dbContext.Majors.ToListAsync();

                var majorDistributionTasks = majors
                    .ToDictionary(
                        major => major.MajorName!,
                        major => {
                            var studentsInMajor = _dbContext.Students
                                .Where(student => student.MajorId == major.MajorId)
                                .ToList();

                            return totalStudents > 0
                                ? Math.Round((double)studentsInMajor.Count / totalStudents * 100, 2)
                                : 0.0;
                        });

                return majorDistributionTasks;
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
