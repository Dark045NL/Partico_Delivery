using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;


namespace Partico_Delivery.Pages
{
    public partial class QRScannerPage : ContentPage
    {
        public QRScannerPage()
        {
            InitializeComponent();
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//home");
        }

        protected void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
        {
            var first = e.Results.FirstOrDefault();
            if (first == null) return;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("QR-code gescand", $"{first.Format} → {first.Value}", "OK");
                await Shell.Current.GoToAsync("//home");
            });
        }
    }
}
