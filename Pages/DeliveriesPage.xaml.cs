using Partico_Delivery.ViewModels;
using Partico_Delivery.Models;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Partico_Delivery.Pages
{
    public partial class DeliveriesPage : ContentPage
    {
        private DeliveriesViewModel ViewModel => BindingContext as DeliveriesViewModel;

        public DeliveriesPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModel != null)
                await ViewModel.LoadOrdersAsync();
        }

        private async void OnReloadClicked(object sender, EventArgs e)
        {
            if (ViewModel != null)
                await ViewModel.LoadOrdersAsync();
        }

        private async void OnUpdateStatusClicked(object sender, EventArgs e)
        {
            await DisplayAlert("DEBUG", "OnUpdateStatusClicked aangeroepen", "OK");
            if (sender is Button btn && btn.CommandParameter is Order order)
            {
                await DisplayAlert("DEBUG", $"OrderId: {order.Id}, DeliveryStates.Count: {order.DeliveryStates.Count}", "OK");
                if (order.DeliveryStates.Count > 0)
                {
                    var deliveryState = order.DeliveryStates[0];
                    string action = await DisplayActionSheet("Kies nieuwe status", "Annuleer", null, "1 - In behandeling", "2 - Onderweg", "3 - Afgeleverd", "4 - Niet thuis");
                    int newState = deliveryState.State;
                    if (action != null && action != "Annuleer")
                    {
                        if (action.StartsWith("1")) newState = 1;
                        else if (action.StartsWith("2")) newState = 2;
                        else if (action.StartsWith("3")) newState = 3;
                        else if (action.StartsWith("4")) newState = 4;
                        else return;

                        bool success = await ViewModel.UpdateOrderStatusAsync(deliveryState.Id, newState);
                        if (success)
                        {
                            await DisplayAlert("Succes", "Status bijgewerkt!", "OK");
                            await ViewModel.LoadOrdersAsync();
                        }
                        else
                        {
                            await DisplayAlert("Fout", "Status bijwerken mislukt.", "OK");
                        }
                    }
                }
            }
        }
    }
}
