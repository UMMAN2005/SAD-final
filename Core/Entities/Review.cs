namespace Core.Entities;

public class Review : BaseEntity
{
    public string AppUserId { get; set; } = default!;
    public AppUser AppUser { get; set; } = default!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = default!;
    public string Text { get; set; } = default!;
    public decimal Rating { get; set; }
}