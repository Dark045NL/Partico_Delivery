using Partico_Delivery.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DeliveryServiceApi = Partico_Delivery.Services.DeliveryService;

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

        public List<string> PossibleStatuses { get; } = new() { "Out for Delivery", "Awaiting Delivery", "Delivered" };

        // Mapping van statusnaam naar statusnummer
        private readonly Dictionary<string, int> StatusNameToNumber = new()
        {
            { "Out for Delivery", 1 },
            { "Awaiting Delivery", 2 },
            { "Delivered", 3 }
        };

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

        public event Action<string>? StatusUpdateMessage;

        public async Task UpdateOrderStatusAsync(Order order, string newStatus)
        {
            if (order.DeliveryStates.Count > 0 && StatusNameToNumber.TryGetValue(newStatus, out int statusNumber))
            {
                var deliveryState = order.DeliveryStates[0];
                deliveryState.Status = newStatus;
                await _deliveryService.UpdateDeliveryStateAsync(deliveryState.Id, statusNumber);
            }
        }

        private async Task UpdateStatusAsync(Order? order)
        {
            if (order == null || order.DeliveryStates.Count == 0)
                return;
            var deliveryState = order.DeliveryStates[0];
            var statuses = PossibleStatuses;
            int currentIndex = statuses.IndexOf(deliveryState.Status);
            int newIndex = (currentIndex + 1) % statuses.Count;
            string newStatus = statuses[newIndex];
            if (StatusNameToNumber.TryGetValue(newStatus, out int statusNumber))
            {
                bool success = await UpdateOrderStatusAsync(deliveryState.Id, statusNumber);
                if (success)
                {
                    deliveryState.Status = newStatus;
                    var idx = Orders.IndexOf(order);
                    if (idx >= 0)
                    {
                        Orders.RemoveAt(idx);
                        Orders.Insert(idx, order);
                    }
                    StatusUpdateMessage?.Invoke($"Status succesvol bijgewerkt naar '{newStatus}'");
                }
                else
                {
                    StatusUpdateMessage?.Invoke($"Fout: status kon niet worden bijgewerkt.");
                }
            }
            else
            {
                StatusUpdateMessage?.Invoke("Fout: ongeldige status.");
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
