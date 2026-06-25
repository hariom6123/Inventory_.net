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
    /// Default implementation of <see cref="ICategoryService"/>.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var ordered = categories.OrderBy(c => c.CategoryName).ToList();
            var dtos = _mapper.Map<List<CategoryDto>>(ordered);

            // Populate product counts.
            var products = await _unitOfWork.Products.GetAllAsync();
            var counts = products.GroupBy(p => p.CategoryId)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var dto in dtos)
            {
                if (counts.TryGetValue(dto.CategoryId, out var count))
                {
                    dto.ProductCount = count;
                }
            }

            return dtos;
        }

        /// <inheritdoc/>
        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryDto>(category);
        }

        /// <inheritdoc/>
        public async Task<CategoryDto> CreateAsync(CategoryDto category)
        {
            var validation = CategoryValidator.Validate(category);
            if (!validation.IsValid)
            {
                throw new ValidationException(validation);
            }

            var entity = _mapper.Map<Category>(category);
            entity.CategoryId = 0;
            entity.CreatedDate = DateTime.UtcNow;

            await _unitOfWork.Categories.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CategoryDto>(entity);
        }

        /// <inheritdoc/>
        public async Task<CategoryDto> UpdateAsync(CategoryDto category)
        {
            var validation = CategoryValidator.Validate(category);
            if (!validation.IsValid)
            {
                throw new ValidationException(validation);
            }

            var existing = await _unitOfWork.Categories.GetByIdAsync(category.CategoryId);
            if (existing == null)
            {
                validation.AddError(nameof(category.CategoryId), "Category not found.");
                throw new ValidationException(validation);
            }

            existing.CategoryName = category.CategoryName;
            existing.Description = category.Description;

            _unitOfWork.Categories.Update(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CategoryDto>(existing);
        }

        /// <inheritdoc/>
        public async Task DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Categories.GetByIdAsync(id);
            if (existing == null) return;

            _unitOfWork.Categories.Remove(existing);
            await _unitOfWork.CompleteAsync();
        }
    }
}