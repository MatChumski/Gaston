using GalaSoft.MvvmLight.Command;
using Gastón.Models;
using Gastón.Views.PopUps;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Gastón.ViewModels
{
    public class ExpenseListViewModel : BaseViewModel
    {

        #region Attributes

        public DateTime startDate;
        public DateTime endDate;

        public string selectedCategory;
        public string selectedSort;
        public bool ascending;

        public string totalAmount;

        public List<string> categories;
        public List<ExpenseCard> expenses;

        public bool isRefreshing;

        public class ExpenseCard
        {
            public ExpenseModel Expense { get; set; }
            public string Amount { get; set; }
            public string Category { get; set; }

            public ExpenseCard(ExpenseModel expense, string amount, string category)
            {
                this.Expense = expense;
                this.Amount = amount;
                this.Category = category;
            }
        }

        #endregion

        #region Properties

        public DateTime StartDateEntry
        {
            get { return startDate; }
            set { SetValue(ref this.startDate, value); }
        }

        public DateTime EndDateEntry
        {
            get { return endDate; }
            set { SetValue(ref this.endDate, value); }
        }

        public string SelectedCategoryEntry
        {
            get { return selectedCategory; }
            set { SetValue(ref this.selectedCategory, value); }
        }

        public string SelectedSortEntry
        {
            get { return selectedSort; }
            set { SetValue(ref this.selectedSort, value); }
        }

        public string TotalAmountLabel
        {
            get { return totalAmount; }
            set { SetValue(ref this.totalAmount, value); }
        }

        public List<string> CategoriesList
        {
            get { return categories; }
            set { SetValue(ref this.categories, value); }
        }

        public List<ExpenseCard> ExpensesList
        {
            get { return expenses; }
            set { SetValue(ref this.expenses, value); }
        }

        public bool AscendingSwitch
        {
            get { return ascending; }
            set { SetValue(ref this.ascending, value); }
        }

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }

        #endregion

        #region Commands

        public ICommand EditExpensePopupCommand
        {
            get
            {
                return new RelayCommand<ExpenseCard>(OpenEditExpensePopup);
            }
        }

        public ICommand DeleteExpensePopupCommand
        {
            get
            {
                return new RelayCommand<ExpenseCard>(OpenDeleteExpensePopup);
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(Refresh);
            }
        }

        #endregion

        #region Methods

        public void Refresh()
        {
            IsRefreshing = true;

            LoadExpenses();
            GetCategories();

            IsRefreshing = false;
        }

        public async void LoadExpenses()
        {

            string order;

            if (AscendingSwitch)
            {
                order = "ASC";
            }
            else
            {
                order = "DESC";
            }

            string sql = $"SELECT * FROM ExpenseModel";
            sql += $" WHERE FkUser = '{App.ActiveUser.Id.ToString()}'";
            //sql += $" AND Date >= '{StartDateEntry.Date.ToString("yyyy-MM-dd")}'";
            //sql += $" AND Date <= '{EndDateEntry.Date.ToString("yyyy-MM-dd")}'";

            // By Category
            if (SelectedCategoryEntry != "All")
            {
                int catId;

                if (SelectedCategoryEntry != "No Category")
                {
                    string sqlCategory = $"SELECT * FROM CategoryModel WHERE Name = '{SelectedCategoryEntry}'";

                    var resCat = await App.Database.Query<CategoryModel>(sqlCategory);

                    catId = resCat[0].Id;

                } else
                {
                    catId = -1;
                }

                sql += $" AND FkCategory = '{catId}'";

            }

            // Sorting type
            if (SelectedSortEntry == "Date")
            {
                sql += $" ORDER BY Date {order}";
            }
            else if (SelectedSortEntry == "Amount")
            {
                sql += $" ORDER BY Amount {order}";
            }

            var res = await App.Database.Query<ExpenseModel>(sql);

            float newTotal = 0;

            List<ExpenseCard> newCardList = new List<ExpenseCard>();

            if (res.Count > 0)
            {
                foreach (ExpenseModel expense in res)
                {
                    if (DateTime.Compare(expense.Date.Date, StartDateEntry.Date) >= 0 && DateTime.Compare(expense.Date.Date, EndDateEntry.Date) <= 0)
                    {
                        string categoryName;

                        if (expense.FkCategory != -1)
                        {
                            string sqlCat = $"SELECT * FROM CategoryModel WHERE Id = {expense.FkCategory}";

                            var expenseCatRes = await App.Database.Query<CategoryModel>(sqlCat);

                            categoryName = expenseCatRes[0].Name;
                        }
                        else
                        {
                            categoryName = "No Category";
                        }

                        ExpenseCard newCard = new ExpenseCard(expense, expense.Amount.ToString("N2"), categoryName);
                        newCardList.Add(newCard);

                        newTotal += expense.Amount;

                    }
                }
            }


            ExpensesList = newCardList;

            TotalAmountLabel = newTotal.ToString("N2");

        }

        public async void GetCategories()
        {
            string lastCategory = SelectedCategoryEntry;

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

            SelectedCategoryEntry = lastCategory;
        }

        public void OpenEditExpensePopup(ExpenseCard expenseInfo)
        {
            PopupNavigation.Instance.PushAsync(new EditExpensePopup(expenseInfo));
        }

        public void OpenDeleteExpensePopup(ExpenseCard expenseInfo)
        {
            PopupNavigation.Instance.PushAsync(new DeleteExpensePopup(expenseInfo));
        }

        #endregion

        public ExpenseListViewModel()
        {
            EndDateEntry = DateTime.Now.Date;
            StartDateEntry = EndDateEntry.AddMonths(-1).Date;

            SelectedCategoryEntry = "All";
            SelectedSortEntry = "Date";
            AscendingSwitch = false;

            Refresh();
        }

    }
}
