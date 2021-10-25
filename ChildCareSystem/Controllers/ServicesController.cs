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
using Microsoft.AspNetCore.Authorization;

namespace ChildCareSystem.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ChildCareSystemContext _context;
        private const int MAX_ITEM_PAGE = 4;
        

        public ServicesController(ChildCareSystemContext context)
        {
            _context = context;
        }

        // GET: Services
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageCount;
            var childCareSystemContext = _context.Service.Include(s => s.Specialty);
            IEnumerable<Service> resultList;
            resultList = await childCareSystemContext.Skip((page - 1) * MAX_ITEM_PAGE)
                                                            .Take(MAX_ITEM_PAGE)
                                                            .ToListAsync();
            if (!String.IsNullOrEmpty(search))
            {
                resultList = resultList.Where(s => s.ServiceName.Contains(search)).ToList();
                pageCount = (int)Math.Ceiling(resultList.Count() / (double)MAX_ITEM_PAGE);

            } 
            else
            {
                pageCount = (int)Math.Ceiling(_context.Service.Count() / (double)MAX_ITEM_PAGE);
            }
            
            if (page <= 0 || page > pageCount)
            {
                return NotFound();
            }
            ViewBag.PageCount = pageCount;
            ViewBag.CurrentPage = page;
            return View(resultList);
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["Specialty"] = new SelectList(_context.Specialty, "Id", "SpecialtyName");
            return View();
        }

        // POST: Services/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ServiceName,ImageLink,ImageName,Description,Price,SpecialtyId")] ServiceViewModel serviceViewModel)
        {
            if (ModelState.IsValid)
            {

                var serviceObj = await _context.Service
               .FirstOrDefaultAsync(b => b.ServiceName.Trim().ToUpper() == serviceViewModel.ServiceName.Trim().ToUpper());
                if (serviceObj != null)
                {
                    ViewBag.ErrorMessage = "This service is already existed.";
                }
                else
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



            }
            ViewData["Specialty"] = new SelectList(_context.Specialty.ToList(), "Id", "SpecialtyName", serviceViewModel.SpecialtyId);
            return View(serviceViewModel);
        }

        // GET: Services/Edit/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
                    //List<Service> listService = await _context.Service.ToListAsync();
                    //listService.RemoveAll(c => c.Id == serviceViewModel.Id);
                    //var serviceDuplicate = listService
                    //                   .FirstOrDefault(b => b.ServiceName.Trim().ToUpper() == serviceViewModel.ServiceName.Trim().ToUpper());
                    
                    //if (serviceDuplicate != null)
                    //{
                    //    ViewBag.ErrorMessage = "This service's name is already existed."; 
                    //    ViewData["Specialty"] = new SelectList(_context.Specialty, "Id", "SpecialtyName", serviceViewModel.SpecialtyId);
                    //}
                    //else
                    //{
                    //    _context.ChangeTracker.Clear();
                        Service service = new Service
                        {
                            Id = serviceViewModel.Id,
                            ServiceName = serviceViewModel.ServiceName,
                            SpecialtyId = serviceViewModel.SpecialtyId,
                            Description = serviceViewModel.Description,
                            Price = serviceViewModel.Price,
                            ThumbnailLink = serviceViewModel.ImageLink
                        };
                        
                        _context.Service.Update(service);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    //}                   
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
