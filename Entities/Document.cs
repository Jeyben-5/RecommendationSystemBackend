using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Document : Entity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public string Link { get; set; }

        public Subject Subject { get; set; }
    }
}
