using ChildCareSystem.Areas.Identity.Data;
using ChildCareSystem.Data;
using ChildCareSystem.ViewModels;
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
            
            return View();
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
