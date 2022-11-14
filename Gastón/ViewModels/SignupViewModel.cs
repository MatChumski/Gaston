using GalaSoft.MvvmLight.Command;
using Gastón.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Gastón.ViewModels
{
    internal class SignupViewModel : BaseViewModel
    {

        #region Attributes

        public string username;
        public string email;
        public string password;
        public string passwordRepeat;

        #endregion

        #region Properties

        public string UsernameEntry
        {
            get { return username; }
            set { SetValue(ref this.username, value); }
        }

        public string EmailEntry
        {
            get { return email; }
            set { SetValue(ref this.email, value); }
        }
        public string PasswordEntry
        {
            get { return password; }
            set { SetValue(ref this.password, value); }
        }
        public string PasswordRepeatEntry
        {
            get { return passwordRepeat; }
            set { SetValue(ref this.passwordRepeat, value); }
        }

        #endregion

        #region Commands

        public ICommand SignupCommand
        {
            get
            {
                return new RelayCommand(SignupMethod);
            }
        }

        #endregion

        #region Methods

        private async void SignupMethod()
        {
            // Check for empty fields
            if (string.IsNullOrEmpty(UsernameEntry) || 
                string.IsNullOrEmpty(EmailEntry) || 
                string.IsNullOrEmpty(PasswordEntry) || 
                string.IsNullOrEmpty(PasswordRepeatEntry))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Fields cannot be empty", "Ok");
                return;
            }

            // Check passwords matching
            if (PasswordEntry != PasswordRepeatEntry)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Passwords don't match", "Ok");
                return;
            }

            // Check if the E-Mail address is valid
            try
            {
                MailAddress m = new MailAddress(EmailEntry);
            }
            catch (FormatException)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid E-Mail address", "Ok");
                return;
            }

            string query = $"SELECT * FROM UserModel WHERE email = '{EmailEntry}'";

            var foundUsers = await App.Database.Query<UserModel>(query);

            if (foundUsers.Count > 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "E-Mail already registered", "Ok");
                return;
            }

            UserModel newUser = new UserModel();

            newUser.Username = UsernameEntry;
            newUser.Email = EmailEntry;
            newUser.Password = PasswordEntry;
            newUser.Role = "User";
            newUser.CreationDate = DateTime.Now;

            int rowsInserted = await App.Database.SaveTableModel(newUser, "create");

            if (rowsInserted <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "User could not be registered", "Ok");
                return;
            }

            await Application.Current.MainPage.DisplayAlert("Signup", "Account created succesfully", "Ok");
            //await Application.Current.MainPage.Navigation.PopAsync();
        }

        #endregion

    }
}
