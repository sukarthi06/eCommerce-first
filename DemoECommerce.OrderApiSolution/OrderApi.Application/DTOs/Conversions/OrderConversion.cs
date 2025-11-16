using OrderApi.Domain.Entities;

namespace OrderApi.Application.DTOs.Conversions;

public static class OrderConversion
{
    public static Order ToEntity(this OrderDTO orderDto)
    {
        return new Order
        {
            Id = orderDto.Id,
            ProductId = orderDto.ProductId,
            ClientId = orderDto.ClientId,
            PurchaseQuantity = orderDto.PurchaseQuantity,
            OrderDate = orderDto.OrderDate
        };
    }

    public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
    {
        // Single Order conversion
        if(order is not null || orders is null)
        {
            var orderDto = new OrderDTO
            (
                order!.Id,
                order.ProductId,
                order.ClientId,
                order.PurchaseQuantity,
                order.OrderDate
            );
            return (orderDto, null);
        }
        // Multiple Orders conversion
        else
        {
            var orderDtos = orders.Select(o => new OrderDTO
            (
                o.Id,
                o.ProductId,
                o.ClientId,
                o.PurchaseQuantity,
                o.OrderDate
            ));
            return (null, orderDtos);
        }
    }
}
