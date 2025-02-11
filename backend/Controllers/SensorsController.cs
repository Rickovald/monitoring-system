using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Возвращает агрегированные данные (среднее, максимум, минимум) за указанный период.
        /// </summary>
        /// <param name="from">Начало периода (UTC).</param>
        /// <param name="to">Конец периода (UTC).</param>
        /// <returns>HTTP-ответ с агрегированными данными или сообщением об ошибке.</returns>
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary(
            [FromQuery, Required(ErrorMessage = "Параметр 'from' обязателен")] DateTime from,
            [FromQuery, Required(ErrorMessage = "Параметр 'to' обязателен")] DateTime to)
        {
            // Проверка корректности временного диапазона
            if (from > to)
            {
                return BadRequest(new { error = "Неверный временной диапазон", details = "'from' не может быть больше 'to'." });
            }

            try
            {
                // Получение агрегированных данных из базы
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

                // Если данных нет, возвращаем пустой список
                if (!summaries.Any())
                {
                    return Ok(new List<SensorSummaryDto>());
                }

                // Возвращаем результат с HTTP 200 OK
                return Ok(summaries);
            }
            catch (Exception ex)
            {
                // Обработка общих ошибок
                return StatusCode(500, new { error = "Ошибка при получении агрегированных данных", details = ex.Message });
            }
        }
    }
}