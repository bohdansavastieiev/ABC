namespace ABC.Domain.Abstractions;

public abstract class Entity
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}