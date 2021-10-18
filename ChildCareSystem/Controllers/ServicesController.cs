﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ChildCareSystem.Data;
using ChildCareSystem.Models;
using ChildCareSystem.ViewModels;
using Microsoft.Data.SqlClient;

namespace ChildCareSystem.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ChildCareSystemContext _context;

        public ServicesController(ChildCareSystemContext context)
        {
            _context = context;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            var childCareSystemContext = _context.Service.Include(s => s.Specialty);
            return View(await childCareSystemContext.ToListAsync());
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service
                .Include(s => s.Specialty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // GET: Services/Create
        public IActionResult Create()
        {
            ViewData["Specialty"] = new SelectList(_context.Specialty, "Id", "SpecialtyName");
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServiceName,ImageLink,ImageName,Description,Price,SpecialtyId")] ServiceViewModel serviceViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Service service = new Service
                    {
                        ServiceName = serviceViewModel.ServiceName,
                        SpecialtyId = serviceViewModel.SpecialtyId,
                        Description = serviceViewModel.Description,
                        Price = serviceViewModel.Price,
                        ThumbnailLink = serviceViewModel.ImageLink
                    };
                    _context.Add(service);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                } 
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 2601))
                {
                    ViewBag.ErrorMessage = "This service's name is already existed.";
                }               
            }
            ViewData["Specialty"] = new SelectList(_context.Specialty.ToList(), "Id", "SpecialtyName", serviceViewModel.SpecialtyId);
            return View(serviceViewModel);
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            ServiceViewModel serviceViewModel = new ServiceViewModel
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Description = service.Description,
                Price = service.Price,
                ImageLink = service.ThumbnailLink,
                SpecialtyId = service.SpecialtyId,
            };
            ViewData["Specialty"] = new SelectList(_context.Specialty, "Id", "SpecialtyName", service.SpecialtyId);
            return View(serviceViewModel);
        }

        // POST: Services/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
                                                [Bind("Id,ServiceName,ImageLink,ImageName,Description,Price,SpecialtyId")] ServiceViewModel serviceViewModel)
        {
            if (id != serviceViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Service service = new Service
                    {
                        ServiceName = serviceViewModel.ServiceName,
                        SpecialtyId = serviceViewModel.SpecialtyId,
                        Description = serviceViewModel.Description,
                        Price = serviceViewModel.Price,
                        ThumbnailLink = serviceViewModel.ImageLink
                    };
                    _context.Service.Update(service);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(serviceViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlException && (sqlException.Number == 2627 || sqlException.Number == 2601))
                {
                    ViewBag.ErrorMessage = "This service's name is already existed.";
                }
                
            }
            ViewData["Specialty"] = new SelectList(_context.Specialty, "Id", "SpecialtyName", serviceViewModel.SpecialtyId);
            return View(serviceViewModel);
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Service
                .Include(s => s.Specialty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Service.FindAsync(id);
            _context.Service.Remove(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(int id)
        {
            return _context.Service.Any(e => e.Id == id);
        }
    }
}