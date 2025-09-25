namespace ABC.Application.Products.Common;

public record ProductsDto
{
    public List<ProductDto> Products { get; set; } = [];

    public ProductsDto(List<ProductDto> products)
    {
        Products = products;
    }
}