using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace backend.Services
{
    /// <summary>
    /// Сервис для валидации XML-файлов по XSD-схеме.
    /// </summary>
    public class XmlValidator
    {
        /// <summary>
        /// Валидирует XML, используя заданную XSD-схему.
        /// </summary>
        /// <param name="xmlStream">Поток XML-файла.</param>
        /// <param name="xsdPath">Путь к файлу XSD.</param>
        /// <returns>Возвращает сообщение об ошибке, если валидация не пройдена, или пустую строку в случае успеха.</returns>
        /// <exception cref="ArgumentException">Выбрасывается, если xsdPath некорректен.</exception>
        public string ValidateXml(Stream xmlStream, string xsdPath)
        {
            if (string.IsNullOrEmpty(xsdPath))
            {
                throw new ArgumentException("Путь к XSD-схеме не может быть пустым.", nameof(xsdPath));
            }

            if (!File.Exists(xsdPath))
            {
                throw new FileNotFoundException("XSD-схема не найдена.", xsdPath);
            }

            try
            {
                // Создаем набор схем для валидации
                var schemas = new XmlSchemaSet();
                schemas.Add("", xsdPath);

                // Настройки для валидации
                var settings = new XmlReaderSettings
                {
                    ValidationType = ValidationType.Schema,
                    Schemas = schemas
                };

                // Сбор ошибок валидации
                var validationErrors = new System.Text.StringBuilder();
                settings.ValidationEventHandler += (s, e) =>
                {
                    validationErrors.AppendLine(e.Message);
                };

                // Чтение XML с валидацией
                xmlStream.Position = 0; // Убедимся, что поток находится в начале
                using (var reader = XmlReader.Create(xmlStream, settings))
                {
                    while (reader.Read()) { }
                }

                // Возвращаем ошибки, если они есть
                return validationErrors.ToString();
            }
            catch (Exception ex)
            {
                // Обработка общих ошибок
                throw new InvalidOperationException("Ошибка при валидации XML.", ex);
            }
        }
    }
}