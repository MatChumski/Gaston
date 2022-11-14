using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gastón.Models
{
    internal class ExpenseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // ID of the creator of the expense
        public string FkUser { get; set; }
    
        // ID of the category the expense belongs to
        public string FkCategory { get; set; }

        // How much
        public float Amount { get; set; }

        // What
        public string Description { get; set; }

        // Cash, Transference
        public string Type { get; set; }

        // When
        public DateTime Date { get; set; }
        
        // Creation of the registry
        public DateTime DateCreation { get; set; }
    }
}
