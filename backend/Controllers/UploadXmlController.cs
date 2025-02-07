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
            _context = context;
            _xmlValidator = new XmlValidator();
        }

        /// <summary>
        /// Принимает XML-файл, валидирует его и сохраняет данные в базу.
        /// </summary>
        /// <param name="file">Загружаемый XML-файл.</param>
        /// <returns>Статус операции.</returns>
        [HttpPost]
        public async Task<IActionResult> UploadXml(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Файл не выбран");

            // Путь к XSD схеме. Файл schema.xsd должен находиться в выходной директории.
            var xsdPath = Path.Combine(AppContext.BaseDirectory, "schema.xsd");

            // Считываем содержимое файла в MemoryStream,
            // чтобы можно было использовать один и тот же поток для валидации и парсинга.
            MemoryStream memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            // Валидация XML по XSD
            memoryStream.Position = 0;
            string errors = await Task.Run(() => _xmlValidator.ValidateXml(memoryStream, xsdPath));
            if (!string.IsNullOrEmpty(errors))
                return BadRequest($"Ошибка валидации: {errors}");

            // Сброс позиции потока для повторного чтения XML
            memoryStream.Position = 0;

            // Парсинг XML с использованием XDocument
            XDocument xmlDoc;
            try
            {
                xmlDoc = XDocument.Load(memoryStream);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка парсинга XML: {ex.Message}");
            }

            // Ожидается, что корневой элемент называется "sensors" и содержит элементы "sensor"
            var sensorElements = xmlDoc.Root?.Elements("sensor");
            if (sensorElements == null)
                return BadRequest("Некорректная структура XML: отсутствуют элементы sensor");

            // Создаем список для накопления данных датчиков
            List<SensorData> sensorDataList = new List<SensorData>();

            foreach (var sensorElem in sensorElements)
            {
                try
                {
                    int sensorId = int.Parse(
                        sensorElem.Element("id")?.Value
                            ?? throw new Exception("Отсутствует элемент id")
                    );
                    double value = double.Parse(
                        sensorElem.Element("value")?.Value
                            ?? throw new Exception("Отсутствует элемент value"),
                        CultureInfo.InvariantCulture
                    );
                    DateTime timestamp = DateTime.Parse(
                        sensorElem.Element("timestamp")?.Value
                            ?? throw new Exception("Отсутствует элемент timestamp"),
                        CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.AdjustToUniversal
                            | System.Globalization.DateTimeStyles.AssumeUniversal
                    );

                    SensorData sensorData = new SensorData
                    {
                        SensorId = sensorId,
                        Value = value,
                        Timestamp = timestamp,
                    };

                    sensorDataList.Add(sensorData);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Ошибка при обработке элемента sensor: {ex.Message}");
                }
            }

            // Сохраняем все данные в базу
            await _context.SensorData.AddRangeAsync(sensorDataList);
            await _context.SaveChangesAsync();

            return Ok("XML успешно загружен, валидирован и данные сохранены в базу");
        }
    }
}
