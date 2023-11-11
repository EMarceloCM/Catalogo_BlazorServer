using BlazorAdmin.Data.Models;
using BlazorAdmin.Data.Services;
using Microsoft.Data.SqlClient;

namespace BlazorAdmin.Data
{
    public class RoleService : IRoleService
    {
        private readonly SqlConnectionConfiguration _configuration;
        public RoleService(SqlConnectionConfiguration config)
        {
            _configuration = config;
        }

        public async Task<bool> CreateRole(Role role)
        {
            if (!VerificaSeRoleExiste(role))
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(_configuration.ConnectionString))
                    {
                        const string query = "INSERT INTO dbo.AspNetRoles " + "(Id,Name,NormalizedName,ConcurrencyStamp) " + "VALUES(@Id,@Name,@NormalizedName,@ConcurrencyStamp)";
                        SqlCommand cmd = new SqlCommand(query, con)
                        {
                            CommandType = System.Data.CommandType.Text
                        };
                        cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@Name", role.Name);
                        cmd.Parameters.AddWithValue("@NormalizedName", role.Name!.ToUpper());
                        cmd.Parameters.AddWithValue("@ConcurrencyStamp", Guid.NewGuid());

                        con.Open();
                        await cmd.ExecuteNonQueryAsync();

                        con.Close();
                        cmd.Dispose();
                    }
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.StackTrace!.ToString());
                    throw;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteRole(Guid id)
        {
            try
            {
                using (SqlConnection con = new(_configuration.ConnectionString))
                {
                    const string query = "DELETE FROM dbo.AspNetRoles WHERE Id = @Id";
                    SqlCommand cmd = new(query, con)
                    {
                        CommandType = System.Data.CommandType.Text
                    };
                    cmd.Parameters.AddWithValue("@Id", id);

                    con.Open();
                    await cmd.ExecuteNonQueryAsync();

                    con.Close();
                    cmd.Dispose();
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> EditRole(Guid id, Role role)
        {
            if (!VerificaSeRoleExiste(role))
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(_configuration.ConnectionString))
                    {
                        const string query = "UPDATE dbo.AspNetRoles SET Name = @Name, NormalizedName = @NormalizedName, " +
                            "ConcurrencyStamp = @ConcurrencyStamp WHERE Id = @Id";
                        
                        SqlCommand cmd = new SqlCommand(query, con)
                        {
                            CommandType = System.Data.CommandType.Text
                        };
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Name", role.Name);
                        cmd.Parameters.AddWithValue("@NormalizedName", role.Name!.ToUpper());
                        cmd.Parameters.AddWithValue("@ConcurrencyStamp", role.ConcurrencyStamp);

                        con.Open();
                        await cmd.ExecuteNonQueryAsync();

                        con.Close();
                        cmd.Dispose();
                    }
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.StackTrace!.ToString());
                    throw;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<Role> GetRole(Guid id)
        {
            Role role = new();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.ConnectionString))
                {
                    const string query = "SELECT * FROM dbo.AspNetRoles WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, con)
                    {
                        CommandType = System.Data.CommandType.Text
                    };

                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());

                    con.Open();
                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    if (rdr.Read())
                    {
                        role.Id = Guid.Parse(rdr["Id"].ToString()!);
                        role.Name = rdr["Name"].ToString()!;
                        role.NormalizedName = rdr["NormalizedName"].ToString()!;
                        role.ConcurrencyStamp = Guid.Parse(rdr["ConcurrencyStamp"].ToString()!);
                    }

                    con.Close();
                    cmd.Dispose();
                }
                return role;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.StackTrace!.ToString());
                throw;
            }
        }

        public async Task<List<Role>> GetRoles()
        {
            List<Role> roles = new();
            try
            {
                using (SqlConnection con = new SqlConnection(_configuration.ConnectionString))
                {
                    const string query = "SELECT * FROM dbo.AspNetRoles";
                    SqlCommand cmd = new SqlCommand(query, con)
                    {
                        CommandType = System.Data.CommandType.Text
                    };

                    con.Open();
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        Role role = new Role
                        {
                            Id = Guid.Parse(reader["Id"].ToString()!),
                            Name = reader["Name"].ToString(),
                            NormalizedName = reader["NormalizedName"].ToString(),
                            ConcurrencyStamp = Guid.Parse(reader["ConcurrencyStamp"].ToString()!)
                        };
                        roles.Add(role);
                    }
                    cmd.Dispose();
                }
                return roles;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<List<Role>> GetRolesUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool VerificaSeRoleExiste(Role role)
        {
            bool flag = false;
            using (SqlConnection con = new SqlConnection(_configuration.ConnectionString))
            {
                const string query = "SELECT Name FROM dbo.AspNetRoles WHERE Name=@Name";

                SqlCommand cmd = new SqlCommand(query, con)
                {
                    CommandType = System.Data.CommandType.Text
                };
                cmd.Parameters.AddWithValue("@Name", role.Name);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
                con.Close();
                cmd.Dispose();
            }
            return flag;
        }
    }
}
