using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FS02P2.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class RoleManagerController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleManagerController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }


        public  IActionResult CreateRole() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (roleName == null) 
            {
                return BadRequest();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    return RedirectToAction("Index");   
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(roleName);
        }
        public async Task <IActionResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete == null)
            {
                return NotFound();

            }

            return View(roleToDelete);
        }

        [HttpPost,ActionName("DeleteRole")]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> DeleteRoleConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete == null)
            {
                return NotFound();

            }
           var roleDeleted = await _roleManager.DeleteAsync(roleToDelete);
            if (roleDeleted.Succeeded) 
            {
                return RedirectToAction("Index");
            }
             return View(roleToDelete); 
        }
    }
}
