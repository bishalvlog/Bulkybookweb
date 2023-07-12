using System.ComponentModel.DataAnnotations;

namespace BulkyBook.Models
{
    public class CoverType
    {
        [Key]
        public int Id { get; set; }
        [Display(Name="Display name")]
        [Required]  
        public string Name { get; set; }    
    }
}
