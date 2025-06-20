﻿using System.Text.Json.Serialization;

namespace Classifly_API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;


        public ICollection<Item> Items { get; set; }
    }
}
