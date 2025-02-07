using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    /// <summary>
    /// Контроллер для получения агрегированных данных по датчикам.
    /// </summary>
    [Route("api/sensors")]
    [ApiController]
    public class SensorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SensorsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Возвращает агрегированные данные (среднее, максимум, минимум) за указанный период.
        /// </summary>
        /// <param name="from">Начало периода (UTC).</param>
        /// <param name="to">Конец периода (UTC).</param>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var summaries = await _context.SensorData
                .Where(d => d.Timestamp >= from && d.Timestamp <= to)
                .GroupBy(d => d.SensorId)
                .Select(g => new SensorSummaryDto
                {
                    SensorId = g.Key,
                    Average = g.Average(d => d.Value),
                    Maximum = g.Max(d => d.Value),
                    Minimum = g.Min(d => d.Value)
                })
                .ToListAsync();

            return Ok(summaries);
        }
    }
}
