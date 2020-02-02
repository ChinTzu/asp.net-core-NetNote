using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace NetNote.ViewModels
{
    public class NoteModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; }
        [Display(Name = "Type")]
        public int Type { get; set; }
        [Display(Name = "password")]
        public string Password { get; set; }
        [Display(Name = "attachment")]
        public IFormFile Attachment { get; set; }
    }
}
