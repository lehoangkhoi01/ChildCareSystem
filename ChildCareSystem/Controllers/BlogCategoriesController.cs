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
    public class BlogCategoriesController : Controller
    {
        private readonly ChildCareSystemContext _context;

        public BlogCategoriesController(ChildCareSystemContext context)
        {
            _context = context;
        }

        // GET: BlogCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.BlogCategory.ToListAsync());
        }

        // GET: BlogCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogCategory = await _context.BlogCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogCategory == null)
            {
                return NotFound();
            }

            return View(blogCategory);
        }

        // GET: BlogCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BlogCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,categoryName")] BlogCategory blogCategory)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.BlogCategory
                    .FirstOrDefaultAsync(b => b.categoryName.Trim().ToUpper() == blogCategory.categoryName.Trim().ToUpper());
                if (category != null)
                {
                    ViewBag.ErrorMessage = "This category is already existed.";
                } 
                else
                {
                    _context.Add(blogCategory);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }              
            }
            return View(blogCategory);
        }

        // GET: BlogCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogCategory = await _context.BlogCategory.FindAsync(id);
            if (blogCategory == null)
            {
                return NotFound();
            }
            return View(blogCategory);
        }

        // POST: BlogCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,categoryName")] BlogCategory blogCategory)
        {
            if (id != blogCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    List<BlogCategory> listCategory = new List<BlogCategory>();
                    listCategory =  await _context.BlogCategory.ToListAsync();          
                    listCategory.RemoveAll(c => c.Id == blogCategory.Id);
                    BlogCategory categoryTest = listCategory
                        .FirstOrDefault(b => b.categoryName.Trim().ToUpper() == blogCategory.categoryName.Trim().ToUpper());

                    if (categoryTest != null)
                    {
                        ViewBag.ErrorMessage = "This category is already existed.";
                    }
                    else
                    {
                        _context.ChangeTracker.Clear();
                        _context.BlogCategory.Update(blogCategory);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogCategoryExists(blogCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }              
            }
            return View(blogCategory);
        }

        // GET: BlogCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogCategory = await _context.BlogCategory
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogCategory == null)
            {
                return NotFound();
            }
            return View(blogCategory);
        }

        // POST: BlogCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogCategory = await _context.BlogCategory.FindAsync(id);
            _context.BlogCategory.Remove(blogCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogCategoryExists(int id)
        {
            return _context.BlogCategory.Any(e => e.Id == id);
        }
    }
}
