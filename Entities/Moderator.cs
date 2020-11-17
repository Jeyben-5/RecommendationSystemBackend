using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Moderator : Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }
        
        public int PhoneNumber { get; set; }

        [Required]
        public int CellPhone { get; set; }

        public string Address { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
