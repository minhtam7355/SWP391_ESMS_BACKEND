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

        public async Task<decimal> CalculateCurrentWagesAsync(Guid id)
        {
            try
            {
                var teacher = await _dbContext.Teachers.Include(t => t.ExamSessions)
                    .FirstOrDefaultAsync(t => t.TeacherId == id);

                if (teacher == null || teacher.ExamSessions == null || !teacher.ExamSessions.Any())
                {
                    return 0; // No exam sessions or teacher not found.
                }

                // Get the Hourly Supervision Fee from configuration settings
                var hourlySupervisionFeeSetting = await _dbContext.ConfigurationSettings
                    .FirstOrDefaultAsync(c => c.SettingName == "Hourly Supervision Fee");

                if (hourlySupervisionFeeSetting == null || hourlySupervisionFeeSetting.SettingValue == null)
                {
                    return 0; // Configuration setting not found or invalid.
                }

                decimal hourlySupervisionFee = hourlySupervisionFeeSetting.SettingValue ?? 0;

                // Get the start date and end date of the current examination supervision allowance period
                DateTime allowanceStartDate;
                DateTime allowanceEndDate;

                if (DateTime.Now.Day <= 15)
                {
                    allowanceStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    allowanceEndDate = allowanceStartDate.AddDays(15);
                }
                else
                {
                    allowanceStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 16);
                    allowanceEndDate = allowanceStartDate.AddMonths(1).AddDays(-1);
                }

                // Calculate the current wages based on the teacher's assigned exam sessions within the allowance period
                decimal currentWages = teacher.ExamSessions
                    .Where(es => es.ExamDate >= allowanceStartDate && es.ExamDate <= allowanceEndDate)
                    .Sum(es =>
                    {
                        // Calculate hours based on ExamShift StartTime and EndTime
                        TimeSpan? startTime = es.Shift?.StartTime;
                        TimeSpan? endTime = es.Shift?.EndTime;

                        if (startTime != null && endTime != null)
                        {
                            double hours = (endTime.Value - startTime.Value).TotalHours;
                            return (decimal)hours * hourlySupervisionFee;
                        }

                        return 0;
                    });

                return currentWages;
            }
            catch (Exception)
            {
                return 0; // Error occurred during the calculation.
            }
        }

        public async Task<decimal> CalculateTotalEarningsAsync(Guid id)
        {
            try
            {
                var teacher = await _dbContext.Teachers.Include(t => t.ExamSessions)
                    .FirstOrDefaultAsync(t => t.TeacherId == id);

                if (teacher == null || teacher.ExamSessions == null || !teacher.ExamSessions.Any())
                {
                    return 0; // No exam sessions or teacher not found.
                }

                // Get the Hourly Supervision Fee from configuration settings
                var hourlySupervisionFeeSetting = await _dbContext.ConfigurationSettings
                    .FirstOrDefaultAsync(c => c.SettingName == "Hourly Supervision Fee");

                if (hourlySupervisionFeeSetting == null || hourlySupervisionFeeSetting.SettingValue == null)
                {
                    return 0; // Configuration setting not found or invalid.
                }

                decimal hourlySupervisionFee = hourlySupervisionFeeSetting.SettingValue ?? 0;

                // Calculate the total earnings based on all paid exam sessions (past and future)
                decimal totalEarnings = teacher.ExamSessions
                    .Where(es => es.IsPaid == true) // Consider only paid exam sessions
                    .Sum(es =>
                    {
                        // Calculate hours based on ExamShift StartTime and EndTime
                        TimeSpan? startTime = es.Shift?.StartTime;
                        TimeSpan? endTime = es.Shift?.EndTime;

                        // Check if both start and end times are not null
                        if (startTime != null && endTime != null)
                        {
                            // Calculate the duration in hours
                            double hours = (endTime.Value - startTime.Value).TotalHours;

                            // Multiply the hours by the hourly supervision fee to get earnings for this session
                            return (decimal)hours * hourlySupervisionFee;
                        }

                        // Return 0 if either start or end time is null
                        return 0;
                    });

                return totalEarnings;
            }
            catch (Exception)
            {
                return 0; // Error occurred during the calculation.
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
