using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Linq;
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
                Device.BeginInvokeOnMainThread(UpdatePaginationButtons);
            }
        }

        private void UpdatePaginationButtons()
        {
            if (BindingContext is not DeliveriesViewModel vm) return;
            PaginationNumbers.Children.Clear();
            foreach (var pageNum in vm.PageNumbers)
            {
                var btn = new Button
                {
                    Text = pageNum.ToString(),
                    Command = vm.GoToPageCommand,
                    CommandParameter = pageNum,
                    Margin = new Thickness(2,0),
                    MinimumWidthRequest = 36,
                    BackgroundColor = (pageNum == vm.Page) ? null : Colors.LightGray,
                    TextColor = (pageNum == vm.Page) ? null : Colors.Black,
                    Opacity = (pageNum == vm.Page) ? 1.0 : 0.7
                };
                btn.Clicked += (s, e) => Device.BeginInvokeOnMainThread(UpdatePaginationButtons);
                PaginationNumbers.Children.Add(btn);
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
            UpdatePaginationButtons();
        }

        private void OnPrevPageClicked(object sender, EventArgs e)
        {
            if (BindingContext is DeliveriesViewModel vm)
            {
                vm.GoToPreviousPage();
                Device.BeginInvokeOnMainThread(UpdatePaginationButtons);
            }
        }

        private async void OnNextPageClicked(object sender, EventArgs e)
        {
            if (BindingContext is DeliveriesViewModel vm)
            {
                await vm.GoToNextPage();
                Device.BeginInvokeOnMainThread(UpdatePaginationButtons);
            }
        }
    }
}