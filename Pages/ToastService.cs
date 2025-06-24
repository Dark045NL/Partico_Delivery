namespace Partico_Delivery.Pages
{
    public static class ToastService
    {
        public static void Show(string message)
        {
#if ANDROID
            Android.Widget.Toast.MakeText(Android.App.Application.Context, message, Android.Widget.ToastLength.Short)?.Show();
#elif IOS
            // iOS: eventueel implementeren met een 3rd party of eigen overlay
#elif MACCATALYST
            // macOS: eventueel implementeren met een 3rd party of eigen overlay
#else
            Application.Current?.MainPage?.DisplayAlert("Melding", message, "OK");
#endif
        }
    }
}