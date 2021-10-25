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
using ChildCareSystem.ViewModels;
using ChildCareSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace ChildCareSystem.Controllers
{

    public class BlogsController : Controller
    {
        private readonly ChildCareSystemContext _context;
        private const int MAX_ITEM_PAGE = 4;
        private readonly UserManager<ChildCareSystemUser> _userManager;
        private readonly SignInManager<ChildCareSystemUser> _signInManager;

        public BlogsController(ChildCareSystemContext context,
                               UserManager<ChildCareSystemUser> userManager,
                               SignInManager<ChildCareSystemUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Blogs
        public async Task<IActionResult> Index(string search, int page = 1)
        {
            int pageCount;
            var childCareSystemContext = _context.Blog.Include(b => b.BlogCategory)
                                                        .Include(b => b.ChildCareSystemUser);

            
            IEnumerable<Blog> resultList;
            resultList = await childCareSystemContext.Skip((page - 1) * MAX_ITEM_PAGE)
                                                            .Take(MAX_ITEM_PAGE)
                                                            .ToListAsync();

            if (!String.IsNullOrEmpty(search))
            {
                resultList = resultList.Where(s => s.Title.Contains(search)).ToList();
                pageCount = (int)Math.Ceiling(resultList.Count() / (double)MAX_ITEM_PAGE);

            }
            else
            {
                pageCount = (int)Math.Ceiling(_context.Blog.Count() / (double)MAX_ITEM_PAGE);
            }


            if (page <= 0 || page > pageCount)
            {
                return NotFound();
            }
            ViewBag.PageCount = pageCount;
            ViewBag.CurrentPage = page;

            return View(resultList);
        }

        // GET: Blogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .Include(b => b.BlogCategory)
                .Include(b => b.ChildCareSystemUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // GET: Blogs/Create
        [Authorize(Roles = "Staff, Admin")]
        public async Task<IActionResult> Create()
        {
            var list = await _context.BlogCategory.ToListAsync();
            ViewBag.Category = new SelectList(list, "Id", "CategoryName");
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Staff, Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,ImageLink,ImageName,CategoryId")] BlogViewModel blogViewModel)
        {
            if (ModelState.IsValid)
            {
                var blogObj = await _context.Blog
                    .FirstOrDefaultAsync(b => b.Title.Trim().ToUpper() == blogViewModel.Title.Trim().ToUpper());
                if (blogObj != null)
                {
                    ViewBag.ErrorMessage = "This blog is already existed.";
                }
                else
                {

                    Blog blog = new Blog
                    {
                        Title = blogViewModel.Title,
                        Content = blogViewModel.Content,
                        Image = blogViewModel.ImageLink,
                        BlogCategoryId = blogViewModel.CategoryId,
                        AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                    };
                    _context.Add(blog);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(blogViewModel);
        }

        // GET: Blogs/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var blog = await _context.Blog.FindAsync(id);
            BlogViewModel blogViewModel = new BlogViewModel();
            if (blog == null)
            {
                return NotFound();
            }
            var list = await _context.BlogCategory.ToListAsync();
            ViewData["Category"] = new SelectList(list, "Id", "CategoryName", blog.BlogCategoryId);
            blogViewModel.Id = blog.Id;
            blogViewModel.ImageLink = blog.Image;
            blogViewModel.Title = blog.Title;
            blogViewModel.Content = blog.Content;
            blogViewModel.CategoryId = blog.BlogCategoryId;
            blogViewModel.AuthorId = blog.AuthorId;


            return View(blogViewModel);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
                                             [Bind("Id,Title,Content,ImageLink,ImageName,CategoryId")] BlogViewModel blogViewModel)
        {
            if (id != blogViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //List<Blog> listBlog = new List<Blog>();
                    //listBlog = await _context.Blog.ToListAsync();
                    List<Blog> listBlog = await _context.Blog.ToListAsync();
                    listBlog.RemoveAll(c => c.Id == blogViewModel.Id);
                    var blogDuplicate = listBlog
                                        .FirstOrDefault(b => b.Title.Trim().ToUpper() == blogViewModel.Title.Trim().ToUpper());
                    if (blogDuplicate != null)
                    {
                        ViewBag.ErrorMessage = "This blog's title is already existed.";
                        var list = await _context.BlogCategory.ToListAsync();
                        ViewData["Category"] = new SelectList(list, "Id", "CategoryName", blogViewModel.CategoryId);
                    } 
                    else
                    {
                        _context.ChangeTracker.Clear();
                        Blog blog = new Blog
                        {
                            Id = blogViewModel.Id,
                            Title = blogViewModel.Title,
                            Content = blogViewModel.Content,
                            AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                            Image = blogViewModel.ImageLink,
                            BlogCategoryId = blogViewModel.CategoryId,                                                     
                        };
                        _context.Blog.Update(blog);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }                                      
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blogViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }
            return View(blogViewModel);
        }

        // GET: Blogs/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blogs/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blog.FindAsync(id);
            _context.Blog.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blog.Any(e => e.Id == id);
        }
    }
}
