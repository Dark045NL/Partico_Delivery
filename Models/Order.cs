using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Partico_Delivery.Models
{
    public class Order : INotifyPropertyChanged
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonPropertyName("customerId")]
        public int CustomerId { get; set; }

        [JsonPropertyName("customer")]
        public Customer Customer { get; set; } = null!;

        [JsonPropertyName("products")]
        public List<Product> Products { get; set; } = new();

        [JsonPropertyName("deliveryStates")]
        public List<DeliveryState> DeliveryStates { get; set; } = new();

        public static string GetStatusText(int state)
        {
            return state switch
            {
                1 => "In behandeling",
                2 => "Onderweg",
                3 => "Afgeleverd",
                4 => "Niet thuis",
                _ => "Onbekend"
            };
        }

        public string? FirstDeliveryStatus => DeliveryStates != null && DeliveryStates.Count > 0 ? DeliveryStates[0].Status : null;

        // Toegevoegd: GPS-coördinaten voor route/navigatie
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private bool _inRoute;
        public bool InRoute
        {
            get => _inRoute;
            set { if (_inRoute != value) { _inRoute = value; OnPropertyChanged(nameof(InRoute)); } }
        }
    }

    public class Customer
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;
    }

    public class DeliveryState
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }
    }

    public class DeliveryService
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class Product
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }

    public class PagedOrderResponse
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }
        [JsonPropertyName("page")]
        public int Page { get; set; }
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
        [JsonPropertyName("data")]
        public List<Order> Data { get; set; } = new();
    }
}
