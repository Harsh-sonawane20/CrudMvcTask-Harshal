﻿using System.Collections.Generic;

namespace CrudMvcTask.Models
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
