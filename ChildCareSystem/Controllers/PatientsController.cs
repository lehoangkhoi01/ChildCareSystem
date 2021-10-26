using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChildCareSystem.Data;
using ChildCareSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ChildCareSystem.Controllers
{
    [Authorize(Roles = "Customer")]
    public class PatientsController : Controller
    {
        private readonly ChildCareSystemContext _context;
        private const int MAX_PATIENT = 4;

        public PatientsController(ChildCareSystemContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index(string? maxPatientError)
        {
            var childCareSystemContext = _context.Patient.Include(p => p.ChildCareSystemUser)
                                                            .Include(p => p.Status)
                                                            .Where(p => p.StatusId == 2);
            if(!String.IsNullOrEmpty(maxPatientError))
            {
                ViewBag.MAX_PATIENT_ERROR = maxPatientError;
            }
            return View(await childCareSystemContext.ToListAsync());
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .Include(p => p.ChildCareSystemUser)
                .Include(p => p.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null || patient.StatusId == 3)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public async Task<IActionResult> Create()
        {
            var listPatient = await _context.Patient.Where(p => p.StatusId == 2
                                                                     && p.CustomerId == User.FindFirstValue(ClaimTypes.NameIdentifier))
                                               .ToListAsync();
            var count = listPatient.Count();
            if (count == MAX_PATIENT)
            {
                var error = "You can only create maximum of 4 patient profiles.";
                return RedirectToAction(nameof(Index), new { maxPatientError = error });
            }
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PatientName,Gender,Birthdate")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                var ageDiff = (DateTime.Today.Year - patient.Birthdate.Year);
                if (ageDiff < 2 || ageDiff > 16)
                {
                    ViewBag.AgeError = "Our center just take care for 2-16 year-old children.";
                } else
                {
                    patient.CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    patient.StatusId = 2; // 2: Active, 3: Unactive
                    _context.Add(patient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }     
            }
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient.FindAsync(id);
            if (patient == null || patient.StatusId == 3)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientName,Gender,Birthdate,CustomerId,StatusId")] Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var ageDiff = (DateTime.Today.Year - patient.Birthdate.Year);
                    if (ageDiff < 2 || ageDiff > 16)
                    {
                        ViewBag.AgeError = "Our center just take care for 2-16 year-old children.";
                    }
                    else
                    {
                        patient.CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        patient.StatusId = 2; // 2: Active, 3: Unactive
                        _context.Update(patient);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patient
                .Include(p => p.ChildCareSystemUser)
                .Include(p => p.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patient = await _context.Patient.FindAsync(id);
            patient.StatusId = 3; //3: Unactive
            _context.Patient.Update(patient);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return _context.Patient.Any(e => e.Id == id);
        }
    }
}
