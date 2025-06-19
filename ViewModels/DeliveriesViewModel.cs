using System.Collections.ObjectModel;
using Partico_Delivery.Models;
using DeliveryServiceApi = Partico_Delivery.Services.DeliveryService;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Partico_Delivery.ViewModels
{
    public class DeliveriesViewModel
    {
        public ObservableCollection<Order> Orders { get; set; } = new();
        public bool IsBusy { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        private readonly DeliveryServiceApi _deliveryService = new();

        public ICommand UpdateStatusCommand { get; }

        public DeliveriesViewModel()
        {
            UpdateStatusCommand = new Command<Order>(async (order) => await UpdateStatusAsync(order));
        }

        public async Task LoadOrdersAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var orders = await _deliveryService.GetOrdersAsync();
                Orders.Clear();
                foreach (var order in orders)
                    Orders.Add(order);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int deliveryStateId, int newState)
        {
            return await _deliveryService.UpdateDeliveryStateAsync(deliveryStateId, newState);
        }

        private async Task UpdateStatusAsync(Order? order)
        {
            if (order == null || order.DeliveryStates.Count == 0)
                return;
            var deliveryState = order.DeliveryStates[0];
            // Toon een keuzemenu voor de status (kan niet direct in ViewModel, dus evt. via MessagingCenter of een event)
            // Voor nu: verhoog status cyclisch
            int newState = deliveryState.State + 1;
            if (newState > 4) newState = 1;
            bool success = await UpdateOrderStatusAsync(deliveryState.Id, newState);
            if (success)
            {
                deliveryState.State = newState;
                // Forceer UI refresh
                var idx = Orders.IndexOf(order);
                if (idx >= 0)
                {
                    Orders.RemoveAt(idx);
                    Orders.Insert(idx, order);
                }
            }
        }
    }
}
