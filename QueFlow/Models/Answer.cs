using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueFlow.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Continutul raspunsului este obligatoriu")]
        public string Text { get; set; }
        public DateTime DatePosted { get; set; }
        public int? QuestionId {  get; set; }
        public virtual Question? Q { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }
    }
}
