﻿using AutoMapper;
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
                // Fetch students taking the course
                var courseEnrollments = await _dbContext.Courses
                    .Where(ce => ce.CourseId == model.CourseId)
                    .Select(ce => ce.Students)
                    .SingleOrDefaultAsync();

                // Determine the number of exam sessions needed
                if (courseEnrollments == null) return false;
                int numStudents = courseEnrollments.Count;
                int maxStudentsPerSession = 30;
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

                // Remove combinations that are already in use on the same date
                var availableCombinations = allCombinations
                    .Where(combination =>
                        !existingSessionsSameDate.Any(existingSession =>
                        existingSession.RoomId == combination.room.RoomId && existingSession.ShiftId == combination.shift.ShiftId))
                    .ToList();

                // Check if the number of available combinations is lower than the number of numSessions
                if (availableCombinations.Count < numSessions)
                {
                    return false;
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
                        ExamDate = model.ExamDate,
                        RoomId = availableCombination.room.RoomId,
                        TeacherId = null,
                        StudentsEnrolled = 0, // Initialize students enrolled to 0
                        StaffId = model.StaffId,
                        ShiftId = availableCombination.shift.ShiftId,
                        IsPassed = false,
                        IsPaid = false
                    };

                    // Fetch 30 students for this session and register them
                    int studentsToTake = Math.Min(30, courseEnrollments.Count); // Take up to 30 students or the remaining students.

                    var studentsToAdd = courseEnrollments.Take(studentsToTake).ToList();
                    var studentsForSession = new List<Student>();
                    studentsForSession.AddRange(studentsToAdd);
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

        public async Task<Boolean> DeleteExamSessionAsync(ExamSessionModel model)
        {
            var deleteExamSession = await _dbContext.ExamSessions.FindAsync(model.ExamSessionId);
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
            return _mapper.Map<List<ExamSessionModel>>(examSessions);

        }

        public async Task<ExamSessionModel> GetExamSessionByIdAsync(Guid id)
        {
            var examSession = await _dbContext.ExamSessions.FindAsync(id);
            return _mapper.Map<ExamSessionModel>(examSession);

        }

        public async Task<Boolean> UpdateExamSessionAsync(ExamSessionModel model)
        {
            var existingExamSession = await _dbContext.ExamSessions.FindAsync(model.ExamSessionId);

            if (existingExamSession != null)
            {
                _mapper.Map(model, existingExamSession);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
