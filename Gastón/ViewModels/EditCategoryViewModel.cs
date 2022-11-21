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
    public class EditCategoryViewModel : BaseViewModel
    {

        #region Attributes

        public string newName;
        public CategoryModel editingCategory;

        #endregion

        #region Properties

        public string NewNameEntry
        {
            get { return newName; }
            set { SetValue(ref this.newName, value);  }
        }

        public CategoryModel EditingCategory
        {
            get { return editingCategory; }
            set { SetValue(ref this.editingCategory, value); }
        }

        #endregion

        #region Commands

        public ICommand EditCategoryCommand
        {
            get
            {
                return new RelayCommand(EditCategoryMethod);
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

        public async void EditCategoryMethod()
        {
            if (string.IsNullOrEmpty(NewNameEntry))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Fields cannot be empty", "Ok");
                return;
            }

            string sql = "SELECT * FROM CategoryModel";
            sql += $" WHERE Name = '{NewNameEntry}'";

            var result = await App.Database.Query<CategoryModel>(sql);

            if (result.Count > 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "There is already a category with this name", "Ok");
                return;
            }

            EditingCategory.Name = NewNameEntry;

            int rowsUpdated = await App.Database.SaveTableModel(EditingCategory, "update");

            if (rowsUpdated <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Category could not be updated", "Ok");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Categories", "Category updated succesfully", "Ok");

            await PopupNavigation.Instance.PopAsync();
        }

        private async void PopupReturn()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        #endregion

        public EditCategoryViewModel(ConfigViewModel.UserCategoriesInfo categoryInfo)
        {
            EditingCategory = categoryInfo.userCategory;

            NewNameEntry = EditingCategory.Name;
        }
        
        public EditCategoryViewModel()
        {

        }

    }
}
