using GalaSoft.MvvmLight.Command;
using Gastón.Models;
using Gastón.Views.PopUps;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;

namespace Gastón.ViewModels
{
    public class EditExpenseViewModel :BaseViewModel
    {

        #region Attributes

        public ExpenseModel editingExpense;

        public string newDescription;
        public string newSpent;
        public string newCategory;
        public string newType;
        public DateTime newDate;

        public List<string> categories;

        public bool isRefreshing;

        #endregion

        #region Properties

        public ExpenseModel EditingExpense
        {
            get { return editingExpense; }
            set { SetValue(ref this.editingExpense, value); }
        }

        public string NewDescriptionEntry
        {
            get { return newDescription; }
            set { SetValue(ref this.newDescription, value); }
        }

        public string NewSpentEntry
        {
            get { return newSpent; }
            set 
            {
                SetValue(ref this.newSpent, value);

                Regex regex = new Regex("((?![0-9]).)+");
                string newVal = regex.Replace(value, "");

                SetValue(ref this.newSpent, newVal);
            }
        }

        public string NewCategoryEntry
        {
            get { return newCategory; }
            set { SetValue(ref this.newCategory, value); }
        }

        public string NewTypeEntry
        {
            get { return newType; }
            set { SetValue(ref this.newType, value); }
        }

        public DateTime NewDateEntry
        {
            get { return newDate; }
            set { SetValue(ref this.newDate, value); }
        }

        public List<string> CategoriesList
        {
            get { return categories; }
            set { SetValue(ref this.categories, value); }
        }

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }

        #endregion

        #region Commands

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(Refresh);
            }
        }

        public ICommand EditExpenseCommand
        {
            get
            {
                return new RelayCommand(EditExpense);
            }
        }

        public ICommand NewCategoryPopupCommand
        {
            get
            {
                return new RelayCommand(OpenNewCategoryPopup);
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

        public void Refresh()
        {
            IsRefreshing = true;

            GetCategories();

            IsRefreshing = false;
        }

        public async void EditExpense()
        {
            if (string.IsNullOrEmpty(NewDescriptionEntry) || string.IsNullOrEmpty(NewSpentEntry) || string.IsNullOrEmpty(NewCategoryEntry) || string.IsNullOrEmpty(NewTypeEntry) || string.IsNullOrEmpty(NewDateEntry.ToString()))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Fields cannot be empty", "Ok");
                return;
            }


            int selectedCategory;

            if (NewCategoryEntry == "No Category")
            {
                selectedCategory = -1;
            }
            else
            {
                string sqlCategory = $"SELECT * FROM CategoryModel WHERE Name = '{NewCategoryEntry}'";

                var categoryRes = await App.Database.Query<CategoryModel>(sqlCategory);

                selectedCategory = categoryRes[0].Id;
            }

            EditingExpense.Description = NewDescriptionEntry;
            EditingExpense.Amount = float.Parse(NewSpentEntry);
            EditingExpense.FkCategory = selectedCategory;
            EditingExpense.Type = NewTypeEntry;
            EditingExpense.Date = NewDateEntry;

            var updatedRows = await App.Database.SaveTableModel<ExpenseModel>(EditingExpense, "update");

            if (updatedRows <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Expense could not be updated", "Ok");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Expenses", "Expense updated succesfully", "Ok");

            await PopupNavigation.Instance.PopAsync();

        }

        public async void GetCategories()
        {
            string lastCategory = NewCategoryEntry;

            string sql = $"SELECT * FROM CategoryModel";
            sql += $" WHERE FkUser = {App.ActiveUser.Id}";

            var result = await App.Database.Query<CategoryModel>(sql);

            List<string> newList = new List<string>();
            newList.Add("All");
            newList.Add("No Category");

            if (result.Count > 0)
            {
                foreach (CategoryModel element in result)
                {
                    newList.Add(element.Name);
                }

            }

            CategoriesList = newList;

            NewCategoryEntry = lastCategory;
        }

        private void OpenNewCategoryPopup()
        {
            PopupNavigation.Instance.PushAsync(new NewCategoryPopup());
        }

        private async void PopupReturn()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        #endregion

        public EditExpenseViewModel(ExpenseListViewModel.ExpenseCard expenseInfo)
        {
            EditingExpense = expenseInfo.Expense;

            NewDescriptionEntry = EditingExpense.Description;
            NewSpentEntry = EditingExpense.Amount.ToString();
            NewCategoryEntry = expenseInfo.Category;
            NewTypeEntry = EditingExpense.Type;
            NewDateEntry = EditingExpense.Date;

            Refresh();
        }

        public EditExpenseViewModel()
        {

        }
    }
}
