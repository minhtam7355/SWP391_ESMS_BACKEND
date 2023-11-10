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
            var students = existingExamSession!.Students;
            // Retrieve the scheduling period setting
            var schedulingPeriodSetting = await _dbContext.ConfigurationSettings.FirstOrDefaultAsync(c => c.SettingName == "Scheduling Period");
            int schedulingPeriod = Convert.ToInt32(schedulingPeriodSetting!.SettingValue);

            // Calculate the minimum allowed date
            DateTime currentDate = DateTime.Now.Date;
            DateTime minAllowedDate = currentDate.AddDays(schedulingPeriod);

            if (existingExamSession != null)
            {
                _mapper.Map(model, existingExamSession);
                existingExamSession.Course = await _dbContext.Courses.FindAsync(model.CourseId);
                existingExamSession.Shift = await _dbContext.ExamShifts.FindAsync(model.ShiftId);
                existingExamSession.Room = await _dbContext.ExamRooms.FindAsync(model.RoomId);
                existingExamSession.Teacher = await _dbContext.Teachers.FindAsync(model.TeacherId);
                existingExamSession.Staff = await _dbContext.Staff.FindAsync(model.StaffId);
                existingExamSession.Students = students;

                if (existingExamSession.ExamDate < minAllowedDate)
                {
                    return false;
                }
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
