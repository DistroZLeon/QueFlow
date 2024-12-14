using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QueFlow.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage ="Name is required")]
        public string? Nume {  get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string? Alias { get; set; }
        public string? ProfPic {  get; set; }
        public string? Desc {  get; set; }
        public virtual ICollection<Question>? Questions { get; set; }
        public virtual ICollection<Answer>? Answers { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}
