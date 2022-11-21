using GalaSoft.MvvmLight.Command;
using Gastón.Models;
using Gastón.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Gastón.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        #region Attributes

        public string userID;
        public string password;

        #endregion

        #region Properties

        public string UserIdEntry
        {
            get { return userID; }
            set { SetValue(ref this.userID, value.Trim()); }
        }

        public string PasswordEntry
        {
            get { return password; }
            set { SetValue(ref this.password, value.Trim()); }
        }

        #endregion

        #region Commands

        public ICommand LoginCommand
        {
            get
            {
                return new RelayCommand(LoginMethod);
            }
        }

        public ICommand NavToSignupCommand
        {
            get
            {
                return new RelayCommand(NavToSignupMethod);
            }
        }

        #endregion

        #region Methods

        private async void LoginMethod()
        {
            if (string.IsNullOrEmpty(UserIdEntry) || string.IsNullOrEmpty(PasswordEntry))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Fields cannot be empty", "Ok");
                return;
            }

            string query = $"SELECT * FROM UserModel";
            query += $" WHERE (username = '{UserIdEntry}' OR email = '{UserIdEntry}') AND password = '{PasswordEntry}'";

            var foundUsers = await App.Database.Query<UserModel>(query);

            if (foundUsers.Count <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Wrong Credentials", "Ok");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Login", "Successful Login", "Ok");

            App.ActiveUser = foundUsers[0];
            var last = Application.Current.MainPage.Navigation.NavigationStack.Last();
            Application.Current.MainPage.Navigation.InsertPageBefore(new MainView(), last);
            await Application.Current.MainPage.Navigation.PopAsync();
        }


        private async void NavToSignupMethod()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new Signup(), true);
        }
        #endregion
    }
}
