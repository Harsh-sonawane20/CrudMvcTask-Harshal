using CrudMvcTask.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace CrudMvcTask.Controllers
{
    public class ProductController : Controller
    {
        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            var startRow = (page - 1) * pageSize + 1;
            var endRow = page * pageSize;

            // Query to get paginated products with row numbers
            var query = @"WITH PaginatedProducts AS (
            SELECT 
                p.ProductId, 
                p.ProductName, 
                p.CategoryId, 
                c.CategoryName,
                ROW_NUMBER() OVER (ORDER BY p.ProductId) AS RowNum
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.CategoryId
        )
        SELECT ProductId, ProductName, CategoryId, CategoryName
        FROM PaginatedProducts
        WHERE RowNum BETWEEN @StartRow AND @EndRow
        ORDER BY RowNum";

            // Execute query
            var dt = DbHelper.ExecuteQuery(query, new SqlParameter[] {
        new SqlParameter("@StartRow", startRow),
        new SqlParameter("@EndRow", endRow)
    });

            var products = new List<Product>();
            foreach (DataRow row in dt.Rows)
            {
                products.Add(new Product
                {
                    ProductId = (int)row["ProductId"],
                    ProductName = row["ProductName"].ToString(),
                    CategoryId = (int)row["CategoryId"],
                    CategoryName = row["CategoryName"].ToString()
                });
            }

            // Query to get total number of products
            var countQuery = @"SELECT COUNT(*) FROM Products";
            var totalProductCount = (int)DbHelper.ExecuteScalar(countQuery);

            // Calculate total pages
            var totalPages = (int)Math.Ceiling((double)totalProductCount / pageSize);

            // Create the ViewModel to pass to the view
            var model = new ProductListViewModel
            {
                Products = products,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(model);
        }



        public ActionResult Create()
        {
            // Populate Categories for Dropdown
            ViewBag.Categories = new SelectList(GetCategories(), "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            // Re-populate dropdown in case of error
            ViewBag.Categories = new SelectList(GetCategories(), "CategoryId", "CategoryName");

           
                try
                {
                       DbHelper.ExecuteNonQuery(@"INSERT INTO Products (ProductName, CategoryId)
                                      VALUES (@ProductName, @CategoryId)",

                    new SqlParameter[]
                        {
                    new SqlParameter("@ProductName", product.ProductName),
                    new SqlParameter("@CategoryId", product.CategoryId)
                        });

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error creating product: " + ex.Message);
                }

            return View(product);
        }



        public ActionResult Edit(int id)
        {
            var dt = DbHelper.ExecuteQuery("SELECT * FROM Products WHERE ProductId = @ProductId",
                new SqlParameter[] { new SqlParameter("@ProductId", id) });

            if (dt.Rows.Count == 0) return HttpNotFound();

            var product = new Product
            {
                ProductId = (int)dt.Rows[0]["ProductId"],
                ProductName = dt.Rows[0]["ProductName"].ToString(),
                CategoryId = (int)dt.Rows[0]["CategoryId"]
            };

            ViewBag.Categories = GetCategories();

            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Product product)
        {
            DbHelper.ExecuteNonQuery("UPDATE Products SET ProductName = @ProductName, CategoryId = @CategoryId WHERE ProductId = @ProductId",
                new SqlParameter[]
                {
                    new SqlParameter("@ProductName", product.ProductName),
                    new SqlParameter("@CategoryId", product.CategoryId),
                    new SqlParameter("@ProductId", product.ProductId)
                });

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            DbHelper.ExecuteNonQuery("DELETE FROM Products WHERE ProductId = @ProductId",
                new SqlParameter[] { new SqlParameter("@ProductId", id) });

            return RedirectToAction("Index");
        }

        private List<Category> GetCategories()
        {
            var categories = new List<Category>();
            var dt = DbHelper.ExecuteQuery("SELECT * FROM Categories");

            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new Category
                {
                    CategoryId = (int)row["CategoryId"],
                    CategoryName = row["CategoryName"].ToString()
                });
            }

            return categories;
        }

    }
}
