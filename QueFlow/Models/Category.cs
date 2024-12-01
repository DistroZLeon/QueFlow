using System.ComponentModel.DataAnnotations;

namespace QueFlow.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="Numele categoriei este obligatoriu")]
        [StringLength(100,ErrorMessage ="Dimensiunea maxima este de 100 caractere")]
        [MinLength(5,ErrorMessage ="Dimensiunea minima de 5 caractere")]
        public string Name { get; set; }
        [StringLength(300,ErrorMessage ="Exista o limita de caractere :)")]
        public string? Description { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }
    }
}
