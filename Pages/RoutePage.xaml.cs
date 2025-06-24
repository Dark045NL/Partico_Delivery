namespace Partico_Delivery.Pages
{
    public partial class RoutePage : ContentPage
    {
        private Location? _currentLocation;

        public RoutePage()
        {
            InitializeComponent();
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

        private async void OnNavigateClicked(object sender, EventArgs e)
        {
            // Vervang dit met een echt adres of co�rdinaten
            string address = "Stationsplein 1, 6221 BT Maastricht";

            var encodedAddress = Uri.EscapeDataString(address);
            var uri = $"https://www.google.com/maps/dir/?api=1&destination={encodedAddress}";

            await Launcher.OpenAsync(uri);
        }
    }
}
