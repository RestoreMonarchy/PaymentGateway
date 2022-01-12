using Dapper;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using System.Data;
using System.Data.SqlClient;

namespace RestoreMonarchy.PaymentGateway.Web.Repositories
{
    public class StoresRepository
    {
        private readonly SqlConnection connection;

        public StoresRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task DeleteStorePaymentProviderAsync(MStorePaymentProvider provider)
        {
            const string sql = "DELETE FROM dbo.StorePaymentProviders " +
                "WHERE StoreId = @StoreId AND PaymentProvider = @PaymentProvider;";
            await connection.ExecuteAsync(sql, provider);
        }

        public async Task UpdateStorePaymentProviderAsync(MStorePaymentProvider provider)
        {
            const string sql = "dbo.UpdateStorePaymentProvider";
            await connection.ExecuteAsync(sql, new 
            {
                provider.StoreId,
                provider.PaymentProvider,
                provider.JsonParameters,
                provider.IsEnabled
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AddStoreAsync(MStore store)
        {
            const string sql = "INSERT INTO dbo.Stores (Name, LogoUrl, BackgroundUrl, DefaultCurrency, DefaultNotifyUrl, DefaultCancelUrl, " +
                "DefaultReturnUrl, IsPublic) " +
                "OUTPUT INSERTED.Id " +
                "VALUES (@Name, @LogoUrl, @BackgroundUrl, @DefaultCurrency, @DefaultNotifyUrl, @DefaultCancelUrl, @DefaultReturnUrl, " +
                "@IsPublic);";
            return await connection.ExecuteScalarAsync<int>(sql, store);
        }

        public async Task UpdateStoreAsync(MStore store)
        {
            const string sql = "UPDATE dbo.Stores SET Name = @Name, LogoUrl = @LogoUrl, BackgroundUrl = @BackgroundUrl, " +
                "DefaultCurrency = @DefaultCurrency, DefaultNotifyUrl = @DefaultNotifyUrl, DefaultCancelUrl = @DefaultCancelUrl, " +
                "DefaultReturnUrl = @DefaultReturnUrl, IsPublic = @IsPublic, UpdateDate = SYSDATETIME() " +
                "WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, store);
        }

        public async Task<IEnumerable<MStore>> GetStoresAsync()
        {
            const string sql = "SELECT * FROM dbo.Stores;";
            return await connection.QueryAsync<MStore>(sql);
        }

        public async Task<MStore> GetStoreAsync(Guid apiKey)
        {
            const string sql = "SELECT s.*, p.* FROM dbo.Stores s " +
                "LEFT JOIN dbo.StorePaymentProviders p ON s.Id = p.StoreId " +
                "WHERE s.APIKey = @apiKey;";
            return await GetStoreSharedAsync(sql, new { apiKey });
        }

        public async Task<MStore> GetStoreAsync(int storeId)
        {
            const string sql = "SELECT s.*, p.* FROM dbo.Stores s " +
                "LEFT JOIN dbo.StorePaymentProviders p ON s.Id = p.StoreId " +
                "WHERE s.Id = @storeId;";
            return await GetStoreSharedAsync(sql, new { storeId });
        }

        private async Task<MStore> GetStoreSharedAsync(string sql, object param)
        {
            MStore store = null;
            await connection.QueryAsync<MStore, MStorePaymentProvider, MStore>(sql, (s, p) =>
            {
                if (store == null)
                {
                    store = s;
                    store.Providers = new List<MStorePaymentProvider>();
                }

                if (p != null)
                {
                    store.Providers.Add(p);
                }

                return null;

            }, param, splitOn: "StoreId");
            return store;
        }
    }
}
