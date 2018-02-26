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
                    : "The email must be specifies")
            .Rule(m => m.ParentsEmail,
                m => m.Age >= 18 || !string.IsNullOrWhiteSpace(m.ParentsEmail)
                    ? Result.Success
                    : "The parents email must be specifies");

        public int Age { get; set; }

        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        public string ParentsEmail { get; set; }
    }
}