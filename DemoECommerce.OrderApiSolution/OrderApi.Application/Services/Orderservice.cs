using Microsoft.AspNetCore.Http;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace OrderApi.Application.Services;

public class Orderservice(IOrder orderInterface,HttpClient httpClient, //IHttpContextAccessor _httpContextAccessor,
    ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
{
    public async Task<ProductDTO?> GetProductByIdAsync(int productId)
    {
        var getProduct = await httpClient.GetAsync($"/api/product/{productId}");
        if (!getProduct.IsSuccessStatusCode)
            return null;
        var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
        return product;
    }
    public async Task<AppUserDTO?> GetUserByIdAsync(int userId)
    {
        //var token = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToString();

        //if (!string.IsNullOrEmpty(token))
        //    httpClient.DefaultRequestHeaders.Authorization =
        //        new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
        var getUser = await httpClient.GetAsync($"/api/authentication/user/{userId}");
        if (!getUser.IsSuccessStatusCode)
            return null;
        var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
        return user;
    }
    public async Task<OrderDetailsDTO?> GetOrderDetailsByIdAsync(int orderId)
    {
        // Prepare order
        var order = await orderInterface.FindByIdAsync(orderId);
        if (order is null || order.Id <= 0)
            return null;

        // Get Retry pipeline
        var retryPipeline = resiliencePipeline.GetPipeline("my-retry-Pipeline");

        // Prepare product
        var product = await retryPipeline.ExecuteAsync(
            async token => await GetProductByIdAsync(order.ProductId));

        // Prepare Client
        var client = await retryPipeline.ExecuteAsync(
            async token => await GetUserByIdAsync(order.ClientId));
        // Prepare OrderDetailsDTO
        var orderDetails = new OrderDetailsDTO
        (
            OrderId: order.Id,
            ProductId: product?.Id ?? 0,
            ClientId: client?.Id ?? 0,
            ClientName: client?.Name ?? string.Empty,
            Email: client?.Email ?? string.Empty,
            Address: client?.Address ?? string.Empty,
            TelephoneNumber: client?.TelephoneNumber ?? string.Empty,
            ProductName: product?.Name ?? string.Empty,
            PurchaseQuantity: order.PurchaseQuantity,
            UnitPrice: product?.Price ?? 0,
            TotalPrice: (product?.Price ?? 0) * order.PurchaseQuantity,
            OrderedDate: order.OrderDate
        );
        return orderDetails;
    }

    public async Task<IEnumerable<OrderDTO>> GetOrdersByClientIdAsync(int clientId)
    {
        //Get orders by client id
        var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
        if (orders is null || !orders.Any())
            return [];

        // Convert to OrderDTO
        var(_, _orders) = OrderConversion.FromEntity(null, orders);
        return _orders ?? [];
    }
}
