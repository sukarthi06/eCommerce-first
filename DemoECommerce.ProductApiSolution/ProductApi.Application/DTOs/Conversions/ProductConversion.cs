using ProductApi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Application.DTOs.Conversions
{
    public static class ProductConversion
    {
        public static Product ToEntity(this ProductDTO dto) =>
            new()
            {
                Id = dto.Id,
                Name = dto.Name,
                Price = dto.Price,
                Quantity = dto.Quantity
            };

        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(this Product? product, IEnumerable<Product?>? products) =>
            (product is null ? null : new ProductDTO(product.Id, product.Name, product.Quantity, product.Price),
             products?.Select(e => new ProductDTO(e.Id, e.Name, e.Quantity, e.Price)).ToList());
    }
}
