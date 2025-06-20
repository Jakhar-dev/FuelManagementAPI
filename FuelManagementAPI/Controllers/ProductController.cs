﻿using Microsoft.AspNetCore.Mvc;
using FuelManagementAPI.Models;
using FuelManagementAPI.ViewModels;
using FuelManagementAPI.Repositories.IRepositories;

namespace FuelManagementAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // GET: api/products/GetProducts
        [HttpGet("GetProducts")]
        public async Task<ActionResult<IEnumerable<string>>> GetProductsAsync()
        {
            var products = await _productRepository.GetProductsAsync();
            //var productNames = products
            //    .Select(p => p.ProductName.Trim())
            //    .Distinct()
            //    .ToList();

            return Ok(products);
        }

        // GET: api/products/by-category?categoryId=1
        [HttpGet("by-category")]
        public async Task<IActionResult> GetProductsByCategoryAsync([FromQuery] int categoryId)
        {
            if (categoryId <= 0)
                return BadRequest("A valid categoryId must be provided.");

            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        [HttpGet("getByCatgTypeId")]
        public async Task<IActionResult> GetFuelProductsAsync([FromQuery] int categoryTypeId)
        {
            if (categoryTypeId <= 0)
                return BadRequest("Invalid category type ID.");

            var fuelProducts = await _productRepository.GetProductsByCategoryTypeIdAsync(categoryTypeId);
            return Ok(fuelProducts);
        }


        // GET: api/products/fuel-products
        [HttpGet("fuel-products")]
        public async Task<IActionResult> GetFuelProductsAsync()
        {          
            var fuelProducts = await _productRepository.GetProductsByCategoryNameAsync("Fuel");
            return Ok(fuelProducts);
        }

        // GET: api/products/lube-products
        [HttpGet("lube-products")]
        public async Task<IActionResult> GetLubeProductsAsync()
        {
            var lubeProducts = await _productRepository.GetProductsByCategoryNameAsync("Lube");
            return Ok(lubeProducts);
        }

        // POST: api/products/add-product
        [HttpPost("add-product")]
        public async Task<IActionResult> AddProductsAsync([FromBody] List<ProductViewModel> productViewModels)
        {
            if (productViewModels == null || !productViewModels.Any())
                return BadRequest("No products provided.");

            var addedProducts = new List<Product>();
            var validationErrors = new List<string>();

            foreach (var vm in productViewModels)
            {
                if (string.IsNullOrWhiteSpace(vm.ProductName))
                {
                    validationErrors.Add($"Product name is required for category '{vm.ProductCategoryTypeId}'.");
                    continue;
                }

                var category = await _productRepository.GetCategoryByIdAsync(vm.ProductCategoryTypeId);
                if (category == null)
                {
                    validationErrors.Add($"Category '{vm.ProductCategoryTypeId}' not found.");
                    continue;
                }

                var productExists = await _productRepository.ProductExistsAsync(vm.ProductName.Trim());
                if (productExists)
                {
                    validationErrors.Add($"Product '{vm.ProductName}' already exists.");
                    continue;
                }

                var newProduct = new Product
                {
                    ProductName = vm.ProductName.Trim(),
                    ProductDescription = vm.ProductDescription?.Trim(),
                    CategoryTypeId = vm.ProductCategoryTypeId
                };

                try
                {
                    await _productRepository.AddProductAsync(newProduct);
                    addedProducts.Add(newProduct);
                }
                catch (Exception ex)
                {
                    validationErrors.Add($"Failed to add product '{vm.ProductName}': {ex.Message}");
                }
            }

            return Ok(new
            {
                Products = addedProducts,
                Errors = validationErrors
            });
        }

        [HttpGet("Sales-chart-by-product")]
        public async Task<IActionResult> GetSalesChartByProduct(
            [FromQuery] string range = "day",
            [FromQuery] int? categoryId = null,
            [FromQuery] int? productId = null)
                {
                  
                    var result = await _productRepository.GetSalesChartByProductAsync(range, categoryId, productId);
                    return Ok(result);
        }

    }
}
