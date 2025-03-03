using HM_SkincareApp.Controllers;
using HM_SkincareApp.Data;
using HM_SkincareApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using Humanizer;
using System.Text.RegularExpressions;
using System.Security.Claims;

namespace HM_SkincareApp.Controllers
{
    [Authorize]
    public class BookmarksController : Controller
    {



        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public BookmarksController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }

        
       
        [AllowAnonymous]
        public IActionResult Index(string filter = "recent")
        {


            var bookmarks = db.Bookmarks.Include("Label").Include("User").Include("Likes").OrderByDescending(b => b.Date).OrderByDescending(b =>b.Likes.Count);

            if (filter == "popular")
            {
                // Sortare după numărul de like-uri 
                bookmarks = bookmarks.OrderByDescending(b => b.Likes.Count);
            }
            else
            {
                // Sortare după dată 
                bookmarks = bookmarks.OrderByDescending(b => b.Date);
            }

            var search = "";
            // MOTOR DE CAUTARE
            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {

                // eliminam spatiile libere
                search =
                Convert.ToString(HttpContext.Request.Query["search"]).Trim();

               
                List<int> bookmarkIds = db.Bookmarks.Where

                (
                at => at.Title.Contains(search)
                || at.Label.Name.Contains(search)
                ).OrderBy(a => a.Title).Select(a => a.Id).ToList();

                // Cautare in comentarii (Content)

                List<int> articleIdsOfCommentsWithSearchString =
                db.Comments.Where(c => c.Content.Contains(search)).Select(c => (int)c.BookmarkId).ToList();

                // Se formeaza o singura lista formata din toate id-urile selectate anterior

                List<int> mergedIds = bookmarkIds.Union(articleIdsOfCommentsWithSearchString).ToList();
                // Lista articolelor care contin cuvantul cautat

                bookmarks = db.Bookmarks.Where(bookmark => mergedIds.Contains(bookmark.Id))
                                .Include("Label")
                                .Include("User")
                                .Include("Likes")
                                .OrderBy(a => a.Date);
            }
            ViewBag.SearchString = search;

            // AFISARE PAGINATA

            int _perPage = 6;



            int totalItems = bookmarks.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }
            var paginatedBookmarks =
            bookmarks.Skip(offset).Take(_perPage);

