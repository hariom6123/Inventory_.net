using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
    /// Provides inventory operations (stock in, stock out, adjustment, history).
    /// </summary>
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryController"/> class.
        /// </summary>
        public InventoryController(
            IInventoryService inventoryService,
            IProductService productService,
            IMapper mapper)
        {
            _inventoryService = inventoryService;
            _productService = productService;
            _mapper = mapper;
        }

        /// <summary>
        /// GET — shows the inventory operations landing page.
        /// </summary>
        public IActionResult Index() => RedirectToAction(nameof(History));

        /// <summary>
        /// GET — shows the stock-in form.
        /// </summary>
        public async Task<IActionResult> StockIn()
        {
            await PopulateProducts();
            return View(new InventoryTransactionDto { TransactionType = TransactionType.StockIn });
        }

        /// <summary>
        /// POST — records an incoming stock movement.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockIn(InventoryTransactionDto model)
        {
            model.TransactionType = TransactionType.StockIn;

            if (ModelState.IsValid)
            {
                try
                {
                    var tx = await _inventoryService.StockInAsync(model.ProductId, model.Quantity, model.Notes);
                    TempData["SuccessMessage"] = $"Stock in recorded: +{tx.QuantityChanged}.";
                    return RedirectToAction(nameof(History));
                }
                catch (ValidationException ex)
                {
                    AddModelErrors(ex);
                }
            }

            await PopulateProducts();
            return View(model);
        }

        /// <summary>
        /// GET — shows the stock-out form.
        /// </summary>
        public async Task<IActionResult> StockOut()
        {
            await PopulateProducts();
            return View(new InventoryTransactionDto { TransactionType = TransactionType.StockOut });
        }

        /// <summary>
        /// POST — records an outgoing stock movement.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockOut(InventoryTransactionDto model)
        {
            model.TransactionType = TransactionType.StockOut;

            if (ModelState.IsValid)
            {
                try
                {
                    var tx = await _inventoryService.StockOutAsync(model.ProductId, model.Quantity, model.Notes);
                    TempData["SuccessMessage"] = $"Stock out recorded: {tx.QuantityChanged}.";
                    return RedirectToAction(nameof(History));
                }
                catch (ValidationException ex)
                {
                    AddModelErrors(ex);
                }
            }

            await PopulateProducts();
            return View(model);
        }

        /// <summary>
        /// GET — shows the adjust-quantity form.
        /// </summary>
        public async Task<IActionResult> Adjust(int productId)
        {
            var product = await _productService.GetByIdAsync(productId);
            if (product == null) return NotFound();

            return View(new InventoryTransactionDto
            {
                ProductId = productId,
                TransactionType = TransactionType.Adjustment,
                Quantity = product.Quantity
            });
        }

        /// <summary>
        /// POST — applies a manual adjustment.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjust(InventoryTransactionDto model)
        {
            model.TransactionType = TransactionType.Adjustment;

            if (ModelState.IsValid)
            {
                try
                {
                    var tx = await _inventoryService.AdjustAsync(model.ProductId, model.Quantity, model.Notes);
                    TempData["SuccessMessage"] = $"Inventory adjusted by {tx.QuantityChanged} units.";
                    return RedirectToAction(nameof(History));
                }
                catch (ValidationException ex)
                {
                    AddModelErrors(ex);
                }
            }

            return View(model);
        }

        /// <summary>
        /// GET — shows the inventory transaction history.
        /// </summary>
        public async Task<IActionResult> History(int? productId)
        {
            var txs = await _inventoryService.GetTransactionsAsync(productId);
            var products = await _productService.GetPagedAsync(null, null, null, 1, 1000);

            var model = new InventoryHistoryViewModel
            {
                ProductId = productId,
                Transactions = _mapper.Map<System.Collections.Generic.IEnumerable<InventoryTransactionViewModel>>(txs),
                Products = products.Items.Select(p => new SelectListItem
                {
                    Value = p.ProductId.ToString(),
                    Text = $"{p.ProductName} ({p.SKU})",
                    Selected = p.ProductId == productId
                })
            };

            return View(model);
        }

        private async Task PopulateProducts()
        {
            var products = await _productService.GetPagedAsync(null, null, null, 1, 1000);
            ViewBag.Products = products.Items.Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = $"{p.ProductName} ({p.SKU})"
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