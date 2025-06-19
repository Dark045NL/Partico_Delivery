using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Partico_Delivery.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            // Laad voorkeur bij openen
            bool isEnabled = Preferences.Default.Get("PushNotifications", true);
            NotificationSwitch.IsToggled = isEnabled;
            StatusLabel.Text = $"Pushnotificaties zijn {(isEnabled ? "ingeschakeld" : "uitgeschakeld")}.";
        }

        private void OnNotificationToggled(object sender, ToggledEventArgs e)
        {
            bool isEnabled = e.Value;

            // Opslaan in lokale voorkeuren
            Preferences.Default.Set("PushNotifications", isEnabled);

            // Feedback geven
            StatusLabel.Text = $"Pushnotificaties zijn {(isEnabled ? "ingeschakeld" : "uitgeschakeld")}.";

            // Hier kun je eventueel je push logic aanroepen (abonneren/uitschrijven)
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//home");
        }
    }
}
