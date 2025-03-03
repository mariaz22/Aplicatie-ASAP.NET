using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace HM_SkincareApp.Models
{
    public class Collection
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The name of the collection is mandatory.")]
        public string Name { get; set; }

        // o colectie este creata de catre un user
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }

        // Proprietatea pentru a determina vizibilitatea colecției
        public bool IsPublic { get; set; } 

        // relatia many-to-many dintre Collection si Bookmark
        public virtual ICollection<BookmarkCollection>? BookmarkCollections { get; set; }

    }
}