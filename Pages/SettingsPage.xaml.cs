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

            // Laad API key bij openen
            string apiKey = Preferences.Default.Get("ApiKey", string.Empty);
            ApiKeyEntry.Text = apiKey;
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

        private void OnApiKeyChanged(object sender, TextChangedEventArgs e)
        {
            Preferences.Default.Set("ApiKey", e.NewTextValue ?? string.Empty);
            // Optioneel: feedback tonen
            // StatusLabel.Text = "API key opgeslagen.";
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//home");
        }

        private void OnSaveApiKeyClicked(object sender, EventArgs e)
        {
            Preferences.Default.Set("ApiKey", ApiKeyEntry.Text ?? string.Empty);
            StatusLabel.Text = "API key opgeslagen.";
        }
    }
}