using ChildCareSystem.Areas.Identity.Data;
using ChildCareSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChildCareSystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class ReservationsController : Controller
    {
        private readonly ChildCareSystemContext _context;
        private readonly UserManager<ChildCareSystemUser> _userManager;
        private readonly SignInManager<ChildCareSystemUser> _signInManager;

        public ReservationsController(ChildCareSystemContext context,
                               UserManager<ChildCareSystemUser> userManager,
                               SignInManager<ChildCareSystemUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index()
        {
            var patientProfileList = _context.Patient.Where(p =>
                                            p.CustomerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                                      .ToList();

            var profileCount = patientProfileList.Count();
            ViewBag.PatientList = patientProfileList;
            ViewBag.PatientCount = profileCount;
            return View();
        }
    }
}
