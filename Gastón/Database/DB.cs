using Gastón.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gastón.Database
{
    public class DB
    {
        private SQLiteAsyncConnection connection;

        public DB(string databasePath)
        {
            connection = new SQLiteAsyncConnection(databasePath);

            connection.CreateTableAsync<UserModel>().Wait();
            connection.CreateTableAsync<ExpenseModel>().Wait();
            connection.CreateTableAsync<CategoryModel>().Wait();
        }

        public SQLiteAsyncConnection getConnection()
        {
            return connection;
        }

        #region CRUD

        // QUERY
        public Task<List<T>> Query<T>(string query) where T : new()
        {
            return connection.QueryAsync<T>(query);
        }

        // SELECT
        public Task<List<T>> GetTableModel<T>() where T : new()
        {
            return connection.Table<T>().ToListAsync();
        }

        // CREATE | UPDATE
        public Task<int> SaveTableModel<T>(T model, string action = "insert") where T : new()
        {
            if (action == "update")
            {
                return connection.UpdateAsync(model);
            }
            
            return connection.InsertAsync(model);
        }

        // DELETE
        public Task<int> DeleteTableModel<T>(T model) where T : new()
        {
            return connection.DeleteAsync(model);
        }
        

        

        #endregion
    }
}
