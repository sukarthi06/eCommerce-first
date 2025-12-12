using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Endpoints
{
    [AllowAnonymous]
    public static class ProductService
    {
        public static void MapProductEndpoints(this WebApplication app)
        {
            var productGroup = app.MapGroup("/api/product")
                .WithTags("Product Endpoints")
                .WithOpenApi();

            productGroup.MapGet("", Get)
                .WithSummary("Retrieves all products")
                .WithDescription("Fetches a list of all products from the repository.")
                .Produces<List<ProductDTO>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithOpenApi();
            productGroup.MapGet("{id:int}", GetById)
                .WithSummary("Retrieves a product by ID")
                .WithDescription("Fetches a single product based on the provided ID.")
                .Produces<ProductDTO>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithOpenApi();
            productGroup.MapPost("", Create)
                .WithSummary("Creates a new product")
                .WithDescription("Adds a new product to the repository.")
                .Accepts<ProductDTO>("application/json")
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status400BadRequest)
                .WithOpenApi();
            productGroup.MapPut("", Update)
                .WithSummary("Updates an existing product")
                .WithDescription("Modifies the details of an existing product in the repository.")
                .Accepts<ProductDTO>("application/json")
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status400BadRequest)
                .WithOpenApi();
            productGroup.MapPost("/delete", Delete)
                .WithSummary("Deletes a product")
                .WithDescription("Removes a product from the repository.")
                .Accepts<ProductDTO>("application/json")
                .Produces<Response>(StatusCodes.Status200OK)
                .Produces<Response>(StatusCodes.Status400BadRequest)
                .WithOpenApi();
        }

        private static async Task<IResult> Get(IProduct productRepo)
        {
            var products = await productRepo.GetAllAsync();            
            var lstProduct = products.Select(prod => new { prod?.Id, prod?.Name, prod?.Price, prod?.Quantity }).ToList();
            return lstProduct == null ? Results.NotFound("No products found!") : Results.Ok(lstProduct);
        }        

        private static async Task<IResult> GetById(IProduct productRepo, int id)
        {
            var product = await productRepo.FindByIdAsync(id);
            return product == null ? Results.NotFound($"Product with ID {id} not found!") 
                : Results.Ok(ProductConversion.FromEntity(product, null).Item1!);
        }
        [Authorize]
        private static async Task<IResult> Create(IProduct productRepo, ProductDTO productDto)
        {   
            var product = ProductConversion.ToEntity(productDto);
            var response = await productRepo.CreateAsync(product);
            return response.Flag ? Results.Ok(response) : Results.BadRequest(response);
        }
        [Authorize(Roles = "Admin")]
        private static async Task<IResult> Update(IProduct productRepo, ProductDTO productDto)
        {
            var product = ProductConversion.ToEntity(productDto);
            var response = await productRepo.UpdateAsync(product);
            return response.Flag ? Results.Ok(response) : Results.BadRequest(response);
        }
        [Authorize(Roles = "Admin")]
        private static async Task<IResult> Delete(IProduct productRepo, ProductDTO productDto)
        {
            var product = ProductConversion.ToEntity(productDto);
            var response = await productRepo.DeleteAsync(product);
            return response.Flag ? Results.Ok(response) : Results.BadRequest(response);
        }
    }
}
