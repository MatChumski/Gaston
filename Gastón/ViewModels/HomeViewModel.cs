using GalaSoft.MvvmLight.Command;
using Gastón.Models;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Gastón.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {

        #region Attributes

        public LineChart lineChart;

        public string totalAmount;
        public DateTime startDate;
        public DateTime endDate;
        public string grandTotal;
        public string lastExpenseDate;

        public string dailyAverage;

        // Class to set the total details for each category
        public class CategoryInfo
        {
            public CategoryModel Category { get; set; }
            public string CategoryAmount { get; set; }

            public string CategoryRecords { get; set; }

            public CategoryInfo (CategoryModel category, string categoryAmount, string categoryRecords)
            {
                Category = category;
                CategoryAmount = categoryAmount;
                CategoryRecords = categoryRecords;
            }
        }

        public List<CategoryInfo> categories;

        public List<ExpenseModel> expenses;

        public bool isRefreshing;

        #endregion

        #region Properties

        public LineChart LineChartView
        {
            get { return lineChart; }
            set { SetValue(ref this.lineChart, value); }
        }

        public string TotalAmountLabel
        {
            get { return totalAmount; }
            set { SetValue(ref this.totalAmount, value); }
        }

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

        public string GrandTotalLabel
        {
            get { return grandTotal; }
            set { SetValue(ref this.grandTotal, value); }
        }

        public string LastExpenseDateLabel
        {
            get { return lastExpenseDate; }
            set { SetValue(ref this.lastExpenseDate, value); }
        }

        public string DailyAverageLabel
        {
            get { return dailyAverage; }
            set { SetValue(ref this.dailyAverage, value); }
        }

        public List<CategoryInfo> CategoriesList
        {
            get { return categories; }
            set { SetValue(ref this.categories, value); }
        }

        public List<ExpenseModel> ExpensesList
        {
            get { return expenses; }
            set { SetValue(ref this.expenses, value); }
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

        #endregion

        #region Methods

        // Reload the data according to the new parameters
        public async void Refresh()
        {
            IsRefreshing = true;

            await GetExpenses();
            await GetCategories();
            LoadChart();

            IsRefreshing = false;
        }

        // Class to build the data for the chart
        public class DateAmount
        {
            public string Date { get; set; }
            public float Amount { get; set; }

            public DateAmount(string date, float amount)
            {
                Date = date;
                Amount = amount;
            }
        }

        public void LoadChart()
        {
            List<Microcharts.ChartEntry> chartEntries = new List<Microcharts.ChartEntry>();

            List<string> allDates = new List<string>();

            // Gets all the dates from the expense list
            foreach(ExpenseModel expense in ExpensesList)
            {
                allDates.Add(expense.Date.ToString("d"));
            }

            // Gets all the different dates from the previous list
            List<string> groupDates = allDates.Distinct().ToList();

            List<float> amountPerDates = new List<float>();

            // Checks the individual dates, and adds to them the values
            // of the corresponding expenses
            foreach(string date in groupDates)
            {
                float amount = 0;
                foreach (ExpenseModel expense in ExpensesList)
                {
                    if (expense.Date.ToString("d") == date)
                    {
                        amount += expense.Amount;
                    }
                }
                amountPerDates.Add(amount);
            }

            // Creates the data list with all the dates and their amounts
            List<DateAmount> dateAmountList = new List<DateAmount>();

            for (int i = 0; i < groupDates.Count; i++)
            {
                dateAmountList.Add(new DateAmount(groupDates[i], amountPerDates[i]));
            }

            // Reverses the list to show the latest values on the right side
            // of the chart
            List<DateAmount> reverseList = Enumerable.Reverse(dateAmountList).ToList();

            // Adds all the data to the chart
            foreach (DateAmount dateAmount in reverseList)
            {
                chartEntries.Add(
                    new Microcharts.ChartEntry(dateAmount.Amount)
                    {
                        Color = SKColor.Parse("#7AA7E6"),
                        Label = dateAmount.Date,
                        ValueLabel = dateAmount.Amount.ToString("C2"),
                        TextColor = SKColor.Parse("#FFFFFF"),
                        ValueLabelColor = SKColor.Parse("#FFFFFF")
                    }
                    );
            }

            LineChart newChart = new LineChart { Entries = chartEntries };

            newChart.BackgroundColor = SKColor.Parse("#2F4159");
            newChart.LineMode = LineMode.Straight;
            newChart.LabelColor = SKColor.Parse("#FFFFFF");
            newChart.LabelTextSize = 20;
            newChart.LabelOrientation = Orientation.Horizontal;

            LineChartView = newChart;
        }

        public async Task GetExpenses()
        {
            string sql = $"SELECT * FROM ExpenseModel";
            sql += $" WHERE FkUser = {App.ActiveUser.Id}";
            sql += " ORDER BY Date DESC";

            var resExpenses = await App.Database.Query<ExpenseModel>(sql);

            float newTotalAmount = 0;
            float newGrandTotal = 0;

            float totalExpenses = 0;

            List<ExpenseModel> newExpensesList = new List<ExpenseModel>();

            if (resExpenses.Count > 0)
            {
                foreach(ExpenseModel expense in resExpenses)
                {
                    newGrandTotal += expense.Amount;

                    if (DateTime.Compare(expense.Date.Date, StartDateEntry.Date) >= 0 && DateTime.Compare(expense.Date.Date, EndDateEntry.Date) <= 0)
                    {
                        newTotalAmount += expense.Amount;
                        totalExpenses++;

                        newExpensesList.Add(expense);
                    }
                }
            }

            TotalAmountLabel = newTotalAmount.ToString("N2");
            GrandTotalLabel = newGrandTotal.ToString("N2");

            LastExpenseDateLabel = resExpenses[0].Date.ToString("d");
            
            DailyAverageLabel = (newTotalAmount / (EndDateEntry - StartDateEntry).Days).ToString("N2");

            ExpensesList = newExpensesList;
        }

        public async Task GetCategories()
        {
            // Gets all the category from the expenses
            List<int> allCategoriesId = new List<int>();

            foreach (ExpenseModel expense in ExpensesList)
            {
                allCategoriesId.Add(expense.FkCategory);
            }

            // Gets each distinct category
            List<int> categoriesId = allCategoriesId.Distinct().ToList();

            List<CategoryModel> categoryModels = new List<CategoryModel>();

            foreach (int categoryId in categoriesId)
            {
                string sql = $"SELECT * FROM CategoryModel WHERE Id = {categoryId}";

                var catRes = await App.Database.Query<CategoryModel>(sql);

                if (catRes.Count > 0)
                {
                    categoryModels.Add(catRes[0]);
                }
            }

            // Gets the data that will be put on the CategoryInfo list
            List<string> amounts = new List<string>();
            List<int> records = new List<int>();

            foreach (CategoryModel category in categoryModels)
            {
                int sumRecords = 0;
                float sumAmounts = 0;
                foreach (ExpenseModel expense in ExpensesList)
                {
                    if (expense.FkCategory == category.Id)
                    {
                        sumAmounts += expense.Amount;
                        sumRecords++;
                    }
                }
                amounts.Add(sumAmounts.ToString("N2"));
                records.Add(sumRecords);
            }

            // Sets the data on the CategoryInfo list
            List<CategoryInfo> newCategoryInfoList = new List<CategoryInfo>();
            for (int i = 0; i < categoryModels.Count; i++)
            {
                newCategoryInfoList.Add(new CategoryInfo(categoryModels[i], amounts[i], records[i].ToString()));
            }

            CategoriesList = newCategoryInfoList;


        }

        #endregion

        public HomeViewModel()
        {
            EndDateEntry = DateTime.Now.Date;
            StartDateEntry = EndDateEntry.AddMonths(-1);

            Refresh();
        }

    }
}
