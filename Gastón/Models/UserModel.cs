using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using SQLite;

namespace Gastón.Models
{
    internal class UserModel
    {
        // User ID
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Username
        [MaxLength(100)]
        public string Username { get; set; }

        // Password
        [MaxLength(255)]
        public string Password { get; set; }

        // Email
        [MaxLength(100)]
        public string Email { get; set; }

        // User, Admin
        public string Role { get; set; }

        // Creation of the account
        [NotNull]
        public DateTime CreationDate { get; set; }
    }
}
