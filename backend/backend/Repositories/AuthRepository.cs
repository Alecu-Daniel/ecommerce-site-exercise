using backend.DataContext;
using backend.Dtos;
using Microsoft.Data.SqlClient;
using System.Data;

namespace backend.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly SqlDataContext _data;

        public AuthRepository(SqlDataContext data)
        {
            _data = data;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            const string sql = "SELECT Email FROM Auth WHERE Email = @Email";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email }
            };

            var result = await _data.LoadDataWithParametersAsync<string>(sql, parameters, reader => reader["Email"].ToString() ?? "");
            return result.Any();
        }

        public async Task<bool> RegisterUserAsync(UserForRegistrationDto userForRegistration, byte[] passwordHash, byte[] passwordSalt)
        {
            const string sql = @"
                INSERT INTO Auth ([Email], [PasswordHash], [PasswordSalt])
                VALUES (@Email, @PasswordHash, @PasswordSalt);

                INSERT INTO Users ([Email]) 
                VALUES (@Email);";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", SqlDbType.NVarChar) { Value = userForRegistration.Email },
                new SqlParameter("@PasswordHash", SqlDbType.VarBinary) { Value = passwordHash },
                new SqlParameter("@PasswordSalt", SqlDbType.VarBinary) { Value = passwordSalt }
            };

            return await _data.ExecuteSqlWithParametersAsync(sql, parameters);
        }

        public async Task<UserForLoginConfirmationDto?> GetUserCredentialsAsync(string email)
        {
            const string sql = "SELECT [PasswordHash], [PasswordSalt] FROM Auth WHERE Email = @Email";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email }
            };

            return await _data.LoadDataSingleWithParametersAsync(sql, parameters, reader => new UserForLoginConfirmationDto
            {
                PasswordHash = (byte[])reader["PasswordHash"],
                PasswordSalt = (byte[])reader["PasswordSalt"]
            });
        }

        public async Task<int> GetUserIdByEmailAsync(string email)
        {
            const string sql = "SELECT UserId FROM Users WHERE Email = @Email";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", SqlDbType.NVarChar) { Value = email }
            };

            return await _data.LoadDataSingleWithParametersAsync(sql, parameters, reader => (int)reader["UserId"]);
        }

    }
}
