
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HM_SkincareApp.Data;
using HM_SkincareApp.Models;

namespace HM_SkincareApp.Controllers
{
    
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;
        }
        [Authorize(Roles = "Admin,User")]
        [AllowAnonymous]
        [HttpGet("Users/Profile/{userId:guid}")]
        public IActionResult Profile(string userId)
        {
            var user = db.Users
                          .Include(u => u.Collections)
                          .Include(u => u.Bookmarks)
                          .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [Authorize(Roles = "Admin,User")]
        public IActionResult Profile()
        {
            var userId = _userManager.GetUserId(User);
            var user = db.Users
                          .Include(u => u.Collections)
                          .Include(u => u.Bookmarks)
                          .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                
                return NotFound();
            }

            return View(user);
        }

        [Authorize(Roles = "Admin")]

        public IActionResult Index()
        {
            // Obține utilizatorii din baza de date și include colecțiile asociate
            var users = db.Users
                          .Include(u => u.Collections) 
                          .OrderBy(u => u.UserName) 
                          .ToList(); 

            ViewBag.UsersList = users; 

            return View();
        }


        [AllowAnonymous]
        public IActionResult PublicIndex()
        {
            
            var users = db.Users
                          .Include(u => u.Collections.Where(c => c.IsPublic)) 
                          .OrderBy(u => u.UserName) 
                          .ToList(); 


            ViewBag.UsersList = users;

            return View(); 
        }



        public async Task<ActionResult> Show(string id)
        {
            ApplicationUser user = db.Users.Find(id);
            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Roles = roles;

            SetAccessRights();

            return View(user);
        }
        
        public async Task<ActionResult> Edit(string id)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();

            var roleNames = await _userManager.GetRolesAsync(user); 

            var currentUserRole = _roleManager.Roles
                                              .Where(r => roleNames.Contains(r.Name))
                                              .Select(r => r.Id)
                                              .First(); 
            ViewBag.UserRole = currentUserRole;

            return View(user);
        }

        [HttpPost]
        
        public async Task<ActionResult> Edit(string id, ApplicationUser newData, [FromForm] string newRole)
        {
            ApplicationUser user = db.Users.Find(id);

            user.AllRoles = GetAllRoles();


            if (ModelState.IsValid)
            {
                user.UserName = newData.UserName;
                user.Email = newData.Email;
                user.FirstName = newData.FirstName;
                user.LastName = newData.LastName;
                user.PhoneNumber = newData.PhoneNumber;
                user.About = newData.About;
                user.ProfilePictureUrl = newData.ProfilePictureUrl;


                var roles = db.Roles.ToList();

                foreach (var role in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                var roleName = await _roleManager.FindByIdAsync(newRole);
                await _userManager.AddToRoleAsync(user, roleName.ToString());

                db.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> EditProfile()
        {
            var userId = _userManager.GetUserId(User); 
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user); 
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(ApplicationUser model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);

                if (user == null)
                {
                    return NotFound();
                }

          
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.About = model.About;
                user.PhoneNumber = model.PhoneNumber;
                user.ProfilePictureUrl = model.ProfilePictureUrl;

                // Actualizează utilizatorul în baza de date
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
               {
                    return RedirectToAction("Profile", "Users", new { userId = user.Id }); 


                }

                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

           return View(model); 
        }



        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = db.Users
                         .Include("Bookmarks")
                         .Include("Comments")
                         .Include("Collections")
                         .Where(u => u.Id == id)
                         .First();

           
            if (user.Comments.Count > 0)
            {
                foreach (var comment in user.Comments)
                {
                    db.Comments.Remove(comment);
                }
            }

           
            if (user.Bookmarks.Count > 0)
            {
                foreach (var bookmark in user.Bookmarks)
                {
                    db.Bookmarks.Remove(bookmark);
                }
            }


            db.ApplicationUsers.Remove(user);

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [NonAction]
        public IEnumerable<SelectListItem> GetAllRoles()
        {
            var selectList = new List<SelectListItem>();

            var roles = from role in db.Roles
                        select role;

            foreach (var role in roles)
            {
                selectList.Add(new SelectListItem
                {
                    Value = role.Id.ToString(),
                    Text = role.Name.ToString()
                });
            }
            return selectList;
        }
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
    }

   
}
