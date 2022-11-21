using Gastón.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gastón.Views.PopUps
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeleteExpensePopup
    {
        public DeleteExpensePopup()
        {
            InitializeComponent();
        }

        public DeleteExpensePopup(ExpenseListViewModel.ExpenseCard expenseInfo)
        {
            InitializeComponent();
            BindingContext = new DeleteExpenseViewModel(expenseInfo);
        }

        private async void PopupReturn(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }
    }
}