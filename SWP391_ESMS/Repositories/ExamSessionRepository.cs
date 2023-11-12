using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Helpers;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;
using System.Linq;

namespace SWP391_ESMS.Repositories
{
    public class ExamSessionRepository : IExamSessionRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExamSessionRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Boolean> AddExamSessionAsync(ExamSessionModel model)
        {
            try
            {
                // Fetch students who are taking the specified course and meet the following criteria:
                // They have no active exam sessions for the same course and format.
                var courseEnrollments = await _dbContext.Courses
                    .Where(ce => ce.CourseId == model.CourseId)
                    .SelectMany(ce => ce.Students
                        .Where(student => !student.ExamSessions
                            .Any(es =>
                                es.CourseId == model.CourseId &&
                                es.ExamFormat == model.ExamFormat &&
                                es.IsPassed == false)))
                    .ToListAsync();


                // Check if there are eligible students in the course
                if (courseEnrollments == null || !courseEnrollments.Any())
                {
                    return false; // No eligible students for the course and format.
                }

                // Determine the number of exam sessions needed
                int numStudents = courseEnrollments.Count;
                int maxStudentsPerSession = (int) await _dbContext.ConfigurationSettings
                                                                .Where(cs => cs.SettingName == "Max Students Per Session")
                                                                .Select(cs => cs.SettingValue).SingleOrDefaultAsync();
                int numSessions = (int)Math.Ceiling((double)numStudents / maxStudentsPerSession);

                // Get all exam rooms and exam shifts
                var allExamRooms = await _dbContext.ExamRooms.ToListAsync();
                var allExamShifts = await _dbContext.ExamShifts.ToListAsync();

                // Generate all possible combinations of exam rooms and shifts
                var allCombinations = allExamRooms.SelectMany(room => allExamShifts, (room, shift) => new { room, shift });

                // Get exam sessions for the same exam date
                var existingSessionsSameDate = await _dbContext.ExamSessions
                    .Where(es => es.ExamDate == model.ExamDate)
                    .Select(es => new { es.RoomId, es.ShiftId })
                    .ToListAsync();

                // Remove combinations that are already in use on the same date and format
                var availableCombinations = allCombinations
                    .Where(combination =>
                        !existingSessionsSameDate.Any(existingSession =>
                            existingSession.RoomId == combination.room.RoomId &&
                            existingSession.ShiftId == combination.shift.ShiftId))
                    .ToList();

                // Check if the number of available combinations is lower than the number of numSessions
                if (availableCombinations.Count < numSessions)
                {
                    return false; // Not enough available combinations.
                }

                // Generate and create unique exam sessions
                var examSessions = new List<ExamSession>();
                for (int i = 0; i < numSessions; i++)
                {
                    // Get the next available combination from availableCombinations
                    var availableCombination = availableCombinations[i % availableCombinations.Count];

                    var examSession = new ExamSession
                    {
                        ExamSessionId = Guid.NewGuid(),
                        CourseId = model.CourseId,
                        ExamFormat = model.ExamFormat,
                        ExamDate = model.ExamDate,
                        ShiftId = availableCombination.shift.ShiftId,
                        RoomId = availableCombination.room.RoomId,
                        StudentsEnrolled = 0, // Initialize students enrolled to 0
                        TeacherId = null,
                        StaffId = model.StaffId,
                        IsPassed = false,
                        IsPaid = false
                    };

                    // Fetch students for this session and register them
                    int studentsToTake = Math.Min(maxStudentsPerSession, courseEnrollments.Count); // Take up to maxStudentsPerSession or the remaining students.
                    var studentsForSession = courseEnrollments.Take(studentsToTake).ToList();
                    courseEnrollments = courseEnrollments.Skip(studentsToTake).ToList();

                    foreach (var student in studentsForSession)
                    {
                        examSession.Students.Add(student);
                    }

                    examSessions.Add(examSession);
                }

                // Add exam sessions to the database
                await _dbContext.ExamSessions.AddRangeAsync(examSessions);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddStudentToExamSessionAsync(Guid examSessionId, Guid studentId)
        {
            try
            {
                var examSession = await _dbContext.ExamSessions.FindAsync(examSessionId);
                var student = await _dbContext.Students.FindAsync(studentId);

                if (examSession == null || student == null)
                {
                    return false; // Exam session or student not found.
                }

                // Check if the ExamDate is today.
                if (examSession.ExamDate == DateTime.Today)
                {
                    return false; // Do not allow to add students on the day of the exam.
                }

                // Check if the student is not already enrolled in the exam session.
                if (!examSession.Students.Contains(student))
                {
                    examSession.Students.Add(student);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }

                return false; // Student is already enrolled in the exam session.
            }
            catch (Exception)
            {
                return false; // Error occurred during the addition process.
            }
        }

        public async Task<bool> AddTeacherToExamSessionAsync(Guid examSessionId, Guid teacherId)
        {
            try
            {
                var examSession = await _dbContext.ExamSessions.FindAsync(examSessionId);
                var teacher = await _dbContext.Teachers.FindAsync(teacherId);

                if (examSession == null || teacher == null)
                {
                    return false; // Exam session or teacher not found.
                }

                // Check if the exam session already has a teacher assigned.
                if (examSession.TeacherId != null)
                {
                    return false; // Teacher already assigned to the exam session.
                }

                examSession.TeacherId = teacher.TeacherId;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false; // Error occurred during the assignment process.
            }
        }

        public async Task<List<decimal>> CalculateMonthlyEarningsAsync()
        {
            try
            {
                // Get the Hourly Supervision Fee from configuration settings
                var hourlySupervisionFeeSetting = await _dbContext.ConfigurationSettings
                    .FirstOrDefaultAsync(c => c.SettingName == "Hourly Supervision Fee");

                if (hourlySupervisionFeeSetting == null || hourlySupervisionFeeSetting.SettingValue == null)
                {
                    return new List<decimal>(); // Configuration setting not found or invalid.
                }

                decimal hourlySupervisionFee = hourlySupervisionFeeSetting.SettingValue ?? 0;

                // Get the current year
                int currentYear = DateTime.Now.Year;

                // Calculate the monthly earnings based on all paid exam sessions (past and future) in the current year
                var monthlyEarnings = Enumerable.Range(1, 12)
                    .Select((int month) =>
                    {
                        // Calculate the start and end dates for each month
                        DateTime startDate = new DateTime(currentYear, month, 1);
                        DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                        // Calculate earnings for the month
                        decimal monthlyEarningsForMonth = _dbContext.ExamSessions
                            .Where(es => es.IsPaid == true && es.ExamDate >= startDate && es.ExamDate <= endDate)
                            .Sum((Func<ExamSession, decimal>)(es =>
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
                            }));

                        return monthlyEarningsForMonth;
                    })
                    .ToList();

                return monthlyEarnings;
            }
            catch (Exception)
            {
                return new List<decimal>(); // Error occurred during the calculation.
            }
        }

