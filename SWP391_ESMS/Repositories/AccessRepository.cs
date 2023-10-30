using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace SWP391_ESMS.Repositories
{
    public class AccessRepository : IAccessRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public AccessRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<UserInfo?> Login(LoginModel model)
        {
            var passwordHash = HashPassword(model.Password!);

            var student = await _dbContext.Students.FirstOrDefaultAsync(student =>
                (student.Username == model.UsernameOrEmail || student.Email == model.UsernameOrEmail) &&
                student.PasswordHash == passwordHash);

            if (student != null) 
            {
                return _mapper.Map<UserInfo>(student);
            }

            var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(teacher =>
                (teacher.Username == model.UsernameOrEmail || teacher.Email == model.UsernameOrEmail) &&
                teacher.PasswordHash == passwordHash);

            if (teacher != null)
            {
                return _mapper.Map<UserInfo>(teacher);
            }

            var staff = await _dbContext.Staff.FirstOrDefaultAsync(staff =>
                (staff.Username == model.UsernameOrEmail || staff.Email == model.UsernameOrEmail) &&
                staff.PasswordHash == passwordHash);

            if (staff != null)
            {
                return _mapper.Map<UserInfo>(staff);
            }

            return null;

        }

        public async Task<Boolean> Signup(SignupModel model)
        {
            try
            {
                var passwordHash = HashPassword(model.Password!);

                var newStudent = _mapper.Map<Student>(model);

                newStudent.StudentId = Guid.NewGuid();
                newStudent.PasswordHash = passwordHash;
                // Add A Default Profile Pic

                await _dbContext.Students.AddAsync(newStudent);
                await _dbContext.SaveChangesAsync();

                return true;

            }catch(Exception ex)
            {
                return false;
            }

        }

        public string HashPassword(string password)
        {
            SHA256 hash = SHA256.Create();
            var passwordBytes = Encoding.Default.GetBytes(password);
            var hashedPassword = hash.ComputeHash(passwordBytes);
            return Convert.ToBase64String(hashedPassword);
        }
    }
}
