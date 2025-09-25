    using ABC.Domain.Abstractions;

    namespace ABC.Domain.Entities;

    public class Product : Entity
    {
        public string Name { get; init; }
        public ProductRating Rating { get; init; } = null!;
        public ICollection<Feedback> Feedbacks { get; init; } = new List<Feedback>();
        
        public Product(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
            
            Name = name;
        }
    }