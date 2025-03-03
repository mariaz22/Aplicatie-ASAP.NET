using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using HM_SkincareApp.Models;
using System.ComponentModel.DataAnnotations.Schema;




namespace HM_SkincareApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        // un user poate posta mai multe comentarii
        public virtual ICollection<Comment>? Comments { get; set; }

        // un user poate posta mai multe articole
        public virtual ICollection<Bookmark>? Bookmarks { get; set; }

        // un user poate sa creeze mai multe colectii
        public virtual ICollection<Collection>? Collections { get; set; }

        // atribute suplimentare adaugate pentru user
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? About { get; set; }

        public string? ProfilePictureUrl { get; set; }

        // variabila in care vom retine rolurile existente in baza de date
        // pentru popularea unui dropdown list
        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}