        public async Task<Boolean> DeleteExamSessionAsync(Guid id)
        {
            var deleteExamSession = await _dbContext.ExamSessions.FindAsync(id);
            if (deleteExamSession != null)
            {
                _dbContext.ExamSessions.Remove(deleteExamSession);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ExamSessionModel>> GetAllExamSessionsAsync()
        {
            var examSessions = await _dbContext.ExamSessions.ToListAsync();

            foreach (var examSession in examSessions)
            {
                if (examSession.ExamDate < DateTime.Now.Date)
                {
                    examSession.IsPassed = true; // Update isPassed to true if the exam date is in the past.
                }
                else
                {
                    examSession.IsPassed = false; // Update isPassed to false if the exam date is in the future.
                }
            }

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<List<ExamSessionModel>>(examSessions);

        }

        public async Task<ExamSessionModel> GetExamSessionByIdAsync(Guid id)
        {
            var examSession = await _dbContext.ExamSessions.FindAsync(id);
            return _mapper.Map<ExamSessionModel>(examSession);

        }

        public async Task<List<ExamSessionModel>?> GetExamSessionsByStudentAsync(Guid studentId)
        {
            try
            {
                var examSessions = await _dbContext.ExamSessions
                    .Where(es => es.Students.Any(s => s.StudentId == studentId))
                    .ToListAsync();

                return _mapper.Map<List<ExamSessionModel>>(examSessions);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ExamSessionModel>?> GetExamSessionsByTeacherAsync(Guid teacherId)
        {
            try
            {
                var examSessions = await _dbContext.ExamSessions
                    .Where(es => es.TeacherId == teacherId)
                    .ToListAsync();

                return _mapper.Map<List<ExamSessionModel>>(examSessions);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<ExamSessionModel>?> GetExamSessionsWithoutTeacherAsync()
        {
            try
            {
                var examSessions = await _dbContext.ExamSessions
                    .Where(es => es.TeacherId == null)
                    .ToListAsync();

                return _mapper.Map<List<ExamSessionModel>>(examSessions);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<int>> GetNumberOfExamsHeldMonthlyAsync()
        {
            try
            {
                int currentYear = DateTime.Now.Year;

                // Materialize the months to avoid capturing the DbContext in the lambda expression
                var months = Enumerable.Range(1, 12).ToList();

                var examsHeldMonthly = new List<int>();

                foreach (var month in months)
                {
                    DateTime startDate = new DateTime(currentYear, month, 1);
                    DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                    int examsHeldForMonth = await _dbContext.ExamSessions
                        .CountAsync(es => es.ExamDate >= startDate && es.ExamDate <= endDate);

                    examsHeldMonthly.Add(examsHeldForMonth);
                }

                return examsHeldMonthly;
            }
            catch (Exception)
            {
                return new List<int>(); // Handle the error appropriately.
            }
        }

        public async Task<List<int>> GetNumberOfStudentsExaminedMonthlyAsync()
        {
            try
            {
                int currentYear = DateTime.Now.Year;

                // Materialize the months to avoid capturing the DbContext in the lambda expression
                var months = Enumerable.Range(1, 12).ToList();

                var studentsExaminedMonthly = new List<int>();

                foreach (var month in months)
                {
                    DateTime startDate = new DateTime(currentYear, month, 1);
                    DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                    int studentsExaminedForMonth = await _dbContext.ExamSessions
                        .Where(es => es.IsPassed == true && es.ExamDate >= startDate && es.ExamDate <= endDate)
                        .SelectMany(es => es.Students)
                        .Distinct()
                        .CountAsync();

                    studentsExaminedMonthly.Add(studentsExaminedForMonth);
                }

                return studentsExaminedMonthly;
            }
            catch (Exception)
            {
                return new List<int>(); // Handle the error appropriately.
            }
        }

        public async Task<bool> RemoveStudentFromExamSessionAsync(Guid examSessionId, Guid studentId)
        {
            try
            {
                var examSession = await _dbContext.ExamSessions.FindAsync(examSessionId);
                var student = await _dbContext.Students.FindAsync(studentId);

                if (examSession == null || student == null)
                {
                    return false; // Exam session or student not found.
                }

                // Check if the ExamDate is today.
                if (examSession.ExamDate == DateTime.Today)
                {
                    return false; // Do not allow to remove students on the day of the exam.
                }

                // Check if the student is enrolled in the exam session.
                if (examSession.Students.Contains(student))
                {
                    examSession.Students.Remove(student);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }

                return false; // Student not enrolled in the exam session.
            }
            catch (Exception)
            {
                return false; // Error occurred during the removal process.
            }
        }

        public async Task<bool> RemoveTeacherFromExamSessionAsync(Guid examSessionId)
        {
            try
            {
                var examSession = await _dbContext.ExamSessions.FindAsync(examSessionId);

                if (examSession == null)
                {
                    return false; // Exam session not found.
                }

                // Check if the exam session has a teacher assigned.
                if (examSession.TeacherId == null)
                {
                    return false; // No teacher assigned to the exam session.
                }

                examSession.TeacherId = null; // Remove the teacher assignment.
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false; // Error occurred during the removal process.
            }
        }

        public async Task<Boolean> UpdateExamSessionAsync(ExamSessionModel model)
        {
            var existingExamSession = await _dbContext.ExamSessions.FindAsync(model.ExamSessionId);

            if (existingExamSession != null)
            {
                _mapper.Map(model, existingExamSession);

                if (existingExamSession.ExamDate < DateTime.Now.Date)
                {
                    existingExamSession.IsPassed = true; // Update isPassed to true if the exam date is in the past.
                }
                else
                {
                    existingExamSession.IsPassed = false; // Update isPassed to false if the exam date is in the future.
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
