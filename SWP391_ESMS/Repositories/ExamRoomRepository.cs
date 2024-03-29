﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;
using System.Linq;

namespace SWP391_ESMS.Repositories
{
    public class ExamRoomRepository : IExamRoomRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public ExamRoomRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Boolean> AddExamRoomAsync(ExamRoomModel model)
        {
            try
            {
                var existingRoomName = await _dbContext.ExamRooms.Select(r => r.RoomName).FirstOrDefaultAsync(r => r == model.RoomName);
                if (existingRoomName != null)
                {
                    return false;
                }
                var newRoom = _mapper.Map<ExamRoom>(model);
                newRoom.RoomId = Guid.NewGuid();
                await _dbContext.ExamRooms.AddAsync(newRoom);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Boolean> DeleteExamRoomAsync(Guid id)
        {
            var deleteRoom = await _dbContext.ExamRooms.FindAsync(id);
            if (deleteRoom != null)
            {
                _dbContext.ExamRooms.Remove(deleteRoom);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<ExamRoomModel>> GetAllExamRoomsAsync()
        {
            var rooms = await _dbContext.ExamRooms.ToListAsync();
            return _mapper.Map<List<ExamRoomModel>>(rooms);
        }

        public async Task<Boolean> GetAvailableRoomsAsync(DateTime? examDate, Guid? shiftId)
        {
            try
            {
                
                var occupiedRooms = await _dbContext.ExamSessions
                    .Where(es => es.ExamDate == examDate && es.ShiftId == shiftId && es.RoomId != null)
                    .Select(es => es.Room)
                    .ToListAsync();

                
                var allRooms = await _dbContext.ExamRooms.ToListAsync();

                
                var availableRooms = allRooms.Except(occupiedRooms);

                return availableRooms.Any();
            }
            catch (Exception)
            {
                return false; 
            }
        }

        public async Task<ExamRoomModel> GetExamRoomByIdAsync(Guid id)
        {
            var room = await _dbContext.ExamRooms.FindAsync(id);
            return _mapper.Map<ExamRoomModel>(room);
        }

        public async Task<Guid> GetExamRoomIdByName(string roomName)
        {
            var room = await _dbContext.ExamRooms.FirstOrDefaultAsync(r => r.RoomName == roomName);
            return room!.RoomId;
        }

        public async Task<Boolean> UpdateExamRoomAsync(ExamRoomModel model)
        {
            var existingRoom = await _dbContext.ExamRooms.FindAsync(model.RoomId);

            if (existingRoom != null)
            {
                if (model.RoomName != existingRoom.RoomName)
                {
                    var existingRoomName = await _dbContext.ExamRooms.Select(r => r.RoomName).FirstOrDefaultAsync(r => r == model.RoomName);
                    if (existingRoomName != null)
                    {
                        return false;
                    }
                }
                _mapper.Map(model, existingRoom);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
