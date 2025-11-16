using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Endpoints;

[Authorize]
public static class OrderServices
{
    public static void MapOrderServicesEndpoints(this WebApplication app)
    {   
        var orderGroup = app.MapGroup("/api/orders")
            .WithTags("Order Service Endpoints");

        orderGroup.MapGet("", GetOrdersAsync)
            .WithSummary("Get All Orders")
            .WithDescription("Retrieves all orders.")
            .Produces<IEnumerable<OrderDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        orderGroup.MapGet("/{orderId:int}", GetOrderAsync)
            .WithSummary("Get Order by Id")
            .WithDescription("Retrieves an order based on the provided Order Id.")
            .Produces<OrderDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        orderGroup.MapPost("", CreateOrderAsync)
            .WithSummary("Create a New Order")
            .WithDescription("Creates a new order with the provided order details.")
            .Accepts<OrderDTO>("application/json")
            .Produces<Response>(StatusCodes.Status200OK)
            .Produces<Response>(StatusCodes.Status400BadRequest);

        orderGroup.MapPut("", UpdateOrderAsync)
            .WithSummary("Update an Existing Order")
            .WithDescription("Updates an existing order with the provided order details.")
            .Accepts<OrderDTO>("application/json")
            .Produces<Response>(StatusCodes.Status200OK)
            .Produces<Response>(StatusCodes.Status400BadRequest);

        orderGroup.MapPost("/delete", DeleteOrderAsync)
            .WithSummary("Delete an Existing Order")
            .WithDescription("Deletes an existing order with the provided order details.")
            .Accepts<OrderDTO>("application/json")
            .Produces<Response>(StatusCodes.Status200OK)
            .Produces<Response>(StatusCodes.Status400BadRequest);

        orderGroup.MapGet("/details/{orderId:int}", GetOrderDetailsByIdAsync)
            .WithSummary("Get Order Details by Id")
            .WithDescription("Retrieves detailed information about an order based on the provided Order Id.")
            .Produces<OrderDetailsDTO>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        orderGroup.MapGet("/client/{clientId:int}", GetOrdersByClientIdAsync)
            .WithName("GetOrdersByClientId")
            .WithSummary("Get Orders by Client Id")
            .WithDescription("Retrieves all orders associated with the provided Client Id.")
            .Produces<IEnumerable<OrderDTO>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

    }

    private static async Task<IResult> GetOrderDetailsByIdAsync(int orderId, IOrderService orderService)
    {
        if (orderId <= 0)
            return Results.BadRequest("Invalid Order Id provided.");
        var orderDetails = await orderService.GetOrderDetailsByIdAsync(orderId);
        if (orderDetails is null || orderDetails.OrderId <= 0)
            return Results.NotFound($"Order with Id: {orderId} not found.");
        return Results.Ok(orderDetails);
    }

    private static async Task<IResult> GetOrdersByClientIdAsync(int clientId, IOrderService orderService)
    {
        if (clientId <= 0)
            return Results.BadRequest("Invalid Client Id provided.");
        var orders = await orderService.GetOrdersByClientIdAsync(clientId);
        if (orders is null || !orders.Any())
            return Results.NotFound($"No orders found for Client with Id: {clientId}.");
        return Results.Ok(orders);
    }

    private static async Task<IResult> GetOrdersAsync(IOrder orderInterface)
    {
        var orders = await orderInterface.GetAllAsync();
        if (orders is null || !orders.Any())
            return Results.NotFound("No orders found.");

        var (_, list) = OrderConversion.FromEntity(null, orders!);
        return Results.Ok(list);
    }

    private static async Task<IResult> GetOrderAsync(int orderId, IOrder orderInterface)
    {
        if (orderId <= 0)
            return Results.BadRequest("Invalid Order Id provided.");
        var order = await orderInterface.FindByIdAsync(orderId);
        if (order is null || order.Id <= 0)
            return Results.NotFound($"Order with Id: {orderId} not found.");
        var (orderDto, _) = OrderConversion.FromEntity(order, null);
        return Results.Ok(orderDto);
    }

    private static async Task<IResult> CreateOrderAsync(OrderDTO orderDto, IOrder orderInterface)
    {        
        var orderEntity = OrderConversion.ToEntity(orderDto);
        var response = await orderInterface.CreateAsync(orderEntity);
        return response.Flag 
            ? Results.Ok(response) 
            : Results.BadRequest(response);
    }

    private static async Task<IResult> UpdateOrderAsync(OrderDTO orderDTO, IOrder orderInterface)
    {
        var orderEntity = OrderConversion.ToEntity(orderDTO);
        var response = await orderInterface.UpdateAsync(orderEntity);
        return response.Flag 
            ? Results.Ok(response) 
            : Results.BadRequest(response);
    }
    private static async Task<IResult> DeleteOrderAsync(OrderDTO orderDTO, IOrder orderInterface)
    {
        var orderEntity = OrderConversion.ToEntity(orderDTO);
        var response = await orderInterface.DeleteAsync(orderEntity);
        return response.Flag
            ? Results.Ok(response)
            : Results.BadRequest(response);
    }

}
