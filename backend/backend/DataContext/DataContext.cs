using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace backend.DataContext
{
    public class SqlDataContext
    {
        private readonly IConfiguration _config;

        public SqlDataContext(IConfiguration config) 
        {
           _config = config;
        }


        public T? LoadDataSingleWithParameters<T>(string sql, List<SqlParameter>? parameters, Func<SqlDataReader, T> mapper)
        {
            SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SqlCommand commandWithParameters = new SqlCommand(sql, dbConnection);

            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    commandWithParameters.Parameters.Add(parameter);
                }
            }

            dbConnection.Open();

            SqlDataReader reader = commandWithParameters.ExecuteReader();

            T? result = default;

            if(reader.Read())
            {
                result = mapper(reader);
            }

            reader.Close();
            dbConnection.Close();

            return result;
        }

        public List<T> LoadDataWithParameters<T>(string sql, List<SqlParameter>? parameters, Func<SqlDataReader, T> mapper)
        {
            SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SqlCommand commandWithParameters = new SqlCommand(sql, dbConnection);

            if(parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    commandWithParameters.Parameters.Add(parameter);
                }
            }

            dbConnection.Open();

            SqlDataReader reader = commandWithParameters.ExecuteReader();

            List<T> results = new List<T>();

            while (reader.Read())
            {
                results.Add(mapper(reader));
            }

            reader.Close();
            dbConnection.Close();
            
            return results;
        }

        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter>? parameters)
        {
            SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            SqlCommand commandWithParameters = new SqlCommand(sql,dbConnection);

            if(parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    commandWithParameters.Parameters.Add(parameter);
                }
            }

            dbConnection.Open();

            int rowsAffected = commandWithParameters.ExecuteNonQuery();

            dbConnection.Close();

            return rowsAffected > 0;

        }


    }
}
