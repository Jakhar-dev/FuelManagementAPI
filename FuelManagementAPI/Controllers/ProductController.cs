using Microsoft.AspNetCore.Mvc;
using FuelManagementAPI.Models;
using FuelManagementAPI.ViewModels;
using FuelManagementAPI.Repositories.IRepositories;

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
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<string>>> GetCategories()
        {
            var products = await _productRepo.GetProductsAsync();
            var categories = products.Select(p => p.ProductCategory).Distinct().ToList();
            return Ok(categories);
        }
        // GET: api/products/by-category?category=Fuel
        [HttpGet("by-category")]
        public async Task<IActionResult> GetProductsByCategory([FromQuery] string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                return BadRequest("Category is required.");

            var products = await _productRepo.GetProductsByCategoryAsync(category);
            return Ok(products);
        }
        // GET: api/products/fuel-products
        [HttpGet("fuel-products")]
        public async Task<IActionResult> GetFuelProducts()
        {
            var fuelProducts = await _productRepo.GetProductsByCategoryAsync("Fuel");
            return Ok(fuelProducts);
        }

        // GET: api/products/lube-products
        [HttpGet("lube-products")]
        public async Task<IActionResult> GetLubeProducts()
        {
            var lubeProducts = await _productRepo.GetProductsByCategoryAsync("Lube");
            return Ok(lubeProducts);
        }

        // POST: api/products/add
        [HttpPost("add")]
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

                var product = new Product
                {
                    ProductCategory = vm.ProductCategory,
                    ProductName = vm.ProductName,
                    ProductDescription = vm.ProductDescription,
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

            return Ok(new { products = addedProducts, errors = validationErrors });
        }
    }
}
