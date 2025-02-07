namespace backend.Models
{
    /// <summary>
    /// DTO для передачи агрегированных данных датчиков.
    /// </summary>
    public class SensorSummaryDto
    {
        public int SensorId { get; set; }
        public double Average { get; set; }
        public double Maximum { get; set; }
        public double Minimum { get; set; }
    }
}