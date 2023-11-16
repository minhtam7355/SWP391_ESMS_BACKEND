using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public CourseRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Boolean> AddCourseAsync(CourseModel model)
        {
            try
            {
                var existingCourseName = await _dbContext.Courses.Select(c => c.CourseName).FirstOrDefaultAsync(c => c == model.CourseName);
                if (existingCourseName != null)
                {
                    return false;
                }
                var newCourse = _mapper.Map<Course>(model);
                newCourse.CourseId = Guid.NewGuid();
                await _dbContext.Courses.AddAsync(newCourse);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Boolean> AddStudentToCourseAsync(Guid courseId, Guid studentId)
        {
            try
            {
                var course = await _dbContext.Courses.FindAsync(courseId);
                var student = await _dbContext.Students.FindAsync(studentId);

                if (course == null || student == null)
                {
                    return false; // Course or student not found.
                }

                // Check if the student is not already enrolled in the course.
                if (!course.Students.Contains(student))
                {
                    course.Students.Add(student);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }

                return false; // Student is already enrolled in the course.
            }
            catch (Exception)
            {
                return false; // Error occurred during the addition process.
            }
        }

        public async Task<Boolean> DeleteCourseAsync(Guid id)
        {
            var deleteCourse = await _dbContext.Courses.FindAsync(id);
            if (deleteCourse != null)
            {
                _dbContext.Courses.Remove(deleteCourse);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<CourseModel>> GetAllCoursesAsync()
        {
            var courses = await _dbContext.Courses.ToListAsync();
            return _mapper.Map<List<CourseModel>>(courses);
        }

        public async Task<CourseModel> GetCourseByIdAsync(Guid id)
        {
            var course = await _dbContext.Courses.FindAsync(id);
            return _mapper.Map<CourseModel>(course);
        }

        public async Task<Guid?> GetCourseIdByNameAsync(string? courseName)
        {
            var course = await _dbContext.Courses.FirstOrDefaultAsync(c => c.CourseName == courseName);
            if (course == null) { return null; }
            return course.CourseId;
        }

        public async Task<Boolean> RemoveStudentFromCourseAsync(Guid courseId, Guid studentId)
        {
            try
            {
                var course = await _dbContext.Courses.FindAsync(courseId);
                var student = await _dbContext.Students.FindAsync(studentId);

                if (course == null || student == null)
                {
                    return false; // Course or student not found.
                }

                // Check if the student is enrolled in the course.
                if (course.Students.Contains(student))
                {
                    course.Students.Remove(student);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }

                return false; // Student not enrolled in the course.
            }
            catch (Exception)
            {
                return false; // Error occurred during the removal process.
            }
        }

        public async Task<Boolean> UpdateCourseAsync(CourseModel model)
        {
            var existingCourse = await _dbContext.Courses.FindAsync(model.CourseId);

            if (existingCourse != null)
            {
                if (model.CourseName != existingCourse.CourseName)
                {
                    var existingCourseName = await _dbContext.Courses.Select(c => c.CourseName).FirstOrDefaultAsync(c => c == model.CourseName);
                    if (existingCourseName != null)
                    {
                        return false;
                    }
                }
                _mapper.Map(model, existingCourse);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
