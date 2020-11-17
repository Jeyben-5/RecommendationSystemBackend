using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Subject : Entity
    {
        [Required]
        [StringLength(70)]
        public string Name { get; set; }

        [Required]
        [StringLength(70)]
        public string Docente { get; set; }
    }
}
