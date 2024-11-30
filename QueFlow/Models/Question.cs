using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueFlow.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string Content { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Category este obligatoriu")]
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<Answer>? Answers { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; } 
    }
}
