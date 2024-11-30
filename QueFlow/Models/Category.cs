using System.ComponentModel.DataAnnotations;

namespace QueFlow.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Numele categoriei este obligatoriu")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }
    }
}
