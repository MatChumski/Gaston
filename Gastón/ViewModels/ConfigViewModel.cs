using GalaSoft.MvvmLight.Command;
using Gastón.Models;
using Gastón.Views;
using Gastón.Views.PopUps;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Gastón.ViewModels
{
    public class ConfigViewModel : BaseViewModel
    {

        #region Attributes

        // User info
        public string activeUsername;
        public string activeEmail;

        // For the expenses with no category
        public int noCategory;
        public string noCategoryAmount;

        /**
         * 
         * Class for the category cards
         * 
         * CategoryModel    userCategory        Model with the info of the category
         * 
         * int              categoryRecords     Number of expenses associated with the category 
         *                                      and the active user
         *                                      
         * string           categoryAmount      Total amount acummulated for the category in
         *                                      price format
         */
        public class UserCategoriesInfo
        {
            public CategoryModel userCategory { get; set; }
            public int categoryRecords { get; set; }

            public string categoryAmount { get; set; }

            public UserCategoriesInfo(CategoryModel userCategory, int categoryRecords, string categoryAmount)
            {
                this.userCategory = userCategory;
                this.categoryRecords = categoryRecords;
                this.categoryAmount = categoryAmount;
            }
        }

        // List with all the categories
        public List<UserCategoriesInfo> userCategoriesInfos;

        // Refreshing status
        public bool isRefreshingList;


        #endregion

        #region Properties

        public string ActiveUsernameLabel
        {
            get { return activeUsername; }
            set { SetValue(ref this.activeUsername, value); }
        }

        public string ActiveEmailLabel
        {
            get { return activeEmail; }
            set { SetValue(ref this.activeEmail, value); }
        }

        public object UserCategoriesInfosList
        {
            get { return userCategoriesInfos; }
            set { SetValue(ref this.userCategoriesInfos, (List<UserCategoriesInfo>)value); }
        }

        public int NoCategoryLabel
        {
            get { return noCategory; }
            set { SetValue(ref this.noCategory, value); }
        }
        public string NoCategoryAmountLabel
        {
            get { return noCategoryAmount; }
            set { SetValue(ref this.noCategoryAmount, value); }
        }
        public bool IsRefreshingListBool
        {
            get { return isRefreshingList; }
            set { SetValue(ref this.isRefreshingList, value); }
        }

        #endregion

        #region Commands

        public ICommand RefreshListCommand
        {
            get
            {
                return new RelayCommand(Refresh);
            }
        }

        public ICommand NewCategoryPopupCommand
        {
            get
            {
                return new RelayCommand(OpenNewCategoryPopup);
            }
        }

        public ICommand EditCategoryPopupCommand
        {
            get
            {
                return new RelayCommand<UserCategoriesInfo>(OpenEditCategoryPopup);
            }
        }

        public ICommand DeleteCategoryPopupCommand
        {
            get
            {
                return new RelayCommand<UserCategoriesInfo>(OpenDeleteCategoryPopup);
            }
        }

        #endregion

        #region Methods

        public void Refresh()
        {
            LoadUserCategories();
            GetNoCategory();
        }

        /**
         * Loads all the categories associated with the user
         * and saves them into the list
         */
        public async void LoadUserCategories()
        {
            // Get all the categories associated with the user

            string sql = "SELECT * FROM CategoryModel";
            sql += " WHERE FkUser = '" + App.ActiveUser.Id.ToString() + "'";

            List<CategoryModel> categories = await App.Database.Query<CategoryModel>(sql);

            List<int> records = new List<int>();
            List<float> amounts = new List<float>();

            // Gets the details of the category's associated expenses
            foreach (CategoryModel category in categories)
            {
                string sql2 = "SELECT * FROM ExpenseModel";
                sql2 += $" WHERE FkCategory = '{category.Id}'";
                sql2 += $" AND FkUser = '{App.ActiveUser.Id}'";

                List<ExpenseModel> expenses = await App.Database.Query<ExpenseModel>(sql2);

                records.Add(expenses.Count);

                float amount = 0;
                foreach (ExpenseModel expense in expenses)
                {
                    amount += expense.Amount;
                }

                amounts.Add(amount);
            }

            // Adds all the info into the list

            List<UserCategoriesInfo> infosList = new List<UserCategoriesInfo>();
            for (int i = 0; i < categories.Count; i++)
            {
                infosList.Add(new UserCategoriesInfo(categories[i], records[i], amounts[i].ToString("N2")));
            }

            UserCategoriesInfosList = infosList;

            IsRefreshingListBool = false;
        }

        // Gets the expenses without a category
        public async void GetNoCategory()
        {
            string sql = $"SELECT * FROM ExpenseModel";
            sql += $" WHERE FkCategory = '-1'";
            sql += $" AND FkUser = '{App.ActiveUser.Id}'";

            var res = await App.Database.Query<ExpenseModel>(sql);

            NoCategoryLabel = res.Count;

            float amount = 0;

            foreach(ExpenseModel expense in res)
            {
                amount += expense.Amount;
            }

            NoCategoryAmountLabel = amount.ToString("N2");
        }

        public void OpenNewCategoryPopup()
        {
            PopupNavigation.Instance.PushAsync(new NewCategoryPopup());
        }

        public void OpenEditCategoryPopup(UserCategoriesInfo categoryInfo)
        {
            PopupNavigation.Instance.PushAsync(new EditCategoryPopup(categoryInfo));
        }

        private void OpenDeleteCategoryPopup(UserCategoriesInfo categoryInfo)
        {
            PopupNavigation.Instance.PushAsync(new DeleteCategoryPopup(categoryInfo));
        }

        #endregion

        public ConfigViewModel()
        {
            Refresh();

            ActiveUsernameLabel = App.ActiveUser.Username;
            ActiveEmailLabel = App.ActiveUser.Email;
        }

    }
}
