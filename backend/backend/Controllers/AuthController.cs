using backend.DataContext;
using backend.Dtos;
using backend.Helpers;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Pipelines;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.RateLimiting;

namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly SqlDataContext _data;
        private readonly AuthHelper _authHelper;
        public AuthController(SqlDataContext data,AuthHelper authHelper)
        {
            _data = data;
            _authHelper = authHelper;
        }


        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = "SELECT * FROM Auth WHERE Email = @Email";

                List<SqlParameter> sqlCheckUserParameters = new List<SqlParameter>
                {
                    new SqlParameter("@Email",System.Data.SqlDbType.NVarChar) { Value = userForRegistration.Email}
                };


                IEnumerable<string> existingUsers = _data.LoadDataWithParameters<string>(sqlCheckUserExists, sqlCheckUserParameters, reader => reader["Email"].ToString() ?? "");

                if (existingUsers.Count() == 0)
                {
                    byte[] passwordSalt = new byte[128 / 8];

                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = @"INSERT INTO Auth ([Email],[PasswordHash],[PasswordSalt])
                                          VALUES (@Email, @PasswordHash, @PasswordSalt)";


                    List<SqlParameter> sqlAddAuthParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@Email",System.Data.SqlDbType.NVarChar) { Value = userForRegistration.Email},
                        new SqlParameter("@PasswordHash",System.Data.SqlDbType.VarBinary) { Value = passwordHash},
                        new SqlParameter("@PasswordSalt",System.Data.SqlDbType.VarBinary) { Value = passwordSalt}
                    };

                    if(_data.ExecuteSqlWithParameters(sqlAddAuth,sqlAddAuthParameters))
                    {
                        string sqlAddUser = @"INSERT INTO Users([Email]) VALUES (@Email)";

                        List<SqlParameter> sqlAddUserParameters = new List<SqlParameter>
                        {
                            new SqlParameter("@Email",System.Data.SqlDbType.NVarChar) { Value = userForRegistration.Email}
                        };


                        _data.ExecuteSqlWithParameters(sqlAddUser, sqlAddUserParameters);

                        return Ok();
                    }
                    throw new Exception("Failed to register user.");
                }
                throw new Exception("User with this Email already exists");

            }
            throw new Exception("Passwords do not match");
        }


        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"SELECT [PasswordHash],[PasswordSalt] FROM Auth WHERE Email = @Email";
            List<SqlParameter> sqlHashAndSaltParameters = new List<SqlParameter>
            {
                new SqlParameter("@Email",System.Data.SqlDbType.NVarChar) { Value = userForLogin.Email}
            };

            UserForLoginConfirmationDto? userForConfirmation = _data.LoadDataSingleWithParameters(sqlForHashAndSalt, sqlHashAndSaltParameters
                                                                , reader => new UserForLoginConfirmationDto
                                                                {
                                                                    PasswordHash = (byte[])reader["PasswordHash"],
                                                                    PasswordSalt = (byte[])reader["PasswordSalt"]
                                                                });

            if( userForConfirmation == null)
            {
                return Unauthorized("Incorrect Email or Password");
            }

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

            for(int i = 0; i < passwordHash.Length;i++)
            {
                if (passwordHash[i] != userForConfirmation.PasswordHash[i])
                {
                    throw new Exception("Incorrect Password");
                }
            }


            string sqlUserId = "SELECT UserId From Users WHERE Email = @Email";
            List<SqlParameter> sqlUserIdParameters = new List<SqlParameter>
            {
                new SqlParameter("@Email",System.Data.SqlDbType.NVarChar) { Value = userForLogin.Email}
            };

            int userId = _data.LoadDataSingleWithParameters<int>(sqlUserId, sqlUserIdParameters, reader => (int)reader["UserId"]);

            return Ok(new Dictionary<string,string>
            {
                {"token", _authHelper.CreateToken(userId) }
            });
        }


    }
}
