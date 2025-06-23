using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Partico_Delivery.ViewModels;

namespace Partico_Delivery.Pages
{
    // Duplicate StatusToTextConverter removed. The implementation now exists only in StatusToTextConverter.cs

    public partial class DeliveriesPage : ContentPage
    {
        public DeliveriesPage()
        {
            InitializeComponent();
            // Remove this line to avoid double BindingContext assignment:
            // BindingContext = new DeliveriesViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is DeliveriesViewModel vm)
            {
                await vm.LoadOrdersAsync();
            }
        }

        private void OnReloadClicked(object sender, EventArgs e)
        {
            if (BindingContext is DeliveriesViewModel vm)
            {
                _ = vm.LoadOrdersAsync();
            }
        }
    }
}