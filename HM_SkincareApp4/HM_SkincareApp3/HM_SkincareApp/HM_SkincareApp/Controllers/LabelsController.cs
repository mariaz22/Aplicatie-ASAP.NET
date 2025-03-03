using HM_SkincareApp.Data;
using HM_SkincareApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HM_SkincareApp.Data;
using HM_SkincareApp.Models;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace HM_SkincareApp.Controllers
{
   
    public class LabelsController : Controller
    {

        

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public LabelsController(ApplicationDbContext context)
        {
            db = context;
        }

        [Authorize(Roles = "Admin,User")]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var labels = from label in db.Labels
                             orderby label.Name
                             select label ;
            ViewBag.Labels = labels;
            return View();
        }

        [Authorize(Roles = "Admin,User")]
        [AllowAnonymous]
        public IActionResult BookmarksByLabel (int labelId)
        {
            var bookmarks = db.Bookmarks.Include("Label").Include("User").Include("Likes")
                                .Where(b => b.LabelId == labelId)
                                .OrderByDescending(b => b.Date)
                                .ToList();

            ViewBag.Bookmarks = bookmarks;
            ViewBag.SelectedLabelId = labelId;  

            return View("Index");  
        }


        [Authorize(Roles = "User,Admin")]
        [AllowAnonymous]
        public IActionResult Show(int id)
        {

            var label = db.Labels
                .Include(c => c.Bookmarks)
                .ThenInclude(b => b.Likes)
                .ThenInclude(b => b.User)
                .FirstOrDefault(c => c.Id == id);

            if (label == null)
            {
                TempData["message"] = "You do not have the necessary rights";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Bookmarks");
            }

            return View(label);
        }



        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Label cat)
        {
            if (ModelState.IsValid)
            {
                db.Labels.Add(cat);
                db.SaveChanges();
                TempData["message"] = "The label has been added";
                return RedirectToAction("Index");
            }

            else
            {
                return View(cat);
            }
        }
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Label label = db.Labels.Find(id);
            return View(label);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Label requestLabel)
        {
            Label label = db.Labels.Find(id);

            if (ModelState.IsValid)
            {

                label.Name = requestLabel.Name;
                db.SaveChanges();
                TempData["message"] = "The label has been modified!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestLabel);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Label label = db.Labels.Find(id);
            db.Labels.Remove(label);
            TempData["message"] = "The label has been deleted";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

       
    }
}
