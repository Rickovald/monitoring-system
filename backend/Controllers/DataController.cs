using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    /// <summary>
    /// Контроллер для работы с данными, полученными от эмулятора датчиков.
    /// </summary>
    [Route("api/data")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DataController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Принимает данные от эмулятора и сохраняет их в базе.
        /// </summary>
        /// <param name="data">Данные от датчика.</param>
        [HttpPost]
        public async Task<IActionResult> PostData([FromBody] SensorData data)
        {
            
            if (data == null)
                return BadRequest("Неверные данные");

            data.Timestamp = DateTime.UtcNow;
            _context.SensorData.Add(data);
            await _context.SaveChangesAsync();
            
            return Ok(data);
        }

        /// <summary>
        /// Возвращает данные за указанный период времени.
        /// </summary>
        /// <param name="from">Начало периода (UTC).</param>
        /// <param name="to">Конец периода (UTC).</param>
        [HttpGet]
        public async Task<IActionResult> GetData([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _context.SensorData
                .Where(d => d.Timestamp >= from && d.Timestamp <= to)
                .ToListAsync();
            return Ok(result);
        }
    }
}
