namespace Partico_Delivery.Pages
{
    public partial class OnboardingPage : ContentPage
    {
        public OnboardingPage()
        {
            InitializeComponent();
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            Preferences.Default.Set("HasSeenOnboarding", true);
            if (Application.Current != null)
            {
                Application.Current.MainPage = new AppShell(); // Gaat naar HomePage
            }
        }

        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            Shell.Current.GoToAsync("//home");
        }
    }
}
