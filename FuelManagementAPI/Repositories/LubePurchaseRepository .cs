using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{

    public class LubePurchaseRepository : ILubePurchaseRepository
    {
        private readonly FuelDbContext _context;

        public LubePurchaseRepository(FuelDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(LubePurchase lubePurchase)
        {
            await _context.LubePurchase.AddAsync(lubePurchase);
        }

        //public async Task UpdateProductPriceAsync(int productId, float price)
        //{
        //    var product = await _context.Products.FindAsync(productId);
        //    if (product != null)
        //    {
        //        product.Price = price;
        //    }
        //}
    }
}
