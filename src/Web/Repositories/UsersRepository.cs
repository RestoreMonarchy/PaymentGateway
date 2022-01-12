using Dapper;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using System.Data;
using System.Data.SqlClient;

namespace RestoreMonarchy.PaymentGateway.Web.Repositories
{
    public class UsersRepository
    {
        private readonly SqlConnection connection;

        public UsersRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<MUser> GetUserAsync(string name, string password)
        {
            const string sql = "dbo.GetUser";
            return await connection.QuerySingleOrDefaultAsync<MUser>(sql, new { name, password }, commandType: CommandType.StoredProcedure);
        }
    }
}
