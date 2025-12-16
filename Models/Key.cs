namespace SportMania.Models;

public class Key
{
    public Guid KeyId { get; set; }
    public string Code { get; set; } = string.Empty;
    public bool IsRedeemed { get; set; }
}