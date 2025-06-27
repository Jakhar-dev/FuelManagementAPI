using FuelManagementAPI.Data;
using FuelManagementAPI.Models;
using FuelManagementAPI.Models.ViewModal;
using FuelManagementAPI.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace FuelManagementAPI.Repositories
{
    public class FuelPurchaseRepository : IFuelPurchaseRepository
    {
        private readonly FuelDbContext _context;

        public FuelPurchaseRepository(FuelDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FuelPurchase fuelPurchase)
        {
            await _context.FuelPurchase.AddAsync(fuelPurchase);
        }

        //public async Task UpdateCategoryPriceAsync(int categoryTypeId, float price)
        //{
        //    var products = await _context.Products
        //        .Where(p => p.CategoryTypeId == categoryTypeId)
        //        .ToListAsync();

        //    foreach (var prod in products)
        //    {
        //        prod.Price = price;
        //    }
        //}
    }
   
}
