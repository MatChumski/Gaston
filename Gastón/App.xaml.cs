using Gastón.Database;
using Gastón.Models;
using Gastón.Views;
using System;
using System.Diagnostics;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Gastón
{
    public partial class App : Application
    {

        static DB database;

        public static DB Database
        {
            get
            {
                if (database == null)
                {
                    string databasePath = Path.Combine(
                        Environment.GetFolderPath(
                            Environment.SpecialFolder.LocalApplicationData), "gastonDB3.db");
                    Debug.WriteLine(databasePath);
                    database = new DB(databasePath);
                }
                return database;
            }
        }
        public App()
        {
            //activeUser = new UserModel()
            //{
            //    Username = "admin",
            //    Password = "admin123",
            //    Email = "mateo0704@hotmail.com",
            //    Role = "admin",
            //    Id = '1',
            //    CreationDate = DateTime.Now
            //};

            InitializeComponent();

            MainPage = new NavigationPage(new Login());
            //MainPage = new NavigationPage(new MainView());
        }

        static UserModel activeUser;

        public static UserModel ActiveUser
        {
            get 
            { 
                return activeUser; 
            }

            set
            {
                activeUser = value;
            }
        }

        private async void CreateAdmin()
        {
            string query = $"SELECT * FROM UserModel WHERE email = 'mateo0704@hotmail.com'";
            var res = await Database.Query<UserModel>(query);

            if (res.Count <= 0)
            {
                UserModel admin = new UserModel();

                admin.Username = "admin";
                admin.Password = "admin123";
                admin.Email = "mateo0704@hotmail.com";
                admin.Role = "admin";
                admin.CreationDate = DateTime.Now;

                await Database.SaveTableModel(admin);
            }

            
        }

        protected override void OnStart()
        {
            CreateAdmin();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
