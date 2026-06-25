using AutoMapper;
using InventoryManagement.Application.DTOs;
using InventoryManagement.Domain.Entities;

namespace InventoryManagement.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile that defines mappings between domain entities and DTOs.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            // Product -> ProductDto
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.CategoryName : null));

            // ProductDto -> Product (skip computed CategoryName)
            CreateMap<ProductDto, Product>()
                .ForMember(d => d.Category, opt => opt.Ignore())
                .ForMember(d => d.InventoryTransactions, opt => opt.Ignore())
                .ForMember(d => d.UpdatedDate, opt => opt.Ignore());

            // Category -> CategoryDto
            CreateMap<Category, CategoryDto>()
                .ForMember(d => d.ProductCount, opt => opt.Ignore());

            // CategoryDto -> Category
            CreateMap<CategoryDto, Category>()
                .ForMember(d => d.Products, opt => opt.Ignore());

            // InventoryTransaction -> InventoryTransactionDto
            CreateMap<InventoryTransaction, InventoryTransactionDto>()
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.ProductName : null))
                .ForMember(d => d.ProductSKU, opt => opt.MapFrom(s => s.Product != null ? s.Product.SKU : null))
                .ForMember(d => d.Quantity, opt => opt.Ignore())
                .ForMember(d => d.QuantityChanged, opt => opt.MapFrom(s => s.QuantityChanged));

            // InventoryTransactionDto -> InventoryTransaction
            CreateMap<InventoryTransactionDto, InventoryTransaction>()
                .ForMember(d => d.Product, opt => opt.Ignore());
        }
    }
}