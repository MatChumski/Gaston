using Gastón.ViewModels;
using Gastón.Views.PopUps;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gastón.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigView : ContentPage
    {
        public ConfigView()
        {
            InitializeComponent();
            BindingContext = new ConfigViewModel();
        }

    }


}