using ECommerce_CommonLibrary.Interfaces;
using ECommerce_CommonLibrary.Logs;
using ECommerce_CommonLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                //check if the product is already exist
                var getProduct = await GetByAsync(p => p.Name!.Equals(entity.Name));
                if (getProduct != null && !string.IsNullOrEmpty(getProduct.Name))
                {
                    return new Response(false, $"{entity.Name} already exist");
                }

                var currEntity = context.products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currEntity != null && currEntity.Id > 0)
                {
                    return new Response(true, $"{entity.Name} added to database successfully");
                }
                else
                {
                    return new Response(false, "Error occured while adding new product");
                }
            }
            catch (Exception ex)
            {
                //log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                return new Response(false, "Error occurs adding new preodcut");
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product != null)
                {
                    context.products.Remove(product);
                    await context.SaveChangesAsync();   
                    return new Response(true, $"{entity.Name} is deleted successfully");
                }
                else
                {
                    return new Response(false, $"{entity.Name} not found");
                }
            }
            catch (Exception ex)
            {
                //log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                return new Response(false, "Error occurs deleting new preodcut");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.products.FindAsync(id);
                return product != null ? product : null!;
            }
            catch (Exception ex)
            {
                //log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                throw new Exception("Error Occured retrieving the product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.products.AsNoTracking().ToListAsync();
                return products is not null ? products : null!;
            }
            catch (Exception ex)
            {
                //log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                throw new InvalidOperationException("Error Occured retrieving the products");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var prduct = await context.products.Where(predicate).FirstOrDefaultAsync()!;
                return prduct != null ? prduct : null!;
            }
            catch (Exception ex)
            {
                //log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                throw new InvalidOperationException("Error Occured retrieving the product");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var product = await FindByIdAsync(entity.Id);
                if (product != null)
                {
                    context.Entry(product).State = EntityState.Detached;
                    context.products.Update(entity);
                    await context.SaveChangesAsync();
                    return new Response(true, $"{entity.Name} is updated successfully");
                }
                else
                {
                    return new Response(false, $"{entity.Name} not found");
                }
            }
            catch (Exception ex)
            {
                //log the original exception
                LogException.LogExceptions(ex);

                //display scary-free message to the client
                return new Response(false, "Error occurs updating exsiting prodcut");
            }
        }

    }
}
