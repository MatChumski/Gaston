using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Gastón.Views.PopUps;
using Gastón.ViewModels;

namespace Gastón.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewExpenseView : ContentPage
    {
        public NewExpenseView()
        {
            InitializeComponent();
            BindingContext = new NewExpenseViewModel();
        }


        

    }
}