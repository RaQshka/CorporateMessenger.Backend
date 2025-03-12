namespace Messenger.Persistence;

public class DbInitializer
{
    public static void Initialize(MessengerDbContext context) {
        context.Database.EnsureCreated(); 
    }
}

