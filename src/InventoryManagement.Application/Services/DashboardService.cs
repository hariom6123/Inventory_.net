using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Interfaces;

namespace InventoryManagement.Application.Services
{
    /// <summary>
    /// Provides aggregated dashboard metrics.
    /// </summary>
    public class DashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Builds a dashboard summary from the current state of the system.
        /// </summary>
        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var products = (await _unitOfWork.Products.GetAllAsync()).ToList();
            var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();

            var totalProducts = products.Count;
            var totalCategories = categories.Count;
            var lowStock = products.Count(p => p.Quantity > 0 && p.Quantity <= p.LowStockThreshold);
            var outOfStock = products.Count(p => p.Quantity <= 0);
            var totalValue = products.Sum(p => p.Price * p.Quantity);

            return new DashboardSummaryDto
            {
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                LowStockProducts = lowStock,
                OutOfStockProducts = outOfStock,
                TotalInventoryValue = Math.Round(totalValue, 2)
            };
        }
    }
}