using System.ComponentModel.DataAnnotations;

namespace RestoreMonarchy.PaymentGateway.Web.Models.View
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Rememmber me")]
        public bool IsPersistent { get; set; } = true;
    }
}
