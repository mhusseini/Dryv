using System.ComponentModel.DataAnnotations;

namespace Dryv.Demo.Models
{
    public class HomeViewModel
    {
        public static readonly Rules Rules = Rules
            .For<HomeViewModel>()
            .Rule(m => m.Email,
                m => m.Age <= 18 || !string.IsNullOrWhiteSpace(m.Email)
                    ? Result.Success
                    : "The email must be specified")
            .Rule(m => m.ParentsEmail,
                m => m.Age >= 18 || !string.IsNullOrWhiteSpace(m.ParentsEmail)
                    ? Result.Success
                    : "The parents email must be specified");

        public int Age { get; set; }

        [Dryv]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Dryv]
        public string ParentsEmail { get; set; }
    }
}