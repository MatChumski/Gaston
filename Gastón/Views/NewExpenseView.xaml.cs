﻿using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using Gastón.Views.PopUps;

namespace Gastón.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewExpenseView : ContentPage
    {
        public NewExpenseView()
        {
            InitializeComponent();
        }

        public async void click()
        {
            await Application.Current.MainPage.DisplayAlert("Alert", "Clickeado", "Ok");
        }

        private void OpenNewCategoryPopup(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new NewCategoryPopup());
        }

    }
}