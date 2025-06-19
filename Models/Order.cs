using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Partico_Delivery.Models
{
    public class Order
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

        [JsonPropertyName("state")]
        public int State { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }

        [JsonPropertyName("deliveryServiceId")]
        public int DeliveryServiceId { get; set; }

        [JsonPropertyName("deliveryService")]
        public DeliveryService DeliveryService { get; set; } = null!;
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
}
