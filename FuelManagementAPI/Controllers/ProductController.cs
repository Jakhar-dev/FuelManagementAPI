using Microsoft.AspNetCore.Mvc;
using FuelManagementAPI.Models;
using FuelManagementAPI.ViewModels;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace FuelManagementAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepo;

        public ProductsController(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        // GET: api/products/categories
        [HttpGet("GetProducts")]
        public async Task<ActionResult<IEnumerable<string>>> GetProducts()
        {
            var products = await _productRepo.GetProductsAsync();
            var ProductName = products.Select(p => p.ProductName).Distinct().ToList();
            return Ok(ProductName);
        }     

        [HttpGet("by-category")]
        public async Task<IActionResult> GetProductsByCategory([FromQuery] int categoryId)
        {
            if (categoryId <= 0)
                return BadRequest("Valid categoryId is required.");

            var products = await _productRepo.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }
        // GET: api/products/fuel-products
        [HttpGet("fuel-products")]
        public async Task<IActionResult> GetFuelProducts()
        {
            var fuelProducts = await _productRepo.GetProductsByCategoryAsync(1);
            return Ok(fuelProducts);
        }

        // GET: api/products/lube-products
        [HttpGet("lube-products")]
        public async Task<IActionResult> GetLubeProducts()
        {
            var lubeProducts = await _productRepo.GetProductsByCategoryAsync(2);
            return Ok(lubeProducts);
        }

        // POST: api/products/add
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct([FromBody] List<ProductViewModel> productVms)
        {
            if (productVms == null || !productVms.Any())
                return BadRequest("No products provided.");

            var addedProducts = new List<Product>();
            var validationErrors = new List<string>();

            foreach (var vm in productVms)
            {
                if (string.IsNullOrWhiteSpace(vm.ProductName))
                {
                    validationErrors.Add($"Product name is required for category: {vm.ProductCategory}");
                    continue;
                }

                // Find category by name
                var category = await _productRepo.GetCategoryByNameAsync(vm.ProductCategory);

                if (category == null)
                {
                    validationErrors.Add($"Category '{vm.ProductCategory}' not found.");
                    continue;
                }


                var product = new Product
                {
                    ProductName = vm.ProductName,
                    ProductDescription = vm.ProductDescription,
                    ProductCategory = category,
                    Date = DateTime.UtcNow
                };

                try
                {
                    await _productRepo.AddProductAsync(product);
                    addedProducts.Add(product);
                }
                catch (Exception ex)
                {
                    validationErrors.Add($"Failed to add product '{vm.ProductName}': {ex.Message}");
                }
            }

            return Ok(new
            {
                products = addedProducts,
                errors = validationErrors
            });
        }


    }
}
