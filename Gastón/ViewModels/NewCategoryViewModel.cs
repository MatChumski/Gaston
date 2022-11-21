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
    public class NewCategoryViewModel : BaseViewModel
    {
        #region Attributes

        public string categoryName;

        #endregion

        #region Properties

        public string CategoryNameEntry
        {
            get { return categoryName; }
            set { SetValue(ref this.categoryName, value); }
        }

        #endregion

        #region Commands
        public ICommand NewCategoryCommand
        {
            get
            {
                return new RelayCommand(NewCategoryMethod);
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

        public async void NewCategoryMethod()
        {
            if (string.IsNullOrEmpty(CategoryNameEntry))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Fields cannot be empty", "Ok");
                return;
            }

            string sql = "SELECT * FROM CategoryModel";
            sql += $" WHERE Name = '{CategoryNameEntry}'";

            var result = await App.Database.Query<CategoryModel>(sql);

            if (result.Count > 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "There is already a category with this name", "Ok");
                return;
            }

            CategoryModel newCategory = new CategoryModel();

            newCategory.Name = CategoryNameEntry;
            newCategory.FkUser = App.ActiveUser.Id;
            newCategory.CreationDate = DateTime.Now;

            int rowsInserted = await App.Database.SaveTableModel(newCategory, "create");

            if (rowsInserted <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Category could not be created", "Ok");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Categories", "Category created succesfully", "Ok");

            await PopupNavigation.Instance.PopAsync();
        }

        private async void PopupReturn()
        {
            await PopupNavigation.Instance.PopAsync();
        }


        #endregion

    }
}
