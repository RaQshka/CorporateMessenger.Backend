﻿using Messenger.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Messenger.Persistence.Migrations;


public class MessengerDbContextFactory : IDesignTimeDbContextFactory<MessengerDbContext>
{
    /*private readonly string _connectionString;
    
    public MessengerDbContextFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }*/
    public MessengerDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MessengerDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CorporateMessengerDb;Trusted_Connection=True;"); // Убедитесь, что подключение корректное

        return new MessengerDbContext(optionsBuilder.Options);
    }
}
