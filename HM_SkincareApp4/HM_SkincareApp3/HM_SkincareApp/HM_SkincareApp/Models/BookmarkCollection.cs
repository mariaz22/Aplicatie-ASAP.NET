using HM_SkincareApp.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace HM_SkincareApp.Models
{
    // tabelul asociativ care face legatura intre Collection si Bookmark
    public class BookmarkCollection
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? BookmarkId { get; set; }
        public int? CollectionId { get; set; }

        public virtual Bookmark? Bookmark { get; set; }
        public virtual Collection? Collection { get; set; }

        public DateTime BookmarkDate { get; set; }
    }
}