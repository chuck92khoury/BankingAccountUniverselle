using System.ComponentModel.DataAnnotations;

namespace BankingPlatformAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } // Stocker les mots de passe de manière sécurisée (hashés)
    }
}
