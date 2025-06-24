namespace Partico_Delivery.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void OnScanClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//scanner");
        }

        private async void OnRouteClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//route");
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//settings");
        }

        private async void OnDeliveriesClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//deliveries");
        }
    }
}
