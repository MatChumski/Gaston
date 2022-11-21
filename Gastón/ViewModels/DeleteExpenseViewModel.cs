using GalaSoft.MvvmLight.Command;
using Gastón.Models;
using Gastón.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Gastón.ViewModels
{
    public class DeleteExpenseViewModel : BaseViewModel
    {

        #region Attributes

        public ExpenseModel deletingExpense;

        public string amount;
        public string category;

        #endregion

        #region Properties

        public ExpenseModel DeletingExpense
        {
            get { return deletingExpense; }
            set { SetValue(ref this.deletingExpense, value); }
        }

        public string AmountLabel
        {
            get { return amount; }
            set { SetValue(ref this.amount, value); }
        }

        public string CategoryLabel
        {
            get { return category; }
            set { SetValue(ref this.category, value); }
        }


        #endregion

        #region Commands

        public ICommand DeleteExpenseCommand
        {
            get
            {
                return new RelayCommand(DeleteExpense);
            }
        }

        public ICommand PopupReturnCommand
        {
            get
            {
                return new RelayCommand(PopupReturn);
            }
        }

        #endregion

        #region Methods

        public async void DeleteExpense()
        {

            var deletedRows = await App.Database.DeleteTableModel<ExpenseModel>(DeletingExpense);

            if (deletedRows < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Expense could not be deleted", "Ok");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Expenses", "Expense deleted succesfully", "Ok");

            await PopupNavigation.Instance.PopAsync();

        }

        private async void PopupReturn()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        #endregion

        public DeleteExpenseViewModel(ExpenseListViewModel.ExpenseCard expenseInfo)
        {
            DeletingExpense = expenseInfo.Expense;

            AmountLabel = expenseInfo.Amount;
            CategoryLabel = expenseInfo.Category;
        }

        public DeleteExpenseViewModel()
        {

        }

    }
}
