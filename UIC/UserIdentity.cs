using System.Text.Json;

namespace UIC;

public class UserIdentity
{
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string? Email { get; set; }
    public List<string> Permissions { get; set; } = null!;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }

    public static void Validate(UserIdentity identity)
    {
        if (identity.UserId <= 0)
            throw new ArgumentException("User id must be > 0");

        ArgumentException.ThrowIfNullOrEmpty(identity.Username);

        if (identity.Permissions is not { Count: > 0 })
            throw new ArgumentException("Permissions mustn't be empty");
    }
}