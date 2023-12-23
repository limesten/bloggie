public class User : BaseEntity
{
    public required string Name { get; set; }
    public string ApiKey { get; set; }
    public List<Feed> Feeds { get; set; }
}
