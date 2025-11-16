using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                //Check if the product with the same name already exists
                var existingProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (existingProduct != null)
                {
                    return new Response
                    {
                        Flag = false,
                        Message = $"A product with the same name - {entity.Name} already exists."
                    };
                }

                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currentEntity != null && currentEntity.Id > 0)
                {
                    return new Response
                    {
                        Flag = true,
                        Message = $"Product - {entity.Name} created successfully."
                    };
                }
                else
                {
                    return new Response
                    {
                        Flag = false,
                        Message = $"Failed to create the product - {entity.Name}."
                    };
                }
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //Display user friendly message to the user
                return new Response
                {
                    Flag = false,
                    Message = "An error occurred while creating the product."
                };
            }
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                var existingProduct = await FindByIdAsync(entity.Id);
                if (existingProduct == null)
                {
                    return new Response
                    {
                        Flag = false,
                        Message = $"Product with Id - {entity.Id} does not exist."
                    };
                }
                context.Products.Remove(existingProduct);
                await context.SaveChangesAsync();
                return new Response
                {
                    Flag = true,
                    Message = $"Product - {entity.Name} deleted successfully."
                };
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //Display user friendly message to the user
                return new Response
                {
                    Flag = false,
                    Message = $"An error occurred while deleting the product - {entity.Name}."
                };
            }
        }

        public async Task<Product?> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product ?? null;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);

                //Display user friendly message to the user
                throw new InvalidOperationException($"An error occurred while fetching the product by Id - {id}.");
            }
        }

        public async Task<IEnumerable<Product?>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display user friendly message to the user
                throw new InvalidOperationException("An error occurred while fetching all products.");
            }
        }

        public async Task<Product?> GetByAsync(Expression<Func<Product, bool>> filter)
        {
            try
            {
                var product = await context.Products.AsNoTracking().Where(filter).FirstOrDefaultAsync();
                return product ?? null;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display user friendly message to the user
                throw new InvalidOperationException("An error occurred while fetching the product.");
            }
        }

        public async Task<IEnumerable<Product?>> GetListAsync(Expression<Func<Product, bool>> filter)
        {
            try
            {
                var products = await context.Products.AsNoTracking().Where(filter).ToListAsync();
                return products;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display user friendly message to the user
                throw new InvalidOperationException("An error occurred while fetching the products.");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            try
            {
                var existingProduct = await FindByIdAsync(entity.Id);
                if (existingProduct == null)
                {
                    return new Response
                    {
                        Flag = false,
                        Message = $"Product with Name - {entity.Name} does not exist."
                    };
                }

                context.Entry(existingProduct).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response
                {
                    Flag = true,
                    Message = $"Product - {entity.Name} updated successfully."
                };
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptions(ex);
                //Display user friendly message to the user
                return new Response
                {
                    Flag = false,
                    Message = $"An error occurred while updating the product - {entity.Name}."
                };
            }
        }
    }
}
