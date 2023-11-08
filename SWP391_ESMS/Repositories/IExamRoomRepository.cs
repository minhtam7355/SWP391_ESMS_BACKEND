using SWP391_ESMS.Models.ViewModels;

namespace SWP391_ESMS.Repositories
{
    public interface IExamRoomRepository
    {
        public Task<List<ExamRoomModel>> GetAllExamRoomsAsync();

        public Task<ExamRoomModel> GetExamRoomByIdAsync(Guid id);

        public Task<Boolean> AddExamRoomAsync(ExamRoomModel model);

        public Task<Boolean> UpdateExamRoomAsync(ExamRoomModel model);

        public Task<Boolean> DeleteExamRoomAsync(Guid id);

        public Task<Guid> GetExamRoomIdByName(string roomName);
    }
}
