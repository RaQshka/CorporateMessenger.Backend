namespace Messenger.Domain;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid(); // 👈 Первичный ключ
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}