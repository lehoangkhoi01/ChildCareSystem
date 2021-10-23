using ChildCareSystem.Areas.Identity.Data;
using ChildCareSystem.Data;
using ChildCareSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChildCareSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageAccountsController : Controller
    {
        private readonly ChildCareSystemContext _context;
        private readonly UserManager<ChildCareSystemUser> _userManager;
        private readonly SignInManager<ChildCareSystemUser> _signInManager;

        public ManageAccountsController(ChildCareSystemContext context,
                               UserManager<ChildCareSystemUser> userManager,
                               SignInManager<ChildCareSystemUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var userList = await _context.Users.Skip(0 * 5).Take(5).ToListAsync();
            List<AccountWithRoleViewModel> accountList = new List<AccountWithRoleViewModel>();
            foreach(var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                AccountWithRoleViewModel account = new AccountWithRoleViewModel
                {
                    Email = user.Email,
                    FullName = user.FullName,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    RoleName = roles[0]
                };
                accountList.Add(account);
            }
            
            return View(accountList);
        }

        
        public IActionResult CreateStaff()
        {
            var list = _context.Specialty.ToList();
            ViewBag.Specialty = new SelectList(list, "Id", "SpecialtyName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([Bind("Email," +
                                                "Password," +
                                                "ConfirmPassword," +
                                                "Fullname,Address," +
                                                "PhoneNumber," +
                                                "RoleAssigned," +
                                                "SpecialtyId")] StaffViewModel staffViewModel)
        {
            if(ModelState.IsValid)
            {
                var user = new ChildCareSystemUser
                {
                    UserName = staffViewModel.Email,
                    Email = staffViewModel.Email,
                    PhoneNumber = staffViewModel.PhoneNumber,
                    FullName = staffViewModel.Fullname,
                    Address = staffViewModel.Address
                };
                var result = await _userManager.CreateAsync(user, staffViewModel.Password);
                if (result.Succeeded)
                {
                    // Add role
                    await _userManager.AddToRoleAsync(user, staffViewModel.RoleAssigned);
                    await _userManager.AddClaimAsync(user, new Claim("SpecialtyId", staffViewModel.SpecialtyId));
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }
    }
}
