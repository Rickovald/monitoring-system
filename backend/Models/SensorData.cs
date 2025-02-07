using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    /// <summary>
    /// Модель данных, полученных с датчиков.
    /// </summary>
    public class SensorData
    {
        [Key]
        public int Id { get; set; }

        public int SensorId { get; set; }

        public double Value { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
