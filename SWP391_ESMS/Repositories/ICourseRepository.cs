﻿using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface ICourseRepository
    {
        public Task<List<CourseModel>> GetAllCoursesAsync();

        public Task<List<CourseModel>> GetCoursesByPeriodAsync(Guid periodId);

        public Task<CourseModel> GetCourseByIdAsync(Guid id);

        public Task<Boolean> AddCourseAsync(CourseModel model);

        public Task<Boolean> UpdateCourseAsync(CourseModel model);

        public Task<Boolean> DeleteCourseAsync(Guid id);

        public Task<Guid?> GetCourseIdByNameAsync(string? courseName);

        public Task<Boolean> RemoveStudentFromCourseAsync(Guid courseId, Guid studentId);

        public Task<Boolean> AddStudentToCourseAsync(Guid courseId, Guid studentId);
    }
}
