using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

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
            var student = await _dbContext.Students.FirstOrDefaultAsync(student =>
                (student.Username == model.UsernameOrEmail || student.Email == model.UsernameOrEmail));

            if (student != null)
            {
                if (!BC.EnhancedVerify(model.Password, student.PasswordHash)) { return null; }
                return _mapper.Map<UserInfo>(student);
            }

            var teacher = await _dbContext.Teachers.FirstOrDefaultAsync(teacher =>
                (teacher.Username == model.UsernameOrEmail || teacher.Email == model.UsernameOrEmail));

            if (teacher != null)
            {
                if (!BC.EnhancedVerify(model.Password, teacher.PasswordHash)) { return null; }
                return _mapper.Map<UserInfo>(teacher);
            }

            var staff = await _dbContext.Staff.FirstOrDefaultAsync(staff =>
                (staff.Username == model.UsernameOrEmail || staff.Email == model.UsernameOrEmail));

            if (staff != null)
            {
                if (!BC.EnhancedVerify(model.Password, staff.PasswordHash)) { return null; }
                return _mapper.Map<UserInfo>(staff);
            }

            return null;
        }

        public async Task<Boolean> Signup(SignupModel model)
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
    }
}
