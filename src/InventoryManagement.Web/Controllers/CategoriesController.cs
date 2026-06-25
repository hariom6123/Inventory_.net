using System.Threading.Tasks;
using InventoryManagement.Application.Common;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Web.Controllers
{
    /// <summary>
    /// Provides CRUD operations for categories.
    /// </summary>
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoriesController"/> class.
        /// </summary>
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// GET — lists all categories.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        /// <summary>
        /// GET — shows the create form.
        /// </summary>
        public IActionResult Create() => View(new CategoryDto());

        /// <summary>
        /// POST — creates a new category.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var created = await _categoryService.CreateAsync(category);
                    TempData["SuccessMessage"] = $"Category '{created.CategoryName}' was created.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ValidationException ex)
                {
                    AddModelErrors(ex);
                }
            }

            return View(category);
        }

        /// <summary>
        /// GET — shows the edit form.
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        /// <summary>
        /// POST — updates the category.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryDto category)
        {
            if (id != category.CategoryId) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var updated = await _categoryService.UpdateAsync(category);
                    TempData["SuccessMessage"] = $"Category '{updated.CategoryName}' was updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ValidationException ex)
                {
                    AddModelErrors(ex);
                }
            }

            return View(category);
        }

        /// <summary>
        /// GET — shows the delete confirmation page.
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        /// <summary>
        /// POST — deletes the category.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Category was deleted.";
            }
            catch (ValidationException ex)
            {
                TempData["ErrorMessage"] = string.Join("; ", ex.Result.Errors.SelectMany(kvp => kvp.Value));
            }

            return RedirectToAction(nameof(Index));
        }

        private void AddModelErrors(ValidationException ex)
        {
            foreach (var kvp in ex.Result.Errors)
            {
                foreach (var msg in kvp.Value)
                {
                    ModelState.AddModelError(kvp.Key, msg);
                }
            }
        }
    }
}