            // Preluam numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)_perPage);
            // Trimitem articolele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Bookmarks = paginatedBookmarks;

            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Bookmarks/Index/?search="
                + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Bookmarks/Index/?page";
            }



            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }




            return View();
        }


        // HttpGet implicit

        [Authorize(Roles = "User,Admin")]
        [AllowAnonymous]
        public IActionResult Show(int id)
        {
            Bookmark bookmark = db.Bookmarks.Include("Label")
                                         .Include("User")
                                         .Include("Comments")
                                         .Include("Comments.User")
                                         .Include("Likes")
                                         .Where(bkm => bkm.Id == id)
                                         .First();

            // Adaugam bookmark-urile utilizatorului pentru dropdown
            ViewBag.UserCollections = db.Collections
                                      .Where(b => b.UserId == _userManager.GetUserId(User))
                                      .ToList();


            SetAccessRights();

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return View(bookmark);
        }


        // Adaugarea unui comentariu asociat unui articol in baza de date
        // Toate rolurile pot adauga comentarii in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Admin")]

        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Bookmarks/Show/" + comment.BookmarkId);
            }

            else
            {
                Bookmark bkm = db.Bookmarks.Include("Label")
                                         .Include("User")
                                         .Include("Comments")
                                         .Include("Comments.User")
                                         .Where(bkm => bkm.Id == comment.BookmarkId)
                                         .First();


                // Adaugam bookmark-urile utilizatorului pentru dropdown
                ViewBag.UserCollections = db.Collections
                                          .Where(b => b.UserId == _userManager.GetUserId(User))
                                          .ToList();

                SetAccessRights();

                return View(bkm);
            }
        }

        [HttpPost]
        public IActionResult AddCollection([FromForm] BookmarkCollection bookmarkCollection)
        {
            // Daca modelul este valid
            if (ModelState.IsValid)
            {
                if (db.BookmarkCollections
                    .Where(bm => bm.BookmarkId == bookmarkCollection.BookmarkId)
                    .Where(bm => bm.CollectionId == bookmarkCollection.CollectionId)
                    .Count() > 0)
                {
                    TempData["message"] = "This bookmark is already added to the collection.";
                    TempData["messageType"] = "alert-danger";
                }
                else
                {
                    db.BookmarkCollections.Add(bookmarkCollection);
                    
                    db.SaveChanges();

                    TempData["message"] = "The bookmark has been added to the selected collection";
                    TempData["messageType"] = "alert-success";
                }

            }
            else
            {
                TempData["message"] = "The bookmark could not be added to the collection";
                TempData["messageType"] = "alert-danger";
            }

            return Redirect("/Bookmarks/Show/" + bookmarkCollection.BookmarkId);
        }






        // HttpGet implicit

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            Bookmark bookmark = new Bookmark();

            // Se preia lista de categorii cu ajutorul metodei GetAllLabels()
            bookmark.Categ = GetAllLabels();


            return View(bookmark);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public IActionResult New(Bookmark bookmark)
        {
            bookmark.Date = DateTime.Now;

            // preluam id-ul utilizatorului care posteaza bookmarkul
            bookmark.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Bookmarks.Add(bookmark);
                db.SaveChanges();
                TempData["message"] = "The bookmark has been added.";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                bookmark.Categ = GetAllLabels();
                return View(bookmark);
            }
        }



        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {

            Bookmark bookmark = db.Bookmarks.Include("Label")
                                        .Where(bm => bm.Id == id)
                                        .First();

            bookmark.Categ = GetAllLabels();

            if (bookmark.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(bookmark);
            }

            else
            {
                TempData["message"] = "You do not have the right to make changes to a bookmark that does not belong to you.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Bookmark requestBookmark)
        {
            Bookmark bookmark = db.Bookmarks.Find(id);


            if (ModelState.IsValid)
            {
                if (bookmark.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    bookmark.Title = requestBookmark.Title;
                    bookmark.Description = requestBookmark.Description;
                    bookmark.LabelId = requestBookmark.LabelId;
                    bookmark.Url = requestBookmark.Url;
                    TempData["message"] = "The bookmark has been modifiedt";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "You do not have the permission to make changes to a bookmark that does not belong to you";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                requestBookmark.Categ = GetAllLabels();
                return View(requestBookmark);
            }
        }


        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Bookmark bookmark = db.Bookmarks.Include("Comments")
                                            .Include("Likes")
                                         .Where(bm => bm.Id == id)
                                         .First();

            if (bookmark.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Bookmarks.Remove(bookmark);
                db.SaveChanges();
                TempData["message"] = "The bookmark has been deleted";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }

            else
            {
                TempData["message"] = "You do not have the permission to delete a bookmark that does not belong to you";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }


        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
        {
            ViewBag.AfisareButoane = false;

            if (User.IsInRole("User"))
            {
                ViewBag.AfisareButoane = true;
            }

            ViewBag.EsteAdmin = User.IsInRole("Admin");

            ViewBag.UserCurent = _userManager.GetUserId(User);
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllLabels()
        {
            // generam o lista de tipul SelectListItem fara elemente
            var selectList = new List<SelectListItem>();

            // extragem toate categoriile din baza de date
            var Labels = from cat in db.Labels
                             select cat;

            // iteram prin categorii
            foreach (var Label in Labels)
            {
                // adaugam in lista elementele necesare pentru dropdown
                // id-ul categoriei si denumirea acesteia
                selectList.Add(new SelectListItem
                {
                    Value = Label.Id.ToString(),
                    Text = Label.Name.ToString()
                });
            }


            // returnam lista de categorii
            return selectList;
        }



        public bool UserLikedPost(int bookmarkId)
        {
            string userId = _userManager.GetUserId(User);
            return db.Likes.Any(l => l.BookmarkId == bookmarkId && l.UserId == userId);
        }

        [HttpPost]
        public async Task<IActionResult> AddLike(int id)
        {
            Bookmark bookmark = await db.Bookmarks.FindAsync(id);

            if (bookmark != null)
            {
                string userId = _userManager.GetUserId(User);


                if (UserLikedPost(bookmark.Id))
                {
                    // Utilizatorul a apreciat deja, retragem aprecierea
                    Like likeToRemove = db.Likes.First(l => l.BookmarkId == bookmark.Id && l.UserId == userId);
                    db.Likes.Remove(likeToRemove);
                }
                else
                {
                    // Utilizatorul nu a apreciat încă, adăugăm aprecierea
                    Like like = new Like { BookmarkId = bookmark.Id, UserId = userId };
                    db.Likes.Add(like);
                }

                await db.SaveChangesAsync();
            }



            return RedirectToAction("Index");
           
        }


        public IActionResult IndexNou()
        {
            return View();
        }

    }
}