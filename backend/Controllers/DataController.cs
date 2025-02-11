using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Принимает данные от эмулятора и сохраняет их в базе.
        /// </summary>
        /// <param name="data">Данные от датчика.</param>
        /// <returns>HTTP-ответ с сохраненными данными или сообщением об ошибке.</returns>
        [HttpPost]
        public async Task<IActionResult> PostData([FromBody] SensorData data)
        {
            // Проверка входных данных на null
            if (data == null)
            {
                return BadRequest(new { error = "Неверные данные", details = "Тело запроса не может быть пустым." });
            }

            // Валидация модели данных
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                // Установка временной метки в UTC
                data.Timestamp = DateTime.UtcNow;

                // Добавление данных в контекст и сохранение изменений
                _context.SensorData.Add(data);
                await _context.SaveChangesAsync();

                // Возвращаем созданные данные с HTTP 201 Created
                return CreatedAtAction(nameof(GetData), new { from = data.Timestamp, to = data.Timestamp }, data);
            }
            catch (DbUpdateException ex)
            {
                // Обработка ошибок базы данных
                return StatusCode(500, new { error = "Ошибка при сохранении данных", details = ex.Message });
            }
        }

        /// <summary>
        /// Возвращает данные за указанный период времени.
        /// </summary>
        /// <param name="from">Начало периода (UTC).</param>
        /// <param name="to">Конец периода (UTC).</param>
        /// <returns>HTTP-ответ с данными за указанный период или сообщением об ошибке.</returns>
        [HttpGet]
        public async Task<IActionResult> GetData(
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
                // Получение данных из базы с фильтрацией по временному диапазону
                var result = await _context.SensorData
                    .Where(d => d.Timestamp >= from && d.Timestamp <= to)
                    .ToListAsync();

                // Возвращаем результат с HTTP 200 OK
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Обработка общих ошибок
                return StatusCode(500, new { error = "Ошибка при получении данных", details = ex.Message });
            }
        }
    }
}