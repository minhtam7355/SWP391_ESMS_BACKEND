using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamFormatsController : ControllerBase
    {
        private readonly IExamFormatRepository _formatRepo;

        public ExamFormatsController(IExamFormatRepository formatRepo)
        {
            _formatRepo = formatRepo;
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllExamFormats()
        {
            try
            {
                return Ok(await _formatRepo.GetAllExamFormatsAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
