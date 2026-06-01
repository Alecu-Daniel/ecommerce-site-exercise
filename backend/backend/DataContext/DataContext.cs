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


        public async Task<T?> LoadDataSingleWithParametersAsync<T>(string sql, List<SqlParameter>? parameters, Func<SqlDataReader, T> mapper)
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

            await dbConnection.OpenAsync();

            using SqlDataReader reader = await commandWithParameters.ExecuteReaderAsync();

            T? result = default;

            if(await reader.ReadAsync())
            {
                result = mapper(reader);
            }

            return result;
        }

        public async Task<List<T>> LoadDataWithParametersAsync<T>(string sql, List<SqlParameter>? parameters, Func<SqlDataReader, T> mapper)
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

            await dbConnection.OpenAsync();

            using SqlDataReader reader = await commandWithParameters.ExecuteReaderAsync();

            List<T> results = new List<T>();

            while (await reader.ReadAsync())
            {
                results.Add(mapper(reader));
            }
            
            return results;
        }

        public async Task<bool> ExecuteSqlWithParametersAsync(string sql, List<SqlParameter>? parameters)
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

            await dbConnection.OpenAsync();

            int rowsAffected = await commandWithParameters.ExecuteNonQueryAsync();

            return rowsAffected > 0;

        }


    }
}
