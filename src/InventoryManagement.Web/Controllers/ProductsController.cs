using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Application.Common;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventoryManagement.Web.Controllers
{
    /// <summary>
    /// Provides CRUD operations for products.
    /// </summary>
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsController"/> class.
        /// </summary>
        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// GET — lists products with search, filter, and pagination.
        /// </summary>
        public async Task<IActionResult> Index(
            string? searchTerm,
            int? categoryId,
            string? stockStatus,
            int pageNumber = 1,
            int pageSize = 10)
        {
            StockStatus? parsedStock = null;
            if (!string.IsNullOrWhiteSpace(stockStatus) &&
                Enum.TryParse<StockStatus>(stockStatus, ignoreCase: true, out var ss))
            {
                parsedStock = ss;
            }

            var result = await _productService.GetPagedAsync(
                searchTerm, categoryId, parsedStock, pageNumber, pageSize);

            var categories = await _categoryService.GetAllAsync();

            var model = new ProductIndexViewModel
            {
                Result = result,
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                StockStatus = stockStatus,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName,
                    Selected = c.CategoryId == categoryId
                })
            };

            return View(model);
        }

        /// <summary>
        /// GET — shows full product details.
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        /// <summary>
        /// GET — shows the create product form.
        /// </summary>
        public async Task<IActionResult> Create()
        {
            await PopulateCategories();
            return View(new ProductDto());
        }

        /// <summary>
        /// POST — creates a new product.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductDto product)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var created = await _productService.CreateAsync(product);
                    TempData["SuccessMessage"] = $"Product '{created.ProductName}' was created.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ValidationException ex)
                {
                    AddModelErrors(ex);
                }
            }

            await PopulateCategories();
            return View(product);
        }

        /// <summary>
        /// GET — shows the edit product form.
        /// </summary>
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            await PopulateCategories();
            return View(product);
        }

        /// <summary>
        /// POST — updates an existing product.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDto product)
        {
            if (id != product.ProductId) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    var updated = await _productService.UpdateAsync(product);
                    TempData["SuccessMessage"] = $"Product '{updated.ProductName}' was updated.";
                    return RedirectToAction(nameof(Index));
                }
                catch (ValidationException ex)
                {
                    AddModelErrors(ex);
                }
            }

            await PopulateCategories();
            return View(product);
        }

        /// <summary>
        /// GET — shows the delete confirmation page.
        /// </summary>
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        /// <summary>
        /// POST — deletes the product.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Product was deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList();
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