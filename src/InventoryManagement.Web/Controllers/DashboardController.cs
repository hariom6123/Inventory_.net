using System;
using System.Threading.Tasks;
using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Services;
using InventoryManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Web.Controllers
{
    /// <summary>
    /// Provides the dashboard view backed by aggregated metrics.
    /// </summary>
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly IInventoryService _inventoryService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardController"/> class.
        /// </summary>
        public DashboardController(
            DashboardService dashboardService,
            IInventoryService inventoryService,
            IMapper mapper)
        {
            _dashboardService = dashboardService;
            _inventoryService = inventoryService;
            _mapper = mapper;
        }

        /// <summary>
        /// Default GET — shows dashboard summary metrics and recent transactions.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var summary = await _dashboardService.GetSummaryAsync();
                var transactions = await _inventoryService.GetTransactionsAsync();

                var model = new DashboardViewModel
                {
                    Summary = summary,
                    RecentTransactions = _mapper.Map<System.Collections.Generic.IEnumerable<InventoryTransactionViewModel>>(transactions)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new DashboardViewModel());
            }
        }
    }
}