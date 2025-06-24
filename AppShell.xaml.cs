namespace Partico_Delivery
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            // Get the current route
            var current = Current?.CurrentItem?.Route;
            if (current != null && current != "home")
            {
                // Navigate to home if not already there
                GoToAsync("//home");
                return true; // Prevent default back behavior
            }
            return base.OnBackButtonPressed();
        }
    }
}
