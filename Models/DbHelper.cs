using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CrudMvcTask.Models
{
    public static class DbHelper
    {
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["CrudMvcTaskDB"].ConnectionString;

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static DataTable GetPaginatedDataProduct(string query, int pageNumber, int pageSize, SqlParameter[] parameters = null)
        {
            // Ensure the query has a valid ORDER BY clause (if it's missing)
            if (!query.ToUpper().Contains("ORDER BY"))
            {
                throw new InvalidOperationException("The query must include an ORDER BY clause for pagination.");
            }

            // Add OFFSET and FETCH for pagination
            query += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            List<SqlParameter> paginatedParams = new List<SqlParameter>
            {
                new SqlParameter("@Offset", (pageNumber - 1) * pageSize), // Calculate the OFFSET value based on the page number
                new SqlParameter("@PageSize", pageSize) // The number of rows per page
            };

            // Add any additional parameters provided
            if (parameters != null)
            {
                paginatedParams.AddRange(parameters);
            }

            // Execute the query and return the paginated result
            return ExecuteQuery(query, paginatedParams.ToArray());
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                con.Open();
                return cmd.ExecuteScalar(); // Executes the query and returns the first column of the first row
            }
        }
    }
}
