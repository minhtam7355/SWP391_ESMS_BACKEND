using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Models.ViewModels;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamRoomsController : ControllerBase
    {
        private readonly IExamRoomRepository _roomRepo;

        public ExamRoomsController(IExamRoomRepository roomRepo)
        {
            _roomRepo = roomRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllExamRooms()
        {
            try
            {
                return Ok(await _roomRepo.GetAllExamRoomsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetExamRoomById([FromRoute] Guid id)
        {
            try
            {
                return Ok(await _roomRepo.GetExamRoomByIdAsync(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> AddExamRoom([FromBody] ExamRoomModel model)
        {
            try
            {
                bool result = await _roomRepo.AddExamRoomAsync(model);

                if (result)
                {
                    return Ok("Added Successfully");
                }
                else
                {
                    return BadRequest("Failed to add the exam room");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateExamRoom([FromBody] ExamRoomModel model)
        {
            try
            {
                bool result = await _roomRepo.UpdateExamRoomAsync(model);

                if (result)
                {
                    return Ok("Updated Successfully");
                }
                else
                {
                    return BadRequest("Failed to update the exam room");
                }


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteExamRoom([FromBody] ExamRoomModel model)
        {
            try
            {
                bool result = await _roomRepo.DeleteExamRoomAsync(model);

                if (result)
                {
                    return Ok("Deleted Successfully");
                }
                else
                {
                    return BadRequest("Failed to delete the exam room");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
