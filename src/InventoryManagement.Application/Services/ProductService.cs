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
    /// Default implementation of <see cref="IProductService"/>.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<PaginatedResultDto<ProductDto>> GetPagedAsync(
            string? searchTerm,
            int? categoryId,
            StockStatus? stockStatus,
            int pageNumber,
            int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var allProducts = await _unitOfWork.Products.GetAllAsync();
            var query = allProducts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLowerInvariant();
                query = query.Where(p =>
                    p.ProductName.ToLower().Contains(term) ||
                    p.SKU.ToLower().Contains(term));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (stockStatus.HasValue)
            {
                query = stockStatus.Value switch
                {
                    StockStatus.OutOfStock => query.Where(p => p.Quantity == 0),
                    StockStatus.LowStock => query.Where(p => p.Quantity > 0 && p.Quantity <= p.LowStockThreshold),
                    StockStatus.InStock => query.Where(p => p.Quantity > p.LowStockThreshold),
                    _ => query
                };
            }

            var totalCount = query.Count();
            var items = query
                .OrderBy(p => p.ProductName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = _mapper.Map<List<ProductDto>>(items);

            return new PaginatedResultDto<ProductDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        /// <inheritdoc/>
        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null) return null;

            var dto = _mapper.Map<ProductDto>(product);
            if (product.Category != null)
            {
                dto.CategoryName = product.Category.CategoryName;
            }
            return dto;
        }

        /// <inheritdoc/>
        public async Task<ProductDto> CreateAsync(ProductDto product)
        {
            var validation = ProductValidator.Validate(product);
            if (!validation.IsValid)
            {
                throw new ValidationException(validation);
            }

            if (await SkuExistsAsync(product.SKU))
            {
                validation.AddError(nameof(product.SKU), "A product with this SKU already exists.");
                throw new ValidationException(validation);
            }

            var entity = _mapper.Map<Product>(product);
            entity.ProductId = 0;
            entity.CreatedDate = DateTime.UtcNow;

            await _unitOfWork.Products.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductDto>(entity);
        }

        /// <inheritdoc/>
        public async Task<ProductDto> UpdateAsync(ProductDto product)
        {
            var validation = ProductValidator.Validate(product);
            if (!validation.IsValid)
            {
                throw new ValidationException(validation);
            }

            var existing = await _unitOfWork.Products.GetByIdAsync(product.ProductId);
            if (existing == null)
            {
                validation.AddError(nameof(product.ProductId), "Product not found.");
                throw new ValidationException(validation);
            }

            if (await SkuExistsAsync(product.SKU, product.ProductId))
            {
                validation.AddError(nameof(product.SKU), "A product with this SKU already exists.");
                throw new ValidationException(validation);
            }

            existing.ProductName = product.ProductName;
            existing.SKU = product.SKU;
            existing.Description = product.Description;
            existing.Price = product.Price;
            existing.Quantity = product.Quantity;
            existing.LowStockThreshold = product.LowStockThreshold;
            existing.CategoryId = product.CategoryId;
            existing.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.Products.Update(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductDto>(existing);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Products.GetByIdAsync(id);
            if (existing == null) return;

            _unitOfWork.Products.Remove(existing);
            await _unitOfWork.CompleteAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> SkuExistsAsync(string sku, int? excludeProductId = null)
        {
            if (string.IsNullOrWhiteSpace(sku)) return false;

            var all = await _unitOfWork.Products.GetAllAsync();
            return all.Any(p =>
                string.Equals(p.SKU, sku, StringComparison.OrdinalIgnoreCase) &&
                (!excludeProductId.HasValue || p.ProductId != excludeProductId.Value));
        }
    }
}