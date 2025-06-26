using Partico_Delivery.ViewModels;

namespace Partico_Delivery.Pages
{
    // Duplicate StatusToTextConverter removed. The implementation now exists only in StatusToTextConverter.cs

    public partial class DeliveriesPage : ContentPage
    {
        public DeliveriesPage()
        {
            InitializeComponent();
            var vm = new DeliveriesViewModel();
            BindingContext = vm;
            vm.StatusUpdateMessage += async (msg) =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Status update", msg, "OK");
                });
            };
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is DeliveriesViewModel vm)
            {
                await vm.LoadOrdersAsync();
            }
        }

        private void OnReloadClicked(object sender, EventArgs e)
        {
            if (BindingContext is DeliveriesViewModel vm)
            {
                _ = vm.LoadOrdersAsync();
            }
        }

        private async void OnStatusChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker && picker.BindingContext is Partico_Delivery.Models.Order order)
            {
                var selectedStatus = picker.SelectedItem as string;
                var currentStatus = order.DeliveryStates.Count > 0 ? order.DeliveryStates[0].Status : null;
                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != currentStatus && BindingContext is DeliveriesViewModel vm)
                {
                    await vm.UpdateOrderStatusAsync(order, selectedStatus);
                }
            }
        }
    }
}