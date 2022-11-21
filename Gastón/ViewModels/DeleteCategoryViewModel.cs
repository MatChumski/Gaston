using GalaSoft.MvvmLight.Command;
using Gastón.Models;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Gastón.ViewModels
{
    public class DeleteCategoryViewModel : BaseViewModel
    {

        #region Attributes

        public CategoryModel deletingCategory;
        public string records;

        public string selectedCategory;
        public List<string> categories;

        #endregion

        #region Properties


        public CategoryModel DeletingCategory
        {
            get { return deletingCategory; }
            set { SetValue(ref this.deletingCategory, value); }
        }

        public string Records
        {
            get { return records; }
            set { SetValue(ref this.records, value); }
        }

        public string SelectedCategoryEntry
        {
            get { return selectedCategory; }
            set { SetValue(ref this.selectedCategory, value); }
        }

        public List<string> CategoriesList
        {
            get { return categories; }
            set { SetValue(ref this.categories, value); }
        }

        #endregion

        #region Commands

        public ICommand DeleteCategoryCommand
        {
            get
            {
                return new RelayCommand(DeleteCategoryMethod);
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

        public async void DeleteCategoryMethod()
        {
            if (string.IsNullOrEmpty(SelectedCategoryEntry))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Fields cannot be empty", "Ok");
                return;
            }

            CategoryModel selCat = new CategoryModel();

            if (SelectedCategoryEntry != "No Category")
            {
                string sqlSelectedCategory = $"SELECT * FROM CategoryModel WHERE Name = '{SelectedCategoryEntry}'";

                var selCatRes = await App.Database.Query<CategoryModel>(sqlSelectedCategory);

                selCat = selCatRes[0];
            }

            string sqlExpenses = $"SELECT * FROM ExpenseModel";
            sqlExpenses += $" WHERE FkCategory = '{DeletingCategory.Id}' AND FkUser = '{App.ActiveUser.Id}'";

            var expensesRes = await App.Database.Query<ExpenseModel>(sqlExpenses);

            foreach(ExpenseModel expense in expensesRes)
            {
                expense.FkCategory = selCat.Id;

                int updateRes = await App.Database.SaveTableModel<ExpenseModel>(expense, "update");

                if (updateRes <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Category could not be deleted", "Ok");
                    return;
                }
            }

            int rowsDeleted = await App.Database.DeleteTableModel<CategoryModel>(DeletingCategory);

            if (rowsDeleted <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Category could not be deleted", "Ok");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Categories", "Category deleted succesfully", "Ok");

            await PopupNavigation.Instance.PopAsync();
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
                foreach (CategoryModel element in result)
                {
                    if (element.Name != DeletingCategory.Name)
                    {
                        newList.Add(element.Name);
                    }
                }

            }

            CategoriesList = newList;
        }

        private async void PopupReturn()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        #endregion

        public DeleteCategoryViewModel(ConfigViewModel.UserCategoriesInfo categoryInfo)
        {
            DeletingCategory = categoryInfo.userCategory;
            Records = categoryInfo.categoryRecords.ToString();

            GetCategories();
        }

        public DeleteCategoryViewModel()
        {

        }

    }
}
