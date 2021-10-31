using ChildCareSystem.Areas.Identity.Data;
using ChildCareSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChildCareSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ChildCareSystemContext _context;
        private readonly UserManager<ChildCareSystemUser> _userManager;
        private readonly SignInManager<ChildCareSystemUser> _signInManager;
        public IActionResult Index()
        {
            return View();
        }
    }
}
