using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gastón.Models
{
    public class CategoryModel
    {
        // ID
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Name
        [MaxLength(100)]
        public string Name { get; set; }

        // Creation date
        public DateTime CreationDate { get; set; }

        // User creator
        public int FkUser { get; set; }
    }
}
