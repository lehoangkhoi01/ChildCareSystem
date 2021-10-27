using ChildCareSystem.Areas.Identity.Data;
using ChildCareSystem.Data;
using ChildCareSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        private const int MAX_ITEM_PAGE = 4;

        public ReservationsController(ChildCareSystemContext context,
                               UserManager<ChildCareSystemUser> userManager,
                               SignInManager<ChildCareSystemUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index(int? service, string? error)
        {
            var patientProfileList = _context.Patient.Where(p =>
                                            p.CustomerId == User.FindFirstValue(ClaimTypes.NameIdentifier)
                                            && p.StatusId == 2)
                                                      .ToList();

            var profileCount = patientProfileList.Count();

            var timeAvailableList = _context.TimeAvailable.ToList();
            ViewBag.PatientList = patientProfileList;
            ViewBag.PatientCount = profileCount;
            ViewBag.TimeList = timeAvailableList;
            ViewData["Service"] = new SelectList(_context.Service, "Id", "ServiceName", service);


            switch(error)
            {
                case "serviceBusy":
                    ViewBag.ERROR = "You can not make reservation for service in chosen time. Try to change time or date.";
                    break;
                case "duplicated":
                    ViewBag.ERROR = "You have already make medical reservation for this patient in chosen time.";
                    break;
                case "invalidTime":
                    ViewBag.ERROR = "You must make reservation for service before at least 1 hour.";
                    break;
            }
  
            return View();
        }


        [HttpPost]
        public async  Task<IActionResult> Create([FromForm] string dateReservation, string timeReservation, int patientId, int serviceId)
        {
            string staffAssignedId = "";
            var dateString = dateReservation + " " + timeReservation;
            DateTime dateTime = DateTime.ParseExact(dateString, "dd-MM-yyyy HH:mm", CultureInfo.CurrentCulture);


            //Check validate datetime (must before at the moment at least 1 hour)
            var timeDiff = (dateTime - DateTime.Now);
            if (timeDiff.TotalHours < 1)
            {
                return RedirectToAction(nameof(Index), new { error = "invalidTime" });
            } 
            else if (timeDiff.TotalDays > 30) //Can not make reservation before 30 days
            {
                return RedirectToAction(nameof(Index), new { error = "serviceBusy" });
            }

            //Check duplicate reservation (same patient id and datetime)
            var duplicateReservation = await _context.Reservations.FirstOrDefaultAsync(u =>
                                                                                            u.CheckInDate == dateTime
                                                                                            && u.PatientId == patientId);

            if(duplicateReservation != null)
            {
                return RedirectToAction(nameof(Index), new { error = "duplicated" });
            }

            // Get service by service id
            var service = await _context.Service
                .FirstOrDefaultAsync(m => m.Id == serviceId); 

            //Get list of staff by service
            var staffListByService = _context.UserClaims.Where(s => s.ClaimType == "SpecialtyId"
                                                               && s.ClaimValue == service.SpecialtyId.ToString());
            

            foreach(var item in staffListByService)
            {
                var existedAssigned = await _context.Reservations.FirstOrDefaultAsync(r => r.StaffAssignedId == item.UserId
                                                                                && r.CheckInDate == dateTime);
                if (existedAssigned == null) 
                {
                    // Found free staff for service in chosen time
                    staffAssignedId = item.UserId;
                    break;
                } 
                
            }

            if(String.IsNullOrEmpty(staffAssignedId))
            {
                //Can not find free staff             
                return RedirectToAction(nameof(Index), new { error = "serviceBusy"});
            }

            Reservation newReservation = new Reservation
            {
                CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                ServiceId = serviceId,
                PatientId = patientId,
                CheckInDate = dateTime,
                Price = service.Price,
                StaffAssignedId = staffAssignedId
            };
            await _context.Reservations.AddAsync(newReservation);
            await _context.SaveChangesAsync();

            return View();
        }
    
        public async Task<IActionResult> GetCustomerReservationsList(string? error, int page = 1)
        {
            
            var reservationList = _context.Reservations.Where(r => r.CustomerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                                        .Include(u => u.Service)
                                                        .Include(u => u.ChildCareSystemUser)
                                                        .Include(u => u.ChildCareSystemStaff)                                
                                                        .Include(u => u.Patient)
                                                        .OrderByDescending(u => u.CheckInDate);

            int pageCount = (int)Math.Ceiling(reservationList.Count() / (double)MAX_ITEM_PAGE);
            if (page <= 0 || page > pageCount)
            {
                return NotFound();
            }

            var resultList = await reservationList.Skip((page - 1) * MAX_ITEM_PAGE)
                                                    .Take(MAX_ITEM_PAGE)
                                                    .ToListAsync();

            if(!String.IsNullOrEmpty(error))
            {
                switch (error)
                {
                    case "invalidCancel":
                        ViewBag.ERROR = "You can only cancel your reservation before at least 45 minutes.";
                        break;
                    case "feedbackError":
                        ViewBag.ERROR = "You can only give feedback about our service at least 1 hour after check in time. ";
                        break;
                }
            }
            
            ViewBag.PageCount = pageCount;
            ViewBag.CurrentPage = page;
            return View("List", resultList);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            
            if(reservation != null)
            {
                var timeDiff = (reservation.CheckInDate - DateTime.Now).TotalMinutes;
                if(timeDiff < 45)
                {
                    return RedirectToAction(nameof(GetCustomerReservationsList), 
                                            new { error = "invalidCancel"});
                }
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(GetCustomerReservationsList));
        }
    }
}
