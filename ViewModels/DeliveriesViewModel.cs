using System.Collections.ObjectModel;
using Partico_Delivery.Models;
using DeliveryServiceApi = Partico_Delivery.Services.DeliveryService;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.ComponentModel;

namespace Partico_Delivery.ViewModels
{
    public class DeliveriesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        ObservableCollection<Order> _orders = new();
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set { if (_orders != value) { _orders = value; OnPropertyChanged(nameof(Orders)); } }
        }
        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(nameof(IsBusy)); } }
        }
        string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); } }
        }
        int _currentPage = 1;
        public int CurrentPage
        {
            get => _currentPage;
            set { if (_currentPage != value) { _currentPage = value; OnPropertyChanged(nameof(CurrentPage)); } }
        }
        int _totalOrders;
        public int TotalOrders
        {
            get => _totalOrders;
            set {
                if (_totalOrders != value) {
                    _totalOrders = value;
                    OnPropertyChanged(nameof(TotalOrders));
                    OnPropertyChanged(nameof(PageCount));
                    OnPropertyChanged(nameof(CanGoNext));
                }
            }
        }
        int _page = 1;
        public int Page
        {
            get => _page;
            set {
                if (_page != value) {
                    _page = value;
                    OnPropertyChanged(nameof(Page));
                    OnPropertyChanged(nameof(CanGoNext));
                }
            }
        }
        int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set {
                if (_pageSize != value) {
                    _pageSize = value;
                    OnPropertyChanged(nameof(PageSize));
                    OnPropertyChanged(nameof(PageCount));
                    OnPropertyChanged(nameof(CanGoNext));
                }
            }
        }
        ObservableCollection<int> _pageNumbers = new();
        public ObservableCollection<int> PageNumbers
        {
            get => _pageNumbers;
            set { if (_pageNumbers != value) { _pageNumbers = value; OnPropertyChanged(nameof(PageNumbers)); } }
        }

        private readonly DeliveryServiceApi _deliveryService;

        public ICommand UpdateStatusCommand { get; }
        public ICommand GoToPageCommand { get; }

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

        public void UpdatePageNumbers()
        {
            PageNumbers.Clear();
            int pageCount = PageCount;
            for (int i = 1; i <= pageCount; i++)
                PageNumbers.Add(i);
            OnPropertyChanged(nameof(PageNumbers));
            // Force UI update for pagination
            OnPropertyChanged(nameof(Page));
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
                UpdatePageNumbers();
                OnPropertyChanged(nameof(Orders));
                OnPropertyChanged(nameof(CanGoNext));
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
            OnPropertyChanged(nameof(CanGoNext));
        }
        public void GoToPreviousPage()
        {
            if (Page > 1)
                GoToPageCommand.Execute(Page - 1);
        }
        public async Task GoToNextPage()
        {
            int pageCount = (int)Math.Ceiling((double)TotalOrders / PageSize);
            if (Page < pageCount)
                await GoToPageAsync(Page + 1);
        }
        public int PageCount => (int)Math.Ceiling((double)TotalOrders / PageSize);
        public bool CanGoNext => Page < PageCount;
    }
}
