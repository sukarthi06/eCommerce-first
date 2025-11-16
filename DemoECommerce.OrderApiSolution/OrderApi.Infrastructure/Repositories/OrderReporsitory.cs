using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderReporsitory(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();
                return order.Id > 0
                    ? new Response
                    (
                        Flag: true,
                        Message: "Order created successfully."
                    )
                    : new Response
                    (
                        Flag: false,
                        Message: "Failed to create order."
                    );
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response
                (
                    Flag: false,
                    Message:  "An error occurred while creating the order."
                );
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await context.Orders.FindAsync(entity.Id);
                if (order == null)
                {
                    return new Response
                    (
                        Flag: false,
                        Message: "Order not found."
                    );
                }
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                return new Response
                (
                    Flag: true,
                    Message: "Order deleted successfully."
                );
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response
                (
                    Flag: false,
                    Message: "An error occurred while deleting the order."
                );
            }
        }

        public async Task<Order?> FindByIdAsync(int id)
        {
            try
            {
                var order = await context.Orders.FindAsync(id);
                return order;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("An error occurred while retrieving the order.");
            }
        }

        public async Task<IEnumerable<Order?>> GetAllAsync()
        {
            try
            {
                var orders = await context.Orders.AsNoTracking().ToListAsync();
                return orders;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("An error occurred while retrieving orders.");
            }
        }

        public async Task<Order?> GetByAsync(Expression<Func<Order, bool>> filter)
        {
            try
            {
                var order = await context.Orders.Where(filter).AsNoTracking().FirstOrDefaultAsync(filter);
                return order;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("An error occurred while retrieving the order.");
            }
        }

        public async Task<IEnumerable<Order>?> GetOrdersAsync(Expression<Func<Order, bool>> filter)
        {
            try
            {
                var orders = await context.Orders.Where(filter).AsNoTracking().ToListAsync();
                return orders;
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                throw new Exception("An error occurred while retrieving orders.");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await context.Orders.FindAsync(entity.Id);
                if (order == null)
                {
                    return new Response
                    (
                        Flag: false,
                        Message: "Order not found."
                    );
                }
                
                context.Entry(order).State = EntityState.Detached;
                context.Orders.Update(entity);
                await context.SaveChangesAsync();
                return new Response
                (
                    Flag: true,
                    Message: "Order updated successfully."
                );

            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new Response
                (
                    Flag: false,
                    Message: "An error occurred while updating the order."
                );
            }
        }
    }
}
