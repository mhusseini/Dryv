using System.ComponentModel.DataAnnotations;

namespace Dryv.Demo.Models
{
    public class HomeViewModel
    {
        public static readonly DryvRules Rules = DryvRules
            .For<HomeViewModel>()
            .Rule(m => m.Email,
                m => m.Age <= 18 || !string.IsNullOrWhiteSpace(m.Email)
                    ? DryvResult.Success
                    : "The email must be specified")
            .Rule(m => m.ParentsEmail,
                m => m.Age >= 18 || !string.IsNullOrWhiteSpace(m.ParentsEmail)
                    ? DryvResult.Success
                    : "The parents email must be specified");

        [Required]
        public int? Age { get; set; }

        [DryvRules]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [DryvRules]
        public string ParentsEmail { get; set; }
    }
}