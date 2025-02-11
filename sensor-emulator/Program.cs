using RestSharp;

class Program
{
    static async Task Main(string[] args)
    {
        string baseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5177";
        var client = new RestClient(baseUrl);
        var sensors = new[] { 1, 2, 3 };

        while (true)
        {
            foreach (var sensor in sensors)
            {
                var value = new Random().NextDouble() * 100;
                var request = new RestRequest("/api/data", Method.Post);

                request.AddJsonBody(new { SensorId = sensor, Value = value, Timestamp = DateTime.UtcNow });

                var response = await client.ExecuteAsync(request);
                await Task.Delay(1000);
            }
        }
    }
}