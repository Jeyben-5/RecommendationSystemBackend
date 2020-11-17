using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Student : Entity
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
