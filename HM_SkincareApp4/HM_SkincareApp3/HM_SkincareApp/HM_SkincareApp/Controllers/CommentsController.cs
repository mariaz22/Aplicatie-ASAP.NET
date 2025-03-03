using HM_SkincareApp.Data;
using HM_SkincareApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HM_SkincareApp.Data;
using HM_SkincareApp.Models;
using System.Data;

namespace HM_SkincareApp.Controllers
{
    public class CommentsController : Controller
    {



        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

       

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Bookmarks/Show/" + comm.BookmarkId);
            }

            else
            {
                TempData["message"] = "You do not have the right to delete the commen";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User))
            {
                return View(comm);
            }

            else
            {
                TempData["message"] = "You do not have the right to edit the comment";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    comm.Content = requestComment.Content;

                    db.SaveChanges();

                    return Redirect("/Bookmarks/Show/" + comm.BookmarkId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            else
            {
                TempData["message"] = "You do not have the right to make changes";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }
        }
    }
}