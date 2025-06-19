using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Partico_Delivery.Models;

namespace Partico_Delivery.Services
{
    public class DeliveryService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://51.137.100.120:5000/api/Order";

        public DeliveryService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("ApiKey", "588a40b1-d4c2-4efb-9a7a-6d2b02baa7c6");
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
            var url = $"http://51.137.100.120:5000/api/DeliveryStates/UpdateDeliveryState";
            var payload = new
            {
                id = deliveryStateId,
                state = newState
            };
            var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            return response.IsSuccessStatusCode;
        }
    }
}
