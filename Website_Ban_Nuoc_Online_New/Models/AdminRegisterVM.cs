using System.ComponentModel.DataAnnotations;

namespace Website_Ban_Nuoc_Online.Models
{
    public class AdminRegisterVM
    {
        [Required, MinLength(4)]
        public string Username { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public string CCCD { get; set; }

        [Required]
        public string Gender { get; set; }

        public string Address { get; set; }
    }
}
