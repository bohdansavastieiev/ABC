using ABC.Application.Feedbacks.Common;
using ABC.Domain.Entities;

namespace ABC.Application.Products.Common;

public static class ProductMapper
{
    public static ProductsDto ToDto(this IEnumerable<Product> products)
    {
        var productDtos = products.Select(product => new ProductDto(
            product.Id,
            product.Name,
            product.CreatedAt,
            product.UpdatedAt,
            new RatingDetailsDto(
                product.Rating.Id,
                product.Rating.Value,
                product.Rating.IsCalculated,
                product.Rating.IsOutdated,
                product.Rating.CreatedAt,
                product.Rating.UpdatedAt
            )
        )).ToList();
        
        return new ProductsDto(productDtos);
    }

    public static FeedbacksDto ToDtoWithFeedbacks(this Product product)
    {
        var feedbackDetails = product.Feedbacks
            .Select(feedback => new FeedbackDetailsDto(
                Id: feedback.Id,
                CreatedAt: feedback.CreatedAt,
                UpdatedAt: feedback.UpdatedAt,
                CustomerName: feedback.CustomerName,
                Text: feedback.Text))
            .ToList();

        return new FeedbacksDto(product.Id, feedbackDetails);
    }
}