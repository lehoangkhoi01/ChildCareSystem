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

namespace ChildCareSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SpecialtiesController : Controller
    {
        private readonly ChildCareSystemContext _context;

        public SpecialtiesController(ChildCareSystemContext context)
        {
            _context = context;
        }

        // GET: Specialties
        public async Task<IActionResult> Index()
        {
            return View(await _context.Specialty.ToListAsync());
        }

        // GET: Specialties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialty
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // GET: Specialties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SpecialtyName")] Specialty specialty)
        {
            if (ModelState.IsValid)
            {
                var specialtyDuplicate = await _context.Specialty
                    .FirstOrDefaultAsync(b => b.SpecialtyName.Trim().ToUpper() == specialty.SpecialtyName.Trim().ToUpper());
                if (specialtyDuplicate != null)
                {
                    ViewBag.ErrorMessage = "This specialty is already existed.";
                } 
                else
                {
                    _context.Add(specialty);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                
            }
            return View(specialty);
        }

        // GET: Specialties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialty.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }
            return View(specialty);
        }

        // POST: Specialties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SpecialtyName")] Specialty specialty)
        {
            if (id != specialty.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<Specialty> listSpecialty = await _context.Specialty.ToListAsync();
                    listSpecialty.RemoveAll(c => c.Id == specialty.Id);
                    Specialty specialtyTest = listSpecialty
                        .FirstOrDefault(b => b.SpecialtyName.Trim().ToUpper() == specialty.SpecialtyName.Trim().ToUpper());

                    if (specialtyTest != null)
                    {
                        ViewBag.ErrorMessage = "This specialty is already existed.";
                    }
                    else
                    {
                        _context.ChangeTracker.Clear();
                        _context.Specialty.Update(specialty);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }                  
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtyExists(specialty.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            return View(specialty);
        }

        // GET: Specialties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialty = await _context.Specialty
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // POST: Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialty = await _context.Specialty.FindAsync(id);
            _context.Specialty.Remove(specialty);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialtyExists(int id)
        {
            return _context.Specialty.Any(e => e.Id == id);
        }
    }
}
