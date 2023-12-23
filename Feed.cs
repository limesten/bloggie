public class Feed : BaseEntity
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public Guid UserId { get; set; }
    public required User User { get; set; }
}
