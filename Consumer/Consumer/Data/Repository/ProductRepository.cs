using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sender.Models;

namespace Consumer.Data.Repository
{
    public class ProductRepository
    {
        private readonly AppDbContext _appDbContext;

        public ProductRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<EntityEntry<ProductModel>> Create(ProductModel model)
        {
           // model.Id = Guid.NewGuid().ToString();
            var res= await _appDbContext.Products.AddAsync(model);
            var k=await _appDbContext.SaveChangesAsync();
            var x = 0;
            return res;
        }

        public int CreateRange(IEnumerable<ProductModel> models)
        {
            //model.Id = Guid.NewGuid().ToString();
            _appDbContext.Products.AddRange(models);
            _appDbContext.SaveChanges();
            return models.Count();
        }

        public async Task<int> CreateRangeAsync(IEnumerable<ProductModel> models)
        {
            //model.Id = Guid.NewGuid().ToString();
            await _appDbContext.Products.AddRangeAsync(models);
            await _appDbContext.SaveChangesAsync();
            return models.Count();
        }

        public async Task<int>SaveChangesAsync()
        {
            return await _appDbContext.SaveChangesAsync();
        }
    }
}
