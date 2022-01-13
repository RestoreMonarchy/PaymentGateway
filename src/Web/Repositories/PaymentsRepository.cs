using Dapper;
using Newtonsoft.Json;
using RestoreMonarchy.PaymentGateway.Models.Helpers;
using RestoreMonarchy.PaymentGateway.Web.Models.Database;
using System.Data;
using System.Data.SqlClient;

namespace RestoreMonarchy.PaymentGateway.Web.Repositories
{
    public class PaymentsRepository
    {
        private readonly SqlConnection connection;

        public PaymentsRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task UpdatePaymentDataAsync(Guid publicId, string jsonData)
        {
            const string sql = "UPDATE dbo.Payments SET JsonData = @jsonData WHERE PublicId = @publicId;";
            await connection.ExecuteAsync(sql, new { publicId, jsonData });
        }

        public async Task<MPayment> GetPaymentWithStoreAsync(Guid publicId)
        {
            const string sql = "SELECT p.*, s.* FROM dbo.Payments p " +
                "JOIN dbo.Stores s ON s.Id = p.StoreId " +
                "WHERE p.PublicId = @publicId;";

            return (await connection.QueryAsync<MPayment, MStore, MPayment>(sql, (p, s) => 
            {
                p.Store = s;
                return p;
            }, new { publicId })).FirstOrDefault();
        }

        public async Task IncrementNotifiedCountAsync(MPayment payment)
        {
            const string sql = "UPDATE dbo.Payments " +
                "SET NotifiedCount = @NotifiedCount, NotifiedDate = SYSDATETIME() " +
                "WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, payment);
        }

        public async Task SetNotifiedAsync(MPayment payment)
        {
            const string sql = "UPDATE dbo.Payments " +
                "SET IsNotified = 1, NotifiedDate = SYSDATETIME(), NotifiedCount = @NotifiedCount " +
                "WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, payment);
        }

        public async Task CompletePaymentAsync(Guid publicId)
        {
            const string sql = "UPDATE dbo.Payments " +
                "SET IsCompleted = 1, CompletedDate = SYSDATETIME() " +
                "WHERE PublicId = @publicId;";

            await connection.ExecuteAsync(sql, new { publicId });
        }

        public async Task<Guid> AddPaymentAsync(MPayment payment)
        {
            const string sql = "dbo.AddPayment";

            return await connection.ExecuteScalarAsync<Guid>(sql, new 
            { 
                payment.StoreId,
                payment.Provider,
                payment.Receiver,
                payment.Custom,
                payment.Amount,
                payment.Currency,
                Items = JsonConvert.SerializeObject(payment.Items)
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<MPayment>> GetPendingPaymentsAsync(string provider)
        {
            const string sql = "SELECT p.*, s.*, i.* FROM dbo.Payments p " +
                "JOIN dbo.Stores s ON s.Id = p.StoreId " +
                "LEFT JOIN dbo.PaymentItems i ON p.Id = i.PaymentId " +
                "WHERE p.Provider = @provider AND p.IsCompleted = 0;";

            return await GetPaymentsSharedAsync(sql, new { provider });
        }

        public async Task<MPayment> GetPaymentAsync(Guid publicId)
        {
            const string sql = "SELECT p.*, s.*, i.* FROM dbo.Payments p " +
                "JOIN dbo.Stores s ON s.Id = p.StoreId " +
                "LEFT JOIN dbo.PaymentItems i ON p.Id = i.PaymentId " +
                "WHERE p.PublicId = @publicId;";

            return await GetPaymentSharedAsync(sql, new { publicId });
        }

        private async Task<MPayment> GetPaymentSharedAsync(string sql, object param)
        {
            MPayment payment = null;

            await connection.QueryAsync<MPayment, MStore, MPaymentItem, MPayment>(sql, (p, s, i) =>
            {
                if (payment == null)
                {
                    payment = p;
                    payment.Store = s;
                    payment.Items = new List<MPaymentItem>();
                }

                if (i != null)
                {
                    payment.Items.Add(i);
                }

                return null;
            }, param);

            return payment;
        }

        private async Task<IEnumerable<MPayment>> GetPaymentsSharedAsync(string sql, object param)
        {
            List<MPayment> payments = new List<MPayment>();

            await connection.QueryAsync<MPayment, MStore, MPaymentItem, MPayment>(sql, (p, s, i) =>
            {
                MPayment payment = payments.FirstOrDefault(x => x.Id == p.Id);

                if (payment == null)
                {
                    payment = p;
                    payment.Store = s;
                    payment.Items = new List<MPaymentItem>();
                    payments.Add(payment);
                }

                if (i != null)
                {
                    payment.Items.Add(i);
                }

                return null;
            }, param);

            return payments;
        }

        public async Task<PaymentWithProvider> GetPaymentWithProviderAsync(Guid publicId)
        {
            const string sql = "SELECT p.*, s.*, pp.*, i.* FROM dbo.Payments p " +
                "JOIN dbo.Stores s ON s.Id = p.StoreId " +
                "LEFT JOIN dbo.StorePaymentProviders pp ON pp.StoreId = s.Id AND pp.PaymentProvider = p.Provider " + 
                "LEFT JOIN dbo.PaymentItems i ON p.Id = i.PaymentId " +
                "WHERE p.PublicId = @publicId;";

            PaymentWithProvider result = null;

            await connection.QueryAsync<MPayment, MStore, MStorePaymentProvider, MPaymentItem, MPayment>(sql, (p, s, pp, i) =>
            {
                if (result == null)
                {
                    result = new PaymentWithProvider()
                    {
                        Payment = p,
                        Provider = pp
                    };
                    result.Payment.Store = s;
                    result.Payment.Items = new List<MPaymentItem>();
                }

                if (i != null)
                {
                    result.Payment.Items.Add(i);
                }

                return null;
            }, new { publicId }, splitOn: "Id,Id,StoreId,PaymentId");

            return result;
        }

        public async Task<IEnumerable<MPayment>> GetPaymentsAsync()
        {
            const string sql = "SELECT p.*, s.*, i.* FROM dbo.Payments p " +
                "JOIN dbo.Stores s ON s.Id = p.StoreId " +
                "LEFT JOIN dbo.PaymentItems i ON p.Id = i.PaymentId;";

            List<MPayment> payments = new();

            await connection.QueryAsync<MPayment, MStore, MPaymentItem, MPayment>(sql, (p, s, i) =>
            {
                MPayment payment = payments.FirstOrDefault(x => x.Id == p.Id);
                if (payment == null)
                {
                    payment = p;
                    payment.Store = s;
                    payment.Items = new List<MPaymentItem>();
                    payments.Add(payment);
                }

                if (i != null)
                {
                    payment.Items.Add(i);
                }

                return null;
            });

            return payments;
        }
    }
}
