using System.ComponentModel.DataAnnotations;

namespace RestoreMonarchy.PaymentGateway.Web.Models.Database
{
    public class MStore
    {
        public int Id { get; set; }

        public Guid APIKey { get; set; }

        [Required, StringLength(255, MinimumLength = 3)]
        public string Name { get; set; }

        [Required, StringLength(255, MinimumLength = 3)]
        public string LogoUrl { get; set; }

        [StringLength(255, MinimumLength = 3)]
        public string BackgroundUrl { get; set; }

        [Required, StringLength(3, MinimumLength = 3)]
        public string DefaultCurrency { get; set; }

        [Required, StringLength(255, MinimumLength = 3)]
        public string DefaultNotifyUrl { get; set; }

        [Required, StringLength(255, MinimumLength = 3)]
        public string DefaultCancelUrl { get; set; }

        [Required, StringLength(255, MinimumLength = 3)]
        public string DefaultReturnUrl { get; set; }

        public bool IsPublic { get; set; }

        public DateTime UpdateDate { get; set; }

        public DateTime CreateDate { get; set; }

        public List<MStorePaymentProvider> Providers { get; set; }

        public MStore Clone()
        {
            return new MStore()
            {
                Id = Id,
                Name = Name,
                APIKey = APIKey,
                BackgroundUrl = BackgroundUrl,
                DefaultCancelUrl = DefaultCancelUrl,
                DefaultReturnUrl = DefaultReturnUrl,
                DefaultCurrency = DefaultCurrency,
                DefaultNotifyUrl = DefaultNotifyUrl,
                IsPublic = IsPublic,
                LogoUrl = LogoUrl,
                UpdateDate = UpdateDate,
                CreateDate = CreateDate,
                Providers = Providers
            };
        }
    }
}
