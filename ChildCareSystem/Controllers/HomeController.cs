using ChildCareSystem.Areas.Identity.Data;
using ChildCareSystem.Data;
using ChildCareSystem.Models;
using ChildCareSystem.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChildCareSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ChildCareSystemContext _context;
        private readonly UserManager<ChildCareSystemUser> _userManager;
        private readonly SignInManager<ChildCareSystemUser> _signInManager;
        private const int MAX_ITEM_PAGE = 3;

        public HomeController(ILogger<HomeController> logger,
                                ChildCareSystemContext context,
                                UserManager<ChildCareSystemUser> userManager,
                                SignInManager<ChildCareSystemUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var serviceList = await _context.Service.OrderByDescending(s => s.Id)
                                                    .Take(MAX_ITEM_PAGE)
                                                    .ToListAsync();

            var blogList = await _context.Blog.OrderByDescending(b => b.Id)
                                                    .Take(MAX_ITEM_PAGE)
                                                    .ToListAsync();

            if (serviceList.Count > 0)
            {
                ViewBag.ServiceList = serviceList;
            }

            if (blogList.Count > 0)
            {
                ViewBag.BlogList = blogList;
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            UserProfileViewModel userProfile = new UserProfileViewModel
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
                Address = user.Address,
            };
            
            return View("UserProfile", userProfile);             
        }

        public async Task<IActionResult> UpdateInfo([Bind] UserProfileViewModel userProfileViewModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            Claim oldNameClaim = new Claim("DisplayName", user.FullName);
            userProfileViewModel.Email = user.Email;
            if (ModelState.IsValid)
            {                
                //Check valid old password
                var result = await _signInManager.PasswordSignInAsync(user.Email,
                                                                        userProfileViewModel.OldPassword,
                                                                        false,
                                                                        lockoutOnFailure: false);
                var resultCheckPassword = await _signInManager.CheckPasswordSignInAsync(user,
                                                                                        userProfileViewModel.OldPassword,
                                                                                        false);

                if (resultCheckPassword.Succeeded)
                {
                    //Current password valid
                    user.FullName = userProfileViewModel.FullName;
                    user.Address = userProfileViewModel.Address;
                    user.PhoneNumber = userProfileViewModel.PhoneNumber;

                    //Update password 
                    if (!String.IsNullOrEmpty(userProfileViewModel.NewPassword))
                    {
                        var changePasswordResult = await _userManager.ChangePasswordAsync(user,
                                                                                            userProfileViewModel.OldPassword,
                                                                                            userProfileViewModel.NewPassword);
                        if (!changePasswordResult.Succeeded)
                        {
                            foreach (var error in changePasswordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }

                            return View("UserProfile", userProfileViewModel);
                        }
                        
                    }
                    _context.Users.Update(user);

                    //Update claim
                    Claim newNameClaim = new Claim("DisplayName", userProfileViewModel.FullName);
                    await _userManager.RemoveClaimAsync(user, oldNameClaim);
                    await _userManager.AddClaimAsync(user, newNameClaim);
                    await _signInManager.RefreshSignInAsync(user);
                    await _context.SaveChangesAsync();
                    ViewBag.UpdateSuccess = "Update successfully!";
                    //return RedirectToAction(nameof(GetProfile));
                }
                else // when current password is wrong
                {
                    ViewBag.PasswordError = "Current password is not correct";
                }
            }
            return View("UserProfile", userProfileViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
