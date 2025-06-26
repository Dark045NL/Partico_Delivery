using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using Partico_Delivery.Models;
using Partico_Delivery.ViewModels;

namespace Partico_Delivery.Pages
{
    public partial class RoutePage : ContentPage
    {
        private Location? _currentLocation;
        private readonly RouteViewModel _sharedRouteViewModel;

        public RoutePage()
        {
            InitializeComponent();
            _sharedRouteViewModel = DeliveriesPage.SharedRouteViewModel;
            BindingContext = _sharedRouteViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _sharedRouteViewModel.LoadOrdersAsync();
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//home");
        }

        private async void OnGetLocationClicked(object sender, EventArgs e)
        {
            try
            {
                _currentLocation = await Geolocation.GetLastKnownLocationAsync();

                if (_currentLocation == null)
                {
                    _currentLocation = await Geolocation.GetLocationAsync(
                        new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));
                }

                if (_currentLocation != null)
                {
                    locationLabel.Text = $"Huidige locatie:\nLat: {_currentLocation.Latitude}\nLong: {_currentLocation.Longitude}";
                }
                else
                {
                    locationLabel.Text = "Geen locatie gevonden.";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", $"Locatie ophalen mislukt: {ex.Message}", "OK");
            }
        }

        private void OnAddOrderClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is Order order)
            {
                _sharedRouteViewModel.AddOrderToRoute(order);
            }
        }

        private void OnMarkDeliveredClicked(object sender, EventArgs e)
        {
            if (_sharedRouteViewModel.CurrentRoute.Orders.Count > _sharedRouteViewModel.CurrentRoute.CurrentOrderIndex)
            {
                var currentOrderId = _sharedRouteViewModel.CurrentRoute.Orders[_sharedRouteViewModel.CurrentRoute.CurrentOrderIndex].OrderId;
                _sharedRouteViewModel.MarkOrderAsDelivered(currentOrderId);
            }
        }

        private async void OnNavigateClicked(object sender, EventArgs e)
        {
            var nextGps = _sharedRouteViewModel.GetNextOrderGps();
            if (nextGps != null)
            {
                var (lat, lng) = nextGps.Value;
                var uri = $"https://www.google.com/maps/dir/?api=1&destination={lat},{lng}";
                await Launcher.OpenAsync(uri);
            }
            else
            {
                await DisplayAlert("Route klaar", "Er zijn geen volgende stops meer in de route.", "OK");
            }
        }

        public async void OnReloadClicked(object sender, EventArgs e)
        {
            await _sharedRouteViewModel.LoadOrdersAsync();
        }
    }
}
