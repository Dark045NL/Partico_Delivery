using System.Collections.ObjectModel;
using System.Linq;
using Partico_Delivery.Models;
using Partico_Delivery.Services;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

namespace Partico_Delivery.ViewModels
{
    public class RouteViewModel : BaseViewModel
    {
        public ObservableCollection<Order> AvailableOrders { get; set; } = new();
        public Route CurrentRoute { get; set; } = new Route { Name = string.Empty };

        private readonly Partico_Delivery.Services.DeliveryService _deliveryService;

        public RouteViewModel()
        {
            string apiKey = Preferences.Default.Get("ApiKey", string.Empty);
            _deliveryService = new Partico_Delivery.Services.DeliveryService(apiKey);
        }

        public async Task LoadOrdersAsync()
        {
            AvailableOrders.Clear();
            var paged = await _deliveryService.GetOrdersPagedAsync(1, 100);
            foreach (var order in paged.Data)
            {
                if (order.Customer != null)
                    AvailableOrders.Add(order);
            }
        }

        public void AddOrderToRoute(Order order)
        {
            if (!CurrentRoute.Orders.Any(ro => ro.OrderId == order.Id))
            {
                int nextSeq = CurrentRoute.Orders.Count + 1;
                CurrentRoute.Orders.Add(new RouteOrder { OrderId = order.Id, Sequence = nextSeq });
            }
        }

        public void MarkOrderAsDelivered(int orderId)
        {
            var routeOrder = CurrentRoute.Orders.FirstOrDefault(ro => ro.OrderId == orderId);
            if (routeOrder != null)
            {
                routeOrder.Delivered = true;
                CurrentRoute.CurrentOrderIndex++;
            }
        }

        public (double lat, double lng)? GetNextOrderGps()
        {
            if (CurrentRoute.CurrentOrderIndex < CurrentRoute.Orders.Count)
            {
                var nextOrderId = CurrentRoute.Orders[CurrentRoute.CurrentOrderIndex].OrderId;
                var nextOrder = AvailableOrders.FirstOrDefault(o => o.Id == nextOrderId);
                if (nextOrder != null)
                    return (nextOrder.Latitude, nextOrder.Longitude);
            }
            return null;
        }

        public bool IsOrderInRoute(int orderId)
        {
            return CurrentRoute.Orders.Any(ro => ro.OrderId == orderId);
        }

        public void RemoveOrderFromRoute(Order order)
        {
            var routeOrder = CurrentRoute.Orders.FirstOrDefault(ro => ro.OrderId == order.Id);
            if (routeOrder != null)
            {
                CurrentRoute.Orders.Remove(routeOrder);
                // Herbereken de volgorde
                int seq = 1;
                foreach (var ro in CurrentRoute.Orders)
                {
                    ro.Sequence = seq++;
                }
            }
        }
    }
}
