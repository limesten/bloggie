public class User : BaseEntity
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
