using OrderApi.Application.DTOs;

namespace OrderApi.Application.Services;

public interface IOrderService
{
    Task<IEnumerable<OrderDTO>> GetOrdersByClientIdAsync(int clientId);
    Task<OrderDetailsDTO?> GetOrderDetailsByIdAsync(int orderId);
}
