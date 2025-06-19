using Partico_Delivery.Pages;

namespace Partico_Delivery
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            bool isFirstTime = Preferences.Default.Get("HasSeenOnboarding", false);

            if (!isFirstTime)
                MainPage = new OnboardingPage();
            else
                MainPage = new AppShell();
        }
    }
}
