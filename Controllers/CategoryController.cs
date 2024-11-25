using CrudMvcTask.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace CrudMvcTask.Controllers
{
    public class CategoryController : Controller
    {
        public ActionResult Index(string searchTerm = "")
        {
            var categories = new List<Category>();

            // Adjust the query to support search filtering
            var query = "SELECT * FROM Categories";

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += " WHERE CategoryName LIKE @SearchTerm OR CategoryId LIKE @SearchTerm";
            }

            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                parameters.Add(new SqlParameter("@SearchTerm", "%" + searchTerm + "%"));
            }

            var dt = DbHelper.ExecuteQuery(query, parameters.ToArray());

            foreach (DataRow row in dt.Rows)
            {
                categories.Add(new Category
                {
                    CategoryId = (int)row["CategoryId"],
                    CategoryName = row["CategoryName"].ToString()
                });
            }

            return View(categories);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            DbHelper.ExecuteNonQuery("INSERT INTO Categories (CategoryName) VALUES (@CategoryName)",
                new SqlParameter[] { new SqlParameter("@CategoryName", category.CategoryName) });

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var dt = DbHelper.ExecuteQuery("SELECT * FROM Categories WHERE CategoryId = @CategoryId",
                new SqlParameter[] { new SqlParameter("@CategoryId", id) });

            if (dt.Rows.Count == 0) return HttpNotFound();

            var category = new Category
            {
                CategoryId = (int)dt.Rows[0]["CategoryId"],
                CategoryName = dt.Rows[0]["CategoryName"].ToString()
            };

            return View(category);
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        {
            DbHelper.ExecuteNonQuery("UPDATE Categories SET CategoryName = @CategoryName WHERE CategoryId = @CategoryId",
                new SqlParameter[] {
                    new SqlParameter("@CategoryName", category.CategoryName),
                    new SqlParameter("@CategoryId", category.CategoryId)
                });

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            DbHelper.ExecuteNonQuery("DELETE FROM Categories WHERE CategoryId = @CategoryId",
                new SqlParameter[] { new SqlParameter("@CategoryId", id) });

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var dt = DbHelper.ExecuteQuery("SELECT * FROM Categories WHERE CategoryId = @CategoryId",
                new SqlParameter[] { new SqlParameter("@CategoryId", id) });

            if (dt.Rows.Count == 0) return HttpNotFound();

            var category = new Category
            {
                CategoryId = (int)dt.Rows[0]["CategoryId"],
                CategoryName = dt.Rows[0]["CategoryName"].ToString()
            };

            return View(category);
        }

    }
}
