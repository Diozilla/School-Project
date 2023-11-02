using FS02P2.Models;
using FS02P2.Models.SchoolViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FS02P2.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class UserRolesController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;  
        public UserRolesController(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new List<UserRolesData>();
            foreach (var user in users) 
            {
                var viewModel = new UserRolesData();
                viewModel.UserId = user.Id;
                viewModel.UserName = user.UserName;
                viewModel.FirstName = user.FirstName;
                viewModel.LastName = user.LastName;
                viewModel.Email = user.Email;
                viewModel.Roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(viewModel);

            }
            return View(userRoles);

        }
    

        public async Task<IActionResult> ManageRoles(string id)
        {
            if (id == null) { return BadRequest(); }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) { return NotFound(); }
            var viewModel = new List<ManageUserRolesData>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesData
                {
                    RoleId = role.Id ,
                    RoleName = role.Name ,
                    Assigned = (await _userManager.IsInRoleAsync(user, role.Name)? true : false)

                };
                viewModel.Add(userRolesViewModel);
            }
            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> ManageRoles(string id, List<ManageUserRolesData> viewModel)
        {
            if (id ==  null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) { return View(); }


            var roles = await _userManager.GetRolesAsync(user);

            var roleStatus = await _userManager.RemoveFromRolesAsync(user,roles);

            if (!roleStatus.Succeeded) 
            {
                return View(viewModel);
            }

            roleStatus = await _userManager.AddToRolesAsync(user, viewModel.Where(vw => vw.Assigned).Select(vw => vw.RoleName));
            if (roleStatus.Succeeded)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
