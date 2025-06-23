using System.Collections.ObjectModel;
using Partico_Delivery.Models;
using DeliveryServiceApi = Partico_Delivery.Services.DeliveryService;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Partico_Delivery.ViewModels
{
    public class DeliveriesViewModel
    {
        public ObservableCollection<Order> Orders { get; set; } = new();
        public bool IsBusy { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        private readonly DeliveryServiceApi _deliveryService;

        public ICommand UpdateStatusCommand { get; }
        public ICommand GoToPageCommand { get; }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    // If you implement INotifyPropertyChanged, raise PropertyChanged here
                }
            }
        }

        public int TotalOrders { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public DeliveriesViewModel()
        {
            // Haal de API key uit Preferences
            string apiKey = Preferences.Default.Get("ApiKey", string.Empty);
            _deliveryService = new DeliveryServiceApi(apiKey);
            UpdateStatusCommand = new Command<Order>(async (order) => await UpdateStatusAsync(order));
            GoToPageCommand = new Command<int>(async (page) => await GoToPageAsync(page));
            if (string.IsNullOrEmpty(apiKey))
            {
                ErrorMessage = "API key ontbreekt. Vul deze in bij Instellingen.";
            }
        }

        public async Task LoadOrdersAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            ErrorMessage = string.Empty;
            try
            {
                var paged = await _deliveryService.GetOrdersPagedAsync(Page, PageSize);
                Orders.Clear();
                foreach (var order in paged.Data)
                {
                    if (order.Customer == null)
                    {
                        ErrorMessage = "Order zonder klantinformatie gevonden.";
                        continue;
                    }
                    if (order.DeliveryStates == null)
                    {
                        order.DeliveryStates = new List<DeliveryState>();
                    }
                    Orders.Add(order);
                }
                TotalOrders = paged.Total;
                Page = paged.Page;
                PageSize = paged.PageSize;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Fout bij laden van bestellingen: {ex.Message}";
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
            // Define the possible statuses in order
            var statuses = new List<string> { "Pending", "InProgress", "Delivered", "Cancelled" };
            int currentIndex = statuses.IndexOf(deliveryState.Status);
            int newIndex = (currentIndex + 1) % statuses.Count;
            string newStatus = statuses[newIndex];
            bool success = await UpdateOrderStatusAsync(deliveryState.Id, newIndex + 1); // Assuming backend expects int
            if (success)
            {
                deliveryState.Status = newStatus;
                // Forceer UI refresh
                var idx = Orders.IndexOf(order);
                if (idx >= 0)
                {
                    Orders.RemoveAt(idx);
                    Orders.Insert(idx, order);
                }
            }
        }

        private async Task GoToPageAsync(int page)
        {
            if (page < 1) return;
            Page = page;
            await LoadOrdersAsync();
        }
    }
}
