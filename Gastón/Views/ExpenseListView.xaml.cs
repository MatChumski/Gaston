using Gastón.ViewModels;
using Gastón.Views.PopUps;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gastón.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExpenseListView : ContentPage
    {
        public ExpenseListView()
        {
            InitializeComponent();
            BindingContext = new ExpenseListViewModel();
        }

        private void OpenDeleteExpensePopup(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new DeleteExpensePopup());
        }

        private void OpenEditExpensePopup(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new EditExpensePopup());
        }
    }
}