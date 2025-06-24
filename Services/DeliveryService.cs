using Partico_Delivery.Models;
using System.Text.Json;

namespace Partico_Delivery.Services
{
    public class DeliveryService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api-partico.onrender.com/api/Order";

        public DeliveryService(string apiKey)
        {
            _httpClient = new HttpClient();
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("API key is not set. Vul de API key in bij Instellingen.");
            _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            var response = await _httpClient.GetAsync(BaseUrl);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Order>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Order>();
        }

        public async Task<bool> UpdateDeliveryStateAsync(int deliveryStateId, int newState)
        {
            // Nieuwe endpoint voor status update (voorbeeld: /api/Order/UpdateDeliveryState)
            var url = $"https://api-partico.onrender.com/api/Order/UpdateDeliveryState";
            var payload = new
            {
                id = deliveryStateId,
                state = newState
            };
            var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<PagedOrderResponse> GetOrdersPagedAsync(int page = 1, int pageSize = 5)
        {
            var url = $"{BaseUrl}?page={page}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PagedOrderResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new PagedOrderResponse();
        }
    }
}
