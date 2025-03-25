using CrazyLibraryAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrazyLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataImportController : ControllerBase
    {
        private readonly LibraryDataImportService _importService;
        private readonly ILogger<DataImportController> _logger;

        public DataImportController(
            LibraryDataImportService importService,
            ILogger<DataImportController> logger)
        {
            _importService = importService;
            _logger = logger;
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportData(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return BadRequest("File path cannot be empty");
                }

                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound($"File not found: {filePath}");
                }

                await _importService.ImportFromJsonFile(filePath);
                return Ok($"Successfully imported data from {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing data");
                return StatusCode(500, $"An error occurred while importing data: {ex.Message}");
            }
        }
    }
}
