using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IRequestRepository
    {
        public Task<List<RequestModel>> GetAllRequestsAsync();

        public Task<List<RequestModel>> GetAllPendingProctoringRequestsAsync();

        public Task<List<RequestModel>> GetAllPendingUnproctoringRequestsAsync();

        public Task<List<RequestModel>> GetProctoringRequestsByTeacherAsync(Guid teacherId);

        public Task<List<RequestModel>> GetUnproctoringRequestsByTeacherAsync(Guid teacherId);

        public Task<RequestModel> GetRequestByIdAsync(Guid id);

        public Task<Boolean> AddProctoringRequestAsync(Guid examSessionId, Guid teacherId);

        public Task<Boolean> AddUnproctoringRequestAsync(Guid examSessionId, Guid teacherId);

        public Task<Boolean> ApproveProctoringRequestAsync(Guid id);

        public Task<Boolean> RejectProctoringRequestAsync(Guid id);

        public Task<Boolean> ApproveUnproctoringRequestAsync(Guid id);

        public Task<Boolean> RejectUnproctoringRequestAsync(Guid id);
    }
}
