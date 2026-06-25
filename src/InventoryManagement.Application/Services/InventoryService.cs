using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using InventoryManagement.Application.Common;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;
using InventoryManagement.Domain.Interfaces;

namespace InventoryManagement.Application.Services
{
    /// <summary>
    /// Default implementation of <see cref="IInventoryService"/>.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryService"/> class.
        /// </summary>
        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<InventoryTransactionDto> StockInAsync(int productId, int quantity, string? notes)
        {
            if (quantity <= 0)
            {
                throw new ValidationException(BuildError(nameof(quantity), "Quantity must be greater than zero."));
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new ValidationException(BuildError(nameof(productId), "Product not found."));
            }

            product.Quantity += quantity;
            product.UpdatedDate = DateTime.UtcNow;

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                QuantityChanged = quantity,
                TransactionType = TransactionType.StockIn,
                TransactionDate = DateTime.UtcNow,
                Notes = notes
            };

            await _unitOfWork.InventoryTransactions.AddAsync(transaction);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            transaction.Product = product;
            return MapTransaction(transaction);
        }

        /// <inheritdoc/>
        public async Task<InventoryTransactionDto> StockOutAsync(int productId, int quantity, string? notes)
        {
            if (quantity <= 0)
            {
                throw new ValidationException(BuildError(nameof(quantity), "Quantity must be greater than zero."));
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new ValidationException(BuildError(nameof(productId), "Product not found."));
            }

            if (product.Quantity - quantity < 0)
            {
                throw new ValidationException(BuildError(nameof(quantity),
                    $"Insufficient stock. Available: {product.Quantity}."));
            }

            product.Quantity -= quantity;
            product.UpdatedDate = DateTime.UtcNow;

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                QuantityChanged = -quantity,
                TransactionType = TransactionType.StockOut,
                TransactionDate = DateTime.UtcNow,
                Notes = notes
            };

            await _unitOfWork.InventoryTransactions.AddAsync(transaction);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            transaction.Product = product;
            return MapTransaction(transaction);
        }

        /// <inheritdoc/>
        public async Task<InventoryTransactionDto> AdjustAsync(int productId, int newQuantity, string? notes)
        {
            if (newQuantity < 0)
            {
                throw new ValidationException(BuildError(nameof(newQuantity), "Adjusted quantity cannot be negative."));
            }

            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new ValidationException(BuildError(nameof(productId), "Product not found."));
            }

            var delta = newQuantity - product.Quantity;
            product.Quantity = newQuantity;
            product.UpdatedDate = DateTime.UtcNow;

            var transaction = new InventoryTransaction
            {
                ProductId = productId,
                QuantityChanged = delta,
                TransactionType = TransactionType.Adjustment,
                TransactionDate = DateTime.UtcNow,
                Notes = notes
            };

            await _unitOfWork.InventoryTransactions.AddAsync(transaction);
            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            transaction.Product = product;
            return MapTransaction(transaction);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<InventoryTransactionDto>> GetTransactionsAsync(int? productId = null)
        {
            IEnumerable<InventoryTransaction> transactions;
            if (productId.HasValue)
            {
                transactions = await _unitOfWork.InventoryTransactions
                    .FindAsync(t => t.ProductId == productId.Value);
            }
            else
            {
                transactions = await _unitOfWork.InventoryTransactions.GetAllAsync();
            }

            var ordered = transactions
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            return ordered.Select(t => MapTransaction(t));
        }

        /// <inheritdoc/>
        public StockStatus GetStockStatus(int quantity, int lowStockThreshold)
        {
            if (quantity <= 0) return StockStatus.OutOfStock;
            if (quantity <= lowStockThreshold) return StockStatus.LowStock;
            return StockStatus.InStock;
        }

        private static ValidationResult BuildError(string field, string message)
        {
            var result = new ValidationResult();
            result.AddError(field, message);
            return result;
        }

        private InventoryTransactionDto MapTransaction(InventoryTransaction t)
        {
            var dto = _mapper.Map<InventoryTransactionDto>(t);
            dto.Quantity = Math.Abs(t.QuantityChanged);
            dto.QuantityChanged = t.QuantityChanged;
            if (t.Product != null)
            {
                dto.ProductName = t.Product.ProductName;
                dto.ProductSKU = t.Product.SKU;
            }
            return dto;
        }
    }
}