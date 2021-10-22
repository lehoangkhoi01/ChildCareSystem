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

        public ReservationsController(ChildCareSystemContext context,
                               UserManager<ChildCareSystemUser> userManager,
                               SignInManager<ChildCareSystemUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult Index(bool? serviceBusy, bool? duplicated, bool? invalidTime)
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
            ViewData["Service"] = new SelectList(_context.Service, "Id", "ServiceName");

            if(serviceBusy != null && serviceBusy == true)
            {
                ViewBag.ERROR = "You can not make reservation for service in chosen time. Try to change time or date.";         
            } 
            else if (duplicated != null && duplicated == true)
            {
                ViewBag.ERROR = "You have already make medical reservation for this patient in chosen time.";
            }
            else if (invalidTime != null && invalidTime == true)
            {
                ViewBag.ERROR = "You must make reservation for service before at least 1 hour.";
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
            var timeDiff = (dateTime - DateTime.Now).TotalHours;
            if (timeDiff < 1)
            {
                return RedirectToAction(nameof(Index), new { invalidTime = true });
            }

            //Check duplicate reservation (same patient id and datetime)
            var duplicateReservation = await _context.Reservations.FirstOrDefaultAsync(u =>
                                                                                            u.CheckInDate == dateTime
                                                                                            && u.PatientId == patientId);

            if(duplicateReservation != null)
            {
                return RedirectToAction(nameof(Index), new { duplicated = true });
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
                return RedirectToAction(nameof(Index), new { serviceBusy = true});
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
    
        public async Task<IActionResult> GetCustomerReservationsList(bool? invalidDelete, bool? feedbackError)
        {
            var reservationList = _context.Reservations.Where(r => r.CustomerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                                        .Include(u => u.Service)
                                                        .Include(u => u.ChildCareSystemUser)
                                                        .Include(u => u.ChildCareSystemStaff)
                                                        .Include(u => u.Patient);

            if(invalidDelete != null && invalidDelete == true)
            {
                ViewBag.ERROR = "You can only cancel your reservation before at least 1 hour";
            } 
            else if (feedbackError != null && feedbackError == true)
            {
                ViewBag.ERROR = "You can only give feedback about our service at least 1 hour after check in time. ";
            }
            return View("List", await reservationList.ToListAsync());


        }

        public async Task<IActionResult> Delete(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            
            if(reservation != null)
            {
                var timeDiff = (reservation.CheckInDate - DateTime.Now).TotalHours;
                if(timeDiff < 1)
                {
                    return RedirectToAction(nameof(GetCustomerReservationsList), 
                                            new { invalidDelete = true});
                }
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(GetCustomerReservationsList));
        }
    }
}
