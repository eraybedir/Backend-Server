using MarketAPI.Data;
using MarketAPI.Interfaces;
using MarketAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;

        public ProductService(IProductRepository productRepository, ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(ProductInputModel inputModel)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == inputModel.CategoryName.ToLower());

            if (category == null)
            {
                category = new Category { Name = inputModel.CategoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            var product = new Product
            {
                Name = inputModel.Name,
                Market = inputModel.Market,
                Price = inputModel.Price,
                CaloriesPer100g = inputModel.CaloriesPer100g,
                ProteinPer100g = inputModel.ProteinPer100g,
                CarbsPer100g = inputModel.CarbsPer100g,
                FatPer100g = inputModel.FatPer100g,
                CategoryId = category.Id,
                ImageUrl = inputModel.ImageUrl,
                CreatedAt = DateTime.Now
            };

            return await _productRepository.AddAsync(product);
        }

        public async Task<Product> UpdateProductAsync(int id, ProductInputModel inputModel)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                throw new KeyNotFoundException($"Product with ID {id} not found.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == inputModel.CategoryName.ToLower());

            if (category == null)
            {
                category = new Category { Name = inputModel.CategoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            product.Name = inputModel.Name;
            product.Market = inputModel.Market;
            product.Price = inputModel.Price;
            product.CaloriesPer100g = inputModel.CaloriesPer100g;
            product.ProteinPer100g = inputModel.ProteinPer100g;
            product.CarbsPer100g = inputModel.CarbsPer100g;
            product.FatPer100g = inputModel.FatPer100g;
            product.CategoryId = category.Id;
            product.ImageUrl = inputModel.ImageUrl;

            return await _productRepository.UpdateAsync(product);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _productRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<object>> GetProductsAsync()
        {
            var products = await _productRepository.GetAllWithCategoryAsync();

            return products.Select(p => new
            {
                p.Id,
                p.Name,
                p.Market,
                p.Price,
                p.CaloriesPer100g,
                p.ProteinPer100g,
                p.CarbsPer100g,
                p.FatPer100g,
                p.ImageUrl,
                Category = p.Category?.Name,
                p.CreatedAt
            });
        }

        public async Task<int> AddOrUpdateProductsAsync(List<ProductInputModel> products)
        {
            var productEntities = new List<Product>();

            foreach (var item in products)
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == item.CategoryName.ToLower());

                if (category == null)
                {
                    category = new Category { Name = item.CategoryName };
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();
                }

                var existingProduct = await _productRepository.GetByNameAndMarketAsync(item.Name, item.Market);

                if (existingProduct != null)
                {
                    existingProduct.ImageUrl = item.ImageUrl;
                    existingProduct.CaloriesPer100g = item.CaloriesPer100g;
                    existingProduct.ProteinPer100g = item.ProteinPer100g;
                    existingProduct.CarbsPer100g = item.CarbsPer100g;
                    existingProduct.FatPer100g = item.FatPer100g;
                    existingProduct.CategoryId = category.Id;

                    await _productRepository.UpdateAsync(existingProduct);
                    continue;
                }

                var newProduct = new Product
                {
                    Name = item.Name,
                    Market = item.Market,
                    Price = item.Price,
                    CaloriesPer100g = item.CaloriesPer100g,
                    ProteinPer100g = item.ProteinPer100g,
                    CarbsPer100g = item.CarbsPer100g,
                    FatPer100g = item.FatPer100g,
                    CategoryId = category.Id,
                    ImageUrl = item.ImageUrl,
                    CreatedAt = DateTime.Now
                };

                await _productRepository.AddAsync(newProduct);
            }

            return products.Count;
        }
    }
}
