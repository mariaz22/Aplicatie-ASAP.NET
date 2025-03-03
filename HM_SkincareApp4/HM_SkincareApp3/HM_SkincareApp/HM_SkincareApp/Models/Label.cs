using System.ComponentModel.DataAnnotations;

namespace HM_SkincareApp.Models
{
    public class Label
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The name of the label is mandatory.")]
        public string Name { get; set; }

        public virtual ICollection<Bookmark>? Bookmarks { get; set; }
    }
}
