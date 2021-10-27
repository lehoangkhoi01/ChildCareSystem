using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChildCareSystem.Data;
using ChildCareSystem.Models;
using ChildCareSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ChildCareSystem.Controllers
{
    [Authorize (Roles = "Customer")]
    public class FeedbacksController : Controller
    {
        private readonly ChildCareSystemContext _context;

        public FeedbacksController(ChildCareSystemContext context)
        {
            _context = context;
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            var childCareSystemContext = _context.Feedback.Include(f => f.ChildCareSystemUser).Include(f => f.Reservation).Include(f => f.Service);
            return View(await childCareSystemContext.ToListAsync());
        }

        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedback
                .Include(f => f.ChildCareSystemUser)
                .Include(f => f.Reservation)
                .Include(f => f.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public async Task<IActionResult> Create(int reservationId)
        {
            var feedback = await _context.Feedback.FirstOrDefaultAsync(u => u.ReservationId == reservationId);
            if(feedback != null)
            {
                return RedirectToAction(nameof(Edit), new { id = feedback.Id });
            }

            var reservation = await _context.Reservations.Include(u => u.Service)
                                                         .Include(u => u.Patient)
                                                         .Include(u => u.ChildCareSystemStaff)
                                                         .Include(u => u.ChildCareSystemUser)
                                                         .FirstOrDefaultAsync(u => u.Id == reservationId);
            if (reservation == null || reservation.CustomerId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }
            // Only allow feedback after checkInTime 1 hour
            var timeDiff = (DateTime.Now - reservation.CheckInDate).TotalHours;
            if (timeDiff < 1)
            {
                return RedirectToAction("GetCustomerReservationsList",
                                        "Reservations",
                                        new { error = "feedbackError" });
            }

            // Process to load information
            ViewBag.ReservationInfo = reservation;
            return View();
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Rate,Comment,ServiceId,ReservationId,CustomerId")] FeedbackViewModel feedbackViewModel)
        {
            
            if (ModelState.IsValid)
            {
                Feedback feedback = new Feedback
                {
                    Rate = feedbackViewModel.Rate,
                    Comment = feedbackViewModel.Comment,
                    CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    ReservationId = feedbackViewModel.ReservationId,
                    ServiceId = feedbackViewModel.ServiceId,
                };
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (feedbackViewModel.Rate < 1)
            {
                ViewBag.RATE_ERROR = "You should rate our service.";
            }

                var reservation = await _context.Reservations.Include(u => u.Service)
                                                         .Include(u => u.Patient)
                                                         .Include(u => u.ChildCareSystemStaff)
                                                         .Include(u => u.ChildCareSystemUser)
                                                         .FirstOrDefaultAsync(u => u.Id == feedbackViewModel.ReservationId);

            ViewBag.ReservationInfo = reservation;
            return View(feedbackViewModel);
        }

        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedback.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }


            // Get reservation info
            var reservation = await _context.Reservations.Include(u => u.Service)
                                                         .Include(u => u.Patient)
                                                         .Include(u => u.ChildCareSystemStaff)
                                                         .Include(u => u.ChildCareSystemUser)
                                                         .FirstOrDefaultAsync(u => u.Id == feedback.ReservationId);
            if (reservation == null || reservation.CustomerId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return NotFound();
            }

            ViewBag.ReservationInfo = reservation;
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rate,Comment,ServiceId,ReservationId")] Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedback
                .Include(f => f.ChildCareSystemUser)
                .Include(f => f.Reservation)
                .Include(f => f.Service)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedback.FindAsync(id);
            _context.Feedback.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedback.Any(e => e.Id == id);
        }
    }
}
