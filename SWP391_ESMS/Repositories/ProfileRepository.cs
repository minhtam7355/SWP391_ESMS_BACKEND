using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProfileRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Boolean> ChangePasswordAsync(ChangePasswordModel model, Guid id, string role)
        {
            if (role == "Student")
            {
                var student = await _dbContext.Students.FirstOrDefaultAsync(student => student.StudentId == id);
                student!.PasswordHash = BC.EnhancedHashPassword(model.NewPassword, 13);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            if (role == "Teacher")
            {
                var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(teacher => teacher.TeacherId == id);
                teacher!.PasswordHash = BC.EnhancedHashPassword(model.NewPassword, 13);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            if (role == "Admin" || role == "Testing Admin" || role == "Testing Staff")
            {
                var staff = await _dbContext.Staff.FirstOrDefaultAsync(staff => staff.StaffId == id);
                staff!.PasswordHash = BC.EnhancedHashPassword(model.NewPassword, 13);
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<UserInfo?> GetUserProfileAsync(Guid id, string role)
        {
            if (role == "Student")
            {
                var student = await _dbContext.Students.FirstOrDefaultAsync(student => student.StudentId == id);
                return _mapper.Map<UserInfo>(student);
            }
            if (role == "Teacher")
            {
                var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(teacher => teacher.TeacherId == id);
                return _mapper.Map<UserInfo>(teacher);
            }
            if (role == "Admin" || role == "Testing Admin" || role == "Testing Staff")
            {
                var staff = await _dbContext.Staff.FirstOrDefaultAsync(staff => staff.StaffId == id);
                return _mapper.Map<UserInfo>(staff);
            }

            return null;
        }

        public async Task<Boolean> SaveProfilePictureAsync(Guid id, string role, string base64Image)
        {
            if (role == "Student")
            {
                var student = await _dbContext.Students.FirstOrDefaultAsync(student => student.StudentId == id);
                student!.ProfilePicture = base64Image;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            if (role == "Teacher")
            {
                var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(teacher => teacher.TeacherId == id);
                teacher!.ProfilePicture = base64Image;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            if (role == "Admin" || role == "Testing Admin" || role == "Testing Staff")
            {
                var staff = await _dbContext.Staff.FirstOrDefaultAsync(staff => staff.StaffId == id);
                staff!.ProfilePicture = base64Image;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<Boolean> UpdateUserProfileAsync(UserInfo model)
        {
            if (model.Role == "Student")
            {
                var existingStudent = await _dbContext.Students.FindAsync(model.UserId);
                _mapper.Map(model, existingStudent);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else if (model.Role == "Teacher")
            {
                var existingTeacher = await _dbContext.Teachers.FindAsync(model.UserId);
                _mapper.Map(model, existingTeacher);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else if (model.Role == "Admin" || model.Role == "Testing Admin" || model.Role == "Testing Staff")
            {
                var existingStaff = await _dbContext.Staff.FindAsync(model.UserId);
                _mapper.Map(model, existingStaff);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else { return false; }
        }
    }
}
