using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SWP391_ESMS.Data;
using SWP391_ESMS.Models.Domain;
using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public class RequestRepository : IRequestRepository
    {
        private readonly ESMSDbContext _dbContext;
        private readonly IMapper _mapper;

        public RequestRepository(ESMSDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> AddProctoringRequestAsync(Guid examSessionId, Guid teacherId)
        {
            try
            {
                var request = new Request
                {
                    RequestId = Guid.NewGuid(),
                    RequestType = "Proctor",
                    RequestStatus = null,
                    RequestDate = DateTime.Now,
                    ExamSessionId = examSessionId,
                    TeacherId = teacherId
                };

                await _dbContext.Requests.AddAsync(request);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddUnproctoringRequestAsync(Guid examSessionId, Guid teacherId)
        {
            try
            {
                var request = new Request
                {
                    RequestId = Guid.NewGuid(),
                    RequestType = "Unproctor",
                    RequestStatus = null,
                    RequestDate = DateTime.Now,
                    ExamSessionId = examSessionId,
                    TeacherId = teacherId
                };

                await _dbContext.Requests.AddAsync(request);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ApproveProctoringRequestAsync(Guid id)
        {
            try
            {
                var request = await _dbContext.Requests.FindAsync(id);
                if (request != null && request!.ExamSession!.TeacherId == null)
                {
                    request.RequestStatus = true;
                    var requests = await _dbContext.Requests.Where(r => r.ExamSessionId == request.ExamSessionId &&
                                                                        r.RequestType == "Proctor" &&
                                                                        r.RequestStatus == null &&
                                                                        r.RequestId != request.RequestId)
                                                            .ToListAsync();
                    foreach (var r in requests)
                    {
                        r.RequestStatus = false;
                    }
                    request.ExamSession!.TeacherId = request.TeacherId;

                    // Get the rooms occupied by exams on the specified date and shift
                    var occupiedRooms = await _dbContext.ExamSessions
                        .Where(es => es.ExamDate == request.ExamSession.ExamDate && es.ShiftId == request.ExamSession.ShiftId && es.RoomId != null)
                        .Select(es => es.Room)
                        .ToListAsync();

                    // Get all rooms
                    var allRooms = await _dbContext.ExamRooms.ToListAsync();

                    // Get available rooms by excluding occupied rooms
                    var availableRooms = allRooms
                        .Except(occupiedRooms)
                        .OrderBy(room => room!.RoomName)
                        .ToList();
                    request.ExamSession!.RoomId = availableRooms.First()!.RoomId;

                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> ApproveUnproctoringRequestAsync(Guid id)
        {
            try
            {
                var request = await _dbContext.Requests.FindAsync(id);
                if (request != null && request!.ExamSession!.TeacherId != null)
                {
                    request.RequestStatus = true;
                    request.ExamSession!.TeacherId = null;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<RequestModel>> GetAllPendingProctoringRequestsAsync()
        {
            var requests = await _dbContext.Requests.Where(r => r.RequestType == "Proctor" && r.RequestStatus == null).ToListAsync();
            return _mapper.Map<List<RequestModel>>(requests);
        }

        public async Task<List<RequestModel>> GetAllPendingUnproctoringRequestsAsync()
        {
            var requests = await _dbContext.Requests.Where(r => r.RequestType == "Unproctor" && r.RequestStatus == null).ToListAsync();
            return _mapper.Map<List<RequestModel>>(requests);
        }

        public async Task<List<RequestModel>> GetAllRequestsAsync()
        {
            var requests = await _dbContext.Requests.ToListAsync();
            return _mapper.Map<List<RequestModel>>(requests);
        }

        public async Task<List<RequestModel>> GetProctoringRequestsByTeacherAsync(Guid teacherId)
        {
            var requests = await _dbContext.Teachers.Where(t => t.TeacherId == teacherId)
                                                .SelectMany(t => t.Requests
                                                    .Where(r => r.RequestType == "Proctor"))
                                                .ToListAsync();
            return _mapper.Map<List<RequestModel>>(requests);
        }

        public async Task<RequestModel> GetRequestByIdAsync(Guid id)
        {
            var request = await _dbContext.Requests.FindAsync(id);
            return _mapper.Map<RequestModel>(request);
        }

        public async Task<List<RequestModel>> GetUnproctoringRequestsByTeacherAsync(Guid teacherId)
        {
            var requests = await _dbContext.Teachers.Where(t => t.TeacherId == teacherId)
                                                .SelectMany(t => t.Requests
                                                    .Where(r => r.RequestType == "Unproctor"))
                                                .ToListAsync();
            return _mapper.Map<List<RequestModel>>(requests);
        }

        public async Task<bool> RejectProctoringRequestAsync(Guid id)
        {
            try
            {
                var request = await _dbContext.Requests.FindAsync(id);
                if (request != null)
                {
                    request.RequestStatus = false;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RejectUnproctoringRequestAsync(Guid id)
        {
            try
            {
                var request = await _dbContext.Requests.FindAsync(id);
                if (request != null)
                {
                    request.RequestStatus = false;
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
