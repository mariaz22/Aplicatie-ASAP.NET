using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HM_SkincareApp.Models
{
    public class Bookmark
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The title is mandatory")]
        [StringLength(100, ErrorMessage = "The title cannot be more than 100 characters.")]
        [MinLength(5, ErrorMessage = "The title must be more than 5 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The bookmark description is mandatory.")]
        public string? Description { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "The label is mandatory.")]
        // un articol are asociata o categorie
        public int? LabelId { get; set; }

        // un articol este postat de catre un user
        public string? UserId { get; set; }

        public string? Url { get; set; }
        // PASUL 6 - useri si roluri
        public virtual ApplicationUser? User { get; set; }

        public virtual Label? Label { get; set; }

        // un articol poate avea o colectie de comentarii
        public virtual ICollection<Comment>? Comments { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }

        // relatia many-to-many dintre Collections si Bookmark
        public virtual ICollection<BookmarkCollection>? BookmarkCollections { get; set; }

        public virtual ICollection<Like>? Likes { get; set; }
    }
}