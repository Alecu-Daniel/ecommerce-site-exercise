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
            using SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using SqlCommand commandWithParameters = new SqlCommand(sql, dbConnection);

            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    commandWithParameters.Parameters.Add(parameter);
                }
            }

            dbConnection.Open();

            using SqlDataReader reader = commandWithParameters.ExecuteReader();

            T? result = default;

            if(reader.Read())
            {
                result = mapper(reader);
            }

            return result;
        }

        public List<T> LoadDataWithParameters<T>(string sql, List<SqlParameter>? parameters, Func<SqlDataReader, T> mapper)
        {
            using SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using SqlCommand commandWithParameters = new SqlCommand(sql, dbConnection);

            if(parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    commandWithParameters.Parameters.Add(parameter);
                }
            }

            dbConnection.Open();

            using SqlDataReader reader = commandWithParameters.ExecuteReader();

            List<T> results = new List<T>();

            while (reader.Read())
            {
                results.Add(mapper(reader));
            }
            
            return results;
        }

        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter>? parameters)
        {
            using SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            using SqlCommand commandWithParameters = new SqlCommand(sql,dbConnection);

            if(parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    commandWithParameters.Parameters.Add(parameter);
                }
            }

            dbConnection.Open();

            int rowsAffected = commandWithParameters.ExecuteNonQuery();

            return rowsAffected > 0;

        }


    }
}
