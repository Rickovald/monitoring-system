using System.Globalization;
using System.Xml.Linq;
using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    /// <summary>
    /// Контроллер для загрузки и валидации XML-файлов.
    /// </summary>
    [Route("api/upload-xml")]
    [ApiController]
    public class UploadXmlController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly XmlValidator _xmlValidator;

        public UploadXmlController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _xmlValidator = new XmlValidator();
        }

        /// <summary>
        /// Принимает XML-файл, валидирует его и сохраняет данные в базу.
        /// </summary>
        /// <param name="file">Загружаемый XML-файл.</param>
        /// <returns>Статус операции.</returns>
        [HttpPost]
        public async Task<IActionResult> UploadXml([FromForm] IFormFile file)
        {
            // Проверка, что файл был загружен
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "Файл не выбран" });
            }

            // Определение пути к XSD-схеме
            var xsdPath = Path.Combine(AppContext.BaseDirectory, "schema.xsd");
            if (!System.IO.File.Exists(xsdPath))
            {
                return StatusCode(500, new { error = "XSD-схема не найдена", details = $"Путь: {xsdPath}" });
            }

            try
            {
                // Считываем содержимое файла в MemoryStream
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                // Валидация XML по XSD
                memoryStream.Position = 0;
                string validationErrors = await Task.Run(() => _xmlValidator.ValidateXml(memoryStream, xsdPath));
                if (!string.IsNullOrEmpty(validationErrors))
                {
                    return BadRequest(new { error = "Ошибка валидации XML", details = validationErrors });
                }

                // Парсинг XML
                memoryStream.Position = 0;
                XDocument xmlDoc;
                try
                {
                    xmlDoc = XDocument.Load(memoryStream);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = "Ошибка парсинга XML", details = ex.Message });
                }

                // Обработка элементов XML
                var sensorElements = xmlDoc.Root?.Elements("sensor");
                if (sensorElements == null || !sensorElements.Any())
                {
                    return BadRequest(new { error = "Некорректная структура XML", details = "Отсутствуют элементы sensor" });
                }

                var sensorDataList = new List<SensorData>();
                foreach (var sensorElem in sensorElements)
                {
                    try
                    {
                        int sensorId = ParseElement<int>(sensorElem, "id", int.Parse);
                        double value = ParseElement<double>(sensorElem, "value", s => double.Parse(s, CultureInfo.InvariantCulture));
                        DateTime timestamp = ParseElement<DateTime>(
                            sensorElem,
                            "timestamp",
                            s => DateTime.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal)
                        );

                        sensorDataList.Add(new SensorData
                        {
                            SensorId = sensorId,
                            Value = value,
                            Timestamp = timestamp
                        });
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { error = "Ошибка при обработке элемента sensor", details = ex.Message });
                    }
                }

                // Сохранение данных в базу
                await _context.SensorData.AddRangeAsync(sensorDataList);
                await _context.SaveChangesAsync();

                return Ok(new { message = "XML успешно загружен, валидирован и данные сохранены в базу" });
            }
            catch (Exception ex)
            {
                // Обработка общих ошибок
                return StatusCode(500, new { error = "Необработанная ошибка", details = ex.Message });
            }
        }

        /// <summary>
        /// Универсальный метод для парсинга элементов XML.
        /// </summary>
        /// <typeparam name="T">Тип данных для парсинга.</typeparam>
        /// <param name="element">Элемент XML.</param>
        /// <param name="elementName">Имя элемента.</param>
        /// <param name="parser">Функция парсинга.</param>
        /// <returns>Распарсенное значение.</returns>
        private T ParseElement<T>(XElement element, string elementName, Func<string, T> parser)
        {
            var value = element.Element(elementName)?.Value;
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception($"Отсутствует или пустой элемент: {elementName}");
            }

            return parser(value);
        }
    }
}