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
        public string ValidateXml(Stream xmlStream, string xsdPath)
        {
            string validationErrors = "";
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", xsdPath);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemas;
            settings.ValidationEventHandler += (s, e) =>
            {
                validationErrors += $"{e.Message}\n";
            };

            using (XmlReader reader = XmlReader.Create(xmlStream, settings))
            {
                while (reader.Read()) ;
            }

            return validationErrors;
        }
    }
}
