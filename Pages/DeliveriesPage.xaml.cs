using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Linq;
using Partico_Delivery.ViewModels;
using Partico_Delivery.Models;

namespace Partico_Delivery.Pages
{
    // Duplicate StatusToTextConverter removed. The implementation now exists only in StatusToTextConverter.cs

    public partial class DeliveriesPage : ContentPage
    {
        public static RouteViewModel SharedRouteViewModel = new RouteViewModel();

        public DeliveriesPage()
        {
            InitializeComponent();
            var vm = new DeliveriesViewModel();
            vm.RouteVM = SharedRouteViewModel;
            BindingContext = vm;
            vm.StatusUpdateMessage += async (msg) =>
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await DisplayAlert("Status update", msg, "OK");
                });
            };
        }

        // Toast helper
        private void ShowToast(string message)
        {
            if (Application.Current?.MainPage != null)
            {
                ToastService.Show(message);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is DeliveriesViewModel vm)
            {
                await vm.LoadOrdersAsync();
                this.Dispatcher.Dispatch(UpdatePaginationButtons);
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
                btn.Clicked += (s, e) => this.Dispatcher.Dispatch(UpdatePaginationButtons);
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
                this.Dispatcher.Dispatch(UpdatePaginationButtons);
            }
        }

        private async void OnNextPageClicked(object sender, EventArgs e)
        {
            if (BindingContext is DeliveriesViewModel vm)
            {
                await vm.GoToNextPage();
                this.Dispatcher.Dispatch(UpdatePaginationButtons);
            }
        }

        private void OnAddToRouteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Order order && BindingContext is DeliveriesViewModel vm)
            {
                vm.RouteVM.AddOrderToRoute(order);
                order.InRoute = true;
                // Force refresh of the item in the ObservableCollection
                int idx = vm.Orders.IndexOf(order);
                if (idx >= 0)
                {
                    vm.Orders.RemoveAt(idx);
                    vm.Orders.Insert(idx, order);
                }
                ToastService.Show($"Toegevoegd aan route: {order.Customer?.Name ?? order.Id.ToString()}");
            }
        }

        private void OnRemoveFromRouteClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Order order && BindingContext is DeliveriesViewModel vm)
            {
                vm.RouteVM.RemoveOrderFromRoute(order);
                order.InRoute = false;
                // Force refresh of the item in the ObservableCollection
                int idx = vm.Orders.IndexOf(order);
                if (idx >= 0)
                {
                    vm.Orders.RemoveAt(idx);
                    vm.Orders.Insert(idx, order);
                }
                ToastService.Show($"Verwijderd uit route: {order.Customer?.Name ?? order.Id.ToString()}");
            }
        }
    }
}