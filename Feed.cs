public class Feed : BaseEntity
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}

public class FeedRequest
{
    public string Name { get; set; }
    public string Url { get; set; }
}