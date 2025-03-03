using System.ComponentModel.DataAnnotations;

namespace HM_SkincareApp.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The content of the comment is mandatory.")]
        public string Content { get; set; }

        public DateTime Date { get; set; }

        // un comentariu apartine unui bookmark
        public int? BookmarkId { get; set; }

        // un comentariu este postat de catre un user
        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual Bookmark? Bookmark { get; set; }
    }
}

