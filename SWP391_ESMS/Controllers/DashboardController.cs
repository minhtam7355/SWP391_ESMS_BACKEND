using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP391_ESMS.Repositories;

namespace SWP391_ESMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IExamSessionRepository _examRepo;
        private readonly IStudentRepository _studentRepo;

        public DashboardController(IExamSessionRepository examRepo, IStudentRepository studentRepo)
        {
            _examRepo = examRepo;
            _studentRepo = studentRepo;
        }

        [HttpGet("linechartdata")]
        public async Task<IActionResult> GetLineChartData()
        {
            try
            {
                // Fetch data for the line chart using the injected repository instance
                var monthlyEarnings = await _examRepo.CalculateMonthlyEarningsAsync();
                var examsHeldMonthly = await _examRepo.GetNumberOfExamsHeldMonthlyAsync();
                var studentsExaminedMonthly = await _examRepo.GetNumberOfStudentsExaminedMonthlyAsync();

                // Create a data transfer object (DTO) for the line chart data
                var lineChartData = new
                {
                    MonthlyEarnings = monthlyEarnings,
                    ExamsHeldMonthly = examsHeldMonthly,
                    StudentsExaminedMonthly = studentsExaminedMonthly
                };

                return Ok(lineChartData);
            }
            catch (Exception)
            {
                // Handle the error appropriately and return a proper response
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("piechartdata")]
        public async Task<IActionResult> GetPieChartData()
        {
            try
            {
                // Fetch data for the pie chart using the injected repository instance
                var majorDistribution = await _studentRepo.GetMajorDistributionAsync();

                // Create a data transfer object (DTO) for the pie chart data
                var pieChartData = new
                {
                    MajorDistribution = majorDistribution
                };

                return Ok(pieChartData);
            }
            catch (Exception)
            {
                // Handle the error appropriately and return a proper response
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
