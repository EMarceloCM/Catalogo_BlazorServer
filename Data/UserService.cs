using BlazorAdmin.Data.Models;
using BlazorAdmin.Data.Services;
using Microsoft.Data.SqlClient;

namespace BlazorAdmin.Data
{
    public class UserService : IUserService
    {
        private readonly SqlConnectionConfiguration _configuration;
        public UserService(SqlConnectionConfiguration config)
        {
            _configuration = config;
        }

        public async Task<List<User>> GetUsers()
        {
            try
            {
                List<User> users = new();
                using (SqlConnection con = new SqlConnection(_configuration.ConnectionString))
                {
                    const string query = "SELECT * FROM dbo.AspNetUsers";
                    SqlCommand cmd = new SqlCommand(query, con)
                    {
                        CommandType = System.Data.CommandType.Text
                    };

                    con.Open();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (rdr.Read())
                    {
                        User usuario = new()
                        {
                            Id = Guid.Parse(rdr["Id"].ToString()!),
                            UserName = rdr["UserName"].ToString(),
                            Email = rdr["Email"].ToString(),
                            RoleId = new Guid()
                        };
                        users.Add(usuario);
                    }
                    cmd.Dispose();
                }
                return users;
            }
            catch (SqlException)
            {

                throw;
            }
        }
        public async Task<User> GetUser(Guid id)
        {
            try
            {
                User user = new();
                using (SqlConnection con = new SqlConnection(_configuration.ConnectionString))
                {
                    const string query = "SELECT * FROM dbo.AspNetUsers WHERE Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.AddWithValue("@Id", id);
                        con.Open();   
                    
                        using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        {
                            if (rdr.Read())
                            {
                                user.Id = Guid.Parse(rdr["Id"].ToString()!);
                                user.UserName = rdr["UserName"].ToString();
                                user.Email = rdr["Email"].ToString();
                            }
                        }
                    }
                }
                return user;
            }
            catch (SqlException)
            {

                throw;
            }
        }
        public async Task<bool> DeleteUser(Guid id)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.ConnectionString))
                {
                    const string query = "DELETE FROM dbo.AspNetUsers WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, con)
                    {
                        CommandType = System.Data.CommandType.Text
                    };
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    _ = await cmd.ExecuteNonQueryAsync();
                    con.Close();
                    cmd.Dispose();
                }
                return true;
            }
            catch (SqlException)
            {

                throw;
            }
        }
        public async Task<bool> UpdateUserRole(Guid id, User user)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.ConnectionString))
                {
                    const string query = "INSERT INTO dbo.AspNetUserRoles " + "(UserId,RoleId) values(@UserId, @RoleId)";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        cmd.Parameters.AddWithValue("@UserId", id);
                        cmd.Parameters.AddWithValue("@RoleId", user.RoleId);

                        con.Open();
                        _ = await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (SqlException)
            {

                throw;
            }
        }
    }
}