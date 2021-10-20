using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChildCareSystem.Data;
using ChildCareSystem.Models;

namespace ChildCareSystem.Controllers
{
    public class TimeAvailablesController : Controller
    {
        private readonly ChildCareSystemContext _context;

        public TimeAvailablesController(ChildCareSystemContext context)
        {
            _context = context;
        }

        // GET: TimeAvailables
        public async Task<IActionResult> Index()
        {
            return View(await _context.TimeAvailable.ToListAsync());
        }

        // GET: TimeAvailables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeAvailable = await _context.TimeAvailable
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeAvailable == null)
            {
                return NotFound();
            }

            return View(timeAvailable);
        }

        // GET: TimeAvailables/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TimeAvailables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TimeString")] TimeAvailable timeAvailable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(timeAvailable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(timeAvailable);
        }

        // GET: TimeAvailables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeAvailable = await _context.TimeAvailable.FindAsync(id);
            if (timeAvailable == null)
            {
                return NotFound();
            }
            return View(timeAvailable);
        }

        // POST: TimeAvailables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TimeString")] TimeAvailable timeAvailable)
        {
            if (id != timeAvailable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(timeAvailable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TimeAvailableExists(timeAvailable.Id))
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
            return View(timeAvailable);
        }

        // GET: TimeAvailables/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var timeAvailable = await _context.TimeAvailable
                .FirstOrDefaultAsync(m => m.Id == id);
            if (timeAvailable == null)
            {
                return NotFound();
            }

            return View(timeAvailable);
        }

        // POST: TimeAvailables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var timeAvailable = await _context.TimeAvailable.FindAsync(id);
            _context.TimeAvailable.Remove(timeAvailable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TimeAvailableExists(int id)
        {
            return _context.TimeAvailable.Any(e => e.Id == id);
        }
    }
}
