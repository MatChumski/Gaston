using GalaSoft.MvvmLight.Command;
using Gastón.Models;
using Gastón.Views.PopUps;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Gastón.ViewModels
{
    public class NewExpenseViewModel : BaseViewModel
    {

        #region Attributes

        public string description;
        public string spent;
        public string category;
        public string type;
        public DateTime date;

        public List<string> categories;

        public bool isRefreshing;

        #endregion

        #region Properties

        public string DescriptionEntry
        {
            get { return description; }
            set { SetValue(ref this.description, value); } 
        }
        public string SpentEntry
        {
            get { return spent; }
            set { SetValue(ref this.spent, value); }
        }
        public string CategoryEntry
        {
            get { return category; }
            set { SetValue(ref this.category, value); }
        }
        public string TypeEntry
        {
            get { return type; }
            set { SetValue(ref this.type, value); }
        }
        public DateTime DateEntry
        {
            get { return date; }
            set { SetValue(ref this.date, value); }
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

        public ICommand NewExpenseCommand
        {
            get
            {
                return new RelayCommand(NewExpense);
            }
        }

        public ICommand NewCategoryPopupCommand
        {
            get
            {
                return new RelayCommand(OpenNewCategoryPopup);
            }
        }

        public ICommand GetCategoriesCommand
        {
            get
            {
                return new RelayCommand(GetCategories);
            }
        }

        #endregion

        #region Methods

        public async void NewExpense()
        {
            if (string.IsNullOrEmpty(DescriptionEntry) || string.IsNullOrEmpty(SpentEntry) || string.IsNullOrEmpty(CategoryEntry) || string.IsNullOrEmpty(TypeEntry) || string.IsNullOrEmpty(DateEntry.ToString()))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Fields cannot be empty", "Ok");
                return;
            }

            string sqlCategory = $"SELECT * FROM CategoryModel WHERE Name = '{CategoryEntry}'";

            int selectedCategory;

            if (CategoryEntry == "No Category")
            {
                selectedCategory = -1;
            } else
            {
                var categoryRes = await App.Database.Query<CategoryModel>(sqlCategory);

                selectedCategory = categoryRes[0].Id;

            }

            ExpenseModel newExpense = new ExpenseModel();

            newExpense.Description = DescriptionEntry;
            newExpense.Amount = float.Parse(SpentEntry);
            newExpense.FkCategory = selectedCategory;
            newExpense.Type = TypeEntry;
            newExpense.FkUser = App.ActiveUser.Id;
            newExpense.Date = DateEntry;
            newExpense.DateCreation = DateTime.Now;

            var rowsInserted = await App.Database.SaveTableModel<ExpenseModel>(newExpense, "insert");

            if (rowsInserted <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Expense could not be created", "Ok");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Expenses", "Expense created succesfully", "Ok");

        }

        public async void GetCategories()
        {
            string sql = $"SELECT * FROM CategoryModel";
            sql += $" WHERE FkUser = {App.ActiveUser.Id}";

            var result = await App.Database.Query<CategoryModel>(sql);

            List<string> newList = new List<string>();
            newList.Add("No Category");

            if (result.Count > 0)
            {
                foreach(CategoryModel element in result)
                {
                    newList.Add(element.Name);
                }

            }
            CategoriesList = newList;

            IsRefreshing = false;
        }

        private void OpenNewCategoryPopup()
        {
            PopupNavigation.Instance.PushAsync(new NewCategoryPopup());
        }

        #endregion

        public NewExpenseViewModel()
        {
            GetCategories();

            DateEntry = DateTime.Now;
        }

    }
}
